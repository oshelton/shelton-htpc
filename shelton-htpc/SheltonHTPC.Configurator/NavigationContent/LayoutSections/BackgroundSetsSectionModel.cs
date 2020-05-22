using DynamicData;
using DynamicData.Binding;
using LiteDB;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent.LayoutSections
{
    public sealed class BackgroundSetsSectionModel : NavigationSectionModelBase<LayoutContentModel>
    {
        public BackgroundSetsSectionModel(LayoutContentModel parent) : base(parent) { }

        public override string Title { get; } = "Backgrounds";

        public override bool Scrollable => false;


        private ObservableCollectionExtended<EditableBackgroundSet> _BackgroundSets;
        /// <summary>
        /// Collection of currently present background sets.
        /// </summary>
        public ObservableCollectionExtended<EditableBackgroundSet> BackgroundSets
        {
            get => CheckIsOnMainThread(_BackgroundSets);
            private set => SetPropertyBackingValue(value, ref _BackgroundSets);
        }

        protected override async Task ActivateCore()
        {
            _BackgroundSetsSource = new SourceList<EditableBackgroundSet>();
            _DeletedBackgroundSets = new SourceCache<EditableBackgroundSet, Guid>(x => x.Id);

            BackgroundSets = new ObservableCollectionExtended<EditableBackgroundSet>();

            List<BackgroundSetDto> dtos = null;
            await Task.Run(() =>
            {
                dtos = LayoutDataManager.Repo.Query<BackgroundSetDto>(collectionName: BackgroundSetDto.CollectionName).ToList();

                _SourceBackgroundSetsObservable = _BackgroundSetsSource
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(_BackgroundSets)
                    .Subscribe();

                var sourceSetsObservable = _BackgroundSetsSource
                    .Connect()
                    .AutoRefresh(x => x.IsDirty)
                    .ToCollection()
                    .Select(x => x.Any(y => y.IsDirty))
                    .StartWith(false);

                var deletedSetsObservable = _DeletedBackgroundSets
                    .CountChanged
                    .Select(x => x != 0)
                    .StartWith(false);

                _SectionIsDirtyObservable = Observable.CombineLatest(sourceSetsObservable, deletedSetsObservable)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x => IsDirty = x.Any(y => y));
            }).ConfigureAwait(true);

            var sets = dtos.Select((x, index) => new EditableBackgroundSet(x, index)).ToList();
            foreach (var set in sets)
            {
                set.UpdateCanMove(sets.Count);
            }
            _BackgroundSetsSource.AddRange(sets);
        }

        protected override Task ParentNavigatesFromCore()
        {
            _SourceBackgroundSetsObservable.Dispose();
            _SectionIsDirtyObservable.Dispose();

            BackgroundSets = null;

            _BackgroundSetsSource.Dispose();
            _DeletedBackgroundSets.Dispose();

            return Task.CompletedTask;
        }

        public override async Task OnSaved()
        {

            var toUpdateOrInsert = _BackgroundSetsSource.Items.Where(x => x.IsDirty).ToArray();
            var toDelete = _DeletedBackgroundSets.Items.ToArray();

            await Task.Run(async () =>
            {
                foreach (var set in toUpdateOrInsert)
                {
                    await set.OnSave().ConfigureAwait(false);
                }

                foreach (var set in toDelete)
                {
                    await set.OnDelete().ConfigureAwait(false);
                }

                await DispatcherHelper.InvokeAsyncIfNecessary(() => _DeletedBackgroundSets.Clear()).ConfigureAwait(false);
            }).ConfigureAwait(true);
        }

        public void CreateBackgroundSet(object sender, EventArgs args)
        {
            var newSet = new EditableBackgroundSet(BackgroundSets.Count);

            _BackgroundSetsSource.Add(newSet);
            if (_BackgroundSetsSource.Count > 1)
            {
                _BackgroundSetsSource.Items.Skip(_BackgroundSetsSource.Count - 2).FirstOrDefault()?.UpdateCanMove(_BackgroundSetsSource.Count);
                _BackgroundSetsSource.Items.Last().UpdateCanMove(_BackgroundSetsSource.Count);
            }
            else
                newSet.IsNext = true;
        }

        public void MakeThisSetNext(object sender, EventArgs e)
        {
            if (sender is null)
                throw new ArgumentNullException(nameof(sender));

            var toMakeNext = ((FrameworkElement)sender).Tag as EditableBackgroundSet;
            foreach (var background in BackgroundSets)
            {
                background.IsNext = false;
            }
            toMakeNext.IsNext = true;

        }

        public void MoveUp(object sender, EventArgs e)
        {
            if (sender is null)
                throw new ArgumentNullException(nameof(sender));

            var toMove = ((FrameworkElement)sender).Tag as EditableBackgroundSet;
            MoveBackgroundSetUp(toMove);
        }

        public void MoveDown(object sender, EventArgs e)
        {
            if (sender is null)
                throw new ArgumentNullException(nameof(sender));

            var toMove = ((FrameworkElement)sender).Tag as EditableBackgroundSet;
            MoveBackgroundSetDown(toMove);
        }

        public async void DeleteThisSet(object sender, EventArgs e)
        {
            if (sender is null)
                throw new ArgumentNullException(nameof(sender));

            var toDelete = ((FrameworkElement)sender).Tag as EditableBackgroundSet;
            await DeleteBackgroundSet((FrameworkElement)sender, toDelete).ConfigureAwait(true);
        }

        private async Task DeleteBackgroundSet(DependencyObject sender, EditableBackgroundSet toDelete)
        {
            var window = Window.GetWindow(sender) as MetroWindow;
            MessageDialogResult result;
            if (!toDelete.IsNew)
                result = await window.ShowMessageAsync($"Confirm Deletion - {toDelete.Name}", "This will permanently delete this background set.  It can be restored by clicking the Reset button (along with any other changes made), but once the Save button is clicked it will be gone forever.", MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
            else
                result = await window.ShowMessageAsync($"Confirm Deletion - {toDelete.Name}", "This will permanently delete this background set.  This is a new set and has not been saved. Once deleted it will be gone forever.", MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);

            if (result == MessageDialogResult.Affirmative)
            {
                if (!toDelete.IsNew)
                    _DeletedBackgroundSets.AddOrUpdate(toDelete);

                _BackgroundSetsSource.Remove(toDelete);
                var toRemoveIndex = BackgroundSets.IndexOf(toDelete);
                for (int i = toRemoveIndex + 1; i < BackgroundSets.Count; ++i)
                {
                    --BackgroundSets[i].OrderIndex;
                    BackgroundSets[i].UpdateCanMove(_BackgroundSetsSource.Count);
                }

                if (toDelete.IsNext && BackgroundSets.Count != 0)
                    BackgroundSets[0].IsNext = true;
            }
        }

        private void MoveBackgroundSetUp(EditableBackgroundSet set)
        {
            int originalIndex = set.OrderIndex;
            EditableBackgroundSet otherSet = BackgroundSets[set.OrderIndex - 1];
            ++otherSet.OrderIndex;
            --set.OrderIndex;
            _BackgroundSetsSource.Move(originalIndex, originalIndex - 1);

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                set.UpdateCanMove(_BackgroundSetsSource.Count);
                otherSet.UpdateCanMove(_BackgroundSetsSource.Count);
            });
        }

        private void MoveBackgroundSetDown(EditableBackgroundSet set)
        {
            int originalIndex = set.OrderIndex;
            EditableBackgroundSet otherSet = BackgroundSets[set.OrderIndex + 1];
            --otherSet.OrderIndex;
            ++set.OrderIndex;
            _BackgroundSetsSource.Move(originalIndex, originalIndex + 1);

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                set.UpdateCanMove(_BackgroundSetsSource.Count);
                otherSet.UpdateCanMove(_BackgroundSetsSource.Count);
            });
        }

        private SourceList<EditableBackgroundSet> _BackgroundSetsSource;
        private SourceCache<EditableBackgroundSet, Guid> _DeletedBackgroundSets;

        private IDisposable _SourceBackgroundSetsObservable;
        private IDisposable _SectionIsDirtyObservable;
    }
}

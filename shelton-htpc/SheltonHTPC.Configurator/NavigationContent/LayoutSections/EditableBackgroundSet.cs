using DynamicData;
using DynamicData.Binding;
using LiteDB;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using ReactiveUI;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFAspects.Core;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent.LayoutSections
{
    public sealed class EditableBackgroundSet : Model
    {
        public const string PerSceneName = "Per Scene";
        public const string SpanningName = "Spanning (one image)";

        /// <summary>
        /// Collection of background set styles.
        /// </summary>
        public static readonly IEnumerable<string> BackgroundSetStyles = new[]
        {
            PerSceneName,
            SpanningName
        };

        public EditableBackgroundSet(BackgroundSetDto dto, int orderIndex)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            Id = dto.Id;
            Name = dto.Name;

            switch (dto.Style)
            {
                case BackgroundSetStyle.PER_SCENE:
                    Style = PerSceneName;
                    break;
                case BackgroundSetStyle.SPANNED:
                    Style = SpanningName;
                    break;
            }

            OrderIndex = orderIndex;

            _ImagesSource = new SourceList<EditableBackgroundSetImage>();
            _ImagesSource.AddRange(dto.Images.Select((imageDto, index) => new EditableBackgroundSetImage(imageDto, index)));

            Initialize();
        }

        public EditableBackgroundSet(int orderIndex)
        {
            Id = Guid.NewGuid();
            Name = "New Background Set";
            Style = PerSceneName;

            IsNew = true;
            OrderIndex = orderIndex;

            _ImagesSource = new SourceList<EditableBackgroundSetImage>();

            Initialize();
        }

        private void Initialize()
        {
            Images = new ObservableCollectionExtended<EditableBackgroundSetImage>();
            ImagesSourceObservable
                .Bind(Images)
                .Subscribe();

            this.WhenAnyValue(x => x.Style)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Skip(Style == SpanningName ? 1 : 0)
                .Do(x => CanAddImages = x == PerSceneName || (x == SpanningName && (Images?.Count ?? 0) == 0))
                .Where(x => x == SpanningName)
                .Subscribe(async x =>
                {
                    var window = Application.Current.MainWindow as MetroWindow;
                    await window.ShowMessageAsync($"Background Set changed to {SpanningName}", "All images but the first will be ignored on this background set; any extras will be deleted on save.", MessageDialogStyle.Affirmative).ConfigureAwait(true);
                });

            _DirtyTracker = new DirtyTracker(this);

            var dirtyTrackerObservable = this.WhenAnyValue(x => x._DirtyTracker.IsDirty)
                .StartWith(false);

            var isNewObservable = this.WhenAnyValue(x => x.IsNew)
                .StartWith(false);

            var imagesObservable = this.ImagesSourceObservable
                .AutoRefresh(x => x.IsDirty)
                .ToCollection()
                .Select(x => x.Any(y => y.IsDirty))
                .StartWith(false);

            Observable.CombineLatest(dirtyTrackerObservable, isNewObservable, imagesObservable)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => IsDirty = x.Any(y => y));

            this.WhenAnyValue(x => x._DirtyTracker.IsDirty, x => x.IsNew)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => IsDirty = x.Item1 || x.Item2);

            foreach (var image in Images)
            {
                image.UpdateCanMove(Images.Count);
            }
        }

        /// <summary>
        /// Id of the background set object.
        /// </summary>
        public Guid Id { get; private set; }

        private string _Name = null;
        /// <summary>
        /// Name of the background set.
        /// </summary>
        public string Name
        {
            get => CheckIsOnMainThread(_Name);
            set => SetPropertyBackingValue(value, ref _Name);
        }

        private string _Style = PerSceneName;
        /// <summary>
        /// Current style used by the BackgroundSet.
        /// </summary>
        public string Style
        {
            get => CheckIsOnMainThread(_Style);
            set
            {
                if (SetPropertyBackingValue(value, ref _Style))
                {
                    if (Images is object)
                    {
                        foreach (var image in Images.Skip(1))
                        {
                            image.UpdateWillBeRemoved(Style);
                        }
                    }
                }
            }
        }

        private bool _IsNext = false;
        public bool IsNext
        {
            get => CheckIsOnMainThread(_IsNext);
            set => SetPropertyBackingValue(value, ref _IsNext);
        }

        private ObservableCollectionExtended<EditableBackgroundSetImage> _Images;
        /// <summary>
        /// COllection of images for the background set.
        /// </summary>
        public ObservableCollectionExtended<EditableBackgroundSetImage> Images
        {
            get => CheckIsOnMainThread(_Images);
            private set => SetPropertyBackingValue(value, ref _Images);
        }

        public IObservable<IChangeSet<EditableBackgroundSetImage>> ImagesSourceObservable => _ImagesSource.Connect();

        private bool _CanMoveUp;
        /// <summary>
        /// Whether or not this background set can be moved up.
        /// </summary>
        public bool CanMoveUp
        {
            get => CheckIsOnMainThread(_CanMoveUp);
            private set => SetPropertyBackingValue(value, ref _CanMoveUp);
        }

        private bool _CanMoveDown;
        /// <summary>
        /// Whether or not the set can be moved down.
        /// </summary>
        public bool CanMoveDown
        {
            get => CheckIsOnMainThread(_CanMoveDown);
            private set => SetPropertyBackingValue(value, ref _CanMoveDown);
        }

        private bool _CanAddImages = false;
        /// <summary>
        /// Whether or not this background set can have images added to it.
        /// </summary>
        public bool CanAddImages
        {
            get => CheckIsOnMainThread(_CanAddImages);
            private set => SetPropertyBackingValue(value, ref _CanAddImages);
        }

        private bool _IsDirty = false;
        /// <summary>
        /// Whether or not this set is dirty.
        /// </summary>
        public bool IsDirty
        {
            get => CheckIsOnMainThread(_IsDirty);
            private set => SetPropertyBackingValue(value, ref _IsDirty);
        }

        private bool _IsNew = false;
        /// <summary>
        /// Whether or not this is a newly created Background set.
        /// </summary>
        public bool IsNew
        {
            get => CheckIsOnMainThread(_IsNew);
            set => SetPropertyBackingValue(value, ref _IsNew);
        }

        private int _OrderIndex = 0;
        /// <summary>
        /// Position of this Background set in the collection of sets.
        /// </summary>
        public int OrderIndex
        {
            get => CheckIsOnMainThread(_OrderIndex);
            set => SetPropertyBackingValue(value, ref _OrderIndex);
        }

        public override HashSet<string> DefaultTrackedProperties { get; } = new HashSet<string>()
        {
            nameof(OrderIndex),
            nameof(Name),
            nameof(Style),
            nameof(IsNext),
            nameof(AnyImagesRemoved),
        };

        public void AddImage(object sender, EventArgs e)
        {
            var imagePickerDialog = new OpenFileDialog();
            imagePickerDialog.Title = "Chose Image File";
            imagePickerDialog.CheckFileExists = true;
            imagePickerDialog.CheckPathExists = true;
            imagePickerDialog.Filter = "Image files (*.png;*.jpeg,*.jpg)|*.png;*.jpeg;*.jpg";

            if (imagePickerDialog.ShowDialog() ?? false)
            {
                var newImage = new EditableBackgroundSetImage(this.Images.Count);
                newImage.SourcePath = imagePickerDialog.FileName;
                this._ImagesSource.Add(newImage);

                foreach (var image in Images)
                {
                    image.UpdateCanMove(Images.Count);
                }
            }
        }

        public void DeleteImage(object sender, EventArgs e)
        {
            var view = sender as FrameworkElement;
            var toRemove = view.Tag as EditableBackgroundSetImage;

            _ImagesSource.Remove(toRemove);

            if (!toRemove.IsNew)
            {
                AnyImagesRemoved = true;
                _ImagesToRemove.Add(toRemove);
            }
        }

        public void MoveImageLeft(object sender, EventArgs e)
        {
            var view = sender as FrameworkElement;
            var toMove = view.Tag as EditableBackgroundSetImage;

            var originalIndex = toMove.OrderIndex;
            var otherImage = _Images[toMove.OrderIndex - 1];
            ++otherImage.OrderIndex;
            --toMove.OrderIndex;
            _ImagesSource.Move(originalIndex, originalIndex - 1);

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                toMove.UpdateCanMove(_ImagesSource.Count);
                otherImage.UpdateCanMove(_ImagesSource.Count);

                toMove.UpdateWillBeRemoved(Style);
                otherImage.UpdateWillBeRemoved(Style);
            });
        }

        public void MoveImageRight(object sender, EventArgs e)
        {
            var view = sender as FrameworkElement;
            var toMove = view.Tag as EditableBackgroundSetImage;

            var originalIndex = toMove.OrderIndex;
            var otherImage = _Images[toMove.OrderIndex + 1];
            --otherImage.OrderIndex;
            ++toMove.OrderIndex;
            _ImagesSource.Move(originalIndex, originalIndex + 1);

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                toMove.UpdateCanMove(_ImagesSource.Count);
                otherImage.UpdateCanMove(_ImagesSource.Count);

                toMove.UpdateWillBeRemoved(Style);
                otherImage.UpdateWillBeRemoved(Style);
            });
        }

        public void UpdateCanMove(int setCount)
        {
            UpdateCanMoveUp();
            UpdateCanMoveDown(setCount);
        }

        public async Task OnSave()
        {
            IList<EditableBackgroundSetImage> imagesToUpdate = null;
            IList<EditableBackgroundSetImage> imagesToRemove = null;
            await DispatcherHelper.InvokeAsyncIfNecessary(() =>
            {
                if (Style == EditableBackgroundSet.PerSceneName)
                {
                    imagesToUpdate = Images.Where(x => x.IsDirty).ToList();
                    imagesToRemove = _ImagesToRemove.ToList();
                }
                else
                {
                    imagesToUpdate = Images.Take(1).Where(x => x.IsDirty).ToList();
                    imagesToRemove = _ImagesToRemove.Union(Images.Skip(1)).ToList();
                }
            }).ConfigureAwait(false);

            foreach (var toUpdate in imagesToUpdate)
            {
                await toUpdate.CommitImagesToFileStorage().ConfigureAwait(false);
                await toUpdate.OnSaved().ConfigureAwait(false);
            }

            foreach (var toRemove in imagesToRemove)
            {
                toRemove.RemoveImagesFromStorage();
            }

            BackgroundSetDto dto = null;
            await DispatcherHelper.InvokeAsyncIfNecessary(() =>
            {
                if (Style == SpanningName && Images.Count > 1)
                {
                    _ImagesSource.RemoveRange(1, Images.Count - 1);
                    if (Images.Count != 0)
                        Images[0].UpdateCanMove(1);
                }

                dto = CreateDto();
            }).ConfigureAwait(false);

            LayoutDataManager.Repo.Upsert<BackgroundSetDto>(dto, BackgroundSetDto.CollectionName);

            await DispatcherHelper.InvokeAsyncIfNecessary(() =>
            {
                _DirtyTracker.SetInitialState();
                IsNew = false;
            }).ConfigureAwait(false);
        }

        public async Task OnDelete()
        {
            Guid id = Guid.Empty;
            IList<EditableBackgroundSetImage> imagesToRemove = null;
            await DispatcherHelper.InvokeAsyncIfNecessary(() =>
            {
                id = Id;
                imagesToRemove = _ImagesToRemove.Union(Images).ToList();
            }).ConfigureAwait(false);

            await Task.WhenAll(imagesToRemove.Select(x => Task.Run(() => x.RemoveImagesFromStorage()))).ConfigureAwait(false);

            LayoutDataManager.Repo.Delete<BackgroundSetDto>(new BsonValue(id), BackgroundSetDto.CollectionName);
        }

        private BackgroundSetDto CreateDto()
        {
            Dtos.Layout.BackgroundSetStyle styleToUse = BackgroundSetStyle.PER_SCENE;
            if (Style == SpanningName)
                styleToUse = BackgroundSetStyle.SPANNED;

            return new BackgroundSetDto()
            {
                Id = Id,
                Name = Name,
                Style = styleToUse,
                Images = Images.Select(x => x.CreateDto()).ToList()
            };
        }

        private bool _AnyImagesRemoved = false;
        private bool AnyImagesRemoved
        {
            get => CheckIsOnMainThread(_AnyImagesRemoved);
            set => SetPropertyBackingValue(value, ref _AnyImagesRemoved);
        }

        private void UpdateCanMoveUp()
        {
            CanMoveUp = OrderIndex > 0;
        }

        private void UpdateCanMoveDown(int setCount)
        {
            CanMoveDown = OrderIndex < setCount - 1;
        }

        private SourceList<EditableBackgroundSetImage> _ImagesSource;
        private List<EditableBackgroundSetImage> _ImagesToRemove = new List<EditableBackgroundSetImage>();

        private DirtyTracker _DirtyTracker;
    }
}

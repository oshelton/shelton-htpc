using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    /// <summary>
    /// Base class for tab models.  Not all tabs need a model of their own.
    /// </summary>
    public abstract class NavigationSectionModelBase<T> : Model where T: NavigationContentModelBase<T>
    {
        public NavigationSectionModelBase(T parent)
        {
            Parent = parent;
        }

        public T Parent { get; }

        /// <summary>
        /// Title of this section.
        /// </summary>
        public abstract string Title { get; }

        private bool _IsDirty = false;
        /// <summary>
        /// Whether or not this tab model has changes.
        /// </summary>
        public bool IsDirty
        {
            get => CheckIsOnMainThread(_IsDirty);
            protected set => SetPropertyBackingValue(value, ref _IsDirty);
        }

        private bool _IsWorking = false;
        /// <summary>
        /// Whether or not this tab is currently doing blocking work.
        /// </summary>
        public bool IsWorking
        {
            get => CheckIsOnMainThread(_IsWorking);
            set => SetPropertyBackingValue(value, ref _IsWorking);
        }

        public abstract bool Scrollable { get; }

        /// <summary>
        /// Called when the parent content model is initialized.
        /// Assume this method is ran from the background thread.
        /// </summary>
        public virtual Task Initialize(string dataPath) => Task.CompletedTask;

        /// <summary>
        /// Called when the tab is activated; its underlying code will only be executed once.
        /// </summary>
        public async Task OnActivated()
        {
            IsWorking = true;
            if (!_Activated)
                await ActivateCore().ConfigureAwait(true);

            _Activated = true;
            IsWorking = false;
        }

        /// <summary>
        /// Underlying logic only called once when a tab is activated.
        /// </summary>
        protected virtual Task ActivateCore() => Task.CompletedTask;

        /// <summary>
        /// Called when the parent main navigation item is changed to clean up stuff.
        /// </summary>
        public virtual async Task OnParentNavigatesFrom()
        {
            if (_Activated)
                await ParentNavigatesFromCore().ConfigureAwait(true);

            _Activated = false;
        }

        protected virtual Task ParentNavigatesFromCore() => Task.CompletedTask;

        /// <summary>
        /// Method called when the Save button is clicked for the parent content model.
        /// </summary>
        public virtual Task OnSaved() => Task.CompletedTask;

        public virtual async Task OnReset()
        {
            if (_Activated)
                await OnResetCore().ConfigureAwait(true);
        }

        /// <summary>
        /// Method called when the Reset button is clicked for the parent content model.
        /// </summary>
        public virtual async Task OnResetCore()
        {
            await ParentNavigatesFromCore().ConfigureAwait(true);
            await ActivateCore().ConfigureAwait(true);
        }

        private bool _Activated = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFAspects.Core;

namespace SheltonHTPC.Utils
{
    /// <summary>
    /// Model class representing ongoing task work.
    /// </summary>
    public class OngoingTaskModel : Model
    {
        /// <summary>
        /// Display states for the task.
        /// </summary>
        public enum ProgressDisplayKind
        {
            INDETERMINATE,
            DISCREET
        }

        public OngoingTaskModel(Action<OngoingTaskModel> taskToRun)
        {
            _TaskToRun = taskToRun ?? throw new ArgumentException($"{nameof(taskToRun)} cannot be null.");
        }

        public async void Start()
        {
            if (_HasRan)
                throw new InvalidOperationException("Task has already been started and cannot be started again.");

            _HasRan = true;
            Task = Task.Run(() => _TaskToRun(this));
            await Task.ConfigureAwait(true);

            Application.Current.Dispatcher.Invoke(() => TaskFinished?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler TaskFinished;

        private Task _Task = null;
        /// <summary>
        /// Underlying Task for the... task.
        /// </summary>
        public Task Task
        {
            get => CheckIsOnMainThread(_Task);
            set => SetPropertyBackingValue(value, ref _Task);
        }

        private string _Title = null;
        /// <summary>
        /// Title to be displayed for the task.
        /// </summary>
        public string Title
        {
            get => CheckIsOnMainThread(_Title);
            set => SetPropertyBackingValue(value, ref _Title);
        }

        private ProgressDisplayKind _DisplayStateKind = ProgressDisplayKind.INDETERMINATE;
        /// <summary>
        /// How the tasks' progress bar should be displayed.
        /// </summary>
        public ProgressDisplayKind DisplayStateKind
        {
            get => CheckIsOnMainThread(_DisplayStateKind);
            set => SetPropertyBackingValue(value, ref _DisplayStateKind);
        }

        private int _TotalDiscreetStepCount = 0;
        /// <summary>
        /// Total number of discreet steps in the task; will be ignored if DisplayStateKind is INDETERMINATE.
        /// </summary>
        public int TotalDiscreetStepCount
        {
            get => CheckIsOnMainThread(_TotalDiscreetStepCount);
            set => SetPropertyBackingValue(value, ref _TotalDiscreetStepCount);
        }

        private int _CurrentStepCount = 0;
        /// <summary>
        /// The current discreet step count; will be ignored if DisplayStateKind is INDETERMINATE.
        /// </summary>
        public int CurrentStepCount
        {
            get => CheckIsOnMainThread(_CurrentStepCount);
            set => SetPropertyBackingValue(value, ref _CurrentStepCount);
        }

        private string _CurrentProgressLabel = null;
        /// <summary>
        /// The label to be displayed for the current progress step; used for both DISCREET and INDETERMINATE kinds.
        /// </summary>
        public string CurrentStepLabel
        {
            get => CheckIsOnMainThread(_CurrentProgressLabel);
            set => SetPropertyBackingValue(value, ref _CurrentProgressLabel);
        }

        private readonly Action<OngoingTaskModel> _TaskToRun;
        private bool _HasRan = false;
    }
}

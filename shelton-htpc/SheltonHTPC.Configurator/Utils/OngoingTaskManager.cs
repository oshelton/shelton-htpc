using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WPFAspects.Core;

namespace SheltonHTPC.Utils
{
    /// <summary>
    /// Class for managing ongoing but non blocking tasks in the UI.
    /// </summary>
    public class OngoingTaskManager : Model
    {
        public OngoingTaskManager()
        {
            OngoingTasks = new ReadOnlyObservableCollection<OngoingTaskModel>(_BackingOngoingTasks);
        }

        private string _Title = NoTasksTitle; 
        /// <summary>
        /// Title to display in the UI.
        /// </summary>
        public string Title
        {
            get => CheckIsOnMainThread(_Title);
            set => SetPropertyBackingValue(value, ref _Title);
        }

        private readonly ObservableCollection<OngoingTaskModel> _BackingOngoingTasks = new ObservableCollection<OngoingTaskModel>();
        /// <summary>
        /// Collection of ongoing tasks.
        /// </summary>
        public ReadOnlyObservableCollection<OngoingTaskModel> OngoingTasks { get; private set; }

        /// <summary>
        /// Create and start a new ongoing task.
        /// </summary>
        public OngoingTaskModel CreateAndStartOngoingTask(string initialTaskTital, OngoingTaskModel.ProgressDisplayKind kind, Action<OngoingTaskModel> task, int totalDiscreetStepCount = 0)
        {
            if (Thread.CurrentThread != Dispatcher.CurrentDispatcher.Thread)
                throw new InvalidOperationException("Ongoing tasks cannot be created on any thread besides the main thread");

            var newTask = new OngoingTaskModel(task)
            {
                Title = initialTaskTital,
                DisplayStateKind = kind,
                TotalDiscreetStepCount = totalDiscreetStepCount
            };

            newTask.TaskFinished += TaskFinished;
            _BackingOngoingTasks.Add(newTask);

            Title = $"Ongoing Tasks - {_BackingOngoingTasks.Count}";

            newTask.Start();

            return newTask;
        }

        private void TaskFinished(object sender, EventArgs args)
        {
            var task = sender as OngoingTaskModel;
            task.TaskFinished -= TaskFinished;
            _BackingOngoingTasks.Remove(task);

            if (_BackingOngoingTasks.Count == 0)
                Title = NoTasksTitle;
            else
                Title = $"Ongoing Tasks - {_BackingOngoingTasks.Count}";
        }

        private static readonly string NoTasksTitle = "No Ongoing Tasks";
    }
}

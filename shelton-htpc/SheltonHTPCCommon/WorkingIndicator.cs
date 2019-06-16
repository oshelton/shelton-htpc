using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFAspects.Utils;

namespace SheltonHTPC.Common
{
    public class WorkingIndicator : Control
    {
        static WorkingIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkingIndicator), new FrameworkPropertyMetadata(typeof(WorkingIndicator)));
        }

        public static readonly string ProgressRingPartName = "PART_ProgressRing";
        public static readonly string FadeInStoryboardName = "FadeInStoryboard";
        public static readonly string FadeOutStoryboardName = "FadeOutStoryboard";

        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
            nameof(BackgroundOpacity), typeof(double), typeof(WorkingIndicator), new PropertyMetadata(0.6));

        /// <summary>
        /// Opacity of the background of the indicator; 1.0 for opaque and 0.0 for transparent.
        /// </summary>
        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        public static readonly DependencyProperty SpinnerWidthProperty = DependencyProperty.Register(
            nameof(SpinnerWidth), typeof(double), typeof(WorkingIndicator), new PropertyMetadata(48.0));

        /// <summary>
        /// Width of the spinner.
        /// </summary>
        public double SpinnerWidth
        {
            get => (double)GetValue(SpinnerWidthProperty);
            set => SetValue(SpinnerWidthProperty, value);
        }

        public static readonly DependencyProperty SpinnerHeightProperty = DependencyProperty.Register(
            nameof(SpinnerHeight), typeof(double), typeof(WorkingIndicator), new PropertyMetadata(48.0));

        /// <summary>
        /// Height of the progress indicator (the spinning part).
        /// </summary>
        public double SpinnerHeight
        {
            get => (double)GetValue(SpinnerHeightProperty);
            set => SetValue(SpinnerHeightProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(WorkingIndicator), new PropertyMetadata("Loading..."));

        /// <summary>
        /// Text to be displayed beneath the spinner; null will hide the text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextBackgroundProperty = DependencyProperty.Register(
            nameof(TextBackground), typeof(Brush), typeof(WorkingIndicator), null);

        /// <summary>
        /// Background brush to use for the container of the text; this container will be hidden if Text is null.
        /// </summary>
        public Brush TextBackground
        {
            get => (Brush)GetValue(TextBackgroundProperty);
            set => SetValue(TextBackgroundProperty, value);
        }

        public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register(
            nameof(IsWorking), typeof(bool), typeof(WorkingIndicator), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsWorkingChanged)));

        /// <summary>
        /// Whether or not work is currently in progress.
        /// </summary>
        public bool IsWorking
        {
            get => (bool)GetValue(IsWorkingProperty);
            set => SetValue(IsWorkingProperty, value);
        }

        public static readonly DependencyProperty ShowDelayProperty = DependencyProperty.Register(
            nameof(ShowDelay), typeof(TimeSpan), typeof(WorkingIndicator), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(500.0)));

        /// <summary>
        /// The amount of time that must pass before the indicator is actually shown when IsWorking becomes true.
        /// This is to prevent the indicator from appearing and disappearing almost instantly for very quick work.
        /// </summary>
        public TimeSpan ShowDelay
        {
            get => (TimeSpan)GetValue(ShowDelayProperty);
            set => SetValue(ShowDelayProperty, value);
        }

        public static readonly DependencyProperty FadeInTimeProperty = DependencyProperty.Register(
            nameof(FadeInTime), typeof(TimeSpan), typeof(WorkingIndicator), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(0.0)));

        /// <summary>
        /// The amount of time the indicator takes to fade in when work starts being done.
        /// </summary>
        public TimeSpan FadeInTime
        {
            get => (TimeSpan)GetValue(FadeInTimeProperty);
            set => SetValue(FadeInTimeProperty, value);
        }

        public static readonly DependencyProperty FadeOutTimeProperty = DependencyProperty.Register(
            nameof(FadeOutTime), typeof(TimeSpan), typeof(WorkingIndicator), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(1000.0)));

        /// <summary>
        /// The amount of time the indicator takes to fade in when work starts being done.
        /// </summary>
        public TimeSpan FadeOutTime
        {
            get => (TimeSpan)GetValue(FadeOutTimeProperty);
            set => SetValue(FadeOutTimeProperty, value);
        }

        public ProgressRing ProgressIndicator { get; private set; }

        public static void OnIsWorkingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WorkingIndicator indicator = sender as WorkingIndicator;
            if (indicator.IsWorking)
                indicator.StartWorking();
            else
                indicator.StopWorking();
        }

        public event EventHandler IndicatorFinished;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ProgressIndicator = VisualTreeWalker.FindChildrenOfType<ProgressRing>(this).First(d => d.Name == ProgressRingPartName);
            _FadeInStoryboard = this.FindResource(FadeInStoryboardName) as Storyboard;
            _FadeOutStoryboard = this.FindResource(FadeOutStoryboardName) as Storyboard;
        }

        private async void StartWorking()
        {
            using (WorkManager.StartScopedWork(() => _IsStarting = true, () => _IsStarting = false))
            {
                await Task.Delay(ShowDelay);

                if (!IsWorking)
                {
                    IndicatorFinished?.Invoke(this, new EventArgs());
                    return;
                }

                this.Visibility = Visibility.Visible;

                if (FadeInTime.TotalMilliseconds != 0)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (_FadeInStoryboard != null)
                            this.BeginStoryboard(_FadeInStoryboard);
                    });
                }
                else
                {
                    this.Opacity = 1.0;
                    await Dispatcher.InvokeAsync(() => ProgressIndicator.IsActive = true);
                }
            }

            if (!IsWorking)
                await Dispatcher.InvokeAsync(StopWorking);
        }

        private async void StopWorking()
        {
            if (!_IsStarting)
            {
                if (_FadeOutStoryboard != null && FadeOutTime.TotalMilliseconds != 0)
                {
                    this.BeginStoryboard(_FadeOutStoryboard);

                    await Task.Delay(FadeOutTime);
                }
                else
                {
                    ProgressIndicator.IsActive = false;
                    this.Visibility = Visibility.Collapsed;
                    this.Opacity = 0.0;
                }

                IndicatorFinished?.Invoke(this, new EventArgs());
            }
        }

        private Storyboard _FadeInStoryboard;
        private Storyboard _FadeOutStoryboard;
        private bool _IsStarting;
    }
}

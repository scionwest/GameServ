using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameServ
{
    public sealed class ServerTimer<T> : CancellationTokenSource, IDisposable
    {
        /// <summary>
        /// How many times we have fired the timer thus far.
        /// </summary>
        long fireCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineTimer{T}"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        public ServerTimer(T state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "EngineTimer constructor requires a non-null argument.");
            }

            this.StateData = state;
        }

        /// <summary>
        /// Gets the object that was provided to the timer when it was instanced.
        /// This object will be provided to the callback at each interval when fired.
        /// </summary>
        public T StateData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the engine timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// <para>
        /// Starts the timer, firing a synchronous callback at each interval specified until `numberOfFires` has been reached.
        /// If `numberOfFires` is 0, then the callback will be called indefinitely until the timer is manually stopped.
        /// </para>
        /// <para>
        /// The following example shows how to start a timer, providing it a callback.
        /// </para>
        /// @code
        /// var timer = new EngineTimer<IPlayer>(new DefaultPlayer());
        /// double startDelay = TimeSpan.FromSeconds(30).TotalMilliseconds;
        /// double interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
        /// int numberOfFires = 0;
        /// 
        /// timer.Start(
        ///     startDelay, 
        ///     interval, 
        ///     numberOfFires, 
        ///     (player, timer) => player.Save());
        /// @endcode
        /// </summary>
        /// <param name="startDelay">
        /// <para>
        /// The `startDelay` is used to specify how much time must pass before the timer can invoke the callback for the first time.
        /// If 0 is provided, then the callback will be invoked immediately upon starting the timer.
        /// </para>
        /// <para>
        /// The `startDelay` is measured in milliseconds.
        /// </para>
        /// </param>
        /// <param name="interval">The interval in milliseconds.</param>
        /// <param name="numberOfFires">Specifies the number of times to invoke the timer callback when the interval is reached. Set to 0 for infinite.</param>
        public void Start(double startDelay, double interval, int numberOfFires, Action<T, ServerTimer<T>> callback)
        {
            this.StartTimer(
                (task, state) => RunTimer(task, (Tuple<Action<T, ServerTimer<T>>, T>)state, interval, numberOfFires),
                Tuple.Create(callback, this.StateData),
                false,
                startDelay);
        }

        /// <summary>
        /// Starts the specified start delay.
        /// </summary>
        /// <param name="startDelay">The start delay in milliseconds.</param>
        /// <param name="interval">The interval in milliseconds.</param>
        /// <param name="numberOfFires">Specifies the number of times to invoke the timer callback when the interval is reached. Set to 0 for infinite.</param>
        public void StartAsync(double startDelay, double interval, int numberOfFires, Func<T, ServerTimer<T>, Task> callback)
        {
            this.StartTimer(
                (task, state) => RunTimerAsync(task, (Tuple<Func<T, ServerTimer<T>, Task>, T>)state, interval, numberOfFires),
                Tuple.Create(callback, this.StateData),
                true,
                startDelay);
        }

        /// <summary>
        /// Stops the timer for this instance.
        /// Stopping the timer will not dispose of the EngineTimer, allowing you to restart the timer if you need to.
        /// </summary>
        public void Stop()
        {
            if (!this.IsCancellationRequested)
            {
                this.Cancel();
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// Sets the timers current state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void SetState(T state)
        {
            this.StateData = state;
        }

        /// <summary>
        /// Stops the timer and releases the unmanaged resources used by the <see cref="T:System.Threading.CancellationTokenSource" /> class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stop();
            }

            base.Dispose(disposing);
        }

        void StartTimer(Func<Task, object, Task> timerDelegate, object data, bool isAsync, double startDelay)
        {
            this.IsRunning = true;

            Task
                .Delay(TimeSpan.FromMilliseconds(startDelay), this.Token)
                .ContinueWith(
                    timerDelegate,
                    data,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);
        }

        Task RunTimer(Task task, Tuple<Action<T, ServerTimer<T>>, T> state, double interval, int numberOfFires) => this.Run(
            task,
            () =>
            {
                state.Item1(state.Item2, this);
                return Task.FromResult(0);
            },
            interval,
            numberOfFires);

        Task RunTimerAsync(Task task, Tuple<Func<T, ServerTimer<T>, Task>, T> state, double interval, int numberOfFires) => this.Run(
            task,
            () => state.Item1(state.Item2, this),
            interval, numberOfFires);

        async Task Run(Task task, Func<Task> callback, double interval, int numberOfFires)
        {
            while (!this.IsCancellationRequested)
            {
                // Only increment if we are supposed to.
                if (numberOfFires > 0)
                {
                    this.fireCount++;
                }

                await callback();
                await PerformTimerCancellationCheck(interval, numberOfFires);
            }
        }

        async Task PerformTimerCancellationCheck(double interval, int numberOfFires)
        {
            // If we have reached our fire count, stop. If set to 0 then we fire until manually stopped.
            if (numberOfFires > 0 && this.fireCount >= numberOfFires)
            {
                this.Stop();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(interval), this.Token).ConfigureAwait(false);
        }
    }
}

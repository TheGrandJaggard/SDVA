using System;
using System.Collections;
using UnityEngine;

namespace SDVA.Utils
{
    /// <summary>
    /// A timer with configurable options. Supports pause/unpause and more.
    /// </summary>
    public class Timer
    {
        private bool timerRunning = false;
        private Coroutine timerCoroutine;
        private float seconds;
        private Action onFinish;
        private float currentTimeRemaining;

        /// <summary>
        /// The number of seconds that this timer should run for.
        /// </summary>
        public float LengthInSeconds { get => seconds;  set => seconds = value; }

        /// <summary>
        /// The action to be called when the timer finishes.
        /// </summary>
        public Action OnFinish { get => onFinish;  set => onFinish = value; }

        /// <summary>
        /// Whether this timer is currently running. Returns false if paused.
        /// </summary>
        public bool IsRunning { get => timerRunning; }

        /// <summary>
        /// The number of seconds left before this timer is done.
        /// </summary>
        public float TimeRemaining { get => currentTimeRemaining; }

        /// <summary>
        /// A timer with configurable options. Supports pause/unpause and more.
        /// </summary>
        /// <returns>A blank timer. Allows you to set variables and start timer manually.</returns>
        public Timer() {}

        /// <summary>
        /// A timer with configurable options. Supports pause/unpause and more.
        /// </summary>
        /// <param name="seconds">The number of seconds that this timer should run for.</param>
        /// <param name="onFinish">The action to be called when the timer finishes.</param>
        /// <returns>A running timer. Allows you create a timer and start it automatically.</returns>
        public Timer(float seconds, Action onFinish = null)
        {
            this.seconds = seconds;
            this.onFinish = onFinish;
            StartTimer();
        }

        public void StartTimer()
        {
            if (timerCoroutine != null) { StopTimer(); }
            timerCoroutine = CoroutineRunner.instance.Run(TimerEnumerator());
        }

        public void StopTimer()
        {
            CoroutineRunner.instance.Stop(timerCoroutine);
            timerCoroutine = null;
        }

        public void PauseTimer() => timerRunning = false;

        public void UnpauseTimer() => timerRunning = true;

        private IEnumerator TimerEnumerator()
        {
            timerRunning = true;

            currentTimeRemaining = seconds;
            while (currentTimeRemaining > 0f)
            {
                if (timerRunning)
                {
                    currentTimeRemaining -= Time.deltaTime;
                    yield return null;
                }
            }
            
            currentTimeRemaining = 0f;
            timerRunning = false;
            onFinish?.Invoke();
            timerCoroutine = null;
        }
    }
}

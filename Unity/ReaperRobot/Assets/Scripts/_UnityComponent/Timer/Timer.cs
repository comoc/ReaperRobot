using System;
using System.Diagnostics;
using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.Timer
{
    public class Timer : MonoBehaviour
    {
        private Stopwatch _stopWatch = new();

        [SerializeField] private float _time = 0f;

        public bool IsTimerRunning => _stopWatch.IsRunning;
        public float GetCurrentSeconds => (float)_stopWatch.Elapsed.TotalSeconds;
        public TimeSpan GetCurrentTimeSpan => _stopWatch.Elapsed;
        public void TimerStart() => _stopWatch.Start();
        public void TimerStop() => _stopWatch.Stop();
        public void TimerReset() => _stopWatch.Reset();
        public void TimerRestart() => _stopWatch.Restart();

        void Update()
        {
            //インスペクタービューに表示するための処理
            _time = GetCurrentSeconds;
        }
    }
}
using System.Collections;
using UnityEngine;
namespace Utility
{
    public class Timer
    {
        private float _timePeriod;
        private float _startTime;
        private float _endTime;
        public TimerState state
        {
            get
            {
                if (Time.time < _startTime)
                    return TimerState.WAITING;
                if (Time.time > _endTime)
                    return TimerState.FINISHED;
                return TimerState.COUNTING;
            }
        }
        public Timer(float delayTime = 0, float timePeriod = 0, MonoBehaviour caller = null, System.Action onStart = null, System.Action onEnd = null)
        {
            if (delayTime < 0)
                delayTime = 0;
            if (timePeriod < 0)
                timePeriod = 0;
            this._timePeriod = timePeriod;
            this._startTime = Time.time + delayTime;
            this._endTime = this._startTime + this._timePeriod;
            if (caller)
                caller.StartCoroutine(CountDownCoroutine(delayTime, timePeriod, onStart, onEnd));
            else if (onStart != null || onEnd != null)
            {
                Debug.LogWarning("未指定caller，因此不会触发回调。");
            }
        }
        IEnumerator CountDownCoroutine(float delayTime, float timePeriod, System.Action onStart, System.Action onEnd)
        {
            if (delayTime > 0)
                yield return new WaitForSeconds(delayTime);
            onStart?.Invoke();
            if (timePeriod > 0)
                yield return new WaitForSeconds(this._endTime - Time.time);
            onEnd?.Invoke();
        }
    }
}
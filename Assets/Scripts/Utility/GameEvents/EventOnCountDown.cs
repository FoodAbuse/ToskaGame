using UnityEngine;
using UnityEngine.Events;

namespace Utility.GameEvents
{
    public class EventOnCountDown : MonoBehaviour
    {
        [Tooltip("How long will this count down?")]
        public float countdownTime;
        private float _countDownProgress = 0.0f;
        public float CountDownProgress {get{return _countDownProgress;}}

        public bool resetTimerOnEnd = false;
    
        [Tooltip("optional. for calling a game event when the countdown ends")]
        public GameEvent gameEvent;
        [Tooltip("the Event called when the countdown ends")]
        public UnityEvent Response;
        [Tooltip("An event called when the countdown is stopped")]
        public UnityEvent ResponseFalse;
        private bool _isCountingDown = false;
        public bool IsCountingDown {get{return _isCountingDown;}}
        public void StartCountDown()
        {
            _isCountingDown = true; // here is where the coundown is started
        }

        public void StopCountDown()
        {
            _isCountingDown = false;
            ResponseFalse.Invoke();
            if (resetTimerOnEnd)
            {
                _countDownProgress = 0f;
            }
        }

        private void CountDownComplete()
        {
            _isCountingDown = false;
            Response.Invoke();
            if (resetTimerOnEnd)
            {
                _countDownProgress = 0f;
            }
        }

        public void Update()
        {
            if (_isCountingDown)
            {
                _countDownProgress += Time.deltaTime;
                if (_countDownProgress >= countdownTime)
                {
                    CountDownComplete();
                }
            }
        }
    
        // this will be a game event
    }
}

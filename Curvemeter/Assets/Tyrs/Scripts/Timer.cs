using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Tyrs { 
    public class Timer
    {
        private int _currentSecond;
        private UnityEvent<string> _changeValueTimer = new UnityEvent<string>();
        private ICoroutineRunner _coroutineRunner;
        private UnityAction<string> _eventChangeViewTimer;
        private Coroutine _tickCoroutine;

        public Timer(ICoroutineRunner coroutineRunner, UnityAction<string> eventChangeViewTimer)
        {
            _coroutineRunner = coroutineRunner;
            _eventChangeViewTimer = eventChangeViewTimer;
        }

        public void Launch() {
            _changeValueTimer.AddListener(_eventChangeViewTimer);
            if(_tickCoroutine != null)
                _coroutineRunner.StopCoroutine(_tickCoroutine);
            _currentSecond = 0;
            _changeValueTimer?.Invoke("00:00");            
            _tickCoroutine = _coroutineRunner.StartCoroutine(Tick());
        }

        public void Stop() {           
            if(_tickCoroutine != null)
                _coroutineRunner.StopCoroutine(_tickCoroutine);
             _currentSecond = 0;
            _changeValueTimer.RemoveListener(_eventChangeViewTimer);
        }
                
        private IEnumerator Tick() { 
            while (true) {
                yield return new WaitForSeconds(1);
                _currentSecond += 1;                
                int minutes = _currentSecond / 60;
                int seconds = _currentSecond % 60;
                string minutesText = minutes >= 10 ? minutes.ToString() : "0" + minutes;
                string secondsText = seconds >= 10 ? seconds.ToString() : "0" + seconds;             
                _changeValueTimer?.Invoke(minutesText + ":" + secondsText);
            }
        }
    }
}


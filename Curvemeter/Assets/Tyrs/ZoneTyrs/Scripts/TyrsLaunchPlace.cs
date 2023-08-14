using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Tyrs.ZoneTyrs
{
    /// <summary>
    /// Компонент отвечает за проверку нахождения в зоне тира
    /// Описание публичного API
    /// Имеет поля(сериализованы для доступа в инспекторе):
    ///     _numberSecondsCountdown - колличество секунд до полного выхода из зоны 
    ///Имеет публичные свойства
    ///     Entered - событие пользователь вошел в зону
    ///     Entered - событие пользователь покинул зону (после отсчета о покидании зоны)
    ///     Activated - зона активирована для определения какие зоны активированы а какие нет в параметре передает зону коорая активирована
    /// </summary>
    public class TyrsLaunchPlace : MonoBehaviour
    {
        [Header("Колличество секунд до полного выхода из зоны")]
        [SerializeField] private int _numberSecondsCountdown;
        private WarningPanelAboutExit _warningPanelAboutExit;
        private Coroutine _exitTimeCountdown;
        private int _remainingSeconds;
        private UnityEvent _exited = new UnityEvent();
        private UnityEvent _entered = new UnityEvent();
        private UnityEvent<TyrsLaunchPlace> _activated = new UnityEvent<TyrsLaunchPlace>();
        public UnityEvent Entered { get => _entered; }
        public UnityEvent Exited { get => _exited; }
        public UnityEvent<TyrsLaunchPlace> Activated { get => _activated; }

        private void Awake()
        {
            _warningPanelAboutExit = FindObjectOfType<WarningPanelAboutExit>(true);
            if (_warningPanelAboutExit == null) Debug.LogError("Компонент WarningPanelAboutExit отсутствует на сцене" );
        }

        public void Show() => this.gameObject.SetActive(true);

        public void Hide() => this.gameObject.SetActive(false);

        private void OnTriggerEnter(Collider other) {
            if (other.name == "PlayerController") PlayerEnter();                        
        }

        private void OnTriggerExit(Collider other) {
            if (other.name == "PlayerController") PlayerExit();
        }

        private void PlayerEnter()
        {
            _activated?.Invoke(this);
            _warningPanelAboutExit.Hide();
            _entered?.Invoke();
            if(_exitTimeCountdown != null)
                StopCoroutine(_exitTimeCountdown);
        }

        private void PlayerExit() => _exitTimeCountdown = StartCoroutine(ExitTimeCountdown());

        private IEnumerator ExitTimeCountdown() {
            _remainingSeconds = _numberSecondsCountdown;
            _warningPanelAboutExit.Show();
            while(_remainingSeconds > 0){
                _warningPanelAboutExit.UpdateRemaningTime(Convert.ToString(_remainingSeconds));
                yield return new WaitForSeconds(1);                
                _remainingSeconds -= 1;
            }
            _warningPanelAboutExit.Hide();
            _exited?.Invoke();
        }
    }
}



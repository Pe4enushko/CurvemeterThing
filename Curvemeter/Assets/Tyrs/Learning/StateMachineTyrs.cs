using LearningSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tyrs.Learning
{
    /// <summary>
    /// Класс отвечающий за пошаговое обучение в тире
    /// Описание публичного API
    /// Имеет поля(сериализованы для доступа в инспекторе):
    ///     _steps - массив типа Step которые заполняются на сцен. 
    ///     Шаги происходят последовательно друг за другом в соотвествии с заданном порядке в массиве
    ///     В тип Т передаем тот тип перечислений который нам нужен   
    ///     _finalAudioClip - Финальный аудиоклип который играет после прохождения последнего шага
    ///     _audioSource - Источник звука для подсказок установить ссылку на сцене
    /// Публичные поля
    ///     _finalEvent - События которые происходят после прохождения всех шагов
    /// </summary>
    public class StateMachineTyrs : MonoBehaviour, ICoroutineRunner
    {
        [Header("Заполните список шагов по порядку:")]
        [SerializeField] private Step<TyrsEvents, TyrsErrors>[] _steps;
        [Header("Финальная фраза")]
        [SerializeField] private AudioClip _finalAudioClip;
        [Header("Финальное событие")]
        public UnityEvent finalEvent;
        [Header("Событие когда обучение закончено с ошибкой")]
        public UnityEvent finalErrorEvent;
        [Header("Установить источник звука для подсказок со цены")]
        [SerializeField] private AudioSource _audioSource;
        private IEnumerator<Step<TyrsEvents, TyrsErrors>> _enumerator;
        private Step<TyrsEvents, TyrsErrors> _currentSteps;

        public Step<TyrsEvents, TyrsErrors>[] Steps { get => _steps; }
        private void Awake()
        {
            if (_audioSource == null) Debug.LogError("Не установлен источник звука для подсказок в " + gameObject.name);
            foreach (var step in _steps) { 
                step.Initialization(_audioSource, this, MoveStep);
                step.errorMade.AddListener(StopWithError);
            }

            IEnumerable<Step<TyrsEvents, TyrsErrors>> enumerable = _steps;
            _enumerator = enumerable.GetEnumerator(); 
        }

        public void Launch()
        {            
            _enumerator.Reset();
            MoveStep();
        }

        public void Stop()
        {
            Debug.Log("Стейт машина остановлена на шаге ");
            if(_currentSteps!= null)
                _currentSteps.Stop();
            _enumerator.Reset();
            _currentSteps = null;
        }

        private void MoveStep() {
            if (_currentSteps != null) { 
                if (_currentSteps == _steps[_steps.Length - 1]) {
                    _audioSource.clip = _finalAudioClip;
                    _audioSource.Play();
                    finalEvent?.Invoke();
                    return;
                }
            }            
            _enumerator.MoveNext();
            _currentSteps = _enumerator.Current;
            _currentSteps.Enter();            
        }

        private void StopWithError() {
            Stop();
            finalErrorEvent?.Invoke();
        }
    }
}


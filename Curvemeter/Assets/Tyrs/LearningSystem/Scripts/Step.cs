using System;
using System.Collections;
using System.Collections.Generic;
using Tyrs;
using UnityEngine;
using UnityEngine.Events;

namespace LearningSystem
{
    /// <summary>
    /// Класс отвечает за соответсвующий шаг при обучении
    /// Описание публичного API
    /// Имеет поля(сериализованы для доступа в инспекторе):
    ///     _name - имя для шага (не участвует в логике, нужно лишь для удобства чтения и понимания контекста при заполнении на поле и для отладки)
    ///     _audioPrompt - аудио подсказка которая проигрывается при старте этого шага (при переходе к этому шагу)
    ///     _nameExitEvent - название события (из доступных) для выхода из шага и перехода к следующему из доступного списка 
    ///     _delayReplayCountSec - через какое количество времени производим повтор аудиоподсказки  
    ///     _isReplayAudioPrompt - нужен ли аудиоповтор (в тестах обычно не нужен, по умолчанию стоит true)
    ///     StepPassed - шаг пройден, для подписи на событие о прохождении шага
    /// Имеет методы:
    ///     Initialization - передаем все необходимые компоненты в шаг (инициализируем) перед использованием 4 секунды по умолчанию
    ///     Enter - запускаем при старте шага
    /// Имеет события:
    ///     endingAudioPrompt - сюда можно установить события которые будут происходить при окончании проигрывания подсказки
    ///     errorMade - сюда можно установить события которые будут происходить при допущении ошибки пользователем в данном шаге
    /// </summary>
    [Serializable]
    public class Step<ExitEvents, ErrorEvents> {
        [Header("Имя шага")]
        [SerializeField] private string _name;
        [Header("Установите ссылку на Аудио подсказку")]
        [SerializeField] private AudioClip _audioPrompt;
        [Header("Выберите событие выхода из шага")]
        [SerializeField] private ExitEvents _nameExitEvent;
        [Header("Выберите события являющиеся ошибками для шага")]
        [SerializeField] private List<ErrorEvents> _eventsErrors;
        [Header("Установите ссылку на вторую Аудио подсказку(для повторения)")]
        [SerializeField] private AudioClip _audioPromptAdditional;
        [Header("Нужен ли аудиоповтор?")]
        [SerializeField] private bool _isReplayAudioPrompt = true;
        [Header("Через какое время происходит повтор аудиоподсказки(сек)")]
        [SerializeField] private int _delayReplayCountSec = 4;
        [Header("События происходят при начале шага")]
        public UnityEvent startStepEvent = new UnityEvent();  
        [Header("События происходят при конце шага")]
        public UnityEvent endStepEvent = new UnityEvent();  
        [Header("События происходят при окончании аудио подсказки")]
        public UnityEvent endingAudioPrompt = new UnityEvent(); 
        [Header("События происходят при совершении ошибки")]
        public UnityEvent errorMade = new UnityEvent();        

        private Action _exitAction;
        private ICoroutineRunner _coroutineRunner;
        private AudioSource _audioSource;
        private Coroutine _audioClipPlayed;
        private Coroutine _replayWaiting;

        public ExitEvents NameExitEvent { get => _nameExitEvent; }

        public void Initialization(AudioSource audioSource, ICoroutineRunner coroutineRunner, Action exitAction) { 
            _exitAction = exitAction;
            _audioSource = audioSource;
            _coroutineRunner = coroutineRunner;
        }
        public void Stop() {
            Debug.Log("Шаг остановлен ");
            if(_audioClipPlayed != null) _coroutineRunner.StopCoroutine(_audioClipPlayed);
            if(_replayWaiting != null) _coroutineRunner.StopCoroutine(_replayWaiting);
            LearningEventManager<ExitEvents>.Instance.GetEvent(_nameExitEvent).RemoveAllListeners();
             _eventsErrors?.ForEach(eventError => LearningEventManager<ErrorEvents>.Instance.GetEvent(eventError).RemoveListener(Error));
        }

        public void Enter() {
            PlayAudioPrompt(_audioPrompt);            
            Debug.Log("Начал шаг обучения: " + _name);
            LearningEventManager<ExitEvents>.Instance.GetEvent(_nameExitEvent).AddListener(Exit);            
            _eventsErrors?.ForEach(eventError => LearningEventManager<ErrorEvents>.Instance.GetEvent(eventError).AddListener(Error));
            startStepEvent?.Invoke();
        }

        private void Exit() {
            endStepEvent?.Invoke();            
            if(_audioClipPlayed != null) _coroutineRunner.StopCoroutine(_audioClipPlayed);
            if(_replayWaiting != null) _coroutineRunner.StopCoroutine(_replayWaiting);
            LearningEventManager<ExitEvents>.Instance.GetEvent(_nameExitEvent).RemoveAllListeners();
            _eventsErrors?.ForEach(eventError => LearningEventManager<ErrorEvents>.Instance.GetEvent(eventError).RemoveListener(Error));
            _exitAction?.Invoke();
        }

        private void Error() {
            Debug.Log("Совершена ошибка в: " + _nameExitEvent.ToString());
            errorMade?.Invoke();
        }

        private IEnumerator AudioClipPlayed(float sec) {
            yield return new WaitForSeconds(sec);
            endingAudioPrompt?.Invoke();
            if(_isReplayAudioPrompt) _replayWaiting = _coroutineRunner.StartCoroutine(ReplayWaiting(_delayReplayCountSec));
        }

        private IEnumerator ReplayWaiting(float sec) {
            yield return new WaitForSeconds(sec);
            PlayAudioPrompt(_audioPromptAdditional);
        }

        private void PlayAudioPrompt(AudioClip audioPrompt) { 
            _audioSource.clip = audioPrompt;
            _audioSource.Play();
            _audioClipPlayed = _coroutineRunner.StartCoroutine(AudioClipPlayed(audioPrompt.length));
        }        
    }
}


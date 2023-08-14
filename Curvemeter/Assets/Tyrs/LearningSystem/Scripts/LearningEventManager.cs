using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace LearningSystem
{
    /// <summary>
    /// Класс синглтон отвечает за сопоставление названий событий 
    /// Описание публичного API
    /// Имеет свойства:    
    ///     Instance - экземпляр синглтона
    /// Имеет методы:
    ///     TriggerEvent(T) - запуск события вызываем там где это событие должно произойти, в праметры передем тот тип с которым создали этот менеджер
    ///     GetEvent(T) - получить событие, вызываем в обучающем шаге для подписи на это событие, в праметры передем тот тип с которым создали этот менеджер
    /// </summary>
    public class LearningEventManager<T>{ 
        private Dictionary<T, UnityEvent> _dictionaryEvents = new Dictionary<T, UnityEvent>();
        private static LearningEventManager<T> _instance;        
        public static LearningEventManager<T> Instance { get => _instance = _instance == null? new LearningEventManager<T>():_instance; }       

        public LearningEventManager()
        {            
            foreach (var learningEvent in Enum.GetValues(typeof(T)))                
                _dictionaryEvents.Add((T)learningEvent, new UnityEvent());                 
        }

        public void TriggerEvent(T nameEvent) => _dictionaryEvents[nameEvent]?.Invoke();
        public UnityEvent GetEvent(T nameEvent) => _dictionaryEvents[nameEvent];
    }
}


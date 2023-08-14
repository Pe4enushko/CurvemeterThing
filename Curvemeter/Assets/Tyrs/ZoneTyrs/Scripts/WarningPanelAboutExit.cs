using System;
using TMPro;
using UnityEngine;

namespace Tyrs.ZoneTyrs
{
    /// <summary>
    /// Компонент отвечает за отображение панели предупреждения о выходе
    /// </summary>
    public class WarningPanelAboutExit : MonoBehaviour{

        [Header("Ссылка на текст с числом оставшегося времени до выхода")]
        [SerializeField] private TextMeshProUGUI _remaningTime;
        public void Show() => gameObject.SetActive(true);      
        public void Hide() => gameObject.SetActive(false);
        public void UpdateRemaningTime(string remaningSecond) => _remaningTime.text = remaningSecond;
    }
}



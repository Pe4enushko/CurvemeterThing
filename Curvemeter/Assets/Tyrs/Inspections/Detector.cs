using HurricaneVR.Framework.Weapons.Guns;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inspections {

    public class Detector : MonoBehaviour
    {
        [Header("Установить ссылку на камеру с которой происходит осмотр")]
        [SerializeField] private GameObject _camera;
        [Header("Угол видимости относительно направления взгляда камеры (+-) в градусах")]
        [SerializeField] private int _viewAngle = 15;
        [Header("Колличество секунд для осмотра")]
        [SerializeField] private int _sec;
        [Header("Установить ссылку на индикатор заполнения осмотра")]
        [SerializeField] private Image _fulled;
        [Header("Установить ссылку на UI индикатора")]
        [SerializeField] private GameObject _indicator;
        [Header("Установить ссылку на оружие которое осматриваем(HVRGunBase)")]
        [SerializeField] private HVRGunBase _gun;
        private UnityEvent _viewed = new UnityEvent();
        private float _currentSeconds = 0;
        private Coroutine _inspection;

        public UnityEvent Viewed { get => _viewed; }

        public void StartInspection() { 
            _inspection = StartCoroutine(Inspection());
            ShowIndicator();
        }

        public void StopInspection() {            
            if(_inspection!= null)
                StopCoroutine(_inspection);
            Reset();
        }

        public void Fulled()
        {            
            _currentSeconds += 1 * Time.deltaTime;
            _fulled.fillAmount = Mathf.Clamp(_currentSeconds * 1 / _sec, 0, _sec);
            if (_currentSeconds >= _sec) { 
                StopCoroutine(_inspection);
                Reset();
                _viewed?.Invoke();
            }         
        }

        public void Reset()
        {
            _currentSeconds = 0;
            _fulled.fillAmount = 0;
            HideIndicator();
        }
        private void ShowIndicator() {
            Debug.Log("Показать индикатор");
            _indicator.SetActive(true);
        }
        private void HideIndicator() {
            Debug.Log("Скрыть индикатор");
            _indicator.SetActive(false);
        }

        private IEnumerator Inspection() {            
            while(true){
                yield return null; 
                Vector3 directionToObject = transform.position - _camera.transform.position; // Направление к предмету
                Vector3 cameraViewDirection = Vector3.Normalize(_camera.transform.forward); //Направление взгляда камеры
                Vector3 directionViewOfObject = Vector3.Normalize(directionToObject);//Направление от камеры до объекта
                float angle = Vector3.Angle(cameraViewDirection, directionViewOfObject);                
                if (angle < _viewAngle && IsCockingHandleRetracted(_gun))
                    Fulled();                  
            }
        }
        /// <summary>
        /// Поверка отведена ли рукоятка взведения
        /// </summary>
        /// <param name="gun">ссылка на оружие в котором проверяем</param>
        /// <returns></returns>
        private bool IsCockingHandleRetracted(HVRGunBase gun) 
            => gun.CockingHandle.gameObject.transform.localPosition == gun.CockingHandle.BackwardPosition ? true : false;
    }
}


using HurricaneVR.Framework.Weapons.Guns;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inspections {

    public class Detector : MonoBehaviour
    {
        [Header("���������� ������ �� ������ � ������� ���������� ������")]
        [SerializeField] private GameObject _camera;
        [Header("���� ��������� ������������ ����������� ������� ������ (+-) � ��������")]
        [SerializeField] private int _viewAngle = 15;
        [Header("����������� ������ ��� �������")]
        [SerializeField] private int _sec;
        [Header("���������� ������ �� ��������� ���������� �������")]
        [SerializeField] private Image _fulled;
        [Header("���������� ������ �� UI ����������")]
        [SerializeField] private GameObject _indicator;
        [Header("���������� ������ �� ������ ������� �����������(HVRGunBase)")]
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
            Debug.Log("�������� ���������");
            _indicator.SetActive(true);
        }
        private void HideIndicator() {
            Debug.Log("������ ���������");
            _indicator.SetActive(false);
        }

        private IEnumerator Inspection() {            
            while(true){
                yield return null; 
                Vector3 directionToObject = transform.position - _camera.transform.position; // ����������� � ��������
                Vector3 cameraViewDirection = Vector3.Normalize(_camera.transform.forward); //����������� ������� ������
                Vector3 directionViewOfObject = Vector3.Normalize(directionToObject);//����������� �� ������ �� �������
                float angle = Vector3.Angle(cameraViewDirection, directionViewOfObject);                
                if (angle < _viewAngle && IsCockingHandleRetracted(_gun))
                    Fulled();                  
            }
        }
        /// <summary>
        /// ������� �������� �� �������� ���������
        /// </summary>
        /// <param name="gun">������ �� ������ � ������� ���������</param>
        /// <returns></returns>
        private bool IsCockingHandleRetracted(HVRGunBase gun) 
            => gun.CockingHandle.gameObject.transform.localPosition == gun.CockingHandle.BackwardPosition ? true : false;
    }
}


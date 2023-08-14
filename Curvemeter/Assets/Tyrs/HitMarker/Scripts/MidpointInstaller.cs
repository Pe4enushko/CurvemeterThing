using System.Collections.Generic;
using UnityEngine;

namespace HitMarker
{
    public class MidpointInstaller : MonoBehaviour
    {
        [SerializeField] private GameObject _midpoint;              
        [SerializeField] private DecalInstaller _decalInstaller;

        private List<Vector3> _currentPoints;
        private void OnEnable() => _decalInstaller.AddPoint.AddListener(SetMidpoint);

        private void OnDisable() => _decalInstaller.AddPoint.RemoveListener(SetMidpoint);

        public void Reset()
        {
            _currentPoints = new List<Vector3>();
            _midpoint.SetActive(false);
        }

        public void SetMidpoint(Vector3 newPoint) {
            _currentPoints.Add(newPoint);
            if (_currentPoints.Count > 1) {
                _midpoint.SetActive(true);
                float totalX = 0f;
                float totalY = 0f;
                foreach(var point in _currentPoints)
                {
                     totalX += point.x;
                     totalY += point.y;
                }
                _midpoint.transform.position = new Vector3(totalX / _currentPoints.Count, totalY / _currentPoints.Count, _midpoint.transform.position.z);
            }
            
        }
    }
}
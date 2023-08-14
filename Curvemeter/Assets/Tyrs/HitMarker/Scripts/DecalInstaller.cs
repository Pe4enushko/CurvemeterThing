using HurricaneVR.Framework.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HitMarker{
    public class DecalInstaller : HVRDamageHandlerBase
    {
        [SerializeField] private GameObject _hitPoint3DDecal;
        private List<GameObject> _3DDecals = new List<GameObject>();
        private UnityEvent<Vector3> _addPoint = new UnityEvent<Vector3>();
        public UnityEvent<Vector3> AddPoint { get => _addPoint; }

        public override void HandleDamageProvider(HVRDamageProvider damageProvider, Vector3 hitPoint, Vector3 direction)
        {
            base.HandleDamageProvider(damageProvider, hitPoint, direction);
            GameObject go3DDecal = Instantiate(_hitPoint3DDecal, hitPoint, Quaternion.identity, transform);
            _3DDecals.Add(go3DDecal);
            _addPoint?.Invoke(go3DDecal.transform.position);
        }

        public void Reset()
        {
            _3DDecals.ForEach(decal => Destroy(decal));
            _3DDecals = new List<GameObject>();
        }
    }
}
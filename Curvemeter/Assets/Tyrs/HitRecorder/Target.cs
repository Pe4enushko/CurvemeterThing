using HurricaneVR.Framework.Weapons.Guns;
using UnityEngine;

namespace Tyrs.HitRecorder
{
    public class Target : MonoBehaviour{        
        [SerializeField] private Collider _mark;
        [SerializeField] private Collider _safeZone;

        public Collider Mark { get => _mark; }
        public Collider SafeZone { get => _safeZone; }
    }
}


using HurricaneVR.Framework.Weapons.Guns;
using UnityEngine;
using UnityEngine.Events;

namespace Tyrs.HitRecorder
{
    public class HitChecker {
        private HVRGunBase _gun;
        private Collider _mark;
        private Collider _safeZone;
        private StatsScreen _statsScreen;
        private int _shotLimit;
        private UnityEvent<int> _hit = new UnityEvent<int>();
        private UnityEvent<int> _miss = new UnityEvent<int>();
        private UnityEvent<int> _randomHit = new UnityEvent<int>();
        private UnityEvent _shotLimitReached = new UnityEvent();

        private int _currentHit = 0;
        private int _currentMiss = 0;
        private int _currentRandomHit = 0;
        private int _currentShot = 0;
        public int CurrentHit { get => _currentHit; }
        public UnityEvent ShotLimitReached { get => _shotLimitReached; }
        public int CurrentShot { get => _currentShot; }

        public HitChecker(HVRGunBase gun, Collider target, Collider safeZone, StatsScreen statsScreen, int shotLimit)
        {
            _gun = gun;
            _mark = target;
            _safeZone = safeZone;
            _statsScreen = statsScreen;
            _shotLimit = shotLimit;
        }

        public void Launch() {
            _hit.AddListener(_statsScreen.ChangeValueHits);
            _randomHit.AddListener(_statsScreen.ChangeValueRandomHits);
            _miss.AddListener(_statsScreen.ChangeValueMisses);
            ResetCheker();
            _gun.Hit.AddListener(ZoneHitCheck);
        }

        public void Stop()
        {
            _gun.Hit.RemoveListener(ZoneHitCheck);
            _hit.RemoveListener(_statsScreen.ChangeValueHits);
            _randomHit.RemoveListener(_statsScreen.ChangeValueRandomHits);
            _miss.RemoveListener(_statsScreen.ChangeValueMisses);
        }

        public void ResetCheker() { 
            _currentHit = 0;
            _currentMiss = 0;
            _currentRandomHit = 0;
            _currentShot = 0;
            _gun.Hit.RemoveListener(ZoneHitCheck);
            
        }
        
        private void ZoneHitCheck(GunHitArgs hitInfo)
        {            
            if (hitInfo.Collider.Equals(_mark)){
                _currentHit += 1;
                _hit?.Invoke(_currentHit);
            }
            else if (hitInfo.Collider.Equals(_safeZone)){                
                _currentMiss += 1;
                _miss?.Invoke(_currentMiss);
            }
            else {
                _currentRandomHit += 1;
                _randomHit?.Invoke(_currentRandomHit);
            }

            _currentShot += 1;
            if (_shotLimit == _currentShot)
                _shotLimitReached?.Invoke();
        }     
    }

}


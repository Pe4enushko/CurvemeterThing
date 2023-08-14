using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tyrs.ZoneTyrs
{
    /// <summary>
    /// Компонент отвечающий за наблюдение за интерактивностью зон если одна зона активна остальные не должны быть активными
    /// </summary>
    public class ZoneWatcher : MonoBehaviour {
        private List<TyrsLaunchPlace> _zones = new List<TyrsLaunchPlace>();
        private TyrsLaunchPlace _currentActiveZone;
        private void Awake()
        {
            _zones = FindObjectsOfType<TyrsLaunchPlace>(true).ToList();
            if (_zones != null)
                _zones.ForEach(zone => zone.Activated.AddListener(Activated));
            else
                Debug.LogError("На сцене нет зон");
        }

        private void Activated(TyrsLaunchPlace activatedZone) {
            _currentActiveZone = activatedZone;
            _currentActiveZone.Exited.AddListener(Deactivated);
            foreach (var zone in _zones) {
                if (!zone.Equals(activatedZone))
                    zone.Hide();
            }
        }

        private void Deactivated() {
            _currentActiveZone.Exited.RemoveListener(Deactivated);
            _zones.ForEach(zone => zone.Show());
        }
    }
}



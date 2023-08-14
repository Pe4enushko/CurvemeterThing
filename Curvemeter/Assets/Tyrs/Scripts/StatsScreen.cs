using TMPro;
using UnityEngine;

namespace Tyrs
{
    public class StatsScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _valueTime;
        [SerializeField] private TextMeshProUGUI _valueHits;
        [SerializeField] private TextMeshProUGUI _valueMisses;
        [SerializeField] private TextMeshProUGUI _valueRandomHits;
        [SerializeField] private TextMeshProUGUI _results;

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            _valueTime.text = "00:00";
            _valueHits.text = "0";
            _valueMisses.text = "0";
            _valueRandomHits.text = "0";
            _results.text = "";
        }

        public void ChangeValueTime(string newValue) {
            _valueTime.text = newValue;
        }

        public void ChangeValueHits(int newValue) {
            _valueHits.text = newValue.ToString();
        }
        public void ChangeValueMisses(int newValue) {
            _valueMisses.text = newValue.ToString();
        }
        public void ChangeValueRandomHits(int newValue) {
            _valueRandomHits.text = newValue.ToString();
        }

        public void ShowResult(string result) {
            _results.text = result;
        }
    }
}

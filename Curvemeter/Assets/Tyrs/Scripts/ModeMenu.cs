using UnityEngine;
using UnityEngine.Events;

namespace Tyrs
{
    public class ModeMenu : MonoBehaviour
    {
        [SerializeField] private UiButton _learningButton;
        [SerializeField] private UiButton _testingButton;
        
        public void Show(UnityAction launchLearningAction, UnityAction launchTestingAction)
        {
            this.gameObject.SetActive(true);
            _learningButton.onClick.AddListener(launchLearningAction);
            _testingButton.onClick.AddListener(launchTestingAction);            
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            _learningButton.onClick.RemoveAllListeners();
            _testingButton.onClick.RemoveAllListeners(); 
        }
    }
}

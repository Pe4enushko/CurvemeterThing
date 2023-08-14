using UnityEngine;
using UnityEngine.EventSystems;

public class UiHoverClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter (PointerEventData eventData)
    {
        ExecuteEvents.Execute (gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }
    
    public void OnPointerExit (PointerEventData eventData)
    {
        ExecuteEvents.Execute (gameObject, eventData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute (gameObject, eventData, ExecuteEvents.pointerClickHandler);
    }
}

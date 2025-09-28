using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //select
        //remove highlight if highlighted
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //release
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //start drag
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //update drag position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //finish drag
    }

}

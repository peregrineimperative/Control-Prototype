using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Renderer renderer;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //renderer.materials[1].OutlineColor = 
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Remove highlight
    }

}

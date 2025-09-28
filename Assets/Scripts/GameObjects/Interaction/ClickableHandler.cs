using UnityEngine;
using UnityEngine.EventSystems;

//Attach this to objects that can do things with a simple click interaction.
//Currently just useful for spawn points (click to create new GamePiece object)
public class ClickableHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //Trigger Click Function
    }
}

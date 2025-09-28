using System.Buffers;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The players' game pieces that move around the board.
/// Instantiated by clicking on SpawnPoint.
/// </summary>
public class GamePiece : PieceParent, IDraggable
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
        //Make sure that object maintains constant Y position
        //Determine the tile directly beneath
            //Highlight that tile
        //update drag position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Check if there is a marked tile
        //If so, snap piece to that tile, set occupied, etc.
        //If not, snap back to previous tile
    }

    private GameObject TryGetCellBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, 6, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.;
        }
        return null;

    }
}

using System.Buffers;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The players' game pieces that move around the board.
/// Instantiated by clicking on SpawnPoint.
/// </summary>
public class GamePiece : PieceParent, IDraggable
{
    
    private Plane _dragPlane;
    private Vector3 _dragOffset;
    private GameObject _hoveredCell;
    private GameObject _nextCell;
    
    private Vector3 _startPosition;
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _startPosition = transform.position;

        //transform.position = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //transform.position = _startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        
        _dragPlane = new Plane(Vector3.up, transform.position);
        
        var ray = Camera.main.ScreenPointToRay(eventData.position);
        
        _dragPlane.Raycast(ray, out float distance);
        
        _dragOffset = transform.position - ray.GetPoint(distance);
        
        _hoveredCell = TryGetCellBelow();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Movement to raycast on _dragPlane
        var ray = Camera.main.ScreenPointToRay(eventData.position);
        
        _dragPlane.Raycast(ray, out float distance);
        
        var targetPosition = ray.GetPoint(distance) + _dragOffset;
        
        transform.position = targetPosition;
        
        //Cell detection
        _hoveredCell = TryGetCellBelow();
        _nextCell = CurrentCell;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //If there is an actively hovered cell when ending drag, have the piece snap to that cell, otherwise return to start
        transform.position = _hoveredCell != null ? _hoveredCell.GetComponent<BoardCell>().GetSnapPosition(gameObject) : _startPosition;
        
        
        //Check if there is a marked tile
        //If so, snap piece to that tile, set occupied, etc.
        //If not, snap back to previous tile
    }

    private GameObject TryGetCellBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, 1 << 6, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.gameObject;
        }
        return null;

    }
}

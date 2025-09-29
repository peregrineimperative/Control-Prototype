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
    
    private BoardCell _hoveredCell;
    private BoardCell _previousCell;
    
    private Vector3 _startPosition;

    protected override void Start()
    {
        base.Start();
        CurrentCell.AddOccupant(this);
    }
    
    //---Events---
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
        //Instantiate plane to keep movement on XZ, maintaining Y position
        _dragPlane = new Plane(Vector3.up, transform.position);
        
        //Get offset between GamePiece centerpoint and pointer
        var ray = Camera.main.ScreenPointToRay(eventData.position);
        _dragPlane.Raycast(ray, out float distance);
        _dragOffset = transform.position - ray.GetPoint(distance);
        
        //Cell detection
        _hoveredCell = TryGetCellBelow();
        
        //Set
        if (_hoveredCell != null)
        {
            _previousCell = _hoveredCell;
            _hoveredCell.PreviewOwnership(this);
        }
        else
        {
            _previousCell = CurrentCell.GetComponent<BoardCell>();
        }
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
        
        //Preview cell color on hover
        if (_hoveredCell != _previousCell)
        {
            _previousCell?.RefreshPaint();
        }
        _previousCell = _hoveredCell;
        if (_hoveredCell != null)
        {
            _hoveredCell.PreviewOwnership(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //If there is an actively hovered cell when ending drag, have the piece snap to that cell, otherwise return to start
        if (_hoveredCell != null)
        {
            if (_hoveredCell != CurrentCell)
            {
                transform.position = _hoveredCell.GetSnapPosition(gameObject);
                _hoveredCell.FinalizeOwnership(this);
                CurrentCell.RemoveOccupant(this);
                _hoveredCell.AddOccupant(this);
                CurrentCell = _hoveredCell;
            }
            else
            {
                transform.position = _startPosition;
                _hoveredCell.RefreshPaint();
            }
        }
        else
        {
            transform.position = _startPosition;       
        }

        if (_previousCell != null && _previousCell != _hoveredCell)
        {
            _previousCell.RefreshPaint();       
        }
        
        //Reset
        _hoveredCell = null;
        _previousCell = null;
        
    }

    private BoardCell TryGetCellBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, 1 << 6, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<BoardCell>();
        }
        return null;

    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

    private Dictionary<BoardCell, int> _reachableCells;
    private List<BoardCell> _highlightedCells;
    private bool _isShowingHighlights;
    private bool _isInteracting;
    private bool _isDragging;
    

    protected override void Start()
    {
        base.Start();
        CurrentCell.AddOccupant(this);
    }

    private void Update()
    {
        if (!_isInteracting) return;
        
        bool modifierHeld = IsModifierHeld();
        
        if (modifierHeld && !_isShowingHighlights)
        {
            ShowHighlights();
        }
        else if (!modifierHeld && _isShowingHighlights)
        {
            ClearHighlights();
        }
    }
    
    
    
    //---Events---
    public void OnPointerDown(PointerEventData eventData)
    {
        _isInteracting = true;
        _isDragging = false;
        _startPosition = transform.position;
        _reachableCells = GridManager.Instance.GetReachableCells(CurrentCell, Owner.Energy);

        if (IsModifierHeld())
        {
            ShowHighlights();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            ResetGamePiece();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        
        //Instantiate plane to keep movement on XZ, maintaining Y position
        _dragPlane = new Plane(Vector3.up, transform.position);
        
        //Get offset between GamePiece centerpoint and pointer
        var ray = Camera.main.ScreenPointToRay(eventData.position);
        _dragPlane.Raycast(ray, out float distance);
        _dragOffset = transform.position - ray.GetPoint(distance);
        
        //Cell detection
        _hoveredCell = TryGetCellBelow();
        
        //Set
        if (IsReachable(_hoveredCell))
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
            RestoreCellVisual(_previousCell);
        }
        _previousCell = _hoveredCell;
        
        if (IsReachable(_hoveredCell))
        {
            _hoveredCell.PreviewOwnership(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //If there is an actively hovered cell when ending drag, have the piece snap to that cell, otherwise return to start
        //Determine if selection is a valid move
        if (IsReachable(_hoveredCell))
        {
            if (_hoveredCell != CurrentCell)
            {
                
                //Deplete energy here
                //if (_reachableCells.TryGetValue(_hoveredCell, out var cost)) {Owner.Energy -= cost;}
                
                transform.position = _hoveredCell.GetSnapPosition(gameObject);
                _hoveredCell.FinalizeOwnership(this);
                CurrentCell.RemoveOccupant(this);
                _hoveredCell.AddOccupant(this);
                CurrentCell = _hoveredCell;
            }
            else
            {
                transform.position = _startPosition;
                RestoreCellVisual(_hoveredCell);
            }
        }
        else
        {
            transform.position = _startPosition;
            RestoreCellVisual(_hoveredCell);
        }

        if (_previousCell != null && _previousCell != _hoveredCell)
        {
            RestoreCellVisual(_previousCell);     
        }
        
        //Reset
        ResetGamePiece();
    }

    private void ResetGamePiece()
    {
        _hoveredCell = null;
        _previousCell = null;
        _isInteracting = false;
        ClearHighlights();
        _reachableCells = null;
        
    }
    
    private BoardCell TryGetCellBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, 1 << 6, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<BoardCell>();
        }
        return null;

    }

    private bool IsReachable(BoardCell cell)
    {
        return _reachableCells != null && cell != null && _reachableCells.ContainsKey(cell);
    }
    
    //---Highlighting within range---
    private bool IsModifierHeld()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void ShowHighlights()
    {
        if (_isShowingHighlights) return;
        _highlightedCells = ColorHelper.HighlightReachableCells(_reachableCells, Owner.Energy);
        _isShowingHighlights = true;
    }

    private void ClearHighlights()
    {
        if (!_isShowingHighlights) return;
        ColorHelper.ClearHighlights(_highlightedCells);
        _highlightedCells = null;
        _isShowingHighlights = false;
    }

    private void RestoreCellVisual(BoardCell cell)
    {
        if (cell == null) return;

        if (_isShowingHighlights && _reachableCells != null && _reachableCells.TryGetValue(cell, out int dist))
        {
            cell.SetMoveHighlight(ColorHelper.StepGradient(dist, Owner.Energy));
        }
        else
        {
            cell.RefreshPaint();
        }
    }
    
}

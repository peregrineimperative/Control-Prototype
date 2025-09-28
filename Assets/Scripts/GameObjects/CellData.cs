using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Information contained by each cell of the grid to be referenced by other game systems.
/// </summary>
public class CellData : MonoBehaviour
{
    [SerializeField] private Collider cellCollider;
    
    public Player CurrentOwner { get; set; }
    public Vector2Int GridPosition { get; private set; }
    
    public bool IsOccupied { get; set; }
    private List<GamePiece> _occupants = new List<GamePiece>();
    
    //If the _occupants list is not zero, record the last entry on the list as the top piece.
    public GamePiece TopPiece => _occupants.Count > 0 ? _occupants[^1] : null;
    
    //---Snap-to variables
    private float CellTopY => cellCollider != null ? cellCollider.bounds.max.y : transform.position.y;
    
    public void SetGridPosition(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
        
        gameObject.name = $"Cell {GridPosition.x} x {GridPosition.y}";
    }

    public void PaintCell()
    {
        //Tally the occupants' owners, and set color to owner with the most occupants.
    }

    public void AddOccupant(GamePiece piece)
    {
        _occupants.Add(piece);
    }
    
    public void RemoveOccupant(GamePiece piece)
    {
        
    }

    public Vector3 GetSnapPosition()
    {
        float snapY = _occupants.Count > 0 ? _occupants[^1].PieceTopY : CellTopY;
        return new Vector3(transform.position.x, snapY, transform.position.z);
    }
    
}


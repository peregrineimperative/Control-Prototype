using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Information contained by each cell of the grid to be referenced by other game systems.
/// </summary>
public class CellData : MonoBehaviour {
    
    [SerializeField] private Collider cellCollider;
    
    public Player CurrentOwner { get; set; }
    public Vector2Int GridPosition { get; private set; }
    
    public bool IsOccupied { get; set; }
    private List<GameObject> _occupants = new List<GameObject>();
    
    //If the _occupants list is not zero, record the last entry on the list as the top piece. Otherwise, return the cell itself.
    public GameObject TopPiece => _occupants.Count > 0 ? _occupants[^1] : gameObject;
    
    //---Snap-to variables
    //private float CellTopY => cellCollider != null ? cellCollider.bounds.max.y : transform.position.y;
    
    
    public void SetGridPosition(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
        
        gameObject.name = $"Cell {GridPosition.x} x {GridPosition.y}";
    }

    public void PaintCell()
    {
        //Tally the occupants' owners, and set color to owner with the most occupants.
        //Priority:
        //Occupying pieces (highest count wins)
        //Within captured tower radius
        //If occupation tie, then default to tower radius.
        //If tie without tower, return to default/neutral color.
    }

    public void AddOccupant(GameObject piece)
    {
        _occupants.Add(piece);
    }
    
    public void RemoveOccupant(GameObject piece)
    {
        _occupants.Remove(piece);
    }
    
    //---Helper Functions---
    //Consider moving these to a cell/grid helper class or something.
    
    //Provide the highest point for a game piece to snap to.
    public Vector3 GetSnapPosition(GameObject inbound, GameObject target)
    {
        var targetPosition = target.GetComponent<Transform>().position;
        
        float snapToY = target.GetComponent<Collider>().bounds.max.y + (inbound.GetComponent<Collider>().bounds.size.y / 2);
        
        Vector3 snapPosition = new Vector3(targetPosition.x, snapToY, targetPosition.z);

        return snapPosition;
    }

    //Figure out who owns a given tile
    //Should be called only after a move has been made.
    //Call in both the tile being moved away from and onto.
    private Player DetermineCurrentOwner()
    {
        if (_occupants.Count == 0) return null; 
        var counts = new Dictionary<Player, int>(); //Dictionary to hold player names and current tally

        foreach (var occupant in _occupants)
        {
            if (occupant == null) continue;
            var owner = occupant.GetComponent<GamePiece>().Owner;
            //Check if the counts dictionary already contains the owner as the key
            if (counts.ContainsKey(owner))
            {
                //If so, increment their count
                counts[owner]++;
            }
            else
            {
                //If not, add them to the count, and give them a tally
                counts.Add(owner, 1);
            }
        }

        Player winner = null;
        int maxCount = 0;
        
        foreach (var count in counts)
        {
            if (count.Value > maxCount)
            {
                winner = count.Key;
                maxCount = count.Value;
            }
            else if (count.Value == maxCount)
            {
                winner = null;
            }
        }
        
        return winner;
    }
    
}


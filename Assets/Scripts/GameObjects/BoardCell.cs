using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Information contained by each cell of the grid to be referenced by other game systems.
/// </summary>
public class BoardCell : MonoBehaviour {
    
    [SerializeField] private Collider cellCollider;
    [SerializeField] public Renderer cellRenderer;
    [SerializeField] private Color defaultColor;
    
    public Vector2Int GridPosition { get; private set; }
    public Player CurrentOwner { get; set; }
    public Player TowerOwner { get; set; } //Set if this cell is within a captured tower's radius.
    
    [SerializeField] public GameObject baseObject; //Object upon which other objects will stack, if any. (i.e., towers or spawn points)
    
    private List<GameObject> _occupants = new List<GameObject>();
    
    //If the _occupants list is not zero, record the last entry on the list as the top piece. Otherwise, return the base object (usually the cell itself)
    [SerializeField] public GameObject TopPiece
    {
        get
        {
            CleanOccupants();
            return _occupants.Count > 0 ? _occupants[^1] : baseObject;
        }
    }

    public bool IsOccupied { get; set; }
    public bool IsHovered { get; set; }
    public bool IsHighlighted { get; set; }
    public bool IsPainted { get; set; }

    private void Start()
    {
        baseObject = gameObject;
        cellRenderer = GetComponent<Renderer>();
        IsHighlighted = false;
    }
    
    //---Physical Space---
    #region Physical Space
    public void SetGridPosition(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
        
        gameObject.name = $"Cell {GridPosition.x} x {GridPosition.y}";
    }
    
    //Provide the highest point for a game piece to snap to.
    public Vector3 GetSnapPosition(GameObject inbound)
    {
        var targetPosition = TopPiece.GetComponent<Transform>().position;
        
        float snapToY = TopPiece.GetComponent<Collider>().bounds.max.y + (inbound.GetComponent<Collider>().bounds.size.y / 2);
        
        Vector3 snapPosition = new Vector3(targetPosition.x, snapToY, targetPosition.z);

        return snapPosition;
    }
    #endregion

    //---Occupant Management---
    #region Occupant Management
    public void AddOccupant(PieceParent piece)
    {
        _occupants.Add(piece.gameObject);
    }
    
    public void RemoveOccupant(PieceParent piece)
    {
        _occupants.Remove(piece.gameObject);
        CleanOccupants();

        CurrentOwner = DetermineOwner(null);
        RefreshPaint();
    }

    private void CleanOccupants()
    {
        _occupants.RemoveAll(occupant => occupant == null);
    }
    
    #endregion
    
    //---Cell Painting/Visuals---
    #region Cell Painting/Visuals
    //To be called when player hovers piece over cell
    //Show what the tile would look like if piece were placed
    public void PreviewOwnership(GamePiece inbound)
    {
        var previewOwner = DetermineOwner(inbound?.Owner);
        ApplyPaint(previewOwner, true);
    }
    
    //To be called when player finalizes piece position
    public void FinalizeOwnership(GamePiece inbound)
    {
        CurrentOwner = DetermineOwner(inbound?.Owner);
        ApplyPaint(CurrentOwner, preview: false);
    }
    
    public void ApplyPaint(Player owner, bool preview)
    {
        if (owner == null)
        {
            cellRenderer.SetBaseColor(defaultColor);
            return;
        }

        //If preview, change color to highlight color, if not, make it the actual paint color
        var color = preview ? owner.ColorTheme.highlightColor : owner.ColorTheme.paintColor;
        cellRenderer.SetBaseColor(color);
    }
    
    //To be called after previewing without committing
    public void RefreshPaint()
    {
        ApplyPaint(CurrentOwner, preview: false);
    }
    #endregion

    //---Cell Ownership---
    #region Cell Ownership
    //Figure out who owns a given cell
    //Should be called only after a move has been made.
    //Call in both the cell being moved away from and onto.
    private Player DetermineOccupantMajority(Player newPieceOwner = null)
    {
        if (_occupants.Count == 0 && newPieceOwner == null) return null; 
        
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
        
        //Allow for preview of potential ownership before dropping piece into place.
        if (newPieceOwner != null)
        {
            if (counts.ContainsKey(newPieceOwner))
            {
                counts[newPieceOwner]++;
            }
            else
            {
                counts.Add(newPieceOwner, 1);
            }
        }

        Player majorityOwner = null;
        int maxCount = 0;
        
        foreach (var count in counts)
        {
            if (count.Value > maxCount)
            {
                majorityOwner = count.Key;
                maxCount = count.Value;
            }
            else if (count.Value == maxCount)
            {
                majorityOwner = null;
            }
        }
        
        return majorityOwner;
    }
        
    //Owner Priority Rules:
    //1. Occupying majority
    //2. Tower owner, if present
    //3. If tied and tower exists, tower owner
    //4. If tied and no tower, default to neutral color
    //5. If no occupants and no tower, retain last painted
    private Player DetermineOwner(Player newPieceOwner)
    {
        var majorityOwner = DetermineOccupantMajority(newPieceOwner);
        
        if (majorityOwner != null)
        {
            return majorityOwner;
        }
        else if (TowerOwner != null)
        {
            return TowerOwner;
        }
        else
        {
            return CurrentOwner;
        }
    }
    #endregion
}


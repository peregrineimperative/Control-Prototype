using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Information contained by each cell of the grid to be referenced by other game systems.
/// </summary>
public class BoardCell : MonoBehaviour {
    
    [SerializeField] private Collider cellCollider;
    [SerializeField] public Renderer cellRenderer;
    [SerializeField] private Color defaultColor;
    
    
    public Player CurrentOwner { get; set; }
    public Player TowerOwner { get; set; } //Set if this cell is within a captured tower's radius.

    public Tower Tower { get; set; }
    
    [SerializeField] public GameObject baseObject; //Object upon which other objects will stack, if any. (i.e., towers or spawn points)
    
    private List<GameObject> _occupants = new List<GameObject>();

    private Vector2Int _gridPosition;
    public Vector2Int GridPosition
    {
        get {return _gridPosition;}

        set
        {
            if (value == _gridPosition) return;
            _gridPosition = value;
            gameObject.name = $"Cell {GridPosition.x} x {GridPosition.y}";
        }
    }
    
    //If the _occupants list is not zero, record the last entry on the list as the top piece. Otherwise, return the base object (usually the cell itself)
    public GameObject TopPiece
    {
        get
        {
            CleanOccupants();
            return _occupants.Count > 0 ? _occupants[^1] : baseObject;
        }
    }
    
    public bool IsHighlighted { get; set; }

    private void Start()
    {
        baseObject = gameObject;
        cellRenderer = GetComponent<Renderer>();
        IsHighlighted = false;
    }
    
    //---Physical Space---
    #region Physical Space
    
    //Provide the highest point for a game piece to snap to.
    public Vector3 GetSnapPosition(GameObject inbound)
    {
        CleanOccupants();
        
        //Find supporting object for the inbound piece so it does not count itself for dragging/preview purposes
        GameObject support = null;
        for (int i = _occupants.Count - 1; i >= 0; i--)
        {
            var occupant = _occupants[i];
            if (occupant != null && occupant != inbound)
            {
                support = occupant;
                break;
            }
        }
        if (support == null)
        {
            support = baseObject != null ? baseObject : gameObject;
        }

        //Compute the snap Y using the support's top and the inbound's half height
        var supportTransform = support.transform;
        var supportCollider = support.GetComponent<Collider>();
        float supportTopY = supportCollider != null ? supportCollider.bounds.max.y : supportTransform.position.y;

        float inboundHalfHeight = 0f;
        var inboundCollider = inbound != null ? inbound.GetComponent<Collider>() : null;
        if (inboundCollider != null)
        {
            inboundHalfHeight = inboundCollider.bounds.size.y * 0.5f;
        }

        float snapToY = supportTopY + inboundHalfHeight;
        Vector3 targetPosition = supportTransform.position;

        return new Vector3(targetPosition.x, snapToY, targetPosition.z);

    }
    #endregion

    //---Occupant Management---
    //These functions relate to making sure the cell keeps track of what is on top of it.
    #region Occupant Management

    
    
    //Returns true if the cell is occupied
    private bool _isOccupied;
    public bool IsOccupied
    {
        get
        {
            CleanOccupants();
            return _occupants.Count > 0;
        }
    }
    
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

        if (Tower != null)
        {
            Tower.Owner = CurrentOwner;
            Tower.ApplyInfluence();
        }
    }

    //Cleans the _occupants list of empty slots.
    private void CleanOccupants()
    {
        _occupants.RemoveAll(occupant => occupant == null);
    }
    
    #endregion
    
    //---Cell Painting/Visuals---
    #region Cell Painting/Visuals

    public void SetMoveHighlight(Color color)
    {
        IsHighlighted = true;
        cellRenderer.SetBaseColor(color);
    }
    
    public void ClearMoveHighlight()
    {
        IsHighlighted = false;
        RefreshPaint();
    }
    
    //To be called when player hovers piece over cell
    //Show what the tile would look like if piece were placed
    public void PreviewOwnership(GamePiece inbound)
    {
        var previewOwner = DetermineOwner(inbound?.Owner);
        ApplyPaint(previewOwner, true);

        if (Tower != null)
        {
            Tower.PreviewInfluence(previewOwner);
        }
    }
    
    //To be called when player finalizes piece position
    public void FinalizeOwnership(GamePiece inbound)
    {
        CurrentOwner = DetermineOwner(inbound?.Owner);
        ApplyPaint(CurrentOwner, preview: false);

        if (Tower != null)
        {
            Tower.ClearPreview();
            Tower.Owner = CurrentOwner;
            Tower.ApplyInfluence();
        }
    }
    
    //Changes the color of the cell to match the controlling player.
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

        if (Tower != null)
        {
            Tower.ClearPreview();
        }
    }
    #endregion

    //---Cell Ownership---
    #region Cell Ownership
    
    //Figure out who owns a given cell
    //Should be called only after a move has been made.
    //Call in both the cell being moved away from and onto.
    private Player DetermineOccupantMajority(Player newPieceOwner, out bool isTie)
    {
        //if (_occupants.Count == 0 && newPieceOwner == null) return null; 
        
        var counts = new Dictionary<Player, int>(); //Dictionary to hold player names and current tally

        foreach (var occupant in _occupants)
        {
            if (occupant == null) continue;
            var owner = occupant.GetComponent<GamePiece>().Owner;
            //Check if the counts dictionary already contains the owner as the key
            counts[owner] = counts.TryGetValue(owner, out var count) ? count + 1 : 1;
        }
        
        //Allow for preview of potential ownership before dropping piece into place.
        if (newPieceOwner != null)
        {
            counts[newPieceOwner] = counts.TryGetValue(newPieceOwner, out var newCount) ? newCount + 1 : 1;
        }

        if (counts.Count == 0)
        {
            isTie = false;
            return null;
        }

        Player majorityOwner = null;
        int maxCount = 0;
        int numAtMax = 0;
        
        foreach (var count in counts)
        {
            if (count.Value > maxCount)
            {
                majorityOwner = count.Key;
                maxCount = count.Value;
                numAtMax = 1;
            }
            else if (count.Value == maxCount)
            {
                numAtMax++;
            }
        }
        
        isTie = numAtMax > 1;
        return isTie ? null : majorityOwner;
    }
        
    //Owner Priority Rules:
    //1. Occupying majority
    //2. Tower owner, if present
    //3. If tied and tower exists, tower owner
    //4. If tied and no tower, default to neutral color
    //5. If no occupants and no tower, retain last painted
    private Player DetermineOwner(Player newPieceOwner)
    {
        var majorityOwner = DetermineOccupantMajority(newPieceOwner, out bool isTie);
        
        if (majorityOwner != null)
        {
            return majorityOwner;
        }

        if (isTie)
        {
            return null;
        }
        
        if (TowerOwner != null)
        {
            return TowerOwner;
        }
        
        return CurrentOwner;
        
    }
    #endregion
}


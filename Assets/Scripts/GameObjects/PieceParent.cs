using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Parent class for all objects directly controlled/instantiated by the player.
/// Currently: GamePiece and SpawnPoint
/// </summary>
public class PieceParent : MonoBehaviour, IHoverable
{
    [SerializeField] private Collider pieceCollider;
    [SerializeField] private Renderer pieceRenderer;
    
    public BoardCell CurrentCell { get; set; }
    public Player Owner { get; set; }
    
    //Color variables
    private bool _isHighlighted;
    private Color _pieceColor;
    public Color _highlightColor;
    
    //Turn management
    public bool ControlsEnabled { get; set; }
    
    protected virtual void Start()
    {
        if (!pieceRenderer) pieceRenderer = GetComponent<Renderer>();
        
        CurrentCell = Owner.SpawnLocation;
        _pieceColor = Owner.ColorTheme.pieceColor;
        _highlightColor = Owner.ColorTheme.highlightColor;
        
        Debug.Log($"{this} is tied to cell {CurrentCell}");
        
        transform.position = CurrentCell.GetSnapPosition(gameObject);
        Debug.Log($"{this} is snapped to {transform.position}");
        
        Debug.Log(CurrentCell.GetComponent<BoardCell>().TopPiece);
        
        pieceRenderer.SetBaseColor(_pieceColor);
    }
    
    //Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ControlsEnabled) return; //Enabled only during player's turn
        
        pieceRenderer.SetBaseColor(_highlightColor);
        _isHighlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!ControlsEnabled) return; //Enabled only during player's turn
        
        pieceRenderer.SetBaseColor(_pieceColor); 
        _isHighlighted = false;
    }  
}

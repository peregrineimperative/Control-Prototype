using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Parent class for all objects directly controlled/instantiated by the player.
/// Currently: GamePiece and SpawnPoint
/// </summary>
public class PieceParent : MonoBehaviour, IHoverable
{
    [SerializeField] private Collider pieceCollider;
    [SerializeField] private Renderer renderer;
    
    public GameObject CurrentCell { get; set; }
    public Player Owner { get; set; }
    
    //Color variables
    private bool _isHighlighted;
    private Color _pieceColor;
    private Color _highlightColor;
    
    private void Start()
    {
        if (!renderer) renderer = GetComponent<Renderer>();
        
        CurrentCell = Owner.SpawnLocation;
        _pieceColor = Owner.ColorTheme.pieceColor;
        _highlightColor = Owner.ColorTheme.highlightColor;
        
        Debug.Log($"{this} is tied to cell {CurrentCell}");
        transform.position = CurrentCell.GetComponent<CellData>().GetSnapPosition(gameObject, CurrentCell.GetComponent<CellData>().TopPiece);
        Debug.Log($"{this} is snapped to {transform.position}");
        CurrentCell.GetComponent<CellData>().AddOccupant(gameObject);
        Debug.Log(CurrentCell.GetComponent<CellData>().TopPiece);
        
        renderer.SetBaseColor(_pieceColor);
    }
    
    //Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        renderer.SetBaseColor(_highlightColor);
        _isHighlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        renderer.SetBaseColor(_pieceColor); 
        _isHighlighted = false;
    }  
}

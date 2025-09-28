using System.Buffers;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The players' game pieces that move around the board.
/// Instantiated by clicking on SpawnPoint.
/// </summary>
public class GamePiece : MonoBehaviour, IHoverable
{
    [SerializeField] private Collider pieceCollider;
    [SerializeField] private Renderer renderer;
    
    
    public GameObject CurrentCell { get; set; }
    public Player Owner { get; set; }
    
    //Color variables
    private bool _isHighlighted;
    private MaterialPropertyBlock _mpb;
    private Color _defaultColor;
    private Color _pieceColor;
    private Color _highlightColor;

    private void Awake()
    {
        Owner = GetComponentInParent<Player>();
    }
    
    private void Start()
    {
        if (!renderer) renderer = GetComponent<Renderer>();
        
        CurrentCell = Owner.SpawnLocation;
        
        Debug.Log($"{this} is tied to cell {CurrentCell}");
        transform.position = CurrentCell.GetComponent<CellData>().GetSnapPosition(gameObject, CurrentCell.GetComponent<CellData>().TopPiece);
        Debug.Log($"{this} is snapped to {transform.position}");
        CurrentCell.GetComponent<CellData>().AddOccupant(gameObject);
        Debug.Log(CurrentCell.GetComponent<CellData>().TopPiece);
        SetColors();
    }

    private void SetColors()
    {
        //_defaultColor = renderer.materials[1].color;
        _pieceColor = Owner.ColorTheme.pieceColor;
        _highlightColor = Owner.ColorTheme.highlightColor;
        
        _mpb = new MaterialPropertyBlock();
        
        renderer.materials[0].SetColor("_Color", _pieceColor);
    }
    
    
    //Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        //renderer.materials[1].OutlineColor        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Remove highlight
    }    
    
    
    //Used in previous iteration of snapping mechanism
    //public float PieceTopY => pieceCollider != null ? pieceCollider.bounds.max.y : transform.position.y;
    //public float PieceBottomY => pieceCollider != null ? pieceCollider.bounds.min.y : transform.position.y;

    //snapping functionality moved to cell; might move to Grid Manager or back here, I don't know yet.
    /*public void SnapToTop(GameObject target)
    {
        var targetPosition = target.GetComponent<Transform>().position;
        
        float snapToY = target.GetComponent<Collider>().bounds.max.y + (pieceCollider.bounds.size.y / 2);
        
        transform.position = new Vector3(targetPosition.x, snapToY, targetPosition.z);
        
    }*/
    //on move to cell
    //paint cell
    //snap to highest center point.
    
}

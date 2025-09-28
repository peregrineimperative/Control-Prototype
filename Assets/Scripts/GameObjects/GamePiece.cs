using System.Buffers;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The players' game pieces that move around the board.
/// Instantiated by clicking on SpawnPoint.
/// </summary>
public class GamePiece : MonoBehaviour
{
    [SerializeField] private Collider pieceCollider;
    [SerializeField] private Renderer renderer;
    
    //Used in previous iteration of snapping mechanism
    //public float PieceTopY => pieceCollider != null ? pieceCollider.bounds.max.y : transform.position.y;
    //public float PieceBottomY => pieceCollider != null ? pieceCollider.bounds.min.y : transform.position.y;
    
    public GameObject CurrentCell { get; set; }
    public Player Owner { get; set; }
    private bool _isHighlighted;

    private void Start()
    {
        CurrentCell = Owner.SpawnLocation;
        Debug.Log($"{this} is tied to cell {CurrentCell}");
        transform.position = CurrentCell.GetComponent<CellData>().GetSnapPosition(gameObject, CurrentCell);
        Debug.Log($"{this} is snapped to {transform.position}");
        CurrentCell.GetComponent<CellData>().AddOccupant(gameObject);
        Debug.Log(CurrentCell.GetComponent<CellData>().TopPiece);
    }

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

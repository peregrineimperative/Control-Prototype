using System.Buffers;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePiece : MonoBehaviour
{
    [SerializeField] private Collider pieceCollider;
    [SerializeField] private Renderer renderer;
    public float PieceTopY => pieceCollider != null ? pieceCollider.bounds.max.y : transform.position.y;
    
    public GameObject CurrentCell { get; set; }
    public Player Owner { get; private set; }
    private bool _isHighlighted;

    private void Start()
    {
        
    }
    
    //on move to cell
    //paint cell
    //snap to highest center point.
    
}

using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Game piece responsible for spawning other game pieces on click.
/// </summary>
public class SpawnPoint : PieceParent, IClickable
{
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private int unitCost;
    
    protected override void Start()
    {
        base.Start();
        CurrentCell.baseObject = gameObject;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!ControlsEnabled) return; //Enabled only during player's turn
        
        Debug.Log("Clicked");
        
        if (Owner == null)
        {
            Debug.LogError($"{name}: No Player found in parents. Cannot spawn piece.");
            return;
        }

        //Try to spawn a piece to check if the player has adequate energy to do so.
        if (!Owner.TrySpendEnergy(unitCost))
        {
            Debug.LogError($"{name}: Not enough energy to spawn piece.");
            return;
        }
        
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        var newPiece = Instantiate(piecePrefab, transform.position, Quaternion.identity);
        
        //Make sure the newly instantiated piece shares the same owner as the spawn point.
        newPiece.GetComponent<GamePiece>().Owner = Owner;
        newPiece.GetComponent<GamePiece>().CurrentCell = CurrentCell;
    }
}

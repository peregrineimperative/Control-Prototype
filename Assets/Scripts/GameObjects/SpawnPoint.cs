using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Game piece responsible for spawning other game pieces on click.
/// </summary>
public class SpawnPoint : PieceParent, IClickable
{
    [SerializeField] private GameObject piecePrefab;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        
        if (Owner == null)
        {
            Debug.LogError($"{name}: No Player found in parents. Cannot spawn piece.");
            return;
        }
        
        var newPiece = Instantiate(piecePrefab, transform.position, Quaternion.identity);
        
        //Make sure the newly instantiated piece shares the same owner as the spawn point.
        newPiece.GetComponent<GamePiece>().Owner = Owner;
    }
}

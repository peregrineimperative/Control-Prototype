using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnPoint :  GamePiece, IClickable
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
        
        newPiece.GetComponent<GamePiece>().Owner = Owner;
    }
}

using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Contains player data.
/// Current energy, cells owned, active game pieces, etc.
/// </summary>
public class Player : MonoBehaviour
{
    // Prefabs
    [SerializeField] private GameObject spawnPointPrefab;
    [SerializeField] private GameObject gamePiecePrefab;
    
    public int Energy { get; set; }
    public BoardCell SpawnLocation { get; set; }
    public ColorTheme ColorTheme { get; set; }
    public List<GamePiece> GamePieces { get; set; }
    //private List<BoardCell> ownedCells;
    
    
    void Start()
    {
        GameObject playerSpawnPoint = Instantiate(spawnPointPrefab, transform.position, Quaternion.identity);
        playerSpawnPoint.GetComponent<SpawnPoint>().Owner = this;
        SpawnLocation.baseObject = playerSpawnPoint;
        Debug.Log($"Spawn base: {SpawnLocation.baseObject}");
    }
    
    private void TurnStart()
    {
        HasControls(true);
        Energy = PlayerManager.Instance.maxEnergy;
    }
    private void TurnEnd()
    {
        HasControls(false);
    }

    private void HasControls( bool isTurn)
    {
        foreach (var piece in GamePieces)
        {
            piece.ControlsEnabled = isTurn;
        }
    }
    
}

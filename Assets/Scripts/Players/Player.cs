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

    private void TrySpendEnergy(int energyCost)
    {
        if (Energy >= energyCost)
        {
            Energy -= energyCost;
            if (Energy <= 0)
            {
                TurnEnd();
            }
        }
        else
        {
            Debug.LogError("Not enough energy to perform action.");
        }
    }
    
    public void TurnStart()
    {
        HasControls(true);
        Energy = PlayerManager.Instance.maxEnergy;
    }
    private void TurnEnd()
    {
        HasControls(false);
        PlayerManager.Instance.GoToNextPlayer();
    }

    private void HasControls( bool isTurn)
    {
        foreach (var piece in GamePieces)
        {
            piece.ControlsEnabled = isTurn;
        }
    }
    
}

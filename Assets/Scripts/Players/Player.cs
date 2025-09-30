using System;
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
    
    public string Name { get; set; }
    public int Energy { get; set; }
    public BoardCell SpawnLocation { get; set; }
    
    public SpawnPoint SpawnPoint { get; set; }
    public ColorTheme ColorTheme { get; set; }
    public List<GamePiece> GamePieces { get; set; } //Keep track of pieces to 
    
    //public bool IsTurn { get; set; }
    //private List<BoardCell> ownedCells;

    private void Awake()
    {
        //Call this in Awake to make sure it can be populated.
        GamePieces = new List<GamePiece>();
    }

    void Start()
    {
        GameObject playerSpawnPoint = Instantiate(spawnPointPrefab, transform.position, Quaternion.identity);
        playerSpawnPoint.GetComponent<SpawnPoint>().Owner = this;
        //SpawnLocation.baseObject = playerSpawnPoint;
        //GamePieces.Add(playerSpawnPoint.GetComponent<SpawnPoint>());
        SpawnPoint = playerSpawnPoint.GetComponent<SpawnPoint>();
        Debug.Log($"Spawn base: {SpawnLocation.baseObject}");
    }

    public bool TrySpendEnergy(int energyCost)
    {
        if (Energy >= energyCost)
        {
            Energy -= energyCost;
            Debug.Log($"{Name} spent {energyCost} energy.");
            Debug.Log($"Remaining energy: {Energy}");
            if (Energy <= 0)
            {
                TurnEnd();
            }

            return true;
        }
        else
        {
            Debug.LogError("Not enough energy to perform action.");
            return false;
        }
    }
    
    public void TurnStart(int energyReplenished)
    {
        HasControls(true);
        Energy = energyReplenished;
        Debug.Log($"{Name} is starting turn with {Energy} energy.");
    }
    private void TurnEnd()
    {
        HasControls(false);
        PlayerManager.Instance.GoToNextPlayer();
    }

    private void HasControls(bool isTurn)
    {
        foreach (var piece in GamePieces)
        {
            piece.ControlsEnabled = isTurn;
        }
        
        SpawnPoint.ControlsEnabled = isTurn;
    }
    
}

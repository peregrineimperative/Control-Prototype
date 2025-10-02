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
    public event Action<int> EnergyChanged; 
    
    //public bool IsTurn { get; set; }
    //private List<BoardCell> ownedCells;

    private void Awake()
    {
        //Call this in Awake to make sure it can be populated.
        GamePieces = new List<GamePiece>();
    }

    void Start()
    {
        //Give the player a spawn point
        GameObject playerSpawnPoint = Instantiate(spawnPointPrefab, transform.position, Quaternion.identity);
        playerSpawnPoint.GetComponent<SpawnPoint>().Owner = this;
        //SpawnLocation.baseObject = playerSpawnPoint;
        //GamePieces.Add(playerSpawnPoint.GetComponent<SpawnPoint>());
        SpawnPoint = playerSpawnPoint.GetComponent<SpawnPoint>();
        Debug.Log($"Spawn base: {SpawnLocation.baseObject}");
    }

    //Attempt to spend energy when moving or spawning a new guy
    public bool TrySpendEnergy(int energyCost)
    {
        if (Energy >= energyCost)
        {
            Energy -= energyCost;
            EnergyChanged?.Invoke(Energy);
            Debug.Log($"{Name} spent {energyCost} energy.");
            Debug.Log($"Remaining energy: {Energy}");
            
            //Turn ends at zero energy. What else are you gonna do?
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
    
    //At start of turn, player gets controls, and goes back to max energy
    public void TurnStart(int energyReplenished)
    {
        HasControls(true);
        Energy = energyReplenished;
        EnergyChanged?.Invoke(Energy);
        Debug.Log($"{Name} is starting turn with {Energy} energy.");
    }
    
    //At end of turn, player loses controls, and has the playermanager go oto next player.
    private void TurnEnd()
    {
        HasControls(false);
        PlayerManager.Instance.GoToNextPlayer();
    }

    //Activate/deactivate contol of all pieces controlled by the player
    private void HasControls(bool isTurn)
    {
        foreach (var piece in GamePieces)
        {
            piece.ControlsEnabled = isTurn;
        }
        
        SpawnPoint.ControlsEnabled = isTurn;
    }
}

using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Singleton manager responsible for instantiating and maintaining players/turn structure
/// </summary>
public class PlayerManager : MonoBehaviour {
    
    //Player instantiation data
    [SerializeField] private Player playerPrefab;
    [SerializeField] private List<Player> players;
    [SerializeField] private List<ColorTheme> colorThemes;
    
    //Variables to test out; playerCount could be set in a menu later on.
    [SerializeField] private int playerCount;
    [SerializeField] public int maxEnergy;
    [SerializeField] private int maxRounds;
    
    //Turn management variables
    private Player ActivePlayer { get; set; }
    private int RoundCount { get; set; }
    
    
    //---Singleton Setup---
    #region Singleton Setup
    public static PlayerManager Instance {get; private set;}
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    
    private void Start()
    {
        for (int i = 0; i< playerCount; i++)
        {
            SpawnPlayer(i);
        }
        
        ActivePlayer = players[0];
        RoundCount = 0;
    }

    private Player SpawnPlayer(int index)
    {
        //Pick a random color theme for a new player
        int themeIndex = Random.Range(0, colorThemes.Count);
        var theme = colorThemes[themeIndex];
        colorThemes.RemoveAt(themeIndex);
        
        //Get spawn location from the grid manager
        var spawnLocation = GridManager.Instance.StartingPositions[index];
        
        //Create a new player
        var player = Instantiate(
            playerPrefab,
            spawnLocation.transform.position, 
            Quaternion.identity);
        player.SpawnLocation = spawnLocation.GetComponent<BoardCell>();;
        player.ColorTheme = theme;
        player.Energy = maxEnergy;
        //player.PlayerNumber = players.Count + 1;
        
        players.Add(player);
        
        return player;
    }
    
    //---Turn Management---

    private void RoundCounter()
    {
        for (int i = 0; i < maxRounds; i++)
        {
            
        }
    }

    private void NewRound()
    {
        for (int i = 0; i < players.Count; i++)
        {
            
        }
    }
    
    //Check Active Player energy to call from energy spending functions.
    //Or TrySpendEnergy() in Player
        //If energy expenditure is valid, check remaining energy
        //If energy is zero, EndTurn()
        //End turn calls GoToNextPlayer() in PlayerManager
        //GoToNextPlayer() increments active player mod players.count
            //If activeplayerindex = 0, check if current round <= roundmax
            //If not, EndGame()
            //If we're not at roundmax, increment roundcount and call next player's StartTurn()
}

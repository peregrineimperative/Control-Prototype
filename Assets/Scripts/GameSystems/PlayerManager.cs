using System.Collections;
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
    [SerializeField] private int firstTurnEnergy;
    [SerializeField] public int maxEnergy;
    [SerializeField] private int maxRounds;
    
    //Turn management variables
    private Player ActivePlayer { get; set; }
    private int RoundCount { get; set; }
    private int ActivePlayerIndex { get; set; }
    
    
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
        //Instantiate players based on player count set in inspector (or menu someday maybe who knows)
        for (int i = 0; i< playerCount; i++)
        {
            SpawnPlayer(i);
        }
        
        //StartGame();
        StartCoroutine(StartGameNextFrame());
    }

    private IEnumerator StartGameNextFrame()
    {
        yield return null;
        StartGame();
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

    //Begin turn order
    private void StartGame()
    {
        ActivePlayerIndex = 0;
        RoundCount = 0;
        Debug.Log($"Starting round {RoundCount + 1} with player {ActivePlayerIndex} as active player.");
        
        
        players[ActivePlayerIndex].TurnStart(firstTurnEnergy);

        foreach (var player in players)
        {
            //Each SpawnPoint should create a GamePiece here
            player.SpawnPoint.SpawnPiece();
        }
    }
    
    public void GoToNextPlayer()
    {
        //Cycle through players
        ActivePlayerIndex = (ActivePlayerIndex + 1) % players.Count;
        
        
        //If we're back to the first player, increment the round
        if (ActivePlayerIndex == 0)
        {
            RoundCount++;
            Debug.Log($"Starting round {RoundCount}");
            
            //When starting through player list again, check to see if the game needs ot end based on round count.
            if (RoundCount >= maxRounds)
            {
                EndGame();
                return;
            }
        }

        //Start next turn
        
        players[ActivePlayerIndex].TurnStart(maxEnergy);
        
        Debug.Log($"Starting round {RoundCount + 1} with player {ActivePlayerIndex} as active player.");
        
    }

    public void EndGame()
    {
        //Tally scores, display winner yay
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

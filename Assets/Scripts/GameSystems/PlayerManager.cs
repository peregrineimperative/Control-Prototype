using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Singleton manager responsible for instantiating and maintaining players/turn structure
/// </summary>
public class PlayerManager : MonoBehaviour {
    
    [SerializeField] private Player playerPrefab;
    [SerializeField] private List<Player> players;
    [SerializeField] private List<ColorTheme> colorThemes;
    
    
    //Variables to test out; playerCount could be set in a menu later on.
    [SerializeField] private int playerCount;
    [SerializeField] private int maxEnergy;
    
    
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
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        for (int i = 0; i< playerCount; i++)
        {
            SpawnPlayer(i);
        }
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
        player.PlayerNumber = players.Count + 1;
        
        players.Add(player);
        
        return player;
    }
    
    //Need turn management
    //Swap players when out of energy
    
}

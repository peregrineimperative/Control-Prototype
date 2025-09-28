using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Contains player data.
/// Current energy, cells owned, active game pieces, etc.
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private GameObject spawnPointPrefab;
    [SerializeField] private GameObject gamePiecePrefab;
    
    public int PlayerNumber { get; set; }
    public int Score { get; private set; }
    public int Energy { get; set; }
    public GameObject SpawnLocation { get; set; }
    public ColorTheme ColorTheme { get; set; }
    private List<GameObject> gamePieces = new List<GameObject>();
    private List<GameObject> ownedCells = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerSpawnPoint = Instantiate(spawnPointPrefab, transform.position, Quaternion.identity);
        playerSpawnPoint.GetComponent<SpawnPoint>().Owner = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TurnEnd()
    {
        //Deactivate controls
        //Reset energy
    }
}

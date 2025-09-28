using UnityEngine;

/// <summary>
/// Singleton manager responsible for setting up the game/other managers in proper order.
/// </summary>

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject gridManagerPrefab;
    [SerializeField] private GameObject playerManagerPrefab;
    
    #region Singleton Setup
    public static GameManager Instance {get; private set;}
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
    
    //Grid Manager needs to exist before Player Manager, otherwise spawn locations will not exist.
    void Start()
    {
        GameObject gridManager = Instantiate(gridManagerPrefab);
        GameObject playerManager = Instantiate(playerManagerPrefab);
    }
}

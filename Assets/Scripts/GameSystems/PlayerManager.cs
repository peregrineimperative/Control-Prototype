using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private int currentPlayerIndex = 0;
    
    //Variables to test out; playerCount could be set in a menu later on.
    [SerializeField] private int playerCount;
    [SerializeField] private int maxEnergy;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    
}

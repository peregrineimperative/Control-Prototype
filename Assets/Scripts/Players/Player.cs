using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public int PlayerNumber { get; set; }
    public int Score { get; private set; }
    public int Energy { get; set; }
    public Color pieceColor;
    public Color paintColor;
    public Color highlightColor;
    private List<GameObject> ownedCells = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TurnEnd()
    {
        
    }
}

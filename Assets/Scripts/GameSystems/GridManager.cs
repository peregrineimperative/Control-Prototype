using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton manager responsible for instantiating the game board.
/// </summary>
public class GridManager : MonoBehaviour
{
    
    [Header("Grid Settings")] 
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridDepth;
    
    //List of all tiles in order of instantiation
    //Placed in a double array for ease of access (grid size should not change after instantiation so no need for list).
    public GameObject[,] Grid { get; private set; }
    
    public List<GameObject> StartingPositions { get; private set; }
    
    #region Singleton Setup
    public static GridManager Instance {get; private set;}
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
    
    void Start()
    {
        Grid = new GameObject[gridWidth, gridDepth];
        GenerateNewGrid(gridWidth, gridDepth);
        
        //Create spawn points
        StartingPositions = new List<GameObject>();
        StartingPositions.Add(Grid[0, 0]);
        StartingPositions.Add(Grid[gridWidth - 1, gridDepth - 1]);
        //Create towers
    }

    void GenerateNewGrid(int width, int depth)
    {
        //Iterate through all desired positions on the XZ plane
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                //Create a tile at all positions, then add that tile to the list.
                Grid[x, z] = CreateCell(cellPrefab, x, z, Quaternion.identity);
            }
        }
    }
    
    private GameObject CreateCell(GameObject cellType, int xPosition, int zPosition, Quaternion rotation)
    {
        Vector3 position = new Vector3(xPosition, .5f, zPosition);
        GameObject cell = Instantiate(cellType, position, rotation);
        var data = cell.GetComponent<CellData>();
        data.SetGridPosition(new Vector2Int(xPosition, zPosition));
        return cell;
    }
    
    //Create spawn points
    //Create towers
}
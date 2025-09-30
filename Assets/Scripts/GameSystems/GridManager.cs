using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton manager responsible for instantiating the game board.
/// </summary>
public class GridManager : MonoBehaviour
{
    
    [Header("Grid Settings")] 
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject towerPrefab;
    
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridDepth;
    
    //List of all tiles in order of instantiation
    //Placed in a double array for ease of access (grid size should not change after instantiation so no need for list).
    public GameObject[,] Grid { get; private set; }
    
    public List<GameObject> StartingPositions { get; private set; }
    
    //---Singleton Setup---
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
        SpawnTowers();
    }

    private void SpawnTowers()
    {
        /*ReadOnlySpan<(int dx, int dz)> towerPositions = stackalloc (int dx, int dz)[]
        {
            (1, 7),
            (3, 1),
            (7, 1),
            (4, 4),
            (5, 7)
        };*/

        Vector2Int[] towerPositions = new Vector2Int[5];
        towerPositions[0] = new Vector2Int(1, 7);
        towerPositions[1] = new Vector2Int(3, 1);
        towerPositions[2] = new Vector2Int(7, 1);
        towerPositions[3] = new Vector2Int(4, 4);
        towerPositions[4] = new Vector2Int(5, 7);
        
        foreach (Vector2Int position in towerPositions)
        {
            var cell = GridManager.Instance.Grid[position.x, position.y].GetComponent<BoardCell>();
            var tower = Instantiate(towerPrefab, cell.transform.position, Quaternion.identity);
            tower.GetComponent<Tower>().CurrentCell = cell;
            cell.Tower = tower.GetComponent<Tower>();
        }
    }
    
    //---Grid Builders---
    #region Grid Builders
    
    void GenerateNewGrid(int width, int depth)
    {
        //Iterate through all desired positions on the XZ plane
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                //Create a tile at all positions, then add that tile to the list.
                Grid[x, z] = CreateCell(cellPrefab, x, z, Quaternion.identity);
                //Grid[x, z].GetComponent<BoardCell>().baseObject = Grid[x, z];
            }
        }
    }
    
    //Make a new cell with a baked-in position that can be referenced.
    private GameObject CreateCell(GameObject cellType, int xPosition, int zPosition, Quaternion rotation)
    {
        Vector3 position = new Vector3(xPosition, .5f, zPosition);
        GameObject cell = Instantiate(cellType, position, rotation);
        var data = cell.GetComponent<BoardCell>();
        data.GridPosition = new Vector2Int(xPosition, zPosition);
        return cell;
    }
    #endregion
    
    //---Grid Navigation Methods---
    #region Grid Navigation Methods
    //Return a list of adjacent cells.
    //If orth is true, only the ones orthogonal to the desired cell; otherwise, all eight surrounding cells.
    public List<BoardCell> GetAdjacentCells(BoardCell start, bool orth = true)
    {
        List<BoardCell> cells = new List<BoardCell>();
        
        Vector2Int cellPosition = start.GridPosition;
        
        //iterate over x-values between one less and one more than cellPosition.X
        for (int dx = -1; dx <= 1; dx++)
        {
            //Do the same for z-values
            for (int dz = -1; dz <= 1; dz++)
            {
                //Skip over self
                if (dx == 0 && dz == 0) continue;
                //Skip diagonals if only orthogonals are desired
                if (orth && (Mathf.Abs(dx) + Mathf.Abs(dz) != 1)) continue;
                
                int x = cellPosition.x + dx;
                int z = cellPosition.y + dz; //Note that we're using a Vector2, thus the .y, even though it's z, I don't know what you want from me
                
                GameObject adjCell = Grid[x, z];
                if (adjCell == null) continue; //Make sure that it's a valid cell (i.e., if you use this on an edge/corner cell)
                
                cells.Add(adjCell.GetComponent<BoardCell>());
            }
        }
        
        return cells;
    }

    //Breadth-first search to return valid cells based on a given number of steps.
    public Dictionary<BoardCell, int> GetReachableCells(BoardCell start, int maxSteps)
    {
        var reachableCells = new Dictionary<BoardCell, int>();
        
        //Make sure that this is a valid thing to do, as unlikely as these conditions are.
        if (start == null || maxSteps <= 0 || Grid == null) return reachableCells;
        
        //Initializing a distance grid to -1, representing unvisited cells.
        int[,] distance = new int[gridWidth, gridDepth];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                distance[x, z] = -1;
            }
        }
        
        //Make sure we're starting within the bounds of the board. (Shouldn't be possible to do otherwise, but things like to break)
        Vector2Int startPosition = start.GridPosition;
        if(startPosition.x < 0 | startPosition.x >= gridWidth || startPosition.y < 0 || startPosition.y >= gridDepth) return reachableCells;
        
        //Initializing a queue for holding valid positions as we go. 
        var positionQueue = new Queue<Vector2Int>(64);
        
        //Starting our search for valid positions from the initial position.
        distance[startPosition.x, startPosition.y] = 0;
        positionQueue.Enqueue(startPosition);
        
        //Orthogonal directions to check around each already-validated cell.
        ReadOnlySpan<(int dx, int dz)> directions = stackalloc (int dx, int dz)[] {(1,0), (-1, 0), (0, 1), (0, -1)};

        while (positionQueue.Count > 0)
        {
            
            Vector2Int pos = positionQueue.Dequeue(); //Check the next item in the queue
            
            int dist = distance[pos.x, pos.y]; //Record the distance from the start position
            
            if (dist == maxSteps) continue; 
            
            //Iterating through each cardinal direction from the given position
            foreach ((int dx, int dz) in directions)
            {
                int x = pos.x + dx;
                int z = pos.y + dz; //Again, the .y is because we're using Vector2 stuff, etc. etc.
                
                if (x < 0 || x >= gridWidth || z < 0 || z >= gridDepth) continue; //Skip if out of bounds
                
                if (distance[x, z] != -1) continue; //Skip if already visited
                
                if (Grid[x, z] == null) continue; //Skip if there is somehow no cell at that position
                
                BoardCell cell = Grid[x, z].GetComponent<BoardCell>(); //Get the cell's data at that position
                
                distance[x, z] = dist + 1; //Record the distance to this cell from origin
                
                positionQueue.Enqueue(new Vector2Int(x, z)); //Since this cell is valid, add it to the queue to process again
                
                reachableCells.Add(cell, distance[x,z]); //Add this cell to the dictionary of reachable cells, with the distance as the value.
            }
        }
        
        return reachableCells; //Behold, a dictionary of valid cells and their distances.
    }
    #endregion
}
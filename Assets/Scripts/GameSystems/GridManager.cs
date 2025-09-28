using UnityEngine;
using System.Collections.Generic;
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
    
    void Start()
    {
        Grid = new GameObject[gridWidth, gridDepth];
        GenerateNewGrid(gridWidth, gridDepth);
        //Create spawn points
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
                Grid[x, z] = CreateCell(cellPrefab, x, z, 0, Quaternion.identity);
            }
        }
    }
    
    
    private GameObject CreateCell(GameObject cellType, int xPosition, int zPosition, float yLayer, Quaternion rotation)
    {
        Vector3 position = new Vector3(xPosition, yLayer + .5f, zPosition);
        GameObject cell = Instantiate(cellType, position, rotation);
        var data = cell.GetComponent<CellData>();
        data.SetGridPosition(new Vector2Int(xPosition, zPosition));
        return cell;
    }
    
    //Create spawn points
    //Create towers
}
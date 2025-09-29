using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class to contain utility functions pertaining to changing colors.
/// 
/// </summary>
public static class ColorHelper
{
    public static void SetBaseColor(this Renderer renderer, Color color)
    {
        if(renderer == null) return;
        
        //Using MaterialPropertyBlock allows for changing an individual object's rendering,
        //since we don't want to change the color of every object using a given material.
        var mbp = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mbp);
        mbp.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(mbp);
    }

    public static Color StepGradient(int steps, int maxSteps)
    {
        float t = maxSteps > 1 ? Mathf.Clamp01((steps - 1f) / (maxSteps - 1f)) : 0f;
        Color32 near = new Color32(150, 250, 70, 255);
        Color32 far = new Color32(250, 150, 70, 255);
        return Color.Lerp(near, far, t);
    }
    
    public static List<BoardCell> HighlightReachableCells(Dictionary<BoardCell, int> cells, int maxSteps)
    {
        var distances = cells;
        var highlightedCells = new List<BoardCell>(distances.Count);
        
        if (distances.Count == 0) return highlightedCells;

        foreach (var distance in distances)
        {
            var cell = distance.Key;
            int steps = distance.Value;
            
            Color color = StepGradient(steps, maxSteps);
            cell.SetMoveHighlight(color);
            highlightedCells.Add(cell);
        }
        
        return highlightedCells;
    }

    public static void ClearHighlights(IEnumerable<BoardCell> cells)
    {
        if (cells == null) return;
        foreach (var cell in cells)
        {
            if (cell != null) cell.ClearMoveHighlight();
        }
    }
    
}

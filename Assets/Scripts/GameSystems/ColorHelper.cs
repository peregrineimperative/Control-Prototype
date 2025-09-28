using UnityEngine;

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
}

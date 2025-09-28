using UnityEngine;

/// <summary>
/// Color schemes for a given player
/// </summary>
[CreateAssetMenu(fileName = "ColorTheme", menuName = "Scriptable Objects/ColorTheme")]
public class ColorTheme : ScriptableObject
{
    public Color pieceColor;
    public Color paintColor;
    public Color highlightColor;
}

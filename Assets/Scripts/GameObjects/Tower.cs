using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script to hold Tower data.
/// Note: need to refactor PieceParent/SpawnPoint/GamePiece to allow for better inheritance structure, since this is just going to be out on its own.
/// </summary>
public class Tower : MonoBehaviour
{
    public BoardCell CurrentCell { get; set; }
    public Player Owner { get; set; }

    private List<BoardCell> _influencedCells;
    private bool _previewActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Owner = null;
        CurrentCell.baseObject = gameObject;
        transform.position = CurrentCell.GetSnapPosition(gameObject);

        _influencedCells = GridManager.Instance.GetAdjacentCells(CurrentCell, false);
    }

    public void PreviewInfluence(Player previewOwner)
    {
        _previewActive = true;

        foreach (BoardCell cell in _influencedCells)
        {
            cell.TowerOwner = previewOwner;
            cell.PreviewOwnership(null);
        }
    }

    public void ClearPreview()
    {
        if (!_previewActive) return;
        _previewActive = false;

        foreach (BoardCell cell in _influencedCells)
        {
            cell.TowerOwner = Owner;
            cell.RefreshPaint();
        }
    }

    public void ApplyInfluence()
    {
        //Tower influence neutralized if tower control is neutralized
        if (Owner == null)
        {
            foreach (BoardCell cell in _influencedCells)
            {
                cell.TowerOwner = Owner;
                if (cell.IsOccupied)
                {
                    cell.FinalizeOwnership(null);
                }
                else
                {
                    cell.CurrentOwner = null;
                    cell.RefreshPaint();
                }

            }
        }
        //Tower influences surroundings if owned.
        else
        {
            foreach (BoardCell cell in _influencedCells)
            {
                cell.TowerOwner = Owner;
                cell.FinalizeOwnership(null);
            }
        }
    }
}

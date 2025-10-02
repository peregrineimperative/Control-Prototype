using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image panelBackground;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text controlsText;

    private Player _activePlayer;

    private void OnEnable()
    {
        PlayerManager.Instance.BeginTurn += HandleBeginTurn;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.BeginTurn -= HandleBeginTurn;

        if (_activePlayer != null)
        {
            _activePlayer.EnergyChanged -= HandleEnergyChanged;
        }
    }

    private void HandleBeginTurn(Player nextPlayer, int round)
    {
        if (_activePlayer != null)
        {
            _activePlayer.EnergyChanged -= HandleEnergyChanged;
        }
        
        _activePlayer = nextPlayer;

        if (_activePlayer != null)
        {
            _activePlayer.EnergyChanged += HandleEnergyChanged;
        }
        
        //Update text
        playerNameText.text = _activePlayer != null ? _activePlayer.Name : "No Player";
        roundText.text = $"Round {round}";
        
        var baseColor = _activePlayer != null ? _activePlayer.ColorTheme.paintColor : Color.white;
        panelBackground.color = baseColor;
        
        //Energy displkay should update on turn start
        HandleEnergyChanged(_activePlayer.Energy);
    }

    private void HandleEnergyChanged(int energy)
    {
        int max = PlayerManager.Instance.maxEnergy;
        energyText.text = $"{energy}/{max}";
    }
}

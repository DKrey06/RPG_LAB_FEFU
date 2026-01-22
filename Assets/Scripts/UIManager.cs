using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _mpText;
    [SerializeField] private Text _levelText;

    private PlayerStats _playerStats;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerStats = player.GetComponent<PlayerStats>();
            if (_playerStats != null)
            {
                _playerStats.OnStatsChanged += UpdateUI;
                UpdateUI(); 
            }
        }
    }

    private void UpdateUI()
    {
        if (_playerStats == null) return;

        _hpText.text = $"HP: {_playerStats.CurrentHP}/{_playerStats.MaxHP}";
        _mpText.text = $"MP: {_playerStats.CurrentMP}/{_playerStats.MaxMP}";
        _levelText.text = $"LVL: {_playerStats.Level}";
    }

    void OnDestroy()
    {
        if (_playerStats != null)
            _playerStats.OnStatsChanged -= UpdateUI;
    }
}
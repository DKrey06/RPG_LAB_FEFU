using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _mpText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _xpText;

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
                _playerStats.OnLevelUp += OnLevelUp;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        if (_playerStats == null) return;

        _hpText.text = $"HP: {_playerStats.CurrentHP}/{_playerStats.MaxHP}";
        _mpText.text = $"MP: {_playerStats.CurrentMP}/{_playerStats.MaxMP}";
        _levelText.text = $"Уровень: {_playerStats.Level}";

        if (_xpText != null)
        {
            _xpText.text = $"Опыт: {_playerStats.CurrentXP}/{_playerStats.XPToNextLevel}";
        }
    }

    private void OnLevelUp(int newLevel)
    {
        Debug.Log($"Поздравляем! Вы достигли {newLevel} уровня!");
        UpdateUI();
    }

    void OnDestroy()
    {
        if (_playerStats != null)
        {
            _playerStats.OnStatsChanged -= UpdateUI;
            _playerStats.OnLevelUp -= OnLevelUp;
        }
    }
}
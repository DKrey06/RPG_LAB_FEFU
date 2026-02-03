using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _mpText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _xpText;
    [SerializeField] private GameObject _playerObject; //Добавлю явную ссылку на игрока

    private PlayerStats _playerStats;

    void Start()
    {
        if (_playerObject == null)
        {
            _playerObject = GameObject.FindGameObjectWithTag("Player");

            if (_playerObject == null)
            {
                _playerObject = GameObject.Find("Player");
            }
        }

        if (_playerObject != null)
        {
            _playerStats = _playerObject.GetComponent<PlayerStats>();

            if (_playerStats != null)
            {
                _playerStats.OnStatsChanged += UpdateUI;
                _playerStats.OnLevelUp += OnLevelUp;
                Debug.Log("UIManager успешно подключился к PlayerStats");
                UpdateUI();
            }
            else
            {
                Debug.LogError("PlayerStats не найден на объекте игрока!");
            }
        }
        else
        {
            Debug.LogError("Игрок не найден в сцене!");
        }

        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        yield return null;

        if (_playerStats == null && _playerObject == null)
        {
            _playerObject = GameObject.FindGameObjectWithTag("Player");

            if (_playerObject != null)
            {
                _playerStats = _playerObject.GetComponent<PlayerStats>();

                if (_playerStats != null)
                {
                    _playerStats.OnStatsChanged += UpdateUI;
                    _playerStats.OnLevelUp += OnLevelUp;
                    UpdateUI();
                    Debug.Log("UIManager инициализирован с задержкой");
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (_playerStats == null)
        {
            Debug.LogWarning("PlayerStats is null in UpdateUI");
            return;
        }

        if (_hpText != null)
            _hpText.text = $"HP: {_playerStats.CurrentHP}/{_playerStats.MaxHP}";

        if (_mpText != null)
            _mpText.text = $"MP: {_playerStats.CurrentMP}/{_playerStats.MaxMP}";

        if (_levelText != null)
            _levelText.text = $"Уровень: {_playerStats.Level}";

        if (_xpText != null)
            _xpText.text = $"Опыт: {_playerStats.CurrentXP}/{_playerStats.XPToNextLevel}";
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

    // Дополнительный метод для ручного обновления ссылок
    public void Reinitialize()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        if (_playerObject != null)
        {
            _playerStats = _playerObject.GetComponent<PlayerStats>();
            if (_playerStats != null)
            {
                _playerStats.OnStatsChanged += UpdateUI;
                _playerStats.OnLevelUp += OnLevelUp;
                UpdateUI();
            }
        }
    }
}
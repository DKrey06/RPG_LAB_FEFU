using UnityEngine;
using System;

[Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Основные характеристики")]
    [SerializeField] private int maxHP = 50;
    [SerializeField] private int maxMP = 50;
    [SerializeField] private int strength = 10;
    [SerializeField] private int dexterity = 10;

    [Header("Броня")]
    [SerializeField] private int armorLevel = 1;
    public int ArmorLevel
    {
        get => armorLevel;
        set
        {
            armorLevel = Mathf.Clamp(value, 1, 5);
            OnStatsChanged?.Invoke();
        }
    }

    [Header("Уровень и опыт")]
    [SerializeField] private int xpToNextLevelBase = 100;
    [SerializeField] private float xpMultiplierPerLevel = 1.5f;

    [Header("Увеличение статов за уровень")]
    [SerializeField] private int hpPerLevel = 10;
    [SerializeField] private int mpPerLevel = 5;
    [SerializeField] private int strengthPerLevel = 2;
    [SerializeField] private int dexterityPerLevel = 2;
    [SerializeField] private int defensePerLevel = 1;

    private int currentHP;
    private int currentMP;
    private int level = 1;
    private int currentXP = 0;
    private int xpToNextLevel;

    private PlayerAnimatorController animController;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public int CurrentMP => currentMP;
    public int MaxMP => maxMP;
    public int Level => level;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;
    public int Strength => strength;
    public int Dexterity => dexterity;
    public int Defense => armorLevel * 2;

    public event System.Action OnStatsChanged;
    public event System.Action<int> OnLevelUp;

    private void Awake()
    {
        var existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null && existingPlayer != this.gameObject)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        xpToNextLevel = CalculateXPForLevel(level + 1);

        animController = GetComponent<PlayerAnimatorController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();

        Debug.Log("PlayerStats инициализирован");
        OnStatsChanged?.Invoke();
    }

    public void AddExperience(int xpAmount)
    {
        if (xpAmount <= 0) return;

        currentXP += xpAmount;
        Debug.Log($"Получено {xpAmount} опыта. Всего {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel && level < 100)
        {
            LevelUp();
        }

        OnStatsChanged?.Invoke();
    }

    private void LevelUp()
    {
        level++;
        int excessXP = currentXP - xpToNextLevel;
        currentXP = excessXP;

        xpToNextLevel = CalculateXPForLevel(level + 1);

        maxHP += hpPerLevel;
        maxMP += mpPerLevel;
        strength += strengthPerLevel;
        dexterity += dexterityPerLevel;
        armorLevel = Mathf.Min(5, armorLevel + 1);

        currentHP = maxHP;
        currentMP = maxMP;

        Debug.LogWarning($"Уровень повышен! Текущий уровень: {level}");
        Debug.LogWarning($"Характеристики: HP +{hpPerLevel}, MP +{mpPerLevel}, Сила +{strengthPerLevel}, Ловкость +{dexterityPerLevel}");

        OnLevelUp?.Invoke(level);
        OnStatsChanged?.Invoke();
    }

    private int CalculateXPForLevel(int targetLevel)
    {
        if (targetLevel <= 1) return 0;
        return Mathf.RoundToInt(xpToNextLevelBase * Mathf.Pow(xpMultiplierPerLevel, targetLevel - 2));
    }

    public float GetLevelProgress()
    {
        if (level >= 100) return 1f;

        int xpForCurrentLevel = CalculateXPForLevel(level);
        int xpEarnedThisLevel = currentXP;
        int xpNeededThisLevel = xpToNextLevel - xpForCurrentLevel;

        return Mathf.Clamp01((float)xpEarnedThisLevel / xpNeededThisLevel);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHP <= 0 || animController?.IsDead() == true) return;

        int actualDamage = Mathf.Max(1, damage - Defense);

        currentHP = Mathf.Max(0, currentHP - actualDamage);
        Debug.Log($"Получено {actualDamage} урона. HP: {currentHP}/{maxHP}");

        if (animController != null)
        {
            animController.TriggerHurt();
        }

        OnStatsChanged?.Invoke();

        if (currentHP == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Игрок погиб!");
        currentXP = Mathf.Max(0, currentXP - (int)(currentXP * 0.1f));

        if (animController != null)
        {
            animController.TriggerDeath();
        }

        OnStatsChanged?.Invoke();
        ShowGameOverScreen();
    }

    private void ShowGameOverScreen()
    {
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.ForceGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager не найден в сцене!");
            if (playerMovement != null) playerMovement.enabled = false;
            if (playerCombat != null) playerCombat.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
        Debug.Log($"Восстановлено {amount} HP. Текущее: {currentHP}/{maxHP}");
        OnStatsChanged?.Invoke();
    }

    public void RestoreMP(int amount)
    {
        if (amount <= 0) return;

        currentMP = Mathf.Min(maxMP, currentMP + amount);
        Debug.Log($"Восстановлено {amount} MP. Текущее: {currentMP}/{maxMP}");
        OnStatsChanged?.Invoke();
    }


    public void TestTakeDamage() => TakeDamage(5);
    public void TestAddXP() => AddExperience(50);
    public void TestHeal() => Heal(10);
}
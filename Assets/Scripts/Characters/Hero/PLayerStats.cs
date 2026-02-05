using UnityEngine;
using System;

[Serializable]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int maxHP = 50; //приватные свойсвта, которые юнити может сохранять и загружать даже под private 
    [SerializeField] private int maxMP = 50;
    [SerializeField] private int strength = 10;
    [SerializeField] private int dexterity = 10;
    [SerializeField] private int defense = 10;
    [SerializeField] private int xpToNextLevelBase = 100;
    [SerializeField] private float xpMultiplierPerLevel = 1.5f; //множитель опыта для следующего уровня

    //Повышение основных статов на эти параметры ниже при повышении уровня
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
    public int Defense => defense;

    public event System.Action OnStatsChanged;
    public event System.Action<int> OnLevelUp; //событие при повышении уровня, то есть передает новый уровень

    private void Awake()
    {
        //Singleton - только один игрок на сцену
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

    //Получение опыта
    public void AddExperience(int xpAmout)
    {
        if (xpAmout <= 0) return;

        currentXP += xpAmout;
        Debug.Log($"Получено {xpAmout} опыта. Всего {currentXP}/{xpToNextLevel}");

        //Посмотрим хватает ли чтобы повысить уровень
        while (currentXP >= xpToNextLevel && level < 100)//Ограничим макс уровень 100 пока что
        {
            LevelUp();
        }
        OnStatsChanged?.Invoke();
    }

    //Повышение уровня
    private void LevelUp()
    {
        level++;
        int excessXP = currentXP - xpToNextLevel;
        currentXP = excessXP;

        xpToNextLevel = CalculateXPForLevel(level + 1); //рассчет необходимого опыта для след. уровня

        //Повышаем статы
        maxHP += hpPerLevel;
        maxMP += mpPerLevel;
        strength += strengthPerLevel;
        dexterity += dexterityPerLevel;
        defense += defensePerLevel;

        //Пусть при повышении уровня будет востанавливаться хп и маны
        currentHP = maxHP;
        currentMP = maxMP;

        Debug.LogWarning($"Уровень повышен! Текущий уровень: {level}");
        Debug.LogWarning($"Характеристики: HP +{hpPerLevel}, MP +{mpPerLevel}, Сила +{strengthPerLevel}, Ловкость +{dexterityPerLevel}, Защита +{defensePerLevel}");

        OnLevelUp?.Invoke(level);
        OnStatsChanged?.Invoke();
    }

    private int CalculateXPForLevel(int targetLevel)
    {
        if (targetLevel <= 1) return 0;
        return Mathf.RoundToInt(xpToNextLevelBase * Mathf.Pow(xpMultiplierPerLevel, targetLevel - 2)); //базовый опыт * множитель^(уровень-1)
    }

    public float GetLevelProgress() //для прогресса до след. уровня
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

        int actualDamage = Mathf.Max(1, damage - defense);

        currentHP = Mathf.Max(0, currentHP - actualDamage);
        Debug.Log($"Получено {actualDamage} урона. HP: {currentHP}/{maxHP}");

        //Анимация получения урона
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
        currentXP = Mathf.Max(0, currentXP - (int)(currentXP * 0.1f)); //потеря опыта при смерти

        //Анимация смерти
        if (animController != null)
        {
            animController.TriggerDeath();
        }

        //Отключаем управление
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerCombat != null) playerCombat.enabled = false;

        OnStatsChanged?.Invoke();
    }

    //Пока методы для востановления здоровья и маны нигде не используются, но в целом пригодится :)
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

    //Тестовые методы
    public void TestTakeDamage() => TakeDamage(5);
    public void TestAddXP() => AddExperience(50);
    public void TestHeal() => Heal(10);
}
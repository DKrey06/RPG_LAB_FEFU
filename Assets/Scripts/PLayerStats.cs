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

    private int currentHP;
    private int currentMP;
    private int level = 1;
    private int currentXP = 0;
    private int xpToNextLevel;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public int CurrentMP => currentMP;
    public int MaxMP => maxMP;
    public int Level => level;
    public int CurrentXP => currentXP;
    public int Strength => strength;
    public int Dexterity => dexterity;
    public int Defense => defense;

    public event System.Action OnStatsChanged;

    private void Awake()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        xpToNextLevel = xpToNextLevelBase;
        OnStatsChanged?.Invoke();

        var existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null && existingPlayer != this.gameObject)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

}

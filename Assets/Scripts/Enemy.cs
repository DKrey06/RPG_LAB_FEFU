using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtFlashTime = 0.2f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"¬раг получил {damageAmount} урона. ќсталось HP: {currentHealth}");

        // Ёффект получени€ урона
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
            Invoke(nameof(ResetColor), hurtFlashTime);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        Debug.Log("¬раг повержен!");
        Destroy(gameObject);
    }
}
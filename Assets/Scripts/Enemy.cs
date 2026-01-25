using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtFlashTime = 0.2f;

    [SerializeField] private int damageToPlayer = 10;
    [SerializeField] private float damageCooldown = 1f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float lastDamageTime;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Урон от игрока
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Враг получил {damageAmount} урона. Осталось HP: {currentHealth}");

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CanDealDamage())
        {
            var playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageToPlayer);
                lastDamageTime = Time.time;
                Debug.LogWarning("получен урон");
            }
        }
    }

    private bool CanDealDamage()
    {
        return Time.time >= lastDamageTime + damageCooldown;
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
        Debug.Log("Враг повержен!");
        Destroy(gameObject);
    }
}
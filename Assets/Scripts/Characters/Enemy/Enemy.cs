using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtFlashTime = 0.2f;

    [SerializeField] private int damageToPlayer = 10;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private int experienceReward = 20; //опыт за убийство врага 

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

    //Урон от игрока
    public void TakeDamage(int damageAmount)
    {
        //Анимация получения урона
        EnemyAnimator enemyAnim = GetComponent<EnemyAnimator>();
        if (enemyAnim != null) enemyAnim.TriggerHurt();

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
            //Анимация атаки
            EnemyAnimator enemyAnim = GetComponent<EnemyAnimator>();
            if (enemyAnim != null) enemyAnim.TriggerAttack();

            var playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageToPlayer);
                lastDamageTime = Time.time;
                Debug.LogWarning("Враг нанес урон игроку");
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

        //Анимация смерти
        EnemyAnimator enemyAnim = GetComponent<EnemyAnimator>();
        if (enemyAnim != null) enemyAnim.TriggerDeath();

        //Даем опыт игроку
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddExperience(experienceReward);
                Debug.Log($"Игрок получил {experienceReward} опыта!");
            }
        }

        //Уничтожаем через время для анимки
        Destroy(gameObject, 1.5f);
    }
}
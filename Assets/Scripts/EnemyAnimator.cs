using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("Компоненты")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyAI enemyAI;
    private Enemy enemy;
    private Rigidbody2D rb;

    [Header("Параметры анимации")]
    private float currentSpeed = 0f;
    private bool isAttacking = false;
    private bool isHurt = false;
    private bool isDead = false;

    void Start()
    {
        //Получаем все необходимые компоненты
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAI = GetComponent<EnemyAI>();
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            Debug.LogError("Animator не найден на враге!");
            animator = gameObject.AddComponent<Animator>();
        }

        Debug.Log("EnemyAnimator инициализирован");
    }

    void Update()
    {
        if (isDead) return;

        UpdateAnimations();
        UpdateSpriteFlip();
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        if (rb != null)
        {
            currentSpeed = rb.linearVelocity.magnitude;
        }

        animator.SetFloat("Speed", currentSpeed);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && !isDead)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            isAttacking = distanceToPlayer < 1.5f;
        }

        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsHurt", isHurt);
        animator.SetBool("IsDead", isDead);
    }

    void UpdateSpriteFlip()
    {
        if (spriteRenderer == null || rb == null) return;

        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0;
        }
    }

    //Методы
    //Враг получил урон
    public void TriggerHurt()
    {
        if (!isDead)
        {
            isHurt = true;
            Invoke(nameof(ResetHurt), 0.3f);
            Debug.Log("Анимация получения урона запущена");
        }
    }

    //Враг умер
    public void TriggerDeath()
    {
        if (isDead) return;

        isDead = true;

        if (enemyAI != null) enemyAI.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }


        Invoke(nameof(DisableColliders), 0.5f);

    }

    //Атака врага
    public void TriggerAttack()
    {
        if (!isDead)
        {
            isAttacking = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }


    void ResetHurt()
    {
        isHurt = false;
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void DisableColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
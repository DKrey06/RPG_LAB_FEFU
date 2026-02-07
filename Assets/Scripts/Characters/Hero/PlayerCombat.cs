using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Настройки атаки")]
    [SerializeField] private float attack_range = 1.2f;
    [SerializeField] private float attack_cooldown = 0.5f;
    [SerializeField] private int base_damage = 10;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attack_radius = 0.5f;

    [Header("Бафф силы")]
    [SerializeField] private int damageBuff = 0;
    [SerializeField] private float damageBuffEndTime = 0f;

    [Header("Отладка")]
    [SerializeField] private bool showAttackGizmo = true;
    [SerializeField] private Color gizmoColor = Color.yellow;
    [SerializeField] private bool alwaysShowInGame = true;

    private bool can_attack = true;
    private SpriteRenderer spriteRenderer;
    private Color original_color;
    private Vector2 lastAttackDirection = Vector2.right;

    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    private PlayerAnimatorController animController;
    private float lastAttackTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            original_color = spriteRenderer.color;
        }

        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
        animController = GetComponent<PlayerAnimatorController>();
    }

    void Update()
    {
        UpdateAttackDirection();

        if (Time.time > damageBuffEndTime && damageBuff > 0)
        {
            damageBuff = 0;
            Debug.Log("Действие зелья силы закончилось");
        }

        if (Input.GetKeyDown(KeyCode.Space) && can_attack == true)
        {
            Attack();
        }
    }

    void UpdateAttackDirection()
    {
        if (playerMovement != null)
        {
            Vector2 moveDir = playerMovement.GetLastMoveDirection();
            if (moveDir.magnitude > 0.1f)
            {
                lastAttackDirection = moveDir;
            }
        }
    }

    void Attack()
    {
        if (can_attack != true) return;

        can_attack = false;
        lastAttackTime = Time.time;
        Invoke(nameof(ResetAttack), attack_cooldown);

        if (animController != null)
        {
            animController.SetAttacking(true);
        }

        int finalDamage = base_damage + (playerStats?.Strength ?? 0) + damageBuff;
        Vector2 attackDirection = lastAttackDirection;
        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attack_radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
                Debug.LogWarning($"Попал! Урон: {finalDamage}");
            }
        }
    }

    public void AddDamageBuff(float duration, int bonus)
    {
        damageBuff = bonus;
        damageBuffEndTime = Time.time + duration;
        Debug.Log($"Получен бафф силы: +{bonus} урона на {duration} секунд");
    }

    void ResetAttack()
    {
        can_attack = true;

        if (animController != null)
        {
            animController.SetAttacking(false);
            Debug.Log("Атака завершена");
        }
    }

    public bool CanAttack() => can_attack;

    public int GetCurrentDamage()
    {
        return base_damage + (playerStats?.Strength ?? 0) + damageBuff;
    }

    void OnDrawGizmosSelected()
    {
        if (!showAttackGizmo) return;

        Vector2 attackDirection = Application.isPlaying ? lastAttackDirection : Vector2.right;
        Gizmos.color = gizmoColor;

        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range;
        Gizmos.DrawWireSphere(attackPosition, attack_radius);
        Gizmos.DrawLine(transform.position, attackPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, 0.05f);
    }

    void OnDrawGizmos()
    {
        if (!showAttackGizmo || !Application.isPlaying || !alwaysShowInGame) return;

        Vector2 attackDirection = lastAttackDirection;
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);

        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range;
        Gizmos.DrawWireSphere(attackPosition, attack_radius);
        Gizmos.DrawLine(transform.position, attackPosition);
    }
}
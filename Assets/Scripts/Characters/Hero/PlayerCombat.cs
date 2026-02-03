using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //Настройки атаки 
    [Header("Настройки атаки")]
    [SerializeField] private float attack_range = 1.2f;
    [SerializeField] private float attack_cooldown = 0.5f;
    [SerializeField] private int attack_damage = 10;
    [SerializeField] private LayerMask enemyLayer; //слой объектов-врагов
    [SerializeField] private Color attack_color = Color.red; //пока что назначю красным цветом
    [SerializeField] private float attack_flashTime = 0.3f; //смена цвета 
    [SerializeField] private float attack_radius = 0.5f; //параметр для радиуса атаки

    //Редактор
    [SerializeField] private bool showAttackGizmo = true;
    [SerializeField] private Color gizmoColor = Color.yellow;

    //Переменные
    private bool can_attack = true;
    private SpriteRenderer spriteRenderer;
    private Color original_color;
    private Vector2 lastAttackDirection = Vector2.right; //храним последнее направление

    void Start()
    {
        //Делаем рендер для смены спрайта, но пока что просто цвета
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            original_color = spriteRenderer.color;
        }
    }


    void Update()
    {
        UpdateAttackDirection();

        if (Input.GetKeyDown(KeyCode.Space) && can_attack == true)
        {
            Attack();
        }
    }

    void UpdateAttackDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            lastAttackDirection = new Vector2(horizontal, vertical).normalized;
        }
    }

    void Attack()
    {
        if (can_attack != true) return;

        can_attack = false;
        Invoke(nameof(ResetAttack), attack_cooldown);

        //Смена цвета
        if (spriteRenderer != null)
        {
            Debug.Log("Смена цвета");
            spriteRenderer.color = attack_color;
            Invoke(nameof(ResetColor), attack_flashTime);
        }

        //Используем последнее сохраненное направление
        Vector2 attackDirection = lastAttackDirection;
        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range;

        //Отладка
        Debug.Log($"Игрок позиция: {transform.position}");
        Debug.Log($"Направление атаки: {attackDirection}");
        Debug.Log($"Позиция атаки: {attackPosition}");
        Debug.Log($"Радиус атаки: {attack_range}");

        //Попадание по врагам 
        RaycastHit2D[] hits = Physics2D.CircleCastAll(attackPosition, attack_radius, Vector2.zero, 0, enemyLayer);
        Debug.Log($"Найдено объектов: {hits.Length}");

        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log($"Попал в объект: {hit.collider.name}");
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attack_damage);
                Debug.LogWarning($"Попал! Урон: {attack_damage}");
            }
        }
    }

    Vector2 GetAttackDirection()
    {
        return lastAttackDirection;
    }

    void ResetAttack()
    {
        can_attack = true;
    }

    void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = original_color;
        }
    }

    //Для отладки визуализация радиуса в редакторе
    void OnDrawGizmosSelected()
    {
        if (!showAttackGizmo) return;

        Vector2 attackDirection = Application.isPlaying ? lastAttackDirection : Vector2.right;
        Gizmos.color = gizmoColor;

        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range; //показываем зону атаки
        Gizmos.DrawWireSphere(attackPosition, attack_radius); //основной круг как зона поражения
        Gizmos.DrawLine(transform.position, attackPosition); //линия от игрока до центра зоны атаки
        Gizmos.color = Color.red; //маленький круг в центре зоны атаки
        Gizmos.DrawWireSphere(attackPosition, 0.05f);
    }

    //Также рисуем не только при выделении
    void OnDrawGizmos()
    {
        if (!showAttackGizmo || !Application.isPlaying) return;

        Vector2 attackDirection = lastAttackDirection;
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);//полупрозрачный

        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range;//зону атаки всегда в режиме игры
        Gizmos.DrawWireSphere(attackPosition, attack_radius);
    }
}

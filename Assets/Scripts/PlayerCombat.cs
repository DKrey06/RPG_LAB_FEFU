using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //Настройки атаки 
    [SerializeField] private float attack_range = 1f;
    [SerializeField] private float attack_cooldown = 0.5f;
    [SerializeField] private int attack_damage = 10;
    [SerializeField] private LayerMask enemyLayer; //слой объектов-врагов
    [SerializeField] private Color attack_color = Color.red; //пока что назначю красным цветом
    [SerializeField] private float attack_flashTime = 0.3f; //смена цвета 


    //Переменные
    private bool can_attack = true;
    private SpriteRenderer spriteRenderer;
    private Color original_color;

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
        if (Input.GetKeyDown(KeyCode.Space) && can_attack == true)
        {
            Attack();
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
            Debug.Log("Смена");
            spriteRenderer.color = attack_color;
            Invoke(nameof(ResetColor), attack_flashTime);
        }

        //Направление для атаки, сделаю вправо 
        Vector2 attackDirection = GetAttackDirection();
        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attack_range;

        //Отладка
        Debug.Log($"Игрок позиция: {transform.position}");
        Debug.Log($"Направление атаки: {attackDirection}");
        Debug.Log($"Позиция атаки: {attackPosition}");
        Debug.Log($"Радиус атаки: {attack_range}");

        //Попадание по врагам 
        RaycastHit2D[] hits = Physics2D.CircleCastAll(attackPosition, 0.5f, Vector2.zero, 0, enemyLayer);
        Debug.Log($"Найдено объектов: {hits.Length}");
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log($"Попал в объект: {hit.collider.name}");
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attack_damage);
                Debug.Log($"Попал! Урон: {attack_damage}");
            }

        }
    }

    Vector2 GetAttackDirection()
    {
        Vector2 direction = Vector2.right; 

        //Получаем ввод с клавиатуры для определения направления
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            direction = new Vector2(horizontal, 0).normalized;
        }
        else if (vertical != 0)
        {
            direction = new Vector2(0, vertical).normalized;
        }

        return direction;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3)GetAttackDirection() * attack_range, 0.5f);
    }
}

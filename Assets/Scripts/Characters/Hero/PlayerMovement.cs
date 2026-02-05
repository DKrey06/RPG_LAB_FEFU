using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimatorController animController;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animController = GetComponent<PlayerAnimatorController>();

        //Настройка физики
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
        else
        {
            Debug.LogError("Rigidbody2D не найден на игроке!");
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;

        //Обновляем направление для анимаций и атаки
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput;

            //Поворот спрайта
            if (horizontal != 0 && spriteRenderer != null)
            {
                spriteRenderer.flipX = horizontal < 0;
            }
        }

        if (animController != null)
        {
            animController.SetSpeed(moveInput.magnitude);
        }
        else
        {
            Debug.LogWarning("PlayerAnimatorController не найден!");
        }
    }

    void FixedUpdate()
    {
        //Движение через физику
        if (rb != null)
        {
            rb.linearVelocity = moveInput * _moveSpeed;
        }
    }

    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
}
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    //Параметры анимаций
    private float speed = 0f;
    private bool isAttacking = false;
    private bool isHurt = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
    }

    void Update()
    {
        //Все анимации обновляются в одном месте
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsAttacking", isAttacking);
            animator.SetBool("IsHurt", isHurt);
            animator.SetBool("IsDead", isDead);
        }
    }

    //Методы для установки параметров (вызываются из других скриптов)
    public void SetSpeed(float value)
    {
        speed = value;
    }

    public void SetAttacking(bool value)
    {
        isAttacking = value;
    }

    public void SetHurt(bool value)
    {
        isHurt = value;
    }

    public void SetDead(bool value)
    {
        isDead = value;
    }


    //Быстрые методы для триггеров
    public void TriggerHurt()
    {
        if (!isDead)
        {
            isHurt = true;
            Invoke(nameof(ResetHurt), 0.3f);
        }
    }

    public void TriggerDeath()
    {
        isDead = true;
    }

    public void TriggerAttack()
    {
        if (!isDead)
        {
            isAttacking = true;
        }
    }

    //Сбросы
    void ResetHurt()
    {
        isHurt = false;
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    public bool IsDead() => isDead;
}
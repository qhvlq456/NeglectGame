using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum AnimStateType
    {
        Idle = 0,
        Run = 1,
        Attack = 2,
        Hurt = 3,
        Death = 4,
    }

    [SerializeField]
    private AnimStateType currentAnimState;

    public void SetAnimState(AnimStateType state)
    {
        currentAnimState = state;
        animator.SetInteger("AnimState", (int)state);
    }

    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    public float currentHealth;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float attackRange = 1f;
    [SerializeField]
    private float attackPerSpeed = 0.5f;

    [SerializeField]
    private Slider hpSlider;

    private IEnumerator MoveCoRoutine = null;

    public bool IsPessive() => currentHealth / maxHealth <= 0.3f;
    public void Init()
    {
        StopAllCoroutines();
        hpSlider.value = 0f;
        maxHealth = PlayerManager.Instance.GetHealth();
        currentHealth = maxHealth;
        hpSlider.value = GetHealthRatio();

        transform.position= Vector3.zero;
        animator.SetBool("isAttacking", false);
        animator.SetBool("Run", false);
    }

    public void Attack(Enemy _enemy)
    {
        animator.ResetTrigger("Hurt");
        animator.SetTrigger("Attack");
        animator.SetBool("isAttacking", true);
        float damage = PlayerManager.Instance.GetAttackDamage() * PlayerManager.Instance.GetSkillDamage(SkillType.Normal);

        Color color = Color.white;

        if(PlayerManager.Instance.isLastAttackWasCritical)
        {
            color = Color.red;
        }

        UIManager.Instance.ShowDamageText(_enemy.transform.position, damage.ToString(), color);
        _enemy.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void TakeDamage(float _amount)
    {
        animator.SetTrigger("Hurt");
        currentHealth = Mathf.Max(currentHealth - _amount, 0);
        hpSlider.value = GetHealthRatio();

        if (IsDead())
        {
            animator.SetTrigger("Death");
            Debug.Log("Player Died");
        }
    }
    public IEnumerator CombatRoutine(Enemy _target)
    {
        while (true)
        {
            if (_target == null || _target.IsDead() || IsDead())
            {
                break;
            }

            yield return MoveToTarget(_target.transform);

            while (!IsInAttackRange(_target))
            {
                yield return null;
            }

            if(PlayerManager.Instance.isSkill)
            {
                yield return StartCoroutine(SkillRoutine(_target));
            }
            else
            {
                Attack(_target);
            }
            Debug.LogError($"PlayerManager.Instance.skill.GetCoolTime(SkillType.Normal) : {PlayerManager.Instance.skill.GetCoolTime(SkillType.Normal)}");
            yield return new WaitForSeconds(PlayerManager.Instance.skill.GetCoolTime(SkillType.Normal)); // 공격 텀
            animator.SetBool("isAttacking", false);
        }
    }
    private IEnumerator SkillRoutine(Enemy _target)
    {
        // animator.SetTrigger("Skill");

        yield return new WaitForSeconds(0.3f); // 스킬 타격 타이밍

        float skillDamage = PlayerManager.Instance.GetSkillDamage(SkillType.Main) * PlayerManager.Instance.GetAttackDamage();
        Debug.LogError($"skillDamage : {skillDamage}");
        UIManager.Instance.ShowDamageText(_target.transform.position, skillDamage.ToString(), Color.cyan);
        _target.TakeDamage(skillDamage);

        // yield return new WaitForSeconds(0.5f); // 스킬 후 딜레이
        PlayerManager.Instance.EndSkill();
    }
    public IEnumerator MoveToTarget(Transform _target)
    {
        if (MoveCoRoutine != null)
        {
            StopCoroutine(MoveCoRoutine);
        }

        MoveCoRoutine = MoveRoutine(_target);
        yield return MoveCoRoutine;
    }

    private IEnumerator MoveRoutine(Transform _target)
    {
        animator.SetBool("Run", true);
        Vector2 dir = _target.position - transform.position;
        ChangeFlip(dir.normalized);

        while (Vector2.Distance(transform.position, _target.position) > 1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.position, PlayerManager.Instance.GetMoveSpeed() * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("Run", false);
    }

    public bool IsInAttackRange(Enemy _target)
    {
        return Vector2.Distance(transform.position, _target.transform.position) < attackRange;
    }

    private void ChangeFlip(Vector2 _normalized)
    {
        if (_normalized.x > 0)
        {
            spriteRenderer.flipX = true; 
        }
        else if (_normalized.x < 0)
        {
            spriteRenderer.flipX = false;
        }

    }

    public float GetHealthRatio()
    {
        if (maxHealth <= 0f)
        {
            Debug.LogError($"[HealthRatio] maxHealth == 0! This will cause NaN. currentHealth: {currentHealth}");
            return 0f;
        }

        float ratio = currentHealth / maxHealth;
        if (float.IsNaN(ratio))
        {
            Debug.LogError("HealthRatio returned NaN!");
        }

        return Mathf.Clamp01(ratio);
    }
}

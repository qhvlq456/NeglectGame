using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float attackPower;
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float attackPerSpeed = 1f;

    [SerializeField]
    private Slider hpSlider;

    public Action<Enemy> OnDeath;

    IEnumerator MoveCoRoutine = null;
    public void Init(sStageData _data)
    {
        if(_data.isBoss)
        {
            maxHealth = _data.enemyHealth * 5;
            attackPower = _data.enemyAttackPower * 2;
            transform.localScale = Vector3.one * 2;
        }
        else
        {
            maxHealth = _data.enemyHealth;
            attackPower = _data.enemyAttackPower;
            transform.localScale = Vector3.one;
        }

        hpSlider.value = 0f; 
        currentHealth = maxHealth;
        hpSlider.value = GetHealthRatio();
        animator.SetBool("Run", false);
        StartCoroutine(CombatRoutine(PlayerManager.Instance.player));
    }

    public void TakeDamage(float _dmg)
    {
        animator.SetTrigger("Hit");
        currentHealth -= _dmg;
        hpSlider.value = GetHealthRatio();

        if (currentHealth <= 0 && gameObject.activeSelf)
        {
            animator.SetTrigger("Death");
            StartCoroutine(DeathDelay());
        }
    }
    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0.3f); // 애니메이션 길이만큼 대기
        OnDeath?.Invoke(this);
        // 코루틴 멈추고, 비활성화 및 풀로 반환
        StopAllCoroutines(); // 전투 중지
    }

    public void Attack(Player _player)
    {
        animator.SetTrigger("Attack");
        UIManager.Instance.ShowDamageText(_player.transform.position, attackPower.ToString(), Color.black);
        _player.TakeDamage(attackPower);
    }
    public IEnumerator CombatRoutine(Player _target)
    {
        while (true)
        {
            if (_target == null || _target.IsDead())
            {
                Debug.LogError($"2.여기서 걸림?");
                break;
            }

            yield return MoveToTarget(_target.transform);

            while (!IsInAttackRange(_target.transform))
            {
                Debug.LogError($"1. 여기서 걸림?");
                yield return null;
            }

            Attack(_target);

            yield return new WaitForSeconds(attackPerSpeed); // 공격 텀
        }
    }

    public bool IsDead() => currentHealth <= 0;

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
            transform.position = Vector2.MoveTowards(transform.position, _target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("Run", false);
    }

    public bool IsInAttackRange(Transform _target)
    {
        return Vector2.Distance(transform.position, _target.transform.position) < 1f;
    }

    private void ChangeFlip(Vector2 _normalized)
    {
        if (_normalized.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (_normalized.x < 0)
        {
            spriteRenderer.flipX = true;
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

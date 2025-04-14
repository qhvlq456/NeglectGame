using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageTextPool
{
    [SerializeField] 
    private DamageText damageTextPrefab;

    private Queue<DamageText> pool = new Queue<DamageText>();

    public DamageTextPool(DamageText _go)
    {
        damageTextPrefab = _go;
    }

    public DamageText Get()
    {
        DamageText damageText;

        if (pool.Count > 0)
        {
            damageText = pool.Dequeue();
        }
        else
        {
            damageText = UnityEngine.Object.Instantiate(damageTextPrefab);
        }

        damageText.gameObject.SetActive(true);
        return damageText;
    }

    public void ReturnToPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        damageText.transform.localScale = damageTextPrefab.transform.localScale;
        damageText.transform.rotation = Quaternion.identity;
        pool.Enqueue(damageText);
    }
}

public class DamageText : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float time;
    public Action<DamageText> OnClose;
    public void StartDamage(Vector2 _rect, string _value, Color _color)
    {
        rectTransform.position = _rect + (Vector2.up * 1.15f);
        text.color = _color;
        text.text = _value;
        StartCoroutine(CoDamage());
    }

    IEnumerator CoDamage()
    {
        float timer = 0;

        while (timer < time)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, speed * Time.deltaTime);
            yield return null;
        }

        OnClose?.Invoke(this); // 풀로 반환
    }
}

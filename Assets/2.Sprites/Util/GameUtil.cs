using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtil
{
    public static class JsonLoader<T>
    {
        public static List<T> LoadJson(string _filePath)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(_filePath);
            if (jsonFile == null)
            {
                Debug.LogError($"not found path file : {_filePath}");
                return new List<T>();
            }

            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonFile.text);
            return wrapper.ItemList;
        }

        [System.Serializable]
        private class Wrapper<U>
        {
            public List<U> ItemList;
        }
    }

    #region Health
    [System.Serializable]
    public class Health
    {
        public float currentHealth;
        public float maxHealth;

        public Health(float _max)
        {
            ResetHealth(_max);
        }

        public void ResetHealth(float _max)
        {
            maxHealth = _max;
            currentHealth = _max;
        }
        // 현재 체력 비율 (0~1)
        public float GetHealthRatio()
        {
            return currentHealth / maxHealth;
        }

        // 현재 체력 백분율 (0~100%)
        public float GetHealthPercentage()
        {
            return GetHealthRatio() * 100f;
        }

        // 소모된 체력 백분율 (0~100%)
        public float GetHealthLostPercentage()
        {
            return (1 - GetHealthRatio()) * 100f;
        }

        // 체력 변경 메서드
        public void TakeDamage(float _damage)
        {
            currentHealth = Mathf.Max(currentHealth - _damage, 0);
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }
    }

    #endregion
}

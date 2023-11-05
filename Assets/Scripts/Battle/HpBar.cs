using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    void Start()
    {
        
    }

    public void SetHp(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHpSmooth(float newHp)
    {
        float currentHp = health.transform.localScale.x;
        float changeAmount = currentHp - newHp;

        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f);
    }
}

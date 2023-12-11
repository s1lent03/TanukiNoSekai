using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] GameObject xp;
    [SerializeField] Color greenHp;
    [SerializeField] Color yellowHp;
    [SerializeField] Color redHp;

    public void SetHp(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
        health.GetComponent<Image>().color = greenHp;
    }

    public void SetXp(float xpNormalized)
    {
        gameObject.GetComponent<Slider>().value = xpNormalized;
    }

    public IEnumerator SetXpSmooth(float newXp)
    {
        float duration = 1f;
        Slider xpSlider = gameObject.GetComponent<Slider>();
        float timer = 0f;
        float startValue = xpSlider.value;

        while (timer < duration)
        {
            xpSlider.value = Mathf.Lerp(startValue, newXp, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        xpSlider.value = newXp;

    }

    public IEnumerator SetHpSmooth(float newHp)
    {
        float currentHp = health.transform.localScale.x;
        float changeAmount = currentHp - newHp;

        if (newHp >= 0.5f && newHp <= 1)
            health.GetComponent<Image>().color = greenHp;
        else if (newHp > 0.2f && newHp < 0.5f)
            health.GetComponent<Image>().color = yellowHp;
        else if (newHp >= 0 && newHp <= 0.2f)
            health.GetComponent<Image>().color = redHp;

        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f);
    }
}

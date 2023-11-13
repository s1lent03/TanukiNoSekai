using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public Tanuki tanukiUnitData;
    GameObject newTanuki;

    public Tanuki Tanuki { get; set; }
    public void Setup(bool isPlayerOrTrainer, Tanuki tanuki)
    {
        Tanuki = tanuki;

        if (gameObject.tag != "WildTanuki")
        {
            newTanuki = Instantiate(Tanuki.Base.TanukiModel, gameObject.transform);
            newTanuki.tag = "Untagged";

            if (isPlayerOrTrainer)
                PlayEnterAnimation();
        }              
    }

    public void PlayEnterAnimation()
    {
        newTanuki.transform.Find("ModelObject").transform.DOScale(new Vector3(1, 1, 1), 1);
    }

    public void PlayAttackAnimation()
    {
        //ANIMAÇÃO DE ATAQUE !!!!!
    }

    public IEnumerator PlayHitAnimation(GameObject hitEffect, Transform tanukiPosition)
    {
        GameObject effect = Instantiate(hitEffect, tanukiPosition);
        yield return new WaitForSeconds(hitEffect.GetComponent<ParticleSystem>().main.duration);
        Destroy(effect);
    }

    public void PlayFaintAnimation(GameObject tanukiToFaint)
    {
        tanukiToFaint.transform.Find("ModelObject").transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1);
    }
}

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
            newTanuki.GetComponent<TanukiMovement>().stunned = true;

            if (isPlayerOrTrainer)
                PlayEnterAnimation();
        }              
    }

    public void PlayEnterAnimation()
    {
        newTanuki.transform.Find("ModelObject").transform.DOScale(new Vector3(1, 1, 1), 1);
    }

    public void PlayAttackAnimation(GameObject tanuki, bool isPlayerTanuki)
    {
        //ANIMAÇÃO DE ATAQUE !!!!!
        if (isPlayerTanuki)
            tanuki.GetComponent<TanukiMovement>().tanukiAnimator.SetTrigger(Animator.StringToHash("Attack"));
        else
            tanuki.transform.GetChild(0).gameObject.GetComponent<TanukiMovement>().tanukiAnimator.SetTrigger(Animator.StringToHash("Attack"));
    }

    public void PlayHurtAnimation(GameObject tanuki, bool isPlayerTanuki)
    {
        //ANIMAÇÃO DE HURT !!!!!
        if (!isPlayerTanuki)
            tanuki.GetComponent<TanukiMovement>().tanukiAnimator.SetTrigger(Animator.StringToHash("Hurt"));
        else
            tanuki.transform.GetChild(0).gameObject.GetComponent<TanukiMovement>().tanukiAnimator.SetTrigger(Animator.StringToHash("Hurt"));
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

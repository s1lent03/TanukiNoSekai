using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] TanukiBase _base;
    [SerializeField] public int level;
    GameObject newTanuki;

    public Tanuki Tanuki { get; set; }
    public void Setup(bool isPlayerOrTrainer)
    {
        Tanuki = new Tanuki(_base, level);

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
        newTanuki.transform.DOScale(new Vector3(1, 1, 1), 1);
    }
}

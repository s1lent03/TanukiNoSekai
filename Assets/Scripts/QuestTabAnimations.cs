using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTabAnimations : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] float inPos;
    [SerializeField] float outPos;

    public void TabEnterAnimation()
    {
        transform.DOLocalMoveX(inPos, 1f).SetEase(Ease.OutBounce);
    }

    public void TabExitAnimation()
    {
        transform.DOLocalMoveX(outPos, 1f).SetEase(Ease.OutBounce);
    }
}

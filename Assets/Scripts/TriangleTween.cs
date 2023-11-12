using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriangleTween : MonoBehaviour
{
    private void Start()
    {
        transform.GetComponent<RectTransform>().DOAnchorPosY(20, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerDialog : MonoBehaviour
{
    [Header("Objects")]
    public GameObject dialogMenu;

    private Vector3 menuPosition;
    private Vector3 menuOutOfSight;

    private void Awake()
    {
        menuPosition = dialogMenu.transform.position;
        menuOutOfSight = menuPosition - new Vector3(0, 400, 0);

        Debug.Log(menuPosition);

        dialogMenu.transform.position = menuOutOfSight;
        dialogMenu.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogMenu.SetActive(true);
            dialogMenu.transform.DOMove(menuPosition, 1).SetEase(Ease.OutCubic);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogMenu.transform.position = menuOutOfSight;
            dialogMenu.SetActive(false);
        }
    }
}

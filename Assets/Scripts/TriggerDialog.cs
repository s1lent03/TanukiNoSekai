using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialog : MonoBehaviour
{
    public GameObject dialogMenu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            dialogMenu.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            dialogMenu.SetActive(false);
    }
}

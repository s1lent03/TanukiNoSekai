using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Character")
        {           
            other.gameObject.transform.Find("TanukiDetector").GetComponent<TanukiDetection>().EnteredCollider(gameObject.transform.parent.gameObject);
            other.gameObject.transform.Find("TanukiDetector").GetComponent<TanukiDetection>().SetupCamera();
            other.gameObject.transform.Find("TanukiDetector").GetComponent<TanukiDetection>().SetupBattle();
        }

        if (gameObject.transform.parent.transform.Find("YukatoriPsy(Clone)"))
        {
            Destroy(gameObject.transform.parent.transform.Find("YukatoriPsy(Clone)").gameObject);
        }
    }
}

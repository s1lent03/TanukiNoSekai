using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] string bossPrefabName;

    void Start()
    {
        if (PlayerPrefs.GetString(bossPrefabName) == "True")
        {
            Destroy(gameObject);
        }
    }
}

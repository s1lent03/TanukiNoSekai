using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class LoadingAnim : MonoBehaviour
{
    int rotValue;

    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotValue);
            rotValue--;
        }
    }
}

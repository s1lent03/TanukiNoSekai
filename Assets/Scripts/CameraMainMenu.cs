using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainMenu : MonoBehaviour
{
    [Header("Main")]
    public int RotationSpeed;
    private float newRotY = 0;

    void Update()
    {
        newRotY = transform.rotation.y + (RotationSpeed * Time.time);
        Vector3 Rotate = new Vector3(transform.rotation.x, newRotY, transform.rotation.z);

        transform.rotation = Quaternion.Euler(Rotate);
    }
}

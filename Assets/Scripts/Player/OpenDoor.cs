using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] float RotateAmount;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool isOpen = false;
    public bool isRotating = false;

    public void OpenCloseDoor()
    {
        if (!isOpen && !isRotating)
        {
            StartCoroutine(RotateObject(RotateAmount));            
        }
        else if (!isRotating)
        {
            StartCoroutine(RotateObject(0));
        }
    }

    private IEnumerator RotateObject(float finalRot)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, finalRot, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime);
            elapsedTime += Time.deltaTime * rotationSpeed;

            yield return null;
        }

        if (finalRot == 0)
            isOpen = false;
        else
            isOpen = true;

        isRotating = false;


    }
}

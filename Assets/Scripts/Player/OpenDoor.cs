using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] float OpenRotation;
    [SerializeField] float CloseRotation;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool isOpen = false;
    public bool isRotating = false;

    [Header("Audio")]
    [SerializeField] AudioSource openDoorSoundFx;
    [SerializeField] AudioSource closeDoorSoundFx;

    public void OpenCloseDoor()
    {
        if (!isOpen && !isRotating)
        {
            StartCoroutine(RotateObject(OpenRotation));
            openDoorSoundFx.Play();
        }
        else if (!isRotating)
        {
            StartCoroutine(RotateObject(CloseRotation));
            closeDoorSoundFx.Play();
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

        if (finalRot == CloseRotation)
            isOpen = false;
        else
            isOpen = true;

        isRotating = false;


    }
}

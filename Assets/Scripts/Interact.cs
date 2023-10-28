using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [Header("MainVals")]
    public float interactDistance;
    public TMP_Text interactText;
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        //Criar um ray que segue a direção da camera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            //Se o ray atingir um objeto com a tag "Interactable" e tiver a menos de 1.5 de distancia vai aparecer um texto a indicar que o objeto é interagivel
            if (hit.collider.gameObject.tag == "Interactable")
            {
                if (Vector3.Distance(hit.point, ray.origin) < interactDistance)
                {
                    interactText.gameObject.SetActive(true);

                    if (Input.GetJoystickNames().Length > 0)
                    {
                        if (Input.GetJoystickNames()[0].ToLower().Contains("xbox"))
                        {
                            interactText.text = "Press X Button to interact.";
                        }
                        else if (Input.GetJoystickNames()[0].ToLower().Contains("playstation"))
                        {
                            interactText.text = "Press Square Button to interact.";
                        }
                        else
                        {
                            interactText.text = "Press F to interact.";
                        }
                    }
                    else
                    {
                        interactText.text = "Press F to interact.";
                    }                                     

                    //Se o jogador clicar no botão pretendido, irá ao script do objeto que faz com que o VFX seja ativado
                    if (playerInput.actions["Interact"].triggered)
                    {
                        Debug.Log("interagiu");
                    }
                }
            }
            else
            {
                interactText.gameObject.SetActive(false);
            }
        }
    }
}

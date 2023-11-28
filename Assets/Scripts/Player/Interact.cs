using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        //Criar um ray que segue a dire��o da camera
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 30f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            //Se o ray atingir um objeto com a tag pretendida e tiver a menos de 1.5 de distancia vai aparecer um texto a indicar que o objeto � interagivel
            if (hit.collider.gameObject.tag == "Interactable" || hit.collider.gameObject.tag == "WildTanuki" && hit.collider.gameObject.transform.parent.name != "Tanuki")
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

                    //Se o jogador clicar no bot�o pretendido, ir� ao script do objeto que faz com que o VFX seja ativado
                    if (playerInput.actions["Interact"].triggered)
                    {
                        gameObject.GetComponentInChildren<TanukiDetection>().EnteredCollider(hit.collider.gameObject);
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

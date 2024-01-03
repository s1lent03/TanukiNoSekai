using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class Interact : MonoBehaviour
{
    [Header("MainVals")]
    public float interactDistance;
    public TMP_Text interactText;
    private PlayerInput playerInput;

    [Header("Buying")]
    [SerializeField] TMP_Text currentMoneyText;

    [Header("Others")]
    GameObject buyingInfoObject;
    string lastInfoName;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        lastInfoName = "";

        //REMOVERRRRRRRRRRRRRRRRRRRRRRRRR
        PlayerPrefs.SetInt("CurrentMoney", 500);
    }

    private void Update()
    {
        //Mostrar dinheiro atual
        currentMoneyText.text = PlayerPrefs.GetInt("CurrentMoney") + "$";
    }

    private void FixedUpdate()
    {
        //Criar um ray que segue a direção da camera
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        RaycastHit hit = new RaycastHit();

        //Ignorar o terreno
        int groundLayer = 3;
        int layerMask = 1 << groundLayer;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, interactDistance, 1 << 0))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            GameObject otherObject = hit.collider.gameObject;

            //Se o ray atingir um objeto com a tag pretendida e tiver a menos de 1.5 de distancia vai aparecer um texto a indicar que o objeto é interagivel
            if (otherObject.tag == "WildTanuki" && otherObject.GetComponent<TanukiMovement>().stunned == true)
            {
                interactText.text = WhatToDisplay("Mouse 2", "LT", "L2", "feed the Tanuki");

                //Se o jogador clicar no botão pretendido, irá ao script do objeto que faz com que o VFX seja ativado
                if (playerInput.actions["GiveBerries"].triggered)
                {
                    gameObject.GetComponent<PlayerHabilities>().PlayDropBerry(otherObject.gameObject);
                }
            }

            //Se o ray atingir uma porta vai abri-la ou fechá-la
            else if(otherObject.tag == "Interactable")
            {
                interactText.text = WhatToDisplay("F", "X", "Square Button", "open/close the door");

                if (playerInput.actions["Interact"].triggered && !otherObject.GetComponent<OpenDoor>().isRotating)
                {
                    otherObject.GetComponent<OpenDoor>().OpenCloseDoor();
                }
            }

            //Se o ray atingir um objeto com a tag pretendida e tiver a menos de 1.5 de distancia vai aparecer um texto a indicar que o objeto é interagivel
            else if (otherObject.tag == "Buyable")
            {
                interactText.text = WhatToDisplay("F", "X", "Square Button", "buy the item"); 

                if (lastInfoName != otherObject.name && buyingInfoObject != null)
                {
                    buyingInfoObject.transform.Find("Info").gameObject.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 1);
                    buyingInfoObject.transform.Find("Info").gameObject.SetActive(false);
                    lastInfoName = buyingInfoObject.name;                    
                }

                //Mostrar custo do item a comprar
                buyingInfoObject = otherObject;
                buyingInfoObject.transform.Find("Info").gameObject.SetActive(true);
                buyingInfoObject.transform.Find("Info").gameObject.transform.DOScale(new Vector3(1f, 1.3f, 1f), 1);

                //Se o jogador clicar no botão pretendido, irá ao script do objeto que faz com que o VFX seja ativado
                if (playerInput.actions["Interact"].triggered)
                {
                    if (otherObject.name == "BallLevel1Buy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 10)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfBall1", PlayerPrefs.GetInt("NumberOfBall1") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 10);
                        }
                    }
                    else if (otherObject.name == "BallLevel2Buy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 25)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfBall2", PlayerPrefs.GetInt("NumberOfBall2") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 25);
                        }
                    }
                    else if (otherObject.name == "BallLevel3Buy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 60)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfBall3", PlayerPrefs.GetInt("NumberOfBall3") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 60);
                        }
                    }
                    else if (otherObject.name == "BerryLevel1Buy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 15)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfBerry1", PlayerPrefs.GetInt("NumberOfBerry1") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 15);
                        }
                    }
                    else if (otherObject.name == "BerryLevel2Buy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 35)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfBerry2", PlayerPrefs.GetInt("NumberOfBerry2") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 35);
                        }
                    }
                    else if (otherObject.name == "BerryLevel3Buy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 80)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfBerry3", PlayerPrefs.GetInt("NumberOfBerry3") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 80);
                        }
                    }
                    else if (otherObject.name == "Level1PotionBuy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 25)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfPotion1", PlayerPrefs.GetInt("NumberOfPotion1") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 25);
                        }
                    }
                    else if (otherObject.name == "Level2PotionBuy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 50)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfPotion2", PlayerPrefs.GetInt("NumberOfPotion2") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 50);
                        }
                    }
                    else if (otherObject.name == "Level3PotionBuy")
                    {
                        if (PlayerPrefs.GetInt("CurrentMoney") > 100)
                        {
                            //Adicionar mais um item do escolhido e remover dinheiro
                            PlayerPrefs.SetInt("NumberOfPotion3", PlayerPrefs.GetInt("NumberOfPotion3") + 1);
                            PlayerPrefs.SetInt("CurrentMoney", PlayerPrefs.GetInt("CurrentMoney") - 100);
                        }
                    }
                }
            }
        }
        else
        {
            interactText.text = "";
            currentMoneyText.text = "";

            if (buyingInfoObject != null)
            {
                buyingInfoObject.transform.Find("Info").gameObject.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 1);
                buyingInfoObject.transform.Find("Info").gameObject.SetActive(false);
            }
        }
    }

    //Dependendo do que o jogador tiver a utilizar para jogar, vai mostrar o devido texto
    public string WhatToDisplay(string keyboardKey, string xboxKey, string psKey, string whatToDo)
    {
        interactText.gameObject.SetActive(true);

        string buttonName;
        if (Input.GetJoystickNames().Length > 0)
        {
            //Escolher o que escrever no nome do botão a clicar, dependendo do controlador
            if (Input.GetJoystickNames()[0].ToLower().Contains("xbox"))
            {
                buttonName = xboxKey + " Button";
            }
            else if (Input.GetJoystickNames()[0].ToLower().Contains("playstation"))
            {
                buttonName = psKey + " Square Button";
            }
            else
            {
                buttonName = keyboardKey;
            }
        }
        else
        {
            buttonName = keyboardKey;
        }

        //Escrever o texto
        return "Press " + buttonName + " to " + whatToDo + ".";
    }
}

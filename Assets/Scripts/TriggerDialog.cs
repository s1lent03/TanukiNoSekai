using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

public class TriggerDialog : MonoBehaviour
{
    [Header("Objects")]
    public RectTransform dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Transform player;
    public TMP_Text notification;
    public CinemachineVirtualCamera dialogueCamera;
    public Animator animator;

    [Header("Variables")]
    [TextArea(4, 6)] public string[] normalDialogueLines;
    [TextArea(4, 6)] public string[] merchantDialogueLines;
    public float typingTime;
    public float cooldownTime;

    private bool isPlayerInRange = false;
    private bool didDialogueStart = false;
    private int lineIndex = 0;
    private float cooldown = 0f;
    private float originalYAxis = 0f;

    private void Awake()
    {
        nameText.text = string.Empty;
        dialogueText.text = string.Empty;
        dialoguePanel.gameObject.SetActive(true);
        originalYAxis = dialoguePanel.anchoredPosition.y;
        dialoguePanel.position = new Vector3(dialoguePanel.position.x, -Screen.height, dialoguePanel.position.z);
        notification.text = "Press F to Interact with " + transform.parent.gameObject.name;
        notification.transform.localScale = new Vector3(1, 0, 1);
        dialogueCamera.Priority = 9;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            notification.text = other.GetComponent<Interact>().WhatToDisplay("F", "X", "Square Button", "interact with " + transform.parent.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (didDialogueStart || !isPlayerInRange)
            notification.transform.DOScaleY(0, 0.25f);
        else
            notification.transform.DOScaleY(1, 0.25f);

        if (didDialogueStart && dialogueCamera.Priority == 11)
            player.GetComponent<PlayerMovement>().isPaused = true;

        animator.SetBool(Animator.StringToHash("Talk"), didDialogueStart);

        if (gameObject.tag == "Merchant")
        {
            
            //Escrever qual tecla usar para interagir
            if ((isPlayerInRange || didDialogueStart) && player.GetComponent<PlayerInput>().actions["Interact"].IsPressed() && Time.timeScale == 1)
            {
                if (!didDialogueStart)
                {
                    didDialogueStart = true;
                    lineIndex = Random.Range(0, merchantDialogueLines.Length);
                    dialogueCamera.Priority = 11;
                    dialoguePanel.DOAnchorPosY(originalYAxis, 0.5f).SetEase(Ease.InOutSine);
                    nameText.text = transform.parent.gameObject.name;
                    StartCoroutine(ShowLine(merchantDialogueLines));
                }
                else if (dialogueText.text == merchantDialogueLines[lineIndex])
                {
                    player.GetComponent<PlayerMovement>().isPaused = false;
                    dialogueCamera.Priority = 9;
                    dialoguePanel.DOMoveY(-Screen.height, 0.5f).SetEase(Ease.InOutSine);
                }
            }

            if (didDialogueStart && dialogueCamera.Priority == 9)
            {
                if (cooldown >= cooldownTime || !isPlayerInRange)
                {
                    didDialogueStart = false;
                    cooldown = 0f;
                }
                else
                {
                    cooldown += Time.deltaTime;
                }
            }
        }
        else
        {
            // code for random line
            if ((isPlayerInRange || didDialogueStart) && player.GetComponent<PlayerInput>().actions["Interact"].IsPressed() && Time.timeScale == 1)
            {
                if (!didDialogueStart)
                {
                    didDialogueStart = true;
                    lineIndex = Random.Range(0, normalDialogueLines.Length);
                    dialogueCamera.Priority = 11;
                    dialoguePanel.DOAnchorPosY(originalYAxis, 0.5f).SetEase(Ease.InOutSine);
                    nameText.text = transform.parent.gameObject.name;
                    StartCoroutine(ShowLine(normalDialogueLines));
                }
                else if (dialogueText.text == normalDialogueLines[lineIndex])
                {
                    player.GetComponent<PlayerMovement>().isPaused = false;
                    dialogueCamera.Priority = 9;
                    dialoguePanel.DOMoveY(-Screen.height, 0.5f).SetEase(Ease.InOutSine);
                }
            }

            if (didDialogueStart && dialogueCamera.Priority == 9)
            {
                if (cooldown >= cooldownTime || !isPlayerInRange)
                {
                    didDialogueStart = false;
                    cooldown = 0f;
                }
                else
                {
                    cooldown += Time.deltaTime;
                }
            }
        }

        // addition to if clause related to player walkspeed

        /*&& lineIndex < dialogueLines.Length*/

        // code for dialog with multiple lines

        /*if ((isPlayerInRange || didDialogueStart) && player.GetComponent<PlayerInput>().actions["Interact"].IsPressed() && Time.timeScale == 1)
        {
            if (!didDialogueStart)
            {
                didDialogueStart = true;
                lineIndex = 0;
                dialogueCamera.Priority = 11;
                dialoguePanel.DOAnchorPosY(originalYAxis, 0.5f).SetEase(Ease.InOutSine);
                nameText.text = transform.parent.gameObject.name;
                StartCoroutine(ShowLine());
            }
            else if (lineIndex >= dialogueLines.Length || dialogueText.text == dialogueLines[lineIndex])
            {
                lineIndex++;

                if (lineIndex < dialogueLines.Length)
                {
                    StartCoroutine(ShowLine());
                }
                else
                {
                    player.GetComponent<PlayerMovement>().isPaused = false;
                    dialogueCamera.Priority = 9;
                    dialoguePanel.DOMoveY(-Screen.height, 0.5f).SetEase(Ease.InOutSine);
                }
            }
        }

        if (didDialogueStart && lineIndex >= dialogueLines.Length)
        {
            if (cooldown >= cooldownTime || !isPlayerInRange)
            {
                didDialogueStart = false;
                cooldown = 0f;
            }
            else
            {
                cooldown += Time.deltaTime;
            }
        }*/
    }

    private IEnumerator ShowLine(string[] dialogueLines)
    {
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
    }
}
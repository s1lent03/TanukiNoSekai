﻿using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;

public class TriggerDialog : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public TMP_Text notification;

    [Header("Other Objects")]
    public CinemachineVirtualCamera dialogueCamera;
    public Animator animator;
    public NPCMovement npcMov;

    [Header("Black Screen Vars")]
    public GameObject TimeManager;
    public float sleepTime;
    [Space]
    public Image BlackScreen;
    public Color BlackScreenOn;
    public Color BlackScreenOff;

    [Header("Dialogue Variables")]
    [TextArea(4, 6)] public string[] normalDialogueLines;
    [TextArea(4, 6)] public string[] questDialogueLines;
    [TextArea(4, 6)] public string[] merchantDialogueLines;
    [TextArea(4, 6)] public string[] medicDialogueLines;
    public bool multiLine;
    public float typingTime;
    public float cooldownTime;

    [Header("Quest Variables")]
    public bool addQuest;
    public QuestManager questManager;
    public TypeOfQuests questType;
    public TanukiNames questTanuki;
    public int questTotal;
    public string questMessage;
    public Rewards questReward;
    public int questAmount;

    [Header("Private Variables")]
    private Transform player;
    private string[] dialogueLines;
    private bool isPlayerInRange = false;
    private bool didDialogueStart = false;
    private int lineIndex = 0;
    private float cooldown = 0f;

    [Header("Audio")]
    [SerializeField] AudioSource healingSoundFX;

    private void Awake()
    {
        nameText.text = string.Empty;
        dialogueText.text = string.Empty;
        dialoguePanel.gameObject.SetActive(true);
        dialoguePanel.position = new Vector3(dialoguePanel.position.x, -Screen.height, dialoguePanel.position.z);
        notification.transform.localScale = new Vector3(1, 0, 1);
        dialogueCamera.Priority = 9;

        if (gameObject.tag == "Merchant")
            dialogueLines = merchantDialogueLines;
        else if (gameObject.tag == "Medic")
            dialogueLines = medicDialogueLines;
        else if (gameObject.tag == "QuestNPC")
            dialogueLines = questDialogueLines;
        else
            dialogueLines = normalDialogueLines;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.transform;
            notification.text = other.GetComponent<Interact>().WhatToDisplay("F", "X", "Square Button", "interact with " + transform.parent.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (!player.GetComponent<PlayerMovement>().chatCooldown)
                player = null;
        }
    }

    private void Update()
    {
        if (didDialogueStart || !isPlayerInRange || (player != null && player.GetComponent<PlayerMovement>().chatCooldown))
            notification.transform.DOScaleY(0, 0.25f);
        else
            notification.transform.DOScaleY(1, 0.25f);

        if (didDialogueStart && dialogueCamera.Priority == 11)
            player.GetComponent<PlayerMovement>().isPaused = true;

        if (npcMov != null)
        {
            npcMov.freeze = didDialogueStart;

            if (player != null)
            {
                Vector3 dir = player.position - npcMov.transform.position;
                dir.y = 0;

                if (!npcMov.freeze)
                    notification.transform.parent.rotation = Quaternion.LookRotation(-dir);

                if (didDialogueStart)
                    npcMov.transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        animator.SetBool(Animator.StringToHash("Talk"), didDialogueStart);

        if (((isPlayerInRange && !player.GetComponent<PlayerMovement>().chatCooldown) || didDialogueStart) && player.GetComponent<PlayerInput>().actions["Interact"].IsPressed() && Time.timeScale == 1)
        {
            if (!didDialogueStart)
            {
                player.GetComponent<PlayerMovement>().chatCooldown = true;
                didDialogueStart = true;
                dialogueCamera.Priority = 11;
                dialoguePanel.DOAnchorPosY(150, 0.5f).SetEase(Ease.InOutSine);
                nameText.text = transform.parent.gameObject.name;

                if (multiLine)
                    lineIndex = 0;
                else
                    lineIndex = Random.Range(0, dialogueLines.Length);

                StartCoroutine(ShowLine());
            }
            else
            {
                if (multiLine && (lineIndex >= dialogueLines.Length || dialogueText.text == dialogueLines[lineIndex]))
                {
                    lineIndex++;

                    if (lineIndex < dialogueLines.Length)
                    {
                        if (addQuest)
                        {
                            questManager.AddQuest(questType, questTanuki, questTotal, questMessage, questReward, questAmount);
                            addQuest = false;
                        }
                        else
                        {
                            StartCoroutine(ShowLine());
                        }
                        
                    }
                }

                if ((!multiLine && dialogueText.text == dialogueLines[lineIndex]) || (multiLine && lineIndex >= dialogueLines.Length))
                {
                    player.GetComponent<PlayerMovement>().isPaused = false;
                    dialogueCamera.Priority = 9;
                    dialoguePanel.DOMoveY(-Screen.height, 0.5f).SetEase(Ease.InOutSine);

                    if (gameObject.tag == "Medic")
                    {
                        StartCoroutine(Heal());
                    }

                    if (addQuest)
                    {
                        questManager.AddQuest(questType, questTanuki, questTotal, questMessage, questReward, questAmount);
                        npcMov.freezeWhenLoad = false;
                        addQuest = false;
                    }
                }
            }
        }

        if (didDialogueStart && dialogueCamera.Priority == 9)
        {
            if (cooldown >= cooldownTime || !isPlayerInRange)
            {
                didDialogueStart = false;
                player.GetComponent<PlayerMovement>().chatCooldown = false;
                cooldown = 0f;

                if (!isPlayerInRange)
                    player = null;
            }
            else
            {
                cooldown += Time.deltaTime;
            }
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
    }

    IEnumerator Heal()
    {
        healingSoundFX.Play();
        BlackScreen.DOColor(BlackScreenOn, 1f);
        yield return new WaitForSeconds(1f);

        //Passar tempo e curar
        TimeManager.GetComponent<DayNightCycle>().TimeHours += sleepTime;
        for (int i = 0; i < player.GetComponent<TanukiParty>().Tanukis.Count; i++)
        {
            Tanuki tanukiToHeal = player.GetComponent<TanukiParty>().Tanukis[i];
            tanukiToHeal.Hp = tanukiToHeal.MaxHp;
            
            for (int j = 0; j < tanukiToHeal.Moves.Count; j++)
            {
                tanukiToHeal.Moves[j].Pp = tanukiToHeal.Moves[j].Base.Pp;
            }
        }

        yield return new WaitForSeconds(1f);

        BlackScreen.DOColor(BlackScreenOff, 1f);
    }
}
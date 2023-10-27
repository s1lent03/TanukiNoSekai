using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TriggerDialog : MonoBehaviour
{
    [Header("Objects")]
    public GameObject dialogPanel;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public PlayerInput playerInput;

    [Header("Variables")]
    [TextArea(4, 6)] public string[] dialogLines;
    public float typingTime;

    private bool isPlayerInRange = false;
    private bool didDialogStart = false;
    private bool didPlayerSkip = false;
    private int lineIndex = 0;
    private CharacterController controller;

    private void Awake()
    {
        nameText.text = string.Empty;
        dialogText.text = string.Empty;
        dialogPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller = other.GetComponent<CharacterController>();
            isPlayerInRange = true;
            Debug.Log("player entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller = null;
            isPlayerInRange = false;
            Debug.Log("player left");
        }
    }

    private void Update()
    {
        if ((isPlayerInRange || didDialogStart) && playerInput.actions["Interact"].triggered && Time.timeScale == 1)
        {
            if (!didDialogStart)
            {
                didDialogStart = true;
                lineIndex = 0;
                controller.enabled = false;
                dialogPanel.SetActive(true);
                nameText.text = gameObject.name;
                StartCoroutine(ShowLine());
            }
            else if (dialogText.text != dialogLines[lineIndex]) {
                didPlayerSkip = true;
            }
            else {
                lineIndex++;
                didPlayerSkip = false;

                if (lineIndex < dialogLines.Length)
                {
                    StartCoroutine(ShowLine());
                }
                else
                {
                    didDialogStart = false;
                    controller.enabled = true;
                    dialogPanel.SetActive(false);
                }
            }
        }
    }

    private IEnumerator ShowLine()
    {
        dialogText.text = string.Empty;

        foreach (char ch in dialogLines[lineIndex])
        {
            if (!didPlayerSkip)
            {
                dialogText.text += ch;
                yield return new WaitForSeconds(typingTime);
            }
        }

        dialogText.text = dialogLines[lineIndex];
    }
}
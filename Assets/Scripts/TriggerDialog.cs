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
    private int lineIndex = 0;

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
            isPlayerInRange = true;
            Debug.Log("player entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("player left");
        }
    }

    private void Update()
    {
        if ((isPlayerInRange || didDialogStart) && playerInput.actions["Interact"].IsPressed() && Time.timeScale == 1)
        {
            if (!didDialogStart)
            {
                didDialogStart = true;
                lineIndex = 0;

                dialogPanel.SetActive(true);
                nameText.text = gameObject.name;
                StartCoroutine(ShowLine());
            }
            else if (dialogText.text == dialogLines[lineIndex])
            {
                lineIndex++;

                if (lineIndex < dialogLines.Length)
                {
                    StartCoroutine(ShowLine());
                }
                else
                {
                    didDialogStart = false;
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
            dialogText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputFieldValidator : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = gameObject.GetComponent<TMP_InputField>();
    }
    public void OnValueChanged()
    {
        string enteredText = inputField.text;

        if (AccentInputValidator.ContainsAccents(enteredText))
        {
            // Accented characters found, display an error message or reject the input
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }

        if (enteredText.Contains(" "))
        {
            inputField.text = enteredText.Replace(" ", "");
        }
    }
}

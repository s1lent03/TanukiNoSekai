using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject updateMenu;
    public GameObject registerMenu;
    public GameObject loginMenu;
    public GameObject profileMenu;
    [Space]
    public GameObject eventSystemGameobject;

    [Header("FirstButtons")]
    public GameObject updateMenuFirstButton;
    public GameObject registerMenuFirstButton;
    public GameObject loginMenuFirstButton;
    public GameObject profileMenuFirstButton;
    public GameObject mainMenuFirstButton;

    [Header("Texts")]
    public TMP_Text versionTxt;
    [Space]
    public TMP_Text registerUsernameTxt;
    public TMP_Text registerEmailTxt;
    public GameObject registerPasswordTxt;
    public TMP_Text registerFeedbackTxt;
    [Space]
    public TMP_Text loginEmailTxt;
    public GameObject loginPasswordTxt;
    public TMP_Text loginFeedbackTxt;

    [Header("Main")]
    private string versionNumber;
    private string updateLink;

    [Header("Others")]
    private bool doOnce = false;

    void Start()
    {
        //Buscar versão do jogo
        StartCoroutine(GetGameVersion());

        //Buscar link de nova versão
        StartCoroutine(GetUpdateLink());
    }

    void Update()
    {
        //Version control
        if (versionNumber != "" && versionNumber != null && doOnce == false)
        {
            //Verificar versão do jogo
            if (PlayerPrefs.GetString("GameVersion") == "" || PlayerPrefs.GetString("GameVersion") == null)
                PlayerPrefs.SetString("GameVersion", versionNumber);

            versionTxt.text = "Version: " + PlayerPrefs.GetString("GameVersion");

            //Atualizar jogo se a versão for diferente
            if (PlayerPrefs.GetString("GameVersion") != versionNumber)
            {
                //Abrir janela de update
                updateMenu.SetActive(true);

                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(updateMenuFirstButton);
            }

            PlayerPrefs.SetString("GameVersion", versionNumber);

            //Verificar se existe algum utilizador logado
            if (PlayerPrefs.GetString("IsUserLogged") != "true")
            {
                registerMenu.SetActive(true);
                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(registerMenuFirstButton);
            }

            doOnce = true;
        }
    }

    //Ir para o menu de login
    public void GoToLoginMenuButton()
    {
        registerMenu.SetActive(false);
        profileMenu.SetActive(false);
        loginMenu.SetActive(true);

        eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(loginMenuFirstButton);
    }

    //Ir para o menu de login
    public void GoToRegisterMenuButton()
    {
        registerMenu.SetActive(true);
        loginMenu.SetActive(false);

        eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(registerMenuFirstButton);
    }

    //Registar utilizador
    public void RegisterButton()
    {
        //Verifica se todos os campos estão preenchidos e se o email possui um @
        if (registerUsernameTxt.text.Length > 0 && registerEmailTxt.text.Length > 0 && registerPasswordTxt.GetComponent<TMP_InputField>().text.Length > 0 && registerEmailTxt.text.Contains("@"))
        {
            StartCoroutine(RegisterUser());
        }      
    }

    IEnumerator RegisterUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", registerUsernameTxt.text);
        form.AddField("email", registerEmailTxt.text);
        form.AddField("password", registerPasswordTxt.GetComponent<TMP_InputField>().text);

        UnityWebRequest www = UnityWebRequest.Post("http://tanukinosekai.atwebpages.com/registerUser.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (!www.downloadHandler.text.Contains("Error: "))
            {
                registerMenu.SetActive(false);
                loginMenu.SetActive(true);

                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(registerMenuFirstButton);
            }
            registerFeedbackTxt.text = www.downloadHandler.text;
        }
    }

    //Dar login
    public void LoginButton()
    {
        //Verifica se todos os campos estão preenchidos e se o email possui um @
        if (loginEmailTxt.text.Length > 0 && loginPasswordTxt.GetComponent<TMP_InputField>().text.Length > 0 && loginEmailTxt.text.Contains("@"))
        {
            StartCoroutine(LoginUser());
        }
    }

    IEnumerator LoginUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", loginEmailTxt.text);
        form.AddField("password", loginPasswordTxt.GetComponent<TMP_InputField>().text);

        UnityWebRequest www = UnityWebRequest.Post("http://tanukinosekai.atwebpages.com/loginUser.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (!www.downloadHandler.text.Contains("Error: "))
            {
                //Guardar valores do user
                int index = loginEmailTxt.text.IndexOf("@");
                string username = loginEmailTxt.text.Substring(0, index);
                PlayerPrefs.SetString("Email", username);

                PlayerPrefs.SetString("IsUserLogged", "true");

                //Desligar janela de login
                loginMenu.SetActive(false);

                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(mainMenuFirstButton);
            }
            loginFeedbackTxt.text = www.downloadHandler.text;
        }
    }

    public void OpenProfile()
    {
        profileMenu.SetActive(true);
    }

    //Dar logout
    public void LogoutButton()
    {
        PlayerPrefs.SetString("IsUserLogged", "false");

        profileMenu.SetActive(false);
        loginMenu.SetActive(true);

        eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemGameobject.GetComponent<EventSystem>().SetSelectedGameObject(loginMenuFirstButton);
    }

    //Sai da janela aberta
    public void Back()
    {
        updateMenu.SetActive(false);
        profileMenu.SetActive(false);
    }

    //Fecha o jogo e leva o jogador para a janela de download da nova versão
    public void UpdateGame()
    {
        Application.OpenURL(updateLink);
        Application.Quit();
    }

    //Vai buscar o número da versão do jogo
    IEnumerator GetGameVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://tanukinosekai.atwebpages.com/gameVersion.php");
        yield return www.SendWebRequest();

        versionNumber = www.downloadHandler.text;
    }

    //Vai buscar o link da nova versão do jogo
    IEnumerator GetUpdateLink()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://tanukinosekai.atwebpages.com/latestVersion.php");
        yield return www.SendWebRequest();

        updateLink = www.downloadHandler.text;
    }
}

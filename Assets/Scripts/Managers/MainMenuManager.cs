using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("MenusGameobjects")]
    public GameObject visualsMenu;
    public GameObject audioMenu;
    public GameObject creditsMenu;
    [Space]
    public GameObject eventSystemObject;
    private GameObject lastSelectedObject;
    [Space]
    public GameObject loadingImage;

    [Header("FirstButtons")]
    public GameObject mainMenuFirstButton;
    public GameObject visualsFirstButton;
    public GameObject audioFirstButton;
    public GameObject creditsFirstButton;

    [Header("Sounds")]
    public AudioSource navegationSoundFX;
    public AudioSource buttonClickSoundFX;

    [Header("Others")]
    [SerializeField] string settingsFileName;

    void Start()
    {
        //Verifica se existe o ficheiro que guarda as settings, se não cria-o
        string filePath = Application.dataPath + settingsFileName;
        if (!File.Exists(filePath))
        {
            string[] content =
            {
                "Selected visual quality: _Medium",
                "",
                "Music Value: _-10",
                "SoundFX Value: _-10",
                "Ambience Value: _-10",
            };
            
            WriteTextToFile(filePath, content);

            PlayerPrefs.SetString("SettingsPath", filePath);
        }

        //Atualiza os valores dos mixers
        audioMenu.GetComponent<AudioMenuManager>().UpdateMixersBasedOnFile();

        //Dar um valor default à variavel
        if (lastSelectedObject == null)
        {
            lastSelectedObject = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject;
        }

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(mainMenuFirstButton);
    }

    void Update()
    {
        //Sempre que o utilizador clicar num botão para navegar vai produzir um sound effect
        if (lastSelectedObject != eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject)
        {
            navegationSoundFX.Play();
            lastSelectedObject = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject;
        }
    }

    void WriteTextToFile(string fileName, string[] content)
    {
        //Escreve as linhas no ficheiro
        File.WriteAllLines(fileName, content);
    }

    //Abre o menu para dar load ou criar uma jornada
    public void JourneyButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Loading animação
        loadingImage.SetActive(true);

        //Abre a janela pretendida
        SceneManager.LoadSceneAsync("StoryMap");
    }

    //Abre o menu dos visuais
    public void VisualsButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Setup à janela dos visuals
        string path = PlayerPrefs.GetString("SettingsPath");
        int lineNumber = 0;
        string[] lines = File.ReadAllLines(path);

        int startOfWord = lines[lineNumber].IndexOf("_");
        string CurrentQuality = lines[lineNumber].Substring(startOfWord + 1);

        visualsMenu.GetComponent<VisualsMenuManager>().SelectButtonBasedOnQuality(CurrentQuality);

        //Ligar e desligar as respetivas janelas
        SwitchWindows(visualsMenu, audioMenu, creditsMenu, visualsFirstButton); 
    }

    //Abre o menu do audio
    public void AudioButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Setup à janela do audio
        audioMenu.GetComponent<AudioMenuManager>().UpdateSlidersBasedOnFile();

        //Ligar e desligar as respetivas janelas
        SwitchWindows(audioMenu, visualsMenu, creditsMenu, audioFirstButton);
    }

    //Abre a janela dos creditos de desenvolvimento
    public void CreditsButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Ligar e desligar as respetivas janelas
        SwitchWindows(creditsMenu, audioMenu, visualsMenu, creditsFirstButton);
    }

    //Fecha o jogo
    public void QuitDesktopButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Dá quit ao jogo
        Application.Quit();
    }

    //Desligar todas as janelas
    public void BackButton()
    {
        //Fecha as janelas
        visualsMenu.SetActive(false);
        audioMenu.SetActive(false);
        creditsMenu.SetActive(false);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(mainMenuFirstButton);
    }

    //Ligar e desligar as respetivas janelas
    void SwitchWindows(GameObject WindowToOpen, GameObject WindowToClose1, GameObject WindowToClose2, GameObject ButtonToSelect)
    {
        //Troca janelas
        WindowToOpen.SetActive(true);
        WindowToClose1.SetActive(false);
        WindowToClose2.SetActive(false);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(ButtonToSelect);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("MenusGameobjects")]
    public GameObject journeyMenu;
    public GameObject loadJourneyMenu;
    public GameObject createJourneyMenu;
    public GameObject pvpMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    [Space]
    public GameObject eventSystemObject;
    private GameObject lastSelectedObject;

    [Header("FirstButtons")]
    public GameObject mainMenuFirstButton;
    public GameObject journeyFirstButton;
    public GameObject loadJourneyFirstButton;
    public GameObject createJourneyFirstButton;
    public GameObject pvpFirstButton;

    [Header("Sounds")]
    public AudioSource navegationSoundFX;
    public AudioSource buttonClickSoundFX;

    void Start()
    {
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

    //Abre o menu para dar load ou criar uma jornada
    public void JourneyButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Fecha outras janelas
        loadJourneyMenu.SetActive(false);
        createJourneyMenu.SetActive(false);
        pvpMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        //Abre a janela pretendida
        journeyMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(journeyFirstButton);
    }

    //Fecha o menu de criar ou dar load a uma jornada e abre o menu apenas de dar load
    public void LoadJourneyButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Fecha outras janelas
        journeyMenu.SetActive(false);
        createJourneyMenu.SetActive(false);
        pvpMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        //Abre a janela pretendida
        loadJourneyMenu.SetActive(true);
        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(loadJourneyFirstButton);

    }

    //Fecha o menu de criar ou dar load a uma jornada e abre o menu apenas de criar
    public void CreateJourneyButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Fecha outras janelas
        journeyMenu.SetActive(false);
        loadJourneyMenu.SetActive(false);
        pvpMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        //Abre a janela pretendida
        createJourneyMenu.SetActive(true);
        SceneManager.LoadScene("StoryMap");

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(createJourneyFirstButton);
    }

    //Abre o menu de criar uma arena 1v1
    public void CreatePvpButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Fecha outras janelas
        journeyMenu.SetActive(false);
        loadJourneyMenu.SetActive(false);
        createJourneyMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        //Abre a janela pretendida
        pvpMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(pvpFirstButton);
    }

    //Abre o menu das definições
    public void SettingsButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Fecha outras janelas
        journeyMenu.SetActive(false);
        loadJourneyMenu.SetActive(false);
        createJourneyMenu.SetActive(false);
        pvpMenu.SetActive(false);
        creditsMenu.SetActive(false);

        //Abre a janela pretendida
        settingsMenu.SetActive(true);


    }

    //Abre a janela dos creditos de desenvolvimento
    public void CreditsButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Fecha outras janelas
        journeyMenu.SetActive(false);
        loadJourneyMenu.SetActive(false);
        createJourneyMenu.SetActive(false);
        pvpMenu.SetActive(false);
        settingsMenu.SetActive(false);

        //Abre a janela pretendida
        creditsMenu.SetActive(true);
    }

    //Fecha o jogo
    public void QuitDesktopButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Dá quit ao jogo
        Application.Quit();
    }
}

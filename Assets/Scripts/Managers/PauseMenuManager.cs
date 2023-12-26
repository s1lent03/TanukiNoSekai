using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("MenusGameobjects")]
    public GameObject pauseMenu;
    public GameObject visualsMenu;
    public GameObject audioMenu;
    public GameObject controlsMenu;
    public GameObject quitMenu;
    [Space]
    public GameObject eventSystemObject;
    private GameObject lastSelectedObject;

    [Header("FirstButtons")]
    public GameObject pauseFirstButton;
    public GameObject visualsFirstButton;
    public GameObject audioFirstButton;
    public GameObject controlsFirstButton;
    public GameObject quitFirstButton;

    [Header("Sounds")]
    public AudioSource navegationSoundFX;
    public AudioSource buttonClickSoundFX;

    [Header("Others")]
    private PlayerInput playerInput;
    public GameObject Player;
    public bool isPaused = false;

    void Start()
    {
        //Atualiza os valores dos mixers
        audioMenu.GetComponent<AudioMenuManager>().UpdateMixersBasedOnFile();

        //Dar um valor default às variaveis
        playerInput = GetComponent<PlayerInput>();

        if (lastSelectedObject == null)
        {
            lastSelectedObject = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject;
        }
    }

    void Update()
    {
        //Parar o jogo ou recomeçar
        if (playerInput.actions["Pause"].triggered && !isPaused && !Player.GetComponentInChildren<TanukiDetection>().isInBattle && !gameObject.GetComponent<ControllerManager>().isPlayerInBattle)
        {
            //Abre o menu
            pauseMenu.SetActive(true);
            isPaused = true;
            Player.GetComponent<PlayerMovement>().isPaused = isPaused;

            //Para qualquer ação do jogador
            StopActions();

            //Escolhe o primeiro butão selecionado deste menu
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(pauseFirstButton);
        }
        else if (playerInput.actions["Pause"].triggered && isPaused == true)
        {
            ResumeButton();
        }

        //Apenas produzir som caso o jogo esteja em pausa
        if (isPaused == true)
        {
            //Sempre que o utilizador clicar num botão para navegar vai produzir um sound effect
            if (lastSelectedObject != eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject)
            {
                navegationSoundFX.Play();
                lastSelectedObject = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject;
            }

            //Prevenir q os botões percam a seleção
            //eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject = pauseFirstButton;
        }        
    }

    //Volta ao jogo
    public void ResumeButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Esconde o menu
        pauseMenu.SetActive(false);
        isPaused = false;
        Player.GetComponent<PlayerMovement>().isPaused = isPaused;

        //Faz o cursor desaparecer
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Deixa o jogador voltar a se mexer;
        ResumeActions();
    }

    //Abre o menu dos visuais/gráficos
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

        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        visualsMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(visualsFirstButton);
    }

    //Abre o menu do audio
    public void AudioButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Setup à janela do audio
        audioMenu.GetComponent<AudioMenuManager>().UpdateSlidersBasedOnFile();

        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        audioMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(audioFirstButton);
    }

    //Abre o menu dos controls
    public void ControlsButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(controlsFirstButton);
    }

    //Abre o menu do audio
    public void QuitButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        quitMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(quitFirstButton);
    }

    //Volta para o menu principal
    public void QuitMainMenuButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Dá load à cena do menu principal
        SceneManager.LoadScene("MainMenu");
    }

    //Fecha o jogo
    public void QuitDesktopButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Dá quit ao jogo
        Application.Quit();
    }

    //Volta de qualquer sub-menu para o menu pausa
    public void BackToPauseMenu()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Desliga os sub-menus
        visualsMenu.SetActive(false);
        audioMenu.SetActive(false);
        controlsMenu.SetActive(false);
        quitMenu.SetActive(false);

        //Liga o menu pausa
        pauseMenu.SetActive(true);

        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(pauseFirstButton);
    }

    private void StopActions()
    {
        //Impede que o jogador se mova enquanto está em pausa
        playerInput.actions.FindAction("Look").Disable();
    }

    private void ResumeActions()
    {
        //Volta a deixar o jogador se mover
        playerInput.actions["Look"].Enable();
    }
}

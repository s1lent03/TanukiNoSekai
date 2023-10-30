using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Others")]
    private PlayerInput playerInput;
    public GameObject Player;
    public bool isPaused = false;

    void Start()
    {
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
        if (playerInput.actions["Pause"].triggered && isPaused == false && Player.GetComponentInChildren<TanukiDetection>().isInBattle == false)
        {
            //Abre o menu
            pauseMenu.SetActive(true);
            isPaused = true;
            Player.GetComponent<PlayerMovement>().isPaused = isPaused;

            //Faz o cursor aparecer
            if (Input.GetJoystickNames().Length <= 0 && Input.GetJoystickNames()[0] == "")
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }            

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
        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        visualsMenu.SetActive(true);
    }

    //Abre o menu do audio
    public void AudioButton()
    {
        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        audioMenu.SetActive(true);
    }

    //Abre o menu dos controls
    public void ControlsButton()
    {
        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    //Abre o menu do audio
    public void QuitButton()
    {
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
        //Dá load à cena do menu principal
        SceneManager.LoadScene("MainMenu");
    }

    //Fecha o jogo
    public void QuitDesktopButton()
    {
        //Dá quit ao jogo
        Application.Quit();
    }

    //Volta de qualquer sub-menu para o menu pausa
    public void BackToPauseMenu()
    {
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

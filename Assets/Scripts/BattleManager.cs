using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject Player;
    [Space]
    private PlayerInput playerInput;
    public GameObject eventSystemObject;
    private GameObject lastSelectedObject;

    [Header("FirstButtons")]
    public GameObject MainBattleMenuFirstButton;

    [Header("Sounds")]
    public AudioSource navegationSoundFX;

    [Header("Others")]
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
        //Apenas produzir som caso o jogo esteja em batalha
        if (Player.GetComponentInChildren<TanukiDetection>().isInBattle == true)
        {
            //Sempre que o utilizador clicar num botão para navegar vai produzir um sound effect
            if (lastSelectedObject != eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject)
            {
                navegationSoundFX.Play();
                lastSelectedObject = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject;
            }
        }
    }

    public void AtivateNav()
    {
        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MainBattleMenuFirstButton);
    }

    //Terminar a batalha
    public void RunButton()
    {
        Player.GetComponentInChildren<TanukiDetection>().EndBattle();
    }
}

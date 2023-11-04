using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("BattleHUD")]
    [SerializeField] TMP_Text playerNameTxt;
    [SerializeField] TMP_Text playerLevelTxt;
    [SerializeField] TMP_Text playerHpTxt;
    [SerializeField] HpBar playerHpBar;
    [Space]
    [SerializeField] TMP_Text enemyNameTxt;
    [SerializeField] TMP_Text enemyLevelTxt;
    [SerializeField] TMP_Text enemyHpTxt;
    [SerializeField] HpBar enemyHpBar;

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

                eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(lastSelectedObject);
            }
        }
    }

    public void AtivateNav()
    {
        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MainBattleMenuFirstButton);
    }

    public void SetData(Tanuki tanuki, bool isPlayer)
    {
        if (isPlayer)
        {
            playerNameTxt.text = tanuki.Base.Name;
            playerLevelTxt.text = "Lvl. " + tanuki.Level;
            playerHpTxt.text = ((tanuki.Hp / tanuki.MaxHp) * 100) + "%";
            playerHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);
        }
        else
        {
            enemyNameTxt.text = tanuki.Base.Name;
            enemyLevelTxt.text = "Lvl. " + tanuki.Level;
            enemyHpTxt.text = ((tanuki.Hp / tanuki.MaxHp) * 100) + "%";
            enemyHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);
        }
    }

    //Terminar a batalha
    public void RunButton()
    {
        Player.GetComponentInChildren<TanukiDetection>().EndBattle();
    }
}

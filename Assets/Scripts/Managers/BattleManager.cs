using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject Player;
    [Space]
    public GameObject ActionSelectionButtons;
    public GameObject MoveSelectionButtons;
    public GameObject MoveInfoBox;
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
    public GameObject MovesMenuFirstButton;

    [Header("MovesButtons")]
    [SerializeField] GameObject Move1ButtonGo;
    [SerializeField] GameObject Move2ButtonGo;
    [SerializeField] GameObject Move3ButtonGo;
    [SerializeField] GameObject Move4ButtonGo;

    [Header("Sounds")]
    public AudioSource navegationSoundFX;
    public AudioSource buttonClickSoundFX;

    [Header("Health Warning")]
    public GameObject healthWarningMat;
    [Space]
    [SerializeField] public float defaultMaskSize;
    [SerializeField] public float defaultOpacityAnim;
    [SerializeField] public float defaultAnimSpeed;
    [Space]
    [SerializeField] float midMaskSize;
    [SerializeField] float midOpacityAnim;
    [SerializeField] float midAnimSpeed;
    [Space]
    [SerializeField] float criticalMaskSize;
    [SerializeField] float criticalOpacityAnim;
    [SerializeField] float criticalAnimSpeed;

    [Header("Others")]
    public bool isPaused = false;
    Tanuki _enemyTanuki;
    Tanuki _playerTanuki;

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

                //Descobrir qual habilidade está selecionada para indicar informação sobre a mesma
                if (eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject != null)
                {
                    int spaceIndex = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.ToString().IndexOf(" ");
                    string buttonName = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.ToString().Substring(0, spaceIndex);
                    switch (buttonName)
                    {
                        case "Move1Btn":
                            gameObject.GetComponent<BattleSystem>().HandleMoveSelection(0);
                            break;

                        case "Move2Btn":
                            gameObject.GetComponent<BattleSystem>().HandleMoveSelection(1);
                            break;

                        case "Move3Btn":
                            gameObject.GetComponent<BattleSystem>().HandleMoveSelection(2);
                            break;

                        case "Move4Btn":
                            gameObject.GetComponent<BattleSystem>().HandleMoveSelection(3);
                            break;
                    }
                }               
            }

            //Se o Tanuki do jogador estiver com pouca vida, ativar um shader de post-processing para demonstrar isso
            string hpText = playerHpTxt.text.Substring(0, playerHpTxt.text.IndexOf("%"));
            int hpPercentage = int.Parse(hpText);
            if (hpPercentage > 50)
            {
                ChangeMatValues(defaultMaskSize, defaultOpacityAnim, defaultAnimSpeed);
            }
            else if (hpPercentage < 50 && hpPercentage > 20)
            {
                ChangeMatValues(midMaskSize, midOpacityAnim, midAnimSpeed);
            }
            else if (hpPercentage < 20)
            {
                ChangeMatValues(criticalMaskSize, criticalOpacityAnim, criticalAnimSpeed);
            }

            //Impedir que o jogador utilize ataques enquanto outros ataques estão a ser usados
            if (gameObject.GetComponent<BattleSystem>().state == BattleState.PlayerAction)
            {
                EnableMoveButtons();
            }
            else
            {
                DisableMoveButtons();
            }
        }
    }

    public void AtivateNav()
    {
        //Escolhe o primeiro butão selecionado deste menu
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MainBattleMenuFirstButton);
    }

    //Desabilitar os botões
    public void DisableMoveButtons()
    {
        Move1ButtonGo.GetComponent<Button>().interactable = false;
        Move2ButtonGo.GetComponent<Button>().interactable = false;
        Move3ButtonGo.GetComponent<Button>().interactable = false;
        Move4ButtonGo.GetComponent<Button>().interactable = false;
    }

    //Habilitar os botões
    public void EnableMoveButtons()
    {
        Move1ButtonGo.GetComponent<Button>().interactable = true;
        Move2ButtonGo.GetComponent<Button>().interactable = true;
        Move3ButtonGo.GetComponent<Button>().interactable = true;
        Move4ButtonGo.GetComponent<Button>().interactable = true;

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MovesMenuFirstButton);
    }

    //Alterar valores do material de health warning
    public void ChangeMatValues(float maskSize, float opAnim, float animSpeed)
    {
        /*healthWarningMat.GetComponent<Material>().SetFloat("_Mask_Size", maskSize);
        healthWarningMat.GetComponent<Material>().SetFloat("_Opacity_Animation", opAnim);
        healthWarningMat.GetComponent<Material>().SetFloat("_Animation_Speed", animSpeed);*/
        var pass = healthWarningMat.GetComponent<CustomPassVolume>().customPasses[0];

        if (pass is FullScreenCustomPass full)
        {
            full.fullscreenPassMaterial.SetFloat("_Mask_Size", maskSize);
            full.fullscreenPassMaterial.SetFloat("_Opacity_Animation", opAnim);
            full.fullscreenPassMaterial.SetFloat("_Animation_Speed", animSpeed);
        }
            
    }

    public void SetData(Tanuki tanuki, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerTanuki = tanuki;

            playerNameTxt.text = tanuki.Base.Name;
            playerLevelTxt.text = "Lvl. " + tanuki.Level;
            playerHpTxt.text = (tanuki.Hp / tanuki.MaxHp * 100) + "%";
            playerHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);
        }
        else
        {
            _enemyTanuki = tanuki;

            enemyNameTxt.text = tanuki.Base.Name;
            enemyLevelTxt.text = "Lvl. " + tanuki.Level;
            enemyHpTxt.text = (tanuki.Hp / tanuki.MaxHp * 100) + "%";
            enemyHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);
        }
    }

    public IEnumerator UpdateHP()
    {
        playerHpTxt.text = (int)((float)_playerTanuki.Hp / (float)_playerTanuki.MaxHp * 100) + "%";
        yield return playerHpBar.SetHpSmooth((float)_playerTanuki.Hp / _playerTanuki.MaxHp);

        enemyHpTxt.text = (int)((float)_enemyTanuki.Hp / (float)_enemyTanuki.MaxHp * 100) + "%";
        yield return enemyHpBar.SetHpSmooth((float)_enemyTanuki.Hp / _enemyTanuki.MaxHp);
    }

    //Utiliza a habilidade 1
    public void Move1Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PerformPlayerMove(0));
    }

    //Utiliza a habilidade 2
    public void Move2Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PerformPlayerMove(1));
    }

    //Utiliza a habilidade 3
    public void Move3Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PerformPlayerMove(2));
    }

    //Utiliza a habilidade 4
    public void Move4Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PerformPlayerMove(3));
    }

    //Mostra as habilidades
    public void BattleButton()
    {
        buttonClickSoundFX.Play();

        ActionSelectionButtons.SetActive(false);
        MoveSelectionButtons.SetActive(true);

        MoveInfoBox.SetActive(true);

        gameObject.GetComponent<BattleSystem>().PlayerMove();

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MovesMenuFirstButton);
    }

    //Mostra as ações
    public void BackToActionsButton()
    {
        buttonClickSoundFX.Play();

        ActionSelectionButtons.SetActive(true);
        MoveSelectionButtons.SetActive(false);

        MoveInfoBox.SetActive(false);

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MainBattleMenuFirstButton);
    }

    //Terminar a batalha
    public void RunButton()
    {
        buttonClickSoundFX.Play();

        Player.GetComponentInChildren<TanukiDetection>().EndBattle();
    }
}

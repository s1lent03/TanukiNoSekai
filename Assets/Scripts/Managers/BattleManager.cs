using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Rendering;

public class BattleManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject Player;
    [Space]
    public GameObject ActionSelectionButtons;
    public GameObject MoveSelectionButtons;
    public GameObject MoveInfoBox;
    [Space]
    public GameObject PartyMenu;
    public GameObject DialogBox;
    [Space]
    private PlayerInput playerInput;
    public GameObject eventSystemObject;
    private GameObject lastSelectedObject;

    [Header("BattleHUD")]
    [SerializeField] Image playerStatusImage;
    [SerializeField] TMP_Text playerNameTxt;
    [SerializeField] TMP_Text playerLevelTxt;
    [SerializeField] TMP_Text playerHpTxt;
    [SerializeField] HpBar playerHpBar;
    [SerializeField] HpBar playerXpBar;
    [Space]
    [SerializeField] Image enemyStatusImage;
    [SerializeField] TMP_Text enemyNameTxt;
    [SerializeField] TMP_Text enemyLevelTxt;
    [SerializeField] TMP_Text enemyHpTxt;
    [SerializeField] HpBar enemyHpBar;

    [Header("TanukiPartyObjects")]
    [SerializeField] PartyMemberUI[] memberSlots;
    [Space]
    [SerializeField] TMP_Text TypesInfoTxt;
    [SerializeField] TMP_Text MovesInfoTxt;

    [Header("FirstButtons")]
    public GameObject MainBattleMenuFirstButton;
    public GameObject MovesMenuFirstButton;
    public GameObject PartyMenuRunButton;
    public GameObject PartyMenuBackButton;

    [Header("MovesButtons")]
    [SerializeField] GameObject Move1ButtonGo;
    [SerializeField] GameObject Move2ButtonGo;
    [SerializeField] GameObject Move3ButtonGo;
    [SerializeField] GameObject Move4ButtonGo;

    [Header("ActionButtons")]
    [SerializeField] GameObject Action1ButtonGo;
    [SerializeField] GameObject Action2ButtonGo;
    [SerializeField] GameObject Action4ButtonGo;

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
    [SerializeField] bool doOnceMoves = false;
    [SerializeField] bool doOnceActions = false;
    Tanuki _enemyTanuki;
    Tanuki _playerTanuki;

    [SerializeField] GameObject EvolveEffect;
    [SerializeField] int TimeToEvolve;

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
            else if (hpPercentage <= 50 && hpPercentage >= 20)
            {
                ChangeMatValues(midMaskSize, midOpacityAnim, midAnimSpeed);
            }
            else if (hpPercentage < 20)
            {
                ChangeMatValues(criticalMaskSize, criticalOpacityAnim, criticalAnimSpeed);
            }

            //Impedir que o jogador utilize ataques enquanto outros ataques estão a ser usados
            if (gameObject.GetComponent<BattleSystem>().state == BattleState.MoveSelection)
            {
                EnableMoveButtons();
            }
            else
            {
                DisableMoveButtons();
            }

            //Impedir que o utilizador utilize botões enquanto dialogo está a ser escrito
            if (gameObject.GetComponent<BattleSystem>().state == BattleState.ActionSelection)
            {
                EnableActionButtons();
            }
            else
            {
                DisableActionButtons();
            }

            //Procurar informações sobre o tanuki selecionado no party selection
            if (PartyMenu.activeSelf)
            {
                GameObject currentTanukiSelected = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;

                //Descobrir os moves do tanuki selecionado
                switch (currentTanukiSelected.name)
                {
                    case "Tanuki1":
                        MovesInfoTxt.text = memberSlots[0].GetMoves(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[0]);
                        break;

                    case "Tanuki2":
                        MovesInfoTxt.text = memberSlots[1].GetMoves(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[1]);
                        break;

                    case "Tanuki3":
                        MovesInfoTxt.text = memberSlots[2].GetMoves(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[2]);
                        break;

                    case "Tanuki4":
                        MovesInfoTxt.text = memberSlots[3].GetMoves(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[3]);
                        break;

                    case "Tanuki5":
                        MovesInfoTxt.text = memberSlots[4].GetMoves(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[4]);
                        break;
                }

                //Descobrir o(s) tipo(s) do tanuki selecionado
                switch (currentTanukiSelected.name)
                {
                    case "Tanuki1":
                        TypesInfoTxt.text = memberSlots[0].GetTypes(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[0]);
                        break;

                    case "Tanuki2":
                        TypesInfoTxt.text = memberSlots[1].GetTypes(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[1]);
                        break;

                    case "Tanuki3":
                        TypesInfoTxt.text = memberSlots[2].GetTypes(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[2]);
                        break;

                    case "Tanuki4":
                        TypesInfoTxt.text = memberSlots[3].GetTypes(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[3]);
                        break;

                    case "Tanuki5":
                        TypesInfoTxt.text = memberSlots[4].GetTypes(gameObject.GetComponent<BattleSystem>().playerParty.Tanukis[4]);
                        break;
                }
            }
        }
        else
        {
            ChangeMatValues(defaultMaskSize, defaultOpacityAnim, defaultAnimSpeed);
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

        doOnceMoves = false;
    }

    //Habilitar os botões
    public void EnableMoveButtons()
    {
        Move1ButtonGo.GetComponent<Button>().interactable = true;
        Move2ButtonGo.GetComponent<Button>().interactable = true;
        Move3ButtonGo.GetComponent<Button>().interactable = true;
        Move4ButtonGo.GetComponent<Button>().interactable = true;

        if (doOnceMoves == false)
        {
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MovesMenuFirstButton);

            doOnceMoves = true;
        }
    }

    //Desabilitar os botões
    public void DisableActionButtons()
    {
        Action1ButtonGo.GetComponent<Button>().interactable = false;
        Action2ButtonGo.GetComponent<Button>().interactable = false;
        Action4ButtonGo.GetComponent<Button>().interactable = false;

        doOnceActions = false;
    }

    //Habilitar os botões
    public void EnableActionButtons()
    {
        Action1ButtonGo.GetComponent<Button>().interactable = true;
        Action2ButtonGo.GetComponent<Button>().interactable = true;
        Action4ButtonGo.GetComponent<Button>().interactable = true;

        if (doOnceActions == false)
        {
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MainBattleMenuFirstButton);

            BackToActionsButton();

            doOnceActions = true;
        }
    }

    //Alterar valores do material de health warning
    public void ChangeMatValues(float maskSize, float opAnim, float animSpeed)
    {
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
            playerHpTxt.text = (int)((float)tanuki.Hp / tanuki.MaxHp * 100) + "%";
            playerHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);

            float currentLevelXP = Mathf.Pow(tanuki.Level, 3);
            float nextLevelXP = Mathf.Pow(tanuki.Level + 1, 3);
            float xpProgress = (tanuki.XpPoints - currentLevelXP) / (nextLevelXP - currentLevelXP);

            xpProgress = Mathf.Clamp01(xpProgress);
            playerXpBar.SetXp(xpProgress);
        }
        else
        {
            _enemyTanuki = tanuki;

            enemyNameTxt.text = tanuki.Base.Name;
            enemyLevelTxt.text = "Lvl. " + tanuki.Level;
            enemyHpTxt.text = (int)((float)tanuki.Hp / tanuki.MaxHp * 100) + "%";
            enemyHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_playerTanuki.HpChanged)
        {
            playerHpTxt.text = (int)((float)_playerTanuki.Hp / (float)_playerTanuki.MaxHp * 100) + "%";
            yield return playerHpBar.SetHpSmooth((float)_playerTanuki.Hp / _playerTanuki.MaxHp);

            _playerTanuki.HpChanged = false;
        }

        if (_enemyTanuki.HpChanged)
        {
            enemyHpTxt.text = (int)((float)_enemyTanuki.Hp / (float)_enemyTanuki.MaxHp * 100) + "%";
            yield return enemyHpBar.SetHpSmooth((float)_enemyTanuki.Hp / _enemyTanuki.MaxHp);

            _enemyTanuki.HpChanged = false;
        }        
    }

    public IEnumerator UpdateXP()
    {
        float currentLevelXP = Mathf.Pow(_playerTanuki.Level, 3);
        float nextLevelXP = Mathf.Pow(_playerTanuki.Level + 1, 3);
        float xpProgress = (_playerTanuki.XpPoints - currentLevelXP) / (nextLevelXP - currentLevelXP);

        xpProgress = Mathf.Clamp01(xpProgress);       
        yield return playerXpBar.SetXpSmooth(xpProgress);
        playerXpBar.SetXp(xpProgress);

        if (xpProgress == 1)
            _playerTanuki.Level++;

        playerLevelTxt.text = "Lvl. " + _playerTanuki.Level;

        if (_playerTanuki.Level == _playerTanuki.Base.EvolveLevel)
        {
            gameObject.GetComponent<ControllerManager>().switchCam = true;

            BattleUnit playerTanuki = gameObject.GetComponent<BattleSystem>().playerUnit;
            GameObject EvEffect = Instantiate(EvolveEffect, playerTanuki.transform.GetChild(0).transform.Find("ModelObject").transform.position, Quaternion.identity, playerTanuki.transform);

            EvEffect.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1.7f);
            yield return new WaitForSeconds(TimeToEvolve * 1.7f);

            GameObject EvolveTanuki = Instantiate(_playerTanuki.Base.EvolveModel.gameObject, playerTanuki.transform.GetChild(0).transform.Find("ModelObject").transform.position, Quaternion.identity, playerTanuki.transform);
            EvolveTanuki.GetComponent<TanukiMovement>().stunned = true;
            EvolveTanuki.transform.Find("ModelObject").DOScale(new Vector3(1, 1, 1), 1);
            Destroy(playerTanuki.transform.GetChild(0).gameObject);

            yield return new WaitForSeconds(1.7f);
            EvEffect.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1.7f);
            yield return new WaitForSeconds(1.7f);
            Destroy(EvEffect);

            _playerTanuki._base = _playerTanuki.Base.EvolveModel.tanukiUnitData.Base;
        }
    }

    public void SetPartyData(List<Tanuki> tanukis)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < tanukis.Count)
                memberSlots[i].SetData(tanukis[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }
    }

    //Utiliza a habilidade 1
    public void Move1Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PlayerMove(0));
    }

    //Utiliza a habilidade 2
    public void Move2Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PlayerMove(1));
    }

    //Utiliza a habilidade 3
    public void Move3Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PlayerMove(2));
    }

    //Utiliza a habilidade 4
    public void Move4Button()
    {
        buttonClickSoundFX.Play();
        StartCoroutine(gameObject.GetComponent<BattleSystem>().PlayerMove(3));
    }

    //Mostra as habilidades
    public void BattleButton()
    {
        buttonClickSoundFX.Play();

        ActionSelectionButtons.SetActive(false);
        MoveSelectionButtons.SetActive(true);

        MoveInfoBox.SetActive(true);

        gameObject.GetComponent<BattleSystem>().MoveSelection();

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MovesMenuFirstButton);
    }

    //Mostra as ações
    public void BackToActionsButton()
    {
        buttonClickSoundFX.Play();

        ActionSelectionButtons.SetActive(true);
        MoveSelectionButtons.SetActive(false);
        PartyMenu.SetActive(false);

        MoveInfoBox.SetActive(false);
        DialogBox.SetActive(true);

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(MainBattleMenuFirstButton);

        if (gameObject.GetComponent<BattleSystem>().state == BattleState.PartyScreen || gameObject.GetComponent<BattleSystem>().state == BattleState.MoveSelection)
            gameObject.GetComponent<BattleSystem>().state = BattleState.ActionSelection;
    }

    //Mostrar party de Tanukis
    public void PartySelection()
    {
        buttonClickSoundFX.Play();

        ActionSelectionButtons.SetActive(false);
        MoveSelectionButtons.SetActive(false);
        PartyMenu.SetActive(true);

        MoveInfoBox.SetActive(false);
        DialogBox.SetActive(false);

        if (gameObject.GetComponent<BattleSystem>().state == BattleState.PerformMove)
        {
            PartyMenuRunButton.SetActive(true);
            PartyMenuBackButton.SetActive(false);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(PartyMenuRunButton);
        }
        else
        {
            PartyMenuRunButton.SetActive(false);
            PartyMenuBackButton.SetActive(true);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(PartyMenuBackButton);
        }

        gameObject.GetComponent<BattleSystem>().OpenPartyScreen();

    }

    //Escolher o tanuki a substituir
    public void ChooseTanukiFromParty()
    {
        GameObject currentTanukiSelected = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;
        switch (currentTanukiSelected.name)
        {
            case "Tanuki1":
                gameObject.GetComponent<BattleSystem>().HandlePartySelection(0);
                break;

            case "Tanuki2":
                gameObject.GetComponent<BattleSystem>().HandlePartySelection(1);
                break;

            case "Tanuki3":
                gameObject.GetComponent<BattleSystem>().HandlePartySelection(2);
                break;

            case "Tanuki4":
                gameObject.GetComponent<BattleSystem>().HandlePartySelection(3);
                break;

            case "Tanuki5":
                gameObject.GetComponent<BattleSystem>().HandlePartySelection(4);
                break;
        }
    }

    //Alterar a cor do efeito que está a ser utilizado (paralyse, burn, etc)
    public void ChangeEffectFeedbackImage(bool isPlayer, Color newColor)
    {
        if(isPlayer)
            playerStatusImage.color = newColor;
        else
            enemyStatusImage.color = newColor;
    }

    //Terminar a batalha
    public void RunButton()
    {
        buttonClickSoundFX.Play();

        Player.GetComponentInChildren<TanukiDetection>().EndBattle();
    }
}

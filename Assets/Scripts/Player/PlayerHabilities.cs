using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PlayerHabilities : MonoBehaviour
{
    [Header("Lantern")]
    [SerializeField] GameObject Lantern;
    bool isLanternOut = false;

    [Header("Throw Ball")]
    [SerializeField] List<GameObject> BallVariants;
    [SerializeField] GameObject Ball;
    [SerializeField] Transform ThrowPoint;
    [Space]
    [SerializeField] float ThrowForce;
    [SerializeField] float ThrowCoolDown;
    bool canThrowNextBall = true;

    [Header("Ball Sprites")]
    [SerializeField] Image BallSprite;
    [SerializeField] TMP_Text NumBallsTxt;
    [SerializeField] Sprite Ball1Sprite;
    [SerializeField] Sprite Ball2Sprite;
    [SerializeField] Sprite Ball3Sprite;

    [Header("Berry Sprites")]
    [SerializeField] Image BerrySprite;
    [SerializeField] TMP_Text NumBerriesTxt;
    [SerializeField] Sprite Berry1Sprite;
    [SerializeField] Sprite Berry2Sprite;
    [SerializeField] Sprite Berry3Sprite;

    [Header("Drop Berry")]
    [SerializeField] List<GameObject> BerryVariants;
    [SerializeField] GameObject Berry;
    [SerializeField] Transform DropPoint;
    [Space]
    [SerializeField] float DropForce;
    [SerializeField] float DropCoolDown;
    bool canDropNextBerry = true;

    [Header("Party")]
    [SerializeField] GameObject eventSystemObject;
    [SerializeField] GameObject PartyScreenLetGo;
    [SerializeField] GameObject PartyMenuRunButtonLetGo;
    [Space]
    [SerializeField] PartyMemberUI[] memberSlotsLetGo;
    [SerializeField] TMP_Text MovesInfoLetGoTxt;
    [SerializeField] TMP_Text TypesInfoLetGoTxt;
    [Space]
    [SerializeField] GameObject PartyScreenToSee;
    [SerializeField] GameObject PartyMenuRunButtonToSee;
    [Space]
    [SerializeField] PartyMemberUI[] memberSlotsToSee;
    [SerializeField] TMP_Text MovesInfoToSeeTxt;
    [SerializeField] TMP_Text TypesInfoToSeeTxt;
    [Space]
    GameObject NewTanuki;
    int TanukiToLetGoIndex;

    [Header("Others")]
    [SerializeField] GameObject Managers;
    [SerializeField] Camera mainCamera;
    PlayerInput playerInput;    

    Vector3 ThrowStartPoint;
    Vector3 DropStartPoint;

    void Start()
    {
        //Dar um valor default às variaveis
        playerInput = GetComponent<PlayerInput>();
        PlayerPrefs.SetInt("CurrentBallLevel", 1);
        PlayerPrefs.SetInt("CurrentBerryLevel", 1);      
    }

    IEnumerator ToggleLight()
    {
        GetComponent<PlayerMovement>().isPaused = true;

        if (!isLanternOut)
        {
            GetComponent<PlayerMovement>().animator.SetTrigger(Animator.StringToHash("LightsOn"));
            yield return new WaitForSeconds(0.75f);
            Lantern.SetActive(true);
            isLanternOut = true;
        } else
        {
            GetComponent<PlayerMovement>().animator.SetTrigger(Animator.StringToHash("LightsOff"));
            yield return new WaitForSeconds(0.75f);
            isLanternOut = false;
            Lantern.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        GetComponent<PlayerMovement>().isPaused = false;
    }

    void Update()
    {
        //Sacar lanterna
        if (playerInput.actions["Lantern"].triggered)
        {
            StartCoroutine(ToggleLight());
        }

        //Lançar bola quando primida a devida tecla e caso o jogo não esteja em pausa nem em batalha
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && playerInput.actions["ThrowBall"].triggered && canThrowNextBall)
        {
            //Atirar bola
            if (PlayerPrefs.GetInt("NumberOfBall" + PlayerPrefs.GetInt("CurrentBallLevel")) > 0)
            {
                StartCoroutine(ThrowBall(PlayerPrefs.GetInt("CurrentBallLevel") - 1));
                PlayerPrefs.SetInt("NumberOfBall" + PlayerPrefs.GetInt("CurrentBallLevel"), PlayerPrefs.GetInt("NumberOfBall" + PlayerPrefs.GetInt("CurrentBallLevel")) - 1);
            }
        }

        //Trocar de bola selecionada
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && playerInput.actions["ChangeBall"].triggered)
        {        
            switch (PlayerPrefs.GetInt("CurrentBallLevel"))
            {
                case 1:
                    PlayerPrefs.SetInt("CurrentBallLevel", 2);
                    ChangeBallSpriteAndValues(Ball2Sprite);
                    break;
                case 2:
                    PlayerPrefs.SetInt("CurrentBallLevel", 3);
                    ChangeBallSpriteAndValues(Ball3Sprite);
                    break;
                case 3:
                    PlayerPrefs.SetInt("CurrentBallLevel", 1);
                    ChangeBallSpriteAndValues(Ball1Sprite);
                    break;                    
            }            
        }

        //Trocar de berry selecionada
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && playerInput.actions["ChangeBerry"].triggered)
        {
            switch (PlayerPrefs.GetInt("CurrentBerryLevel"))
            {
                case 1:
                    PlayerPrefs.SetInt("CurrentBerryLevel", 2);
                    ChangeBerrySpriteAndValues(Berry2Sprite);
                    break;
                case 2:
                    PlayerPrefs.SetInt("CurrentBerryLevel", 3);
                    ChangeBerrySpriteAndValues(Berry3Sprite);
                    break;
                case 3:
                    PlayerPrefs.SetInt("CurrentBerryLevel", 1);
                    ChangeBerrySpriteAndValues(Berry1Sprite);
                    break;
            }
        }

        //Abrir o menu de party dos Tanuki
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && playerInput.actions["ShowParty"].triggered && !PartyScreenToSee.activeInHierarchy)
        {
            //Abre o menu de party
            Managers.GetComponent<ControllerManager>().isPlayerInBattle = true;
            gameObject.GetComponent<PlayerMovement>().isPaused = true;

            PartyScreenToSee.SetActive(true);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(PartyMenuRunButtonToSee);

            UpdateToSeeParty();
        }
        else if (Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && playerInput.actions["ShowParty"].triggered && PartyScreenToSee.activeInHierarchy)
        {
            BackTanukiParty();
        }

        //Valor de quantas bolas e berries a selecionada possui
        NumBallsTxt.text = PlayerPrefs.GetInt("NumberOfBall" + PlayerPrefs.GetInt("CurrentBallLevel")) + "x";
        NumBerriesTxt.text = PlayerPrefs.GetInt("NumberOfBerry" + PlayerPrefs.GetInt("CurrentBerryLevel")) + "x";

        //Atualizar valores das parties
        if (PartyScreenLetGo.activeSelf)
        {
            MovesInfoLetGoTxt.text = TanukiPartyMovesInfo(memberSlotsLetGo);
            TypesInfoLetGoTxt.text = TanukiPartyTypesInfo(memberSlotsLetGo);
        }

        if (PartyScreenToSee.activeSelf)
        {
            MovesInfoToSeeTxt.text = TanukiPartyMovesInfo(memberSlotsToSee);
            TypesInfoToSeeTxt.text = TanukiPartyTypesInfo(memberSlotsToSee);
        }      
    }

    public void UpdateToSeeParty()
    {
        List<Tanuki> tanukis = gameObject.GetComponent<TanukiParty>().tanukis;
        for (int i = 0; i < memberSlotsToSee.Length; i++)
        {
            if (i < tanukis.Count)
            {
                memberSlotsToSee[i].SetData(tanukis[i]);
            }
            else
                memberSlotsToSee[i].gameObject.SetActive(false);
        }
    }

    string TanukiPartyMovesInfo(PartyMemberUI[] memberSlots)
    {
        //Procurar informações sobre o tanuki selecionado no party selection
        GameObject currentTanukiSelected = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;

        //Descobrir os moves do tanuki selecionado
        switch (currentTanukiSelected.name)
        {
            case "Tanuki1":
                return memberSlots[0].GetMoves(gameObject.GetComponent<TanukiParty>().Tanukis[0]);
                break;

            case "Tanuki2":
                return memberSlots[1].GetMoves(gameObject.GetComponent<TanukiParty>().Tanukis[1]);
                break;

            case "Tanuki3":
                return memberSlots[2].GetMoves(gameObject.GetComponent<TanukiParty>().Tanukis[2]);
                break;

            case "Tanuki4":
                return memberSlots[3].GetMoves(gameObject.GetComponent<TanukiParty>().Tanukis[3]);
                break;

            case "Tanuki5":
                return memberSlots[4].GetMoves(gameObject.GetComponent<TanukiParty>().Tanukis[4]);
                break;
            default:
                return "";
                break;
        }
    }

    string TanukiPartyTypesInfo(PartyMemberUI[] memberSlots)
    {
        //Procurar informações sobre o tanuki selecionado no party selection
        GameObject currentTanukiSelected = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;

        //Descobrir o(s) tipo(s) do tanuki selecionado
        switch (currentTanukiSelected.name)
        {
            case "Tanuki1":
                return memberSlots[0].GetTypes(gameObject.GetComponent<TanukiParty>().Tanukis[0]);
                break;

            case "Tanuki2":
                return memberSlots[1].GetTypes(gameObject.GetComponent<TanukiParty>().Tanukis[1]);
                break;

            case "Tanuki3":
                return memberSlots[2].GetTypes(gameObject.GetComponent<TanukiParty>().Tanukis[2]);
                break;

            case "Tanuki4":
                return memberSlots[3].GetTypes(gameObject.GetComponent<TanukiParty>().Tanukis[3]);
                break;

            case "Tanuki5":
                return memberSlots[4].GetTypes(gameObject.GetComponent<TanukiParty>().Tanukis[4]);
                break;
            default:
                return "";
                break;
        }
    }

    void ChangeBallSpriteAndValues(Sprite ballSprite)
    {
        //Trocar a sprite da bola e o numero de bolas da selecionada
        BallSprite.sprite = ballSprite;        
    }

    void ChangeBerrySpriteAndValues(Sprite berrySprite)
    {
        //Trocar a sprite da berry e o numero de berries da selecionada
        BerrySprite.sprite = berrySprite;
    }

    public void PlayDropBerry(GameObject Tanuki)
    {
        //Dropar berries para apanhar o Tanuki
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && canDropNextBerry)
        {

            if (PlayerPrefs.GetInt("NumberOfBerry" + PlayerPrefs.GetInt("CurrentBerryLevel")) > 0)
            {
                StartCoroutine(DropBerry(PlayerPrefs.GetInt("CurrentBerryLevel") - 1, Tanuki));
                PlayerPrefs.SetInt("NumberOfBerry" + PlayerPrefs.GetInt("CurrentBerryLevel"), PlayerPrefs.GetInt("NumberOfBerry" + PlayerPrefs.GetInt("CurrentBerryLevel")) - 1);
            }
        }
    } 

    IEnumerator DropBerry(int berryIndex, GameObject tanuki)
    {
        canDropNextBerry = false;

        //Assinar a berry selecionada 
        Berry = BerryVariants[berryIndex];

        //Criar variaveis para guardar os dados de onde a berry vai sair e em que direção ela vai
        DropStartPoint = DropPoint.position;
        Vector3 lookAtPosition = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).direction;

        // Animação
        GetComponent<PlayerMovement>().animator.SetTrigger(Animator.StringToHash("Berry"));
        GetComponent<PlayerMovement>().isPaused = true;
        yield return new WaitForSeconds(2.3f);

        //Criar a berry e fazer com que vá para a boca do tanuki
        GameObject BerryToDrop = Instantiate(Berry, DropStartPoint, Quaternion.identity);
        Transform Mouth = tanuki.gameObject.transform.Find("ModelObject").transform.Find("Model").transform.Find("Armature").transform.Find("Body").transform.Find("Mouth").gameObject.transform;
        BerryToDrop.transform.DOMove(new Vector3(Mouth.position.x, Mouth.position.y, Mouth.position.z), 1f);

        //Destruir berry
        StartCoroutine(DestroyBall(BerryToDrop, 1f));
        yield return new WaitForSeconds(1f);

        // Tirar o freeze
        yield return new WaitForSeconds(0.867f);
        GetComponent<PlayerMovement>().isPaused = false;

        //Apanhar Tanuki
        int currentBerryNum = PlayerPrefs.GetInt("CurrentBerryLevel");

        int rand = Random.Range(1, tanuki.gameObject.GetComponent<BattleUnit>().tanukiUnitData.Level / (currentBerryNum * 2));

        if (rand <= currentBerryNum * 2)
        {
            if (gameObject.GetComponent<TanukiParty>().Tanukis.Count == 5)
            {
                //Impedir o tanuki de atacar
                tanuki.gameObject.GetComponent<TanukiMovement>().isBeingCaught = true;

                //Abre o menu de party para escolher qual Tanuki vai ser trocado
                Managers.GetComponent<ControllerManager>().isPlayerInBattle = true;
                gameObject.GetComponent<PlayerMovement>().isPaused = true;

                PartyScreenLetGo.SetActive(true);
                eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(PartyMenuRunButtonLetGo);

                List<Tanuki> tanukis = gameObject.GetComponent<TanukiParty>().tanukis;
                for (int i = 0; i < memberSlotsLetGo.Length; i++)
                {
                    if (i < tanukis.Count)
                    {
                        memberSlotsLetGo[i].SetData(tanukis[i]);
                    }
                    else
                        memberSlotsLetGo[i].gameObject.SetActive(false);
                }

                NewTanuki = tanuki;
            }
            else
            {
                //Adiciona o tanuki à party
                gameObject.GetComponent<TanukiParty>().Tanukis.Add(tanuki.gameObject.GetComponent<BattleUnit>().tanukiUnitData);

                tanuki.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1f);
                yield return new WaitForSeconds(1f);
                Destroy(tanuki.gameObject);
            }
        }       

        //Esperar algum tempo para mandar outra
        yield return new WaitForSeconds(DropCoolDown);
        canDropNextBerry = true;
    }

    IEnumerator ThrowBall(int ballIndex)
    {
        canThrowNextBall = false;

        //Assinar a bola selecionada
        Ball = BallVariants[ballIndex];

        //Criar variaveis para guardar os dados de onde a bola vai sair e em que direção ela vai
        ThrowStartPoint = ThrowPoint.position;
        Vector3 lookAtPosition = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).direction;

        // Animação
        GetComponent<PlayerMovement>().animator.SetTrigger(Animator.StringToHash("Throw"));
        GetComponent<PlayerMovement>().isPaused = true;
        yield return new WaitForSeconds(2f);

        //Criar a bola e dar-lhe força para ir na direção criada anteriormente
        GameObject BallToThrow = Instantiate(Ball, ThrowStartPoint, Quaternion.identity);
        BallToThrow.GetComponent<Rigidbody>().AddForce(lookAtPosition * ThrowForce);

        //Destruir bola
        StartCoroutine(DestroyBall(BallToThrow, 10f));

        // Tirar o freeze
        yield return new WaitForSeconds(1f);
        GetComponent<PlayerMovement>().isPaused = false;

        //Esperar algum tempo para mandar outra
        yield return new WaitForSeconds(ThrowCoolDown);
        canThrowNextBall = true;
    }

    //Destruir a pedra após algum tempo
    IEnumerator DestroyBall(GameObject ball, float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);

        Destroy(ball);
    }

    public void LetTanukiGo()
    {
        //Troca o tanuki selecionado pelo apanhado
        gameObject.GetComponent<TanukiParty>().Tanukis[int.Parse(eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject.name)] = NewTanuki.gameObject.GetComponent<BattleUnit>().tanukiUnitData;

        StartCoroutine(DestroyWildTanuki(NewTanuki));

        //Fecha o menu de party
        Managers.GetComponent<ControllerManager>().isPlayerInBattle = false;
        gameObject.GetComponent<PlayerMovement>().isPaused = false;

        PartyScreenLetGo.SetActive(false);
    }

    IEnumerator DestroyWildTanuki(GameObject tanukiToDestroy)
    {
        //Destruir o modelo do tanuki selvagem
        tanukiToDestroy.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1f);
        yield return new WaitForSeconds(1f);
        Destroy(tanukiToDestroy.gameObject);
    }

    public void BackTanukiParty()
    {
        //Fecha o menu de party
        Managers.GetComponent<ControllerManager>().isPlayerInBattle = false;
        gameObject.GetComponent<PlayerMovement>().isPaused = false;

        PartyScreenToSee.SetActive(false);
    }

    public void RunTanukiParty()
    {
        //Impedir o tanuki de atacar
        NewTanuki.gameObject.GetComponent<TanukiMovement>().isBeingCaught = false;

        //Abre o menu de party para escolher qual Tanuki vai ser trocado
        Managers.GetComponent<ControllerManager>().isPlayerInBattle = false;
        gameObject.GetComponent<PlayerMovement>().isPaused = false;

        PartyScreenLetGo.SetActive(false);
    }
}

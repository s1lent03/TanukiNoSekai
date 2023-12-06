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
    [SerializeField] GameObject PartyScreen;
    [SerializeField] GameObject PartyMenuRunButton;
    [Space]
    [SerializeField] PartyMemberUI[] memberSlots;

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

    void Update()
    {
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

        //Valor de quantas bolas e berries a selecionada possui
        NumBallsTxt.text = PlayerPrefs.GetInt("NumberOfBall" + PlayerPrefs.GetInt("CurrentBallLevel")) + "x";
        NumBerriesTxt.text = PlayerPrefs.GetInt("NumberOfBerry" + PlayerPrefs.GetInt("CurrentBerryLevel")) + "x";
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

        //Criar a berry e fazer com que vá para a boca do tanuki
        GameObject BerryToDrop = Instantiate(Berry, DropStartPoint, Quaternion.identity);
        Transform Mouth = tanuki.gameObject.transform.Find("ModelObject").transform.Find("Mouth").gameObject.transform;
        BerryToDrop.transform.DOMove(new Vector3(Mouth.position.x, Mouth.position.y, Mouth.position.z), 1f);

        //Destruir berry
        StartCoroutine(DestroyBall(BerryToDrop, 1f));
        yield return new WaitForSeconds(1f);

        //Apanhar Tanuki
        int currentBerryNum = PlayerPrefs.GetInt("CurrentBerryLevel");

        int rand = Random.Range(1, tanuki.gameObject.GetComponent<BattleUnit>().tanukiUnitData.Level / (currentBerryNum * 2));

        Debug.Log("Current berry num: " + currentBerryNum);
        Debug.Log("Random: " + rand);
        Debug.Log("Tanuki lvl: " + tanuki.gameObject.GetComponent<BattleUnit>().tanukiUnitData.Level);

        if (rand <= currentBerryNum * 2)
        {
            if (gameObject.GetComponent<TanukiParty>().Tanukis.Count == 5)
            {
                //Abre o menu de party para escolher qual Tanuki vai ser trocado
                Managers.GetComponent<ControllerManager>().isPlayerInBattle = true;
                gameObject.GetComponent<PlayerMovement>().isPaused = true;

                PartyScreen.SetActive(true);
                eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(PartyMenuRunButton);

                List<Tanuki> tanukis = gameObject.GetComponent<TanukiParty>().tanukis;
                for (int i = 0; i < memberSlots.Length; i++)
                {
                    if (i < tanukis.Count)
                        memberSlots[i].SetData(tanukis[i]);
                    else
                        memberSlots[i].gameObject.SetActive(false);
                }
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

        //Criar a bola e dar-lhe força para ir na direção criada anteriormente
        GameObject BallToThrow = Instantiate(Ball, ThrowStartPoint, Quaternion.identity);
        BallToThrow.GetComponent<Rigidbody>().AddForce(lookAtPosition * ThrowForce);

        //Destruir bola
        StartCoroutine(DestroyBall(BallToThrow, 10f));

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
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("Drop Berry")]
    [SerializeField] List<GameObject> BerryVariants;
    [SerializeField] GameObject Berry;
    [SerializeField] Transform DropPoint;
    [Space]
    [SerializeField] float DropForce;
    [SerializeField] float DropCoolDown;
    bool canDropNextBerry = true;

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

        //Valor de quantas bolas a selecionada possui
        NumBallsTxt.text = PlayerPrefs.GetInt("NumberOfBall" + PlayerPrefs.GetInt("CurrentBallLevel")) + "x";
    }

    void ChangeBallSpriteAndValues(Sprite ballSprite)
    {
        //Trocar a sprite da bola e o numero de bolas da selecionada
        BallSprite.sprite = ballSprite;        
    }

    public void PlayDropBerry()
    {
        //Dropar berries para apanhar o Tanuki
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && canDropNextBerry)
        {
            //Atirar bola
            StartCoroutine(DropBerry());
        }
    } 

    IEnumerator DropBerry()
    {
        canDropNextBerry = false;

        //Assinar a berry selecionada
        Berry = BerryVariants[0];

        //Criar variaveis para guardar os dados de onde a berry vai sair e em que direção ela vai
        DropStartPoint = DropPoint.position;
        Vector3 lookAtPosition = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).direction;

        //Criar a berry e dar-lhe força para ir na direção criada anteriormente
        GameObject BerryToDrop = Instantiate(Berry, DropStartPoint, Quaternion.identity);
        BerryToDrop.GetComponent<Rigidbody>().AddForce(lookAtPosition * DropForce);

        //Destruir berry
        StartCoroutine(DestroyBall(BerryToDrop, 3f));

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

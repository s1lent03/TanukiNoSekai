using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHabilities : MonoBehaviour
{
    [Header("Throw Stone")]
    [SerializeField] GameObject Stone;
    [SerializeField] Transform ThrowPoint;
    [Space]
    [SerializeField] float ThrowForce;
    [SerializeField] float ThrowCoolDown;
    bool canThrowNextRock = true;

    [Header("Others")]
    [SerializeField] GameObject Managers;
    [SerializeField] Camera mainCamera;
    PlayerInput playerInput;
    

    Vector3 ThrowStartPoint;

    void Start()
    {
        //Dar um valor default �s variaveis
        playerInput = GetComponent<PlayerInput>();        
    }

    void Update()
    {
        //Lan�ar pedra quando primida a devida tecla e caso o jogo n�o esteja em pausa nem em batalha
        if (!Managers.GetComponent<ControllerManager>().isPlayerInBattle && !Managers.GetComponent<PauseMenuManager>().isPaused && playerInput.actions["ThrowRock"].triggered && canThrowNextRock)
        {
            //Atirar pedra
            StartCoroutine(ThrowRock());
        }
    }

    IEnumerator ThrowRock()
    {
        canThrowNextRock = false;

        //Criar variaveis para guardar os dados de onde a pedra vai sair e em que dire��o ela vai
        ThrowStartPoint = ThrowPoint.position;
        Vector3 lookAtPosition = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).direction;

        //Criar a pedra e dar-lhe for�a para ir na dire��o criada anteriormente
        GameObject RockToThrow = Instantiate(Stone, ThrowStartPoint, Quaternion.identity);
        RockToThrow.GetComponent<Rigidbody>().AddForce(lookAtPosition * ThrowForce);

        //Destruir pedra
        StartCoroutine(DestroyRock(RockToThrow));

        //Esperar algum tempo para mandar outra
        yield return new WaitForSeconds(ThrowCoolDown);
        canThrowNextRock = true;
    }

    //Destruir a pedra ap�s algum tempo
    IEnumerator DestroyRock(GameObject rock)
    {
        yield return new WaitForSeconds(5f);

        Destroy(rock);
    }
}

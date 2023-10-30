using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TanukiDetection : MonoBehaviour
{
    [Header("GameOjects")]
    public GameObject WildTanukiDetected;
    public GameObject Player;
    [Space]
    public CinemachineVirtualCamera thirdPersonCamera;
    private PlayerInput playerInput;
    [Space]
    public GameObject BattleHud;
    public GameObject Managers;

    [Header("Values")]
    public bool isInBattle;
    [Space]
    public float distanceToMoveBack;
    public float playerTravelSpeed;
    public float tanukiRotationSpeed;
    [Space]
    public Transform cameraBattleFollowPoint;
    [Space]
    public float zoomCamera;
    public float unzoomCamera;
    public float minZoomCamera;
    public float maxZoomCamera;

    void Start()
    {
        //Valores default para as variaveis
        isInBattle = false;
        playerInput = GetComponent<PlayerInput>();
    }

    //Quando se aproxima de um Tanuki sem estar agachado, o Tanuki vai detetar e começar a batalha
    private void OnTriggerEnter(Collider other)
    {
        if (!playerInput.actions["Crouch"].IsPressed() && other.tag == "WildTanuki")
        {
            EnteredCollider(other.gameObject);
        }
    }

    private void Update()
    {
        //Se a batalha tiver começado, ambos os personagens se metem em posição para lutar
        if (isInBattle)
        {
            StarBattle();
            Player.GetComponent<PlayerMovement>().isPaused = true;

            //Dar zoom ou unzoom à camera
            float cameraDistance = thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
            if (playerInput.actions["UnzoomCamera"].IsPressed() && cameraDistance < maxZoomCamera)
            {
                thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance += zoomCamera;
            }

            if (playerInput.actions["ZoomCamera"].IsPressed() && cameraDistance > minZoomCamera)
            {
                thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance -= unzoomCamera;
            }
        }
    }

    public void EnteredCollider(GameObject Tanuki)
    {
        WildTanukiDetected = Tanuki;
        isInBattle = true;

        //Ligar o HUD de Batalha
        BattleHud.SetActive(true);

        //Desbloquear o cursor caso esteja a jogar sem comando
        if (Input.GetJoystickNames().Length <= 0 && Input.GetJoystickNames()[0] == "")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //Ativar a navegação do HUD
        Managers.GetComponent<BattleManager>().AtivateNav();
    }

    public void StarBattle()
    {
        //Virar o Tanuki em direção ao jogador
        Vector3 tanukiDir = Player.transform.position - WildTanukiDetected.transform.position;
        Quaternion tanukiTargetRot = Quaternion.LookRotation(tanukiDir);
        WildTanukiDetected.transform.rotation = Quaternion.Slerp(WildTanukiDetected.transform.rotation, tanukiTargetRot, tanukiRotationSpeed * Time.deltaTime);

        //Virar o Player em direção ao Tanuki
        Vector3 playerDir = WildTanukiDetected.transform.position - Player.transform.position;
        Quaternion playerTargetRot = Quaternion.LookRotation(playerDir);
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, playerTargetRot, tanukiRotationSpeed * Time.deltaTime);

        //Mover Player para trás
        Vector3 targetPlayerPos = WildTanukiDetected.transform.position + WildTanukiDetected.transform.forward * distanceToMoveBack;

        //Manter a altura do player
        targetPlayerPos = new Vector3(targetPlayerPos.x, Player.transform.position.y, targetPlayerPos.z);

        Player.transform.position = Vector3.MoveTowards(Player.transform.position, targetPlayerPos, playerTravelSpeed * Time.deltaTime);

        //Colocar a camera a olhar para o centro da batalha
        Vector3 battleMidPoint = (Player.transform.position + WildTanukiDetected.transform.position) * 0.5f;
        cameraBattleFollowPoint.position = battleMidPoint;

        ChangeCameraValues(cameraBattleFollowPoint, new Vector3(0, 0, 0), 0.5f, 0.5f, 7);
    }

    public void ChangeCameraValues(Transform follow, Vector3 offset, float screenPosX, float screenPosY, float distance)
    {
        //Aletarar os valores da camera para ela ficar adaptada à batalha ou ao andar normal
        //Alterar o ponto central da camera
        thirdPersonCamera.LookAt = follow;
        thirdPersonCamera.Follow = follow;

        //Alterar o offset
        thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;

        //Alterar a posição do ecrã para que está a apontar
        thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = screenPosX;
        thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = screenPosY;

        //Alterar a distância
        //thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = distance;
    }

    public void EndBattle()
    {
        //Volta a colocar as variaveis como estavam antes da batalha começar
        WildTanukiDetected = null;
        isInBattle = false;
        Player.GetComponent<PlayerMovement>().isPaused = false;

        //Colocar a camera com os seus valores normais
        ChangeCameraValues(Player.transform, new Vector3(0, 1.5f, 0), 0.35f, 0.45f, 3);

        //Desligar o HUD de batalha
        BattleHud.SetActive(false);

        //Bloquear o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera thirdPersonCamera;
    public CinemachineVirtualCamera BattleCamera1;
    public CinemachineVirtualCamera BattleCamera2;
    public float gamePadSensitivity;
    public float mouseSensitivity;

    [Header("Others")]
    private PlayerInput playerInput;
    public bool isPlayerInBattle;
    [SerializeField] private int currentCameraNumber = 0;

    void Start()
    {
        //Verificar se tem algum comando ligado ou não
        if (Input.GetJoystickNames().Length > 0)
        {
            //Comando encontra-se ligado
            thirdPersonCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = gamePadSensitivity;
            thirdPersonCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = gamePadSensitivity;
        }
        else
        {
            //Comando encontra-se desligado
            thirdPersonCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = mouseSensitivity;
            thirdPersonCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = mouseSensitivity;
        }

        //Dar um valor default às variaveis
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        //Mostrar ou esconder o cursor
        if (playerInput.actions["ShowCursor"].triggered && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (playerInput.actions["ShowCursor"].triggered && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //Se o jogador estiver em batalha trocar para camera de batalha
        if (playerInput.actions["SwitchBattleCamera"].triggered && isPlayerInBattle)
        {
            Debug.Log("triggered");
            if (currentCameraNumber == 0)
            {
                ChangeToBattleCamera();
            }
            else if (currentCameraNumber == 1)
            {
                currentCameraNumber = 2;
                thirdPersonCamera.Priority = 5;
                BattleCamera1.Priority = 5;
                BattleCamera2.Priority = 10;
            }
            else if (currentCameraNumber == 2)
            {
                DefaultCameraValues();
            }
        }
    }

    //Camera principal volta a ser principal
    public void DefaultCameraValues()
    {
        currentCameraNumber = 0;
        thirdPersonCamera.Priority = 10;
        BattleCamera1.Priority = 5;
        BattleCamera2.Priority = 5;
    }

    //Inicializa a camera de combate
    public void ChangeToBattleCamera()
    {
        currentCameraNumber = 1;
        thirdPersonCamera.Priority = 5;
        BattleCamera1.Priority = 10;
        BattleCamera2.Priority = 5;
    }
}

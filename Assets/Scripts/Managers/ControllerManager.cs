using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera thirdPersonCamera;
    public float gamePadSensitivity;
    public float mouseSensitivity;

    [Header("Others")]
    private PlayerInput playerInput;

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
    }
}

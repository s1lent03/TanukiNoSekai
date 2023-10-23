using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera thirdPersonCamera;
    public float gamePadSensitivity;
    public float mouseSensitivity;

    void Start()
    {
        //Verificar se tem algum comando ligado ou não
        if (Input.GetJoystickNames()[0] != "")
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

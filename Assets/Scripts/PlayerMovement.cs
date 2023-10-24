using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float crouchSpeed;
    public float normalSpeed;
    public float sprintSpeed;
    public float rotationSpeed;
    public float gravity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private float speed;
    private float height = 0f;
    private bool isGrounded;

    [Header("Others")]
    //Parar movimento se o jogo estiver em pausa
    private bool isPaused;
    public GameObject pauseMenu;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Não deixar o jogador se mexer caso o jogo esteja em pausa
        isPaused = pauseMenu.GetComponent<PauseMenuManager>().isPaused;

        if (isPaused != true)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();

            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;

            forward = forward.normalized;
            right = right.normalized;

            Vector3 direction = (move.x * right) + (move.y * forward);

            if (!isGrounded)
            {
                height += gravity * Time.deltaTime;
                direction.y = height;
            }

            if (playerInput.actions["Crouch"].IsPressed())
                speed = crouchSpeed;
            else if (playerInput.actions["Sprint"].IsPressed() && move.y > 0)
                speed = sprintSpeed;
            else
                speed = normalSpeed;

            characterController.Move(direction * speed * Time.deltaTime);

            Quaternion toRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }       
    }
}

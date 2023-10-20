using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float rotationSpeed;
    public float gravity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private PlayerInput playerInput;
    private float height;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 direction = new Vector3(move.x, 0, move.y);
        direction.Normalize();

        if (!isGrounded)
        {
            height += gravity * Time.deltaTime;
            direction.y = height;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

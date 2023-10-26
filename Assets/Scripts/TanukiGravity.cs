using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanukiGravity : MonoBehaviour
{
    [Header("Movement")]
    public float gravity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController characterController;
    private float height = 0f;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 direction = Vector3.zero;

        if (!isGrounded)
        {
            height -= gravity * Time.deltaTime;
            direction.y = height;
        }

        characterController.Move(direction * Time.deltaTime);
    }
}
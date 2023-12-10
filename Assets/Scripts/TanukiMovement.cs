using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanukiMovement : MonoBehaviour
{
    [Header("Movement")]
    public float gravity;
    public float speed = 3f;
    public float chaseSpeed = 6f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Booleans")]
    public bool stunned;
    public bool angry;
    public bool isBeingCaught;

    private bool isGrounded;
    private CharacterController characterController;
    private Collider zone;
    private float height = 0f;
    private Vector3 startPosition;
    private Vector3 nextPosition;

    private void Awake()
    {
        startPosition = transform.position;
        nextPosition = startPosition;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // movement
        if (!stunned && !isBeingCaught)
        {
            if (angry)
            {
                Vector3 playerPosition = new Vector3(GameObject.Find("Character").transform.position.x, transform.position.y, GameObject.Find("Character").transform.position.z);
                Vector3 playerDirection = (playerPosition - transform.position).normalized;

                characterController.Move(playerDirection * chaseSpeed * Time.deltaTime);
                transform.LookAt(playerPosition);
            } else
            {
                if (Mathf.Round(transform.position.x) == Mathf.Round(nextPosition.x) && Mathf.Round(transform.position.z) == Mathf.Round(nextPosition.z))
                {
                    NewPosition();
                }

                Vector3 lookPosition = nextPosition;
                lookPosition.y = transform.position.y;

                characterController.Move((nextPosition - transform.position).normalized * speed * Time.deltaTime);
                transform.LookAt(lookPosition);
            }
        }

        // graivty
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 direction = Vector3.zero;

        if (!isGrounded)
        {
            height -= gravity * Time.deltaTime;
            direction.y = height;
        }

        characterController.Move(direction * Time.deltaTime);
    }

    public void SendCollider(Collider collider)
    {
        zone = collider;
    }

    private void NewPosition()
    {
        if (zone == null)
            return;

        Vector3 areaCenter = zone.bounds.center;
        Vector3 areaExtents = zone.bounds.extents;

        float randomX = Random.Range(areaCenter.x - areaExtents.x, areaCenter.x + areaExtents.x);
        float randomZ = Random.Range(areaCenter.z - areaExtents.z, areaCenter.z + areaExtents.z);

        nextPosition = new Vector3(randomX, startPosition.y, randomZ);
    }
}
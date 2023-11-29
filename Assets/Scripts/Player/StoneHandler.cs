using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneHandler : MonoBehaviour
{
    [Header("Main")]
    bool hasHitGround = false;
    bool hasHitTanuki = false;

    [SerializeField] Transform raycastOrigin;
    [SerializeField] float raycastDistance;

    void Update()
    {
        //Cria um raycast do objeto em direção a baixo
        Ray ray = new Ray(transform.position, Vector3.down);

        //Verifica se esse ray acertou em algo
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            //Verifica se esse algo é o chão
            if (hit.collider.gameObject.tag == "Ground" && !hasHitGround)
            {
                hasHitGround = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Verifica se acertou num Tanuki
        if (collision.collider.tag == "WildTanuki" && !hasHitGround && !hasHitTanuki)
        {
            hasHitTanuki = true;

            collision.collider.gameObject.GetComponent<TanukiMovement>().stunned = true;
        }            
    }
}

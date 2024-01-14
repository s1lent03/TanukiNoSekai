using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrowHandler : MonoBehaviour
{
    [Header("Main")]
    bool hasHitGround = false;
    bool hasHitTanuki = false;

    [SerializeField] Transform raycastOrigin;
    [SerializeField] float raycastDistance;
    [SerializeField] GameObject particleEffect;

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
        GameObject enemyTanuki = collision.collider.gameObject;
        //Verifica se acertou num Tanuki
        if (enemyTanuki.tag == "WildTanuki" && !hasHitGround && !hasHitTanuki)
        {
            hasHitTanuki = true;

            Instantiate(particleEffect, collision.transform.position, Quaternion.identity, collision.transform);

            enemyTanuki.GetComponent<TanukiMovement>().angry = true;

            int currentBallNum = PlayerPrefs.GetInt("CurrentBallLevel");

            int rand = Random.Range(1, enemyTanuki.GetComponent<BattleUnit>().tanukiUnitData.Level / (currentBallNum * 2));

            if (rand <= currentBallNum * 2)
            {
                enemyTanuki.GetComponent<TanukiMovement>().stunned = true;
                enemyTanuki.GetComponent<TanukiMovement>().tanukiAnimator.SetTrigger(Animator.StringToHash("Hurt"));
                enemyTanuki.GetComponent<TanukiMovement>().tanukiAnimator.SetBool(Animator.StringToHash("Stunned"), true);
                StartCoroutine(StunnedTime(enemyTanuki));
            }                
        }            
    }
    
    //Tempo total que o Tanuki fica stunned
    IEnumerator StunnedTime(GameObject enemy)
    {
        float randTime = Random.Range(4, 7);
        yield return new WaitForSeconds(randTime);
       
        enemy.GetComponent<TanukiMovement>().tanukiAnimator.SetBool(Animator.StringToHash("Stunned"), false);
        yield return new WaitForSeconds(1f);
        enemy.GetComponent<TanukiMovement>().stunned = false;
    }
}

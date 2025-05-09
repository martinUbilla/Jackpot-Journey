using System;
using UnityEngine;


public class punoLimpio : MonoBehaviour
{
    [SerializeField] float timeToAttack = 4f;
    float timer;

    [SerializeField] GameObject leftAttackObject;
    [SerializeField] GameObject rightAttackObject;
    [SerializeField] GameObject upAttackObject;
    [SerializeField] GameObject downAttackObject;
    [SerializeField] GameObject diagonalupleftAttackObject;
    [SerializeField] GameObject diagonaluprightAttackObject;
    [SerializeField] GameObject diagonaldownleftAttackObject;
    [SerializeField] GameObject diagonaldownrightAttackObject;


    PlayerMove playerMove;
    [SerializeField] Vector2 AttackSize = new Vector2(4f, 2f);
    [SerializeField] int attackDamage = 200;
    private void Awake()
    {
        leftAttackObject = Instantiate(leftAttackObject);
        rightAttackObject = Instantiate(rightAttackObject);
        upAttackObject = Instantiate(upAttackObject);
        downAttackObject = Instantiate(downAttackObject);
        diagonaldownleftAttackObject = Instantiate(diagonaldownleftAttackObject);
        diagonaldownrightAttackObject = Instantiate(diagonaldownrightAttackObject);
        diagonalupleftAttackObject = Instantiate(diagonalupleftAttackObject);
        diagonaluprightAttackObject = Instantiate(diagonaluprightAttackObject);
        playerMove = GetComponentInParent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f){
            
            Attack();
        }
    }
    private void Attack()
    {
        Debug.Log(timer);
        Debug.Log("Attack");
        timer = timeToAttack;

        if (playerMove.lastHorizontalVector > 0 && playerMove.lastVerticalVector == 0)
        {
            rightAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(rightAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastHorizontalVector < 0 && playerMove.lastVerticalVector == 0)
        {
            leftAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(leftAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastVerticalVector > 0 && playerMove.lastHorizontalVector == 0)
        {
            upAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(upAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastVerticalVector < 0 && playerMove.lastHorizontalVector == 0)
        {
            downAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(downAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastVerticalVector > 0 && playerMove.lastHorizontalVector < 0)
        {
            diagonalupleftAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(diagonalupleftAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastVerticalVector > 0 && playerMove.lastHorizontalVector > 0)
        {
            diagonaluprightAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(diagonaluprightAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastVerticalVector < 0 && playerMove.lastHorizontalVector < 0)
        {
            diagonaldownleftAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(diagonaldownleftAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        if (playerMove.lastVerticalVector < 0 && playerMove.lastHorizontalVector > 0)
        {
            diagonaldownrightAttackObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(diagonaldownrightAttackObject.transform.position, AttackSize, 0f);
            ApplyDamage(colliders);
        }
        Debug.Log(timer);

    }

    private void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 1; i < colliders.Length; i++)
        {
            EnemyCard0 e = colliders[i].GetComponent<EnemyCard0>();
            if (e != null)
            {
                colliders[i].GetComponent<EnemyCard0>().TakeDamage(attackDamage);
            }
        }
    }
}

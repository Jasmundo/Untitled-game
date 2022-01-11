using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAi : EnemyAi
{
    public float punchForce;
    public override void Idling()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        base.Idling();
    }

    public override void ChasePlayer()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

        base.ChasePlayer();
    }
    public override void AttackPlayer()
    {
        base.AttackPlayer();

        if (!alreadyAttacked)
        {
            animator.SetBool("isAttacking", true);

            Invoke(nameof(DelayedAttack), 0.9f);
            /*
            RaycastHit hit;
            if (Physics.Raycast(Rigidbody.position, transform.forward, out hit, 1.5f, LayerMask.GetMask("Player")))
            {
                Player player = hit.collider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(attackDamage);
                    Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
                    playerRigidbody.AddForce(lookVector * punchForce);
                }
            }
            */
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void DelayedAttack()
    {
        Vector3 lookVector = (player.position - transform.position).normalized;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

        Vector3 attackCenter = transform.position + new Vector3(0f, 0f, 2f);
        Collider[] playerCollider = Physics.OverlapSphere(attackCenter, 4f, whatIsPlayer);

        if (playerCollider.Length > 0)
        {
            Player playerHit = playerCollider[0].GetComponent<Player>();
            playerHit.TakeDamage(attackDamage);
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.AddForce(lookVector * punchForce, ForceMode.Impulse);
            
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls animations and what happens when Ghoul attacks the player.
/// </summary>
public class GhoulAi : EnemyAi
{
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

            Vector3 lookVector = player.position - transform.position;
            lookVector.y = transform.position.y;
            Quaternion rot = Quaternion.LookRotation(lookVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

            //Attack code(does twice the damage somehow)
            RaycastHit hit;
            if (Physics.Raycast(Rigidbody.position, transform.forward, out hit, 1.5f, LayerMask.GetMask("Player")))
            {
                Player player = hit.collider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(attackDamage);
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

}

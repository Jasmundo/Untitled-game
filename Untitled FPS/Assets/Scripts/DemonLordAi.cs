using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonLordAi : EnemyAi
{
    //public GameObject projectile;
    //public float shotforce;

    public GameObject projectile;
    public float shotforce;
    public override void Idling()
    {
        animator.SetBool("isWalking", false);

        base.Idling();
    }

    public override void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        base.ChasePlayer();
    }
    public override void AttackPlayer()
    {
        base.AttackPlayer();

        Vector3 lookVector = player.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

        if (!alreadyAttacked)
        {
            

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void DelayedAttack()
    {
        
    }
}

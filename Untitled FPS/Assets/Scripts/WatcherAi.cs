using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls animations and what happens when Watcher attacks the player.
/// </summary>
public class WatcherAi : EnemyAi
{
    public GameObject projectile;
    public float shotforce;
    public override void Idling()
    {
        animator.SetBool("isFlying", false);
        animator.SetBool("isAttacking", false);

        base.Idling();
    }

    public override void ChasePlayer()
    {
        animator.SetBool("isFlying", true);
        animator.SetBool("isAttacking", false);

        base.ChasePlayer();
    }
    public override void AttackPlayer()
    {
        base.AttackPlayer();

        animator.SetBool("isAttacking", true);
        if (!alreadyAttacked)
        {
            Vector3 lookVector = player.position - transform.position;
            lookVector.y = transform.position.y;
            Quaternion rot = Quaternion.LookRotation(lookVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

            Vector3 direction = player.transform.position - transform.position;
            GameObject currentProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            currentProjectile.transform.forward = direction.normalized;

            currentProjectile.GetComponent<Rigidbody>().AddForce(direction.normalized * shotforce, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
}

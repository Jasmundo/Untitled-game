using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls animations and what happens when Wizard attacks the player.
/// </summary>
public class WizardAi : EnemyAi
{
    //public GameObject projectile;
    //public float shotforce;
    public LayerMask whatIsEnemy;
    public int healValue;
    public GameObject healEffect;

    public GameObject projectile;
    public float shotforce;
    public override void Idling()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isInRange", false);
        animator.SetBool("isAttacking", false);

        base.Idling();
    }

    public override void ChasePlayer()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isInRange", false);
        animator.SetBool("isAttacking", false);

        base.ChasePlayer();
    }
    public override void AttackPlayer()
    {
        base.AttackPlayer();

        Vector3 lookVector = player.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

        animator.SetBool("isInRange", true);
        if (!alreadyAttacked)
        {   
            Collider[] mobs = Physics.OverlapSphere(transform.position, 30f, whatIsEnemy);
            int healedMob = Random.Range(0, mobs.Length);
            EnemyAi enemy = mobs[healedMob].GetComponent<EnemyAi>();

            if (enemy != null && (enemy.health < enemy.maxHealth))
            {
                Debug.Log("Enemy healed");
                enemy.GetHealed(healValue);
                Instantiate(healEffect, enemy.transform.position, Quaternion.identity);
            }
            else
            {
                animator.Play("attack_short_001", -1, 0f);
                Invoke(nameof(DelayedAttack), 0.6f);
            }

            /*
            Vector3 direction = player.transform.position - transform.position;
            GameObject currentProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            currentProjectile.transform.forward = direction.normalized;

            currentProjectile.GetComponent<Rigidbody>().AddForce(direction.normalized * shotforce, ForceMode.Impulse);
            */
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void DelayedAttack()
    {
        Vector3 direction = player.transform.position - transform.position;
        Vector3 instantiantePosition = transform.position + new Vector3(0, 1, 0);
        GameObject currentProjectile = Instantiate(projectile, instantiantePosition, Quaternion.identity);
        currentProjectile.transform.forward = direction.normalized;

        currentProjectile.GetComponent<Rigidbody>().AddForce(direction.normalized * shotforce, ForceMode.Impulse);
    }
}

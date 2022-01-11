using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, ignoreRaycast;
    public float maxHealth;
    public float health;
    public int attackDamage;
    public bool boss;

    protected Rigidbody Rigidbody;
    protected Animator animator;

    //Attacking
    public float timeBetweenAttacks;
    protected bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    protected bool playerInSightRange, playerInAttackRange, isDead;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        Rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        alreadyAttacked = false;
        isDead = false;
        health = maxHealth;

        SetRigidbodyState(true);
        SetColliderState(false);
    }

    private void FixedUpdate()
    {
        //checking for sight and attack range
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInSightRange = PlayerInSightRange(sightRange);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if ((!playerInSightRange && !playerInAttackRange) || isDead) Idling();
        if (playerInSightRange && !playerInAttackRange && !isDead) ChasePlayer();
        if (playerInSightRange && playerInAttackRange && !isDead) AttackPlayer();
    }

    public virtual void Idling()
    {
        //agent.SetDestination(transform.position);
        agent.isStopped = true;
    }

    public virtual void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        //looking at player without using LookAt;
        Vector3 lookVector = player.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
    }
    public virtual void AttackPlayer()
    {
        //agent.SetDestination(transform.position);
        agent.isStopped = true;
        //doesnt work because player transform is above ground and ghoul transform is at ground level
        //transform.LookAt(player);
    }
    private bool PlayerInSightRange(float sightRange)
    {
        Vector3 lookVector = player.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(Rigidbody.position, lookVector, out hit, sightRange, ignoreRaycast))
        {
            Player player = hit.collider.GetComponent<Player>();
            if (player != null)
            {
                return true;
            }
            else return false;
        }
        else return false;
    }
    void SetRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
        Rigidbody.isKinematic = !state;
    }
    void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        GetComponent<Collider>().enabled = !state;
    }
    public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
    public void GetHealed(int heal)
    {
        if (health + heal <= maxHealth)
        {
            health += heal;
        }
        else
        {
            health = maxHealth;
        }
    }
    private void Die()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Vector3 direction = transform.position - player.transform.position;

        //Rigidbody.AddExplosionForce(500f, transform.position, 50f);

        isDead = true;
        Destroy(gameObject, 5f);
        animator.enabled = false;
        SetColliderState(true);
        SetRigidbodyState(false);

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.AddForce(direction * 100f);
        }
        if (boss)
        {
            Invoke(nameof(GameWon), 2f);
        }
    }
    private void GameWon()
    {
        FindObjectOfType<GameManager>().GameWon();
    }
}

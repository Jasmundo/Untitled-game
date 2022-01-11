using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // --- Config ---
    public float speed = 100;
    public int damage;
    public LayerMask collisionLayerMask;

    // --- Explosion VFX ---
    public GameObject rocketExplosion;

    // --- Projectile Mesh ---
    public MeshRenderer projectileMesh;

    // --- Script Variables ---
    private bool targetHit;

    // --- Audio ---
    public AudioSource inFlightAudioSource;

    // --- VFX ---
    public ParticleSystem disableOnHit;


    private void Update()
    {
        // --- Check to see if the target has been hit. We don't want to update the position if the target was hit ---
        if (targetHit) return;

        // --- moves the game object in the forward direction at the defined speed ---
        transform.position += transform.forward * (speed * Time.deltaTime);
    }


    /// <summary>
    /// Explodes on contact.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
        if (!enabled) return;

        // --- Explode when hitting an object and disable the projectile mesh ---
        Explode();
        projectileMesh.enabled = false;
        targetHit = true;
        inFlightAudioSource.Stop();
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }
        disableOnHit.Stop();


        // --- Destroy this object after 2 seconds. Using a delay because the particle system needs to finish ---
        Destroy(gameObject, 5f);
    }


    /// <summary>
    /// Instantiates an explode object.
    /// </summary>
    private void Explode()
    {
        // --- Instantiate new explosion option. I would recommend using an object pool ---
        GameObject newExplosion = Instantiate(rocketExplosion, transform.position, rocketExplosion.transform.rotation, null);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            EnemyAi enemy = hitCollider.GetComponent<EnemyAi>();
            Player player = hitCollider.GetComponent<Player>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (player != null)
            {
                player.TakeDamage(10);
            }
        }
        /*
        if (Physics.SphereCastAll(transform.position, 0f,transform.forward, out hit, 4f, collisionLayerMask))
        {
            Debug.Log("hit");
            EnemyAi enemy = hit.collider.GetComponent<EnemyAi>();
            if (enemy != null)
            {
                Debug.Log("enemy");
                enemy.TakeDamage(50);
            }
        }*/
    }
}

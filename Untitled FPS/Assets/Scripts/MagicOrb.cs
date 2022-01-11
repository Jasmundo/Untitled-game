using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicOrb : MonoBehaviour
{
    public int damage;
	public GameObject hitEffect;
	private bool alreadyHit = false;

	private void OnCollisionEnter(Collision collision)
	{
		Vector3 finalScale = new Vector3(0.1f, 0.1f, 0.1f);
		Player player = collision.collider.GetComponent<Player>();
		if (player != null && !alreadyHit)
		{
			player.TakeDamage(damage);
		}
		if (!alreadyHit) 
		{
			Instantiate(hitEffect, transform.position, Quaternion.identity);
		}
		alreadyHit = true;
		//Destroy(gameObject);
		Invoke(nameof(DestroyOrb), 0.05f);
	}

	private void DestroyOrb()
	{
		Destroy(gameObject);
	}

}

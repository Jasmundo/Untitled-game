using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

	private void OnTriggerEnter(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if (player != null && player.healthPackNo < 5)
		{
			player.healthPackNo += 1;
			player.RefreshHealthPack();
			Destroy(gameObject);
		}
	}
}

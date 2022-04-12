using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to detect collisions on newly spawned objects. Every dungeon part has this script as a component.
/// </summary>
public class CollisionDetect : MonoBehaviour
{
    public bool isColliding = false;
	List<GameObject> collisions = new List<GameObject>();

	private void OnTriggerEnter(Collider other)
	{
		isColliding = true;
		collisions.Add(other.gameObject);
	}
	void Update()
	{
		if (collisions == null)
		{
			isColliding = false;
		}
	}
}

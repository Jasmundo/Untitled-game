using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

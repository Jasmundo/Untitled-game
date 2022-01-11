using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	private Dungeon dungeon;
	Transform location;
	public GameObject mob;

	public SpawnPoint(Dungeon dungeon, Transform location)
	{
		this.dungeon = dungeon;
		this.location = location;
	}
	public void SpawnMob()
	{
		Instantiate(mob, location.position, location.rotation, dungeon.transform);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component of transforms existing in rooms that designate a location of a health pack.
/// </summary>
public class HealthSpawnPoint : MonoBehaviour
{
	private Dungeon dungeon;
	Transform location;
	public GameObject healthpack;

	public HealthSpawnPoint(Dungeon dungeon, Transform location, GameObject healthpack)
	{
		this.dungeon = dungeon;
		this.location = location;
		this.healthpack = healthpack;
	}
	public void SpawnHealth()
	{
		Instantiate(healthpack, location.position, location.rotation, dungeon.transform);
	}
}

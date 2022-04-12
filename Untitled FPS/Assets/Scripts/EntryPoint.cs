using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;

/// <summary>
/// Responsible for spawning elements at different transforms existing in dungeon parts.
/// Those elements can be: adjacent rooms/corridors, doors, walls.
/// </summary>
public class EntryPoint : MonoBehaviour
{
    private Dungeon dungeon;
    Transform entryPoint;
	bool isExit = false;
	public bool openingActive = false;
	public bool openingValidated = false;
	public GameObject spawnedPart;

	public EntryPoint(Dungeon dungeon, Transform entryPoint)
	{
		this.dungeon = dungeon;
		this.entryPoint = entryPoint;
	}
	/// <summary>
	/// Used to create new parts of the dungeon. Spawnes a prefab of a new room/corridor at the location stored in the entryPoint variable. Then uses it to create an EntryPoint type obejct.
	/// </summary>
	/// <param name="parent"> - the DungeonPart object that the newly spawned part is connected to</param>
	/// <param name="parentEntryPoint"> - which of the parent's EntryPoint variables is the newly spawned part connected to</param>
	/// <param name="partType"> - what type of DungeonPart is the parent object- room or corridor. If the parent part is a room the spawned part is going to be a corridor and vice versa</param>
	/// <returns>The newly spawned DungeonPart object</returns>
	public DungeonPart SpawnPart(DungeonPart parent, int parentEntryPoint, int partType)
	{
		GameObject spawnedPartObject = GetRandomPart(partType);
		Quaternion objectRotation = entryPoint.rotation;
		spawnedPartObject.transform.rotation = objectRotation;
		GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation, dungeon.transform);
		spawnedPart = inGameSpawnedPartObject;

		//CollisionDetect collisionDetect = inGameSpawnedPartObject.GetComponent<CollisionDetect>();
		//Debug.Log(collisionDetect.isColliding);

		EntryPoint[] inGameSpawnedObjectEntryPoints = dungeon.GetEntryPoints(inGameSpawnedPartObject);
		if (partType == 1)
		{
			SpawnPoint[] inGameSpawnedObjectSpawnPoints = dungeon.GetSpawnPoints(inGameSpawnedPartObject);
			DungeonPart spawnedPart = new DungeonPart(inGameSpawnedPartObject, dungeon, inGameSpawnedObjectEntryPoints, inGameSpawnedObjectSpawnPoints, GetNewPartType(partType), parent, parentEntryPoint);
			openingActive = true;
			return spawnedPart;
		}
		else
		{
			HealthSpawnPoint inGameSpawnedObjectHealthSpawnPoints = dungeon.GetHealthSpawnPoint(inGameSpawnedPartObject);
			DungeonPart spawnedPart = new DungeonPart(inGameSpawnedPartObject, dungeon, inGameSpawnedObjectEntryPoints, inGameSpawnedObjectHealthSpawnPoints, GetNewPartType(partType), parent, parentEntryPoint);
			openingActive = true;
			return spawnedPart;
		}
	}

	public DungeonPart SpawnArmory(DungeonPart parent, int parentEntryPoint)
	{
		GameObject spawnedPartObject = dungeon.armory;
		Quaternion objectRotation = entryPoint.rotation;
		spawnedPartObject.transform.rotation = objectRotation;
		GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation, dungeon.transform);
		spawnedPart = inGameSpawnedPartObject;

		DungeonPart spawnedPartObejct = new DungeonPart(inGameSpawnedPartObject, dungeon, 0, parent, parentEntryPoint);
		openingActive = true;
		return spawnedPartObejct;
	}

	public DungeonPart SpawnBossRoom(DungeonPart parent, int parentEntryPoint)
	{
		GameObject spawnedPartObject = dungeon.bossRoom;
		Quaternion objectRotation = entryPoint.rotation;
		spawnedPartObject.transform.rotation = objectRotation;
		GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation, dungeon.transform);
		spawnedPart = inGameSpawnedPartObject;

		DungeonPart spawnedPartObejct = new DungeonPart(inGameSpawnedPartObject, dungeon, 0, parent, parentEntryPoint);
		openingActive = true;
		return spawnedPartObejct;
	}

	public void SpawnDoor()
	{
		Quaternion objectRotation = entryPoint.rotation;
		float rotationDegrees = 90;
		objectRotation = objectRotation * Quaternion.Euler(0, rotationDegrees, 0);

		if (openingActive) Instantiate(dungeon.door, entryPoint.position, objectRotation, dungeon.transform);
		else if(!isExit) Instantiate(dungeon.wall, entryPoint.position, objectRotation, dungeon.transform);

	}
	public bool SpawnExitDoor()
	{
		Quaternion objectRotation = entryPoint.rotation;
		float rotationDegrees = 90;
		objectRotation = objectRotation * Quaternion.Euler(0, rotationDegrees, 0);

		if (!openingActive)
		{
			Instantiate(dungeon.exitDoor, entryPoint.position, objectRotation, dungeon.transform);
			isExit = true;
			return true;
		}
		else return false;

	}
	public void SpawnWall()
	{
		Quaternion objectRotation = entryPoint.rotation;
		float rotationDegrees = 90;
		objectRotation = objectRotation * Quaternion.Euler(0, rotationDegrees, 0);

		if (openingActive) Instantiate(dungeon.door, entryPoint.position, objectRotation, dungeon.transform);
		else Instantiate(dungeon.wall, entryPoint.position, objectRotation, dungeon.transform);
	}
	private GameObject GetRandomPart(int partType)
	{
		GameObject newPart;
		if (partType == 0)
		{
			int partNo = Random.Range(0, dungeon.corridors.Count);
			newPart = dungeon.corridors[partNo];
		}
		else
		{
			int partNo = Random.Range(0, dungeon.rooms.Count);
			newPart = dungeon.rooms[partNo];
		}
		return newPart;
	}
	private int GetNewPartType(int partType)
	{
		if (partType == 0) return 1;
		else return 0;
	}
		/*
		GameObject spawnedPartObject = dungeon.corridors[0];
		BoxCollider boxCollider = spawnedPartObject.GetComponent<BoxCollider>();
		Quaternion objectRotation = new Quaternion();
		float rotationDegrees = -90;
		objectRotation = Quaternion.Euler(0, rotationDegrees, 0);
		Debug.Log(Physics.CheckBox(entryPoint.position + boxCollider.center, boxCollider.size / 2.0F, objectRotation, dungeon.layerMask));
		Debug.Log(entryPoint.position + boxCollider.center);
		Debug.Log(Physics.CheckBox(new Vector3( 0, 2, 12), boxCollider.size / 2.0F, objectRotation, dungeon.layerMask));
		GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation);

		BoxCollider inGameBoxCollider = inGameSpawnedPartObject.GetComponent<BoxCollider>();
		Debug.Log(entryPoint.position + inGameBoxCollider.center);
		*/
		/*
		BoxCollider inGameBoxCollider = inGameSpawnedPartObject.GetComponent<BoxCollider>();
		Debug.Log(inGameSpawnedPartObject.transform.position + inGameBoxCollider.center);

		Debug.Log(Physics.CheckBox(new Vector3(0, 12, 12), inGameBoxCollider.size / 2.0F, objectRotation, dungeon.layerMask));
		Destroy(inGameSpawnedPartObject);
		Debug.Log(Physics.CheckBox(new Vector3(0, 12, 12), inGameBoxCollider.size / 2.0F, objectRotation, dungeon.layerMask));
		*/
		/*
		for (int i = 0; i < 4; ++i)
		{
			GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation);
			BoxCollider inGameBoxCollider = inGameSpawnedPartObject.GetComponent<BoxCollider>();
			Debug.Log(Physics.CheckBox(boxCollider.center, boxCollider.size / 2.0F, spawnedPartObject.transform.rotation, dungeon.layerMask));
			Debug.Log(Physics.CheckBox(inGameBoxCollider.center, inGameBoxCollider.size / 2.0F, inGameSpawnedPartObject.transform.rotation, dungeon.layerMask));
			objectRotation = Quaternion.Euler(0, rotationDegrees, 0);
			rotationDegrees -= 90.0F;
			Destroy(inGameSpawnedPartObject);
		}
		/////////////////////////////////////////////////////
		for (int i = 0; i < 4; ++i)
		{
			if (Physics.CheckBox(location, boxCollider.size / 2.0F, spawnedPartObject.transform.rotation, dungeon.layerMask))
			{
				Debug.Log(i + " " + location + " " + boxCollider.size / 2.0F + " " + spawnedPartObject.transform.rotation);
				objectRotation = Quaternion.Euler(0, rotationDegrees, 0);
				spawnedPartObject.transform.rotation = objectRotation;
				rotationDegrees -= 90.0F;
			}
			else
			{
				GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation);
				EntryPoint[] inGameSpawnedObjectEntryPoints = dungeon.GetEntryPoints(inGameSpawnedPartObject);
				DungeonPart spawnedPart = new DungeonPart(inGameSpawnedPartObject, dungeon, inGameSpawnedObjectEntryPoints);
				Debug.Log(inGameSpawnedObjectEntryPoints[0].entryPoint.position);
				openingActive = true;
				return spawnedPart;
			}
		}*/
		//spawn a wall

		/************************************************************/
		//Quaternion objectRotation = Quaternion.Euler(0, -90, 0);
		//spawnedPartObject.transform.rotation = objectRotation;

		//EntryPoint[] spawnedObjectEntryPoints = dungeon.GetEntryPoints(spawnedPartObject);
		//Debug.Log(spawnedObjectEntryPoints[1].entryPoint.position);

		//Vector3 adjustedLocation = spawnedPartObject.transform.position - spawnedObjectEntryPoints[0].entryPoint.position;

		//GameObject inGameSpawnedPartObject = Instantiate(spawnedPartObject, entryPoint.position, objectRotation);
		//EntryPoint[] inGameSpawnedObjectEntryPoints = dungeon.GetEntryPoints(inGameSpawnedPartObject);

		//DungeonPart spawnedPart = new DungeonPart(inGameSpawnedPartObject, dungeon, inGameSpawnedObjectEntryPoints);

		/*
		Debug.Log(inGameSpawnedObjectEntryPoints[0].entryPoint.position);
		Debug.Log(inGameSpawnedObjectEntryPoints[0].openingActive);
		Debug.Log(inGameSpawnedObjectEntryPoints[1].entryPoint.position);
		Debug.Log(inGameSpawnedObjectEntryPoints[1].openingActive);*/

		/*
		if (openingActive == false)
		{
			spawnedPartObject = Instantiate(dungeon.corridors[0], entryPoint.position + spawnedObjectEntryPoints[0].entryPoint.position, objectRotation);
			DungeonPart spawnedPart = new DungeonPart(spawnedPartObject, dungeon, spawnedObjectEntryPoints);
			openingActive = true;
			spawnedPart.entryPoints[0].openingActive = true;
			return spawnedPart;
		}
		else
		{
			return null;
		}*/
	
}

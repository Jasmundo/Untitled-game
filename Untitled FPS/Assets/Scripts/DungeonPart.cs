using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonPart : MonoBehaviour
{
    public GameObject thisPart;
    private Dungeon dungeon;
    public DungeonPart parent;
    public int parentEntryPoint;
    public EntryPoint[] entryPoints;
    public SpawnPoint[] spawnPoints;
    public HealthSpawnPoint healthSpawnPoint;
    public int partType; // 0 means room, 1 means corridor
    public NavMeshSurface NavMeshSurface;
    public CollisionDetect CollisionDetect;
    public bool isColliding = false;
    public bool validated = false;
    public DungeonPart(GameObject thisPart, Dungeon dungeon, int partType, DungeonPart parent, int parentEntryPoint)
    {
        this.thisPart = thisPart;
        this.dungeon = dungeon;
        this.partType = partType;
        this.parent = parent;
        this.parentEntryPoint = parentEntryPoint;
        NavMeshSurface = thisPart.GetComponent<NavMeshSurface>();
        CollisionDetect = thisPart.GetComponent<CollisionDetect>();
    }
    public DungeonPart(GameObject thisPart, Dungeon dungeon, EntryPoint[] entryPoints, int partType)
    {
        this.thisPart = thisPart;
        this.dungeon = dungeon;
        this.entryPoints = entryPoints;
        this.partType = partType;
        NavMeshSurface = thisPart.GetComponent<NavMeshSurface>();
        CollisionDetect = thisPart.GetComponent<CollisionDetect>();
    }
    public DungeonPart(GameObject thisPart, Dungeon dungeon, EntryPoint[] entryPoints, HealthSpawnPoint healthSpawnPoint, int partType, DungeonPart parent, int parentEntryPoint)
    {
        this.thisPart = thisPart;
        this.dungeon = dungeon;
        this.entryPoints = entryPoints;
        this.healthSpawnPoint = healthSpawnPoint;
        this.partType = partType;
        this.parent = parent;
        this.parentEntryPoint = parentEntryPoint;
        NavMeshSurface = thisPart.GetComponent<NavMeshSurface>();
        CollisionDetect = thisPart.GetComponent<CollisionDetect>();
    }
    public DungeonPart(GameObject thisPart, Dungeon dungeon, EntryPoint[] entryPoints, SpawnPoint[] spawnPoints, int partType, DungeonPart parent, int parentEntryPoint)
	{
		this.thisPart = thisPart;
		this.dungeon = dungeon;
		this.entryPoints = entryPoints;
        this.spawnPoints = spawnPoints;
        this.partType = partType;
        this.parent = parent;
        this.parentEntryPoint = parentEntryPoint;
        NavMeshSurface = thisPart.GetComponent<NavMeshSurface>();
        CollisionDetect = thisPart.GetComponent<CollisionDetect>();
    }

	public DungeonPart[] Generate()
    {
        /*
        GameObject entryPointsObject = thisPart.transform.Find("EntryPoints").gameObject;
        Transform[] entryPointsPositions = entryPointsObject.GetComponentsInChildren<Transform>();
        entryPoints = new EntryPoint[entryPointsPositions.Length - 1];
        for (int i = 1; i < entryPointsPositions.Length; ++i)
        {
            entryPoints[i - 1] = new EntryPoint(dungeon, entryPointsPositions[i]);
        }
        */
        int noOfUnusedOpenings = NoOfUnusedOpenings();
        DungeonPart[] generatedParts = new DungeonPart[noOfUnusedOpenings];
        int n = 0;
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            if (!entryPoints[i].openingActive)
            {
                DungeonPart generatedPart = entryPoints[i].SpawnPart(this,i, partType);
                generatedParts[n] = generatedPart;
                ++n;
            }
            /*if(generatedPart != null)
            {
                generatedParts[i] = generatedPart;
            }*/
        }
        return generatedParts;
    }

    public DungeonPart SpawnArmory() 
    {
        DungeonPart[] spawnedArmory = new DungeonPart[1];
        bool isSpawned = false;
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            if (!isSpawned && !entryPoints[i].openingActive)
            {
                spawnedArmory[0] = entryPoints[i].SpawnArmory(this, i);
                if (spawnedArmory != null) isSpawned = true; ;
            }
        }
        return spawnedArmory[0];
    }

    public DungeonPart SpawnBossRoom()
    {
        DungeonPart[] spawnedBossRoom = new DungeonPart[1];
        bool isSpawned = false;
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            if (!isSpawned && !entryPoints[i].openingActive)
            {
                spawnedBossRoom[0] = entryPoints[i].SpawnBossRoom(this, i);
                if (spawnedBossRoom != null) isSpawned = true; ;
            }
        }
        return spawnedBossRoom[0];
    }

    public void SpawnDoors()
    {
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            entryPoints[i].SpawnDoor();
        }
    }
    public void SpawnExitDoor()
    {
        bool isSpawned;
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            isSpawned = entryPoints[i].SpawnExitDoor();
            if (isSpawned) return;
        }
    }
    public void SpawnWalls()
    {
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            entryPoints[i].SpawnWall();
        }
    }
    public void SpawnMobs()
    {
        if (partType == 0)
        {
            for (int i = 0; i < spawnPoints.Length; ++i)
            {
                int rand = Random.Range(0, 99);
                for (int j = 0; j < dungeon.mobs.Count; ++j)
                {
                    if (rand > dungeon.mobBias[j] && rand < dungeon.mobBias[j + 1])
                    {
                        GameObject mob = dungeon.mobs[j];
                        spawnPoints[i].mob = mob;
                        spawnPoints[i].SpawnMob();
                    }
                }
                
            }
        }
    }
    public void SpawnHealth()
    {
        healthSpawnPoint.SpawnHealth();
    }
    public int NoOfUnusedOpenings()
    {
        int unusedOpenings = 0;
        for (int i = 0; i < entryPoints.Length; ++i)
        {
            if (entryPoints[i].openingActive == false)
            {
                ++unusedOpenings;
            }
        }
        return unusedOpenings;
    }
    public int NoOfAllOpenings()
    {
        return entryPoints.Length;
    }
    
    public bool CheckForCollison()
    {
        if (CollisionDetect.isColliding) return true;
        else return false;

    }
}

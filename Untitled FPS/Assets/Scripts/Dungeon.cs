using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class used for generating a single dungeon floor.
/// Generation is done in stages in the update method.The reason is to make the OnTriggerEnter() function work properly
/// After a single floor is generated it is rated and given a score based on certain desirable aspects
/// Only the best dungeon gets finalized by the FinalizeDungeon() function.
/// </summary>
public class Dungeon : MonoBehaviour
{
    public List<GameObject> rooms;
    public List<GameObject> startingRooms;
    public List<GameObject> corridors;
    public List<GameObject> mobs;
    public List<int> gunTypes;
    public GameObject armory;
    public GameObject bossRoom;
    public GameObject door;
    public GameObject exitDoor;
    public GameObject wall;
    public GameObject healthpack;
    private NavMeshSurface[] surfaces;
    private float waitTime = 0.01f;
    private float currentTime = 0.0f;
    private float generationTime = 0.0f;
    private int repeats = 0;
    private List<DungeonPart> currentDungeonParts = new List<DungeonPart>();
    public float floorScore;
    public int goalNumberOfRooms, floorNumber;
    public int[] mobBias, gunTypeBias, gunRarityBias;
    public LayerMask layerMask;
    private bool dungeonFinished, stage1Finished, stage2Finished, stage3Finished, stage4Finished, extraStage = false;

    string weaponScorePath = @"c:\Users\Jasmundo\Documents\scores\weaponScore.txt";
    string roomScorePath = @"c:\Users\Jasmundo\Documents\scores\roomScore.txt";
    string timeScorePath = @"c:\Users\Jasmundo\Documents\scores\time.txt";
    string overallScorePath = @"c:\Users\Jasmundo\Documents\scores\overallScore.txt";
    string winnerScorePath = @"c:\Users\Jasmundo\Documents\scores\winnerScore.txt";

    // Spawns the starting room and adjacent corridors.
    void Start()
    {
        surfaces = GetComponents<NavMeshSurface>();

        GameObject startingRoomObj = Instantiate(startingRooms[0], transform.position, new Quaternion(), transform);
        DungeonPart startingRoom = new DungeonPart(startingRoomObj, this, GetEntryPoints(startingRoomObj), 0);
        startingRoom.validated = true;
        currentDungeonParts.Add(startingRoom);

        DungeonPart[] startingGeneratedRooms = currentDungeonParts[0].Generate();
        currentDungeonParts.AddRange(startingGeneratedRooms);
    }
    private void Update()
    {
        if (!dungeonFinished)
        {
            if (!stage1Finished && currentTime <= 0.0f)
            {
                if (currentDungeonParts.Count < goalNumberOfRooms && !isEveryPartValidated())
                {
                    for (int i = 0; i < currentDungeonParts.Count; ++i)
                    {
                        if (!currentDungeonParts[i].validated)
                        {
                            if (currentDungeonParts[i].CheckForCollison())
                            {
                                currentDungeonParts[i].parent.entryPoints[currentDungeonParts[i].parentEntryPoint].openingActive = false;
                                Destroy(currentDungeonParts[i].thisPart);
                                currentDungeonParts.RemoveAt(i);
                                currentTime = waitTime;
                                return;
                                //i--;
                            }
                            else
                            {
                                currentDungeonParts[i].validated = true;
                            }
                        }
                    }
                    int tempDungeonPartCount = currentDungeonParts.Count;
                    for (int i = 0; i < tempDungeonPartCount; ++i)
                    {
                        if (currentDungeonParts.Count < goalNumberOfRooms)
                        {
                            if (currentDungeonParts[i].NoOfUnusedOpenings() > 0)
                            {
                                DungeonPart[] generatedRooms = currentDungeonParts[i].Generate();
                                currentDungeonParts.AddRange(generatedRooms);
                            }
                        }
                    }
                    currentTime = waitTime;
                    return;
                }
                else if (currentDungeonParts.Count >= goalNumberOfRooms && !isEveryPartValidated())
                {
                    for (int i = 0; i < currentDungeonParts.Count; ++i)
                    {
                        //Debug.Log("Is validated(2): " + currentDungeonParts[i].validated);
                        if (!currentDungeonParts[i].validated)
                        {
                            if (currentDungeonParts[i].CheckForCollison())
                            {
                                currentDungeonParts[i].parent.entryPoints[currentDungeonParts[i].parentEntryPoint].openingActive = false;
                                Destroy(currentDungeonParts[i].thisPart);
                                currentDungeonParts.RemoveAt(i);
                                currentTime = waitTime;
                                return;
                            }
                            else
                            {
                                currentDungeonParts[i].validated = true;
                            }
                        }
                    }
                    currentTime = waitTime;
                    return;
                }
                else if (currentDungeonParts.Count < goalNumberOfRooms && isEveryPartValidated())
                {
                    repeats++;
                    int tempDungeonPartCount = currentDungeonParts.Count;
                    if (tempDungeonPartCount > currentDungeonParts.Count - 5 && repeats < 4)
                    {
                        for (int i = 0; i < tempDungeonPartCount; ++i)
                        {
                            if (currentDungeonParts[i].NoOfUnusedOpenings() > 0)
                            {
                                DungeonPart[] generatedRooms = currentDungeonParts[i].Generate();
                                currentDungeonParts.AddRange(generatedRooms);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < currentDungeonParts.Count; ++i)
                        {
                            if (currentDungeonParts[i].NoOfAllOpenings() == currentDungeonParts[i].NoOfUnusedOpenings())
                            {
                                currentDungeonParts[i].parent.entryPoints[currentDungeonParts[i].parentEntryPoint].openingActive = false;
                                Destroy(currentDungeonParts[i].thisPart);
                                currentDungeonParts.RemoveAt(i);
                                --i;
                            }
                        }
                        tempDungeonPartCount = currentDungeonParts.Count;
                        for (int i = 0; i < tempDungeonPartCount; ++i)
                        {
                            if (currentDungeonParts[i].NoOfUnusedOpenings() > 0)
                            {
                                DungeonPart[] generatedRooms = currentDungeonParts[i].Generate();
                                currentDungeonParts.AddRange(generatedRooms);
                            }
                        }
                        repeats = 0;
                    }
                    currentTime = waitTime;
                    return;
                }
                else if (currentDungeonParts.Count >= goalNumberOfRooms && isEveryPartValidated())
                {
                    currentTime = waitTime;
                    stage1Finished = true;
                }
            }
            /////////////////////////////////////////////////STAGE 2
            else if (stage1Finished && !stage2Finished && currentTime <= 0.0f)
            {
                int tempDungeonPartCount = currentDungeonParts.Count;
                for (int i = 0; i < tempDungeonPartCount; ++i)
                {
                    if (currentDungeonParts[i].partType == 1)
                    {
                        DungeonPart[] generatedRooms = currentDungeonParts[i].Generate();
                        currentDungeonParts.AddRange(generatedRooms);
                    }
                }
                currentTime = waitTime;
                stage2Finished = true;
                return;
            }
            /////////////////////////////////////////////////STAGE 3
            else if (stage2Finished && !stage3Finished && currentTime <= 0.0f)
            {
                for (int i = 0; i < currentDungeonParts.Count; ++i)
                {
                    //Debug.Log("Is validated: " + currentDungeonParts[i].validated);
                    if (!currentDungeonParts[i].validated)
                    {
                        if (currentDungeonParts[i].CheckForCollison())
                        {
                            currentDungeonParts[i].parent.entryPoints[currentDungeonParts[i].parentEntryPoint].openingActive = false;
                            Destroy(currentDungeonParts[i].thisPart);
                            currentDungeonParts.RemoveAt(i);
                            currentTime = waitTime;
                            return;
                            //i--;
                        }
                        else
                        {
                            currentDungeonParts[i].validated = true;
                        }
                    }
                }
                for (int i = 0; i < currentDungeonParts.Count; ++i)
                {
                    if (currentDungeonParts[i].partType == 1)
                    {
                        if (currentDungeonParts[i].NoOfAllOpenings() == currentDungeonParts[i].NoOfUnusedOpenings())
                        {
                            currentDungeonParts[i].parent.entryPoints[currentDungeonParts[i].parentEntryPoint].openingActive = false;
                            Destroy(currentDungeonParts[i].thisPart);
                            currentDungeonParts.RemoveAt(i);
                            --i;
                        }
                    }
                }

                currentTime = waitTime;
                stage3Finished = true;

                int partNo = GetRandomPartWithUnusedOpenings();
                DungeonPart armory = currentDungeonParts[partNo].SpawnArmory();
                currentDungeonParts.Add(armory);
                return;
            }
            ////////////////////////////////////////////////STAGE 4
            else if (stage3Finished && !stage4Finished && currentTime <= 0.0f)
            {
                if (currentDungeonParts[currentDungeonParts.Count - 1].CheckForCollison())
                {    
                    int tmp = currentDungeonParts.Count - 1;
                    currentDungeonParts[tmp].parent.entryPoints[currentDungeonParts[tmp].parentEntryPoint].openingActive = false;
                    Destroy(currentDungeonParts[tmp].thisPart);
                    currentDungeonParts.RemoveAt(tmp);
                    gunTypes.RemoveRange(gunTypes.Count - 3, 3);
                    currentTime = waitTime;

                    int partNo = GetRandomPartWithUnusedOpenings();
                    DungeonPart armory = currentDungeonParts[partNo].SpawnArmory();
                    currentDungeonParts.Add(armory);

                    return;
                }
                else stage4Finished = true;
                if (floorNumber == 5)
                {
                    int partNo2 = GetRandomPartWithUnusedOpenings();
                    DungeonPart bossRoom = currentDungeonParts[partNo2].SpawnBossRoom();
                    currentDungeonParts.Add(bossRoom);
                }
            }
            /////////////////////////////////////////////////EXTRA STAGE
            else if (stage4Finished && !dungeonFinished && currentTime <= 0.0f && floorNumber ==5 && !extraStage)
            {
                if (currentDungeonParts[currentDungeonParts.Count - 1].CheckForCollison())
                {
                    int tmp = currentDungeonParts.Count - 1;
                    currentDungeonParts[tmp].parent.entryPoints[currentDungeonParts[tmp].parentEntryPoint].openingActive = false;
                    Destroy(currentDungeonParts[tmp].thisPart);
                    currentDungeonParts.RemoveAt(tmp);
                    currentTime = waitTime;

                    int partNo = GetRandomPartWithUnusedOpenings();
                    DungeonPart bossRoom2 = currentDungeonParts[partNo].SpawnBossRoom();
                    currentDungeonParts.Add(bossRoom2);

                    return;
                }
                else extraStage = true;
            }
            /////////////////////////////////////////////////STAGE 5
            else if (stage4Finished && !dungeonFinished && currentTime <= 0.0f && (extraStage || floorNumber!=5))
            {
                float roomScore, overallScore;
                float weaponScore = 0f;
                float[] desiredWeaponValues = new float[gunTypeBias.Length - 1];
                float[] actualWeaponValues = new float[gunTypeBias.Length - 1];
                int[] weaponTypesCount = new int[gunTypeBias.Length - 1];

                roomScore = (Mathf.Abs((float)(currentDungeonParts.Count - goalNumberOfRooms) / (float)goalNumberOfRooms)) * 1.845f;

                for (int i = 0; i < desiredWeaponValues.Length; ++i)
                {
                    desiredWeaponValues[i] = ((float)gunTypeBias[i + 1] - (float)gunTypeBias[i]) / 100f;
                }
                for (int i = 0; i < gunTypes.Count; ++i)
                {
                    weaponTypesCount[gunTypes[i]]++;
                }
                for (int i = 0; i < actualWeaponValues.Length; ++i)
                {
                    actualWeaponValues[i] = (float)weaponTypesCount[i] / (float)gunTypes.Count;
                }
                for (int i = 0; i < actualWeaponValues.Length; ++i)
                {
                    weaponScore += Mathf.Abs(desiredWeaponValues[i] - actualWeaponValues[i]);
                }
                //lower = better
                overallScore = roomScore + weaponScore;
                floorScore = overallScore;

                dungeonFinished = true;
                //Debug.Log("Generation Time: " + generationTime + " s");
                //Debug.Log("Room Score = " + roomScore);
                //Debug.Log("Corridor Score = " + corridorScore);
                //Debug.Log("desiredWeaponValues0 = " + desiredWeaponValues[0]);
                //Debug.Log("actualWeaponValues0 = " + actualWeaponValues[0]);
                //Debug.Log("desiredWeaponValues1 = " + desiredWeaponValues[1]);
                //Debug.Log("actualWeaponValues1 = " + actualWeaponValues[1]);
                //Debug.Log("actualWeaponValues1 = " + gunTypes.Count);
                //Debug.Log("actualWeaponValues1 = " + weaponTypesCount.Length);
                //Debug.Log("weaponTypesCount0 = " + weaponTypesCount[0]);
                //Debug.Log("weaponTypesCount1 = " + weaponTypesCount[1]);
                //Debug.Log("Weapon Score = " + weaponScore);
                //Debug.Log("Overall Score = " + overallScore);

                WriteScoreToFile(roomScorePath, roomScore.ToString());
                WriteScoreToFile(weaponScorePath, weaponScore.ToString());
                WriteScoreToFile(overallScorePath, overallScore.ToString());

            }
            currentTime -= Time.deltaTime;
            generationTime += Time.deltaTime;
        }
    }
    private void WriteScoreToFile(string path, string value)
    {
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(value);
            }
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(value);
        }
    }
    /// Used to finalized a dungeon.
    /// 
    /// Spawns elements such as:\n
    /// -decorative elements\n
    /// -doors and walls\n
    /// -mobs\n
    /// -health packs\n
    /// Additonally it builds the NavMeshes and turns off the collison detection scripts.
    public void FinalizeDungeon()
    {
        int goalHealthpackNo = Mathf.CeilToInt(currentDungeonParts.Count / 10);
        int currentHealthpackNo = 0;
        CollisionDetect[] collisionDetects = GetComponentsInChildren<CollisionDetect>();
        List<DecorativeElement> decorativeElements = new List<DecorativeElement>();
        
        for (int i = 0; i < currentDungeonParts.Count; ++i)
        {
            DecorativeElement[] roomDecor = currentDungeonParts[i].thisPart.GetComponentsInChildren<DecorativeElement>();
            if (roomDecor != null) decorativeElements.AddRange(roomDecor);
        }

        for (int i = 0; i < decorativeElements.Count; ++i)
        {
            decorativeElements[i].SpawnElement();
        }

        surfaces[0].BuildNavMesh();
        surfaces[1].BuildNavMesh();

        for (int i= 0; i < collisionDetects.Length; ++i) 
        {
            collisionDetects[i].enabled = false;
        }

        for (int i = 0; i < currentDungeonParts.Count - 2; ++i)
        {
            if (currentDungeonParts[i].partType == 0) currentDungeonParts[i].SpawnDoors();
            else currentDungeonParts[i].SpawnWalls();
        }
        if (floorNumber != 5)
        {
            currentDungeonParts[currentDungeonParts.Count - 2].SpawnExitDoor();
            currentDungeonParts[currentDungeonParts.Count - 2].SpawnDoors();
        }
        else
        {
            currentDungeonParts[currentDungeonParts.Count - 3].SpawnDoors();
        }
        if (floorNumber != 5)
        {
            for (int i = 3; i < currentDungeonParts.Count - 1; ++i)
            {
                currentDungeonParts[i].SpawnMobs();
            }
        }

        else
        {
            for (int i = 3; i < currentDungeonParts.Count - 2; ++i)
            {
                currentDungeonParts[i].SpawnMobs();
            }
        }

        while (currentHealthpackNo < goalHealthpackNo)
        {
            int partNo = Random.Range(0, currentDungeonParts.Count);
            if (currentDungeonParts[partNo].partType == 1)
            {
                currentDungeonParts[partNo].SpawnHealth();
                currentHealthpackNo++;
            }

        }
        dungeonFinished = true;
        WriteScoreToFile(timeScorePath, generationTime.ToString());
        WriteScoreToFile(winnerScorePath, floorScore.ToString());
        Debug.Log("dungeonFinished: " + dungeonFinished);
        Debug.Log("Generation Time: " + generationTime + " s");
        GameObject.Find("CanvasLoading").SetActive(false);
    }
    private bool isEveryPartValidated()
    {
        for (int i = 0; i < currentDungeonParts.Count; ++i)
        {
            if (!currentDungeonParts[i].validated) return false;
        }
        return true;
    }
    private int GetRandomPartWithUnusedOpenings()
    {
        int partNo;
        while (true)
        {
            partNo = Random.Range(0, currentDungeonParts.Count - 1);
            if (currentDungeonParts[partNo].NoOfUnusedOpenings() > 0) return partNo; ;
        } 
    }
    /// <summary>
    /// Used to create new EntryPoint objects for a room or corridor.
    /// </summary>
    /// <param name="thisPart"> -a newly spawned room or corridor</param>
    /// <returns>A list of created EntryPoint objects</returns>
    public EntryPoint[] GetEntryPoints(GameObject thisPart)
    {
        GameObject entryPointsObject = thisPart.transform.Find("EntryPoints").gameObject;
        Transform[] entryPointsPositions = entryPointsObject.GetComponentsInChildren<Transform>();
        EntryPoint[] entryPoints = new EntryPoint[entryPointsPositions.Length - 1];
        for (int i = 1; i < entryPointsPositions.Length; ++i)
        {
            entryPoints[i - 1] = new EntryPoint(this, entryPointsPositions[i]);
        }
        return entryPoints;
    }
    /// <summary>
    /// Used to create new SpawnPoint objects for a room or corridor.
    /// </summary>
    /// <param name="thisPart"> -a newly spawned room or corridor</param>
    /// <returns>A list of created SpawnPoint objects</returns>
    public SpawnPoint[] GetSpawnPoints(GameObject thisPart)
    {
        GameObject spawnPointsObject = thisPart.transform.Find("SpawnPoints").gameObject;
        Transform[] spawnPointsPositions = spawnPointsObject.GetComponentsInChildren<Transform>();
        SpawnPoint[] spawnPoints = new SpawnPoint[spawnPointsPositions.Length - 1];
        for (int i = 1; i < spawnPointsPositions.Length; ++i)
        {
            spawnPoints[i - 1] = new SpawnPoint(this, spawnPointsPositions[i]);
        }
        return spawnPoints;
    }
    /// <summary>
    /// Used to create new HealthSpawnPoint objects for a room or corridor.
    /// </summary>
    /// <param name="thisPart"> -a newly spawned room or corridor</param>
    /// <returns>A list of created HealthSpawnPoint objects</returns>
    public HealthSpawnPoint GetHealthSpawnPoint(GameObject thisPart)
    {
        GameObject spawnPointObject = thisPart.transform.Find("HealthSpawnPoint").gameObject;
        Transform spawnPointPosition = spawnPointObject.GetComponent<Transform>();
        HealthSpawnPoint spawnPoint = new HealthSpawnPoint(this, spawnPointPosition, healthpack);
        return spawnPoint;
    }
    public bool IsDungeonFinished()
    {
        return dungeonFinished;
    }
    private void BuildMeshSurfaces()
    {
        NavMeshSurface[] meshSurfaces = new NavMeshSurface[currentDungeonParts.Count]; ;
        for (int i = 0; i < meshSurfaces.Length; ++i)
        {
            meshSurfaces[i] = currentDungeonParts[i].NavMeshSurface;
        }
        for (int i = 0; i < meshSurfaces.Length; ++i)
        {
            meshSurfaces[i].BuildNavMesh();
        }
    }

}

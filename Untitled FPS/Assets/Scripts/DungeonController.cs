using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public List<GameObject> dungeonFloors;
    public List<GameObject> initialDungeonFloors;
    public List<GameObject>[] decorativeElements = new List<GameObject>[5];
    public List<GameObject> decor0;
    public List<GameObject> decor1;
    public List<GameObject> decor2;
    public List<GameObject> decor3;
    public List<GameObject> decor4;
    public GameObject currentDungeonFloor;
    public GameObject loadingScreen;
    public int currentFloorNr = 0;
    public Vector3[] floorLocations;
    private List<float> floorScores = new List<float>();
    private bool finalizationCompleted = false;
    void Start()
    {
        decorativeElements[0] = decor0;
        decorativeElements[1] = decor1;
        decorativeElements[2] = decor2;
        decorativeElements[3] = decor3;
        decorativeElements[4] = decor4;

        for (int i = 0; i < floorLocations.Length; ++i)
        {
            initialDungeonFloors.Add(Instantiate(dungeonFloors[currentFloorNr], floorLocations[i], new Quaternion()));
        }

        loadingScreen.SetActive(true);
    }
	private void Update()
	{
        if (!finalizationCompleted)
        {
            if (AllFloorsGenerated())
            {
                int bestScoreFloorId;
                GetScores();
                bestScoreFloorId = GetBestScore();
                currentDungeonFloor = initialDungeonFloors[bestScoreFloorId];
                DeleteLowScoreFloors(bestScoreFloorId);
                currentDungeonFloor.GetComponent<Dungeon>().FinalizeDungeon();
                GameObject.Find("Player").transform.position = currentDungeonFloor.transform.position + new Vector3(0, 1, 0);
                loadingScreen.SetActive(false);

                finalizationCompleted = true;
            }
        }
	}
	public void LoadNextFloor()
    {
        Destroy(currentDungeonFloor);
        initialDungeonFloors.Clear();
        floorScores.Clear();
        currentFloorNr += 1;
        loadingScreen.SetActive(true);
        for (int i = 0; i < floorLocations.Length; ++i)
        {
            initialDungeonFloors.Add(Instantiate(dungeonFloors[currentFloorNr], floorLocations[i], new Quaternion()));
        }
        finalizationCompleted = false;

    }
    private bool AllFloorsGenerated()
    {
        for (int i = 0; i < initialDungeonFloors.Count; ++i)
        {
            if (!initialDungeonFloors[i].GetComponent<Dungeon>().IsDungeonFinished()) return false;
        }
        return true;
    }
    private void GetScores()
    {
        for (int i = 0; i < initialDungeonFloors.Count; ++i)
        {
            floorScores.Add(initialDungeonFloors[i].GetComponent<Dungeon>().floorScore);
        }
    }
    private int GetBestScore()
    {
        //lower = better
        float min = floorScores[0];
        int minPosition = 0;
        for (int i = 1; i < floorScores.Count; ++i)
        {
            if (floorScores[i] < min)
            {
                min = floorScores[i];
                minPosition = i;
            }
        }
        return minPosition;
    }
    private void DeleteLowScoreFloors(int highestScoreFloorId)
    {
        for (int i = 0; i < initialDungeonFloors.Count; ++i)
        {
            if (i < highestScoreFloorId)
            {
                Destroy(initialDungeonFloors[i]);
                initialDungeonFloors.RemoveAt(i);
                i--;
                highestScoreFloorId--;
            }
            else if (i > highestScoreFloorId)
            {
                Destroy(initialDungeonFloors[i]);
                initialDungeonFloors.RemoveAt(i);
                i--;
            }
        }
    }
}

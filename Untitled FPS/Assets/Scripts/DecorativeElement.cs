using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorativeElement : MonoBehaviour
{
    public int decorativeElementType;
    private DungeonController dungeonController;
    void Start()
    {
        dungeonController = GameObject.Find("DungeonController").GetComponent<DungeonController>();
    }

    public void SpawnElement()
    {
        Instantiate(dungeonController.decorativeElements[decorativeElementType][Random.Range(0, dungeonController.decorativeElements[decorativeElementType].Count)], transform.position, transform.rotation, transform.parent);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component of transforms existing in rooms that designate a location of a decorative element.
/// </summary>
public class DecorativeElement : MonoBehaviour
{
    public int decorativeElementType;
    private DungeonController dungeonController;
    void Start()
    {
        dungeonController = GameObject.Find("DungeonController").GetComponent<DungeonController>();
    }
    /// <summary>
    /// Spawns a random element of desired type in the right location.
    /// </summary>
    public void SpawnElement()
    {
        Instantiate(dungeonController.decorativeElements[decorativeElementType][Random.Range(0, dungeonController.decorativeElements[decorativeElementType].Count)], transform.position, transform.rotation, transform.parent);
    }

}

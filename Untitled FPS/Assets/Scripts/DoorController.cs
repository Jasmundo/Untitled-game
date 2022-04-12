using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Opens and closes a door depending on the player's distance from it. Also controlls animations and audio.
/// </summary>
public class DoorController : MonoBehaviour
{
    public GameObject player, door1, door2;
    public float distance;
    private Animator animator;
    private AudioSource audio;
    private bool isDoorOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= distance)
        {
            animator.SetBool("character_nearby", true);
            door1.GetComponent<BoxCollider>().enabled = false;
            door2.GetComponent<BoxCollider>().enabled = false;
            if (!isDoorOpen)
            {
                audio.Play();
                isDoorOpen = true;
            }
        }
        else
        {
            animator.SetBool("character_nearby", false);
            door1.GetComponent<BoxCollider>().enabled = true;
            door2.GetComponent<BoxCollider>().enabled = true;
            if (isDoorOpen)
            {
                //audio.Play();
                isDoorOpen = false;
            }
        }
    }
}

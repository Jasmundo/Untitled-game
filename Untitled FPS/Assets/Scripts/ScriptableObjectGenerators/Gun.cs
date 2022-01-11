using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName ="Gun")]
public class Gun : ScriptableObject
{
	public string gunName;
	public string rarity;
	public int rarityInt;
	public int damage;
	public int pellets;
	public GameObject prefab;
	public float bloom;
	public float recoil;
	public float kickback;
	public float aimSpeed;
	public float fireRate;
	public bool isAutomatic;
	public bool isProjectile;
	public Sprite icon;
	public AudioClip[] gunshotSounds;
	public AudioClip equipSound;
}

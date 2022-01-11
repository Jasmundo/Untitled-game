using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCrate : MonoBehaviour
{
    private Dungeon dungeonFloor;
    private int gunType, gunRarity;
    private int[][] pistolDamage = new int[4][] { new int[2] { 14, 17 }, new int[2] { 18, 21 }, new int[2] { 22, 25 }, new int[2] { 26, 29 } };
    private float[][] pistolFireRate = new float[4][] { new float[2] { 0.2f, 0.19f }, new float[2] { 0.18f, 0.17f }, new float[2] { 0.16f, 0.15f }, new float[2] { 0.14f, 0.13f } };
    private float[][] pistolAccuracy = new float[4][] { new float[2] { 20, 17 }, new float[2] { 16, 13 }, new float[2] { 12, 9 }, new float[2] { 8, 5 } };
    private int[][] machinegunDamage = new int[4][] { new int[2] { 15, 17 }, new int[2] { 18, 21 }, new int[2] { 22, 24 }, new int[2] { 25, 28 } };
    private float[][] machinegunFireRate = new float[4][] { new float[2] { 0.18f, 0.17f }, new float[2] { 0.16f, 0.14f }, new float[2] { 0.13f, 0.12f }, new float[2] { 0.11f, 0.1f } };
    private float[][] machinegunAccuracy = new float[4][] { new float[2] { 25, 22 }, new float[2] { 21, 17 }, new float[2] { 16, 13 }, new float[2] { 12, 9 } };
    private int[][] shotgunDamage = new int[4][] { new int[2] { 7, 9}, new int[2] { 10, 12 }, new int[2] { 13, 15 }, new int[2] { 16, 17 } };
    private float[][] shotgunFireRate = new float[4][] { new float[2] { 1f, 0.87f }, new float[2] { 0.86f, 0.72f }, new float[2] { 0.71f, 0.6f }, new float[2] { 0.59f, 0.5f } };
    private float[][] shotgunAccuracy = new float[4][] { new float[2] { 40, 36 }, new float[2] { 35, 31 }, new float[2] { 30, 25 }, new float[2] { 24, 20 } };
    private int[][] rocketLauncherDamage = new int[4][] { new int[2] { 110, 139 }, new int[2] { 140, 175 }, new int[2] { 176, 219 }, new int[2] { 220, 250 } };
    private float[][] rocketLauncherFireRate = new float[4][] { new float[2] { 2f, 1.71f }, new float[2] { 1.7f, 1.47f }, new float[2] { 1.46f, 1.21f }, new float[2] { 1.2f, 1f } };
    public GameObject[] gunPrefabs;
    public Sprite[] gunIcons;
    public AudioClip[] pistolSounds, machinegunSounds, shotgunSounds;
    public AudioClip[] equipSounds;
    public Gun generatedGun;
    public bool isEmpty = false;
    void Start()
    {
        GameObject room = transform.parent.gameObject;
        dungeonFloor = room.transform.parent.gameObject.GetComponent<Dungeon>();
        gunType = GetGunType(dungeonFloor.gunTypeBias);
        gunRarity = GetGunRarity(dungeonFloor.gunRarityBias);
        generatedGun = GenerateGun(gunType, gunRarity);

        dungeonFloor.gunTypes.Add(gunType);
    }

    private int GetGunType(int[] gunTypeBias)
    {
        int rand = Random.Range(0, 99);
        for (int i = 0; i < gunTypeBias.Length -1; ++i)
        {
            if (rand > gunTypeBias[i] && rand < gunTypeBias[i + 1])
            {
                return i;
            }
        }
        return 0;
    }

    private int GetGunRarity(int[] gunRarityBias)
    {
        int rand = Random.Range(0, 99);
        for (int i = 0; i < gunRarityBias.Length -1; ++i)
        {
            if (rand > gunRarityBias[i] && rand < gunRarityBias[i + 1])
            {
                return i;
            }
        }
        return 0;
    }
    private Gun GenerateGun(int gunType, int gunRarity)
    {
        Gun tempGun = new Gun();

        if (gunType == 0)
        {
            tempGun.gunName = "Pistol";
            tempGun.rarity = GetRarityString(gunRarity);
            tempGun.rarityInt = gunRarity;
            tempGun.damage = Random.Range(pistolDamage[gunRarity][0], pistolDamage[gunRarity][1] + 1);
            tempGun.pellets = 0;
            tempGun.prefab = gunPrefabs[0];
            tempGun.bloom = Random.Range(pistolAccuracy[gunRarity][0], pistolAccuracy[gunRarity][1]);
            tempGun.recoil = 10;
            tempGun.kickback = 0.15f;
            tempGun.aimSpeed = 20;
            tempGun.fireRate = Random.Range(pistolFireRate[gunRarity][0], pistolFireRate[gunRarity][1]);
            tempGun.isAutomatic = false;
            tempGun.isProjectile = false;
            tempGun.icon = gunIcons[gunType];
            tempGun.gunshotSounds = pistolSounds;
            tempGun.equipSound = equipSounds[gunType];
        }
        if (gunType == 1)
        {
            tempGun.gunName = "Machinegun";
            tempGun.rarity = GetRarityString(gunRarity);
            tempGun.rarityInt = gunRarity;
            tempGun.damage = Random.Range(machinegunDamage[gunRarity][0], machinegunDamage[gunRarity][1] + 1);
            tempGun.pellets = 0;
            tempGun.prefab = gunPrefabs[1];
            tempGun.bloom = Random.Range(machinegunAccuracy[gunRarity][0], machinegunAccuracy[gunRarity][1]);
            tempGun.recoil = 10;
            tempGun.kickback = 0.15f;
            tempGun.aimSpeed = 20;
            tempGun.fireRate = Random.Range(machinegunFireRate[gunRarity][0], machinegunFireRate[gunRarity][1]);
            tempGun.isAutomatic = true;
            tempGun.isProjectile = false;
            tempGun.icon = gunIcons[gunType];
            tempGun.gunshotSounds = machinegunSounds;
            tempGun.equipSound = equipSounds[gunType];
        }
        if (gunType == 2)
        {
            tempGun.gunName = "Shotgun";
            tempGun.rarity = GetRarityString(gunRarity);
            tempGun.rarityInt = gunRarity;
            tempGun.damage = Random.Range(shotgunDamage[gunRarity][0], shotgunDamage[gunRarity][1] + 1);
            tempGun.pellets = 10;
            tempGun.prefab = gunPrefabs[2];
            tempGun.bloom = Random.Range(shotgunAccuracy[gunRarity][0], shotgunAccuracy[gunRarity][1]);
            tempGun.recoil = 10;
            tempGun.kickback = 0.3f;
            tempGun.aimSpeed = 20;
            tempGun.fireRate = Random.Range(shotgunFireRate[gunRarity][0], shotgunFireRate[gunRarity][1]);
            tempGun.isAutomatic = false;
            tempGun.isProjectile = false;
            tempGun.icon = gunIcons[gunType];
            tempGun.gunshotSounds = shotgunSounds;
            tempGun.equipSound = equipSounds[gunType];
        }
        if (gunType == 3)
        {
            tempGun.gunName = "Rocket Launcher";
            tempGun.rarity = GetRarityString(gunRarity);
            tempGun.rarityInt = gunRarity;
            tempGun.damage = Random.Range(rocketLauncherDamage[gunRarity][0], rocketLauncherDamage[gunRarity][1] + 1);
            tempGun.pellets = 0;
            tempGun.prefab = gunPrefabs[3];
            tempGun.bloom = 1;
            tempGun.recoil = 10;
            tempGun.kickback = 0.15f;
            tempGun.aimSpeed = 20;
            tempGun.fireRate = Random.Range(rocketLauncherFireRate[gunRarity][0], rocketLauncherFireRate[gunRarity][1]);
            tempGun.isAutomatic = false;
            tempGun.isProjectile = true;
            tempGun.icon = gunIcons[gunType];
        }

        return tempGun;
    }
    private string GetRarityString(int gunRarity)
    {
        if (gunRarity == 0) return "Common";
        else if (gunRarity == 1) return "Rare";
        else if (gunRarity == 2) return "Epic";
        else return "Legendary";
    }
}

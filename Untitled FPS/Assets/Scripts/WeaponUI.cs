using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    public GameObject[] weapons;
    public TextMeshProUGUI[] gunType;
    public TextMeshProUGUI[] gunRarity;
    public TextMeshProUGUI[] gunDamage;
    public TextMeshProUGUI[] gunFirerate;
    public TextMeshProUGUI[] gunAccuracy;
    public TextMeshProUGUI[] gunDPS;
    public Image[] gunIcon;
    private bool[] slotUsed = { false, false, false, false };
    private int currentlySelected;

    public void FillGunWindow(Gun gun, int weaponNo)
    {
        float firerate = 1 / gun.fireRate;
        float accuracy = (50 - gun.bloom) * 2;
        float dps = gun.damage * firerate;
        if (gun.pellets != 0) dps *= 10;

        gunType[weaponNo].text = gun.gunName;
        gunRarity[weaponNo].text = gun.rarity;
        gunRarity[weaponNo].color = GetRarityColor(gun.rarityInt);
        gunDamage[weaponNo].text = "Damage: " + gun.damage;
        gunFirerate[weaponNo].text = "Firerate: " + firerate.ToString("0.00") + "/s";
        gunAccuracy[weaponNo].text = "Accuracy: " + accuracy.ToString("0.00");
        gunDPS[weaponNo].text = "DPS: " + dps.ToString("0.00");
        gunIcon[weaponNo].sprite = gun.icon;
        slotUsed[weaponNo] = true;
    }

    public void ShowGunWindows()
    {
        for (int i = 0; i < 4; ++i)
        {
            if (slotUsed[i])
            {
                weapons[i].SetActive(true);
            }
        }
    }

    public void HideGunWindows()
    {
        for (int i = 0; i < 4; ++i)
        {
            weapons[i].SetActive(false);
        }
    }

    public void ChangeCurentlySelected(int newSelection)
    {
        currentlySelected = newSelection;
        for (int i = 0; i < 4; ++i)
        {
            if (i == currentlySelected)
            {
                Image tmp = weapons[i].GetComponent<Image>();
                tmp.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
            }
            else
            {
                Image tmp = weapons[i].GetComponent<Image>();
                tmp.color = new Color(0.0f, 0.0f, 0.0f, 0.6f);
            }
        }
        
    }

    private Color GetRarityColor(int rarity)
    {
        if (rarity == 0) return new Color(0, 255, 0);
        else if (rarity == 1) return new Color(0, 47, 255);
        else if (rarity == 2) return new Color(179, 0, 255);
        else return new Color(255, 213, 0);
    }
}

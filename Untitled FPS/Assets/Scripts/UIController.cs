using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Camera normalCam;
    public LayerMask isCrate;
    public GameObject tooltip;
    public TextMeshProUGUI gunType;
    public TextMeshProUGUI gunRarity;
    public TextMeshProUGUI gunDamage;
    public TextMeshProUGUI gunFirerate;
    public TextMeshProUGUI gunAccuracy;
    public TextMeshProUGUI gunDPS;
    public Image gunIcon;
    RaycastHit hit;
    void Start()
    {
        
    }

    void Update()
    {
        if (Physics.Raycast(normalCam.transform.position, normalCam.transform.forward, out hit, 2f, isCrate))
        {
            WeaponCrate crate = hit.collider.GetComponent<WeaponCrate>();
            if (!crate.isEmpty)
            {
                if (!tooltip.activeInHierarchy)
                {
                    //Debug.Log("isCrate hit");

                    Gun gun = crate.generatedGun;

                    if (gun != null)
                    {
                        float firerate = 1 / gun.fireRate;
                        float accuracy = (50 - gun.bloom) * 2;
                        float dps = gun.damage * firerate;
                        if (gun.pellets != 0) dps *= 10;

                        gunType.text = gun.gunName;
                        gunRarity.text = gun.rarity;
                        gunRarity.color = GetRarityColor(gun.rarityInt);
                        gunDamage.text = "Damage: " + gun.damage;
                        gunFirerate.text = "Firerate: " + firerate.ToString("0.00") + "/s";
                        gunAccuracy.text = "Accuracy: " + accuracy.ToString("0.00");
                        gunDPS.text = "DPS: " + dps.ToString("0.00");
                        gunIcon.sprite = gun.icon;

                        tooltip.SetActive(true);
                    }
                }
            }
            else
            {
                tooltip.SetActive(false);
            }
        }
        else
        {
            tooltip.SetActive(false);
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

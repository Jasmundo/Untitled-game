using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	#region Variables

	public Gun[] loadout;
    public Transform weaponParent;
    public GameObject bulletholePrefab;
    public LayerMask canBeShot;
    public LayerMask canTakeDamage;
    public WeaponUI weaponUI;
    public GameObject crosshair;
    public AudioSource audioSource;

    private bool isAiming;
    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;

    #endregion
    #region Monobehaviour callbacks

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Equip(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Equip(3);
        if (currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));

            if (Input.GetMouseButtonDown(1)) UpdateCrosshair();
            if (Input.GetMouseButtonUp(1)) UpdateCrosshair();

            if (Input.GetMouseButtonDown(0) && currentCooldown <=0)
            {
                Shoot();
            }

            if (Input.GetMouseButton(0) && currentCooldown <= 0 && loadout[currentIndex].isAutomatic ==true)
            {
                Shoot();
            }

            //weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            //firerate cooldown
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
        }
    }

	#endregion
	#region Private Methods
	void Equip(int p_ind)
    {
        if (loadout[p_ind] != null)
        {
            if (currentWeapon != null) Destroy(currentWeapon);

            currentIndex = p_ind;

            GameObject newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localEulerAngles = Vector3.zero;

            currentWeapon = newWeapon;

            weaponUI.ChangeCurentlySelected(p_ind);

            audioSource.PlayOneShot(loadout[currentIndex].equipSound);
        }
    }
    void Aim(bool p_isAiming)
    {
        Transform anchor = currentWeapon.transform.Find("Anchor");
        Transform ads = currentWeapon.transform.Find("States/ADS");
        Transform hip = currentWeapon.transform.Find("States/Hip");

        if (p_isAiming)
        {
            anchor.position = Vector3.Lerp(anchor.position, ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            isAiming = true;
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            isAiming = false;
        }
    }
    
    void Shoot()
    {
        Transform spawn = transform.Find("Cameras/NormalCamera");

        //cooldown
        currentCooldown = loadout[currentIndex].fireRate;
        if (loadout[currentIndex].isProjectile)
        {
            GunfireController gunfireController = currentWeapon.transform.Find("Anchor/Design").GetComponentInChildren<GunfireController>();
            gunfireController.FireWeapon(loadout[currentIndex].damage, loadout[currentIndex].fireRate);

        }
        else
        {
            for (int i = 0; i < Mathf.Max(1, loadout[currentIndex].pellets); i++)
            {
                //bloom(rozrzut pocisków)
                Vector3 bloom = spawn.position + spawn.forward * 1000f;
                bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.up;
                bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.right;
                bloom -= spawn.position;
                bloom.Normalize();

                //raycast
                RaycastHit hit;

                if (Physics.Raycast(spawn.position, bloom, out hit, 1000f, canTakeDamage))
                {
                    EnemyAi enemy = hit.collider.GetComponent<EnemyAi>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(loadout[currentIndex].damage);
                    }
                }
                else if (Physics.Raycast(spawn.position, bloom, out hit, 1000f, canBeShot))
                {
                    GameObject newBulletHole = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    newBulletHole.transform.LookAt(hit.point + hit.normal);
                    Destroy(newBulletHole, 5f);
                }
            }
            int gunshotSound = Random.Range(0, loadout[currentIndex].gunshotSounds.Length);
            audioSource.PlayOneShot(loadout[currentIndex].gunshotSounds[gunshotSound]);
        }
        
        //recoil
        if (!isAiming)
        {
            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        }

        //kickback
        if (!isAiming)
        {
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
        }
        else
        {
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback * 0.2f;
        }

    }
    public void NewWeapon(Gun gun)
    {
        for (int i = 0; i < loadout.Length; ++i)
        {
            if (loadout[i] == null)
            {
                loadout[i] = gun;
                Equip(i);
                weaponUI.FillGunWindow(gun, i);
                return;
            }
        }
        loadout[currentIndex] = gun;
        Equip(currentIndex);
        weaponUI.FillGunWindow(gun, currentIndex);
    }
    private void UpdateCrosshair()
    {
        if(isAiming) crosshair.SetActive(false);
        else crosshair.SetActive(true);
    }
	#endregion
}

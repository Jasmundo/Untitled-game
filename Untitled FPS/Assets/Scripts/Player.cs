using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	#region Variables

	public float speed;
    public float sprintModifier;
    public float jumpForce;
    public int maxHealth;
    public int healthPackNo;

    public Camera normalCam;
    public Transform groundDetector;
    public Transform weaponParent;
    public LayerMask ground, exit, crate;

    private Transform healthbar;
    private Text healthValue;
    private Text healthPackNoText;
    private DungeonController dungeonController;
    private Weapon weapon;
    private WeaponUI weaponUI;
    private AudioSource audioSource;

    private Rigidbody rig;

    private Vector3 weaponParentOrigin;

    private float movementCounter;
    private float idleCounter;

    private float baseFov;
    private float sprintFovModifier = 1.3f;
    private int currentHealth;

	#endregion
	#region Monobehaviour Callbacks
	void Start()
    {
        currentHealth = maxHealth;
        healthPackNo = 0;

        baseFov = normalCam.fieldOfView;
        //Camera.main.enabled = false;
        rig = GetComponent<Rigidbody>();
        weaponParentOrigin = weaponParent.localPosition;

        healthbar = GameObject.Find("HUD/Health/Healthbar").transform;
        healthValue = GameObject.Find("HUD/Health/HealthValue").GetComponent<Text>();
        healthPackNoText = GameObject.Find("HUD/HealthPacks/HealthPackNumber").GetComponent<Text>();
        dungeonController = GameObject.Find("DungeonController").GetComponent<DungeonController>();
        weaponUI = GameObject.Find("CanvasUI/EquipedWeaponsUI").GetComponent<WeaponUI>();
        weapon = GetComponent<Weapon>();
        audioSource = GetComponent<AudioSource>();
        RefreshHealthbar();
        RefreshHealthPack();
    }
	private void Update()
	{
        //Axis
        float hmove = Input.GetAxisRaw("Horizontal");
        float vmove = Input.GetAxisRaw("Vertical");

        //Controls
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool interact = Input.GetKeyDown(KeyCode.E);
        bool heal = Input.GetKeyDown(KeyCode.H);
        bool inventory = Input.GetKey(KeyCode.Tab);

        //States
        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jump && isGrounded;
        bool isSprinting = sprint && vmove > 0 && !isJumping && isGrounded;
        bool isAiming = Input.GetMouseButton(1);

        //Jumping
        if (isJumping)
        {
            rig.AddForce(Vector3.up * jumpForce);
        }

        //Footsteps audio
        if (vmove > 0 && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if(vmove <=0)
        {
            audioSource.Stop();
        }

        //Headbob
        if (hmove == 0 && vmove == 0)
        {
            if(isAiming) HeadBob(idleCounter, 0, 0, 2f);
            else HeadBob(idleCounter, 0.012f, 0.012f, 2f);
            idleCounter += Time.deltaTime;
        }
        else if (!isSprinting)
        {
            if (isAiming) HeadBob(movementCounter, 0.012f, 0.015f, 6f);
            else HeadBob(movementCounter, 0.03f, 0.04f, 6f);
            movementCounter += Time.deltaTime * 3f;
        }
        else
        {
            HeadBob(movementCounter, 0.05f, 0.06f, 10f);
            movementCounter += Time.deltaTime * 5f;
        }

        if (interact)
        {
            Transform spawn = transform.Find("Cameras/NormalCamera");
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(spawn.position, spawn.forward, out hit, 3f, exit))
            {
                dungeonController.LoadNextFloor();
                gameObject.transform.position = new Vector3(0, 1.3f, 0);
            }
            else if (Physics.Raycast(spawn.position, spawn.forward, out hit, 3f, crate))
            {
                if (!hit.collider.GetComponent<WeaponCrate>().isEmpty)
                {
                    weapon.NewWeapon(hit.collider.GetComponent<WeaponCrate>().generatedGun);
                    hit.collider.GetComponent<WeaponCrate>().isEmpty = true;
                }
            }
        }
        if (heal && healthPackNo > 0)
        {
            GetHealed(20);
            healthPackNo -= 1;
            RefreshHealthPack();
        }
        if (inventory)
        {
            weaponUI.ShowGunWindows();
        }
        else
        {
            weaponUI.HideGunWindows();
        }
    }
	void FixedUpdate()
    {
        //Axis
        float hmove = Input.GetAxisRaw("Horizontal");
        float vmove = Input.GetAxisRaw("Vertical");

        //Controls
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);

        //States
        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jump && isGrounded;
        bool isSprinting = sprint && vmove > 0 && !isJumping && isGrounded;

        //Movement
        Vector3 direction = new Vector3(hmove, 0, vmove);
        direction.Normalize();

        float adjustedSpeed = speed;
        if (isSprinting) adjustedSpeed *= sprintModifier;

        Vector3 targetVelocity = transform.TransformDirection(direction) * adjustedSpeed * Time.fixedDeltaTime;
        targetVelocity.y = rig.velocity.y;
        rig.velocity = targetVelocity;

        //Field of View
        if (isSprinting) normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFov * sprintFovModifier, Time.deltaTime * 8f);
        else normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFov, Time.deltaTime * 8f);
    }
    #endregion
    #region Private Methods

    void HeadBob(float z, float x_intensity, float y_intensity, float smoothing)
    {
        Vector3 targetPosition = weaponParentOrigin + new Vector3(Mathf.Cos(z) * x_intensity, Mathf.Sin(z * 2) * y_intensity, 0);
        weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetPosition, Time.deltaTime * smoothing);
    }

    #endregion

    #region Public Methods
    //maybe change TakeDamage to just changeHealth
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        RefreshHealthbar();

        if (currentHealth <= 0) PlayerDeath();

    }

    public void GetHealed(int heal)
    {
        if (currentHealth + heal < maxHealth)
        {
            currentHealth += heal;
        }
        else
        {
            currentHealth = maxHealth;
        }
        RefreshHealthbar();
    }

	private void PlayerDeath()
	{
        FindObjectOfType<GameManager>().GameOver();
	}
    
    private void RefreshHealthbar()
    {
        float healthRatio = (float)currentHealth / (float)maxHealth;
        healthbar.localScale = new Vector3(healthRatio, 1, 1);
		healthValue.text = currentHealth.ToString();
    }

    public void RefreshHealthPack()
    {
        healthPackNoText.text = healthPackNo.ToString();
    }
    
	#endregion
}

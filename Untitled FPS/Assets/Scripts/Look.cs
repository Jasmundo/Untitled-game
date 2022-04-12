﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Translates mouse movement to in-game camera movement.
/// </summary>
public class Look : MonoBehaviour
{
	#region Variables

	public static bool cursorLocked;

    public Transform player;
    public Transform cams;
    public Transform weapon;

    public float xSensitivity;
    public float ySensitivity;
    public float maxAngle;

    private Quaternion camCenter;

	#endregion
	#region Monobehaviour Callbacks
	void Start()
    {
        camCenter = cams.localRotation;
        cursorLocked = true;
    }

    void Update()
    {
        SetY();
        SetX();
        UpdateCursorLock();
    }
	#endregion
	#region Private Methods
	void SetY()
    {
        float input = Input.GetAxis("Mouse Y") * ySensitivity * Time.fixedDeltaTime;
        Quaternion adj = Quaternion.AngleAxis(input, -Vector3.right);
        Quaternion delta = cams.localRotation * adj;
        if (Quaternion.Angle(camCenter, delta)<maxAngle)
        {
            cams.localRotation = delta;
        }
        weapon.localRotation = cams.localRotation;
    }
    void SetX()
    {
        float input = Input.GetAxis("Mouse X") * xSensitivity * Time.fixedDeltaTime;
        Quaternion adj = Quaternion.AngleAxis(input, Vector3.up);
        Quaternion delta = player.localRotation * adj;
        player.localRotation = delta;
    }
    void UpdateCursorLock()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = true;
            }
        }
    }
	#endregion
}

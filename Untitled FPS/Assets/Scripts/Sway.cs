using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    #region Variables

    public float swayIntensity;
    public float swaySmoothing;

	private Quaternion originRotation;

	#endregion
	#region Monobehaviour callbacks
	private void Start()
	{
		originRotation = transform.localRotation;
	}

	private void Update()
	{
		UpdateSway();
	}

	#endregion
	#region Private Methods

	private void UpdateSway()
	{
		float x_mouse = Input.GetAxis("Mouse X");
		float y_mouse = Input.GetAxis("Mouse Y");

		Quaternion xAdjustment = Quaternion.AngleAxis(-swayIntensity * x_mouse, Vector3.up);
		Quaternion yAdjustment = Quaternion.AngleAxis(swayIntensity * y_mouse, Vector3.right);
		Quaternion targetRotation = originRotation * xAdjustment * yAdjustment;

		transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * swaySmoothing);
	}

	#endregion
}

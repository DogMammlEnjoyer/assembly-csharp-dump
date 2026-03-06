using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DroneFakeCameraMimicer : MonoBehaviour
{
	private void Awake()
	{
		this._mimicer = base.GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		if (this._target == null)
		{
			Debug.LogWarning("No target assigned to DroneFakeCameraMimicer");
			return;
		}
		this._mimicer.fieldOfView = this._target.fieldOfView;
	}

	[SerializeField]
	private Camera _target;

	private Camera _mimicer;
}

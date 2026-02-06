using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MetroSpotlight : MonoBehaviour
{
	public void Tick()
	{
		if (!this._light)
		{
			return;
		}
		if (!this._target)
		{
			return;
		}
		this._time += this.speed * Time.deltaTime * Time.deltaTime;
		Vector3 position = this._target.position;
		Vector3 normalized = (position - this._light.position).normalized;
		Vector3 vector = Vector3.Cross(normalized, this._blimp.forward);
		Vector3 yDir = Vector3.Cross(normalized, vector);
		Vector3 worldPosition = MetroSpotlight.Figure8(position, vector, yDir, this._radius, this._time, this._offset, this._theta);
		this._light.LookAt(worldPosition);
	}

	private static Vector3 Figure8(Vector3 origin, Vector3 xDir, Vector3 yDir, float scale, float t, float offset, float theta)
	{
		float num = 2f / (3f - Mathf.Cos(2f * (t + offset)));
		float d = scale * num * Mathf.Cos(t + offset);
		float d2 = scale * num * Mathf.Sin(2f * (t + offset)) / 2f;
		Vector3 axis = Vector3.Cross(xDir, yDir);
		Quaternion rotation = Quaternion.AngleAxis(theta, axis);
		xDir = rotation * xDir;
		yDir = rotation * yDir;
		Vector3 b = xDir * d + yDir * d2;
		return origin + b;
	}

	[SerializeField]
	private Transform _blimp;

	[SerializeField]
	private Transform _light;

	[SerializeField]
	private Transform _target;

	[FormerlySerializedAs("_scale")]
	[SerializeField]
	private float _radius = 1f;

	[SerializeField]
	private float _offset;

	[SerializeField]
	private float _theta;

	public float speed = 16f;

	[Space]
	private float _time;
}

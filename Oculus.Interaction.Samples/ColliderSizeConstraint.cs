using System;
using UnityEngine;

public class ColliderSizeConstraint : MonoBehaviour
{
	private void Update()
	{
		float num = Vector3.Distance(this.pointA.position, this.pointB.position);
		num -= this.wideSideOffset;
		Vector3 lossyScale = base.transform.parent.lossyScale;
		Vector3 vector = this.size;
		vector[this.expandingAxis] = num;
		Vector3 localScale = new Vector3(vector.x / lossyScale.x, vector.y / lossyScale.y, vector.z / lossyScale.z);
		base.transform.localScale = localScale;
	}

	public Vector3 size;

	public int expandingAxis;

	public Transform pointA;

	public Transform pointB;

	public float wideSideOffset;
}

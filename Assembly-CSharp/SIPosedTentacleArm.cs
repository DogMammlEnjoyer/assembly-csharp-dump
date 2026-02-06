using System;
using GorillaExtensions;
using UnityEngine;

[ExecuteAlways]
public class SIPosedTentacleArm : MonoBehaviour
{
	public void ConfigureFrom(SIGadgetTentacleArm source, MeshRenderer rend1, MeshRenderer rend2, Transform anchor1, Transform anchor2)
	{
		this.LengthFactor = source.LengthFactor;
		this.tentacleRenderer = rend1;
		this.tentacleRenderer2 = rend2;
		this.tentacleAnchor = anchor1;
		this.tentacleAnchor2 = anchor2;
		this.tentacleSharedMaterial = rend1.sharedMaterial;
	}

	private void Start()
	{
		this.UpdateTentaclePose();
	}

	private bool CanUpdateTentaclePose()
	{
		return true;
	}

	private void EnsureMaterialsInitialized()
	{
		if (this._initialized)
		{
			return;
		}
		this._tentacleMat = new Material(this.tentacleSharedMaterial);
		this.tentacleRenderer.material = this._tentacleMat;
		this._hasTentacle2 = this.tentacleRenderer2;
		if (this._hasTentacle2)
		{
			this._tentacleMat2 = new Material(this.tentacleSharedMaterial);
			this.tentacleRenderer2.material = this._tentacleMat2;
		}
		this._initialized = true;
	}

	private void UpdateTentaclePose()
	{
		if (!this.CanUpdateTentaclePose())
		{
			return;
		}
		this.EnsureMaterialsInitialized();
		this.UpdateTentacle(this._tentacleMat, this.tentacleRenderer.transform, this.tentacleAnchor);
		if (this._hasTentacle2)
		{
			this.UpdateTentacle(this._tentacleMat2, this.tentacleRenderer2.transform, this.tentacleAnchor2);
		}
	}

	private void UpdateTentacle(Material material, Transform tentacle, Transform anchor)
	{
		Vector3 vector = Vector3.forward * this.LengthFactor;
		material.SetVector(this.tentacleStartDir_HASH, vector);
		Vector3 vector2 = tentacle.InverseTransformPoint(anchor.position);
		material.SetVector(this.tentacleEnd_HASH, vector2);
		Vector3 vector3 = -tentacle.InverseTransformDirection(anchor.forward) * this.LengthFactor;
		material.SetVector(this.tentacleEndDir_HASH, vector3);
		Vector3 vector4 = SIGadgetTentacleArm.SplineSample(0.25f, vector, vector2, vector3);
		Vector3 a = SIGadgetTentacleArm.SplineSample(0.26f, vector, vector2, vector3);
		Vector3 vector5 = SIGadgetTentacleArm.SplineSample(0.75f, vector, vector2, vector3);
		Vector3 a2 = SIGadgetTentacleArm.SplineSample(0.76f, vector, vector2, vector3);
		Vector3 planeIntersection = SIGadgetTentacleArm.GetPlaneIntersection(vector4, (a - vector4).normalized, vector5, (a2 - vector5).normalized, Quaternion.AngleAxis(90f, Vector3.forward) * vector2.WithZ(0f).normalized);
		material.SetVector(this.tentacleRingOrigin_HASH, planeIntersection);
	}

	public float LengthFactor = 1.5f;

	public MeshRenderer tentacleRenderer;

	public MeshRenderer tentacleRenderer2;

	public Transform tentacleAnchor;

	public Transform tentacleAnchor2;

	public Material tentacleSharedMaterial;

	private bool _initialized;

	private bool _hasTentacle2;

	private Material _tentacleMat;

	private Material _tentacleMat2;

	private Vector3 _lastPos;

	private Vector3 _lastAnchorPos;

	private ShaderHashId tentacleStartDir_HASH = "_TentacleStartDir";

	private ShaderHashId tentacleEnd_HASH = "_TentacleEndPos";

	private ShaderHashId tentacleEndDir_HASH = "_TentacleEndDir";

	private ShaderHashId tentacleRingOrigin_HASH = "_TentacleRingOrigin";
}

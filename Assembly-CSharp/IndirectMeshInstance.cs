using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public sealed class IndirectMeshInstance : MonoBehaviour
{
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshFilter = base.GetComponent<MeshFilter>();
	}

	private void OnEnable()
	{
		if (this._registered)
		{
			return;
		}
		this._registered = true;
		IndirectMeshGroup componentInParent = base.GetComponentInParent<IndirectMeshGroup>();
		IndirectMeshRenderer.Register(this, (componentInParent != null) ? componentInParent.GetInstanceID() : 0);
		if (this.dynamic)
		{
			this.meshRenderer.enabled = false;
			return;
		}
		Object.Destroy(base.gameObject);
	}

	[Tooltip("When true, the transform is tracked and updated each frame instead of baked at registration time.")]
	[SerializeField]
	internal bool dynamic;

	internal MeshRenderer meshRenderer;

	internal MeshFilter meshFilter;

	private bool _registered;
}

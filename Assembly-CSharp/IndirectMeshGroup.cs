using System;
using UnityEngine;

public sealed class IndirectMeshGroup : MonoBehaviour
{
	private void OnEnable()
	{
		IndirectMeshRenderer.SetGroupVisible(base.GetInstanceID(), true);
	}

	private void OnDisable()
	{
		IndirectMeshRenderer.SetGroupVisible(base.GetInstanceID(), false);
	}
}

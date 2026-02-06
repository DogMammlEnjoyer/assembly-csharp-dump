using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NexusGroupId", menuName = "Nexus/NexusGroupId")]
public class NexusGroupId : ScriptableObject
{
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	[SerializeField]
	private string code;

	[SerializeField]
	private string sandboxCode;
}

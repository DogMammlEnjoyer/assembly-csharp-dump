using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NexusCreatorCode", menuName = "Nexus/NexusCreatorCode")]
public class NexusCreatorCode : ScriptableObject
{
	public string Code
	{
		get
		{
			return this.code;
		}
	}

	public NexusGroupId GroupId
	{
		get
		{
			return this.groupId;
		}
	}

	[SerializeField]
	private string code;

	[SerializeField]
	private NexusGroupId groupId;
}

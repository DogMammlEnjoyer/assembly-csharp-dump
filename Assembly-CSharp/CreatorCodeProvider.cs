using System;
using Cosmetics;
using UnityEngine;

public class CreatorCodeProvider : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
{
	string ICreatorCodeProvider.TerminalId
	{
		get
		{
			return this.nexusCreatorCode.GroupId.Code + this.nexusCreatorCode.Code;
		}
	}

	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusCreatorCode == null)
		{
			Debug.LogError("The CreatorCodeProvider component on " + base.name + " must be assigned a nexusCreatorCode.");
			return false;
		}
		return true;
	}

	void ICreatorCodeProvider.GetCreatorCode(out string code, out NexusGroupId[] groups)
	{
		code = this.nexusCreatorCode.Code;
		groups = new NexusGroupId[]
		{
			this.nexusCreatorCode.GroupId
		};
	}

	GameObject ICreatorCodeProvider.GameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	[SerializeField]
	private NexusCreatorCode nexusCreatorCode;
}

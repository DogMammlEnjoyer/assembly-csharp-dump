using System;
using UnityEngine;

namespace Oculus.Assistant.VoiceCommand.Configuration
{
	[CreateAssetMenu(fileName = "Action-VoiceCommandActionName", menuName = "Voice SDK/Voice Command Action")]
	public class VoiceCommand : ScriptableObject
	{
		public virtual byte[] InputData { get; }

		public override string ToString()
		{
			return base.name;
		}

		public string actionId;
	}
}

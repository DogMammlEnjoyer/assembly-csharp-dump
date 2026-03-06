using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX.Utility;

[Serializable]
internal class VisualEffectActivationBehaviour : PlayableBehaviour
{
	[SerializeField]
	public ExposedProperty onClipEnter = "OnPlay";

	[SerializeField]
	public ExposedProperty onClipExit = "OnStop";

	[SerializeField]
	public VisualEffectActivationBehaviour.EventState[] clipEnterEventAttributes;

	[SerializeField]
	public VisualEffectActivationBehaviour.EventState[] clipExitEventAttributes;

	[Serializable]
	public enum AttributeType
	{
		Float = 1,
		Float2,
		Float3,
		Float4,
		Int32,
		Uint32,
		Boolean = 17
	}

	[Serializable]
	public struct EventState
	{
		public ExposedProperty attribute;

		public VisualEffectActivationBehaviour.AttributeType type;

		public float[] values;
	}
}

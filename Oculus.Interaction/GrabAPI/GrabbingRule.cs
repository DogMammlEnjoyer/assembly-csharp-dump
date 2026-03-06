using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	[Serializable]
	public struct GrabbingRule
	{
		public FingerUnselectMode UnselectMode
		{
			get
			{
				return this._unselectMode;
			}
		}

		public bool SelectsWithOptionals
		{
			get
			{
				return this._thumbRequirement != FingerRequirement.Required && this._indexRequirement != FingerRequirement.Required && this._middleRequirement != FingerRequirement.Required && this._ringRequirement != FingerRequirement.Required && this._pinkyRequirement != FingerRequirement.Required;
			}
		}

		public FingerRequirement this[HandFinger fingerID]
		{
			get
			{
				switch (fingerID)
				{
				case HandFinger.Thumb:
					return this._thumbRequirement;
				case HandFinger.Index:
					return this._indexRequirement;
				case HandFinger.Middle:
					return this._middleRequirement;
				case HandFinger.Ring:
					return this._ringRequirement;
				case HandFinger.Pinky:
					return this._pinkyRequirement;
				default:
					return FingerRequirement.Ignored;
				}
			}
			set
			{
				switch (fingerID)
				{
				case HandFinger.Thumb:
					this._thumbRequirement = value;
					return;
				case HandFinger.Index:
					this._indexRequirement = value;
					return;
				case HandFinger.Middle:
					this._middleRequirement = value;
					return;
				case HandFinger.Ring:
					this._ringRequirement = value;
					return;
				case HandFinger.Pinky:
					this._pinkyRequirement = value;
					return;
				default:
					return;
				}
			}
		}

		public void StripIrrelevant(ref HandFingerFlags fingerFlags)
		{
			for (int i = 0; i < 5; i++)
			{
				HandFinger fingerID = (HandFinger)i;
				if (this[fingerID] == FingerRequirement.Ignored)
				{
					fingerFlags &= (HandFingerFlags)(~(HandFingerFlags)(1 << i));
				}
			}
		}

		public GrabbingRule(HandFingerFlags mask, in GrabbingRule otherRule)
		{
			this._thumbRequirement = (((mask & HandFingerFlags.Thumb) != HandFingerFlags.None) ? otherRule._thumbRequirement : FingerRequirement.Ignored);
			this._indexRequirement = (((mask & HandFingerFlags.Index) != HandFingerFlags.None) ? otherRule._indexRequirement : FingerRequirement.Ignored);
			this._middleRequirement = (((mask & HandFingerFlags.Middle) != HandFingerFlags.None) ? otherRule._middleRequirement : FingerRequirement.Ignored);
			this._ringRequirement = (((mask & HandFingerFlags.Ring) != HandFingerFlags.None) ? otherRule._ringRequirement : FingerRequirement.Ignored);
			this._pinkyRequirement = (((mask & HandFingerFlags.Pinky) != HandFingerFlags.None) ? otherRule._pinkyRequirement : FingerRequirement.Ignored);
			GrabbingRule grabbingRule = otherRule;
			this._unselectMode = grabbingRule.UnselectMode;
		}

		public static GrabbingRule DefaultPalmRule { get; } = new GrabbingRule
		{
			_thumbRequirement = FingerRequirement.Optional,
			_indexRequirement = FingerRequirement.Required,
			_middleRequirement = FingerRequirement.Required,
			_ringRequirement = FingerRequirement.Required,
			_pinkyRequirement = FingerRequirement.Optional,
			_unselectMode = FingerUnselectMode.AllReleased
		};

		public static GrabbingRule DefaultPinchRule { get; } = new GrabbingRule
		{
			_thumbRequirement = FingerRequirement.Optional,
			_indexRequirement = FingerRequirement.Optional,
			_middleRequirement = FingerRequirement.Optional,
			_ringRequirement = FingerRequirement.Ignored,
			_pinkyRequirement = FingerRequirement.Ignored,
			_unselectMode = FingerUnselectMode.AllReleased
		};

		public static GrabbingRule FullGrab { get; } = new GrabbingRule
		{
			_thumbRequirement = FingerRequirement.Required,
			_indexRequirement = FingerRequirement.Required,
			_middleRequirement = FingerRequirement.Required,
			_ringRequirement = FingerRequirement.Required,
			_pinkyRequirement = FingerRequirement.Required,
			_unselectMode = FingerUnselectMode.AllReleased
		};

		[SerializeField]
		private FingerRequirement _thumbRequirement;

		[SerializeField]
		private FingerRequirement _indexRequirement;

		[SerializeField]
		private FingerRequirement _middleRequirement;

		[SerializeField]
		private FingerRequirement _ringRequirement;

		[SerializeField]
		private FingerRequirement _pinkyRequirement;

		[SerializeField]
		private FingerUnselectMode _unselectMode;
	}
}

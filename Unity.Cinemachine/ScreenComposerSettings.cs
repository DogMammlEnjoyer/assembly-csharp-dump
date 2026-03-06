using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct ScreenComposerSettings
	{
		public void Validate()
		{
			this.ScreenPosition.x = Mathf.Clamp(this.ScreenPosition.x, -1.5f, 1.5f);
			this.ScreenPosition.y = Mathf.Clamp(this.ScreenPosition.y, -1.5f, 1.5f);
			this.DeadZone.Size.x = Mathf.Clamp(this.DeadZone.Size.x, 0f, 2f);
			this.DeadZone.Size.y = Mathf.Clamp(this.DeadZone.Size.y, 0f, 2f);
			this.HardLimits.Size = new Vector2(Mathf.Clamp(this.HardLimits.Size.x, this.DeadZone.Size.x, 3f), Mathf.Clamp(this.HardLimits.Size.y, this.DeadZone.Size.y, 3f));
			this.HardLimits.Offset.x = Mathf.Clamp(this.HardLimits.Offset.x, -1f, 1f);
			this.HardLimits.Offset.y = Mathf.Clamp(this.HardLimits.Offset.y, -1f, 1f);
		}

		public Vector2 EffectiveDeadZoneSize
		{
			get
			{
				if (!this.DeadZone.Enabled)
				{
					return Vector2.zero;
				}
				return this.DeadZone.Size;
			}
		}

		public Vector2 EffectiveHardLimitSize
		{
			get
			{
				if (!this.HardLimits.Enabled)
				{
					return new Vector2(3f, 3f);
				}
				return this.HardLimits.Size;
			}
		}

		public Rect DeadZoneRect
		{
			get
			{
				Vector2 effectiveDeadZoneSize = this.EffectiveDeadZoneSize;
				return new Rect(this.ScreenPosition - effectiveDeadZoneSize * 0.5f + new Vector2(0.5f, 0.5f), effectiveDeadZoneSize);
			}
			set
			{
				Vector2 effectiveDeadZoneSize = this.EffectiveDeadZoneSize;
				if (this.DeadZone.Enabled)
				{
					effectiveDeadZoneSize = new Vector2(Mathf.Clamp(value.width, 0f, 2f), Mathf.Clamp(value.height, 0f, 2f));
					this.DeadZone.Size = effectiveDeadZoneSize;
				}
				this.ScreenPosition = new Vector2(Mathf.Clamp(value.x - 0.5f + effectiveDeadZoneSize.x * 0.5f, -1.5f, 1.5f), Mathf.Clamp(value.y - 0.5f + effectiveDeadZoneSize.y * 0.5f, -1.5f, 1.5f));
				this.HardLimits.Size = new Vector2(Mathf.Clamp(this.HardLimits.Size.x, effectiveDeadZoneSize.x, 3f), Mathf.Clamp(this.HardLimits.Size.y, effectiveDeadZoneSize.y, 3f));
			}
		}

		public Rect HardLimitsRect
		{
			get
			{
				if (!this.HardLimits.Enabled)
				{
					return new Rect(-this.EffectiveHardLimitSize * 0.5f, this.EffectiveHardLimitSize);
				}
				Rect result = new Rect(this.ScreenPosition - this.HardLimits.Size * 0.5f + new Vector2(0.5f, 0.5f), this.HardLimits.Size);
				Vector2 effectiveDeadZoneSize = this.EffectiveDeadZoneSize;
				result.position += new Vector2(this.HardLimits.Offset.x * 0.5f * (this.HardLimits.Size.x - effectiveDeadZoneSize.x), this.HardLimits.Offset.y * 0.5f * (this.HardLimits.Size.y - effectiveDeadZoneSize.y));
				return result;
			}
			set
			{
				this.HardLimits.Size.x = Mathf.Clamp(value.width, 0f, 6f);
				this.HardLimits.Size.y = Mathf.Clamp(value.height, 0f, 6f);
				this.DeadZone.Size.x = Mathf.Min(this.DeadZone.Size.x, this.HardLimits.Size.x);
				this.DeadZone.Size.y = Mathf.Min(this.DeadZone.Size.y, this.HardLimits.Size.y);
			}
		}

		public static ScreenComposerSettings Lerp(in ScreenComposerSettings a, in ScreenComposerSettings b, float t)
		{
			ScreenComposerSettings result = default(ScreenComposerSettings);
			result.ScreenPosition = Vector2.Lerp(a.ScreenPosition, b.ScreenPosition, t);
			ScreenComposerSettings.DeadZoneSettings deadZone = default(ScreenComposerSettings.DeadZoneSettings);
			deadZone.Enabled = (a.DeadZone.Enabled || b.DeadZone.Enabled);
			ScreenComposerSettings screenComposerSettings = a;
			Vector2 effectiveDeadZoneSize = screenComposerSettings.EffectiveDeadZoneSize;
			screenComposerSettings = b;
			deadZone.Size = Vector2.Lerp(effectiveDeadZoneSize, screenComposerSettings.EffectiveDeadZoneSize, t);
			result.DeadZone = deadZone;
			ScreenComposerSettings.HardLimitSettings hardLimits = default(ScreenComposerSettings.HardLimitSettings);
			hardLimits.Enabled = (a.HardLimits.Enabled || b.HardLimits.Enabled);
			screenComposerSettings = a;
			Vector2 effectiveHardLimitSize = screenComposerSettings.EffectiveHardLimitSize;
			screenComposerSettings = b;
			hardLimits.Size = Vector2.Lerp(effectiveHardLimitSize, screenComposerSettings.EffectiveHardLimitSize, t);
			hardLimits.Offset = Vector2.Lerp(a.HardLimits.Offset, b.HardLimits.Offset, t);
			result.HardLimits = hardLimits;
			return result;
		}

		public static bool Approximately(in ScreenComposerSettings a, in ScreenComposerSettings b)
		{
			if (Mathf.Approximately(a.ScreenPosition.x, b.ScreenPosition.x) && Mathf.Approximately(a.ScreenPosition.y, b.ScreenPosition.y))
			{
				ScreenComposerSettings screenComposerSettings = a;
				float x = screenComposerSettings.EffectiveDeadZoneSize.x;
				screenComposerSettings = b;
				if (Mathf.Approximately(x, screenComposerSettings.EffectiveDeadZoneSize.x))
				{
					screenComposerSettings = a;
					float y = screenComposerSettings.EffectiveDeadZoneSize.y;
					screenComposerSettings = b;
					if (Mathf.Approximately(y, screenComposerSettings.EffectiveDeadZoneSize.y))
					{
						screenComposerSettings = a;
						float x2 = screenComposerSettings.EffectiveHardLimitSize.x;
						screenComposerSettings = b;
						if (Mathf.Approximately(x2, screenComposerSettings.EffectiveHardLimitSize.x))
						{
							screenComposerSettings = a;
							float y2 = screenComposerSettings.EffectiveHardLimitSize.y;
							screenComposerSettings = b;
							if (Mathf.Approximately(y2, screenComposerSettings.EffectiveHardLimitSize.y) && Mathf.Approximately(a.HardLimits.Offset.x, b.HardLimits.Offset.x))
							{
								return Mathf.Approximately(a.HardLimits.Offset.y, b.HardLimits.Offset.y);
							}
						}
					}
				}
			}
			return false;
		}

		public static ScreenComposerSettings Default
		{
			get
			{
				return new ScreenComposerSettings
				{
					DeadZone = new ScreenComposerSettings.DeadZoneSettings
					{
						Enabled = false,
						Size = new Vector2(0.2f, 0.2f)
					},
					HardLimits = new ScreenComposerSettings.HardLimitSettings
					{
						Enabled = false,
						Size = new Vector2(0.8f, 0.8f)
					}
				};
			}
		}

		[Tooltip("Screen position for target. The camera will adjust to position the tracked object here.  0 is screen center, and +0.5 or -0.5 is screen edge")]
		[DelayedVector]
		public Vector2 ScreenPosition;

		[Tooltip("The camera will not adjust if the target is within this range of the screen position")]
		[FoldoutWithEnabledButton("Enabled")]
		public ScreenComposerSettings.DeadZoneSettings DeadZone;

		[Tooltip("The target will not be allowed to be outside this region. When the target is within this region, the camera will gradually adjust to re-align towards the desired position, depending on the damping speed")]
		[FoldoutWithEnabledButton("Enabled")]
		public ScreenComposerSettings.HardLimitSettings HardLimits;

		[Serializable]
		public struct DeadZoneSettings
		{
			public bool Enabled;

			[Tooltip("The camera will not adjust if the target is within this range of the screen position.  Full screen size is 1.")]
			[DelayedVector]
			public Vector2 Size;
		}

		[Serializable]
		public struct HardLimitSettings
		{
			public bool Enabled;

			[Tooltip("The target will not be allowed to be outside this region. When the target is within this region, the camera will gradually adjust to re-align towards the desired position, depending on the damping speed.  Full screen size is 1")]
			[DelayedVector]
			public Vector2 Size;

			[Tooltip("A zero Offset means that the hard limits will be centered around the target screen position.  A nonzero Offset will uncenter the hard limits relative to the target screen position.")]
			[DelayedVector]
			public Vector2 Offset;
		}
	}
}

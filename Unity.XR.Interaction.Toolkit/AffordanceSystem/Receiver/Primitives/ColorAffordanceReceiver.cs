using System;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives
{
	[AddComponentMenu("Affordance System/Receiver/Primitives/Color Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives.ColorAffordanceReceiver.html")]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class ColorAffordanceReceiver : BaseAsyncAffordanceStateReceiver<Color>
	{
		public ColorAffordanceThemeDatumProperty affordanceThemeDatum
		{
			get
			{
				return this.m_AffordanceThemeDatum;
			}
			set
			{
				this.m_AffordanceThemeDatum = value;
			}
		}

		public ColorUnityEvent valueUpdated
		{
			get
			{
				return this.m_ValueUpdated;
			}
			set
			{
				this.m_ValueUpdated = value;
			}
		}

		protected override BaseAffordanceTheme<Color> defaultAffordanceTheme
		{
			get
			{
				if (this.m_AffordanceThemeDatum == null)
				{
					return null;
				}
				return this.m_AffordanceThemeDatum.Value;
			}
		}

		protected override BindableVariable<Color> affordanceValue { get; } = new BindableVariable<Color>(default(Color), true, null, false);

		protected override JobHandle ScheduleTweenJob(ref TweenJobData<Color> jobData)
		{
			ColorAffordanceTheme colorAffordanceTheme = (ColorAffordanceTheme)base.affordanceTheme;
			return new ColorTweenJob
			{
				jobData = jobData,
				colorBlendAmount = colorAffordanceTheme.blendAmount,
				colorBlendMode = (byte)colorAffordanceTheme.colorBlendMode
			}.Schedule(default(JobHandle));
		}

		protected override BaseAffordanceTheme<Color> GenerateNewAffordanceThemeInstance()
		{
			return new ColorAffordanceTheme();
		}

		protected override void OnAffordanceValueUpdated(Color newValue)
		{
			ColorUnityEvent valueUpdated = this.m_ValueUpdated;
			if (valueUpdated == null)
			{
				return;
			}
			valueUpdated.Invoke(newValue);
		}

		[SerializeField]
		[Tooltip("Color Affordance Theme datum property used to map affordance state to a color affordance value. Can store an asset or a serialized value.")]
		private ColorAffordanceThemeDatumProperty m_AffordanceThemeDatum;

		[SerializeField]
		[Tooltip("The event that is called when the current affordance value is updated.")]
		private ColorUnityEvent m_ValueUpdated;
	}
}

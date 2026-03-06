using System;
using System.Text;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Audio;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Audio
{
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Affordance System/Receiver/Audio/Audio Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Audio.AudioAffordanceReceiver.html")]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class AudioAffordanceReceiver : MonoBehaviour
	{
		public BaseAffordanceStateProvider affordanceStateProvider
		{
			get
			{
				return this.m_AffordanceStateProvider;
			}
			set
			{
				this.m_AffordanceStateProvider = value;
			}
		}

		public AudioAffordanceThemeDatumProperty affordanceThemeDatum
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

		public AudioSource audioSource
		{
			get
			{
				return this.m_AudioSource;
			}
			set
			{
				this.m_AudioSource = value;
			}
		}

		protected void OnValidate()
		{
			if (this.m_AudioSource == null)
			{
				this.m_AudioSource = base.GetComponent<AudioSource>();
			}
		}

		protected void Awake()
		{
			if (this.m_AudioSource == null)
			{
				this.m_AudioSource = base.GetComponent<AudioSource>();
			}
			if (this.m_AffordanceThemeDatum != null && this.m_AffordanceThemeDatum.Value != null)
			{
				this.m_AffordanceThemeDatum.Value.ValidateTheme();
				this.LogIfMissingAffordanceStates(this.m_AffordanceThemeDatum.Value);
			}
		}

		protected void OnEnable()
		{
			if (this.m_AffordanceStateProvider == null)
			{
				XRLoggingUtils.LogError(string.Format("Missing Affordance State Provider reference. Please set one on {0}.", this), this);
				base.enabled = false;
				return;
			}
			if (this.m_AffordanceThemeDatum == null || this.m_AffordanceThemeDatum.Value == null)
			{
				XRLoggingUtils.LogError(string.Format("Missing Audio Affordance Theme Datum on {0}.", this), this);
				base.enabled = false;
				return;
			}
			this.m_BindingsGroup.AddBinding(this.m_AffordanceStateProvider.currentAffordanceStateData.Subscribe(new Action<AffordanceStateData>(this.OnAffordanceStateUpdated)));
		}

		protected void OnDisable()
		{
			this.m_BindingsGroup.Clear();
		}

		private void LogIfMissingAffordanceStates(AudioAffordanceTheme theme)
		{
			if (theme.GetAffordanceThemeDataForIndex(AffordanceStateShortcuts.stateCount - 1) == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				for (byte b = 0; b < AffordanceStateShortcuts.stateCount; b += 1)
				{
					AudioAffordanceThemeData affordanceThemeDataForIndex = theme.GetAffordanceThemeDataForIndex(b);
					stringBuilder.Append(string.Format("Expected: {0} \"{1}\",\tActual: ", b, AffordanceStateShortcuts.GetNameForIndex(b)));
					stringBuilder.AppendLine((affordanceThemeDataForIndex != null) ? string.Format("{0} \"{1}\"", b, affordanceThemeDataForIndex.stateName) : "<b>(None)</b>");
					if (affordanceThemeDataForIndex != null)
					{
						num++;
					}
				}
				Debug.LogWarning("Affordance Theme on affordance receiver is missing a potential affordance state. Expected states:" + string.Format("\nExpected Count: {0}, Actual Count: {1}", AffordanceStateShortcuts.stateCount, num) + string.Format("\n{0}", stringBuilder), this);
			}
		}

		private void OnAffordanceStateUpdated(AffordanceStateData affordanceStateData)
		{
			byte stateIndex = affordanceStateData.stateIndex;
			if (stateIndex != this.m_LastAffordanceStateIndex)
			{
				bool flag = stateIndex == 5;
				bool flag2 = stateIndex == 2;
				bool flag3 = stateIndex == 4;
				bool flag4 = this.m_LastAffordanceStateIndex == 4;
				bool flag5 = this.m_LastAffordanceStateIndex == 5;
				bool flag6 = flag && flag4;
				bool flag7 = flag3 && flag5;
				bool flag8 = flag2 && flag4;
				bool flag9 = flag2 && flag4;
				if (!flag6 && !flag8)
				{
					AudioAffordanceTheme value = this.m_AffordanceThemeDatum.Value;
					AudioAffordanceThemeData audioAffordanceThemeData = (value != null) ? value.GetAffordanceThemeDataForIndex(this.m_LastAffordanceStateIndex) : null;
					if (audioAffordanceThemeData != null)
					{
						this.PlayAudioClip(audioAffordanceThemeData.stateExited);
					}
				}
				if (!flag7 && !flag9)
				{
					AudioAffordanceTheme value2 = this.m_AffordanceThemeDatum.Value;
					AudioAffordanceThemeData audioAffordanceThemeData2 = (value2 != null) ? value2.GetAffordanceThemeDataForIndex(stateIndex) : null;
					if (audioAffordanceThemeData2 != null)
					{
						this.PlayAudioClip(audioAffordanceThemeData2.stateEntered);
					}
					else
					{
						string nameForIndex = AffordanceStateShortcuts.GetNameForIndex(stateIndex);
						XRLoggingUtils.LogError(string.Format("Missing theme data for affordance state index {0} \"{1}\" with {2}.", stateIndex, nameForIndex, this), this);
					}
				}
				this.m_LastAffordanceStateIndex = stateIndex;
			}
		}

		private void PlayAudioClip(AudioClip clipToPlay)
		{
			if (clipToPlay == null)
			{
				return;
			}
			this.m_AudioSource.PlayOneShot(clipToPlay);
		}

		[SerializeField]
		[Tooltip("Affordance state provider component to subscribe to.")]
		private BaseAffordanceStateProvider m_AffordanceStateProvider;

		[SerializeField]
		[Tooltip("Audio Affordance Theme datum property used to map affordance state to a Audio affordance value. Can store an asset or a serialized value.")]
		private AudioAffordanceThemeDatumProperty m_AffordanceThemeDatum;

		[SerializeField]
		[Tooltip("Audio Source where the audio clip will be played.")]
		private AudioSource m_AudioSource;

		private readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

		private byte m_LastAffordanceStateIndex;
	}
}

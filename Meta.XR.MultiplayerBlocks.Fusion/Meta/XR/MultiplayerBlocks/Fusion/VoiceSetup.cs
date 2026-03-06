using System;
using System.Collections;
using System.Reflection;
using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using POpusCodec.Enums;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	public class VoiceSetup : MonoBehaviour
	{
		public GameObject Speaker { get; private set; }

		private void Awake()
		{
			CustomNetworkObjectProvider.RegisterCustomNetworkObject(100000U, delegate
			{
				GameObject gameObject = new GameObject("Voice");
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.bypassReverbZones = true;
				audioSource.spatialBlend = 1f;
				Recorder recorder = gameObject.AddComponent<Recorder>();
				recorder.StopRecordingWhenPaused = true;
				recorder.SamplingRate = SamplingRate.Sampling48000;
				gameObject.AddComponent<Speaker>();
				gameObject.AddComponent<LipSyncPhotonFix>();
				gameObject.AddComponent<MicAmplifier>().AmplificationFactor = 2f;
				gameObject.AddComponent<VoiceNetworkObject>();
				gameObject.AddComponent<NetworkTransform>();
				NetworkObject obj = gameObject.AddComponent<NetworkObject>();
				FieldInfo field = typeof(NetworkObject).GetField("ObjectInterest", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo field2 = typeof(NetworkObject).GetNestedType("ObjectInterestModes", BindingFlags.NonPublic).GetField("AreaOfInterest");
				if (field != null && field2 != null)
				{
					field.SetValue(obj, (int)field2.GetValue(null));
				}
				return gameObject;
			});
		}

		private void OnEnable()
		{
			FusionBBEvents.OnSceneLoadDone += this.OnLoaded;
		}

		private void OnDisable()
		{
			FusionBBEvents.OnSceneLoadDone -= this.OnLoaded;
		}

		private void OnLoaded(NetworkRunner networkRunner)
		{
			base.StartCoroutine(this.SpawnSpeaker(networkRunner));
		}

		private IEnumerator SpawnSpeaker(NetworkRunner networkRunner)
		{
			while (networkRunner == null)
			{
				yield return null;
			}
			NetworkObject networkObject = networkRunner.Spawn(NetworkPrefabId.FromRaw(100000U), new Vector3?(this.centerEyeAnchor.position), new Quaternion?(this.centerEyeAnchor.rotation), new PlayerRef?(networkRunner.LocalPlayer), null, (NetworkSpawnFlags)0);
			networkObject.transform.SetParent(this.centerEyeAnchor.transform);
			this.Speaker = networkObject.gameObject;
			yield break;
		}

		public Transform centerEyeAnchor;

		private const uint CustomSpeakerPrefabID = 100000U;
	}
}

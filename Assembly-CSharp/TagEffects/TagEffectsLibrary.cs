using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace TagEffects
{
	public class TagEffectsLibrary : MonoBehaviour
	{
		public static float FistBumpSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.fistBumpSpeedThreshold;
			}
		}

		public static float HighFiveSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.highFiveSpeedThreshold;
			}
		}

		public static bool DebugMode
		{
			get
			{
				return TagEffectsLibrary._instance.debugMode;
			}
		}

		private void Awake()
		{
			if (TagEffectsLibrary._instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TagEffectsLibrary._instance = this;
			this.tagEffectsPool = new Dictionary<string, Queue<GameObjectOnDisableDispatcher>>();
			this.tagEffectsComboLookUp = new Dictionary<TagEffectsCombo, TagEffectPack[]>();
		}

		public static void PlayEffect(Transform target, bool isLeftHand, float rigScale, TagEffectsLibrary.EffectType effectType, TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack, Quaternion rotation)
		{
			if (TagEffectsLibrary._instance == null)
			{
				return;
			}
			ModeTagEffect modeTagEffect = null;
			TagEffectPack tagEffectPack = null;
			GameModeType item = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
			for (int i = 0; i < TagEffectsLibrary._instance.defaultTagEffects.Length; i++)
			{
				if (TagEffectsLibrary._instance.defaultTagEffects[i] != null && TagEffectsLibrary._instance.defaultTagEffects[i].Modes.Contains(item))
				{
					modeTagEffect = TagEffectsLibrary._instance.defaultTagEffects[i];
					tagEffectPack = modeTagEffect.tagEffect;
					break;
				}
			}
			if (tagEffectPack == null)
			{
				return;
			}
			GameObject firstPerson = tagEffectPack.firstPerson;
			GameObject thirdPerson = tagEffectPack.thirdPerson;
			GameObject fistBump = tagEffectPack.fistBump;
			GameObject highFive = tagEffectPack.highFive;
			bool firstPersonParentEffect = tagEffectPack.firstPersonParentEffect;
			bool thirdPersonParentEffect = tagEffectPack.thirdPersonParentEffect;
			bool flag = tagEffectPack.fistBumpParentEffect;
			bool highFiveParentEffect = tagEffectPack.highFiveParentEffect;
			if (playerCosmeticTagEffectPack != null)
			{
				TagEffectPack tagEffectPack2 = TagEffectsLibrary.comboLookup(playerCosmeticTagEffectPack, otherPlayerCosmeticTagEffectPack);
				if (!modeTagEffect.blockFistBumpOverride && playerCosmeticTagEffectPack.fistBump != null)
				{
					fistBump = tagEffectPack2.fistBump;
					flag = tagEffectPack2.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockHiveFiveOverride && playerCosmeticTagEffectPack.highFive != null)
				{
					highFive = tagEffectPack2.highFive;
					highFiveParentEffect = tagEffectPack2.highFiveParentEffect;
				}
			}
			if (otherPlayerCosmeticTagEffectPack != null)
			{
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.firstPerson != null)
				{
					firstPerson = otherPlayerCosmeticTagEffectPack.firstPerson;
					firstPersonParentEffect = otherPlayerCosmeticTagEffectPack.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.thirdPerson != null)
				{
					thirdPerson = otherPlayerCosmeticTagEffectPack.thirdPerson;
					thirdPersonParentEffect = otherPlayerCosmeticTagEffectPack.thirdPersonParentEffect;
				}
			}
			switch (effectType)
			{
			case TagEffectsLibrary.EffectType.FIRST_PERSON:
				TagEffectsLibrary.placeEffects(firstPerson, target, firstPersonParentEffect ? 1f : rigScale, false, firstPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.THIRD_PERSON:
				TagEffectsLibrary.placeEffects(thirdPerson, target, thirdPersonParentEffect ? 1f : rigScale, false, thirdPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.HIGH_FIVE:
				TagEffectsLibrary.placeEffects(highFive, target, highFiveParentEffect ? 1f : rigScale, isLeftHand, highFiveParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.FIST_BUMP:
				TagEffectsLibrary.placeEffects(fistBump, target, flag ? 1f : rigScale, isLeftHand, flag, rotation);
				return;
			default:
				return;
			}
		}

		private static TagEffectPack comboLookup(TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack)
		{
			if (otherPlayerCosmeticTagEffectPack == null)
			{
				return playerCosmeticTagEffectPack;
			}
			TagEffectsCombo tagEffectsCombo = new TagEffectsCombo();
			tagEffectsCombo.inputA = playerCosmeticTagEffectPack;
			tagEffectsCombo.inputB = otherPlayerCosmeticTagEffectPack;
			TagEffectPack[] array;
			if (!TagEffectsLibrary._instance.tagEffectsComboLookUp.TryGetValue(tagEffectsCombo, out array))
			{
				return playerCosmeticTagEffectPack;
			}
			int num = 0;
			if (GorillaComputer.instance != null)
			{
				num = GorillaComputer.instance.GetServerTime().Second;
			}
			return array[num % array.Length];
		}

		public static void placeEffects(GameObject prefab, Transform target, float scale, bool flipZAxis, bool parentEffect, Quaternion rotation)
		{
			if (prefab == null)
			{
				return;
			}
			Queue<GameObjectOnDisableDispatcher> queue;
			if (!TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(prefab.name, out queue))
			{
				queue = new Queue<GameObjectOnDisableDispatcher>();
				TagEffectsLibrary._instance.tagEffectsPool.Add(prefab.name, queue);
			}
			if (queue.Count == 0 || (queue.Peek().gameObject.activeInHierarchy && queue.Count < 12))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(prefab, target.transform.position, rotation, parentEffect ? target : TagEffectsLibrary._instance.transform);
				gameObject.name = prefab.name;
				gameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
				GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher;
				if (!gameObject.TryGetComponent<GameObjectOnDisableDispatcher>(out gameObjectOnDisableDispatcher))
				{
					gameObjectOnDisableDispatcher = gameObject.AddComponent<GameObjectOnDisableDispatcher>();
				}
				gameObjectOnDisableDispatcher.OnDisabled += TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				gameObject.SetActive(true);
				queue.Enqueue(gameObjectOnDisableDispatcher);
				return;
			}
			GameObjectOnDisableDispatcher recycledGameObject = queue.Dequeue();
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.RecycleGameObject(recycledGameObject, target, scale, flipZAxis, parentEffect));
		}

		private static void NewGameObjectOnDisableDispatcher_OnDisabled(GameObjectOnDisableDispatcher goodd)
		{
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.ReclaimDisabled(goodd.transform));
		}

		private IEnumerator RecycleGameObject(GameObjectOnDisableDispatcher recycledGameObject, Transform target, float scale, bool flipZAxis, bool parentEffect)
		{
			if (recycledGameObject.gameObject.activeInHierarchy)
			{
				recycledGameObject.gameObject.SetActive(false);
				recycledGameObject.OnDisabled -= TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				yield return null;
			}
			recycledGameObject.transform.position = target.transform.position;
			recycledGameObject.transform.rotation = target.transform.rotation;
			recycledGameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
			recycledGameObject.transform.parent = (parentEffect ? target : TagEffectsLibrary._instance.transform);
			Queue<GameObjectOnDisableDispatcher> queue;
			if (TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(recycledGameObject.gameObject.name, out queue))
			{
				recycledGameObject.gameObject.SetActive(true);
				queue.Enqueue(recycledGameObject);
			}
			yield break;
		}

		private IEnumerator ReclaimDisabled(Transform transform)
		{
			yield return null;
			transform.parent = TagEffectsLibrary._instance.transform;
			yield break;
		}

		private const int OBJECT_QUEUE_LIMIT = 12;

		[OnEnterPlay_SetNull]
		private static TagEffectsLibrary _instance;

		[SerializeField]
		private float fistBumpSpeedThreshold = 1f;

		[SerializeField]
		private float highFiveSpeedThreshold = 1f;

		[SerializeField]
		private ModeTagEffect[] defaultTagEffects;

		[SerializeField]
		private TagEffectsComboResult[] tagEffectsCombos;

		[SerializeField]
		private bool debugMode;

		private Dictionary<string, Queue<GameObjectOnDisableDispatcher>> tagEffectsPool;

		private Dictionary<TagEffectsCombo, TagEffectPack[]> tagEffectsComboLookUp;

		public enum EffectType
		{
			FIRST_PERSON,
			THIRD_PERSON,
			HIGH_FIVE,
			FIST_BUMP
		}
	}
}

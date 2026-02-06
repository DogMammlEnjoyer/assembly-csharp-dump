using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
	[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networkmechanimanimator")]
	[DisallowMultipleComponent]
	[AddComponentMenu("Fusion/Network Mecanim Animator")]
	[NetworkBehaviourWeaved(-1)]
	public sealed class NetworkMecanimAnimator : NetworkBehaviour, IAfterAllTicks, IPublicFacingInterface
	{
		public override int? DynamicWordCount
		{
			get
			{
				this.EnsureInitialized();
				return new int?(this.TotalWords);
			}
		}

		public override void Spawned()
		{
			this.EnsureInitialized();
			bool flag = this.Animator == null;
			if (flag)
			{
				base.enabled = false;
			}
		}

		void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
		{
			this.EnsureInitialized();
			bool hasStateAuthority = base.Object.HasStateAuthority;
			if (hasStateAuthority)
			{
				this.CaptureAnimatorData();
			}
		}

		public override void Render()
		{
			this.EnsureInitialized();
			bool isProxy = base.Object.IsProxy;
			if (isProxy)
			{
				RenderSource applyTiming = this.ApplyTiming;
				bool flag = applyTiming == RenderSource.Latest && base.Runner.Tick > this._lastAppliedTick;
				if (flag)
				{
					this.ApplyAnimatorData(base.StateBuffer);
					this._lastAppliedTick = base.Runner.Tick;
				}
				else
				{
					NetworkBehaviourBuffer networkBehaviourBuffer;
					NetworkBehaviourBuffer networkBehaviourBuffer2;
					float num;
					bool flag2 = base.TryGetSnapshotsBuffers(out networkBehaviourBuffer, out networkBehaviourBuffer2, out num);
					if (flag2)
					{
						NetworkBehaviourBuffer buffer = (this.ApplyTiming == RenderSource.To) ? networkBehaviourBuffer2 : networkBehaviourBuffer;
						Tick tick = buffer.Tick;
						bool flag3 = tick > this._lastAppliedTick;
						if (flag3)
						{
							this.ApplyAnimatorData(buffer);
							this._lastAppliedTick = tick;
						}
					}
				}
			}
		}

		public void SetTrigger(int triggerHash, bool passThroughOnInputAuthority = false)
		{
			this.EnsureInitialized();
			bool hasStateAuthority = base.Object.HasStateAuthority;
			if (hasStateAuthority)
			{
				this._pendingTriggers.Add(triggerHash);
			}
			else
			{
				bool flag = passThroughOnInputAuthority && base.Object.HasInputAuthority;
				if (flag)
				{
					this.Animator.SetTrigger(triggerHash);
				}
			}
		}

		public void SetTrigger(string trigger, bool passThroughOnInputAuthority = false)
		{
			this.EnsureInitialized();
			bool hasStateAuthority = base.Object.HasStateAuthority;
			if (hasStateAuthority)
			{
				int item = Animator.StringToHash(trigger);
				this._pendingTriggers.Add(item);
			}
			else
			{
				bool flag = passThroughOnInputAuthority && base.Object.HasInputAuthority;
				if (flag)
				{
					this.Animator.SetTrigger(trigger);
				}
			}
		}

		private void CaptureAnimatorData()
		{
			int num = 0;
			this.CaptureParameters(ref num);
			bool flag = (this.SyncSettings & AnimatorSyncSettings.StateRoot) == AnimatorSyncSettings.StateRoot;
			if (flag)
			{
				this.CaptureStates(ref num);
			}
			bool flag2 = (this.SyncSettings & AnimatorSyncSettings.LayerWeights) == AnimatorSyncSettings.LayerWeights;
			if (flag2)
			{
				this.CaptureLayerWeights(ref num);
			}
		}

		private void ApplyAnimatorData(NetworkBehaviourBuffer buffer)
		{
			int num = 0;
			this.ApplyParameters(buffer, ref num);
			bool flag = (this.SyncSettings & AnimatorSyncSettings.StateRoot) == AnimatorSyncSettings.StateRoot;
			if (flag)
			{
				this.ApplyStates(buffer, ref num);
			}
			bool flag2 = (this.SyncSettings & AnimatorSyncSettings.LayerWeights) == AnimatorSyncSettings.LayerWeights;
			if (flag2)
			{
				this.ApplyLayerWeights(buffer, ref num);
			}
		}

		private unsafe void CaptureStates(ref int wordOffset)
		{
			for (int i = 0; i < this._animatorData.SyncedLayerCount; i++)
			{
				bool flag = this.Animator.IsInTransition(i);
				if (flag)
				{
					int num = wordOffset;
					wordOffset = num + 1;
					*base.ReinterpretState<int>(num) = 0;
					num = wordOffset;
					wordOffset = num + 1;
					*base.ReinterpretState<FloatCompressed>(num) = 0f;
				}
				else
				{
					AnimatorStateInfo currentAnimatorStateInfo = this.Animator.GetCurrentAnimatorStateInfo(i);
					int num2 = currentAnimatorStateInfo.fullPathHash;
					int num3 = Array.IndexOf<int>(this.StateHashes, num2);
					bool flag2 = num3 >= 0;
					if (flag2)
					{
						num2 = num3;
					}
					else
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn(base.name + ":" + base.GetType().Name + " cannot find hash in indexes. Inspect the component to refresh the controller hash lookup. Sending full hash instead of index as fallback.");
						}
					}
					int num = wordOffset;
					wordOffset = num + 1;
					*base.ReinterpretState<int>(num) = num2;
					num = wordOffset;
					wordOffset = num + 1;
					*base.ReinterpretState<FloatCompressed>(num) = currentAnimatorStateInfo.normalizedTime;
				}
			}
		}

		private void ApplyStates(NetworkBehaviourBuffer buffer, ref int wordOffset)
		{
			for (int i = 0; i < this._animatorData.SyncedLayerCount; i++)
			{
				int num = wordOffset;
				wordOffset = num + 1;
				int num2 = buffer.ReinterpretState<int>(num);
				num = wordOffset;
				wordOffset = num + 1;
				FloatCompressed q = buffer.ReinterpretState<FloatCompressed>(num);
				bool flag = num2 == 0;
				if (flag)
				{
					break;
				}
				bool flag2 = num2 > 0 && num2 < this.StateHashes.Length;
				if (flag2)
				{
					num2 = this.StateHashes[num2];
				}
				else
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn(base.name + ":" + base.GetType().Name + " cannot find hash in indexes. Inspect the component to refresh the controller hash lookup. Sending full hash instead of index as fallback.");
					}
				}
				this.Animator.Play(num2, i, q);
			}
		}

		private unsafe void CaptureParameters(ref int wordOffset)
		{
			bool flag = (this.SyncSettings & AnimatorSyncSettings.ParameterFloats) == AnimatorSyncSettings.ParameterFloats;
			bool flag2 = (this.SyncSettings & AnimatorSyncSettings.ParameterInts) == AnimatorSyncSettings.ParameterInts;
			bool flag3 = (this.SyncSettings & AnimatorSyncSettings.ParameterBools) == AnimatorSyncSettings.ParameterBools;
			bool flag4 = (this.SyncSettings & AnimatorSyncSettings.ParameterTriggers) == AnimatorSyncSettings.ParameterTriggers;
			bool flag5 = true;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			AnimatorControllerParameter[] parameters = this._animatorData.Parameters;
			int[] parameterHashes = this._animatorData.ParameterHashes;
			int i = 0;
			int parameterCount = this._animatorData.ParameterCount;
			while (i < parameterCount)
			{
				int num4 = parameterHashes[i];
				AnimatorControllerParameterType type = parameters[i].type;
				AnimatorControllerParameterType animatorControllerParameterType = type;
				switch (animatorControllerParameterType)
				{
				case AnimatorControllerParameterType.Float:
				{
					bool flag6 = flag;
					if (flag6)
					{
						float v = this.Animator.IsParameterControlledByCurve(num4) ? 0f : this.Animator.GetFloat(num4);
						int num5 = wordOffset;
						wordOffset = num5 + 1;
						*base.ReinterpretState<FloatCompressed>(num5) = v;
					}
					break;
				}
				case (AnimatorControllerParameterType)2:
					break;
				case AnimatorControllerParameterType.Int:
				{
					bool flag7 = flag2;
					if (flag7)
					{
						int num5 = wordOffset;
						wordOffset = num5 + 1;
						*base.ReinterpretState<int>(num5) = this.Animator.GetInteger(num4);
					}
					break;
				}
				case AnimatorControllerParameterType.Bool:
				{
					bool flag8 = flag3;
					if (flag8)
					{
						bool flag9 = flag5;
						if (flag9)
						{
							num3 = this._animatorData.ParamBoolsPtrOffset;
							flag5 = false;
						}
						int num6 = 4 * num;
						int num7 = 15 << num6;
						num3 &= ~num7;
						bool @bool = this.Animator.GetBool(num4);
						if (@bool)
						{
							num3 |= 1 << num6;
						}
						num++;
						bool flag10 = num == 8;
						if (flag10)
						{
							num = 0;
							*base.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2++) = num3;
							bool flag11 = num2 < this._animatorData.ParamBoolsWordCount;
							if (flag11)
							{
								num3 = *base.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2);
							}
						}
					}
					break;
				}
				default:
					if (animatorControllerParameterType == AnimatorControllerParameterType.Trigger)
					{
						bool flag12 = this._pendingTriggers.Contains(num4);
						bool flag13 = flag12;
						if (flag13)
						{
							this.Animator.SetTrigger(num4);
						}
						bool flag14 = flag4;
						if (flag14)
						{
							bool flag15 = flag5;
							if (flag15)
							{
								num3 = *base.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset);
								flag5 = false;
							}
							int num8 = 4 * num;
							int num9 = 15 << num8;
							int num10 = (num3 & num9) >> num8;
							int num11 = num10 >> 1;
							bool flag16 = (num10 & 1) != 0;
							bool flag17 = flag12 || flag16;
							if (flag17)
							{
								num10 = num11 + 1 << 1;
								bool flag18 = flag12;
								if (flag18)
								{
									num10 |= 1;
								}
								num10 <<= num8;
								num3 &= ~num9;
								num3 |= (num10 & num9);
							}
							num++;
							bool flag19 = num == 8;
							if (flag19)
							{
								num = 0;
								*base.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2++) = num3;
								bool flag20 = num2 < this._animatorData.ParamBoolsWordCount;
								if (flag20)
								{
									num3 = *base.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2);
								}
							}
						}
					}
					break;
				}
				i++;
			}
			bool flag21 = num > 0;
			if (flag21)
			{
				*base.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2) = num3;
			}
			wordOffset += this._animatorData.ParamBoolsWordCount;
			this._pendingTriggers.Clear();
		}

		private void ApplyParameters(NetworkBehaviourBuffer buffer, ref int wordOffset)
		{
			bool flag = (this.SyncSettings & AnimatorSyncSettings.ParameterFloats) == AnimatorSyncSettings.ParameterFloats;
			bool flag2 = (this.SyncSettings & AnimatorSyncSettings.ParameterInts) == AnimatorSyncSettings.ParameterInts;
			bool flag3 = (this.SyncSettings & AnimatorSyncSettings.ParameterBools) == AnimatorSyncSettings.ParameterBools;
			bool flag4 = (this.SyncSettings & AnimatorSyncSettings.ParameterTriggers) == AnimatorSyncSettings.ParameterTriggers;
			bool flag5 = true;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			AnimatorControllerParameter[] parameters = this._animatorData.Parameters;
			int[] parameterHashes = this._animatorData.ParameterHashes;
			int i = 0;
			int parameterCount = this._animatorData.ParameterCount;
			while (i < parameterCount)
			{
				int num5 = parameterHashes[i];
				AnimatorControllerParameterType type = parameters[i].type;
				AnimatorControllerParameterType animatorControllerParameterType = type;
				switch (animatorControllerParameterType)
				{
				case AnimatorControllerParameterType.Float:
				{
					bool flag6 = flag;
					if (flag6)
					{
						bool flag7 = this.Animator.IsParameterControlledByCurve(num5);
						if (flag7)
						{
							wordOffset++;
						}
						else
						{
							int num6 = wordOffset;
							wordOffset = num6 + 1;
							float value = buffer.ReinterpretState<FloatCompressed>(num6);
							this.Animator.SetFloat(num5, value);
						}
					}
					break;
				}
				case (AnimatorControllerParameterType)2:
					break;
				case AnimatorControllerParameterType.Int:
				{
					bool flag8 = flag2;
					if (flag8)
					{
						int num6 = wordOffset;
						wordOffset = num6 + 1;
						int value2 = buffer.ReinterpretState<int>(num6);
						this.Animator.SetInteger(num5, value2);
					}
					break;
				}
				case AnimatorControllerParameterType.Bool:
				{
					bool flag9 = flag3;
					if (flag9)
					{
						bool flag10 = flag5;
						if (flag10)
						{
							num3 = this._animatorData.PrevBoolsBitmask[0];
							num4 = buffer.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset);
							flag5 = false;
						}
						int num7 = 4 * num;
						bool value3 = (num4 & 1 << num7) != 0;
						this.Animator.SetBool(num5, value3);
						num++;
						bool flag11 = num == 8;
						if (flag11)
						{
							this._animatorData.PrevBoolsBitmask[num2] = num4;
							num = 0;
							num2++;
							num3 = this._animatorData.PrevBoolsBitmask[num2];
							bool flag12 = num2 < this._animatorData.ParamBoolsWordCount;
							if (flag12)
							{
								num3 = this._animatorData.PrevBoolsBitmask[num2];
								num4 = buffer.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2);
							}
						}
					}
					break;
				}
				default:
					if (animatorControllerParameterType == AnimatorControllerParameterType.Trigger)
					{
						bool flag13 = flag4;
						if (flag13)
						{
							bool flag14 = flag5;
							if (flag14)
							{
								num3 = this._animatorData.PrevBoolsBitmask[0];
								num4 = buffer.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset);
								flag5 = false;
							}
							int num8 = 4 * num;
							int num9 = 15 << num8;
							int num10 = (num3 & num9) >> num8;
							int num11 = (num4 & num9) >> num8;
							bool flag15 = num10 != num11;
							if (flag15)
							{
								bool flag16 = (num10 & 1) != 0;
								bool flag17 = (num11 & 1) != 0 || !flag16;
								if (flag17)
								{
									this.Animator.SetTrigger(num5);
								}
							}
							num++;
							bool flag18 = num == 8;
							if (flag18)
							{
								this._animatorData.PrevBoolsBitmask[num2] = num4;
								num = 0;
								num2++;
								bool flag19 = num2 < this._animatorData.ParamBoolsWordCount;
								if (flag19)
								{
									num3 = this._animatorData.PrevBoolsBitmask[num2];
									num4 = buffer.ReinterpretState<int>(this._animatorData.ParamBoolsPtrOffset + num2);
								}
							}
						}
					}
					break;
				}
				IL_332:
				i++;
				continue;
				goto IL_332;
			}
			bool flag20 = num > 0;
			if (flag20)
			{
				this._animatorData.PrevBoolsBitmask[num2] = num4;
			}
			wordOffset += this._animatorData.ParamBoolsWordCount;
		}

		private unsafe void CaptureLayerWeights(ref int wordOffset)
		{
			int i = 1;
			int layerCount = this._animatorData.LayerCount;
			while (i < layerCount)
			{
				*base.ReinterpretState<FloatCompressed>(wordOffset) = this.Animator.GetLayerWeight(i);
				i++;
				wordOffset++;
			}
		}

		private void ApplyLayerWeights(NetworkBehaviourBuffer buffer, ref int wordOffset)
		{
			int i = 1;
			int layerCount = this._animatorData.LayerCount;
			while (i < layerCount)
			{
				this.Animator.SetLayerWeight(i, buffer.ReinterpretState<FloatCompressed>(wordOffset));
				i++;
				wordOffset++;
			}
		}

		private void EnsureInitialized()
		{
			bool isInitialized = this._isInitialized;
			if (!isInitialized)
			{
				bool flag = !this.Animator;
				if (flag)
				{
					this.Animator = base.GetComponent<Animator>();
				}
				bool flag2 = !this.Animator || !this.Animator.gameObject.activeSelf;
				if (flag2)
				{
					this._animatorData = default(NetworkMecanimAnimator.AnimatorData);
				}
				else
				{
					this._animatorData = new NetworkMecanimAnimator.AnimatorData(this.Animator, this.SyncSettings);
					this._isInitialized = true;
					bool flag3 = this.TotalWords != this._animatorData.WordCount;
					if (flag3)
					{
						LogStream logWarn = InternalLogStreams.LogWarn;
						if (logWarn != null)
						{
							logWarn.Log("Baked and runtime word counts don't match! Does the prefab need to be reimported?");
						}
					}
				}
			}
		}

		[InlineHelp]
		public Animator Animator;

		[InlineHelp]
		[SerializeField]
		public RenderSource ApplyTiming = RenderSource.To;

		[InlineHelp]
		[SerializeField]
		[ExpandableEnum]
		internal AnimatorSyncSettings SyncSettings = AnimatorSyncSettings.ParameterInts | AnimatorSyncSettings.ParameterFloats | AnimatorSyncSettings.ParameterBools | AnimatorSyncSettings.ParameterTriggers | AnimatorSyncSettings.LayerWeights;

		[InlineHelp]
		[SerializeField]
		internal int[] StateHashes;

		[InlineHelp]
		[SerializeField]
		internal int[] TriggerHashes;

		[InlineHelp]
		[ReadOnly]
		[SerializeField]
		internal int TotalWords;

		private readonly HashSet<int> _pendingTriggers = new HashSet<int>();

		private NetworkMecanimAnimator.AnimatorData _animatorData;

		private bool _isInitialized;

		private const int BITS_PER_BOOL = 4;

		private int _lastAppliedTick;

		internal readonly struct AnimatorData
		{
			public AnimatorData(Animator animator, AnimatorSyncSettings syncSettings)
			{
				Assert.Check(animator);
				this.Parameters = animator.parameters;
				this.ParameterCount = animator.parameterCount;
				this.ParameterHashes = new int[this.ParameterCount];
				this.LayerCount = animator.layerCount;
				this.WordCount = NetworkMecanimAnimator.AnimatorData.GetWordCount(syncSettings, this.Parameters, this.ParameterHashes, this.LayerCount, out this.Param32Count, out this.ParamBoolCount, out this.SyncedLayerCount, out this.ParamBoolsWordCount);
				this.ParamBoolsPtrOffset = this.Param32Count;
				this.PrevBoolsBitmask = new int[this.ParamBoolsWordCount];
			}

			public static int GetWordCount(AnimatorSyncSettings syncSettings, AnimatorControllerParameter[] parameters, int[] parameterHashes, int layerCount, out int param32Count, out int paramBoolCount, out int syncedLayerCount, out int wordsUsedForBools)
			{
				bool flag = (syncSettings & AnimatorSyncSettings.ParameterFloats) == AnimatorSyncSettings.ParameterFloats;
				bool flag2 = (syncSettings & AnimatorSyncSettings.ParameterInts) == AnimatorSyncSettings.ParameterInts;
				bool flag3 = (syncSettings & AnimatorSyncSettings.ParameterBools) == AnimatorSyncSettings.ParameterBools;
				bool flag4 = (syncSettings & AnimatorSyncSettings.ParameterTriggers) == AnimatorSyncSettings.ParameterTriggers;
				bool flag5 = (syncSettings & AnimatorSyncSettings.StateRoot) == AnimatorSyncSettings.StateRoot;
				bool flag6 = (syncSettings & AnimatorSyncSettings.LayerWeights) == AnimatorSyncSettings.LayerWeights;
				bool flag7 = (syncSettings & AnimatorSyncSettings.StateLayers) == AnimatorSyncSettings.StateLayers;
				param32Count = 0;
				paramBoolCount = 0;
				int i = 0;
				int num = parameters.Length;
				while (i < num)
				{
					AnimatorControllerParameter animatorControllerParameter = parameters[i];
					parameterHashes[i] = animatorControllerParameter.nameHash;
					AnimatorControllerParameterType type = animatorControllerParameter.type;
					AnimatorControllerParameterType animatorControllerParameterType = type;
					switch (animatorControllerParameterType)
					{
					case AnimatorControllerParameterType.Float:
					{
						bool flag8 = flag;
						if (flag8)
						{
							param32Count++;
						}
						break;
					}
					case (AnimatorControllerParameterType)2:
						break;
					case AnimatorControllerParameterType.Int:
					{
						bool flag9 = flag2;
						if (flag9)
						{
							param32Count++;
						}
						break;
					}
					case AnimatorControllerParameterType.Bool:
					{
						bool flag10 = flag3;
						if (flag10)
						{
							paramBoolCount++;
						}
						break;
					}
					default:
						if (animatorControllerParameterType == AnimatorControllerParameterType.Trigger)
						{
							bool flag11 = flag4;
							if (flag11)
							{
								paramBoolCount++;
							}
						}
						break;
					}
					i++;
				}
				syncedLayerCount = (flag7 ? layerCount : 1);
				int num2 = param32Count;
				int num3 = flag5 ? (2 * syncedLayerCount) : 0;
				int num4 = (flag6 && layerCount > 0) ? (layerCount - 1) : 0;
				wordsUsedForBools = paramBoolCount * 4 + 31 >> 5;
				return num2 + wordsUsedForBools + num3 + num4;
			}

			public readonly int Param32Count;

			public readonly int ParamBoolCount;

			public readonly AnimatorControllerParameter[] Parameters;

			public readonly int ParameterCount;

			public readonly int[] ParameterHashes;

			public readonly int LayerCount;

			public readonly int SyncedLayerCount;

			public readonly int ParamBoolsWordCount;

			public readonly int ParamBoolsPtrOffset;

			public readonly int WordCount;

			public readonly int[] PrevBoolsBitmask;
		}
	}
}

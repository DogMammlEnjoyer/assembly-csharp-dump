using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	public class CosmeticsProximityReactorManager : MonoBehaviour, IGorillaSliceableSimple
	{
		public static CosmeticsProximityReactorManager Instance
		{
			get
			{
				return CosmeticsProximityReactorManager._instance;
			}
		}

		private void Awake()
		{
			if (CosmeticsProximityReactorManager._instance != null && CosmeticsProximityReactorManager._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			CosmeticsProximityReactorManager._instance = this;
		}

		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (CosmeticsProximityReactorManager._instance == this)
			{
				CosmeticsProximityReactorManager._instance = null;
			}
		}

		public void Register(CosmeticsProximityReactor cosmetic)
		{
			if (cosmetic == null)
			{
				return;
			}
			if (cosmetic.IsGorillaBody())
			{
				if (!this.gorillaBodyPart.Contains(cosmetic))
				{
					this.gorillaBodyPart.Add(cosmetic);
				}
				return;
			}
			if (!this.cosmetics.Contains(cosmetic))
			{
				this.cosmetics.Add(cosmetic);
			}
			IReadOnlyList<string> types = cosmetic.GetTypes();
			for (int i = 0; i < types.Count; i++)
			{
				string text = types[i];
				if (!string.IsNullOrEmpty(text))
				{
					List<CosmeticsProximityReactor> list;
					if (!this.byType.TryGetValue(text, out list))
					{
						list = new List<CosmeticsProximityReactor>();
						this.byType[text] = list;
					}
					if (!list.Contains(cosmetic))
					{
						list.Add(cosmetic);
						this.typeKeysDirty = true;
					}
				}
			}
		}

		public void Unregister(CosmeticsProximityReactor cosmetic)
		{
			if (cosmetic == null)
			{
				return;
			}
			this.cosmetics.Remove(cosmetic);
			this.gorillaBodyPart.Remove(cosmetic);
			this.matchedFrame.Remove(cosmetic);
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				if (keyValuePair.Value.Remove(cosmetic))
				{
					this.typeKeysDirty = true;
				}
			}
		}

		public void SliceUpdate()
		{
			if (this.cosmetics.Count == 0)
			{
				return;
			}
			if (this.AnyGroupHasTwo())
			{
				if (this.typeKeysDirty)
				{
					this.RebuildTypeKeysCache();
				}
				if (this.typeKeysCache.Count > 0)
				{
					for (int i = 0; i < this.typeKeysCache.Count; i++)
					{
						string key = this.typeKeysCache[i];
						List<CosmeticsProximityReactor> list;
						if (this.byType.TryGetValue(key, out list) && list != null && list.Count > 0)
						{
							this.ProcessOneGroup(list);
						}
					}
				}
			}
			if (this.gorillaBodyPart.Count > 0)
			{
				for (int j = 0; j < this.cosmetics.Count; j++)
				{
					CosmeticsProximityReactor cosmeticsProximityReactor = this.cosmetics[j];
					if (!(cosmeticsProximityReactor == null))
					{
						if (!cosmeticsProximityReactor.AcceptsAnySource())
						{
							cosmeticsProximityReactor.OnSourceAboveAll();
						}
						else
						{
							bool flag = false;
							Vector3 contact = default(Vector3);
							for (int k = 0; k < this.gorillaBodyPart.Count; k++)
							{
								CosmeticsProximityReactor cosmeticsProximityReactor2 = this.gorillaBodyPart[k];
								if (!(cosmeticsProximityReactor2 == null) && cosmeticsProximityReactor.AcceptsThisSource(cosmeticsProximityReactor2.gorillaBodyParts))
								{
									bool flag2;
									float sourceThresholdFor = cosmeticsProximityReactor.GetSourceThresholdFor(cosmeticsProximityReactor2, out flag2);
									Vector3 vector;
									if (flag2 && CosmeticsProximityReactorManager.AreCollidersWithinThreshold(cosmeticsProximityReactor2, cosmeticsProximityReactor, sourceThresholdFor, out vector))
									{
										cosmeticsProximityReactor.OnSourceBelow(vector, cosmeticsProximityReactor2.gorillaBodyParts, cosmeticsProximityReactor2.GetComponentInParent<VRRig>());
										contact = vector;
										flag = true;
									}
								}
							}
							if (flag)
							{
								cosmeticsProximityReactor.WhileSourceBelow(contact, CosmeticsProximityReactor.GorillaBodyPart.HandLeft | CosmeticsProximityReactor.GorillaBodyPart.HandRight | CosmeticsProximityReactor.GorillaBodyPart.Mouth, (this.gorillaBodyPart[0] != null) ? this.gorillaBodyPart[0].GetComponentInParent<VRRig>() : null);
							}
							else
							{
								cosmeticsProximityReactor.OnSourceAboveAll();
							}
						}
					}
				}
			}
			if (this.typeKeysDirty)
			{
				this.RebuildTypeKeysCache();
			}
			for (int l = 0; l < this.typeKeysCache.Count; l++)
			{
				string key2 = this.typeKeysCache[l];
				List<CosmeticsProximityReactor> list2;
				if (this.byType.TryGetValue(key2, out list2) && list2 != null && list2.Count > 0)
				{
					this.BreakTheBoundForGroup(list2);
				}
			}
		}

		private void ProcessOneGroup(List<CosmeticsProximityReactor> group)
		{
			if (!this.CheckProximity(group))
			{
				this.BreakTheBoundForGroup(group);
			}
		}

		private bool CheckProximity(List<CosmeticsProximityReactor> group)
		{
			bool result = false;
			for (int i = 0; i < group.Count; i++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = group[i];
				if (!(cosmeticsProximityReactor == null))
				{
					for (int j = i + 1; j < group.Count; j++)
					{
						CosmeticsProximityReactor cosmeticsProximityReactor2 = group[j];
						if (!(cosmeticsProximityReactor2 == null) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(cosmeticsProximityReactor, cosmeticsProximityReactor2))
						{
							bool flag;
							float cosmeticPairThresholdWith = cosmeticsProximityReactor.GetCosmeticPairThresholdWith(cosmeticsProximityReactor2, out flag);
							bool flag2;
							float cosmeticPairThresholdWith2 = cosmeticsProximityReactor2.GetCosmeticPairThresholdWith(cosmeticsProximityReactor, out flag2);
							if (flag && flag2)
							{
								float threshold = Mathf.Min(cosmeticPairThresholdWith, cosmeticPairThresholdWith2);
								Vector3 contact;
								if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(cosmeticsProximityReactor, cosmeticsProximityReactor2, threshold, out contact))
								{
									cosmeticsProximityReactor.OnCosmeticBelowWith(cosmeticsProximityReactor2, contact);
									cosmeticsProximityReactor2.OnCosmeticBelowWith(cosmeticsProximityReactor, contact);
									if (cosmeticsProximityReactor.IsBelow && cosmeticsProximityReactor2.IsBelow)
									{
										cosmeticsProximityReactor.RefreshAggregateMatched();
										cosmeticsProximityReactor2.RefreshAggregateMatched();
										this.matchedFrame[cosmeticsProximityReactor] = Time.frameCount;
										this.matchedFrame[cosmeticsProximityReactor2] = Time.frameCount;
										result = true;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private void BreakTheBoundForGroup(List<CosmeticsProximityReactor> group)
		{
			for (int i = 0; i < group.Count; i++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = group[i];
				int num;
				if (!(cosmeticsProximityReactor == null) && cosmeticsProximityReactor.HasAnyCosmeticMatch() && (!this.matchedFrame.TryGetValue(cosmeticsProximityReactor, out num) || num != Time.frameCount))
				{
					CosmeticsProximityReactor cosmeticsProximityReactor2;
					Vector3 contact;
					if (this.TryFindAnyCosmeticPartner(cosmeticsProximityReactor, out cosmeticsProximityReactor2, out contact))
					{
						cosmeticsProximityReactor.WhileCosmeticBelowWith(cosmeticsProximityReactor2, contact);
						cosmeticsProximityReactor2.WhileCosmeticBelowWith(cosmeticsProximityReactor, contact);
					}
					else
					{
						cosmeticsProximityReactor.OnCosmeticAboveAll();
					}
				}
			}
		}

		private bool TryFindAnyCosmeticPartner(CosmeticsProximityReactor a, out CosmeticsProximityReactor partner, out Vector3 contact)
		{
			partner = null;
			contact = default(Vector3);
			IReadOnlyList<string> types = a.GetTypes();
			for (int i = 0; i < types.Count; i++)
			{
				string text = types[i];
				List<CosmeticsProximityReactor> list;
				if (!string.IsNullOrEmpty(text) && this.byType.TryGetValue(text, out list) && list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						CosmeticsProximityReactor cosmeticsProximityReactor = list[j];
						if (!(cosmeticsProximityReactor == null) && !(cosmeticsProximityReactor == a) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(a, cosmeticsProximityReactor))
						{
							bool flag;
							float cosmeticPairThresholdWith = a.GetCosmeticPairThresholdWith(cosmeticsProximityReactor, out flag);
							bool flag2;
							float cosmeticPairThresholdWith2 = cosmeticsProximityReactor.GetCosmeticPairThresholdWith(a, out flag2);
							if (flag && flag2)
							{
								float threshold = Mathf.Min(cosmeticPairThresholdWith, cosmeticPairThresholdWith2);
								Vector3 vector;
								if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(a, cosmeticsProximityReactor, threshold, out vector))
								{
									partner = cosmeticsProximityReactor;
									contact = vector;
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		private static bool ShouldSkipSameIdPair(CosmeticsProximityReactor a, CosmeticsProximityReactor b)
		{
			return (a.ignoreSameCosmeticInstances || b.ignoreSameCosmeticInstances) && !string.IsNullOrEmpty(a.PlayFabID) && !string.IsNullOrEmpty(b.PlayFabID) && string.Equals(a.PlayFabID, b.PlayFabID, StringComparison.Ordinal);
		}

		private static bool AreCollidersWithinThreshold(CosmeticsProximityReactor a, CosmeticsProximityReactor b, float threshold, out Vector3 contactPoint)
		{
			Vector3 vector = (b.collider == null) ? b.transform.position : b.collider.ClosestPoint(a.transform.position);
			Vector3 a2 = (a.collider == null) ? a.transform.position : a.collider.ClosestPoint(vector);
			contactPoint = (a2 + vector) * 0.5f;
			return Vector3.Distance(a2, vector) <= threshold;
		}

		private bool AnyGroupHasTwo()
		{
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				List<CosmeticsProximityReactor> value = keyValuePair.Value;
				if (value != null && value.Count >= 2)
				{
					return true;
				}
			}
			return false;
		}

		private void RebuildTypeKeysCache()
		{
			this.typeKeysCache.Clear();
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				List<CosmeticsProximityReactor> value = keyValuePair.Value;
				if (value != null && value.Count > 0)
				{
					this.typeKeysCache.Add(keyValuePair.Key);
				}
			}
			this.typeKeysDirty = false;
			if (this.groupCursor >= this.typeKeysCache.Count)
			{
				this.groupCursor = 0;
			}
		}

		private static CosmeticsProximityReactorManager _instance;

		private readonly List<CosmeticsProximityReactor> cosmetics = new List<CosmeticsProximityReactor>();

		private readonly List<CosmeticsProximityReactor> gorillaBodyPart = new List<CosmeticsProximityReactor>();

		private readonly Dictionary<string, List<CosmeticsProximityReactor>> byType = new Dictionary<string, List<CosmeticsProximityReactor>>(StringComparer.Ordinal);

		private readonly Dictionary<CosmeticsProximityReactor, int> matchedFrame = new Dictionary<CosmeticsProximityReactor, int>();

		private readonly List<string> typeKeysCache = new List<string>();

		private bool typeKeysDirty;

		private int groupCursor;

		internal static readonly List<string> SharedKeysCache = new List<string>();
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Meta.WitAi.Attributes;
using Meta.WitAi.TTS.Data;
using UnityEngine;

namespace Meta.WitAi.TTS.LipSync
{
	public abstract class BaseVisemeBlendShapeLipSync : MonoBehaviour, ILipsyncAnimator
	{
		public abstract SkinnedMeshRenderer SkinnedMeshRenderer { get; }

		protected virtual void Reset()
		{
			if (this.VisemeBlendShapes != null && this.VisemeBlendShapes.Length != 0)
			{
				return;
			}
			List<BaseVisemeBlendShapeLipSync.VisemeBlendShapeData> list = new List<BaseVisemeBlendShapeLipSync.VisemeBlendShapeData>();
			foreach (object obj in Enum.GetValues(typeof(Viseme)))
			{
				Viseme viseme = (Viseme)obj;
				list.Add(new BaseVisemeBlendShapeLipSync.VisemeBlendShapeData
				{
					viseme = viseme
				});
			}
			this.VisemeBlendShapes = list.ToArray();
		}

		protected virtual void Awake()
		{
			this.RefreshBlendShapeLookup();
		}

		public void RefreshBlendShapeLookup()
		{
			if (this.VisemeBlendShapes == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			this._visemeLookup.Clear();
			this._blendShapeLookup.Clear();
			for (int i = 0; i < this.VisemeBlendShapes.Length; i++)
			{
				Viseme viseme = this.VisemeBlendShapes[i].viseme;
				if (this._visemeLookup.ContainsKey(viseme))
				{
					stringBuilder.AppendLine(string.Format("{0} Viseme already set (VisemeBlendShapes[{1}] ignored)", viseme, i));
				}
				else
				{
					this._visemeLookup[viseme] = i;
					foreach (BaseVisemeBlendShapeLipSync.VisemeBlendShapeWeight visemeBlendShapeWeight in this.VisemeBlendShapes[i].weights)
					{
						if (!string.IsNullOrEmpty(visemeBlendShapeWeight.blendShapeId) && !this._blendShapeLookup.ContainsKey(visemeBlendShapeWeight.blendShapeId))
						{
							this._blendShapeLookup[visemeBlendShapeWeight.blendShapeId] = -1;
						}
					}
				}
			}
			foreach (object obj in Enum.GetValues(typeof(Viseme)))
			{
				Viseme viseme2 = (Viseme)obj;
				if (!this._visemeLookup.ContainsKey(viseme2))
				{
					stringBuilder.AppendLine(string.Format("{0} Viseme missing texture", viseme2));
				}
			}
			this.GetBlendShapeNames();
			for (int k = 0; k < this._blendShapeNames.Count; k++)
			{
				if (this._blendShapeLookup.ContainsKey(this._blendShapeNames[k]))
				{
					this._blendShapeLookup[this._blendShapeNames[k]] = k;
				}
			}
			if (stringBuilder.Length > 0)
			{
				VLog.E(base.GetType().Name, string.Format("Setup Warnings:\n{0}", stringBuilder), null);
			}
		}

		public void OnVisemeStarted(Viseme viseme)
		{
		}

		public void OnVisemeFinished(Viseme viseme)
		{
		}

		public virtual void OnVisemeLerp(Viseme fromEvent, Viseme toEvent, float percentage)
		{
			if (this.SkinnedMeshRenderer == null)
			{
				VLog.E(base.GetType().Name, "Skinned Mesh Renderer unassigned", null);
			}
			if (this._blendShapeLookup == null)
			{
				return;
			}
			foreach (string text in this._blendShapeLookup.Keys)
			{
				int num = this._blendShapeLookup[text];
				if (num != -1)
				{
					float num2;
					if (percentage >= 1f)
					{
						num2 = this.GetBlendShapeWeight(fromEvent, text);
					}
					else if (percentage <= 0f)
					{
						num2 = this.GetBlendShapeWeight(fromEvent, text);
					}
					else
					{
						float blendShapeWeight = this.GetBlendShapeWeight(fromEvent, text);
						float blendShapeWeight2 = this.GetBlendShapeWeight(toEvent, text);
						num2 = Mathf.Lerp(blendShapeWeight, blendShapeWeight2, percentage);
					}
					this.SkinnedMeshRenderer.SetBlendShapeWeight(num, num2 * this.blendShapeWeightScale);
				}
			}
		}

		public float GetBlendShapeWeight(Viseme viseme, string blendShapeName)
		{
			int num;
			if (this._visemeLookup.TryGetValue(viseme, out num) && num >= 0 && num < this.VisemeBlendShapes.Length)
			{
				BaseVisemeBlendShapeLipSync.VisemeBlendShapeData visemeBlendShapeData = this.VisemeBlendShapes[num];
				for (int i = 0; i < visemeBlendShapeData.weights.Length; i++)
				{
					if (string.Equals(visemeBlendShapeData.weights[i].blendShapeId, blendShapeName))
					{
						return visemeBlendShapeData.weights[i].weight;
					}
				}
			}
			return 0f;
		}

		public string[] GetBlendShapeNames()
		{
			if (this._blendShapeNames == null)
			{
				this._blendShapeNames = new List<string>();
			}
			if (this.SkinnedMeshRenderer != null && this.SkinnedMeshRenderer.sharedMesh != null && this._blendShapeNames.Count != this.SkinnedMeshRenderer.sharedMesh.blendShapeCount)
			{
				this._blendShapeNames.Clear();
				for (int i = 0; i < this.SkinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
				{
					this._blendShapeNames.Add(this.SkinnedMeshRenderer.sharedMesh.GetBlendShapeName(i));
				}
			}
			return this._blendShapeNames.ToArray();
		}

		public float blendShapeWeightScale = 1f;

		public BaseVisemeBlendShapeLipSync.VisemeBlendShapeData[] VisemeBlendShapes;

		private Dictionary<Viseme, int> _visemeLookup = new Dictionary<Viseme, int>();

		private Dictionary<string, int> _blendShapeLookup = new Dictionary<string, int>();

		private List<string> _blendShapeNames = new List<string>();

		[Serializable]
		public struct VisemeBlendShapeData
		{
			public Viseme viseme;

			public BaseVisemeBlendShapeLipSync.VisemeBlendShapeWeight[] weights;
		}

		[Serializable]
		public struct VisemeBlendShapeWeight
		{
			[DropDown("GetBlendShapeNames", true, false, true, true, null, true)]
			public string blendShapeId;

			public float weight;
		}
	}
}

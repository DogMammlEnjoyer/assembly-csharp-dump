using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Liv.Lck.Rendering
{
	public class LckCompositionEngine : MonoBehaviour
	{
		public static LckCompositionEngine Instance { get; private set; }

		public bool HasActiveLayers { get; private set; }

		public List<ILckCompositionLayer> ActiveLayers { get; private set; } = new List<ILckCompositionLayer>();

		public bool IsDirty { get; set; } = true;

		private void OnEnable()
		{
			if (LckCompositionEngine.Instance != null && LckCompositionEngine.Instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			LckCompositionEngine.Instance = this;
			this.SetDirty();
		}

		private void OnDisable()
		{
			if (LckCompositionEngine.Instance == this)
			{
				LckCompositionEngine.Instance = null;
			}
		}

		public void SetDirty()
		{
			this.IsDirty = true;
			LckCompositionProfile compositionProfile = this._compositionProfile;
			bool? flag;
			if (compositionProfile == null)
			{
				flag = null;
			}
			else
			{
				List<LckCompositionLayer> layers = compositionProfile.Layers;
				if (layers == null)
				{
					flag = null;
				}
				else
				{
					flag = new bool?(layers.Any((LckCompositionLayer layer) => layer.IsActive && layer.CurrentTexture != null));
				}
			}
			bool? flag2 = flag;
			this.HasActiveLayers = flag2.GetValueOrDefault();
		}

		public void UpdateActiveLayers()
		{
			this.ActiveLayers.Clear();
			LckCompositionProfile compositionProfile = this._compositionProfile;
			if (((compositionProfile != null) ? compositionProfile.Layers : null) != null)
			{
				foreach (LckCompositionLayer lckCompositionLayer in this._compositionProfile.Layers)
				{
					if (lckCompositionLayer != null && lckCompositionLayer.IsActive && lckCompositionLayer.CurrentTexture != null)
					{
						this.ActiveLayers.Add(lckCompositionLayer);
					}
				}
			}
			this.HasActiveLayers = (this.ActiveLayers.Count > 0);
		}

		[SerializeField]
		private LckCompositionProfile _compositionProfile;

		[Tooltip("The material to use for blending if a layer does not define its own.")]
		[SerializeField]
		public Material DefaultBlendMaterial;
	}
}

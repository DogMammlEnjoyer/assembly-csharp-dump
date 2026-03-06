using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Liv.Lck.Rendering
{
	[CreateAssetMenu(fileName = "Lck Composition Profile", menuName = "LIV/LCK/Composition Profile")]
	public class LckCompositionProfile : ScriptableObject
	{
		public void SetOrientation(bool isHorizontal)
		{
			Debug.Log("LCK SetOrientation 0");
			if (this.Layers == null)
			{
				return;
			}
			foreach (LckCompositionLayer lckCompositionLayer in this.Layers)
			{
				Debug.Log("LCK SetOrientation 1");
				ILckOrientationAwareLayer lckOrientationAwareLayer = lckCompositionLayer as ILckOrientationAwareLayer;
				if (lckOrientationAwareLayer != null)
				{
					Debug.Log("LCK SetOrientation 2");
					lckOrientationAwareLayer.SetOrientation(isHorizontal);
				}
			}
			LckCompositionEngine instance = LckCompositionEngine.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SetDirty();
		}

		public T GetLayer<T>(string name) where T : LckCompositionLayer
		{
			if (this.Layers == null)
			{
				return default(T);
			}
			return this.Layers.FirstOrDefault((LckCompositionLayer layer) => layer.Name == name) as T;
		}

		public void SetLayerActive(string name, bool isActive)
		{
			LckCompositionLayer layer = this.GetLayer<LckCompositionLayer>(name);
			if (layer != null)
			{
				layer.IsActive = isActive;
				LckCompositionEngine instance = LckCompositionEngine.Instance;
				if (instance == null)
				{
					return;
				}
				instance.SetDirty();
			}
		}

		public List<ILckCompositionLayer> GetActiveLayers()
		{
			List<ILckCompositionLayer> list = new List<ILckCompositionLayer>();
			if (this.Layers != null)
			{
				foreach (LckCompositionLayer lckCompositionLayer in this.Layers)
				{
					if (lckCompositionLayer != null && lckCompositionLayer.IsActive && lckCompositionLayer.CurrentTexture != null)
					{
						list.Add(lckCompositionLayer);
					}
				}
			}
			return list;
		}

		[SerializeReference]
		[Tooltip("The list of layers to be composed. Order matters.")]
		public List<LckCompositionLayer> Layers = new List<LckCompositionLayer>();
	}
}

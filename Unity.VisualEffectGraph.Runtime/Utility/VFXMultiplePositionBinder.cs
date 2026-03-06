using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Multiple Position Binder")]
	[VFXBinder("Point Cache/Multiple Position Binder")]
	internal class VFXMultiplePositionBinder : VFXBinderBase
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateTexture();
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Targets != null && component.HasTexture(this.PositionMapProperty) && component.HasInt(this.PositionCountProperty);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			if (this.EveryFrame || Application.isEditor)
			{
				this.UpdateTexture();
			}
			component.SetTexture(this.PositionMapProperty, this.positionMap);
			component.SetInt(this.PositionCountProperty, this.count);
		}

		private void UpdateTexture()
		{
			if (this.Targets == null || this.Targets.Length == 0)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			foreach (GameObject gameObject in this.Targets)
			{
				if (gameObject != null)
				{
					list.Add(gameObject.transform.position);
				}
			}
			this.count = list.Count;
			if (this.positionMap == null || this.positionMap.width != this.count)
			{
				this.positionMap = new Texture2D(this.count, 1, TextureFormat.RGBAFloat, false);
			}
			List<Color> list2 = new List<Color>();
			foreach (Vector3 vector in list)
			{
				list2.Add(new Color(vector.x, vector.y, vector.z));
			}
			this.positionMap.name = base.gameObject.name + "_PositionMap";
			this.positionMap.filterMode = FilterMode.Point;
			this.positionMap.wrapMode = TextureWrapMode.Repeat;
			this.positionMap.SetPixels(list2.ToArray(), 0);
			this.positionMap.Apply();
		}

		public override string ToString()
		{
			return string.Format("Multiple Position Binder ({0} positions)", this.count);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Texture2D"
		})]
		[FormerlySerializedAs("PositionMapParameter")]
		public ExposedProperty PositionMapProperty = "PositionMap";

		[VFXPropertyBinding(new string[]
		{
			"System.Int32"
		})]
		[FormerlySerializedAs("PositionCountParameter")]
		public ExposedProperty PositionCountProperty = "PositionCount";

		public GameObject[] Targets;

		public bool EveryFrame;

		private Texture2D positionMap;

		private int count;
	}
}

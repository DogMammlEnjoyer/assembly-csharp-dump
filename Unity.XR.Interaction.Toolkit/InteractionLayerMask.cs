using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Serializable]
	public struct InteractionLayerMask : ISerializationCallbackReceiver
	{
		public static implicit operator int(InteractionLayerMask mask)
		{
			return mask.m_Mask;
		}

		public static implicit operator InteractionLayerMask(int intVal)
		{
			InteractionLayerMask result;
			result.m_Mask = intVal;
			result.m_Bits = (uint)intVal;
			return result;
		}

		public int value
		{
			get
			{
				return this.m_Mask;
			}
			set
			{
				this.m_Mask = value;
				this.m_Bits = (uint)value;
			}
		}

		public static string LayerToName(int layer)
		{
			if (layer < 0 || layer >= 32)
			{
				return string.Empty;
			}
			return ScriptableSettings<InteractionLayerSettings>.Instance.GetLayerNameAt(layer);
		}

		public static int NameToLayer(string layerName)
		{
			return ScriptableSettings<InteractionLayerSettings>.Instance.GetLayer(layerName);
		}

		public static int GetMask(params string[] layerNames)
		{
			if (layerNames == null)
			{
				throw new ArgumentNullException("layerNames");
			}
			int num = 0;
			for (int i = 0; i < layerNames.Length; i++)
			{
				int num2 = InteractionLayerMask.NameToLayer(layerNames[i]);
				if (num2 != -1)
				{
					num |= 1 << num2;
				}
			}
			return num;
		}

		public void OnAfterDeserialize()
		{
			this.m_Mask = (int)this.m_Bits;
		}

		public void OnBeforeSerialize()
		{
		}

		[SerializeField]
		private uint m_Bits;

		private int m_Mask;
	}
}

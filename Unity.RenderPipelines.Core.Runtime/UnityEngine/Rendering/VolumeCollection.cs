using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	internal class VolumeCollection
	{
		public int count
		{
			get
			{
				return this.m_Volumes.Count;
			}
		}

		public bool Register(Volume volume, int layer)
		{
			if (volume == null)
			{
				throw new ArgumentNullException("volume", "The volume to register is null");
			}
			if (this.m_Volumes.Contains(volume))
			{
				return false;
			}
			this.m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<Volume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & 1 << layer) != 0 && !keyValuePair.Value.Contains(volume))
				{
					keyValuePair.Value.Add(volume);
				}
			}
			this.SetLayerIndexDirty(layer);
			return true;
		}

		public bool Unregister(Volume volume, int layer)
		{
			if (volume == null)
			{
				throw new ArgumentNullException("volume", "The volume to unregister is null");
			}
			this.m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<Volume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & 1 << layer) != 0)
				{
					keyValuePair.Value.Remove(volume);
				}
			}
			this.SetLayerIndexDirty(layer);
			return true;
		}

		public bool ChangeLayer(Volume volume, int previousLayerIndex, int currentLayerIndex)
		{
			if (volume == null)
			{
				throw new ArgumentNullException("volume", "The volume to change layer is null");
			}
			this.Unregister(volume, previousLayerIndex);
			return this.Register(volume, currentLayerIndex);
		}

		internal static void SortByPriority(List<Volume> volumes)
		{
			for (int i = 1; i < volumes.Count; i++)
			{
				Volume volume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > volume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = volume;
			}
		}

		public List<Volume> GrabVolumes(LayerMask mask)
		{
			List<Volume> list;
			if (!this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				list = new List<Volume>();
				int count = this.m_Volumes.Count;
				for (int i = 0; i < count; i++)
				{
					Volume volume = this.m_Volumes[i];
					if ((mask & 1 << volume.gameObject.layer) != 0)
					{
						list.Add(volume);
						this.m_SortNeeded[mask] = true;
					}
				}
				this.m_SortedVolumes.Add(mask, list);
			}
			bool flag;
			if (this.m_SortNeeded.TryGetValue(mask, out flag) && flag)
			{
				this.m_SortNeeded[mask] = false;
				VolumeCollection.SortByPriority(list);
			}
			return list;
		}

		public void SetLayerIndexDirty(int layerIndex)
		{
			foreach (KeyValuePair<int, List<Volume>> keyValuePair in this.m_SortedVolumes)
			{
				int key = keyValuePair.Key;
				if ((key & 1 << layerIndex) != 0)
				{
					this.m_SortNeeded[key] = true;
				}
			}
		}

		public bool IsComponentActiveInMask<T>(LayerMask layerMask) where T : VolumeComponent
		{
			int value = layerMask.value;
			foreach (KeyValuePair<int, List<Volume>> keyValuePair in this.m_SortedVolumes)
			{
				if (keyValuePair.Key == value)
				{
					foreach (Volume volume in keyValuePair.Value)
					{
						T t;
						if (volume.enabled && !(volume.profileRef == null) && volume.profileRef.TryGet<T>(out t) && t.active)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal const int k_MaxLayerCount = 32;

		private readonly Dictionary<int, List<Volume>> m_SortedVolumes = new Dictionary<int, List<Volume>>();

		private readonly List<Volume> m_Volumes = new List<Volume>();

		private readonly Dictionary<int, bool> m_SortNeeded = new Dictionary<int, bool>();
	}
}

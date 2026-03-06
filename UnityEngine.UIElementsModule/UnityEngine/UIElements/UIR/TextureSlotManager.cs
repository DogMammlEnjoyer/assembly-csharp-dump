using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR
{
	internal class TextureSlotManager
	{
		static TextureSlotManager()
		{
			TextureSlotManager.k_SlotCount = 8;
			TextureSlotManager.slotIds = new int[TextureSlotManager.k_SlotCount];
			for (int i = 0; i < TextureSlotManager.k_SlotCount; i++)
			{
				TextureSlotManager.slotIds[i] = Shader.PropertyToID(string.Format("_Texture{0}", i));
			}
		}

		public TextureSlotManager()
		{
			this.m_Textures = new TextureId[TextureSlotManager.k_SlotCount];
			this.m_Tickets = new int[TextureSlotManager.k_SlotCount];
			this.m_GpuTextures = new Vector4[TextureSlotManager.k_SlotCount * TextureSlotManager.k_SlotSize];
			this.Reset();
		}

		public void Reset()
		{
			this.m_CurrentTicket = 0;
			this.m_FirstUsedTicket = 0;
			for (int i = 0; i < TextureSlotManager.k_SlotCount; i++)
			{
				this.m_Textures[i] = TextureId.invalid;
				this.m_Tickets[i] = -1;
				this.SetGpuData(i, TextureId.invalid, 1, 1, 0f, 0f, false);
			}
		}

		public void StartNewBatch()
		{
			int num = this.m_CurrentTicket + 1;
			this.m_CurrentTicket = num;
			this.m_FirstUsedTicket = num;
			this.FreeSlots = TextureSlotManager.k_SlotCount;
		}

		public int IndexOf(TextureId id)
		{
			for (int i = 0; i < TextureSlotManager.k_SlotCount; i++)
			{
				bool flag = this.m_Textures[i].index == id.index;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void MarkUsed(int slotIndex)
		{
			int num = this.m_Tickets[slotIndex];
			bool flag = num < this.m_FirstUsedTicket;
			int num2;
			if (flag)
			{
				num2 = this.FreeSlots - 1;
				this.FreeSlots = num2;
			}
			int[] tickets = this.m_Tickets;
			num2 = this.m_CurrentTicket + 1;
			this.m_CurrentTicket = num2;
			tickets[slotIndex] = num2;
		}

		public int FreeSlots { get; private set; } = TextureSlotManager.k_SlotCount;

		public int FindOldestSlot()
		{
			int num = this.m_Tickets[0];
			int result = 0;
			for (int i = 1; i < TextureSlotManager.k_SlotCount; i++)
			{
				bool flag = this.m_Tickets[i] < num;
				if (flag)
				{
					num = this.m_Tickets[i];
					result = i;
				}
			}
			return result;
		}

		public void Bind(TextureId id, float sdfScale, float sharpness, bool isPremultiplied, int slot, MaterialPropertyBlock mat, CommandList commandList = null)
		{
			Texture texture = this.textureRegistry.GetTexture(id);
			bool flag = texture == null;
			if (flag)
			{
				texture = Texture2D.whiteTexture;
			}
			this.m_Textures[slot] = id;
			this.MarkUsed(slot);
			this.SetGpuData(slot, id, texture.width, texture.height, sdfScale, sharpness, isPremultiplied);
			bool flag2 = commandList == null;
			if (flag2)
			{
				mat.SetTexture(TextureSlotManager.slotIds[slot], texture);
				mat.SetVectorArray(TextureSlotManager.textureTableId, this.m_GpuTextures);
			}
			else
			{
				int num = slot * TextureSlotManager.k_SlotSize;
				commandList.SetTexture(TextureSlotManager.slotIds[slot], texture, num, this.m_GpuTextures[num], this.m_GpuTextures[num + 1]);
			}
		}

		public void SetGpuData(int slotIndex, TextureId id, int textureWidth, int textureHeight, float sdfScale, float sharpness, bool isPremultiplied)
		{
			int num = slotIndex * TextureSlotManager.k_SlotSize;
			float y = 1f / (float)textureWidth;
			float z = 1f / (float)textureHeight;
			this.m_GpuTextures[num] = new Vector4(id.ConvertToGpu(), y, z, sdfScale);
			this.m_GpuTextures[num + 1] = new Vector4((float)textureWidth, (float)textureHeight, sharpness, isPremultiplied ? 1f : 0f);
		}

		internal static readonly int k_SlotCount;

		internal static readonly int k_SlotSize = 2;

		internal static int[] slotIds;

		internal static readonly int textureTableId = Shader.PropertyToID("_TextureInfo");

		private TextureId[] m_Textures;

		private int[] m_Tickets;

		private int m_CurrentTicket;

		private int m_FirstUsedTicket;

		private Vector4[] m_GpuTextures;

		internal TextureRegistry textureRegistry = TextureRegistry.instance;
	}
}

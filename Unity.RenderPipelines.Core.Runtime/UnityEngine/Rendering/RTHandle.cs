using System;

namespace UnityEngine.Rendering
{
	public class RTHandle
	{
		public void SetCustomHandleProperties(in RTHandleProperties properties)
		{
			this.m_UseCustomHandleScales = true;
			this.m_CustomHandleProperties = properties;
		}

		public void ClearCustomHandleProperties()
		{
			this.m_UseCustomHandleScales = false;
		}

		public Vector2 scaleFactor { get; internal set; }

		public bool useScaling { get; internal set; }

		public Vector2Int referenceSize { get; internal set; }

		public RTHandleProperties rtHandleProperties
		{
			get
			{
				if (!this.m_UseCustomHandleScales)
				{
					return this.m_Owner.rtHandleProperties;
				}
				return this.m_CustomHandleProperties;
			}
		}

		public RenderTexture rt
		{
			get
			{
				return this.m_RT;
			}
		}

		public Texture externalTexture
		{
			get
			{
				return this.m_ExternalTexture;
			}
		}

		public RenderTargetIdentifier nameID
		{
			get
			{
				return this.m_NameID;
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public bool isMSAAEnabled
		{
			get
			{
				return this.m_EnableMSAA;
			}
		}

		internal RTHandle(RTHandleSystem owner)
		{
			this.m_Owner = owner;
		}

		public static implicit operator RenderTargetIdentifier(RTHandle handle)
		{
			if (handle == null)
			{
				return default(RenderTargetIdentifier);
			}
			return handle.nameID;
		}

		public static implicit operator Texture(RTHandle handle)
		{
			if (handle == null)
			{
				return null;
			}
			if (!(handle.rt != null))
			{
				return handle.m_ExternalTexture;
			}
			return handle.rt;
		}

		public static implicit operator RenderTexture(RTHandle handle)
		{
			if (handle == null)
			{
				return null;
			}
			return handle.rt;
		}

		internal void SetRenderTexture(RenderTexture rt, bool transferOwnership = true)
		{
			this.m_RT = rt;
			this.m_ExternalTexture = null;
			this.m_RTHasOwnership = transferOwnership;
			this.m_NameID = new RenderTargetIdentifier(rt);
		}

		internal void SetTexture(Texture tex)
		{
			this.m_RT = null;
			this.m_ExternalTexture = tex;
			this.m_NameID = new RenderTargetIdentifier(tex);
		}

		internal void SetTexture(RenderTargetIdentifier tex)
		{
			this.m_RT = null;
			this.m_ExternalTexture = null;
			this.m_NameID = tex;
		}

		public int GetInstanceID()
		{
			if (this.m_RT != null)
			{
				return this.m_RT.GetInstanceID();
			}
			if (this.m_ExternalTexture != null)
			{
				return this.m_ExternalTexture.GetInstanceID();
			}
			return this.m_NameID.GetHashCode();
		}

		public void Release()
		{
			this.m_Owner.Remove(this);
			if (this.m_RTHasOwnership)
			{
				CoreUtils.Destroy(this.m_RT);
			}
			this.m_NameID = BuiltinRenderTextureType.None;
			this.m_RT = null;
			this.m_ExternalTexture = null;
			this.m_RTHasOwnership = true;
		}

		public Vector2Int GetScaledSize(Vector2Int refSize)
		{
			if (!this.useScaling)
			{
				return refSize;
			}
			if (this.scaleFunc != null)
			{
				return this.scaleFunc(refSize);
			}
			return new Vector2Int(Mathf.RoundToInt(this.scaleFactor.x * (float)refSize.x), Mathf.RoundToInt(this.scaleFactor.y * (float)refSize.y));
		}

		public Vector2Int GetScaledSize()
		{
			if (!this.useScaling)
			{
				return this.referenceSize;
			}
			if (this.scaleFunc != null)
			{
				return this.scaleFunc(this.referenceSize);
			}
			return new Vector2Int(Mathf.RoundToInt(this.scaleFactor.x * (float)this.referenceSize.x), Mathf.RoundToInt(this.scaleFactor.y * (float)this.referenceSize.y));
		}

		public void SwitchToFastMemory(CommandBuffer cmd, float residencyFraction = 1f, FastMemoryFlags flags = FastMemoryFlags.SpillTop, bool copyContents = false)
		{
			residencyFraction = Mathf.Clamp01(residencyFraction);
			cmd.SwitchIntoFastMemory(this.m_RT, flags, residencyFraction, copyContents);
		}

		public void CopyToFastMemory(CommandBuffer cmd, float residencyFraction = 1f, FastMemoryFlags flags = FastMemoryFlags.SpillTop)
		{
			this.SwitchToFastMemory(cmd, residencyFraction, flags, true);
		}

		public void SwitchOutFastMemory(CommandBuffer cmd, bool copyContents = true)
		{
			cmd.SwitchOutOfFastMemory(this.m_RT, copyContents);
		}

		internal RTHandleSystem m_Owner;

		internal RenderTexture m_RT;

		internal Texture m_ExternalTexture;

		internal RenderTargetIdentifier m_NameID;

		internal bool m_EnableMSAA;

		internal bool m_EnableRandomWrite;

		internal bool m_EnableHWDynamicScale;

		internal bool m_RTHasOwnership = true;

		internal string m_Name;

		internal bool m_UseCustomHandleScales;

		internal RTHandleProperties m_CustomHandleProperties;

		internal ScaleFunc scaleFunc;
	}
}

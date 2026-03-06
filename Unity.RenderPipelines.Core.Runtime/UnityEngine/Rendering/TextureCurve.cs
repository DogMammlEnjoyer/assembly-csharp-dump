using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class TextureCurve : IDisposable
	{
		public int length { get; private set; }

		public Keyframe this[int index]
		{
			get
			{
				return this.m_Curve[index];
			}
		}

		public TextureCurve(AnimationCurve baseCurve, float zeroValue, bool loop, in Vector2 bounds) : this(baseCurve.keys, zeroValue, loop, bounds)
		{
		}

		public TextureCurve(Keyframe[] keys, float zeroValue, bool loop, in Vector2 bounds)
		{
			this.m_Curve = new AnimationCurve(keys);
			this.m_ZeroValue = zeroValue;
			this.m_Loop = loop;
			Vector2 vector = bounds;
			this.m_Range = vector.magnitude;
			this.length = keys.Length;
			this.SetDirty();
		}

		public void Dispose()
		{
			this.Release();
		}

		public void Release()
		{
			if (this.m_Texture != null)
			{
				CoreUtils.Destroy(this.m_Texture);
			}
			this.m_Texture = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDirty()
		{
			this.m_IsCurveDirty = true;
			this.m_IsTextureDirty = true;
		}

		private static GraphicsFormat GetTextureFormat()
		{
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16_SFloat, GraphicsFormatUsage.SetPixels))
			{
				return GraphicsFormat.R16_SFloat;
			}
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, GraphicsFormatUsage.SetPixels))
			{
				return GraphicsFormat.R8_UNorm;
			}
			return GraphicsFormat.R8G8B8A8_UNorm;
		}

		public Texture2D GetTexture()
		{
			if (this.m_Texture == null)
			{
				this.m_Texture = new Texture2D(128, 1, TextureCurve.GetTextureFormat(), TextureCreationFlags.None);
				this.m_Texture.name = "CurveTexture";
				this.m_Texture.hideFlags = HideFlags.HideAndDontSave;
				this.m_Texture.filterMode = FilterMode.Bilinear;
				this.m_Texture.wrapMode = TextureWrapMode.Clamp;
				this.m_Texture.anisoLevel = 0;
				this.m_IsTextureDirty = true;
			}
			if (this.m_IsTextureDirty)
			{
				Color[] array = new Color[128];
				for (int i = 0; i < array.Length; i++)
				{
					array[i].r = this.Evaluate((float)i * 0.0078125f);
				}
				this.m_Texture.SetPixels(array);
				this.m_Texture.Apply(false, false);
				this.m_IsTextureDirty = false;
			}
			return this.m_Texture;
		}

		public float Evaluate(float time)
		{
			if (this.m_IsCurveDirty)
			{
				this.length = this.m_Curve.length;
			}
			if (this.length == 0)
			{
				return this.m_ZeroValue;
			}
			if (!this.m_Loop || this.length == 1)
			{
				return this.m_Curve.Evaluate(time);
			}
			if (this.m_IsCurveDirty)
			{
				if (this.m_LoopingCurve == null)
				{
					this.m_LoopingCurve = new AnimationCurve();
				}
				Keyframe key = this.m_Curve[this.length - 1];
				key.time -= this.m_Range;
				Keyframe key2 = this.m_Curve[0];
				key2.time += this.m_Range;
				this.m_LoopingCurve.keys = this.m_Curve.keys;
				this.m_LoopingCurve.AddKey(key);
				this.m_LoopingCurve.AddKey(key2);
				this.m_IsCurveDirty = false;
			}
			return this.m_LoopingCurve.Evaluate(time);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int AddKey(float time, float value)
		{
			int num = this.m_Curve.AddKey(time, value);
			if (num > -1)
			{
				this.SetDirty();
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int MoveKey(int index, in Keyframe key)
		{
			int result = this.m_Curve.MoveKey(index, key);
			this.SetDirty();
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveKey(int index)
		{
			this.m_Curve.RemoveKey(index);
			this.SetDirty();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SmoothTangents(int index, float weight)
		{
			this.m_Curve.SmoothTangents(index, weight);
			this.SetDirty();
		}

		private const int k_Precision = 128;

		private const float k_Step = 0.0078125f;

		[SerializeField]
		private bool m_Loop;

		[SerializeField]
		private float m_ZeroValue;

		[SerializeField]
		private float m_Range;

		[SerializeField]
		private AnimationCurve m_Curve;

		private AnimationCurve m_LoopingCurve;

		private Texture2D m_Texture;

		private bool m_IsCurveDirty;

		private bool m_IsTextureDirty;
	}
}

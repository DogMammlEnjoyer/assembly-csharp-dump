using System;
using System.Collections.Generic;
using System.Text;
using Meta.Voice.Logging;
using Meta.WitAi.TTS.Data;
using UnityEngine;

namespace Meta.WitAi.TTS.LipSync
{
	public abstract class BaseTextureFlipLipSync : MonoBehaviour, ILipsyncAnimator
	{
		public abstract Renderer Renderer { get; }

		protected virtual void Reset()
		{
			if (this.VisemeTextures != null && this.VisemeTextures.Length != 0)
			{
				return;
			}
			List<BaseTextureFlipLipSync.VisemeTextureData> list = new List<BaseTextureFlipLipSync.VisemeTextureData>();
			foreach (object obj in Enum.GetValues(typeof(Viseme)))
			{
				Viseme viseme = (Viseme)obj;
				list.Add(new BaseTextureFlipLipSync.VisemeTextureData
				{
					viseme = viseme
				});
			}
			this.VisemeTextures = list.ToArray();
		}

		protected virtual void Awake()
		{
			this.RefreshTextureLookup();
		}

		private void Start()
		{
			if (!this.Renderer)
			{
				this._log.Warning("Texture Flip material unassigned on {0}", new object[]
				{
					base.name
				});
			}
			this.SetViseme(Viseme.sil);
		}

		public void RefreshTextureLookup()
		{
			if (this.VisemeTextures == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			this._textureLookup.Clear();
			for (int i = 0; i < this.VisemeTextures.Length; i++)
			{
				Viseme viseme = this.VisemeTextures[i].viseme;
				if (this.VisemeTextures[i].textures == null || this.VisemeTextures[i].textures.Length == 0)
				{
					stringBuilder.AppendLine(string.Format("VisemeTextures[{0}] Warning: No textures are set.", i));
				}
				else if (this._textureLookup.ContainsKey(viseme))
				{
					stringBuilder.AppendLine(string.Format("VisemeTextures[{0}] Warning: Viseme '{1}' already used by VisemeTextures[{2}].", i, viseme, this._textureLookup[viseme]));
				}
				else
				{
					this._textureLookup[viseme] = i;
				}
			}
			this.CheckForMissingVisemes(stringBuilder);
			if (stringBuilder.Length > 0)
			{
				VLog.E(base.GetType().Name, string.Format("Setup Warnings:\n{0}", stringBuilder), null);
			}
		}

		private void CheckForMissingVisemes(StringBuilder log)
		{
			foreach (object obj in Enum.GetValues(typeof(Viseme)))
			{
				Viseme viseme = (Viseme)obj;
				if (!this._textureLookup.ContainsKey(viseme))
				{
					log.AppendLine(string.Format("{0} Viseme missing texture", viseme));
				}
			}
		}

		private void SetViseme(Viseme v)
		{
			if (!this._textureLookup.ContainsKey(v))
			{
				if (v != Viseme.sil)
				{
					this.SetViseme(Viseme.sil);
				}
				return;
			}
			int num = this._textureLookup[v];
			int num2 = 0;
			BaseTextureFlipLipSync.VisemeTextureData visemeTextureData = this.VisemeTextures[num];
			if (visemeTextureData.textures.Length > 1)
			{
				num2 = Random.Range(0, visemeTextureData.textures.Length);
			}
			this.SetTexture(visemeTextureData.textures[num2]);
		}

		protected virtual void SetTexture(Texture2D texture)
		{
			if (!this.Renderer)
			{
				return;
			}
			this.Renderer.material.SetTexture("_MainTex", texture);
		}

		public void OnVisemeStarted(Viseme viseme)
		{
			this.SetViseme(viseme);
		}

		public void OnVisemeFinished(Viseme viseme)
		{
		}

		public void OnVisemeLerp(Viseme oldVieseme, Viseme newViseme, float percentage)
		{
		}

		private readonly IVLogger _log = LoggerRegistry.Instance.GetLogger(LogCategory.TextToSpeech, null);

		[Header("Texture Settings")]
		public BaseTextureFlipLipSync.VisemeTextureData[] VisemeTextures;

		private Dictionary<Viseme, int> _textureLookup = new Dictionary<Viseme, int>();

		[Serializable]
		public struct VisemeTextureData
		{
			public Viseme viseme;

			public Texture2D[] textures;
		}
	}
}

using System;
using System.Collections;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ActiveStateFingerVisual : MonoBehaviour
	{
		public HandFingerFlags FingersMask
		{
			get
			{
				return this._fingersMask;
			}
			set
			{
				this._fingersMask = value;
			}
		}

		public float GlowLerpSpeed
		{
			get
			{
				return this._glowLerpSpeed;
			}
			set
			{
				this._glowLerpSpeed = value;
			}
		}

		public Color FingerGlowColor
		{
			get
			{
				return this._fingerGlowColor;
			}
			set
			{
				this._fingerGlowColor = value;
			}
		}

		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		private void Update()
		{
			if (this._prevActive == this.ActiveState.Active)
			{
				return;
			}
			base.StopAllCoroutines();
			this._prevActive = this.ActiveState.Active;
			this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetColor(this._fingerGlowColorPropertyId, this._fingerGlowColor);
			float targetGlow = this.ActiveState.Active ? 1f : 0f;
			for (int i = 0; i < 5; i++)
			{
				if (this._fingersMask.HasFlag((HandFingerFlags)(1 << i)))
				{
					base.StartCoroutine(this.UpdateGlowValue(i, targetGlow));
				}
			}
			this._handMaterialPropertyBlockEditor.UpdateMaterialPropertyBlock();
		}

		private IEnumerator UpdateGlowValue(int fingerIndex, float targetGlow)
		{
			float startGlow = this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.GetFloat(this._handShaderGlowPropertyIds[fingerIndex]);
			float startTime = Time.time;
			float currentGlow = startGlow;
			do
			{
				currentGlow = Mathf.MoveTowards(startGlow, targetGlow, this._glowLerpSpeed * (Time.time - startTime));
				this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._handShaderGlowPropertyIds[fingerIndex], currentGlow);
				yield return null;
			}
			while (currentGlow != targetGlow);
			yield break;
		}

		public void InjectAllActiveStateFingerVisual(IActiveState activeState, MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
		{
			this.InjectActiveState(activeState);
			this.InjectHandMaterialPropertyBlockEditor(handMaterialPropertyBlockEditor);
		}

		public void InjectActiveState(IActiveState activeState)
		{
			this.ActiveState = activeState;
			this._activeState = (activeState as Object);
		}

		public void InjectHandMaterialPropertyBlockEditor(MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
		{
			this._handMaterialPropertyBlockEditor = handMaterialPropertyBlockEditor;
		}

		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		private IActiveState ActiveState;

		[SerializeField]
		private HandFingerFlags _fingersMask;

		[SerializeField]
		private MaterialPropertyBlockEditor _handMaterialPropertyBlockEditor;

		[SerializeField]
		private float _glowLerpSpeed = 2f;

		[SerializeField]
		private Color _fingerGlowColor;

		private readonly int[] _handShaderGlowPropertyIds = new int[]
		{
			Shader.PropertyToID("_ThumbGlowValue"),
			Shader.PropertyToID("_IndexGlowValue"),
			Shader.PropertyToID("_MiddleGlowValue"),
			Shader.PropertyToID("_RingGlowValue"),
			Shader.PropertyToID("_PinkyGlowValue")
		};

		private readonly int _fingerGlowColorPropertyId = Shader.PropertyToID("_FingerGlowColor");

		private bool _prevActive;

		protected bool _started;
	}
}

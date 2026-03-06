using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandPokeOvershootGlow : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this._glowEnabled = false;
			this.BeginStart(ref this._started, null);
			HandFingerMaskGenerator.GenerateFingerMask(this._handRenderer, this._handVisual, this._materialEditor.MaterialPropertyBlock);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._pokeInteractor.WhenPostprocessed += this.UpdateVisual;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._pokeInteractor.WhenPostprocessed -= this.UpdateVisual;
			}
		}

		private void UpdateOvershoot(float normalizedDistance)
		{
			if (this._materialEditor == null)
			{
				return;
			}
			MaterialPropertyBlock materialPropertyBlock = this._materialEditor.MaterialPropertyBlock;
			materialPropertyBlock.SetFloat(this._glowParameterID, Mathf.Clamp01(normalizedDistance));
			materialPropertyBlock.SetInt(this._generateGlowID, 1);
			materialPropertyBlock.SetColor(this._glowColorID, this._glowColor);
			materialPropertyBlock.SetInt(this._glowTypeID, (int)this._glowType);
			materialPropertyBlock.SetInt(this._glowFingerIndexID, (int)this._pokeFinger);
			materialPropertyBlock.SetFloat(this._glowMaxLengthID, this._maxGradientLength);
		}

		private void UpdateVisual()
		{
			if (this._pokeInteractor.State == InteractorState.Select)
			{
				this._glowEnabled = true;
				Vector3 touchPoint = this._pokeInteractor.TouchPoint;
				Vector3 origin = this._pokeInteractor.Origin;
				float normalizedDistance = Mathf.Clamp01(Vector3.Distance(touchPoint, origin) / this._overshootMaxDistance);
				this.UpdateOvershoot(normalizedDistance);
				return;
			}
			if (this._glowEnabled)
			{
				if (this._materialEditor == null)
				{
					return;
				}
				this._materialEditor.MaterialPropertyBlock.SetInt(this._generateGlowID, 0);
				this._glowEnabled = false;
			}
		}

		public void InjectAllHandPokeOvershootGlow(IHand hand, PokeInteractor pokeInteractor, MaterialPropertyBlockEditor materialEditor, Color glowColor, float distanceMultiplier, Transform wristTransform, HandPokeOvershootGlow.GlowType glowType)
		{
			this.InjectHand(hand);
			this.InjectPokeInteractor(pokeInteractor);
			this.InjectMaterialPropertyBlockEditor(materialEditor);
			this.InjectGlowColor(glowColor);
			this.InjectOvershootMaxDistance(distanceMultiplier);
			this.InjectGlowType(glowType);
		}

		public void InjectAllHandPokeOvershootGlow(IHand hand, PokeInteractor pokeInteractor, HandVisual handVisual, SkinnedMeshRenderer handRenderer, MaterialPropertyBlockEditor materialEditor)
		{
			this.InjectHand(hand);
			this.InjectPokeInteractor(pokeInteractor);
			this.InjectHandVisual(handVisual);
			this.InjectHandRenderer(handRenderer);
			this.InjectMaterialPropertyBlockEditor(materialEditor);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectPokeInteractor(PokeInteractor pokeInteractor)
		{
			this._pokeInteractor = pokeInteractor;
		}

		public void InjectHandRenderer(SkinnedMeshRenderer handRenderer)
		{
			this._handRenderer = handRenderer;
		}

		public void InjectHandVisual(HandVisual handVisual)
		{
			this._handVisual = handVisual;
		}

		public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor materialEditor)
		{
			this._materialEditor = materialEditor;
		}

		public void InjectGlowColor(Color glowColor)
		{
			this._glowColor = glowColor;
		}

		public void InjectOvershootMaxDistance(float overshootMaxDistance)
		{
			this._overshootMaxDistance = overshootMaxDistance;
		}

		public void InjectGlowType(HandPokeOvershootGlow.GlowType glowType)
		{
			this._glowType = glowType;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private PokeInteractor _pokeInteractor;

		[SerializeField]
		private HandVisual _handVisual;

		[SerializeField]
		private SkinnedMeshRenderer _handRenderer;

		[SerializeField]
		private MaterialPropertyBlockEditor _materialEditor;

		[SerializeField]
		private Color _glowColor;

		[SerializeField]
		private float _overshootMaxDistance = 0.15f;

		[SerializeField]
		private HandFinger _pokeFinger = HandFinger.Index;

		[SerializeField]
		[Range(0f, 1f)]
		private float _maxGradientLength;

		[SerializeField]
		private HandPokeOvershootGlow.GlowType _glowType = HandPokeOvershootGlow.GlowType.Outline;

		private IHand Hand;

		private bool _glowEnabled;

		private readonly int _glowFingerIndexID = Shader.PropertyToID("_FingerGlowIndex");

		private readonly int _generateGlowID = Shader.PropertyToID("_GenerateGlow");

		private readonly int _glowColorID = Shader.PropertyToID("_GlowColor");

		private readonly int _glowTypeID = Shader.PropertyToID("_GlowType");

		private readonly int _glowParameterID = Shader.PropertyToID("_GlowParameter");

		private readonly int _glowMaxLengthID = Shader.PropertyToID("_GlowMaxLength");

		protected bool _started;

		public enum GlowType
		{
			Fill = 30,
			Outline,
			Both
		}
	}
}

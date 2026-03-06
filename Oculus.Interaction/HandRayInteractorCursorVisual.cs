using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandRayInteractorCursorVisual : MonoBehaviour
	{
		public Transform PlayerHead
		{
			get
			{
				return this._playerHead;
			}
			set
			{
				this._playerHead = value;
				if (this._started && value == null)
				{
					base.transform.localScale = this._startScale;
				}
			}
		}

		public Color OutlineColor
		{
			get
			{
				return this._outlineColor;
			}
			set
			{
				this._outlineColor = value;
			}
		}

		public float OffsetAlongNormal
		{
			get
			{
				return this._offsetAlongNormal;
			}
			set
			{
				this._offsetAlongNormal = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.Hand = (this._hand as IHand);
			this._startScale = base.transform.localScale;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed += this.UpdateVisual;
				this._rayInteractor.WhenStateChanged += this.UpdateVisualState;
				this.UpdateVisual();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed -= this.UpdateVisual;
				this._rayInteractor.WhenStateChanged -= this.UpdateVisualState;
			}
		}

		private void UpdateVisual()
		{
			if (this._rayInteractor.State == InteractorState.Disabled)
			{
				this._cursor.SetActive(false);
				return;
			}
			if (this._rayInteractor.CollisionInfo == null)
			{
				this._cursor.SetActive(false);
				return;
			}
			if (!this._cursor.activeSelf)
			{
				this._cursor.SetActive(true);
			}
			Vector3 normal = this._rayInteractor.CollisionInfo.Value.Normal;
			base.transform.position = this._rayInteractor.End + normal * this._offsetAlongNormal;
			base.transform.rotation = Quaternion.LookRotation(this._rayInteractor.CollisionInfo.Value.Normal, Vector3.up);
			if (this.PlayerHead != null)
			{
				float d = Vector3.Distance(base.transform.position, this.PlayerHead.position);
				base.transform.localScale = this._startScale * d;
			}
			if (this._rayInteractor.State == InteractorState.Select)
			{
				this._selectObject.SetActive(true);
				this._renderer.material.SetFloat(this._shaderRadialGradientScale, 0.7f);
				this._renderer.material.SetFloat(this._shaderRadialGradientIntensity, 1f);
				this._renderer.material.SetFloat(this._shaderRadialGradientBackgroundOpacity, 1f);
				this._renderer.material.SetColor(this._shaderOutlineColor, this._outlineColor);
				return;
			}
			this._selectObject.SetActive(false);
			float fingerPinchStrength = this.Hand.GetFingerPinchStrength(HandFinger.Index);
			float num = 1f - fingerPinchStrength;
			num = Mathf.Max(num, 0.7f);
			this._renderer.material.SetFloat(this._shaderRadialGradientScale, num);
			this._renderer.material.SetFloat(this._shaderRadialGradientIntensity, fingerPinchStrength);
			this._renderer.material.SetFloat(this._shaderRadialGradientBackgroundOpacity, Mathf.Lerp(0.7f, 1f, fingerPinchStrength));
			this._renderer.material.SetColor(this._shaderOutlineColor, this._outlineColor);
		}

		private void UpdateVisualState(InteractorStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		public void InjectAllHandRayInteractorCursorVisual(IHand hand, RayInteractor rayInteractor, GameObject cursor, Renderer renderer)
		{
			this.InjectHand(hand);
			this.InjectRayInteractor(rayInteractor);
			this.InjectCursor(cursor);
			this.InjectRenderer(renderer);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectRayInteractor(RayInteractor rayInteractor)
		{
			this._rayInteractor = rayInteractor;
		}

		public void InjectCursor(GameObject cursor)
		{
			this._cursor = cursor;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;

		[SerializeField]
		private RayInteractor _rayInteractor;

		[SerializeField]
		private GameObject _cursor;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Color _outlineColor = Color.black;

		[SerializeField]
		private float _offsetAlongNormal = 0.005f;

		[Tooltip("Players head transform, used to maintain the same cursor size on screen as it is moved in the scene.")]
		[SerializeField]
		[Optional]
		private Transform _playerHead;

		private Vector3 _startScale;

		private int _shaderRadialGradientScale = Shader.PropertyToID("_RadialGradientScale");

		private int _shaderRadialGradientIntensity = Shader.PropertyToID("_RadialGradientIntensity");

		private int _shaderRadialGradientBackgroundOpacity = Shader.PropertyToID("_RadialGradientBackgroundOpacity");

		private int _shaderOutlineColor = Shader.PropertyToID("_OutlineColor");

		[SerializeField]
		private GameObject _selectObject;

		protected bool _started;
	}
}

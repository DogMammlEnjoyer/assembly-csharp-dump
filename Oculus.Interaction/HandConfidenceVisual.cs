using System;
using System.Runtime.CompilerServices;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandConfidenceVisual : MonoBehaviour
	{
		private IHand Hand { get; set; }

		public float Speed
		{
			get
			{
				return this._speed;
			}
			set
			{
				this._speed = value;
			}
		}

		private void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._lastTime = Time.time;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.UpdateVisual;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.UpdateVisual;
			}
		}

		private void UpdateVisual()
		{
			HandConfidenceVisual.<>c__DisplayClass18_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.changeRate = (Time.time - this._lastTime) * this.Speed;
			this._lastTime = Time.time;
			float b = this.Hand.IsHighConfidence ? 0f : 1f;
			this._jointsConfidence[0] = Mathf.Lerp(this._jointsConfidence[0], b, CS$<>8__locals1.changeRate);
			this.<UpdateVisual>g__FillConfidence|18_0(HandFinger.Thumb, 1, 4, ref CS$<>8__locals1);
			this.<UpdateVisual>g__FillConfidence|18_0(HandFinger.Index, 5, 3, ref CS$<>8__locals1);
			this.<UpdateVisual>g__FillConfidence|18_0(HandFinger.Middle, 8, 3, ref CS$<>8__locals1);
			this.<UpdateVisual>g__FillConfidence|18_0(HandFinger.Ring, 11, 3, ref CS$<>8__locals1);
			this.<UpdateVisual>g__FillConfidence|18_0(HandFinger.Pinky, 14, 4, ref CS$<>8__locals1);
			this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetFloatArray(this._handConfidenceId, this._jointsConfidence);
			this._handMaterialPropertyBlockEditor.UpdateMaterialPropertyBlock();
		}

		public void InjectAllHandConfidenceVisual(IHand hand, MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
		{
			this.InjectHand(hand);
			this.InjectHandMaterialPropertyBlockEditor(handMaterialPropertyBlockEditor);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectHandMaterialPropertyBlockEditor(MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
		{
			this._handMaterialPropertyBlockEditor = handMaterialPropertyBlockEditor;
		}

		[CompilerGenerated]
		private void <UpdateVisual>g__FillConfidence|18_0(HandFinger finger, int offset, int lenght, ref HandConfidenceVisual.<>c__DisplayClass18_0 A_4)
		{
			int num = this.Hand.GetFingerIsHighConfidence(finger) ? 0 : 1;
			for (int i = offset; i < offset + lenght; i++)
			{
				this._jointsConfidence[i] = Mathf.Lerp(this._jointsConfidence[i], (float)num, A_4.changeRate);
			}
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private MaterialPropertyBlockEditor _handMaterialPropertyBlockEditor;

		[SerializeField]
		private float _speed = 5f;

		private readonly int _handConfidenceId = Shader.PropertyToID("_JointsGlow");

		private float[] _jointsConfidence = new float[18];

		protected bool _started;

		private float _lastTime;
	}
}

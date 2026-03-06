using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class ShapeRecognizerActiveState : MonoBehaviour, IActiveState
	{
		public IHand Hand { get; private set; }

		public IReadOnlyList<ShapeRecognizer> Shapes
		{
			get
			{
				return this._shapes;
			}
		}

		public Handedness Handedness
		{
			get
			{
				return this.Hand.Handedness;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.FingerFeatureStateProvider = (this._fingerFeatureStateProvider as IFingerFeatureStateProvider);
		}

		protected virtual void Start()
		{
			this._allFingerStates = this.FlattenUsedFeatures();
			this.InitStateProvider();
		}

		private void InitStateProvider()
		{
			foreach (ShapeRecognizerActiveState.FingerFeatureStateUsage fingerFeatureStateUsage in this._allFingerStates)
			{
				string text;
				this.FingerFeatureStateProvider.GetCurrentState(fingerFeatureStateUsage.handFinger, fingerFeatureStateUsage.config.Feature, out text);
			}
		}

		private List<ShapeRecognizerActiveState.FingerFeatureStateUsage> FlattenUsedFeatures()
		{
			List<ShapeRecognizerActiveState.FingerFeatureStateUsage> list = new List<ShapeRecognizerActiveState.FingerFeatureStateUsage>();
			foreach (ShapeRecognizer shapeRecognizer in this._shapes)
			{
				int num = 0;
				for (int j = 0; j < 5; j++)
				{
					HandFinger handFinger = (HandFinger)j;
					foreach (ShapeRecognizer.FingerFeatureConfig config in shapeRecognizer.GetFingerFeatureConfigs(handFinger))
					{
						num++;
						list.Add(new ShapeRecognizerActiveState.FingerFeatureStateUsage
						{
							handFinger = handFinger,
							config = config
						});
					}
				}
			}
			return list;
		}

		public bool Active
		{
			get
			{
				if (!base.isActiveAndEnabled || this._allFingerStates.Count == 0)
				{
					bool result = this._nativeActive = false;
					return result;
				}
				foreach (ShapeRecognizerActiveState.FingerFeatureStateUsage fingerFeatureStateUsage in this._allFingerStates)
				{
					if (!this.FingerFeatureStateProvider.IsStateActive(fingerFeatureStateUsage.handFinger, fingerFeatureStateUsage.config.Feature, fingerFeatureStateUsage.config.Mode, fingerFeatureStateUsage.config.State))
					{
						bool result = this._nativeActive = false;
						return result;
					}
				}
				if (!this._nativeActive)
				{
					NativeMethods.isdk_NativeComponent_Activate(5210787310278567284UL);
				}
				return this._nativeActive = true;
			}
		}

		public void InjectAllShapeRecognizerActiveState(IHand hand, IFingerFeatureStateProvider fingerFeatureStateProvider, ShapeRecognizer[] shapes)
		{
			this.InjectHand(hand);
			this.InjectFingerFeatureStateProvider(fingerFeatureStateProvider);
			this.InjectShapes(shapes);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectFingerFeatureStateProvider(IFingerFeatureStateProvider fingerFeatureStateProvider)
		{
			this._fingerFeatureStateProvider = (fingerFeatureStateProvider as Object);
			this.FingerFeatureStateProvider = fingerFeatureStateProvider;
		}

		public void InjectShapes(ShapeRecognizer[] shapes)
		{
			this._shapes = shapes;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[Interface(typeof(IFingerFeatureStateProvider), new Type[]
		{

		})]
		private Object _fingerFeatureStateProvider;

		protected IFingerFeatureStateProvider FingerFeatureStateProvider;

		[SerializeField]
		private ShapeRecognizer[] _shapes;

		private List<ShapeRecognizerActiveState.FingerFeatureStateUsage> _allFingerStates = new List<ShapeRecognizerActiveState.FingerFeatureStateUsage>();

		private bool _nativeActive;

		private struct FingerFeatureStateUsage
		{
			public HandFinger handFinger;

			public ShapeRecognizer.FingerFeatureConfig config;
		}
	}
}

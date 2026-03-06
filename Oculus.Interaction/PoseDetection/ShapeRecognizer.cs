using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[CreateAssetMenu(menuName = "Meta/Interaction/SDK/Pose Detection/Shape")]
	public class ShapeRecognizer : ScriptableObject
	{
		public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> ThumbFeatureConfigs
		{
			get
			{
				return this._thumbFeatureConfigs.Value;
			}
		}

		public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> IndexFeatureConfigs
		{
			get
			{
				return this._indexFeatureConfigs.Value;
			}
		}

		public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> MiddleFeatureConfigs
		{
			get
			{
				return this._middleFeatureConfigs.Value;
			}
		}

		public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> RingFeatureConfigs
		{
			get
			{
				return this._ringFeatureConfigs.Value;
			}
		}

		public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> PinkyFeatureConfigs
		{
			get
			{
				return this._pinkyFeatureConfigs.Value;
			}
		}

		public string ShapeName
		{
			get
			{
				return this._shapeName;
			}
		}

		public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> GetFingerFeatureConfigs(HandFinger finger)
		{
			switch (finger)
			{
			case HandFinger.Thumb:
				return this.ThumbFeatureConfigs;
			case HandFinger.Index:
				return this.IndexFeatureConfigs;
			case HandFinger.Middle:
				return this.MiddleFeatureConfigs;
			case HandFinger.Ring:
				return this.RingFeatureConfigs;
			case HandFinger.Pinky:
				return this.PinkyFeatureConfigs;
			default:
				throw new ArgumentException("must be a HandFinger enum value", "finger");
			}
		}

		public IEnumerable<ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>>> GetFingerFeatureConfigs()
		{
			int num;
			for (int fingerIdx = 0; fingerIdx < 5; fingerIdx = num)
			{
				HandFinger handFinger = (HandFinger)fingerIdx;
				IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> fingerFeatureConfigs = this.GetFingerFeatureConfigs(handFinger);
				if (fingerFeatureConfigs.Count != 0)
				{
					yield return new ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>>(handFinger, fingerFeatureConfigs);
				}
				num = fingerIdx + 1;
			}
			yield break;
		}

		public void InjectAllShapeRecognizer(IDictionary<HandFinger, ShapeRecognizer.FingerFeatureConfig[]> fingerFeatureConfigs)
		{
			ShapeRecognizer.<>c__DisplayClass22_0 CS$<>8__locals1;
			CS$<>8__locals1.fingerFeatureConfigs = fingerFeatureConfigs;
			this._thumbFeatureConfigs = ShapeRecognizer.<InjectAllShapeRecognizer>g__ReadFeatureConfigs|22_0(HandFinger.Thumb, ref CS$<>8__locals1);
			this._indexFeatureConfigs = ShapeRecognizer.<InjectAllShapeRecognizer>g__ReadFeatureConfigs|22_0(HandFinger.Index, ref CS$<>8__locals1);
			this._middleFeatureConfigs = ShapeRecognizer.<InjectAllShapeRecognizer>g__ReadFeatureConfigs|22_0(HandFinger.Middle, ref CS$<>8__locals1);
			this._ringFeatureConfigs = ShapeRecognizer.<InjectAllShapeRecognizer>g__ReadFeatureConfigs|22_0(HandFinger.Ring, ref CS$<>8__locals1);
			this._pinkyFeatureConfigs = ShapeRecognizer.<InjectAllShapeRecognizer>g__ReadFeatureConfigs|22_0(HandFinger.Pinky, ref CS$<>8__locals1);
		}

		public void InjectThumbFeatureConfigs(ShapeRecognizer.FingerFeatureConfig[] configs)
		{
			this._thumbFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList(new List<ShapeRecognizer.FingerFeatureConfig>(configs));
		}

		public void InjectIndexFeatureConfigs(ShapeRecognizer.FingerFeatureConfig[] configs)
		{
			this._indexFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList(new List<ShapeRecognizer.FingerFeatureConfig>(configs));
		}

		public void InjectMiddleFeatureConfigs(ShapeRecognizer.FingerFeatureConfig[] configs)
		{
			this._middleFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList(new List<ShapeRecognizer.FingerFeatureConfig>(configs));
		}

		public void InjectRingFeatureConfigs(ShapeRecognizer.FingerFeatureConfig[] configs)
		{
			this._ringFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList(new List<ShapeRecognizer.FingerFeatureConfig>(configs));
		}

		public void InjectPinkyFeatureConfigs(ShapeRecognizer.FingerFeatureConfig[] configs)
		{
			this._pinkyFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList(new List<ShapeRecognizer.FingerFeatureConfig>(configs));
		}

		public void InjectShapeName(string shapeName)
		{
			this._shapeName = shapeName;
		}

		[CompilerGenerated]
		internal static ShapeRecognizer.FingerFeatureConfigList <InjectAllShapeRecognizer>g__ReadFeatureConfigs|22_0(HandFinger finger, ref ShapeRecognizer.<>c__DisplayClass22_0 A_1)
		{
			ShapeRecognizer.FingerFeatureConfig[] collection;
			if (!A_1.fingerFeatureConfigs.TryGetValue(finger, out collection))
			{
				collection = Array.Empty<ShapeRecognizer.FingerFeatureConfig>();
			}
			return new ShapeRecognizer.FingerFeatureConfigList(new List<ShapeRecognizer.FingerFeatureConfig>(collection));
		}

		[SerializeField]
		private string _shapeName;

		[SerializeField]
		private ShapeRecognizer.FingerFeatureConfigList _thumbFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList();

		[SerializeField]
		private ShapeRecognizer.FingerFeatureConfigList _indexFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList();

		[SerializeField]
		private ShapeRecognizer.FingerFeatureConfigList _middleFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList();

		[SerializeField]
		private ShapeRecognizer.FingerFeatureConfigList _ringFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList();

		[SerializeField]
		private ShapeRecognizer.FingerFeatureConfigList _pinkyFeatureConfigs = new ShapeRecognizer.FingerFeatureConfigList();

		[Serializable]
		public class FingerFeatureConfigList
		{
			public IReadOnlyList<ShapeRecognizer.FingerFeatureConfig> Value
			{
				get
				{
					return this._value;
				}
			}

			public FingerFeatureConfigList()
			{
			}

			public FingerFeatureConfigList(List<ShapeRecognizer.FingerFeatureConfig> value)
			{
				this._value = value;
			}

			[SerializeField]
			private List<ShapeRecognizer.FingerFeatureConfig> _value;
		}

		[Serializable]
		public class FingerFeatureConfig : FeatureConfigBase<FingerFeature>
		{
		}
	}
}

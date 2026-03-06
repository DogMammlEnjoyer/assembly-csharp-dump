using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class OneEuroFilter : IOneEuroFilter<float>
	{
		public float Value { get; private set; }

		private OneEuroFilter()
		{
			this._xfilt = new OneEuroFilter.LowPassFilter();
			this._dxfilt = new OneEuroFilter.LowPassFilter();
			this._isFirstUpdate = true;
			OneEuroFilterPropertyBlock @default = OneEuroFilterPropertyBlock.Default;
			this.SetProperties(@default);
		}

		public void SetProperties(in OneEuroFilterPropertyBlock properties)
		{
			this._properties = properties;
		}

		public float Step(float newValue, float deltaTime)
		{
			if (deltaTime > 0f)
			{
				float num = 1f / deltaTime;
				float x = this._isFirstUpdate ? 0f : ((newValue - this._xfilt.PrevValue) * num);
				this._isFirstUpdate = false;
				float f = this._dxfilt.Filter(x, this.GetAlpha(num, this._properties.DCutoff));
				float cutoff = this._properties.MinCutoff + this._properties.Beta * Mathf.Abs(f);
				this.Value = this._xfilt.Filter(newValue, this.GetAlpha(num, cutoff));
			}
			return this.Value;
		}

		public void Reset()
		{
			this.Value = 0f;
			this._xfilt.Reset();
			this._dxfilt.Reset();
			this._isFirstUpdate = true;
		}

		private float GetAlpha(float rate, float cutoff)
		{
			float num = 1f / (6.2831855f * cutoff);
			float num2 = 1f / rate;
			return 1f / (1f + num / num2);
		}

		public static IOneEuroFilter<float> CreateFloat()
		{
			return new OneEuroFilter();
		}

		public static IOneEuroFilter<Vector2> CreateVector2()
		{
			return new OneEuroFilter.OneEuroFilterMulti<Vector2>(2, (float[] values) => new Vector2(values[0], values[1]), (Vector2 value, int index) => value[index]);
		}

		public static IOneEuroFilter<Vector3> CreateVector3()
		{
			return new OneEuroFilter.OneEuroFilterMulti<Vector3>(3, (float[] values) => new Vector3(values[0], values[1], values[2]), (Vector3 value, int index) => value[index]);
		}

		public static IOneEuroFilter<Vector4> CreateVector4()
		{
			return new OneEuroFilter.OneEuroFilterMulti<Vector4>(4, (float[] values) => new Vector4(values[0], values[1], values[2], values[3]), (Vector4 value, int index) => value[index]);
		}

		public static IOneEuroFilter<Quaternion> CreateQuaternion()
		{
			return new OneEuroFilter.OneEuroFilterMulti<Quaternion>(4, (float[] values) => new Quaternion(values[0], values[1], values[2], values[3]).normalized, (Quaternion value, int index) => value[index]);
		}

		public static IOneEuroFilter<Pose> CreatePose()
		{
			return new OneEuroFilter.OneEuroFilterMulti<Pose>(7, (float[] values) => new Pose(new Vector3(values[0], values[1], values[2]), new Quaternion(values[3], values[4], values[5], values[6]).normalized), delegate(Pose value, int index)
			{
				if (index <= 2)
				{
					return value.position[index];
				}
				return value.rotation[index - 3];
			});
		}

		void IOneEuroFilter<float>.SetProperties(in OneEuroFilterPropertyBlock properties)
		{
			this.SetProperties(properties);
		}

		public const float _DEFAULT_FREQUENCY_HZ = 60f;

		private OneEuroFilterPropertyBlock _properties;

		private bool _isFirstUpdate;

		private OneEuroFilter.LowPassFilter _xfilt;

		private OneEuroFilter.LowPassFilter _dxfilt;

		private class LowPassFilter
		{
			public float PrevValue
			{
				get
				{
					return this._hatxprev;
				}
			}

			public LowPassFilter()
			{
				this._isFirstUpdate = true;
			}

			public void Reset()
			{
				this._isFirstUpdate = true;
				this._hatx = (this._hatxprev = 0f);
			}

			public float Filter(float x, float alpha)
			{
				if (this._isFirstUpdate)
				{
					this._isFirstUpdate = false;
					this._hatxprev = x;
				}
				this._hatx = alpha * x + (1f - alpha) * this._hatxprev;
				this._hatxprev = this._hatx;
				return this._hatx;
			}

			private bool _isFirstUpdate;

			private float _hatx;

			private float _hatxprev;
		}

		private class OneEuroFilterMulti<TData> : IOneEuroFilter<TData>
		{
			public TData Value { get; private set; }

			public OneEuroFilterMulti(int numComponents, Func<float[], TData> arrayToType, Func<TData, int, float> getValAtIndex)
			{
				IOneEuroFilter<float>[] filters = new OneEuroFilter[numComponents];
				this._filters = filters;
				this._componentValues = new float[numComponents];
				this._arrayToType = arrayToType;
				this._getValAtIndex = getValAtIndex;
				for (int i = 0; i < this._filters.Length; i++)
				{
					this._filters[i] = new OneEuroFilter();
				}
			}

			public void SetProperties(in OneEuroFilterPropertyBlock properties)
			{
				IOneEuroFilter<float>[] filters = this._filters;
				for (int i = 0; i < filters.Length; i++)
				{
					filters[i].SetProperties(properties);
				}
			}

			public TData Step(TData newValue, float deltaTime)
			{
				for (int i = 0; i < this._filters.Length; i++)
				{
					float rawValue = this._getValAtIndex(newValue, i);
					this._componentValues[i] = this._filters[i].Step(rawValue, deltaTime);
				}
				this.Value = this._arrayToType(this._componentValues);
				return this.Value;
			}

			public void Reset()
			{
				IOneEuroFilter<float>[] filters = this._filters;
				for (int i = 0; i < filters.Length; i++)
				{
					filters[i].Reset();
				}
			}

			void IOneEuroFilter<!0>.SetProperties(in OneEuroFilterPropertyBlock properties)
			{
				this.SetProperties(properties);
			}

			private readonly Func<float[], TData> _arrayToType;

			private readonly Func<TData, int, float> _getValAtIndex;

			private readonly IOneEuroFilter<float>[] _filters;

			private readonly float[] _componentValues;
		}
	}
}

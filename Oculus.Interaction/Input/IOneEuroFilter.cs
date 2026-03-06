using System;

namespace Oculus.Interaction.Input
{
	public interface IOneEuroFilter<TData>
	{
		TData Value { get; }

		void SetProperties(in OneEuroFilterPropertyBlock properties);

		TData Step(TData rawValue, float deltaTime = 0.016666668f);

		void Reset();
	}
}

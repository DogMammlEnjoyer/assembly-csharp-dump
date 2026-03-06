using System;

namespace gs
{
	public interface IFalloffFunction
	{
		double FalloffT(double t);

		IFalloffFunction Duplicate();
	}
}

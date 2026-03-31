using System;

namespace Cysharp.Threading.Tasks
{
	public interface IUniTaskSource<out T> : IUniTaskSource
	{
		T GetResult(short token);
	}
}

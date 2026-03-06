using System;

namespace g3
{
	public class MeshBoundaryLoopsException : Exception
	{
		public MeshBoundaryLoopsException(string message) : base(message)
		{
		}

		public bool UnclosedLoop;

		public bool BowtieFailure;

		public bool RepeatedEdge;
	}
}

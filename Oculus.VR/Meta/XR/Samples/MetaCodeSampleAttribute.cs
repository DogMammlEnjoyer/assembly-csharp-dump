using System;

namespace Meta.XR.Samples
{
	public class MetaCodeSampleAttribute : Attribute
	{
		public MetaCodeSampleAttribute(string sampleName)
		{
			this.SampleName = sampleName;
		}

		public string SampleName { get; private set; }
	}
}

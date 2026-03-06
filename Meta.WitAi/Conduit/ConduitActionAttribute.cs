using System;
using System.Collections.Generic;
using System.Linq;

namespace Meta.Conduit
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ConduitActionAttribute : Attribute
	{
		public string Intent { get; private set; }

		public float MinConfidence { get; protected set; }

		public float MaxConfidence { get; protected set; }

		public List<string> Aliases { get; private set; }

		public bool ValidatePartial { get; private set; }

		protected ConduitActionAttribute(string intent = "", params string[] aliases)
		{
			this.Intent = intent;
			this.Aliases = aliases.ToList<string>();
		}

		protected ConduitActionAttribute(string intent = "", float minConfidence = 0.51f, float maxConfidence = 1f, bool validatePartial = false, params string[] aliases)
		{
			this.Intent = intent;
			this.MinConfidence = minConfidence;
			this.MaxConfidence = maxConfidence;
			this.ValidatePartial = validatePartial;
			this.Aliases = aliases.ToList<string>();
		}

		protected const float DEFAULT_MIN_CONFIDENCE = 0.51f;

		protected const float DEFAULT_MAX_CONFIDENCE = 1f;
	}
}

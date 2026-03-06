using System;
using System.Collections.Generic;

namespace Meta.XR.MRUtilityKit
{
	public struct LabelFilter
	{
		public LabelFilter(MRUKAnchor.SceneLabels? labelFlags = null, MRUKAnchor.ComponentType? componentTypes = null)
		{
			this.SceneLabels = labelFlags;
			this.ComponentTypes = componentTypes;
		}

		[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
		public static LabelFilter Included(List<string> included)
		{
			return LabelFilter.Included(Utilities.StringLabelsToEnum(included));
		}

		[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
		public static LabelFilter Excluded(List<string> excluded)
		{
			return LabelFilter.Excluded(Utilities.StringLabelsToEnum(excluded));
		}

		[Obsolete("Use 'Included()' instead.")]
		public static LabelFilter FromEnum(MRUKAnchor.SceneLabels labels)
		{
			return LabelFilter.Included(labels);
		}

		[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
		public bool PassesFilter(List<string> labels)
		{
			return this.PassesFilter(Utilities.StringLabelsToEnum(labels));
		}

		[Obsolete("Use `new LabelFilter(labelFlags)` instead")]
		public static LabelFilter Included(MRUKAnchor.SceneLabels labelFlags)
		{
			return new LabelFilter(new MRUKAnchor.SceneLabels?(labelFlags), null);
		}

		[Obsolete("Use `new LabelFilter(~labelFlags)` instead")]
		public static LabelFilter Excluded(MRUKAnchor.SceneLabels labelFlags)
		{
			return new LabelFilter
			{
				SceneLabels = new MRUKAnchor.SceneLabels?(~labelFlags),
				ComponentTypes = null
			};
		}

		public bool PassesFilter(MRUKAnchor.SceneLabels labelFlags)
		{
			return this.SceneLabels == null || (this.SceneLabels.Value & labelFlags) > (MRUKAnchor.SceneLabels)0;
		}

		public MRUKAnchor.SceneLabels? SceneLabels;

		public MRUKAnchor.ComponentType? ComponentTypes;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class TagSetFilter : MonoBehaviour, IGameObjectFilter
	{
		protected virtual void Start()
		{
			foreach (string item in this._requireTags)
			{
				this._requireTagSet.Add(item);
			}
			foreach (string item2 in this._excludeTags)
			{
				this._excludeTagSet.Add(item2);
			}
		}

		public bool Filter(GameObject gameObject)
		{
			TagSet tagSet;
			bool flag = gameObject.TryGetComponent<TagSet>(out tagSet);
			if (!flag && this._requireTagSet.Count > 0)
			{
				return false;
			}
			foreach (string tag in this._requireTagSet)
			{
				if (!tagSet.ContainsTag(tag))
				{
					return false;
				}
			}
			if (!flag)
			{
				return true;
			}
			foreach (string tag2 in this._excludeTagSet)
			{
				if (tagSet.ContainsTag(tag2))
				{
					return false;
				}
			}
			return true;
		}

		public bool ContainsRequireTag(string tag)
		{
			return this._requireTagSet.Contains(tag);
		}

		public void AddRequireTag(string tag)
		{
			this._requireTagSet.Add(tag);
		}

		public void RemoveRequireTag(string tag)
		{
			this._requireTagSet.Remove(tag);
		}

		public bool ContainsExcludeTag(string tag)
		{
			return this._excludeTagSet.Contains(tag);
		}

		public void AddExcludeTag(string tag)
		{
			this._excludeTagSet.Add(tag);
		}

		public void RemoveExcludeTag(string tag)
		{
			this._excludeTagSet.Remove(tag);
		}

		public void InjectOptionalRequireTags(string[] requireTags)
		{
			this._requireTags = requireTags;
		}

		public void InjectOptionalExcludeTags(string[] excludeTags)
		{
			this._excludeTags = excludeTags;
		}

		[Tooltip("A GameObject must meet all required tags.")]
		[SerializeField]
		[Optional]
		private string[] _requireTags;

		[Tooltip("A GameObject must not meet any exclude tags.")]
		[SerializeField]
		[Optional]
		[FormerlySerializedAs("_avoidTags")]
		private string[] _excludeTags;

		private readonly HashSet<string> _requireTagSet = new HashSet<string>();

		private readonly HashSet<string> _excludeTagSet = new HashSet<string>();
	}
}

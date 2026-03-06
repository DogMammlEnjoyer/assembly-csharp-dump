using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TagSet : MonoBehaviour
	{
		protected virtual void Start()
		{
			foreach (string item in this._tags)
			{
				this._tagSet.Add(item);
			}
		}

		public bool ContainsTag(string tag)
		{
			return this._tagSet.Contains(tag);
		}

		public void AddTag(string tag)
		{
			this._tagSet.Add(tag);
		}

		public void RemoveTag(string tag)
		{
			this._tagSet.Remove(tag);
		}

		public void InjectOptionalTags(List<string> tags)
		{
			this._tags = tags;
		}

		[Tooltip("The tags that should apply to this GameObject.")]
		[SerializeField]
		private List<string> _tags;

		private readonly HashSet<string> _tagSet = new HashSet<string>();
	}
}

using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class DeprecatedPrefab : MonoBehaviour
	{
		protected virtual void Start()
		{
			if (this._supressWarning)
			{
				return;
			}
			string text = "#3366ff";
			string name = base.gameObject.name;
			Debug.LogWarning(string.Concat(new string[]
			{
				"At GameObject <color=",
				text,
				"><b>",
				name,
				"</b></color>. ",
				DeprecatedPrefab.label
			}), this);
		}

		public static readonly string label = "This prefab has been deprecated. Consider using using the replacement provided or unpack the prefab before upgrading to a new version in order to avoid losing information.";

		[SerializeField]
		[HideInInspector]
		private Object _replacement;

		[SerializeField]
		[HideInInspector]
		private bool _supressWarning;
	}
}

using System;
using TMPro;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckNormalNotification : LckBaseNotification
	{
		public GameObject UI { get; private set; }

		public TMP_Text Text { get; private set; }
	}
}

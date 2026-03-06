using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OVROverlayCanvas_TMPChanged : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod]
	private static void OnLoad()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<Object>(OVROverlayCanvas_TMPChanged.OnTextChanged));
	}

	private static void OnTextChanged(Object target)
	{
		TMP_Text tmp_Text = target as TMP_Text;
		if (tmp_Text == null)
		{
			return;
		}
		OVROverlayCanvas ovroverlayCanvas;
		if (!OVROverlayCanvas_TMPChanged._textObjectToCanvas.TryGetValue(tmp_Text.gameObject, out ovroverlayCanvas))
		{
			return;
		}
		ovroverlayCanvas.SetFrameDirty();
	}

	protected void OnEnable()
	{
		OVROverlayCanvas_TMPChanged._textObjectToCanvas.Add(base.gameObject, this.TargetCanvas);
	}

	protected void OnDisable()
	{
		OVROverlayCanvas_TMPChanged._textObjectToCanvas.Remove(base.gameObject);
	}

	public OVROverlayCanvas TargetCanvas;

	private static Dictionary<GameObject, OVROverlayCanvas> _textObjectToCanvas = new Dictionary<GameObject, OVROverlayCanvas>();
}

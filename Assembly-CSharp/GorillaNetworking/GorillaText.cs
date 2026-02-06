using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	[Serializable]
	public class GorillaText
	{
		public void Initialize(Material[] originalMaterials, Material failureMaterial, UnityEvent<string> callback = null, UnityEvent<Material[]> materialCallback = null)
		{
			this.failureMaterial = failureMaterial;
			this.originalMaterials = originalMaterials;
			this.currentMaterials = originalMaterials;
			Debug.Log("Original text = " + this.originalText);
			this.updateTextCallback = callback;
			this.updateMaterialCallback = materialCallback;
			GorillaTextManager.RegisterText(this);
		}

		public void InvokeIfUpdated()
		{
			if (!this.modified)
			{
				return;
			}
			this.modified = false;
			string b = this.stringBuilder.ToString();
			if (this.currentText != b)
			{
				this.currentText = b;
				UnityEvent<string> unityEvent = this.updateTextCallback;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentText);
			}
		}

		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.failureText = failText;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(failText);
			}
			this.originalText = this.currentText;
			this.currentText = failText;
			this.currentMaterials = (Material[])this.originalMaterials.Clone();
			this.currentMaterials[0] = this.failureMaterial;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		public void DisableFailedState()
		{
			this.failedState = false;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.originalText);
			}
			this.failureText = "";
			this.currentText = this.originalText;
			this.currentMaterials = this.originalMaterials;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		public void Append(string str)
		{
			this.modified = true;
			this.stringBuilder.Append(str);
		}

		public void Set(string str)
		{
			this.modified = true;
			this.stringBuilder.Clear();
			this.stringBuilder.Append(str);
		}

		private string failureText;

		public string currentText;

		private string originalText = string.Empty;

		private StringBuilder stringBuilder = new StringBuilder();

		private bool modified;

		private bool failedState;

		private Material[] originalMaterials;

		private Material failureMaterial;

		internal Material[] currentMaterials;

		private UnityEvent<string> updateTextCallback;

		private UnityEvent<Material[]> updateMaterialCallback;
	}
}

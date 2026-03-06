using System;
using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
	public class TargetMeasurement : MonoBehaviour
	{
		private void Update()
		{
			if (Camera.main != null)
			{
				Vector3 position = Camera.main.transform.position;
				position.y = this.endPoint.position.y;
				float num = Vector3.Distance(position, this.endPoint.position);
				Vector3 position2 = Vector3.Lerp(position, this.endPoint.position, 0.5f);
				base.transform.position = position2;
				base.transform.forward = this.endPoint.position - position;
				this.measurementTape.localScale = new Vector3(0.05f, num, 0.05f);
				if (Mathf.Abs(num - this.lastDistance) > 0.01f)
				{
					this.measurementTextM.text = num.ToString("00.0m");
					this.measurementTextFT.text = ((double)num * 3.28084).ToString("00.0ft");
					this.lastDistance = num;
				}
				if (this.drawTape)
				{
					this.visualWrapper.SetActive(num < this.maxDistanceToDraw);
					return;
				}
				this.visualWrapper.SetActive(false);
			}
		}

		public GameObject visualWrapper;

		public Transform measurementTape;

		public Transform endPoint;

		public Text measurementTextM;

		public Text measurementTextFT;

		public float maxDistanceToDraw = 6f;

		public bool drawTape;

		private float lastDistance;
	}
}

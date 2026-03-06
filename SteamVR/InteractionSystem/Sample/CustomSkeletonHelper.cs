using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class CustomSkeletonHelper : MonoBehaviour
	{
		private void Update()
		{
			for (int i = 0; i < this.fingers.Length; i++)
			{
				CustomSkeletonHelper.Finger finger = this.fingers[i];
				finger.metacarpal.destination.rotation = finger.metacarpal.source.rotation;
				finger.proximal.destination.rotation = finger.proximal.source.rotation;
				finger.middle.destination.rotation = finger.middle.source.rotation;
				finger.distal.destination.rotation = finger.distal.source.rotation;
			}
			for (int j = 0; j < this.thumbs.Length; j++)
			{
				CustomSkeletonHelper.Thumb thumb = this.thumbs[j];
				thumb.metacarpal.destination.rotation = thumb.metacarpal.source.rotation;
				thumb.middle.destination.rotation = thumb.middle.source.rotation;
				thumb.distal.destination.rotation = thumb.distal.source.rotation;
			}
			this.wrist.destination.position = this.wrist.source.position;
			this.wrist.destination.rotation = this.wrist.source.rotation;
		}

		public CustomSkeletonHelper.Retargetable wrist;

		public CustomSkeletonHelper.Finger[] fingers;

		public CustomSkeletonHelper.Thumb[] thumbs;

		public enum MirrorType
		{
			None,
			LeftToRight,
			RightToLeft
		}

		[Serializable]
		public class Retargetable
		{
			public Retargetable(Transform source, Transform destination)
			{
				this.source = source;
				this.destination = destination;
			}

			public Transform source;

			public Transform destination;
		}

		[Serializable]
		public class Thumb
		{
			public Thumb(CustomSkeletonHelper.Retargetable metacarpal, CustomSkeletonHelper.Retargetable middle, CustomSkeletonHelper.Retargetable distal, Transform aux)
			{
				this.metacarpal = metacarpal;
				this.middle = middle;
				this.distal = distal;
				this.aux = aux;
			}

			public CustomSkeletonHelper.Retargetable metacarpal;

			public CustomSkeletonHelper.Retargetable middle;

			public CustomSkeletonHelper.Retargetable distal;

			public Transform aux;
		}

		[Serializable]
		public class Finger
		{
			public Finger(CustomSkeletonHelper.Retargetable metacarpal, CustomSkeletonHelper.Retargetable proximal, CustomSkeletonHelper.Retargetable middle, CustomSkeletonHelper.Retargetable distal, Transform aux)
			{
				this.metacarpal = metacarpal;
				this.proximal = proximal;
				this.middle = middle;
				this.distal = distal;
				this.aux = aux;
			}

			public CustomSkeletonHelper.Retargetable metacarpal;

			public CustomSkeletonHelper.Retargetable proximal;

			public CustomSkeletonHelper.Retargetable middle;

			public CustomSkeletonHelper.Retargetable distal;

			public Transform aux;
		}
	}
}

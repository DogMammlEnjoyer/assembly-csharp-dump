using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class SkeletonUIOptions : MonoBehaviour
	{
		public void AnimateHandWithController()
		{
			for (int i = 0; i < Player.instance.hands.Length; i++)
			{
				Hand hand = Player.instance.hands[i];
				if (hand != null)
				{
					hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithController, 0.1f);
				}
			}
		}

		public void AnimateHandWithoutController()
		{
			for (int i = 0; i < Player.instance.hands.Length; i++)
			{
				Hand hand = Player.instance.hands[i];
				if (hand != null)
				{
					hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithoutController, 0.1f);
				}
			}
		}

		public void ShowController()
		{
			for (int i = 0; i < Player.instance.hands.Length; i++)
			{
				Hand hand = Player.instance.hands[i];
				if (hand != null)
				{
					hand.ShowController(true);
				}
			}
		}

		public void SetRenderModel(RenderModelChangerUI prefabs)
		{
			for (int i = 0; i < Player.instance.hands.Length; i++)
			{
				Hand hand = Player.instance.hands[i];
				if (hand != null)
				{
					if (hand.handType == SteamVR_Input_Sources.RightHand)
					{
						hand.SetRenderModel(prefabs.rightPrefab);
					}
					if (hand.handType == SteamVR_Input_Sources.LeftHand)
					{
						hand.SetRenderModel(prefabs.leftPrefab);
					}
				}
			}
		}

		public void HideController()
		{
			for (int i = 0; i < Player.instance.hands.Length; i++)
			{
				Hand hand = Player.instance.hands[i];
				if (hand != null)
				{
					hand.HideController(true);
				}
			}
		}
	}
}

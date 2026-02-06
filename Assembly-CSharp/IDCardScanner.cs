using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class IDCardScanner : MonoBehaviour
{
	public event IDCardScanner.CardSwipeEvent OnPlayerCardSwipe;

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ScannableIDCard>() != null)
		{
			UnityEvent unityEvent = this.onCardSwiped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			GameEntity component = other.GetComponent<GameEntity>();
			if (component == null && other.attachedRigidbody != null)
			{
				component = other.attachedRigidbody.GetComponent<GameEntity>();
			}
			if (component != null && component.heldByActorNumber != -1)
			{
				bool flag = !this.requireSpecificPlayer || (this.restrictToPlayer != null && this.restrictToPlayer.ActorNumber == component.heldByActorNumber && component.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
				bool flag2 = !this.requireAuthority || component.manager.IsAuthority();
				if (flag && flag2)
				{
					UnityEvent<int> unityEvent2 = this.onCardSwipedByPlayer;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(component.heldByActorNumber);
					}
					IDCardScanner.CardSwipeEvent onPlayerCardSwipe = this.OnPlayerCardSwipe;
					if (onPlayerCardSwipe == null)
					{
						return;
					}
					onPlayerCardSwipe(component.heldByActorNumber);
				}
			}
		}
	}

	public UnityEvent onCardSwiped;

	public UnityEvent<int> onCardSwipedByPlayer;

	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onSucceeded;

	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onFailed;

	public bool requireSpecificPlayer;

	public bool requireAuthority;

	[NonSerialized]
	public NetPlayer restrictToPlayer;

	public delegate void CardSwipeEvent(int actorNumber);
}

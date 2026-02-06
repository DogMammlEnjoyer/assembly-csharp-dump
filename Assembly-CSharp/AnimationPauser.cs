using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimationPauser : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AnimationPauser.<OnStateEnter>d__4 <OnStateEnter>d__;
		<OnStateEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnStateEnter>d__.<>4__this = this;
		<OnStateEnter>d__.animator = animator;
		<OnStateEnter>d__.stateInfo = stateInfo;
		<OnStateEnter>d__.layerIndex = layerIndex;
		<OnStateEnter>d__.<>1__state = -1;
		<OnStateEnter>d__.<>t__builder.Start<AnimationPauser.<OnStateEnter>d__4>(ref <OnStateEnter>d__);
	}

	[SerializeField]
	private int _maxTimeBetweenAnims = 5;

	[SerializeField]
	private int _minTimeBetweenAnims = 1;

	private int _animPauseDuration;

	private static readonly string Restart_Anim_Name = "RestartAnim";
}

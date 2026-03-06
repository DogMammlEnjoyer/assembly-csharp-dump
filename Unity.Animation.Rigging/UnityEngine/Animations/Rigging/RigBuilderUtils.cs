using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Animations.Rigging
{
	internal static class RigBuilderUtils
	{
		public static Playable[] BuildRigPlayables(PlayableGraph graph, IRigLayer layer)
		{
			if (layer == null || layer.jobs == null || layer.jobs.Length == 0)
			{
				return null;
			}
			int num = layer.jobs.Length;
			Playable[] array = new Playable[num];
			for (int i = 0; i < num; i++)
			{
				IAnimationJobBinder binder = layer.constraints[i].binder;
				array[i] = binder.CreatePlayable(graph, layer.jobs[i]);
			}
			for (int j = 1; j < num; j++)
			{
				array[j].AddInput(array[j - 1], 0, 1f);
			}
			return array;
		}

		public static IEnumerable<RigBuilderUtils.PlayableChain> BuildPlayables(Animator animator, PlayableGraph graph, IList<IRigLayer> layers, SyncSceneToStreamLayer syncSceneToStreamLayer)
		{
			RigBuilderUtils.PlayableChain[] array = new RigBuilderUtils.PlayableChain[layers.Count + 1];
			int num = 1;
			foreach (IRigLayer rigLayer in layers)
			{
				RigBuilderUtils.PlayableChain playableChain = default(RigBuilderUtils.PlayableChain);
				playableChain.name = rigLayer.name;
				if (rigLayer.Initialize(animator))
				{
					playableChain.playables = RigBuilderUtils.BuildRigPlayables(graph, rigLayer);
				}
				array[num++] = playableChain;
			}
			if (syncSceneToStreamLayer.Initialize(animator, layers) && syncSceneToStreamLayer.IsValid())
			{
				array[0] = new RigBuilderUtils.PlayableChain
				{
					name = "syncSceneToStream",
					playables = new Playable[]
					{
						RigUtils.syncSceneToStreamBinder.CreatePlayable(graph, syncSceneToStreamLayer.job)
					}
				};
			}
			return array;
		}

		public static PlayableGraph BuildPlayableGraph(Animator animator, IList<IRigLayer> layers, SyncSceneToStreamLayer syncSceneToStreamLayer)
		{
			PlayableGraph playableGraph = PlayableGraph.Create(animator.gameObject.transform.name + "_Rigs");
			playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
			RigBuilderUtils.BuildPlayableGraph(playableGraph, animator, layers, syncSceneToStreamLayer);
			return playableGraph;
		}

		public static void BuildPlayableGraph(PlayableGraph graph, Animator animator, IList<IRigLayer> layers, SyncSceneToStreamLayer syncSceneToStreamLayer)
		{
			foreach (RigBuilderUtils.PlayableChain playableChain in RigBuilderUtils.BuildPlayables(animator, graph, layers, syncSceneToStreamLayer))
			{
				if (playableChain.IsValid())
				{
					AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, string.Format("{0}-Output", playableChain.name), animator);
					output.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);
					output.SetSortingOrder(RigBuilderUtils.k_AnimationOutputPriority);
					output.SetSourcePlayable(playableChain.playables[playableChain.playables.Length - 1]);
				}
			}
		}

		private static readonly ushort k_AnimationOutputPriority = 1000;

		public struct PlayableChain
		{
			public bool IsValid()
			{
				return this.playables != null && this.playables.Length != 0;
			}

			public string name;

			public Playable[] playables;
		}
	}
}

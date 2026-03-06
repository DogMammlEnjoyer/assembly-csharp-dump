using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[AddComponentMenu("XR/XR Target Filter", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Filtering.XRTargetFilter.html")]
	public sealed class XRTargetFilter : XRBaseTargetFilter, IEnumerable<XRTargetEvaluator>, IEnumerable
	{
		private static int InteractableScoreDescendingComparison(IXRInteractable x, IXRInteractable y)
		{
			float num = XRTargetFilter.s_InteractableFinalScoreMap[x];
			float num2 = XRTargetFilter.s_InteractableFinalScoreMap[y];
			if (num < num2)
			{
				return 1;
			}
			if (num > num2)
			{
				return -1;
			}
			return 0;
		}

		internal List<IXRInteractor> linkedInteractors
		{
			get
			{
				return this.m_LinkedInteractors;
			}
		}

		internal List<XRTargetEvaluator> evaluators
		{
			get
			{
				return this.m_Evaluators;
			}
		}

		public int evaluatorCount
		{
			get
			{
				return this.m_Evaluators.Count;
			}
		}

		internal bool isProcessing { get; private set; }

		public event Action<IXRInteractor> interactorLinked;

		public event Action<IXRInteractor> interactorUnlinked;

		public override bool canProcess
		{
			get
			{
				return !this.isProcessing && base.canProcess;
			}
		}

		private void Awake()
		{
			this.m_IsAwake = true;
			List<XRTargetEvaluator> list;
			using (XRTargetFilter.s_EvaluatorListPool.Get(out list))
			{
				this.GetEvaluators(list);
				int num = 0;
				while (num < list.Count && this.m_IsAwake)
				{
					list[num].AwakeInternal();
					num++;
				}
			}
		}

		private void OnEnable()
		{
			List<XRTargetEvaluator> list;
			using (XRTargetFilter.s_EvaluatorListPool.Get(out list))
			{
				this.GetEvaluators(list);
				int num = 0;
				while (num < list.Count && base.isActiveAndEnabled)
				{
					XRTargetEvaluator xrtargetEvaluator = list[num];
					if (xrtargetEvaluator.enabled)
					{
						xrtargetEvaluator.EnableInternal();
					}
					num++;
				}
			}
		}

		private void OnDisable()
		{
			List<XRTargetEvaluator> list;
			using (XRTargetFilter.s_EvaluatorListPool.Get(out list))
			{
				this.GetEnabledEvaluators(list);
				int num = 0;
				while (num < list.Count && !base.isActiveAndEnabled)
				{
					list[num].DisableInternal();
					num++;
				}
			}
		}

		private void OnDestroy()
		{
			this.m_IsAwake = false;
			List<XRTargetEvaluator> list;
			using (XRTargetFilter.s_EvaluatorListPool.Get(out list))
			{
				this.GetEvaluators(list);
				foreach (XRTargetEvaluator xrtargetEvaluator in list)
				{
					xrtargetEvaluator.DisposeInternal();
				}
			}
		}

		private void Reset()
		{
			this.AddEvaluator<XRDistanceEvaluator>().Reset();
		}

		internal void RegisterEvaluatorHandlers(XRTargetEvaluator evaluator)
		{
			IXRTargetEvaluatorLinkable ixrtargetEvaluatorLinkable = evaluator as IXRTargetEvaluatorLinkable;
			if (ixrtargetEvaluatorLinkable != null)
			{
				this.interactorLinked += ixrtargetEvaluatorLinkable.OnLink;
				this.interactorUnlinked += ixrtargetEvaluatorLinkable.OnUnlink;
				foreach (IXRInteractor interactor in this.m_LinkedInteractors)
				{
					ixrtargetEvaluatorLinkable.OnLink(interactor);
				}
			}
		}

		internal void UnregisterEvaluatorHandlers(XRTargetEvaluator evaluator)
		{
			IXRTargetEvaluatorLinkable ixrtargetEvaluatorLinkable = evaluator as IXRTargetEvaluatorLinkable;
			if (ixrtargetEvaluatorLinkable != null)
			{
				this.interactorLinked -= ixrtargetEvaluatorLinkable.OnLink;
				this.interactorUnlinked -= ixrtargetEvaluatorLinkable.OnUnlink;
				foreach (IXRInteractor interactor in this.m_LinkedInteractors)
				{
					ixrtargetEvaluatorLinkable.OnUnlink(interactor);
				}
			}
		}

		public void GetLinkedInteractors(List<IXRInteractor> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			results.Clear();
			results.AddRange(this.m_LinkedInteractors);
		}

		public void GetEvaluators(List<XRTargetEvaluator> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			results.Clear();
			results.AddRange(this.m_Evaluators);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.m_Evaluators).GetEnumerator();
		}

		public IEnumerator<XRTargetEvaluator> GetEnumerator()
		{
			return this.m_Evaluators.GetEnumerator();
		}

		public XRTargetEvaluator GetEvaluatorAt(int index)
		{
			return this.m_Evaluators[index];
		}

		public XRTargetEvaluator GetEvaluator(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			foreach (XRTargetEvaluator xrtargetEvaluator in this.m_Evaluators)
			{
				if (type.IsInstanceOfType(xrtargetEvaluator))
				{
					return xrtargetEvaluator;
				}
			}
			return null;
		}

		public T GetEvaluator<T>()
		{
			return (T)((object)this.GetEvaluator(typeof(T)));
		}

		public void GetEnabledEvaluators(List<XRTargetEvaluator> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			results.Clear();
			foreach (XRTargetEvaluator xrtargetEvaluator in this.m_Evaluators)
			{
				if (xrtargetEvaluator.enabled)
				{
					results.Add(xrtargetEvaluator);
				}
			}
		}

		public XRTargetEvaluator AddEvaluator(Type evaluatorType)
		{
			if (evaluatorType == null)
			{
				throw new ArgumentNullException("evaluatorType");
			}
			XRTargetEvaluator xrtargetEvaluator = XRTargetEvaluator.CreateInstance(evaluatorType, this);
			if (xrtargetEvaluator == null)
			{
				return null;
			}
			this.m_Evaluators.Add(xrtargetEvaluator);
			if (this.m_IsAwake)
			{
				xrtargetEvaluator.AwakeInternal();
				if (base.isActiveAndEnabled && xrtargetEvaluator.enabled)
				{
					xrtargetEvaluator.EnableInternal();
				}
			}
			return xrtargetEvaluator;
		}

		public T AddEvaluator<T>() where T : XRTargetEvaluator
		{
			return this.AddEvaluator(typeof(T)) as T;
		}

		public void RemoveEvaluatorAt(int index)
		{
			if (this.isProcessing)
			{
				throw new InvalidOperationException("Cannot remove evaluators while a filter " + base.name + " is processing.");
			}
			XRTargetEvaluator xrtargetEvaluator = this.m_Evaluators[index];
			if (this.m_IsAwake && xrtargetEvaluator != null)
			{
				if (base.isActiveAndEnabled && xrtargetEvaluator.enabled)
				{
					xrtargetEvaluator.DisableInternal();
				}
				xrtargetEvaluator.DisposeInternal();
			}
			this.m_Evaluators.RemoveAt(index);
		}

		public void RemoveEvaluator(XRTargetEvaluator evaluator)
		{
			if (this.isProcessing)
			{
				throw new InvalidOperationException("Cannot remove evaluators while a filter " + base.name + " is processing.");
			}
			int num = this.m_Evaluators.IndexOf(evaluator);
			if (num < 0)
			{
				return;
			}
			this.RemoveEvaluatorAt(num);
		}

		public void MoveEvaluatorTo(XRTargetEvaluator evaluator, int newIndex)
		{
			int num = this.m_Evaluators.IndexOf(evaluator);
			if (num < 0 || num == newIndex)
			{
				return;
			}
			this.m_Evaluators.RemoveAt(num);
			this.m_Evaluators.Insert(newIndex, evaluator);
		}

		public override void Link(IXRInteractor interactor)
		{
			if (interactor == null)
			{
				throw new ArgumentNullException("interactor");
			}
			if (!this.m_LinkedInteractors.Contains(interactor))
			{
				this.m_LinkedInteractors.Add(interactor);
				Action<IXRInteractor> action = this.interactorLinked;
				if (action == null)
				{
					return;
				}
				action(interactor);
			}
		}

		public override void Unlink(IXRInteractor interactor)
		{
			if (interactor == null)
			{
				throw new ArgumentNullException("interactor");
			}
			if (this.isProcessing)
			{
				throw new InvalidOperationException(string.Format("Cannot unlink an interactor {0} while the filter {1} is processing.", interactor, base.name));
			}
			if (this.m_LinkedInteractors.Remove(interactor))
			{
				Action<IXRInteractor> action = this.interactorUnlinked;
				if (action == null)
				{
					return;
				}
				action(interactor);
			}
		}

		public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
		{
			if (this.isProcessing)
			{
				throw new InvalidOperationException("Process for filter " + base.name + " is already running, cannot start a new one.");
			}
			this.isProcessing = true;
			try
			{
				results.Clear();
				List<XRTargetEvaluator> list;
				using (XRTargetFilter.s_EvaluatorListPool.Get(out list))
				{
					this.GetEnabledEvaluators(list);
					foreach (IXRInteractable ixrinteractable in targets)
					{
						float num = 1f;
						foreach (XRTargetEvaluator xrtargetEvaluator in list)
						{
							float weightedScore = xrtargetEvaluator.GetWeightedScore(interactor, ixrinteractable);
							num *= weightedScore;
							if (num <= 0f)
							{
								break;
							}
						}
						if (num >= 0f)
						{
							results.Add(ixrinteractable);
							XRTargetFilter.s_InteractableFinalScoreMap[ixrinteractable] = num;
						}
					}
				}
				results.Sort(XRTargetFilter.s_InteractableScoreComparison);
			}
			finally
			{
				this.isProcessing = false;
				XRTargetFilter.s_InteractableFinalScoreMap.Clear();
			}
		}

		private static readonly LinkedPool<List<XRTargetEvaluator>> s_EvaluatorListPool = new LinkedPool<List<XRTargetEvaluator>>(() => new List<XRTargetEvaluator>(), null, delegate(List<XRTargetEvaluator> list)
		{
			list.Clear();
		}, null, false, 10000);

		private static readonly Dictionary<IXRInteractable, float> s_InteractableFinalScoreMap = new Dictionary<IXRInteractable, float>();

		private static readonly Comparison<IXRInteractable> s_InteractableScoreComparison = new Comparison<IXRInteractable>(XRTargetFilter.InteractableScoreDescendingComparison);

		private List<IXRInteractor> m_LinkedInteractors = new List<IXRInteractor>();

		[SerializeReference]
		private List<XRTargetEvaluator> m_Evaluators = new List<XRTargetEvaluator>();

		private bool m_IsAwake;
	}
}

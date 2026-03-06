using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	[CreateAssetMenu(menuName = "Meta/Interaction/SDK/Scene Group")]
	public class SampleSceneGroup : ScriptableObject
	{
		public string GroupName
		{
			get
			{
				return this._groupName;
			}
		}

		public bool GroupEnabled
		{
			get
			{
				return this._groupEnabled;
			}
		}

		public int GroupDisplayOrder
		{
			get
			{
				return this._groupDisplayOrder;
			}
		}

		public int SceneCount
		{
			get
			{
				return this._sceneInfos.Length;
			}
		}

		public IEnumerable<SampleSceneGroup.ISceneInfo> GetScenes()
		{
			foreach (SampleSceneGroup.SceneInfo sceneInfo in this._sceneInfos)
			{
				yield return sceneInfo;
			}
			SampleSceneGroup.SceneInfo[] array = null;
			yield break;
		}

		[Tooltip("Scenes in this group will be displayed under this header in the scene menu.")]
		[SerializeField]
		private string _groupName;

		[Tooltip("Only Enabled scene groups will be shown in the scene menu.")]
		[SerializeField]
		private bool _groupEnabled = true;

		[Tooltip("Scene groups will appear in the scene menu sorted in ascending order by this value.")]
		[SerializeField]
		private int _groupDisplayOrder;

		[SerializeField]
		[HideInInspector]
		private SampleSceneGroup.SceneInfo[] _sceneInfos;

		public interface ISceneInfo
		{
			string DisplayName { get; }

			string SceneName { get; }

			string SceneGuid { get; }

			Sprite Thumbnail { get; }
		}

		[Serializable]
		private class SceneInfo : SampleSceneGroup.ISceneInfo
		{
			string SampleSceneGroup.ISceneInfo.DisplayName
			{
				get
				{
					return this.DisplayName;
				}
			}

			string SampleSceneGroup.ISceneInfo.SceneName
			{
				get
				{
					return this.SceneName;
				}
			}

			Sprite SampleSceneGroup.ISceneInfo.Thumbnail
			{
				get
				{
					return this.Thumbnail;
				}
			}

			string SampleSceneGroup.ISceneInfo.SceneGuid
			{
				get
				{
					return this.SceneGuid;
				}
			}

			public string DisplayName;

			public string SceneName;

			public string SceneGuid;

			public Sprite Thumbnail;
		}
	}
}

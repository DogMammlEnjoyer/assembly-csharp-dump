using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class SceneGroupLoader : MonoBehaviour
	{
		private void Start()
		{
			this.BuildSceneGroups();
		}

		private void BuildSceneGroups()
		{
			bool flag = false;
			foreach (SampleSceneGroup sampleSceneGroup in from x in SceneGroupLoader.FindSceneGroupAssets()
			where x.GroupEnabled
			select x into g
			where g.SceneCount > 0
			orderby g.GroupDisplayOrder
			select g)
			{
				this.<BuildSceneGroups>g__InitializeGroupViewTemplate|12_0();
				GameObject gameObject = Object.Instantiate<GameObject>(this._groupTemplateParent, this._sceneGroupContainer);
				gameObject.name = sampleSceneGroup.GroupName;
				gameObject.SetActive(true);
				SceneGroupLoader.SceneGroupView component = gameObject.GetComponent<SceneGroupLoader.SceneGroupView>();
				component.GroupName.text = sampleSceneGroup.GroupName;
				using (IEnumerator<SampleSceneGroup.ISceneInfo> enumerator2 = sampleSceneGroup.GetScenes().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SampleSceneGroup.ISceneInfo sceneMenuItem = enumerator2.Current;
						this.<BuildSceneGroups>g__InitializeTileViewTemplate|12_1();
						bool flag2 = SceneGroupLoader.CheckSceneExists(sceneMenuItem);
						flag |= !flag2;
						GameObject gameObject2 = Object.Instantiate<GameObject>(this._tileTemplateParent, component.TileContainer);
						gameObject2.name = sceneMenuItem.DisplayName;
						gameObject2.SetActive(true);
						SceneGroupLoader.SceneTileView component2 = gameObject2.GetComponent<SceneGroupLoader.SceneTileView>();
						component2.Label.text = sceneMenuItem.DisplayName;
						component2.Toggle.enabled = flag2;
						component2.Toggle.onValueChanged.AddListener(delegate(bool v)
						{
							this.LoadScene(sceneMenuItem);
						});
						component2.Image.sprite = sceneMenuItem.Thumbnail;
						component2.Image.enabled = flag2;
						component2.SceneMissingOverlay.sprite = sceneMenuItem.Thumbnail;
						component2.SceneMissingOverlay.gameObject.SetActive(!flag2);
					}
				}
			}
			this._missingSceneWarning.SetActive(flag);
		}

		private void LoadScene(SampleSceneGroup.ISceneInfo sceneInfo)
		{
			this._sceneLoader.Load(sceneInfo.SceneName);
		}

		private static bool CheckSceneExists(SampleSceneGroup.ISceneInfo sceneInfo)
		{
			return SceneUtility.GetBuildIndexByScenePath(sceneInfo.SceneName) >= 0;
		}

		private static IEnumerable<SampleSceneGroup> FindSceneGroupAssets()
		{
			return Resources.LoadAll<SampleSceneGroup>("");
		}

		[CompilerGenerated]
		private void <BuildSceneGroups>g__InitializeGroupViewTemplate|12_0()
		{
			SceneGroupLoader.SceneGroupView sceneGroupView;
			if (!this._groupTemplateParent.TryGetComponent<SceneGroupLoader.SceneGroupView>(out sceneGroupView))
			{
				sceneGroupView = this._groupTemplateParent.AddComponent<SceneGroupLoader.SceneGroupView>();
				sceneGroupView.GroupName = this._groupTemplateLabel;
				sceneGroupView.TileContainer = this._groupTileContainer;
			}
		}

		[CompilerGenerated]
		private void <BuildSceneGroups>g__InitializeTileViewTemplate|12_1()
		{
			SceneGroupLoader.SceneTileView sceneTileView;
			if (!this._tileTemplateParent.TryGetComponent<SceneGroupLoader.SceneTileView>(out sceneTileView))
			{
				sceneTileView = this._tileTemplateParent.AddComponent<SceneGroupLoader.SceneTileView>();
				sceneTileView.Image = this._tileTemplateImage;
				sceneTileView.Label = this._tileTemplateLabel;
				sceneTileView.Toggle = this._tileTemplateToggle;
				sceneTileView.SceneMissingOverlay = this._tileTemplateSceneMissingOverlay;
			}
		}

		[SerializeField]
		private SceneLoader _sceneLoader;

		[SerializeField]
		private Transform _sceneGroupContainer;

		[SerializeField]
		private GameObject _missingSceneWarning;

		[Header("Group Template")]
		[SerializeField]
		private GameObject _groupTemplateParent;

		[SerializeField]
		private TextMeshProUGUI _groupTemplateLabel;

		[SerializeField]
		private RectTransform _groupTileContainer;

		[Header("Tile Template")]
		[SerializeField]
		private GameObject _tileTemplateParent;

		[SerializeField]
		private TextMeshProUGUI _tileTemplateLabel;

		[SerializeField]
		private Image _tileTemplateImage;

		[SerializeField]
		private Toggle _tileTemplateToggle;

		[SerializeField]
		private Image _tileTemplateSceneMissingOverlay;

		private class SceneGroupView : MonoBehaviour
		{
			public TextMeshProUGUI GroupName;

			public RectTransform TileContainer;
		}

		private class SceneTileView : MonoBehaviour
		{
			public TextMeshProUGUI Label;

			public Image Image;

			public Toggle Toggle;

			public Image SceneMissingOverlay;
		}
	}
}

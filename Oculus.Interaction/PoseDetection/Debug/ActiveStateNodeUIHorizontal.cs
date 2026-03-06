using System;
using Oculus.Interaction.DebugTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class ActiveStateNodeUIHorizontal : MonoBehaviour, INodeUI<IActiveState>
	{
		public RectTransform ChildArea
		{
			get
			{
				return this._childArea;
			}
		}

		public void Bind(ITreeNode<IActiveState> node, bool isRoot, bool isDuplicate)
		{
			this._isRoot = isRoot;
			this._isDuplicate = isDuplicate;
			this._boundNode = node;
			this._label.text = this.GetLabelText(node);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			this._activeImage.color = (this._boundNode.Value.Active ? this._activeColor : this._inactiveColor);
			this._childArea.gameObject.SetActive(this._childArea.childCount > 0);
			this._connectingLine.gameObject.SetActive(!this._isRoot);
		}

		private string GetLabelText(ITreeNode<IActiveState> node)
		{
			string str = this._isDuplicate ? "<i>" : "";
			Object @object = node.Value as Object;
			if (@object != null)
			{
				str = str + @object.name + Environment.NewLine;
			}
			return str + string.Format("<color=#dddddd><size=85%>{0}</size></color>", node.Value.GetType().Name);
		}

		[SerializeField]
		private RectTransform _childArea;

		[SerializeField]
		private RectTransform _connectingLine;

		[SerializeField]
		private TextMeshProUGUI _label;

		[SerializeField]
		private Image _activeImage;

		[SerializeField]
		private Color _activeColor = Color.green;

		[SerializeField]
		private Color _inactiveColor = Color.red;

		private const string OBJNAME_FORMAT = "<color=#dddddd><size=85%>{0}</size></color>";

		private ITreeNode<IActiveState> _boundNode;

		private bool _isRoot;

		private bool _isDuplicate;
	}
}

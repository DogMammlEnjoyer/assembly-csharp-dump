using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.DebugTree
{
	public class InteractorGroupNodeUI : MonoBehaviour, INodeUI<IInteractor>
	{
		public RectTransform ChildArea
		{
			get
			{
				return this._childArea;
			}
		}

		public void Bind(ITreeNode<IInteractor> node, bool isRoot, bool isDuplicate)
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
			switch (this._boundNode.Value.State)
			{
			case InteractorState.Normal:
				this._activeImage.color = this._normalColor;
				break;
			case InteractorState.Hover:
				this._activeImage.color = this._hoverColor;
				break;
			case InteractorState.Select:
				this._activeImage.color = this._selectColor;
				break;
			case InteractorState.Disabled:
				this._activeImage.color = this._disabledColor;
				break;
			}
			this._childArea.gameObject.SetActive(this._childArea.childCount > 0);
			this._connectingLine.gameObject.SetActive(!this._isRoot);
		}

		private string GetLabelText(ITreeNode<IInteractor> node)
		{
			string str = this._isDuplicate ? "<i>" : "";
			Object @object = node.Value as Object;
			if (@object != null)
			{
				str = str + @object.name + " - ";
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
		private Color _selectColor = Color.green;

		[SerializeField]
		private Color _hoverColor = Color.blue;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _disabledColor = Color.grey;

		private const string OBJNAME_FORMAT = "<color=#dddddd><size=85%>{0}</size></color>";

		private ITreeNode<IInteractor> _boundNode;

		private bool _isRoot;

		private bool _isDuplicate;
	}
}

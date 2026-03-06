using System;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.Samples.PalmMenu
{
	public class PalmMenuExampleButtonHandlers : MonoBehaviour
	{
		private void Start()
		{
			this._currentColorIdx = this._colors.Length;
			this.CycleColor();
			this._rotationEnabled = false;
			this.ToggleRotationEnabled();
			this._currentRotationDirectionIdx = this._rotationDirections.Length;
			this.CycleRotationDirection();
			this._targetPosition = this._controlledObject.transform.position;
			this.IncrementElevation(true);
			this.IncrementElevation(false);
			this._currentShapeIdx = this._shapes.Length;
			this.CycleShape(true);
		}

		private void Update()
		{
			if (this._rotationEnabled)
			{
				Quaternion lhs = Quaternion.Slerp(Quaternion.identity, this._rotationDirections[this._currentRotationDirectionIdx], this._rotationLerpSpeed * Time.deltaTime);
				this._controlledObject.transform.rotation = lhs * this._controlledObject.transform.rotation;
			}
			this._controlledObject.transform.position = Vector3.Lerp(this._controlledObject.transform.position, this._targetPosition, this._elevationChangeLerpSpeed * Time.deltaTime);
		}

		public void CycleColor()
		{
			this._currentColorIdx++;
			if (this._currentColorIdx >= this._colors.Length)
			{
				this._currentColorIdx = 0;
			}
			this._controlledObject.GetComponent<Renderer>().material.color = this._colors[this._currentColorIdx];
		}

		public void ToggleRotationEnabled()
		{
			this._rotationEnabled = !this._rotationEnabled;
			this._rotationEnabledIcon.SetActive(!this._rotationEnabled);
			this._rotationDisabledIcon.SetActive(this._rotationEnabled);
		}

		public void CycleRotationDirection()
		{
			this._currentRotationDirectionIdx++;
			if (this._currentRotationDirectionIdx >= this._rotationDirections.Length)
			{
				this._currentRotationDirectionIdx = 0;
			}
			int num = this._currentRotationDirectionIdx + 1;
			if (num >= this._rotationDirections.Length)
			{
				num = 0;
			}
			this._rotationDirectionText.text = this._rotationDirectionNames[num];
			for (int i = 0; i < this._rotationDirections.Length; i++)
			{
				this._rotationDirectionIcons[i].SetActive(i == num);
			}
		}

		public void IncrementElevation(bool up)
		{
			float num = this._elevationChangeIncrement;
			if (!up)
			{
				num *= -1f;
			}
			this._targetPosition = new Vector3(this._targetPosition.x, Mathf.Clamp(this._targetPosition.y + num, 0.2f, 2f), this._targetPosition.z);
			this._elevationText.text = "Elevation: " + this._targetPosition.y.ToString("0.0");
		}

		public void CycleShape(bool cycleForward)
		{
			this._currentShapeIdx += (cycleForward ? 1 : -1);
			if (this._currentShapeIdx >= this._shapes.Length)
			{
				this._currentShapeIdx = 0;
			}
			else if (this._currentShapeIdx < 0)
			{
				this._currentShapeIdx = this._shapes.Length - 1;
			}
			this._shapeNameText.text = this._shapeNames[this._currentShapeIdx];
			this._controlledObject.GetComponent<MeshFilter>().mesh = this._shapes[this._currentShapeIdx];
		}

		[SerializeField]
		private GameObject _controlledObject;

		[SerializeField]
		private Color[] _colors;

		[SerializeField]
		private GameObject _rotationEnabledIcon;

		[SerializeField]
		private GameObject _rotationDisabledIcon;

		[SerializeField]
		private float _rotationLerpSpeed = 1f;

		[SerializeField]
		private TMP_Text _rotationDirectionText;

		[SerializeField]
		private string[] _rotationDirectionNames;

		[SerializeField]
		private GameObject[] _rotationDirectionIcons;

		[SerializeField]
		private Quaternion[] _rotationDirections;

		[SerializeField]
		private TMP_Text _elevationText;

		[SerializeField]
		private float _elevationChangeIncrement;

		[SerializeField]
		private float _elevationChangeLerpSpeed = 1f;

		[SerializeField]
		private TMP_Text _shapeNameText;

		[SerializeField]
		private string[] _shapeNames;

		[SerializeField]
		private Mesh[] _shapes;

		private int _currentColorIdx;

		private bool _rotationEnabled;

		private int _currentRotationDirectionIdx;

		private Vector3 _targetPosition;

		private int _currentShapeIdx;
	}
}

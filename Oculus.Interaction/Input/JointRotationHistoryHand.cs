using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class JointRotationHistoryHand : Hand
	{
		protected override void Start()
		{
			base.Start();
			for (int i = 0; i < this._jointHistory.Length; i++)
			{
				this._jointHistory[i] = new Quaternion[this._historyLength];
				for (int j = 0; j < this._historyLength; j++)
				{
					this._jointHistory[i][j] = Quaternion.identity;
				}
			}
		}

		protected override void Apply(HandDataAsset data)
		{
			if (!data.IsDataValid)
			{
				return;
			}
			if (this._capturedDataVersion != this.ModifyDataFromSource.CurrentDataVersion)
			{
				this._capturedDataVersion = this.ModifyDataFromSource.CurrentDataVersion;
				this._historyIndex = (this._historyIndex + 1) % this._historyLength;
				for (int i = 0; i < this._jointHistory.Length; i++)
				{
					this._jointHistory[i][this._historyIndex] = data.Joints[i];
				}
			}
			this._historyOffset = Mathf.Clamp(this._historyOffset, 0, this._historyLength);
			int num = (this._historyIndex + this._historyLength - this._historyOffset) % this._historyLength;
			for (int j = 0; j < this._jointHistory.Length; j++)
			{
				data.Joints[j] = this._jointHistory[j][num];
			}
		}

		public void SetHistoryOffset(int offset)
		{
			this._historyOffset = offset;
			this.MarkInputDataRequiresUpdate();
		}

		public void InjectAllJointHistoryHand(DataSource<HandDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier, int historyLength, int historyOffset)
		{
			base.InjectAllHand(updateMode, updateAfter, modifyDataFromSource, applyModifier);
			this.InjectHistoryLength(historyLength);
			this.SetHistoryOffset(historyOffset);
		}

		public void InjectHistoryLength(int historyLength)
		{
			this._historyLength = historyLength;
		}

		[SerializeField]
		private int _historyLength = 60;

		[SerializeField]
		private int _historyOffset = 5;

		private Quaternion[][] _jointHistory = new Quaternion[26][];

		private int _historyIndex;

		private int _capturedDataVersion;
	}
}

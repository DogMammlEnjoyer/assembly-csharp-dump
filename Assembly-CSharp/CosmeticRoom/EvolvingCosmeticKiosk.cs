using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmeticRoom
{
	public class EvolvingCosmeticKiosk : MonoBehaviour
	{
		public bool Initialized { get; private set; }

		public VRRig VRRig
		{
			get
			{
				return VRRig.LocalRig;
			}
		}

		public bool CosmeticsListBuilding { get; private set; }

		private void Awake()
		{
			EvolvingCosmeticKioskButtonSet[] buttonSets = this._buttonSets;
			for (int i = 0; i < buttonSets.Length; i++)
			{
				buttonSets[i].RegisterKiosk(this);
			}
			this.Initialized = true;
		}

		private Task BuildCosmeticsList()
		{
			EvolvingCosmeticKiosk.<BuildCosmeticsList>d__14 <BuildCosmeticsList>d__;
			<BuildCosmeticsList>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<BuildCosmeticsList>d__.<>4__this = this;
			<BuildCosmeticsList>d__.<>1__state = -1;
			<BuildCosmeticsList>d__.<>t__builder.Start<EvolvingCosmeticKiosk.<BuildCosmeticsList>d__14>(ref <BuildCosmeticsList>d__);
			return <BuildCosmeticsList>d__.<>t__builder.Task;
		}

		private void ResetButtonSets()
		{
			this._cosmeticIdx = 0;
			EvolvingCosmeticKioskButtonSet[] buttonSets = this._buttonSets;
			for (int i = 0; i < buttonSets.Length; i++)
			{
				buttonSets[i].Reset();
			}
		}

		private void UpdateButtonSets()
		{
			for (int i = 0; i < this._buttonSets.Length; i++)
			{
				int num = this._cosmeticIdx + i;
				if (num >= this._cosmetics.Count)
				{
					this._buttonSets[i].Reset();
				}
				else
				{
					EvolvingCosmeticKiosk.CosmeticData cosmeticData = this._cosmetics[num];
					this._buttonSets[i].SetCosmetic(cosmeticData.PlayfabId, cosmeticData.EvolvingCosmetic);
				}
			}
		}

		public void OnHandScanned(NetPlayer player)
		{
			EvolvingCosmeticKiosk.<OnHandScanned>d__17 <OnHandScanned>d__;
			<OnHandScanned>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnHandScanned>d__.<>4__this = this;
			<OnHandScanned>d__.player = player;
			<OnHandScanned>d__.<>1__state = -1;
			<OnHandScanned>d__.<>t__builder.Start<EvolvingCosmeticKiosk.<OnHandScanned>d__17>(ref <OnHandScanned>d__);
		}

		public void ScrollForward()
		{
			this.Scroll(1);
		}

		public void ScrollBackward()
		{
			this.Scroll(-1);
		}

		private void Scroll(int direction)
		{
			this._cosmeticIdx = Math.Clamp(this._cosmeticIdx + direction, 0, this._cosmetics.Count - 1);
			this.UpdateButtonSets();
		}

		[SerializeField]
		private EvolvingCosmeticKioskButtonSet[] _buttonSets;

		private readonly List<EvolvingCosmeticKiosk.CosmeticData> _cosmetics = new List<EvolvingCosmeticKiosk.CosmeticData>();

		private int _cosmeticIdx;

		[NullableContext(1)]
		[Nullable(0)]
		private class CosmeticData : IEquatable<EvolvingCosmeticKiosk.CosmeticData>
		{
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(EvolvingCosmeticKiosk.CosmeticData);
				}
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CosmeticData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("EvolvingCosmetic = ");
				builder.Append(this.EvolvingCosmetic);
				builder.Append(", PlayfabId = ");
				builder.Append(this.PlayfabId);
				return true;
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(EvolvingCosmeticKiosk.CosmeticData left, EvolvingCosmeticKiosk.CosmeticData right)
			{
				return !(left == right);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(EvolvingCosmeticKiosk.CosmeticData left, EvolvingCosmeticKiosk.CosmeticData right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EvolvingCosmetic>.Default.GetHashCode(this.EvolvingCosmetic)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PlayfabId);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as EvolvingCosmeticKiosk.CosmeticData);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(EvolvingCosmeticKiosk.CosmeticData other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EvolvingCosmetic>.Default.Equals(this.EvolvingCosmetic, other.EvolvingCosmetic) && EqualityComparer<string>.Default.Equals(this.PlayfabId, other.PlayfabId));
			}

			[CompilerGenerated]
			protected CosmeticData(EvolvingCosmeticKiosk.CosmeticData original)
			{
				this.EvolvingCosmetic = original.EvolvingCosmetic;
				this.PlayfabId = original.PlayfabId;
			}

			public CosmeticData()
			{
			}

			[Nullable(0)]
			public EvolvingCosmetic EvolvingCosmetic;

			[Nullable(0)]
			public string PlayfabId;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;

namespace Modio.Customizations
{
	public class Agreement
	{
		public long Id { get; private set; }

		public bool IsActive { get; private set; }

		public bool IsLatest { get; private set; }

		public AgreementType Type { get; private set; }

		public DateTime DateAdded { get; private set; }

		public DateTime DateUpdated { get; private set; }

		public DateTime DateLive { get; private set; }

		public string Name { get; private set; }

		public string Changelog { get; private set; }

		public string Content { get; private set; }

		internal Agreement(AgreementVersionObject agreementObject)
		{
			this.Id = agreementObject.Id;
			this.ApplyDetailsFromAgreementObject(agreementObject);
		}

		internal Agreement ApplyDetailsFromAgreementObject(AgreementVersionObject agreementObject)
		{
			this.Name = agreementObject.Name;
			this.IsActive = agreementObject.IsActive;
			this.IsLatest = agreementObject.IsLatest;
			this.Type = (AgreementType)agreementObject.Type;
			this.Changelog = agreementObject.Changelog;
			this.Content = agreementObject.Description;
			DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(agreementObject.DateAdded);
			DateTimeOffset dateTimeOffset2 = DateTimeOffset.FromUnixTimeSeconds(agreementObject.DateUpdated);
			DateTimeOffset dateTimeOffset3 = DateTimeOffset.FromUnixTimeSeconds(agreementObject.DateLive);
			this.DateAdded = dateTimeOffset.Date;
			this.DateUpdated = dateTimeOffset2.Date;
			this.DateLive = dateTimeOffset3.Date;
			return this;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public static Task<ValueTuple<Error, Agreement>> GetAgreement(AgreementType type, bool forceUpdate = false)
		{
			Agreement.<GetAgreement>d__43 <GetAgreement>d__;
			<GetAgreement>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Agreement>>.Create();
			<GetAgreement>d__.type = type;
			<GetAgreement>d__.forceUpdate = forceUpdate;
			<GetAgreement>d__.<>1__state = -1;
			<GetAgreement>d__.<>t__builder.Start<Agreement.<GetAgreement>d__43>(ref <GetAgreement>d__);
			return <GetAgreement>d__.<>t__builder.Task;
		}

		internal static Dictionary<AgreementType, Agreement> _agreementCache = new Dictionary<AgreementType, Agreement>();
	}
}

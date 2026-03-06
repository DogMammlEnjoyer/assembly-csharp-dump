using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	internal class BindingUpdater
	{
		public bool ShouldProcessBindingAtStage(Binding bindingObject, BindingUpdateStage stage, bool versionChanged, bool dirty)
		{
			if (!true)
			{
			}
			DataBinding dataBinding = bindingObject as DataBinding;
			bool result;
			if (dataBinding == null)
			{
				CustomBinding customBinding = bindingObject as CustomBinding;
				if (customBinding == null)
				{
					throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
				}
				result = this.ShouldProcessBindingAtStage(customBinding, stage, versionChanged, dirty);
			}
			else
			{
				result = BindingUpdater.ShouldProcessBindingAtStage(dataBinding, stage, versionChanged, dirty);
			}
			if (!true)
			{
			}
			return result;
		}

		private static bool ShouldProcessBindingAtStage(DataBinding dataBinding, BindingUpdateStage stage, bool versionChanged, bool dirty)
		{
			bool result;
			if (stage != BindingUpdateStage.UpdateUI)
			{
				if (stage != BindingUpdateStage.UpdateSource)
				{
					throw new ArgumentOutOfRangeException("stage", stage, null);
				}
				BindingMode bindingMode = dataBinding.bindingMode;
				bool flag = bindingMode == BindingMode.ToTarget || bindingMode == BindingMode.ToTargetOnce;
				result = !flag;
			}
			else
			{
				bool flag2 = dataBinding.bindingMode == BindingMode.ToSource;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = dataBinding.updateTrigger == BindingUpdateTrigger.EveryUpdate || dirty;
					if (flag3)
					{
						result = true;
					}
					else
					{
						bool flag4 = dataBinding.bindingMode == BindingMode.ToTargetOnce;
						result = (!flag4 && (dataBinding.updateTrigger == BindingUpdateTrigger.OnSourceChanged && versionChanged));
					}
				}
			}
			return result;
		}

		private bool ShouldProcessBindingAtStage(CustomBinding customBinding, BindingUpdateStage stage, bool versionChanged, bool dirty)
		{
			bool result;
			if (stage != BindingUpdateStage.UpdateUI)
			{
				if (stage != BindingUpdateStage.UpdateSource)
				{
					throw new ArgumentOutOfRangeException("stage", stage, null);
				}
				result = false;
			}
			else
			{
				BindingUpdateTrigger updateTrigger = customBinding.updateTrigger;
				if (!true)
				{
				}
				bool flag;
				if (updateTrigger != BindingUpdateTrigger.OnSourceChanged)
				{
					if (updateTrigger == BindingUpdateTrigger.EveryUpdate)
					{
						flag = true;
						goto IL_3D;
					}
				}
				else if (versionChanged || dirty)
				{
					flag = true;
					goto IL_3D;
				}
				flag = dirty;
				IL_3D:
				if (!true)
				{
				}
				result = flag;
			}
			return result;
		}

		public BindingResult UpdateUI(in BindingContext context, Binding bindingObject)
		{
			if (!true)
			{
			}
			DataBinding dataBinding = bindingObject as DataBinding;
			BindingResult result;
			if (dataBinding == null)
			{
				CustomBinding customBinding = bindingObject as CustomBinding;
				if (customBinding == null)
				{
					throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
				}
				result = this.UpdateUI(context, customBinding);
			}
			else
			{
				result = this.UpdateUI(context, dataBinding);
			}
			if (!true)
			{
			}
			return result;
		}

		public BindingResult UpdateSource(in BindingContext context, Binding bindingObject)
		{
			if (!true)
			{
			}
			DataBinding dataBinding = bindingObject as DataBinding;
			BindingResult result;
			if (dataBinding == null)
			{
				CustomBinding customBinding = bindingObject as CustomBinding;
				if (customBinding == null)
				{
					throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
				}
				result = this.UpdateDataSource(context, customBinding);
			}
			else
			{
				result = this.UpdateDataSource(context, dataBinding);
			}
			if (!true)
			{
			}
			return result;
		}

		private BindingResult UpdateUI(in BindingContext context, DataBinding dataBinding)
		{
			VisualElement targetElement = context.targetElement;
			object dataSource = context.dataSource;
			bool flag = dataSource == null;
			BindingResult result;
			if (flag)
			{
				string str = string.IsNullOrEmpty(targetElement.name) ? TypeUtility.GetTypeDisplayName(targetElement.GetType()) : targetElement.name;
				string message = "[UI Toolkit] Could not bind '" + str + "' because there is no data source.";
				result = new BindingResult(BindingStatus.Pending, message);
			}
			else
			{
				PropertyPath dataSourcePath = context.dataSourcePath;
				bool isEmpty = dataSourcePath.IsEmpty;
				if (isEmpty)
				{
					bool flag2 = !TypeTraits.IsContainer(dataSource.GetType());
					if (flag2)
					{
						result = BindingUpdater.TryUpdateUIWithNonContainer(context, dataBinding, dataSource);
					}
					else
					{
						ValueTuple<bool, VisitReturnCode, BindingResult> valueTuple = BindingUpdater.VisitRoot(dataBinding, ref dataSource, context);
						bool flag3 = !valueTuple.Item1;
						if (flag3)
						{
							string visitationErrorString = BindingUpdater.GetVisitationErrorString(valueTuple.Item2, context);
							result = new BindingResult(BindingStatus.Failure, visitationErrorString);
						}
						else
						{
							result = BindingUpdater.s_VisitDataSourceAsRootVisitor.result;
						}
					}
				}
				else
				{
					BindingUpdateStage direction = BindingUpdateStage.UpdateUI;
					dataSourcePath = context.dataSourcePath;
					ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult> valueTuple2 = BindingUpdater.VisitAtPath<object>(dataBinding, direction, ref dataSource, dataSourcePath, context);
					bool flag4 = !valueTuple2.Item1;
					if (flag4)
					{
						string visitationErrorString2 = BindingUpdater.GetVisitationErrorString(valueTuple2.Item2, context);
						result = new BindingResult(BindingStatus.Failure, visitationErrorString2);
					}
					else
					{
						bool flag5 = valueTuple2.Item3 > VisitReturnCode.Ok;
						if (flag5)
						{
							VisitReturnCode item = valueTuple2.Item3;
							object dataSource2 = context.dataSource;
							dataSourcePath = context.dataSourcePath;
							string extractValueErrorString = BindingUpdater.GetExtractValueErrorString(item, dataSource2, dataSourcePath);
							result = new BindingResult(BindingStatus.Failure, extractValueErrorString);
						}
						else
						{
							result = valueTuple2.Item4;
						}
					}
				}
			}
			return result;
		}

		private BindingResult UpdateUI(in BindingContext context, CustomBinding customBinding)
		{
			return customBinding.Update(context);
		}

		private BindingResult UpdateDataSource(in BindingContext context, DataBinding dataBinding)
		{
			VisualElement targetElement = context.targetElement;
			object dataSource = context.dataSource;
			PropertyPath dataSourcePath = context.dataSourcePath;
			bool flag = dataSource == null;
			BindingResult result;
			if (flag)
			{
				string str = string.IsNullOrEmpty(targetElement.name) ? TypeUtility.GetTypeDisplayName(targetElement.GetType()) : targetElement.name;
				string message = "[UI Toolkit] Could not set value on '" + str + "' because there is no data source.";
				result = new BindingResult(BindingStatus.Pending, message);
			}
			else
			{
				bool isEmpty = dataSourcePath.IsEmpty;
				if (isEmpty)
				{
					string rootDataSourceError = BindingUpdater.GetRootDataSourceError(dataSource);
					result = new BindingResult(BindingStatus.Failure, rootDataSourceError);
				}
				else
				{
					BindingUpdateStage direction = BindingUpdateStage.UpdateSource;
					BindingId bindingId = context.bindingId;
					PropertyPath propertyPath = bindingId;
					ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult> valueTuple = BindingUpdater.VisitAtPath<VisualElement>(dataBinding, direction, ref targetElement, propertyPath, context);
					bool flag2 = !valueTuple.Item1;
					if (flag2)
					{
						string visitationErrorString = BindingUpdater.GetVisitationErrorString(valueTuple.Item2, context);
						result = new BindingResult(BindingStatus.Failure, visitationErrorString);
					}
					else
					{
						bool flag3 = valueTuple.Item3 > VisitReturnCode.Ok;
						if (flag3)
						{
							VisitReturnCode item = valueTuple.Item3;
							object target = targetElement;
							bindingId = context.bindingId;
							propertyPath = bindingId;
							string extractValueErrorString = BindingUpdater.GetExtractValueErrorString(item, target, propertyPath);
							result = new BindingResult(BindingStatus.Failure, extractValueErrorString);
						}
						else
						{
							result = valueTuple.Item4;
						}
					}
				}
			}
			return result;
		}

		private BindingResult UpdateDataSource(in BindingContext context, CustomBinding customBinding)
		{
			return new BindingResult(BindingStatus.Pending, null);
		}

		private static BindingResult TryUpdateUIWithNonContainer(in BindingContext context, DataBinding binding, object value)
		{
			Type type = value.GetType();
			bool isEnum = type.IsEnum;
			BindingResult result;
			if (isEnum)
			{
				MethodInfo methodInfo = DataBinding.updateUIMethod.MakeGenericMethod(new Type[]
				{
					type
				});
				result = (BindingResult)methodInfo.Invoke(binding, new object[]
				{
					context,
					value
				});
			}
			else
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
				{
					bool flag = (bool)value;
					return binding.UpdateUI<bool>(context, ref flag);
				}
				case TypeCode.Char:
				{
					char c = (char)value;
					return binding.UpdateUI<char>(context, ref c);
				}
				case TypeCode.SByte:
				{
					sbyte b = (sbyte)value;
					return binding.UpdateUI<sbyte>(context, ref b);
				}
				case TypeCode.Byte:
				{
					byte b2 = (byte)value;
					return binding.UpdateUI<byte>(context, ref b2);
				}
				case TypeCode.Int16:
				{
					short num = (short)value;
					return binding.UpdateUI<short>(context, ref num);
				}
				case TypeCode.UInt16:
				{
					ushort num2 = (ushort)value;
					return binding.UpdateUI<ushort>(context, ref num2);
				}
				case TypeCode.Int32:
				{
					int num3 = (int)value;
					return binding.UpdateUI<int>(context, ref num3);
				}
				case TypeCode.UInt32:
				{
					uint num4 = (uint)value;
					return binding.UpdateUI<uint>(context, ref num4);
				}
				case TypeCode.Int64:
				{
					long num5 = (long)value;
					return binding.UpdateUI<long>(context, ref num5);
				}
				case TypeCode.UInt64:
				{
					ulong num6 = (ulong)value;
					return binding.UpdateUI<ulong>(context, ref num6);
				}
				case TypeCode.Single:
				{
					float num7 = (float)value;
					return binding.UpdateUI<float>(context, ref num7);
				}
				case TypeCode.Double:
				{
					double num8 = (double)value;
					return binding.UpdateUI<double>(context, ref num8);
				}
				case TypeCode.String:
				{
					string text = (string)value;
					return binding.UpdateUI<string>(context, ref text);
				}
				}
				result = new BindingResult(BindingStatus.Failure, "[UI Toolkit] Unsupported primitive type");
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"succeeded",
			"visitationReturnCode",
			"bindingResult"
		})]
		private static ValueTuple<bool, VisitReturnCode, BindingResult> VisitRoot(DataBinding dataBinding, ref object container, in BindingContext context)
		{
			BindingUpdater.s_VisitDataSourceAsRootVisitor.Reset();
			BindingUpdater.s_VisitDataSourceAsRootVisitor.Binding = dataBinding;
			BindingUpdater.s_VisitDataSourceAsRootVisitor.bindingContext = context;
			VisitReturnCode item2;
			bool item = PropertyContainer.TryAccept<object>(BindingUpdater.s_VisitDataSourceAsRootVisitor, ref container, out item2, default(VisitParameters));
			return new ValueTuple<bool, VisitReturnCode, BindingResult>(item, item2, BindingUpdater.s_VisitDataSourceAsRootVisitor.result);
		}

		[return: TupleElementNames(new string[]
		{
			"succeeded",
			"visitationReturnCode",
			"atPathReturnCode",
			"bindingResult"
		})]
		private static ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult> VisitAtPath<TContainer>(DataBinding dataBinding, BindingUpdateStage direction, ref TContainer container, in PropertyPath path, in BindingContext context)
		{
			BindingUpdater.s_VisitDataSourceAtPathVisitor.Reset();
			BindingUpdater.s_VisitDataSourceAtPathVisitor.binding = dataBinding;
			BindingUpdater.s_VisitDataSourceAtPathVisitor.direction = direction;
			BindingUpdater.s_VisitDataSourceAtPathVisitor.Path = path;
			BindingUpdater.s_VisitDataSourceAtPathVisitor.bindingContext = context;
			VisitReturnCode item2;
			bool item = PropertyContainer.TryAccept<TContainer>(BindingUpdater.s_VisitDataSourceAtPathVisitor, ref container, out item2, default(VisitParameters));
			return new ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult>(item, item2, BindingUpdater.s_VisitDataSourceAtPathVisitor.ReturnCode, BindingUpdater.s_VisitDataSourceAtPathVisitor.result);
		}

		internal static string GetVisitationErrorString(VisitReturnCode returnCode, in BindingContext context)
		{
			string str = string.Format("[UI Toolkit] Could not bind target of type '<b>{0}</b>' at path '<b>{1}</b>':", context.targetElement.GetType().Name, context.bindingId);
			string result;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidCast:
			case VisitReturnCode.AccessViolation:
				throw new InvalidOperationException(str + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.InvalidContainerType:
				result = str + " the data source cannot be a primitive, a string or an enum.";
				break;
			case VisitReturnCode.MissingPropertyBag:
				result = str + " the data source is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				result = str + " the path from the data source to the target is either invalid or contains a null value.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		internal static string GetExtractValueErrorString(VisitReturnCode returnCode, object target, in PropertyPath path)
		{
			string str = string.Format("[UI Toolkit] Could not retrieve the value at path '<b>{0}</b>' for source of type '<b>{1}</b>':", path, (target != null) ? target.GetType().Name : null);
			string result;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidCast:
			case VisitReturnCode.AccessViolation:
				throw new InvalidOperationException(str + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.InvalidContainerType:
				result = str + " the source cannot be a primitive, a string or an enum.";
				break;
			case VisitReturnCode.MissingPropertyBag:
				result = str + " the source is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				result = str + " the path from the source to the target is either invalid or contains a null value.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		internal static string GetSetValueErrorString(VisitReturnCode returnCode, object source, in PropertyPath sourcePath, object target, in PropertyPath targetPath, object extractedValueFromSource)
		{
			string text = string.Format("[UI Toolkit] Could not set value for target of type '<b>{0}</b>' at path '<b>{1}</b>':", target.GetType().Name, targetPath);
			string result;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidContainerType:
				throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.MissingPropertyBag:
				result = text + " the type '" + target.GetType().Name + "' is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				result = text + " the path is either invalid or contains a null value.";
				break;
			case VisitReturnCode.InvalidCast:
			{
				bool isEmpty = sourcePath.IsEmpty;
				if (isEmpty)
				{
					object obj;
					bool flag = PropertyContainer.TryGetValue<object, object>(ref target, targetPath, out obj) && obj != null;
					if (flag)
					{
						result = ((extractedValueFromSource == null) ? (text + " could not convert from '<b>null</b>' to '<b>" + obj.GetType().Name + "</b>'.") : string.Concat(new string[]
						{
							text,
							" could not convert from type '<b>",
							extractedValueFromSource.GetType().Name,
							"</b>' to type '<b>",
							obj.GetType().Name,
							"</b>'."
						}));
						break;
					}
				}
				IProperty property;
				bool flag2 = PropertyContainer.TryGetProperty<object>(ref source, sourcePath, out property);
				if (flag2)
				{
					object obj2;
					bool flag3 = PropertyContainer.TryGetValue<object, object>(ref target, targetPath, out obj2) && obj2 != null;
					if (flag3)
					{
						result = ((extractedValueFromSource == null) ? string.Concat(new string[]
						{
							text,
							" could not convert from '<b>null (",
							property.DeclaredValueType().Name,
							")</b>' to '<b>",
							obj2.GetType().Name,
							"</b>'."
						}) : string.Concat(new string[]
						{
							text,
							" could not convert from type '<b>",
							extractedValueFromSource.GetType().Name,
							"</b>' to type '<b>",
							obj2.GetType().Name,
							"</b>'."
						}));
						break;
					}
				}
				result = text + " conversion failed.";
				break;
			}
			case VisitReturnCode.AccessViolation:
				result = text + " the path is read-only.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		internal static string GetRootDataSourceError(object target)
		{
			return "[UI Toolkit] Could not set value for target of type '<b>" + target.GetType().Name + "</b>': no path was provided.";
		}

		private static readonly BindingUpdater.CastDataSourceVisitor s_VisitDataSourceAsRootVisitor = new BindingUpdater.CastDataSourceVisitor();

		private static readonly BindingUpdater.UIPathVisitor s_VisitDataSourceAtPathVisitor = new BindingUpdater.UIPathVisitor();

		private sealed class CastDataSourceVisitor : ConcreteTypeVisitor
		{
			public DataBinding Binding { get; set; }

			public BindingContext bindingContext { get; set; }

			public BindingResult result { get; set; }

			public void Reset()
			{
				this.Binding = null;
				this.bindingContext = default(BindingContext);
				this.result = default(BindingResult);
			}

			protected override void VisitContainer<TContainer>(ref TContainer container)
			{
				DataBinding binding = this.Binding;
				BindingContext bindingContext = this.bindingContext;
				this.result = binding.UpdateUI<TContainer>(bindingContext, ref container);
			}
		}

		private sealed class UIPathVisitor : PathVisitor
		{
			public DataBinding binding { get; set; }

			public BindingUpdateStage direction { get; set; }

			public BindingContext bindingContext { get; set; }

			public BindingResult result { get; set; }

			public override void Reset()
			{
				base.Reset();
				this.binding = null;
				this.direction = BindingUpdateStage.UpdateUI;
				this.bindingContext = default(BindingContext);
				this.result = default(BindingResult);
				base.ReadonlyVisit = true;
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				BindingUpdateStage direction = this.direction;
				if (!true)
				{
				}
				BindingResult result;
				if (direction != BindingUpdateStage.UpdateUI)
				{
					if (direction != BindingUpdateStage.UpdateSource)
					{
						throw new ArgumentOutOfRangeException();
					}
					DataBinding binding = this.binding;
					BindingContext bindingContext = this.bindingContext;
					result = binding.UpdateSource<TValue>(bindingContext, ref value);
				}
				else
				{
					DataBinding binding2 = this.binding;
					BindingContext bindingContext = this.bindingContext;
					result = binding2.UpdateUI<TValue>(bindingContext, ref value);
				}
				if (!true)
				{
				}
				this.result = result;
			}
		}
	}
}

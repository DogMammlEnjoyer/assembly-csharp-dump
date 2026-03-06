using System;
using System.Collections.Generic;
using System.Linq;

namespace Modio
{
	public static class ModioServices
	{
		public static ModioServices.IBindType<T> Bind<T>()
		{
			ModioServices.ServiceBindings serviceBindings;
			if (!ModioServices.Bindings.TryGetValue(typeof(T), out serviceBindings))
			{
				serviceBindings = (ModioServices.Bindings[typeof(T)] = new ModioServices.ServiceBindings<T>());
			}
			return (ModioServices.IBindType<T>)serviceBindings;
		}

		public static void BindInstance<T>(T instance, ModioServicePriority priority = ModioServicePriority.DeveloperOverride)
		{
			ModioServices.Bind<T>().FromInstance(instance, priority, null);
		}

		public static void BindErrorMessage<T>(string message, ModioServicePriority priority = ModioServicePriority.Fallback)
		{
			ModioServices.Bind<T>().FromMethod(delegate
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(message);
				}
				throw new KeyNotFoundException("Could not resolve type " + typeof(T).FullName + ". " + message);
			}, priority, null);
		}

		internal static void RemoveAllBindingsWithPriority(ModioServicePriority priority)
		{
			foreach (Type key in new List<Type>(ModioServices.Bindings.Keys))
			{
				ModioServices.ServiceBindings serviceBindings = ModioServices.Bindings[key];
				serviceBindings.RemoveAllWithPriority(priority);
				if (serviceBindings.BindingCount == 0)
				{
					ModioServices.Bindings.Remove(key);
				}
			}
		}

		public static T Resolve<T>()
		{
			return ModioServices.GetBindings<T>(false).Resolve();
		}

		public static bool TryResolve<T>(out T result)
		{
			ModioServices.ServiceBindings serviceBindings;
			if (!ModioServices.Bindings.TryGetValue(typeof(T), out serviceBindings))
			{
				result = default(T);
				return false;
			}
			return ((ModioServices.ServiceBindings<T>)serviceBindings).TryResolve(out result);
		}

		public static ModioServices.IResolveType<T> GetBindings<T>(bool createIfMissing = false)
		{
			ModioServices.ServiceBindings serviceBindings;
			if (!ModioServices.Bindings.TryGetValue(typeof(T), out serviceBindings))
			{
				if (!createIfMissing)
				{
					throw new KeyNotFoundException("Could not resolve type " + typeof(T).FullName);
				}
				serviceBindings = (ModioServices.Bindings[typeof(T)] = new ModioServices.ServiceBindings<T>());
			}
			return (ModioServices.ServiceBindings<T>)serviceBindings;
		}

		public static void AddBindingChangedListener<T>(Action<T> onNewValue, bool fireImmediatelyIfValueBound = true)
		{
			ModioServices.IResolveType<T> bindings = ModioServices.GetBindings<T>(true);
			bindings.OnNewBinding += onNewValue;
			T obj;
			if (fireImmediatelyIfValueBound && bindings.TryResolve(out obj))
			{
				onNewValue(obj);
			}
		}

		public static void RemoveBindingChangedListener<T>(Action<T> onNewValue)
		{
			ModioServices.GetBindings<T>(true).OnNewBinding -= onNewValue;
		}

		private static readonly Dictionary<Type, ModioServices.ServiceBindings> Bindings = new Dictionary<Type, ModioServices.ServiceBindings>();

		public interface IBindType<T>
		{
			ModioServices.Binding<T> FromInstance(T value, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null);

			ModioServices.Binding<T> FromMethod(Func<T> factory, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null);

			ModioServices.Binding<T> FromNew<TResolved>(ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null) where TResolved : T, new();

			ModioServices.Binding<T> FromNew(Type type, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null);

			ModioServices.Binding<T> WithOtherBinding<TOther>(ModioServices.Binding<TOther> binding, Func<bool> condition = null);

			ModioServices.IBindType<T> WithInterfaces<TI1>(Func<bool> condition = null);

			ModioServices.IBindType<T> WithInterfaces<TI1, TI2>(Func<bool> condition = null);

			ModioServices.IBindType<T> WithInterfaces<TI1, TI2, TI3>(Func<bool> condition = null);
		}

		public interface IResolveType<T>
		{
			T Resolve();

			bool TryResolve(out T value);

			event Action<T> OnNewBinding;

			IEnumerable<ValueTuple<T, ModioServicePriority>> ResolveAll();
		}

		private abstract class ServiceBindings
		{
			public abstract void RemoveAllWithPriority(ModioServicePriority priority);

			public abstract int BindingCount { get; }
		}

		public class Binding<T>
		{
			public Binding(T value, ModioServicePriority priority, Func<bool> condition = null)
			{
				this._value = value;
				this.Priority = priority;
				this.Condition = condition;
			}

			public Binding(Func<T> factory, ModioServicePriority priority, Func<bool> condition = null)
			{
				this._factory = factory;
				this.Priority = priority;
				this.Condition = condition;
			}

			public T Resolve()
			{
				if (this._value != null || this._factory == null)
				{
					return this._value;
				}
				if (this._runningFactoryMethod)
				{
					ModioLog error = ModioLog.Error;
					if (error != null)
					{
						error.Log("Cyclic dependency detected when resolving type " + typeof(T).FullName + ". This will cause issues.");
					}
					return default(T);
				}
				this._runningFactoryMethod = true;
				try
				{
					this._value = this._factory();
				}
				finally
				{
					this._runningFactoryMethod = false;
				}
				return this._value;
			}

			public readonly ModioServicePriority Priority;

			public readonly Func<bool> Condition;

			private readonly Func<T> _factory;

			private T _value;

			private bool _runningFactoryMethod;
		}

		private class ServiceBindings<T> : ModioServices.ServiceBindings, ModioServices.IBindType<T>, ModioServices.IResolveType<T>
		{
			public override int BindingCount
			{
				get
				{
					return this.Bindings.Count;
				}
			}

			public event Action<T> OnNewBinding;

			public ModioServices.Binding<T> FromInstance(T value, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null)
			{
				ModioServices.Binding<T> binding = new ModioServices.Binding<T>(value, priority, condition);
				this.Bindings.Add(binding);
				this.InvokeNewBindingIfHighestPriority(priority);
				return binding;
			}

			public ModioServices.Binding<T> FromMethod(Func<T> factory, ModioServicePriority priority, Func<bool> condition = null)
			{
				ModioServices.Binding<T> binding = new ModioServices.Binding<T>(factory, priority, condition);
				this.Bindings.Add(binding);
				this.InvokeNewBindingIfHighestPriority(priority);
				return binding;
			}

			public ModioServices.Binding<T> FromNew<TResolved>(ModioServicePriority priority, Func<bool> condition = null) where TResolved : T, new()
			{
				return this.FromMethod(() => (T)((object)Activator.CreateInstance<TResolved>()), priority, condition);
			}

			public ModioServices.Binding<T> FromNew(Type type, ModioServicePriority priority, Func<bool> condition = null)
			{
				if (!typeof(T).IsAssignableFrom(type))
				{
					throw new ArgumentException(string.Concat(new string[]
					{
						"Type '",
						type.FullName,
						"' is not assignable to '",
						typeof(T).FullName,
						"'"
					}));
				}
				return this.FromMethod(() => (T)((object)Activator.CreateInstance(type)), priority, condition);
			}

			public ModioServices.Binding<T> WithOtherBinding<TOther>(ModioServices.Binding<TOther> binding, Func<bool> condition = null)
			{
				if (!typeof(T).IsAssignableFrom(typeof(TOther)))
				{
					throw new ArgumentException(string.Concat(new string[]
					{
						"Type '",
						typeof(T).FullName,
						"' is not assignable to '",
						typeof(TOther).FullName,
						"'"
					}));
				}
				if (condition == null)
				{
					condition = binding.Condition;
				}
				else if (binding.Condition != null)
				{
					condition = (() => condition() && binding.Condition());
				}
				return this.FromMethod(() => (T)((object)binding.Resolve()), binding.Priority, condition);
			}

			public ModioServices.IBindType<T> WithInterfaces<TI1>(Func<bool> condition = null)
			{
				return new ModioServices.ServiceBindings<T>.MultiBind(this, delegate(ModioServices.Binding<T> b)
				{
					ModioServices.Bind<TI1>().WithOtherBinding<T>(b, condition);
				});
			}

			public ModioServices.IBindType<T> WithInterfaces<TI1, TI2>(Func<bool> condition = null)
			{
				return new ModioServices.ServiceBindings<T>.MultiBind(this, delegate(ModioServices.Binding<T> b)
				{
					ModioServices.Bind<TI1>().WithOtherBinding<T>(b, condition);
					ModioServices.Bind<TI2>().WithOtherBinding<T>(b, condition);
				});
			}

			public ModioServices.IBindType<T> WithInterfaces<TI1, TI2, TI3>(Func<bool> condition = null)
			{
				return new ModioServices.ServiceBindings<T>.MultiBind(this, delegate(ModioServices.Binding<T> b)
				{
					ModioServices.Bind<TI1>().WithOtherBinding<T>(b, condition);
					ModioServices.Bind<TI2>().WithOtherBinding<T>(b, condition);
					ModioServices.Bind<TI3>().WithOtherBinding<T>(b, condition);
				});
			}

			public override void RemoveAllWithPriority(ModioServicePriority priority)
			{
				for (int i = this.Bindings.Count - 1; i >= 0; i--)
				{
					if (this.Bindings[i].Priority == priority)
					{
						this.Bindings.RemoveAt(i);
					}
				}
			}

			private void InvokeNewBindingIfHighestPriority(ModioServicePriority priority)
			{
				if (this.OnNewBinding == null)
				{
					return;
				}
				using (List<ModioServices.Binding<T>>.Enumerator enumerator = this.Bindings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Priority > priority)
						{
							return;
						}
					}
				}
				T obj;
				if (this.TryResolve(out obj))
				{
					this.OnNewBinding(obj);
				}
			}

			public T Resolve()
			{
				T result;
				if (!this.TryResolve(out result))
				{
					throw new KeyNotFoundException("Could not resolve type " + typeof(T).FullName);
				}
				return result;
			}

			public bool TryResolve(out T value)
			{
				ModioServicePriority? modioServicePriority = null;
				ModioServices.Binding<T> binding = null;
				foreach (ModioServices.Binding<T> binding2 in this.Bindings)
				{
					if ((modioServicePriority == null || modioServicePriority.Value <= binding2.Priority) && (binding2.Condition == null || binding2.Condition()))
					{
						modioServicePriority = new ModioServicePriority?(binding2.Priority);
						binding = binding2;
					}
				}
				if (modioServicePriority == null)
				{
					value = default(T);
					return false;
				}
				value = binding.Resolve();
				return true;
			}

			public IEnumerable<ValueTuple<T, ModioServicePriority>> ResolveAll()
			{
				return from b in this.Bindings
				where b.Condition == null || b.Condition()
				select new ValueTuple<T, ModioServicePriority>(b.Resolve(), b.Priority);
			}

			public readonly List<ModioServices.Binding<T>> Bindings = new List<ModioServices.Binding<T>>();

			private class MultiBind : ModioServices.IBindType<T>
			{
				public MultiBind(ModioServices.ServiceBindings<T> coreBinding, Action<ModioServices.Binding<T>> afterBinding)
				{
					this._coreBinding = coreBinding;
					this._afterBinding = afterBinding;
				}

				public ModioServices.Binding<T> FromInstance(T value, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null)
				{
					return this.BindWith(this._coreBinding.FromInstance(value, priority, condition));
				}

				public ModioServices.Binding<T> FromMethod(Func<T> factory, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null)
				{
					return this.BindWith(this._coreBinding.FromMethod(factory, priority, condition));
				}

				public ModioServices.Binding<T> FromNew<TResolved>(ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null) where TResolved : T, new()
				{
					return this.BindWith(this._coreBinding.FromNew<TResolved>(priority, condition));
				}

				public ModioServices.Binding<T> FromNew(Type type, ModioServicePriority priority = ModioServicePriority.DeveloperOverride, Func<bool> condition = null)
				{
					return this.BindWith(this._coreBinding.FromNew(type, priority, condition));
				}

				public ModioServices.Binding<T> WithOtherBinding<TOther>(ModioServices.Binding<TOther> binding, Func<bool> condition = null)
				{
					return this.BindWith(this._coreBinding.WithOtherBinding<TOther>(binding, condition));
				}

				public ModioServices.IBindType<T> WithInterfaces<TI1>(Func<bool> condition = null)
				{
					return new ModioServices.ServiceBindings<T>.MultiBind(this._coreBinding, delegate(ModioServices.Binding<T> b)
					{
						this._afterBinding(b);
						ModioServices.Bind<TI1>().WithOtherBinding<T>(b, condition);
					});
				}

				public ModioServices.IBindType<T> WithInterfaces<TI1, TI2>(Func<bool> condition = null)
				{
					return new ModioServices.ServiceBindings<T>.MultiBind(this._coreBinding, delegate(ModioServices.Binding<T> b)
					{
						this._afterBinding(b);
						ModioServices.Bind<TI1>().WithOtherBinding<T>(b, condition);
						ModioServices.Bind<TI2>().WithOtherBinding<T>(b, condition);
					});
				}

				public ModioServices.IBindType<T> WithInterfaces<TI1, TI2, TI3>(Func<bool> condition = null)
				{
					return new ModioServices.ServiceBindings<T>.MultiBind(this._coreBinding, delegate(ModioServices.Binding<T> b)
					{
						this._afterBinding(b);
						ModioServices.Bind<TI1>().WithOtherBinding<T>(b, condition);
						ModioServices.Bind<TI2>().WithOtherBinding<T>(b, condition);
						ModioServices.Bind<TI3>().WithOtherBinding<T>(b, condition);
					});
				}

				private ModioServices.Binding<T> BindWith(ModioServices.Binding<T> core)
				{
					this._afterBinding(core);
					return core;
				}

				private readonly ModioServices.ServiceBindings<T> _coreBinding;

				private readonly Action<ModioServices.Binding<T>> _afterBinding;
			}
		}
	}
}

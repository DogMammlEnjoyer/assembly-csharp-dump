using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Cysharp.Threading.Tasks
{
	public static class UnityBindingExtensions
	{
		public static void BindTo(this IUniTaskAsyncEnumerable<string> source, Text text, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		public static void BindTo(this IUniTaskAsyncEnumerable<string> source, Text text, CancellationToken cancellationToken, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore(source, text, cancellationToken, rebindOnError).Forget();
		}

		private static UniTaskVoid BindToCore(IUniTaskAsyncEnumerable<string> source, Text text, CancellationToken cancellationToken, bool rebindOnError)
		{
			UnityBindingExtensions.<BindToCore>d__2 <BindToCore>d__;
			<BindToCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<BindToCore>d__.source = source;
			<BindToCore>d__.text = text;
			<BindToCore>d__.cancellationToken = cancellationToken;
			<BindToCore>d__.rebindOnError = rebindOnError;
			<BindToCore>d__.<>1__state = -1;
			<BindToCore>d__.<>t__builder.Start<UnityBindingExtensions.<BindToCore>d__2>(ref <BindToCore>d__);
			return <BindToCore>d__.<>t__builder.Task;
		}

		public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, Text text, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore<T>(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, Text text, CancellationToken cancellationToken, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore<T>(source, text, cancellationToken, rebindOnError).Forget();
		}

		public static void BindTo<T>(this AsyncReactiveProperty<T> source, Text text, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore<T>(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		private static UniTaskVoid BindToCore<T>(IUniTaskAsyncEnumerable<T> source, Text text, CancellationToken cancellationToken, bool rebindOnError)
		{
			UnityBindingExtensions.<BindToCore>d__6<T> <BindToCore>d__;
			<BindToCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<BindToCore>d__.source = source;
			<BindToCore>d__.text = text;
			<BindToCore>d__.cancellationToken = cancellationToken;
			<BindToCore>d__.rebindOnError = rebindOnError;
			<BindToCore>d__.<>1__state = -1;
			<BindToCore>d__.<>t__builder.Start<UnityBindingExtensions.<BindToCore>d__6<T>>(ref <BindToCore>d__);
			return <BindToCore>d__.<>t__builder.Task;
		}

		public static void BindTo(this IUniTaskAsyncEnumerable<bool> source, Selectable selectable, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore(source, selectable, selectable.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		public static void BindTo(this IUniTaskAsyncEnumerable<bool> source, Selectable selectable, CancellationToken cancellationToken, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore(source, selectable, cancellationToken, rebindOnError).Forget();
		}

		private static UniTaskVoid BindToCore(IUniTaskAsyncEnumerable<bool> source, Selectable selectable, CancellationToken cancellationToken, bool rebindOnError)
		{
			UnityBindingExtensions.<BindToCore>d__9 <BindToCore>d__;
			<BindToCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<BindToCore>d__.source = source;
			<BindToCore>d__.selectable = selectable;
			<BindToCore>d__.cancellationToken = cancellationToken;
			<BindToCore>d__.rebindOnError = rebindOnError;
			<BindToCore>d__.<>1__state = -1;
			<BindToCore>d__.<>t__builder.Start<UnityBindingExtensions.<BindToCore>d__9>(ref <BindToCore>d__);
			return <BindToCore>d__.<>t__builder.Task;
		}

		public static void BindTo<TSource, TObject>(this IUniTaskAsyncEnumerable<TSource> source, TObject monoBehaviour, Action<TObject, TSource> bindAction, bool rebindOnError = true) where TObject : MonoBehaviour
		{
			UnityBindingExtensions.BindToCore<TSource, TObject>(source, monoBehaviour, bindAction, monoBehaviour.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		public static void BindTo<TSource, TObject>(this IUniTaskAsyncEnumerable<TSource> source, TObject bindTarget, Action<TObject, TSource> bindAction, CancellationToken cancellationToken, bool rebindOnError = true)
		{
			UnityBindingExtensions.BindToCore<TSource, TObject>(source, bindTarget, bindAction, cancellationToken, rebindOnError).Forget();
		}

		private static UniTaskVoid BindToCore<TSource, TObject>(IUniTaskAsyncEnumerable<TSource> source, TObject bindTarget, Action<TObject, TSource> bindAction, CancellationToken cancellationToken, bool rebindOnError)
		{
			UnityBindingExtensions.<BindToCore>d__12<TSource, TObject> <BindToCore>d__;
			<BindToCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<BindToCore>d__.source = source;
			<BindToCore>d__.bindTarget = bindTarget;
			<BindToCore>d__.bindAction = bindAction;
			<BindToCore>d__.cancellationToken = cancellationToken;
			<BindToCore>d__.rebindOnError = rebindOnError;
			<BindToCore>d__.<>1__state = -1;
			<BindToCore>d__.<>t__builder.Start<UnityBindingExtensions.<BindToCore>d__12<TSource, TObject>>(ref <BindToCore>d__);
			return <BindToCore>d__.<>t__builder.Task;
		}
	}
}

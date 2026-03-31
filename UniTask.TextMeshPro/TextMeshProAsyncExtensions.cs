using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;
using TMPro;

namespace Cysharp.Threading.Tasks
{
	public static class TextMeshProAsyncExtensions
	{
		public static void BindTo(this IUniTaskAsyncEnumerable<string> source, TMP_Text text, bool rebindOnError = true)
		{
			TextMeshProAsyncExtensions.BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		public static void BindTo(this IUniTaskAsyncEnumerable<string> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError = true)
		{
			TextMeshProAsyncExtensions.BindToCore(source, text, cancellationToken, rebindOnError).Forget();
		}

		private static UniTaskVoid BindToCore(IUniTaskAsyncEnumerable<string> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError)
		{
			TextMeshProAsyncExtensions.<BindToCore>d__2 <BindToCore>d__;
			<BindToCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<BindToCore>d__.source = source;
			<BindToCore>d__.text = text;
			<BindToCore>d__.cancellationToken = cancellationToken;
			<BindToCore>d__.rebindOnError = rebindOnError;
			<BindToCore>d__.<>1__state = -1;
			<BindToCore>d__.<>t__builder.Start<TextMeshProAsyncExtensions.<BindToCore>d__2>(ref <BindToCore>d__);
			return <BindToCore>d__.<>t__builder.Task;
		}

		public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, TMP_Text text, bool rebindOnError = true)
		{
			TextMeshProAsyncExtensions.BindToCore<T>(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError = true)
		{
			TextMeshProAsyncExtensions.BindToCore<T>(source, text, cancellationToken, rebindOnError).Forget();
		}

		public static void BindTo<T>(this AsyncReactiveProperty<T> source, TMP_Text text, bool rebindOnError = true)
		{
			TextMeshProAsyncExtensions.BindToCore<T>(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
		}

		private static UniTaskVoid BindToCore<T>(IUniTaskAsyncEnumerable<T> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError)
		{
			TextMeshProAsyncExtensions.<BindToCore>d__6<T> <BindToCore>d__;
			<BindToCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<BindToCore>d__.source = source;
			<BindToCore>d__.text = text;
			<BindToCore>d__.cancellationToken = cancellationToken;
			<BindToCore>d__.rebindOnError = rebindOnError;
			<BindToCore>d__.<>1__state = -1;
			<BindToCore>d__.<>t__builder.Start<TextMeshProAsyncExtensions.<BindToCore>d__6<T>>(ref <BindToCore>d__);
			return <BindToCore>d__.<>t__builder.Task;
		}

		public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, false);
		}

		public static UniTask<string> OnValueChangedAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnValueChangedAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, cancellationToken);
		}

		public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, false);
		}

		public static UniTask<string> OnEndEditAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnEndEditAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, cancellationToken);
		}

		public static IAsyncEndTextSelectionEventHandler<ValueTuple<string, int, int>> GetAsyncEndTextSelectionEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onEndTextSelection), inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncEndTextSelectionEventHandler<ValueTuple<string, int, int>> GetAsyncEndTextSelectionEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onEndTextSelection), cancellationToken, false);
		}

		public static UniTask<ValueTuple<string, int, int>> OnEndTextSelectionAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onEndTextSelection), inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<ValueTuple<string, int, int>> OnEndTextSelectionAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onEndTextSelection), cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<ValueTuple<string, int, int>> OnEndTextSelectionAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onEndTextSelection), inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<ValueTuple<string, int, int>> OnEndTextSelectionAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onEndTextSelection), cancellationToken);
		}

		public static IAsyncTextSelectionEventHandler<ValueTuple<string, int, int>> GetAsyncTextSelectionEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onTextSelection), inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncTextSelectionEventHandler<ValueTuple<string, int, int>> GetAsyncTextSelectionEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onTextSelection), cancellationToken, false);
		}

		public static UniTask<ValueTuple<string, int, int>> OnTextSelectionAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onTextSelection), inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<ValueTuple<string, int, int>> OnTextSelectionAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onTextSelection), cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<ValueTuple<string, int, int>> OnTextSelectionAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onTextSelection), inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<ValueTuple<string, int, int>> OnTextSelectionAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<ValueTuple<string, int, int>>(new TextSelectionEventConverter(inputField.onTextSelection), cancellationToken);
		}

		public static IAsyncDeselectEventHandler<string> GetAsyncDeselectEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onDeselect, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncDeselectEventHandler<string> GetAsyncDeselectEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onDeselect, cancellationToken, false);
		}

		public static UniTask<string> OnDeselectAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onDeselect, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnDeselectAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onDeselect, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnDeselectAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onDeselect, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnDeselectAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onDeselect, cancellationToken);
		}

		public static IAsyncSelectEventHandler<string> GetAsyncSelectEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSelect, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncSelectEventHandler<string> GetAsyncSelectEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSelect, cancellationToken, false);
		}

		public static UniTask<string> OnSelectAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSelect, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnSelectAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSelect, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnSelectAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSelect, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnSelectAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSelect, cancellationToken);
		}

		public static IAsyncSubmitEventHandler<string> GetAsyncSubmitEventHandler(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSubmit, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncSubmitEventHandler<string> GetAsyncSubmitEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSubmit, cancellationToken, false);
		}

		public static UniTask<string> OnSubmitAsync(this TMP_InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSubmit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnSubmitAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onSubmit, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnSubmitAsAsyncEnumerable(this TMP_InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSubmit, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnSubmitAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSubmit, cancellationToken);
		}
	}
}

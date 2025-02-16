using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Common.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Common.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("c23f1588-7af9-4d9e-83af-15e922501b7f")]

[assembly: SuppressMessage("Microsoft.Design",
                           "CA1045",
                           Scope = "member",
                           Target = "~M:BadEcho.Serialization.JsonPolymorphicConverter`2.ReadFromDescriptor(System.Text.Json.Utf8JsonReader@,`0)~`1",
                           Justification = "System.Text.Json.Serialization.JsonConverter design is centered around the use of the System.Text.Json.Utf8JsonReader being passed around by reference.")]

[assembly: SuppressMessage("Style", 
                           "IDE0022:Use expression body for methods",
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensions.ReflectionExtensions.GetAttribute``1(System.Reflection.ICustomAttributeProvider)~``0",
                           Justification = "Rather disgusting when a generic type constraint is involved.")]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Collections.LazyConcurrentDictionary`2",
                           Justification = "All the constructors make calls to base constructors, and expression bodies don't look appetizing at all next to such invocations.")]

[assembly: SuppressMessage("Performance", 
                           "CA1810:Initialize reference type static fields inline", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Logging.Logger.#cctor",
                           Justification = "Normally this is a good idea, however we must guarantee the listener's initialization upon any method of this class being used so it can capture the messages; static fields that are initialized inline are marked with the beforefieldinit flag, and don't actually get initialized until code is called that accesses the static field in question. Because the listener isn't actually used directly by the messaging methods of this class, it'll actually never end up being initialized so it can do its job. The static constructor is the only way; the convenience offered by this class for the crucial area of concern of diagnostic messaging is large, and merits us doing things just a tad bit differently, as long as we go about it carefully and correctly.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter",
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensions.ObjectExtensions.GetHashCode``2(``0,``1)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensions.ObjectExtensions.GetHashCode``3(``0,``1,``2)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter",
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensions.ObjectExtensions.GetHashCode``4(``0,``1,``2,``3)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensions.ObjectExtensions.GetHashCode``5(``0,``1,``2,``3,``4)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors",
                           Scope = "member", 
                           Target = "~M:BadEcho.Interop.WindowHandle.#ctor(System.IntPtr,System.Boolean)",
                           Justification = "The constructor makes a call to a constructor overload, and expression bodies don't look appetizing at all next to such an invocation.")]

[assembly: SuppressMessage("Globalization", 
                           "CA2101:Specify marshaling for P/Invoke string arguments", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Interop.Kernel32.GetProcAddress(System.IntPtr,System.String)~System.IntPtr",
                           Justification = "This is really a false positive -- ThrowOnUnmappableChar is set to true, closing any potential security hole in regards to Unicode characters being converted into dangerous ANSI characters. Only explicitly specifying Unicode marshalling for the string parameter causes the warning to go away, which is problematic as no Unicode variant of GetProcAddress exists. Will regard as code analysis bug for now: https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2101")]

[assembly: SuppressMessage("Design", 
                           "CA1031:Do not catch general exception types",
                           Scope = "member", 
                           Target = "~M:BadEcho.Threading.ThreadExecutorOperation.Execute",
                           Justification = "The exception grabbed here is essentially 'rethrown' when it is propagated to the operation's underlying Task via its TaskCompletionSource -- nothing is being suppressed here.")]

[assembly: SuppressMessage("Maintainability", 
                           "CA1508:Avoid dead conditional code", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Interop.MessageOnlyWindowWrapper.#ctor(BadEcho.Threading.IThreadExecutor)",
                           Justification = "This is a false positive, with several issues open that seem related to it on the dotnet GitHub. Even though the CreateWindowEx function returns a non-null reference type, the Handle property will of course still be null if an exception was thrown in the try block.")]

[assembly: SuppressMessage("Design", 
                           "CA1028:Enum Storage should be Int32", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Interop.WindowStyles",
                           Justification = "Not possible, window style flags use values that exceed the maximum value of a signed integer.")]

[assembly: SuppressMessage("Style", 
                           "IDE0251:Make member 'readonly'", 
                           Scope = "member", 
                           Target = "~P:BadEcho.Interop.NotifyIconDataMarshaller.ManagedToUnmanagedRef.NOTIFYICONDATAW.Tip",
                           Justification = "This is technically an incorrect recommendation. Although, at the time of writing, a readonly modifier here would compile fine, this setter is in fact modifying the state of this struct. The state is being modified through the copying of the input value's contents into a fixed buffer, whose location in memory is being pointed to by the Span<T> returned by the SzTip property.")]

[assembly: SuppressMessage("Style", 
                           "IDE0251:Make member 'readonly'", 
                           Scope = "member", 
                           Target = "~P:BadEcho.Interop.NotifyIconDataMarshaller.ManagedToUnmanagedRef.NOTIFYICONDATAW.Info",
                           Justification = "This is technically an incorrect recommendation. Although, at the time of writing, a readonly modifier here would compile fine, this setter is in fact modifying the state of this struct. The state is being modified through the copying of the input value's contents into a fixed buffer, whose location in memory is being pointed to by the Span<T> returned by the SzInfo property.")]

[assembly: SuppressMessage("Style",
                           "IDE0251:Make member 'readonly'",
                           Scope = "member",
                           Target = "~P:BadEcho.Interop.NotifyIconDataMarshaller.ManagedToUnmanagedRef.NOTIFYICONDATAW.InfoTitle",
                           Justification = "This is technically an incorrect recommendation. Although, at the time of writing, a readonly modifier here would compile fine, this setter is in fact modifying the state of this struct. The state is being modified through the copying of the input value's contents into a fixed buffer, whose location in memory is being pointed to by the Span<T> returned by the SzInfoTitle property.")]

[assembly: SuppressMessage("Design", 
                           "CA1008:Enums should have zero value", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Interop.WindowStyles",
                           Justification = "One must not attempt to weave changes into the fabric of ancient codespace; according to official Win32 documentation, a constant value of 0x0 represents an 'overlapped' window style. Therefore, 'Overlapped' its name shall be.")]

[assembly: SuppressMessage("Performance", 
                           "CA1869:Cache and reuse 'JsonSerializerOptions' instances", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Serialization.JsonFlattenedObjectConverter`1.Read(System.Text.Json.Utf8JsonReader@,System.Type,System.Text.Json.JsonSerializerOptions)~`0",
                           Justification = "Using a cached JsonSerializerOptions is not possible here as we need to base the options do use off of what is passed in as a parameter, as there's a good chance the options that are passed in will change across invocations.")]

[assembly: SuppressMessage("Design",
                           "CA1031:Modify 'TaskDialogCallbackProc' to catch a more specific allowed exception type, or rethrow the exception",
                           Scope = "member",
                           Target = "~M:BadEcho.Interop.Dialogs.TaskDialog.TaskDialogCallbackProc(System.IntPtr,BadEcho.Interop.Dialogs.TaskDialogNotification,System.IntPtr,System.IntPtr)~BadEcho.Interop.ResultHandle",
                           Justification = "All exceptions caught here are rethrown, with the exception state preserved. However, we have to hold off rethrowing the exception until the dialog is closed, otherwise the exception will bubble up to native code and get wrapped in an SEHException.")]
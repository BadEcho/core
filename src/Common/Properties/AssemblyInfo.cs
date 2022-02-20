using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if RELEASE
using BadEcho.Odin.Properties;

[assembly: InternalsVisibleTo("BadEcho.Odin.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Odin.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("c23f1588-7af9-4d9e-83af-15e922501b7f")]

[assembly: SuppressMessage("Microsoft.Design",
                           "CA1045",
                           Scope = "member",
                           Target = "~M:BadEcho.Odin.Serialization.JsonPolymorphicConverter`2.ReadFromDescriptor(System.Text.Json.Utf8JsonReader@,`0)~`1",
                           Justification = "System.Text.Json.Serialization.JsonConverter design is centered around the use of the System.Text.Json.Utf8JsonReader being passed around by reference.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter", 
                           Scope = "member",
                           Target = "~M:BadEcho.Odin.Extensibility.Hosting.PluginContextStrategyExtensions.LoadConventions(BadEcho.Odin.Extensibility.Hosting.IPluginContextStrategy,System.Composition.Hosting.ContainerConfiguration)",
                           Justification = "This is an extension method for an interface that provides code useful to all said interface's implementations. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0016:Use 'throw' expression",                            
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensibility.Hosting.RoutableProxy.Create``1(BadEcho.Odin.Extensibility.Hosting.IHostAdapter)~``0",
                           Justification = "If method info is null, I rather an exception be thrown prior to dynamic proxy creation.")]

[assembly: SuppressMessage("Style", 
                           "IDE0022:Use expression body for methods",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensions.ReflectionExtensions.GetAttribute``1(System.Reflection.ICustomAttributeProvider)~``0",
                           Justification = "Rather disgusting when a generic type constraint is involved.")]

[assembly: SuppressMessage("Style",
                           "IDE0022:Use expression body for methods",
                           Scope = "member",
                           Target = "~M:BadEcho.Odin.Extensibility.Hosting.PluginHost.LoadAdapter``1~BadEcho.Odin.Extensibility.Hosting.HostAdapter{``0}",
                           Justification = "Rather disgusting when a generic type constraint is involved.")]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Odin.Collections.LazyConcurrentDictionary`2",
                           Justification = "All the constructors make calls to base constructors, and expression bodies don't look appetizing at all next to such invocations.")]

[assembly: SuppressMessage("Design", 
                           "CA1019:Define accessors for attribute arguments", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensibility.FilterableAttribute.#ctor(System.String,System.Type)",
                           Justification = "Accessors are available for processed constructor input, which is an acceptable reason to suppress this warning as evidenced by several of Microsoft's own attributes that build non-primitive typed properties from primitive typed parameters. Performing validation of the string to ensure it is a valid GUID saves us loads of effort down the road when dealing with imported metadata. Implementing the property explicitly and creating a string property in order to satisfy this rule does not work as it breaks MEF's MetadataViewProvider.")]

[assembly: SuppressMessage("Design", 
                           "CA1019:Define accessors for attribute arguments",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensibility.FilterableFamilyAttribute.#ctor(System.String,System.String)",
                           Justification = "Accessors are available for processed constructor input, which is an acceptable reason to suppress this warning as evidenced by several of Microsoft's own attributes that build non-primitive typed properties from primitive typed parameters. Performing validation of the string to ensure it is a valid GUID saves us loads of effort down the road when dealing with imported metadata. Implementing the property explicitly and creating a string property in order to satisfy this rule does not work as it breaks MEF's MetadataViewProvider.")]

[assembly: SuppressMessage("Design", 
                           "CA1019:Define accessors for attribute arguments",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensibility.RoutableAttribute.#ctor(System.String,System.Type)",
                           Justification = "Accessors are available for processed constructor input, which is an acceptable reason to suppress this warning as evidenced by several of Microsoft's own attributes that build non-primitive typed properties from primitive typed parameters. Performing validation of the string to ensure it is a valid GUID saves us loads of effort down the road when dealing with imported metadata. Implementing the property explicitly and creating a string property in order to satisfy this rule does not work as it breaks MEF's MetadataViewProvider. The segmented contract type parameter is also processed by the constructor, and accessors for it are available in the form of the base ExportAttribute class's ContractType property, which the segmented type influences.")]

[assembly: SuppressMessage("Performance", 
                           "CA1810:Initialize reference type static fields inline", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Logging.Logger.#cctor",
                           Justification = "Normally this is a good idea, however we must guarantee the listener's initialization upon any method of this class being used so it can capture the messages; static fields that are initialized inline are marked with the beforefieldinit flag, and don't actually get initialized until code is called that accesses the static field in question. Because the listener isn't actually used directly by the messaging methods of this class, it'll actually never end up being initialized so it can do its job. The static constructor is the only way; the convenience offered by this class for the crucial area of concern of diagnostic messaging is large, and merits us doing things just a tad bit differently, as long as we go about it carefully and correctly.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensions.ObjectExtensions.GetHashCode``2(``0,``1)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensions.ObjectExtensions.GetHashCode``3(``0,``1,``2)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Extensions.ObjectExtensions.GetHashCode``4(``0,``1,``2,``3)~System.Int32",
                           Justification = "This is an extension method that provides code useful to all objects. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Design", 
                           "CA1028:Enum Storage should be Int32", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Odin.Interop.VirtualKey",
                           Justification = "Native unmanaged functions expect a virtual key code in the form of an unsigned integer, and there's no way I'm going the WPF route and creating an int-based enum coupled with a static mapping class containing the largest switch statement ever seen by man.")]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Interop.WindowHandle.#ctor(System.IntPtr,System.Boolean)",
                           Justification = "The constructor makes a call to a constructor overload, and expression bodies don't look appetizing at all next to such an invocation.")]

[assembly: SuppressMessage("Globalization", 
                           "CA2101:Specify marshaling for P/Invoke string arguments", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Interop.Kernel32.GetProcAddress(System.IntPtr,System.String)~System.IntPtr",
                           Justification = "This is really a false positive -- ThrowOnUnmappableChar is set to true, closing any potential security hole in regards to Unicode characters being converted into dangerous ANSI characters. Only explicitly specifying Unicode marshalling for the string parameter causes the warning to go away, which is problematic as no Unicode variant of GetProcAddress exists. Will regard as code analysis bug for now: https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2101")]

[assembly: SuppressMessage("Design", 
                           "CA1031:Do not catch general exception types",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Threading.ThreadExecutorOperation.Execute",
                           Justification = "The exception grabbed here is essentially 'rethrown' when it is propagated to the operation's underlying Task via its TaskCompletionSource -- nothing is being suppressed here.")]

[assembly: SuppressMessage("Interoperability", 
                           "CA1419:Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Interop.DeviceContextHandle.#ctor",
                           Justification = "Providing a public parameterless constructor for a SafeHandle is something you do NOT do.")]

[assembly: SuppressMessage("Interoperability", 
                           "CA1419:Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'",
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Interop.WindowHandle.#ctor",
                           Justification = "Providing a public parameterless constructor for a SafeHandle is something you do NOT do.")]

[assembly: SuppressMessage("Maintainability", 
                           "CA1508:Avoid dead conditional code", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Odin.Interop.MessageOnlyWindowWrapper.#ctor(BadEcho.Odin.Threading.IThreadExecutor)",
                           Justification = "This is a false positive, with several issues open that seem related to it on the dotnet GitHub. Even though the CreateWindowEx function returns a non-null reference type, the Handle property will of course still be null if an exception was thrown in the try block.")]
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("c23f1588-7af9-4d9e-83af-15e922501b7f")]

#if RELEASE
[assembly: InternalsVisibleTo("BadEcho.Odin.Tests,PublicKey="+BuildInfo.PUBLIC_KEY)]
#else
[assembly: InternalsVisibleTo("BadEcho.Odin.Tests")]
#endif

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
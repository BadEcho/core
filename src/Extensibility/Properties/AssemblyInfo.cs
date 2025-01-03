using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if RELEASE
using BadEcho.Extensiblity.Properties;

[assembly: InternalsVisibleTo("BadEcho.Extensibility.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Extensibility.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("c23f1588-7af9-4d9e-83af-15e922501b7f")]

[assembly: SuppressMessage("Style", 
                           "IDE0060:Remove unused parameter", 
                           Scope = "member",
                           Target = "~M:BadEcho.Extensibility.Hosting.PluginContextStrategyExtensions.LoadConventions(BadEcho.Extensibility.Hosting.IPluginContextStrategy,System.Composition.Hosting.ContainerConfiguration)",
                           Justification = "This is an extension method for an interface that provides code useful to all said interface's implementations. Whether or not the parameter is used is moot, it is very much required to be here.")]

[assembly: SuppressMessage("Style", 
                           "IDE0016:Use 'throw' expression",                            
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensibility.Hosting.RoutableProxy.Create``1(BadEcho.Extensibility.Hosting.IHostAdapter)~``0",
                           Justification = "If method info is null, I rather an exception be thrown prior to dynamic proxy creation.")]

[assembly: SuppressMessage("Style",
                           "IDE0022:Use expression body for methods",
                           Scope = "member",
                           Target = "~M:BadEcho.Extensibility.Hosting.PluginHost.LoadAdapter``1~BadEcho.Extensibility.Hosting.HostAdapter{``0}",
                           Justification = "Rather disgusting when a generic type constraint is involved.")]

[assembly: SuppressMessage("Design", 
                           "CA1019:Define accessors for attribute arguments", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensibility.FilterableAttribute.#ctor(System.String,System.Type)",
                           Justification = "Accessors are available for processed constructor input, which is an acceptable reason to suppress this warning as evidenced by several of Microsoft's own attributes that build non-primitive typed properties from primitive typed parameters. Performing validation of the string to ensure it is a valid GUID saves us loads of effort down the road when dealing with imported metadata. Implementing the property explicitly and creating a string property in order to satisfy this rule does not work as it breaks MEF's MetadataViewProvider.")]

[assembly: SuppressMessage("Design", 
                           "CA1019:Define accessors for attribute arguments",
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensibility.FilterableFamilyAttribute.#ctor(System.String,System.String)",
                           Justification = "Accessors are available for processed constructor input, which is an acceptable reason to suppress this warning as evidenced by several of Microsoft's own attributes that build non-primitive typed properties from primitive typed parameters. Performing validation of the string to ensure it is a valid GUID saves us loads of effort down the road when dealing with imported metadata. Implementing the property explicitly and creating a string property in order to satisfy this rule does not work as it breaks MEF's MetadataViewProvider.")]

[assembly: SuppressMessage("Design", 
                           "CA1019:Define accessors for attribute arguments",
                           Scope = "member", 
                           Target = "~M:BadEcho.Extensibility.RoutableAttribute.#ctor(System.String,System.Type)",
                           Justification = "Accessors are available for processed constructor input, which is an acceptable reason to suppress this warning as evidenced by several of Microsoft's own attributes that build non-primitive typed properties from primitive typed parameters. Performing validation of the string to ensure it is a valid GUID saves us loads of effort down the road when dealing with imported metadata. Implementing the property explicitly and creating a string property in order to satisfy this rule does not work as it breaks MEF's MetadataViewProvider. The segmented contract type parameter is also processed by the constructor, and accessors for it are available in the form of the base ExportAttribute class's ContractType property, which the segmented type influences.")]
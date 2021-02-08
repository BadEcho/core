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

[assembly: SuppressMessage("Performance", 
                           "CA1813:Avoid unsealed attributes", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Odin.Extensibility.ExtensibilityAttribute",
                           Justification = "This attribute is meant to be derived to allow for greater customization of the plugin discovery process. MEF's own ExportAttribute is unsealed.")]
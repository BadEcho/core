using System.Diagnostics.CodeAnalysis;
using BadEcho.Fenestra;
using System.Windows;
using System.Windows.Markup;
using BadEcho.Odin.Extensibility;

[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Fenestra.Controls")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, 
                     ResourceDictionaryLocation.SourceAssembly)]

[assembly: ExtensibilityPoint]

[assembly: SuppressMessage("Microsoft.Maintainability",
                           "CA1501",
                           Scope = "type",
                           Target = "~T:BadEcho.Fenestra.Controls.OutlinedTextElement",
                           Justification = "Base classes are Microsoft controlled; cannot influence their hierarchy.")]

[assembly:SuppressMessage("Microsoft.Design",
                          "CA1045",
                          Scope = "member",
                          Target = "~M:BadEcho.Fenestra.ViewModels.ViewModel.NotifyIfChanged``1(``0@,``0,System.String)~System.Boolean",
                          Justification = "The method is protected, not fully public, and the immeasurable amount of convenience provided by this function vastly outweighs the inconvenience of passing an argument by reference. On top of all that, there is simply no way to provide automatic property notification without requiring the backing field to be passed by reference.")]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Fenestra.EngineException.#ctor(System.String,System.Exception,System.Boolean)",
                           Justification = "This constructor makes a call to the base constructor, and expression bodies don't look appetizing at all next to such an invocation.")]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors",
                           Scope = "member", 
                           Target = "~M:BadEcho.Fenestra.EngineException.#ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)",
                           Justification = "This constructor makes a call to the base constructor, and expression bodies don't look appetizing at all next to such an invocation.")]
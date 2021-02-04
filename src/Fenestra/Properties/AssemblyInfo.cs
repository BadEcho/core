using System.Diagnostics.CodeAnalysis;
using BadEcho.Fenestra;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Fenestra.Controls")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, 
                     ResourceDictionaryLocation.SourceAssembly)]

[assembly: SuppressMessage("Microsoft.Maintainability",
                           "CA1501",
                           Scope = "type",
                           Target = "~T:BadEcho.Fenestra.Controls.OutlinedTextElement",
                           Justification = "Base classes are Microsoft controlled, cannot influence their hierarchy.")]

[assembly:SuppressMessage("Microsoft.Design",
                          "CA1045",
                          Scope = "member",
                          Target = "~M:BadEcho.Fenestra.ViewModels.ViewModel.NotifyIfChanged``1(``0@,``0,System.String)~System.Boolean",
                          Justification = "The method is protected, not fully public, and the immeasurable amount of convenience provided by this function vastly outweighs the inconvenience of passing an argument by reference. On top of all that, there is simply no way to provide automatic property notification without requiring the backing field to be passed by reference.")]
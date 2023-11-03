using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BadEcho.Presentation;
using System.Windows;
using System.Windows.Markup;
using BadEcho.Extensibility;
#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Presentation.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Presentation.Tests")]
#endif

[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Presentation.Markup")]
[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Presentation.Behaviors")]
[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Presentation.Converters")]
[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Presentation.Windows")]
[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Presentation.Selectors")]
[assembly: XmlnsDefinition(Constants.Namespace, "BadEcho.Presentation")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None,
                     ResourceDictionaryLocation.SourceAssembly)]

[assembly: ExtensibilityPoint]

[assembly: SuppressMessage("Microsoft.Maintainability",
                           "CA1501",
                           Scope = "type",
                           Target = "~T:BadEcho.Presentation.OutlinedTextElement",
                           Justification = "Base classes are Microsoft controlled; cannot influence their hierarchy.")]

[assembly: SuppressMessage("Microsoft.Design",
                          "CA1045",
                          Scope = "member",
                          Target = "~M:BadEcho.Presentation.ViewModels.ViewModel.NotifyIfChanged``1(``0@,``0,System.String)~System.Boolean",
                          Justification = "The method is not public, and the immeasurable amount of convenience provided by this function vastly outweighs the inconvenience of passing an argument by reference. On top of all that, there is simply no way to provide automatic property notification without requiring the backing field to be passed by reference.")]

[assembly: SuppressMessage("Style",
                           "IDE0021:Use expression body for constructors",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.EngineException.#ctor(System.String,System.Exception,System.Boolean)",
                           Justification = "This constructor makes a call to the base constructor, and expression bodies don't look appetizing at all next to such an invocation.")]

[assembly: SuppressMessage("Design",
                           "CA1033:Interface methods should be callable by child types",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.Markup.MultiBindingExtension.System#Windows#Markup#IAddChild#AddChild(System.Object)",
                           Justification = "This is a proper use of explicit interface implementation, and is exactly how Microsoft implements this very same interface in its own binding class analogues.")]

[assembly: SuppressMessage("Design",
                           "CA1033:Interface methods should be callable by child types",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.Markup.MultiBindingExtension.System#Windows#Markup#IAddChild#AddText(System.String)",
                           Justification = "This is a proper use of explicit interface implementation, and is exactly how Microsoft implements this very same interface in its own binding class analogues.")]

[assembly: SuppressMessage("Maintainability",
                           "CA1508:Avoid dead conditional code",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.Behaviors.Behavior`2.Detach(`0)",
                           Justification = "This is a strange false positive. The source of the value is clearly from a dictionary with NULLABLE values. So, a null check is required.")]

[assembly: SuppressMessage("Maintainability",
                           "CA1508:Avoid dead conditional code",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.ViewModels.ViewModel`1.Disconnect",
                           Justification = "This seems to be a false positive and bug in code analysis. We're doing a type conversion using the 'as' keyword of an object of generic type T to IViewModel. There is no guarantee at all, given the lack of generic constraints, that T is an IViewModel and won't be null.")]

[assembly: SuppressMessage("Maintainability",
                           "CA1508:Avoid dead conditional code",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.ViewModels.CollectionViewModelEngine`2.HandleItemsChanged(System.Object,BadEcho.Collections.CollectionPropertyChangedEventArgs)",
                           Justification = "This is yet another a strange false positive. The source of the value is clearly from a property that has been marked as being a reference that can be null. So, a null check is required.")]

[assembly: SuppressMessage("Style",
                           "IDE0022:Use expression body for methods",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.ViewModels.CollectionViewModel`2.FindItem``1(`0)~``0",
                           Justification = "Rather disgusting when a generic type constraint is involved.")]

[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.Markup.BindingExtensionSkeleton`2.DoBindingAction(System.Func{System.Boolean})~System.Boolean",
                           Justification = "It is important for us to adopt the same approach that Microsoft uses in their own data binding logic when dealing with user input; specifically: all exceptions must be caught. There is no application code on the stack when this code is ran, so any exceptions thrown are not actionable by the application.")]

[assembly: SuppressMessage("Design",
                           "CA1031:Do not catch general exception types",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.Markup.BindingExtensionSkeleton`2.DoBindingAction(System.Action)",
                           Justification = "It is important for us to adopt the same approach that Microsoft uses in their own data binding logic when dealing with user input; specifically: all exceptions must be caught. There is no application code on the stack when this code is ran, so any exceptions thrown are not actionable by the application.")]

[assembly: SuppressMessage("Style",
                           "IDE0270:Use coalesce expression",
                           Scope = "member",
                           Target = "~M:BadEcho.Presentation.PackUri.MakePackUri(System.Reflection.Assembly,System.String,System.Boolean)~System.String",
                           Justification = "Simplifying this null check makes the code harder to read (nested ternary operators).")]
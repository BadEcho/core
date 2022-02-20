using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("e1d2e0ee-4b75-4187-bf00-d201417c71c8")]

[assembly: SuppressMessage("Performance",
                           "CA1812:Avoid uninstantiated internal classes",
                           Scope = "type",
                           Target = "~T:BadEcho.Odin.XmlConfiguration.Extensibility.ContractElement",
                           Justification = "False positive which occurs due to ContractElement only being instantiated through its use as a generic type parameter with the NamedElementCollection<TElement> type, which instantiates it via the CreateNewElement override. To be clear, ContractElement IS instantiated by the assembly.")]

[assembly: SuppressMessage("Performance",
                           "CA1812:Avoid uninstantiated internal classes",
                           Scope = "type",
                           Target = "~T:BadEcho.Odin.XmlConfiguration.Extensibility.MethodClaimElement",
                           Justification = "False positive which occurs due to MethodClaimElement only being instantiated through its use as a generic type parameter with the NamedElementCollection<TElement> type, which instantiates it via the CreateNewElement override. To be clear, MethodClaimElement IS instantiated by the assembly.")]

[assembly: SuppressMessage("Performance",
                           "CA1812:Avoid uninstantiated internal classes",
                           Scope = "type",
                           Target = "~T:BadEcho.Odin.XmlConfiguration.Extensibility.RoutablePluginElement",
                           Justification = "False positive which occurs due to RoutablePluginElement only being instantiated through its use as a generic type parameter with the GuidElementCollection<TElement> type, which instantiates it via the CreateNewElement override. To be clear, RoutablePluginElement IS instantiated by the assembly.")]

[assembly: SuppressMessage("Performance", 
                           "CA1812:Avoid uninstantiated internal classes",
                           Scope = "type", 
                           Target = "~T:BadEcho.Odin.XmlConfiguration.GuidElementCollection`1",
                           Justification = "False positive which occurs due to GuidElementCollection<TElement> only being instantiated via Reflection through the use of ConfigurationManager's programmatic coding model.")]
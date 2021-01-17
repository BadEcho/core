using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("c23f1588-7af9-4d9e-83af-15e922501b7f")]

[assembly: SuppressMessage("Microsoft.Design",
                           "CA1045",
                           Scope = "member",
                           Target = "BadEcho.Odin.Serialization.JsonPolymorphicConverter`2.ReadFromDescriptor",
                           Justification = "System.Text.Json.Serialization.JsonConverter design is centered around the use of the System.Text.Json.Utf8JsonReader being passed around by reference.")]

[assembly: SuppressMessage("Microsoft.Performance",
                           "CA1812",
                           Scope = "type",
                           Target = "BadEcho.Odin.Extensibility.CompositionElement+CompositionElementDebuggerProxy",
                           Justification = "Not entirely sure why this is being flagged. Besides being actually set up as a display proxy for CompositionElement, the Roslyn analyzers, according to the GitHub at least, are supposed to have support for types specified as parameters decorated with DynamicallyAccessedMembersAttribute, as is the case with DebuggerTypeProxyAttribute's Type parameter. This is either a bug, or the analyzers don't actually support DynamicallyAccessedMembersAttribute usage for this rule.")]
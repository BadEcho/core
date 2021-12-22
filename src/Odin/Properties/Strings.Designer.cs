﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BadEcho.Odin.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BadEcho.Odin.Properties.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot retrieve a value from an export that isn&apos;t representing an object..
        /// </summary>
        internal static string ArgumentExportValueNoObject {
            get {
                return ResourceManager.GetString("ArgumentExportValueNoObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided string cannot be empty..
        /// </summary>
        internal static string ArgumentStringEmpty {
            get {
                return ResourceManager.GetString("ArgumentStringEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid INotifyPropertyChanged implementation; event data is missing property name information..
        /// </summary>
        internal static string BadINotifyPropertyChangedImplementation {
            get {
                return ResourceManager.GetString("BadINotifyPropertyChangedImplementation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EnumDisplayMonitors failed unexpectedly..
        /// </summary>
        internal static string DisplayEnumDisplayMonitorsFailed {
            get {
                return ResourceManager.GetString("DisplayEnumDisplayMonitorsFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HRESULT of {0} returned when attempting to query for a monitor&apos;s DPI. Because of this, display device will default to using the system-wide DPI..
        /// </summary>
        internal static string DisplayGetDpiForMonitorFailed {
            get {
                return ResourceManager.GetString("DisplayGetDpiForMonitorFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumeration has already finished; no more objects exist in the sequence..
        /// </summary>
        internal static string EnumerationAtEnd {
            get {
                return ResourceManager.GetString("EnumerationAtEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumeration has not started. A call to MoveNext() must occur at least once prior to reading the Current object..
        /// </summary>
        internal static string EnumerationNotStarted {
            get {
                return ResourceManager.GetString("EnumerationNotStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumeration type&apos;s underlying integral numeric type is not an integer..
        /// </summary>
        internal static string EnumIntegralTypeNotInteger {
            get {
                return ResourceManager.GetString("EnumIntegralTypeNotInteger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executor operation must either be complete or canceled prior to finalizing completion..
        /// </summary>
        internal static string ExecutorFinalizedBeforeDone {
            get {
                return ResourceManager.GetString("ExecutorFinalizedBeforeDone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executor may not wait on an operation executing on the same thread..
        /// </summary>
        internal static string ExecutorWaitOperationSameThread {
            get {
                return ResourceManager.GetString("ExecutorWaitOperationSameThread", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A plugin directory was explicitly specified in this application&apos;s configuration, however &apos;{0}&apos; does not exist. Please ensure that a desired plugin directory exists when configuring its use, or simply forego specifying an explicit plugin directory name if its existence cannot be guaranteed..
        /// </summary>
        internal static string ExtensibilityConfigurationDirectoryNotFound {
            get {
                return ResourceManager.GetString("ExtensibilityConfigurationDirectoryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a registered filterable family identity value. Registration of a filterable family requires a FilterableFamily export action..
        /// </summary>
        internal static string FamilyIdNotRegistered {
            get {
                return ResourceManager.GetString("FamilyIdNotRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid filterable family identity value. Values must be parseable as standard globally unique identifiers..
        /// </summary>
        internal static string FamilyIdNotValid {
            get {
                return ResourceManager.GetString("FamilyIdNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No routable plugin has been registered to handle calls made to &apos;{0}&apos;..
        /// </summary>
        internal static string HostAdapterUnregisteredMethod {
            get {
                return ResourceManager.GetString("HostAdapterUnregisteredMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Injection of exported parts into an attributed pluggable part of type &apos;{0}&apos; within a self-armed context requires said part to be assignable to the specified dependency value of type &apos;{1}&apos;..
        /// </summary>
        internal static string IncompatibleDependencyTypeForInjection {
            get {
                return ResourceManager.GetString("IncompatibleDependencyTypeForInjection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided JSON object&apos;s object data property is not a JSON object..
        /// </summary>
        internal static string JsonDataValueNotObject {
            get {
                return ResourceManager.GetString("JsonDataValueNotObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; property found on the provided JSON object is not the expected type descriptor property &apos;{1}&apos;..
        /// </summary>
        internal static string JsonInvalidTypeName {
            get {
                return ResourceManager.GetString("JsonInvalidTypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided JSON text is malformed. A collection of one or more key-value pairs is expected for an object..
        /// </summary>
        internal static string JsonMalformedText {
            get {
                return ResourceManager.GetString("JsonMalformedText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided JSON text does not begin with the start of a JSON object..
        /// </summary>
        internal static string JsonNotStartObject {
            get {
                return ResourceManager.GetString("JsonNotStartObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided JSON object&apos;s type descriptor property is not a number..
        /// </summary>
        internal static string JsonTypeValueNotNumber {
            get {
                return ResourceManager.GetString("JsonTypeValueNotNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing exception payload..
        /// </summary>
        internal static string LoggingMissingException {
            get {
                return ResourceManager.GetString("LoggingMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing message payload..
        /// </summary>
        internal static string LoggingMissingMessage {
            get {
                return ResourceManager.GetString("LoggingMissingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple exports for contract type &apos;{0}&apos; belong to the &apos;{1}&apos; filterable family; any part beyond the first of its type from a filterable context is ignored..
        /// </summary>
        internal static string MultipleExportsFoundForFamily {
            get {
                return ResourceManager.GetString("MultipleExportsFoundForFamily", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple exports for required contract type &apos;{0}&apos; were found. Only a single provider should ever be available for contract types loaded as unique requirements..
        /// </summary>
        internal static string MultipleExportsFoundForRequiredContract {
            get {
                return ResourceManager.GetString("MultipleExportsFoundForRequiredContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extensibility configuration lacks registration for contract &apos;{0}&apos;..
        /// </summary>
        internal static string NoContractInConfiguration {
            get {
                return ResourceManager.GetString("NoContractInConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No comparable exception class can be mapped to the provided HRESULT as it does not represent a failed operation..
        /// </summary>
        internal static string NoExceptionFromSuccessfulResult {
            get {
                return ResourceManager.GetString("NoExceptionFromSuccessfulResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No export for contract type &apos;{0}&apos; belongs to the &apos;{1}&apos; filterable family..
        /// </summary>
        internal static string NoExportFoundForFamily {
            get {
                return ResourceManager.GetString("NoExportFoundForFamily", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No export for required contract type &apos;{0}&apos; was found. Components must be designed so that it isn&apos;t possible for required contracts to not be present, such as by exporting the required contract within the component&apos;s own assembly..
        /// </summary>
        internal static string NoExportFoundForRequiredContract {
            get {
                return ResourceManager.GetString("NoExportFoundForRequiredContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; being scanned for pluggable exports failed to load due to most likely not being a valid .NET assembly..
        /// </summary>
        internal static string PluginBadImageException {
            get {
                return ResourceManager.GetString("PluginBadImageException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; being scanned for pluggable exports failed to load..
        /// </summary>
        internal static string PluginFileLoadException {
            get {
                return ResourceManager.GetString("PluginFileLoadException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid call-routable plugin identity value. Values must be parseable as standard globally unique identifiers..
        /// </summary>
        internal static string RoutablePluginIdNotValid {
            get {
                return ResourceManager.GetString("RoutablePluginIdNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RoutableProxy was not provided a IHostAdapter due to improper initialization. Use a static factory method such as RoutableProxy.Create instead to initialize the proxy properly..
        /// </summary>
        internal static string RoutableProxyNotInitializedCorrectly {
            get {
                return ResourceManager.GetString("RoutableProxyNotInitializedCorrectly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RouteProxy instance was somehow invoked with null method information..
        /// </summary>
        internal static string RoutableProxyNullMethodInfo {
            get {
                return ResourceManager.GetString("RoutableProxyNullMethodInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Subclass message processing procedure invoked while in a detached state..
        /// </summary>
        internal static string SubclassDetachedWndProc {
            get {
                return ResourceManager.GetString("SubclassDetachedWndProc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to detach a subclassed window&apos;s WndProc from the window&apos;s message chain..
        /// </summary>
        internal static string SubclassDetachmentFailed {
            get {
                return ResourceManager.GetString("SubclassDetachmentFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to forcibly detach a subclassed window&apos;s WndProc from the window&apos;s message chain..
        /// </summary>
        internal static string SubclassForcibleDetachmentFailed {
            get {
                return ResourceManager.GetString("SubclassForcibleDetachmentFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot load an operation into an already initialized ThreadExecutorOperationTaskSource..
        /// </summary>
        internal static string ThreadExecutorSourceAlreadyInitialized {
            get {
                return ResourceManager.GetString("ThreadExecutorSourceAlreadyInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ThreadExecutorOperationTaskSource has not been initialized..
        /// </summary>
        internal static string ThreadExecutorSourceNotInitialized {
            get {
                return ResourceManager.GetString("ThreadExecutorSourceNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Weak list enumerator requires a strong reference to enumerate..
        /// </summary>
        internal static string WeakListEnumeratorNoReference {
            get {
                return ResourceManager.GetString("WeakListEnumeratorNoReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A hot key has already been registered with the identifier &apos;{0}&apos;..
        /// </summary>
        internal static string WindowHotKeyDuplicateId {
            get {
                return ResourceManager.GetString("WindowHotKeyDuplicateId", resourceCulture);
            }
        }
    }
}

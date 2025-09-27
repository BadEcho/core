using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Extensions.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Extensions.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("0b32bf92-effd-4c56-81fd-c34fe2c519b3")]

[assembly: SuppressMessage("Performance",
                           "CA1848:Use the LoggerMessage delegates",
                           Scope = "member",
                           Target = "~M:BadEcho.Extensions.EventSourceLogForwarder.LogEvent(System.Diagnostics.Tracing.EventWrittenEventArgs)",
                           Justification = "It's not possible to make use of LoggerMessage delegates due to the forwarded messages having a number of varying formats. Microsoft also does not use LoggerMessage delegates in their log forwarders.")]

[assembly: SuppressMessage("Performance",
                           "CA2254: Template should be a static expression",
                           Scope = "member",
                           Target = "~M:BadEcho.Extensions.EventSourceLogForwarder.LogEvent(System.Diagnostics.Tracing.EventWrittenEventArgs)",
                           Justification = "It's not possible to make use of static message templates due to the forwarded messages having a number of varying formats. Microsoft also does not use static message templates in their log forwarders.")]

[assembly: SuppressMessage("Design",
                           "CA1062: Validate arguments of public methods",
                           Scope = "member",
                           Target = "M:BadEcho.Extensions.WritableOptions`1.#ctor(Microsoft.Extensions.Options.IOptionsFactory{`0},System.Collections.Generic.IEnumerable{Microsoft.Extensions.Options.IOptionsChangeTokenSource{`0}},System.Collections.Generic.IEnumerable{BadEcho.Extensions.ConfigureWritableOptions{`0}},Microsoft.Extensions.Options.IOptionsMonitorCache{`0},Microsoft.Extensions.Hosting.IHostEnvironment)",
                           Justification = "This type is meant to be initialized by a dependency injection container. This code analysis rule seems to ignore interfaces of commonly injected types, but it gets flagged for custom service types. This is unnecessary, due to the fact that the runtime will throw an exception if any of the injected parameters cannot be obtained.")]
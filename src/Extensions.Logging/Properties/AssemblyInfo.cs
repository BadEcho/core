using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Extensions.Logging.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Extensions.Logging.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("0b32bf92-effd-4c56-81fd-c34fe2c519b3")]

[assembly: SuppressMessage("Performance",
                           "CA1848:Use the LoggerMessage delegates",
                           Scope = "member",
                           Target = "~M:BadEcho.Extensions.Logging.EventSourceLogForwarder.LogEvent(System.Diagnostics.Tracing.EventWrittenEventArgs)",
                           Justification = "It's not possible to make use of LoggerMessage delegates due to the forwarded messages having a number of varying formats. Microsoft also does not use LoggerMessage delegates in their log forwarders.")]

[assembly: SuppressMessage("Performance",
                           "CA2254: Template should be a static expression",
                           Scope = "member",
                           Target = "~M:BadEcho.Extensions.Logging.EventSourceLogForwarder.LogEvent(System.Diagnostics.Tracing.EventWrittenEventArgs)",
                           Justification = "It's not possible to make use of static message templates due to the forwarded messages having a number of varying formats. Microsoft also does not use static message templates in their log forwarders.")]

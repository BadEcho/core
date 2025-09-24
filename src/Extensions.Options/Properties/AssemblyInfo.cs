using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Extensions.Options.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Extensions.Options.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("acff6c85-b239-42fc-8677-1f6b13b1e5a7")]

[assembly: SuppressMessage("Design",
                           "CA1062: Validate arguments of public methods",
                           Scope = "member",
                           Target = "M:BadEcho.Extensions.Options.WritableOptions`1.#ctor(Microsoft.Extensions.Options.IOptionsFactory{`0},System.Collections.Generic.IEnumerable{Microsoft.Extensions.Options.IOptionsChangeTokenSource{`0}},System.Collections.Generic.IEnumerable{BadEcho.Extensions.Options.ConfigureWritableOptions{`0}},Microsoft.Extensions.Options.IOptionsMonitorCache{`0},Microsoft.Extensions.Hosting.IHostEnvironment)",
                           Justification = "This type is meant to be initialized by a dependency injection container. This code analysis rule seems to ignore interfaces of commonly injected types, but it gets flagged for custom service types. This is unnecessary, due to the fact that the runtime will throw an exception if any of the injected parameters cannot be obtained.")]
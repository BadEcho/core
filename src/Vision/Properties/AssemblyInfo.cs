using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
#if RELEASE
using BadEcho.Odin.Properties;

[assembly: InternalsVisibleTo("BadEcho.Omnified.Vision.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Omnified.Vision.Tests")]
#endif

[assembly: ThemeInfo(ResourceDictionaryLocation.None,
                     ResourceDictionaryLocation.SourceAssembly)]

[assembly: SuppressMessage("Style", 
                           "IDE0021:Use expression body for constructors",
                           Scope = "member", 
                           Target = "~M:BadEcho.Omnified.Vision.Windows.VisionWindow.#ctor",
                           Justification = "This constructor makes a call to the base constructor, and expression bodies don't look appetizing at all next to such an invocation.")]
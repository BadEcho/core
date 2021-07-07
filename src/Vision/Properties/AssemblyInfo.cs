using System.Runtime.CompilerServices;
using System.Windows;

#if RELEASE
[assembly: InternalsVisibleTo("BadEcho.Omnified.Vision.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Omnified.Vision.Tests")]
#endif

[assembly: ThemeInfo(ResourceDictionaryLocation.None,
                     ResourceDictionaryLocation.SourceAssembly)]

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Hooks.Tests,PublicKeys="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Hooks.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("acef76e7-ce85-4917-9182-89d9b04beb3a")]

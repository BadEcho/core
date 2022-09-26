using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if RELEASE
using BadEcho.Properties;

[assembly: InternalsVisibleTo("BadEcho.Game.Tests,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Game.Tests")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("76aa12cf-f354-4e26-a74f-6a20380548fc")]

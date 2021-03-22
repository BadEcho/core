using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if RELEASE
[assembly: InternalsVisibleTo("BadEcho.Omnified.Vision.Sandbox,PublicKey="+BuildInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BadEcho.Omnified.Vision.Sandbox")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("399aa837-a511-46ba-b2a1-2d17ed059ab3")]
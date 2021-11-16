using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("c72244d1-f152-4aaf-a2c7-13b1ec5d3f5f")]

[assembly: SuppressMessage("Performance", 
                           "CA1812:Avoid uninstantiated internal classes", 
                           Scope = "type", 
                           Target = "~T:BadEcho.Omnified.Vision.Apocalypse.ViewModels.ApocalypseViewModel",
                           Justification = "This class is absolutely instantiated by ApocalypseModule, as a consequence of that class being derived from VisionModule, which instantiates the provided TViewModel type via the new() constraint. Not sure how the Roslyn analyzer is missing this.")]
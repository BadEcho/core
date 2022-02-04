//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;
using System.Composition.Convention;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin;

[Export(typeof(IFakePart))]
public sealed class FakePart : IFakePart
{
    public int DoSomething()
    {
        return 69;
    }
}

[Export(typeof(IFakePart))]
public sealed class AnotherFakePart : IFakePart
{
    public int DoSomething()
    {
        return 42;
    }
}

[Export(typeof(IFakeDependency))]
public class FakeDependency : IFakeDependency
{ }

[Export(typeof(INonSharedFakePart))]
public sealed class NonSharedFakePart : INonSharedFakePart
{
    public int DoSomething()
    {
        return 99;
    }
}

[Export(typeof(ISharedFakePart))]
public sealed class SharedFakePart : ISharedFakePart
{
    public int DoSomething()
    {
        return -1;
    }

    [Export(typeof(IConventionProvider))]
    private sealed class SharedConventionProvider : IConventionProvider
    {
        public void ConfigureRules(ConventionBuilder conventions)
        {
            conventions.ForTypesDerivedFrom<ISharedFakePart>()
                       .Shared();
        }
    }
}
//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Tests.Extensibility
{
    public interface IFakePart
    {
        int DoSomething();
    }
    public interface IFilterableFakePart
    {
        int DoSomething();
    }
    public interface INonSharedFakePart
    {
        int DoSomething();
    }
    public interface ISharedFakePart
    {
        int DoSomething();
    }
}

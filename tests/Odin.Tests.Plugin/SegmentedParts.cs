//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin;

[Routable(FakeAdapterIds.AlphaFakeIdValue, typeof(ISegmentedContract))]
public class AlphaFakeAdapter : IPluginAdapter<ISegmentedContract>
{
    private readonly SegmentedStub _stub = new();

    public ISegmentedContract Contract
        => _stub;

    private class SegmentedStub : ISegmentedContract
    {
        public string SomeMethod()
        {
            return ISegmentedContract.FirstSomeMethod;
        }

        public string SomeOtherMethod()
        {
            return ISegmentedContract.FirstSomeOtherMethod;
        }
    }
}

[Routable(FakeAdapterIds.BetaFakeIdValue, typeof(ISegmentedContract))]
public class BetaFakeAdapter : IPluginAdapter<ISegmentedContract>
{
    private readonly SegmentedStub _stub = new();

    public ISegmentedContract Contract
        => _stub;

    private class SegmentedStub : ISegmentedContract
    {
        public string SomeMethod()
        {
            return ISegmentedContract.SecondSomeMethod;
        }

        public string SomeOtherMethod()
        {
            return ISegmentedContract.SecondSomeOtherMethod;
        }
    }
}
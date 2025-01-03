//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensibility;
using BadEcho.Extensibility.Tests;

namespace BadEcho.Plugin.Tests;

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
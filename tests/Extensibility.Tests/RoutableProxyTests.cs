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

using BadEcho.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Tests.Extensibility;

public class RoutableProxyTests
{
    private readonly ISegmentedContract _proxy = RoutableProxy.Create<ISegmentedContract>(new HostAdapterStub());

    [Fact]
    public void SomeMethod_FirstContract()
    {
        var result = _proxy.SomeMethod();

        Assert.Equal(ISegmentedContract.FirstSomeMethod, result);
    }

    [Fact]
    public void SomeOtherMethod_SecondContract()
    {
        var result = _proxy.SomeOtherMethod();

        Assert.Equal(ISegmentedContract.SecondSomeOtherMethod, result);
    }

    private sealed class HostAdapterStub : IHostAdapter
    {
        private readonly FirstContractStub _first = new();
        private readonly SecondContractStub _second = new();

        public object Route(string methodName)
        {
            return methodName switch
            {
                nameof(ISegmentedContract.SomeMethod) 
                    => _first,
                nameof(ISegmentedContract.SomeOtherMethod) 
                    => _second,
                _ 
                    => throw new InvalidOperationException()
            };
        }
    }

    private sealed class FirstContractStub : ISegmentedContract
    {
        public string SomeMethod()
            => ISegmentedContract.FirstSomeMethod;

        public string SomeOtherMethod()
            => ISegmentedContract.FirstSomeOtherMethod;
    }

    private sealed class SecondContractStub : ISegmentedContract
    {
        public string SomeMethod()
            => ISegmentedContract.SecondSomeMethod;

        public string SomeOtherMethod()
            => ISegmentedContract.SecondSomeOtherMethod;
    }
}
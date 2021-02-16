//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using BadEcho.Odin.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    public class RoutableProxyTests
    {
        private readonly ISegmentedContract _proxy;
        private const string FIRST_SOME_METHOD = "First";
        private const string FIRST_SOME_OTHER_METHOD = "StillFirst";
        private const string SECOND_SOME_METHOD = "Second";
        private const string SECOND_SOME_OTHER_METHOD = "StillSecond";

        public RoutableProxyTests() 
            => _proxy = RoutableProxy.Create<ISegmentedContract>(new HostAdapterStub());

        [Fact]
        public void FirstContract_SomeMethod()
        {
            var result = _proxy.SomeMethod();

            Assert.Equal(FIRST_SOME_METHOD, result);
        }

        [Fact]
        public void SecondContract_SomeOtherMethod()
        {
            var result = _proxy.SomeOtherMethod();

            Assert.Equal(SECOND_SOME_OTHER_METHOD, result);
        }

        private sealed class HostAdapterStub : IHostAdapter
        {
            private readonly FirstContractStub _first = new FirstContractStub();
            private readonly SecondContractStub _second = new SecondContractStub();

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

        private interface ISegmentedContract
        {
            string SomeMethod();

            string SomeOtherMethod();
        }

        private sealed class FirstContractStub : ISegmentedContract
        {
            public string SomeMethod() 
                => FIRST_SOME_METHOD;

            public string SomeOtherMethod() 
                => FIRST_SOME_OTHER_METHOD;
        }

        private sealed class SecondContractStub : ISegmentedContract
        {
            public string SomeMethod() 
                => SECOND_SOME_METHOD;

            public string SomeOtherMethod() 
                => SECOND_SOME_OTHER_METHOD;
        }
    }
}

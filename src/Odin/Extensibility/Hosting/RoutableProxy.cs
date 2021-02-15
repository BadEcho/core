//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using BadEcho.Odin.Logging;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a proxy object that handles method dispatch by routing segmented contract method calls to the call-routable
    /// plugins pointed to by a provided <see cref="IHostAdapter"/> instance.
    /// </summary>
    /// <suppresions>
    /// ReSharper disable EmptyConstructor
    /// </suppresions>
    public class RoutableProxy : DispatchProxy
    {
        private IHostAdapter? _adapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutableProxy"/> class.
        /// </summary>
        /// <remarks>
        /// Consumers should not initialize this object by calling this constructor directly. It exists only to fulfill
        /// requirements of <see cref="DispatchProxy"/>.
        /// </remarks>
        public RoutableProxy()
        { }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> that will route method calls through the provided host adapter.
        /// </summary>
        /// <param name="adapter">The host adapter to route calls through.</param>
        /// <returns>
        /// An object instance implementing <typeparamref name="T"/> that will route calls through <c>adapter</c>.
        /// </returns>
        /// <typeparam name="T">The segmented contract type whose calls will be routed through the proxy.</typeparam>
        public static T Create<T>(IHostAdapter adapter)
            where T : class
        {
            Require.NotNull(adapter, nameof(adapter));

            object proxy = Create<T, RoutableProxy>();
            var routableProxy = (RoutableProxy)proxy;

            routableProxy._adapter = adapter;

            return (T) proxy;
        }

        /// <inheritdoc/>
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (_adapter == null)
                throw new InvalidOperationException(Strings.RoutableProxyNotInitializedCorrectly);
            
            // It is unclear to me as to how targetMethod ever ends up being null.
            // Exception won't be thrown since, by virtue of the method signature, we must support null values for this argument.
            if (targetMethod == null)
            {
                Logger.Warning(Strings.RoutableProxyNullMethodInfo);
                return null;
            }

            object contractImpl = _adapter.Route(targetMethod.Name);

            return targetMethod.Invoke(contractImpl, args);
        }
    }
}

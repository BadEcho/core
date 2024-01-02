//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a scope in which an activation context conforming to an embedded side-by-side assembly manifest is active.
/// </summary>
public sealed class ActivationScope : IDisposable
{
    private const int MANIFEST_RESOURCE_ID_EXECUTABLE = 1;
    private const int MANIFEST_RESOURCE_ID_DLL = 2;

    private readonly ActivationContextHandle _contextHandle;
    private ActivationContextCookieHandle? _cookieHandle;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivationScope"/> class.
    /// </summary>
    /// <param name="contextHandle">A handle to the activation context to activate.</param>
    private ActivationScope(ActivationContextHandle contextHandle)
    {
        _contextHandle = contextHandle;
    }

    /// <summary>
    /// Creates a scope in which an activation context conforming to the Bad Echo Core Framework's embedded side-by-side assembly manifest
    /// is active.
    /// </summary>
    /// <returns>An <see cref="ActivationScope"/> instance representing the currently active activation context.</returns>
    public static ActivationScope Create()
    {
        Module module = typeof(ActivationScope).Module;
        IntPtr hModule = Kernel32.GetModuleHandle(module.Name);

        if (hModule != IntPtr.Zero)
            return Create(hModule, MANIFEST_RESOURCE_ID_DLL);

        // If we couldn't load a handle to this module, then this has been compiled as a single-file package.
        // We'll extract the manifest resource directly instead.
        using (Stream? stream = module.Assembly.GetManifestResourceStream("BadEcho.Properties.app.manifest"))
        {
            if (stream == null)
                throw new InvalidOperationException(Strings.ActivationContextManifestResourceNotFound);

            return Create(stream);
        }
    }

    /// <summary>
    /// Creates a scope in which an activation context conforming to the side-by-side assembly manifest embedded in the specified module
    /// is active.
    /// </summary>
    /// <param name="module">The module which houses the embedded side-by-side assembly manifest.</param>
    /// <param name="isExecutable">Value indicating whether <paramref name="module"/> belongs to an executable or DLL.</param>
    /// <returns>An <see cref="ActivationScope"/> instance representing the currently active activation context.</returns>
    public static ActivationScope Create(Module module, bool isExecutable)
    {
        Require.NotNull(module, nameof(module));

        IntPtr hModule = Kernel32.GetModuleHandle(module.Name);

        if (hModule == IntPtr.Zero)
            throw new InvalidOperationException(Strings.ActivationContextModuleNotFound);

        return Create(hModule, isExecutable ? MANIFEST_RESOURCE_ID_EXECUTABLE : MANIFEST_RESOURCE_ID_DLL);
    }

    /// <summary>
    /// Creates a scope in which an activation context conforming to the side-by-side assembly manifest found in the provided stream
    /// is active.
    /// </summary>
    /// <param name="manifestStream">
    /// A manifest resource stream, the contents of which is that of a side-by-side assembly manifest.
    /// </param>
    /// <returns>An <see cref="ActivationScope"/> instance representing the currently active activation context.</returns>
    /// <remarks>Use this overload if no module containing a native embedded manifest resource is available.</remarks>
    public static ActivationScope Create(Stream manifestStream)
    {
        Require.NotNull(manifestStream, nameof(manifestStream));

        // Activation contexts are sourced from either a native embedded manifest resource, or an external XML file.
        // Creating an activation scope using a manifest resource stream means the former option is not available,
        // therefore we need to write the contents of the stream out into an external XML file so that it can then be
        // processed by the Activation Context API.
        string tempManifestPath = Path.Join(Path.GetTempPath(), Path.GetRandomFileName());

        using (var tempManifestStream = new FileStream(tempManifestPath,
                                                       FileMode.CreateNew,
                                                       FileAccess.ReadWrite,
                                                       FileShare.Delete | FileShare.ReadWrite))
        {
            manifestStream.CopyTo(tempManifestStream);
        }

        var context = new ActivationContext(ActivationContextFlags.SourceValid)
                      {
                          Source = tempManifestPath
                      };

        ActivationContextHandle contextHandle = Kernel32.CreateActCtx(ref context);

        return Create(contextHandle);
    }

    /// <summary>
    /// Activates the activation context created within this scope.
    /// </summary>
    public void Activate()
    {
        if (!Kernel32.ActivateActCtx(_contextHandle, out _cookieHandle))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _cookieHandle?.Dispose();
        _contextHandle.Dispose();

        _disposed = true;
    }

    private static ActivationScope Create(IntPtr hModule, int resourceId)
    {
        var context = new ActivationContext(ActivationContextFlags.ModuleValid | ActivationContextFlags.ResourceNameValid)
                      {
                          ResourceName = new IntPtr(resourceId),
                          Module = hModule
                      };

        ActivationContextHandle contextHandle = Kernel32.CreateActCtx(ref context);

        return Create(contextHandle);
    }

    private static ActivationScope Create(ActivationContextHandle contextHandle)
    {
        if (contextHandle.IsInvalid)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var scope = new ActivationScope(contextHandle);

        scope.Activate();

        return scope;
    }
}

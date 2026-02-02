// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with COM.
/// </summary>
internal static unsafe partial class Ole32
{
    private const string LIBRARY_NAME = "ole32";

    /// <summary>
    /// Activates a COM class object.
    /// </summary>
    /// <typeparam name="T">The COM interface the class implements.</typeparam>
    /// <param name="clsid">The CLSID associated with the data and code that will be used to create the object.</param>
    /// <param name="iid">The identifier to be used to communicate with the object.</param>
    /// <returns>The initialized COM class object.</returns>
    public static T Activate<T>(Guid clsid, Guid iid)
    {
        ResultHandle result = CoCreateInstance(ref clsid, nint.Zero, ClassContext.All, ref iid, out void* pInstance);

        if (result.Failed())
            throw result.GetException();

        T instance = ComInterfaceMarshaller<T>.ConvertToManaged(pInstance)
            ?? throw new InvalidOperationException(Strings.CannotConvertComToManaged);

        return instance;
    }

    /// <summary>
    /// Creates and default-initializes a single object of the class associated with a specified CLSID.
    /// </summary>
    /// <param name="rClsid">A reference to the CLSID associated with the data and code that will be used to create the object.</param>
    /// <param name="pUnkOther">Pointer to an aggregate object's <c>IUnknown</c> interface if not null.</param>
    /// <param name="clsContext">Context in which the code that manages the newly created object will run.</param>
    /// <param name="rIid">A reference to the identifier to be used to communicate with the object.</param>
    /// <param name="ppv">Address of pointer variable that receives the interface pointer requested in <c>rIid</c>.</param>
    /// <returns>The result of the operation.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial ResultHandle CoCreateInstance(ref Guid rClsid, nint pUnkOther, ClassContext clsContext, ref Guid rIid, out void* ppv);
}

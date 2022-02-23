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

using System.IO;
using System.Reflection;
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation;

/// <summary>
/// Provides an object representation of a uniform resource identifier (URI) using the pack URI scheme as defined by the Open
/// Packaging Conventions specification.
/// </summary>
/// <remarks>
/// <para>
/// This class gives us a simple way to construct pack URIs, which are identifiers used frequently by WPF to locate resources
/// and the like. The pack URIs created are used to point to referenced assembly resource files. The name of the assembly which
/// the resource file exists in is included in the generated URIs. Note that these URIs are still usable even when dealing with
/// local assembly resource files -- the inclusion of the assembly name, which is not required normally when dealing with local
/// assembly resource files, does no harm to us.
/// </para>
/// <para>
/// An additional note in regards to the pack URIs created using <see cref="UriKind.Relative"/>: while the generated pack URI
/// is indeed relative according to the pack URI specification, be advised that these URIs are constructed using a leading
/// backslash. The presence of this leading backslash means that the relative pack URI is considered relative to the <c>root</c>
/// of the application, which makes them not so different from absolute URIs, except for the space saved from not having to
/// include the scheme and authority in the URI.
/// </para>
/// </remarks>
public sealed class PackUri : Uri
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PackUri"/> class.
    /// </summary>
    /// <param name="resourceAssembly">The assembly that the resource file pointed to by the URI is compiled into.</param>
    /// <param name="path">
    /// The path to the resource file, including the name of said file, relative to the root of the project folder for the
    /// assembly the resource is compiled into.
    /// </param>
    /// <remarks>This overload creates a URI of the <see cref="UriKind.Relative"/> variety.</remarks>
    /// <exception cref="ArgumentException"><c>resourceAssembly</c> does not have a name.</exception>
    public PackUri(Assembly resourceAssembly, string path)
        : this(resourceAssembly, path, UriKind.Relative)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PackUri"/> class.
    /// </summary>
    /// <param name="resourceAssembly">The assembly that the resource file pointed to by the URI is compiled into.</param>
    /// <param name="path">
    /// The path to the resource file, including the name of said file, relative to the root of the project folder for the
    /// assembly the resource is compiled into.
    /// </param>
    /// <param name="uriKind">An enumeration value specifying the kind of URI to form.</param>
    /// <exception cref="ArgumentException"><c>resourceAssembly</c> does not have a name.</exception>
    public PackUri(Assembly resourceAssembly, string path, UriKind uriKind)
        : base(MakePackUri(resourceAssembly, path, uriKind == UriKind.Relative), uriKind)
    { }

    /// <summary>
    /// Constructs a pack URI which points to the resource file whose path is relative to the location containing the declaration
    /// of the provided type.
    /// </summary>
    /// <param name="relativeType">
    /// Type whose location represents the working directory that the specified path is relative to.
    /// </param>
    /// <param name="relativePath">
    /// The path to the resource file, including the name of said file, relative to the location containing the declaration of the
    /// provided type.
    /// </param>
    /// <param name="uriKind">An enumeration value specifying the kind of URI to form.</param>
    /// <returns>A pack URI pointing to the resource file named in <c>relativePath</c>,</returns>
    /// <remarks>
    /// Normally an absolute path to the resource file must be provided when constructing a <see cref="PackUri"/>, use this factory
    /// method when there is a desire to only provide a path to the resource file in relation to the location of a type declared in
    /// the same project.
    /// </remarks>
    public static PackUri FromRelativePath(Type relativeType, string relativePath, UriKind uriKind)
    {
        Require.NotNull(relativeType, nameof(relativeType));
        Require.NotNull(relativePath, nameof(relativePath));

        string path = Path.Combine(relativeType.GetNonAssemblyNamespaceIdentifiers()
                                               .Replace('.', '/'),
                                   relativePath);

        return new PackUri(relativeType.Assembly, path, uriKind);
    }

    /// <summary>
    /// Constructs a pack URI which points to the resource file whose path is relative to the location containing the declaration of
    /// the provided type.
    /// </summary>
    /// <param name="relativeType">Type whose location represents the working directory that the specified path is relative to.</param>
    /// <param name="relativePath">
    /// The path to the resource file, including the name of said file, relative to the location containing the declaration of the
    /// provided type.
    /// </param>
    /// <returns>A pack URI pointing to the resource file named in <c>relativePath</c>,</returns>
    /// <remarks>
    /// <para>This overload creates a URI of the <see cref="UriKind.Relative"/> variety.</para>
    /// <para>
    /// Normally an absolute path to the resource file must be provided when constructing a <see cref="PackUri"/>, use this factory
    /// method when there is a desire to only provide a path to the resource file in relation to the location of a type declared in
    /// the same project.
    /// </para>
    /// </remarks>
    public static PackUri FromRelativePath(Type relativeType, string relativePath)
        => FromRelativePath(relativeType, relativePath, UriKind.Relative);

    /// <summary>
    /// Constructs a pack URI which points to the resource file whose path is relative to the location containing the declaration
    /// of the <typeparamref name="T"/> type.
    /// </summary>
    /// <param name="relativePath">
    /// The path to the resource file, including the name of said file, relative to the location containing the declaration of the
    /// provided type.
    /// </param>
    /// <param name="uriKind">An enumeration value specifying the kind of URI to form.</param>
    /// <returns>A pack URI pointing to the resource file named in <c>relativePath</c>,</returns>
    /// <typeparam name="T">A type belonging to the assembly containing the resource file to point to with the URI.</typeparam>
    /// <remarks>
    /// Normally an absolute path to the resource file must be provided when constructing a <see cref="PackUri"/>, use this
    /// factory method when there is a desire to only provide a path to the resource file in relation to the location of a type
    /// declared in the same project.
    /// </remarks>
    public static PackUri FromRelativePath<T>(string relativePath, UriKind uriKind)
        => FromRelativePath(typeof(T), relativePath, uriKind);

    /// <summary>
    /// Constructs a pack URI which points to the resource file whose path is relative to the location containing the declaration
    /// of the <typeparamref name="T"/> type.
    /// </summary>
    /// <param name="relativePath">
    /// The path to the resource file, including the name of said file, relative to the location containing the declaration of the
    /// provided type.
    /// </param>
    /// <returns>A pack URI pointing to the resource file named in <c>relativePath</c>,</returns>
    /// <typeparam name="T">A type belonging to the assembly containing the resource file to point to with the URI.</typeparam>
    /// <remarks>
    /// <para>This overload creates a URI of the <see cref="UriKind.Relative"/> variety.</para>
    /// <para>
    /// Normally an absolute path to the resource file must be provided when constructing a <see cref="PackUri"/>, use this
    /// factory method when there is a desire to only provide a path to the resource file in relation to the location of a type
    /// declared in the same project.
    /// </para>
    /// </remarks>
    public static PackUri FromRelativePath<T>(string relativePath)
        => FromRelativePath<T>(relativePath, UriKind.Relative);
        
    private static string MakePackUri(Assembly resourceAssembly, string path, bool isRelative)
    {
        Require.NotNull(resourceAssembly, nameof(resourceAssembly));
        Require.NotNull(path, nameof(path));

        var assemblyShortName = resourceAssembly.GetName().Name;

        if (assemblyShortName == null)
            throw new ArgumentException(Strings.PackUriRequiresAssemblyName, nameof(resourceAssembly));
            
        return isRelative
            ? $"/{assemblyShortName};component/{path}"
            : $"pack://application:,,,/{assemblyShortName};component/{path}";
    }
}
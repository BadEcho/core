//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Extensions;

/// <summary>
/// Provides a set of static methods that aids in matters related to the parsing of XAML.
/// </summary>
public static class XamlExtensions
{
    /// <summary>
    /// Reads this string as loose XAML, loading an object that is the root of its corresponding object tree.
    /// </summary>
    /// <typeparam name="T">The type of object at the root of the loose XAML's corresponding object tree.</typeparam>
    /// <param name="xaml">The loose XAML to parse.</param>
    /// <returns>The object that is the root of the object tree corresponding to <c>xaml</c>.</returns>
    public static T ParseLooseXaml<T>(this string xaml)
        where T : DispatcherObject
    {
        Require.NotNull(xaml, nameof(xaml));

        return xaml.ParseLooseXaml() is T typedRoot
            ? typedRoot
            : throw new ArgumentException(Strings.WrongXamlRootType, nameof(xaml));
    }

    /// <summary>
    /// Reads this string as loose XAML, loading an object that is the root of its corresponding object tree.
    /// </summary>
    /// <param name="xaml">The loose XAML to parse.</param>
    /// <returns>The object that is the root of the object tree corresponding to <c>xaml</c>.</returns>
    public static object ParseLooseXaml(this string xaml)
    {
        Require.NotNull(xaml, nameof(xaml));

        using (var stringReader = new StringReader(xaml))
        {
            using (var xmlReader = new XmlTextReader(stringReader))
            {
                return XamlReader.Load(xmlReader);
            }
        }
    }
}
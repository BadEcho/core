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

namespace BadEcho.XmlConfiguration;

/// <summary>
/// Specifies the type of entity that is missing from a configuration.
/// </summary>
public enum MissingConfigurationType
{
    /// <summary>
    /// The configuration as a whole (i.e. there is no configuration file) is missing.
    /// </summary>
    Whole,
    /// <summary>
    /// A configuration section is missing.
    /// </summary>
    Section,
    /// <summary>
    /// A configuration element is missing.
    /// </summary>
    Element,
    /// <summary>
    /// A configuration element is missing an attribute or some other constituent part.
    /// </summary>
    Attribute
}
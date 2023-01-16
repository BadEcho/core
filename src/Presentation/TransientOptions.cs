//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Data;

namespace BadEcho.Presentation;

/// <summary>
/// Provides source and target state impermanence options.
/// </summary>
public class TransientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransientOptions"/> class.
    /// </summary>
    /// <param name="binding">A binding that facilitates the realization of source and target impermanence.</param>
    public TransientOptions(IBinding binding)
    {
        Require.NotNull(binding, nameof(binding));

        Binding = binding;
    }

    /// <summary>
    /// Gets the binding that will have its changes propagated.
    /// </summary>
    public IBinding Binding
    { get; }

    /// <summary>
    /// Gets or sets the converter to use.
    /// </summary>
    public IValueConverter? Converter
    { get; set; }
}
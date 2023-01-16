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

using BadEcho.Presentation.ViewModels;

namespace BadEcho.Vision.Extensibility;

/// <summary>
/// Defines a snap-in module granting vision to Omnified data.
/// </summary>
public interface IVisionModule
{
    /// <summary>
    /// Gets the location of the module's anchor point.
    /// </summary>
    AnchorPointLocation Location { get; }

    /// <summary>
    /// Gets the direction the module will grow from its anchor point.
    /// </summary>
    GrowthDirection GrowthDirection { get; }

    /// <summary>
    /// Gets the name of the file containing messages for the Vision module.
    /// </summary>
    string MessageFile { get; }

    /// <summary>
    /// Gets a value indicating if only new messages should be processed, as opposed to the entire file, whenever
    /// the Vision module's file is updated.
    /// </summary>
    bool ProcessNewMessagesOnly { get; }

    /// <summary>
    /// Brings the Vision module online by giving it a live message file provider, causing a view model to be returned
    /// that can be used to display the Vision module.
    /// </summary>
    /// <param name="messageProvider">
    /// A live message provider that will feed the module with new messages being posted.
    /// </param>
    /// <returns>A view model that allows for the displaying of the Vision module.</returns>
    IViewModel EnableModule(IMessageFileProvider messageProvider);
}
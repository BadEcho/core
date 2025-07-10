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

using System.Diagnostics.CodeAnalysis;
using BadEcho.Properties;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides a task dialog control.
/// </summary>
public abstract class TaskDialogControl
{
    /// <summary>
    /// Gets the task dialog this control has been applied to.
    /// </summary>
    public TaskDialog? Host
    { get; private set; }

    /// <summary>
    /// Gets the identifier for the control.
    /// </summary>
    public int Id
    { get; protected set; }

    /// <summary>
    /// Gets the configuration for the task dialog this control is attached to.
    /// </summary>
    protected TaskDialogConfiguration? HostConfiguration
        => Host?.AttachedConfiguration;

    /// <summary>
    /// Gets a value indicating if this control has executed the initialization logic required after dialog creation.
    /// </summary>
    protected bool IsInitialized
    { get; private set; }

    /// <summary>
    /// Gets a value indicating if this control is attached to a dialog.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Host), nameof(HostConfiguration))]
    protected bool Attached
        => Host is { AttachedConfiguration: not null };

    /// <summary>
    /// Attaches the control to the provided task dialog.
    /// </summary>
    /// <param name="host">The task dialog to attach the control to.</param>
    /// <param name="id">Optional. The identifier for the control.</param>
    /// <returns>
    /// The <see cref="TaskDialogFlags"/> values to add to the task dialog's configuration as a result of attaching
    /// this control to <c>host</c>.
    /// </returns>
    internal TaskDialogFlags Attach(TaskDialog host, int id = 0)
    {
        Require.NotNull(host, nameof(host));

        Host = host;

        return AttachCore(id);
    }

    /// <summary>
    /// Detaches this control from the hosting task dialog.
    /// </summary>
    internal void Detach()
    {
        Host = null;
        IsInitialized = false;

        DetachCore();
    }

    /// <summary>
    /// Performs the initialization logic required by the control after dialog creation.
    /// </summary>
    internal void Initialize()
    {
        if (Host != null)
        {
            IsInitialized = true;
            InitializeCore();
        }
    }

    /// <summary>
    /// Throws an exception if this control isn't attached to a task dialog.
    /// </summary>
    [MemberNotNull(nameof(Host), nameof(HostConfiguration))]
    protected void EnsureAttached()
    {
        if (!Attached)
            throw new InvalidOperationException(Strings.TaskDialogChangeRequiresAttachment);
    }

    /// <summary>
    /// Throws an exception if this control isn't attached to a task dialog and initialized.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [MemberNotNull(nameof(Host), nameof(HostConfiguration))]
    protected void EnsureAttachedAndInitialized()
    {
        if (!Attached || !IsInitialized)
            throw new InvalidOperationException(Strings.TaskDialogOperationRequiresAttachmentInitialization);
    }

    /// <summary>
    /// Throws an exception if this control is attached to a task dialog.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    protected void EnsureUnattached()
    {
        if (Attached)
            throw new InvalidOperationException(Strings.TaskDialogChangeRequiresDetachment);
    }

    /// <summary>
    /// Executes custom logic needed to attach the control to the host.
    /// </summary>
    /// <param name="id">Optional. The identifier for the control.</param>
    /// <returns>
    /// The <see cref="TaskDialogFlags"/> values to add to the task dialog's configuration as a result of attaching
    /// this control to a task dialog.
    /// </returns>
    private protected virtual TaskDialogFlags AttachCore(int id = 0)
        => default;

    /// <summary>
    /// Executes custom logic needed to detach the control from the ho st.
    /// </summary>
    private protected virtual void DetachCore()
    { }

    /// <summary>
    /// Executes custom initialization logic required by the control after dialog creation.
    /// </summary>
    private protected virtual void InitializeCore()
    { }
}

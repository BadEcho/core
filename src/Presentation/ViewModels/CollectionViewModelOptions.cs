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

using BadEcho.Collections;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides options that configure the behavior of <see cref="ICollectionViewModel{TModel, TChildViewModel}"/> related
/// classes.
/// </summary>
/// <remarks>
/// Note that types that accept these options may override specific settings with values required for their own proper
/// operation.
/// </remarks>
public sealed class CollectionViewModelOptions
{
    /// <summary>
    /// Gets or sets a value indicating if batch processes in which numerous model data are being bound should be offloaded
    /// to a background thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Binding arbitrary large batches of data can be an expensive process, potentially tying up the user interface. Unless such
    /// operations need be synchronous for a particular reason, these operations can be made asynchronous by enabling this option,
    /// helping to alleviate any burden that would be placed on the UI thread by offloading the batch binding operation to a background
    /// thread.
    /// </para>
    /// </remarks>
    public bool AsyncBatchBindings
    { get; set; } = true;

    /// <summary>
    /// Gets or sets the amount of time that must elapse before new data bindings result in additions to the child view model
    /// collection.
    /// </summary>
    /// <remarks>
    /// Introducing a binding delay will essentially stagger the displaying of items on a collection view; instead of appearing all
    /// at once, the items will appear over time. This type of effect may be aesthetically pleasing for certain types of views.
    /// </remarks>
    public TimeSpan BindingDelay
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the bound data of view models added to the child view model collection should be bound
    /// to the collection view model itself.
    /// </summary>
    public bool BindChildren
    { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of items that must be present in a batch binding operation in order for child view model creations
    /// to be executed in parallel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Often the most computationally expensive part of binding a model to a collection view model is the creation of the view model
    /// for the data. If the child view model creation task is CPU bound, there may be substantial performance benefits in running
    /// the child view model creation tasks on multiple cores in parallel.
    /// </para>
    /// <para>
    /// Any sort of performance benefit, however, can only be realized given a sufficient number of items needing child view model creation.
    /// If the number is too small then performance will be lost on account of the overhead incurred with parallelization. This option
    /// allows you to specify how large a batch of model data to bind needs to be before enabling parallel child view model creation.
    /// </para>
    /// </remarks>
    public int BindingParallelizationThreshold
    { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum number of items the child view model collection can hold at any given time.
    /// </summary>
    public int Capacity 
    { get; set; }

    /// <summary>
    /// Gets or sets the amount of time that must elapse before any actions required to ensure that the items in the child view model collection
    /// do not exceed any defined capacity are executed.
    /// </summary>
    public TimeSpan CapacityEnforcementDelay
    { get; set; }

    /// <summary>
    /// Gets or sets a limit on the number of items the child view model collection may hold before any delay imposed on capacity enforcement
    /// is simply skipped, resulting in the immediate removal of excess items.
    /// </summary>
    public int CapacityEnforcementDelayLimit
    { get; set; }

    /// <summary>
    /// Gets or sets the method meant to handle changes in either the children collection's composition or property values of items
    /// belonging to said collection.
    /// </summary>
    public EventHandler<CollectionPropertyChangedEventArgs>? ChildrenChangedHandler 
    { get; set; }
}
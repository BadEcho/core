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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BadEcho.Presentation.Views;

/// <summary>
/// Provides the root presentational logic for all views used in a Bad Echo Presentation framework application.
/// </summary>
public class View : UserControl
{
    private object? _localDesignContext;

    /// <summary>
    /// Initializes the <see cref="View"/> class.
    /// </summary>
    static View() 
        => DataContextProperty.OverrideMetadata(typeof(View), new FrameworkPropertyMetadata(null, OnCoerceDataContext));

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    protected View() 
        => UserInterface.BuildEnvironment();

    private static object? OnCoerceDataContext(DependencyObject sender, object? baseValue)
    {
        View view = (View) sender;

        return view.EnsureLocalContext(baseValue);
    }

    /// <summary>
    /// Ensures that the view maintains its locally defined design-time data context.
    /// </summary>
    /// <param name="newContext">A new data context value being coerced.</param>
    /// <returns>The coerced data context to use.</returns>
    /// <remarks>
    /// <para>
    /// The WPF designer for .NET 5.0 lacks proper support for the <c>DesignData</c> markup extension, which means
    /// (in my opinion) that it lacks support for the best and cleanest way to define and display meaningful design-time
    /// data.
    /// </para>
    /// <para>
    /// The approach used as an alternative to the aforementioned markup extension is the very undocumented
    /// <c>DesignProperties.DataContext</c> attached property, which allows us to essentially define a design-time data
    /// context in much the same way it would be defined in a design data file (keeping in mind that it must be done in the 
    /// same *.xaml file where a particular view is actually defined).
    /// </para>
    /// <para>
    /// Unfortunately, due to the way that dependency property inheritance works, we begin to see problems with design-time views
    /// that have design-time data contexts defined both locally in regards to said view's definition as well as
    /// locally in regards to a separate view's definition that references said view. Indeed, in such an instance we will observe
    /// that the original design-time data will overwrite the local (in regards to the active file that is referencing the view)
    /// design-time data.
    /// </para>
    /// <para>
    /// As an example, assume we have a WidgetView, with its own design-time data defined in WidgetView.xaml. Next, assume we also have
    /// a WidgetsView composed of multiple WidgetViews defined, with its own design-time data that includes a few WidgetViews.
    /// In this case, we would only see the design-time data that was defined in WidgetView.xaml show up for each instance of
    /// WidgetView in WidgetViews.xaml.
    /// </para>
    /// <para>
    /// This is quite untenable for reasons that are hopefully obvious. In order to prevent this from happening, this method is invoked
    /// during the coercion of a view's data context. It prevents the above problem from happening by ensuring that only the inherited
    /// data context (which, from the developer's point of view actually would seem to be the local data context, as it is defined in
    /// the same XAML document in which the data context is intended for use -- from the perspective of the dependency property system,
    /// however, it will actually be tracked as inherited, whereas the design-time data context originally defined in the view's own
    /// *.xaml file is the one regarded as "local") for a view is accepted as a valid data context, except for when there is a change in
    /// the data context's type. This last condition is required in order to support nested view model data contexts, which will be
    /// present in almost all but the most trivial of views.
    /// </para>
    /// </remarks>
    private object? EnsureLocalContext(object? newContext)
    {
        if (!DesignerProperties.GetIsInDesignMode(this))
            return newContext;

        if (_localDesignContext != null)
        {
            if (_localDesignContext.GetType() != newContext?.GetType())
                _localDesignContext = newContext;

            return _localDesignContext;
        }

        _localDesignContext = newContext;

        return newContext;
    }
}
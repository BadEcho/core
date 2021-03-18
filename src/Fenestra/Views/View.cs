//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace BadEcho.Fenestra.Views
{
    /// <summary>
    /// Provides the root presentational logic for all views used in a Fenestra-based application.
    /// </summary>
    public class View : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        protected View() 
            => UserInterface.BuildEnvironment();
    }
}

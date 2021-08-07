//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;
using BadEcho.Fenestra.Markup;

namespace BadEcho.Omnified.Vision.Extensibility.Themes
{
    /// <summary>
    /// Provides resource keys for <see cref="Color"/> resources used by Vision and its modules.
    /// </summary>
    public static class Colors
    {
        private static readonly FenestraKeyProvider _Provider = new(typeof(Colors));

        /// <summary>
        /// The standard color for standard text appearing in Vision.
        /// </summary>
        public static readonly ResourceKey TextColorKey =
            _Provider.CreateKey(nameof(TextColorKey));

        /// <summary>
        /// The color for text emphasizing a critical event in Vision.
        /// </summary>
        public static readonly ResourceKey CriticalTextColorKey =
            _Provider.CreateKey(nameof(CriticalTextColorKey));

        /// <summary>
        /// The color used for outlining standard text appearing in Vision.
        /// </summary>
        public static readonly ResourceKey OutlineColorKey =
            _Provider.CreateKey(nameof(OutlineColorKey));
    }
}

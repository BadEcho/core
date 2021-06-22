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
    /// Provides resource keys for <see cref="Brush"/> resources used by Vision and its modules.
    /// </summary>
    public static class Brushes
    {
        private static readonly FenestraKeyProvider _Provider = new(typeof(Brushes));

        /// <summary>
        /// The foreground brush used for text appearing in Vision.
        /// </summary>
        public static readonly ResourceKey TextForegroundKey =
            _Provider.CreateKey(nameof(TextForegroundKey));

        /// <summary>
        /// The foreground brush used for text emphasizing a critical event in Vision.
        /// </summary>
        public static readonly ResourceKey CriticalTextForegroundKey =
            _Provider.CreateKey(nameof(CriticalTextForegroundKey));
    }
}
//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Fenestra.Markup;

namespace BadEcho.Omnified.Vision.Extensibility.Themes
{
    /// <summary>
    /// Provides resource keys for font-related resources used by Vision and its modules.
    /// </summary>
    public static class Fonts
    {
        private static readonly FenestraKeyProvider _Provider = new(typeof(Fonts));

        /// <summary>
        /// The standard font family for text appearing in Vision.
        /// </summary>
        public static readonly ResourceKey FontFamilyKey =
            _Provider.CreateKey(nameof(FontFamilyKey));
    }
}

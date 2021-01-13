//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace BadEcho.Fenestra.Controls
{
    /// <summary>
    /// Provides a text element that supports both a fill brush for the glyphs and a stroke brush surrounding the glyphs.
    /// </summary>
    [ContentProperty(nameof(Text))]
    public sealed class OutlinedTextElement : FrameworkElement
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty
            = DependencyProperty.Register(nameof(Fill),
                                          typeof(Brush),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(Brushes.Black,
                                                                        FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// Identifies the <see cref="Stroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty
            = DependencyProperty.Register(nameof(Stroke),
                                          typeof(Brush),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(Brushes.Black,
                                                                        FrameworkPropertyMetadataOptions.AffectsRender,
                                                                        OnStrokePropertyChanged));
        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty
            = DependencyProperty.Register(nameof(StrokeThickness),
                                          typeof(double),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(1.0,
                                                                        FrameworkPropertyMetadataOptions.AffectsRender,
                                                                        OnStrokePropertyChanged));
        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty
            = TextElement.FontFamilyProperty.AddOwner(typeof(OutlinedTextElement),
                                                      new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty
            = TextElement.FontSizeProperty.AddOwner(typeof(OutlinedTextElement),
                                                    new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="FontStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty
            = TextElement.FontStretchProperty.AddOwner(typeof(OutlinedTextElement),
                                                       new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty
            = TextElement.FontStyleProperty.AddOwner(typeof(OutlinedTextElement),
                                                     new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty
            = TextElement.FontWeightProperty.AddOwner(typeof(OutlinedTextElement),
                                                      new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty
            = Block.LineHeightProperty.AddOwner(typeof(OutlinedTextElement),
                                                new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(nameof(Text),
                                          typeof(string),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(OnTextInvalidated));
        /// <summary>
        /// Identifies the <see cref="TextAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty
            = DependencyProperty.Register(nameof(TextAlignment),
                                          typeof(TextAlignment),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="TextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty
            = DependencyProperty.Register(nameof(TextDecorations),
                                          typeof(TextDecorationCollection),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="TextTrimming"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextTrimmingProperty
            = DependencyProperty.Register(nameof(TextTrimming),
                                          typeof(TextTrimming),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(OnTextFormatUpdated));
        /// <summary>
        /// Identifies the <see cref="TextWrapping"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty
            = DependencyProperty.Register(nameof(TextWrapping),
                                          typeof(TextWrapping),
                                          typeof(OutlinedTextElement),
                                          new FrameworkPropertyMetadata(OnTextFormatUpdated));

        private FormattedText? _formattedText;
        private Geometry? _textGeometry;
        private Pen _textPen;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlinedTextElement"/> class.
        /// </summary>
        public OutlinedTextElement()
        {
            UpdateTextPen();
            TextDecorations = new TextDecorationCollection();
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that specifies how the glyph interiors are painted.
        /// </summary>
        public Brush Fill
        {
            get => (Brush) GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that specifies how the outline is painted.
        /// </summary>
        public Brush Stroke
        {
            get => (Brush) GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the outline.
        /// </summary>
        public double StrokeThickness
        {
            get => (double) GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the preferred top-level font family for the content of the element.
        /// </summary>
        public FontFamily FontFamily
        {
            get => (FontFamily) GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the font size for the content of the element.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double) GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the glyph width of the font in a family to select.
        /// </summary>
        public FontStretch FontStretch
        {
            get => (FontStretch) GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
        }

        /// <summary>
        /// Gets or sets the font style for the content in this element.
        /// </summary>
        public FontStyle FontStyle
        {
            get => (FontStyle) GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the top-level font weight to select from the font family for the content
        /// in this element.
        /// </summary>
        public FontWeight FontWeight
        {
            get => (FontWeight) GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the height of each line of content.
        /// </summary>
        public double LineHeight
        {
            get => (double) GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="TextDecorationCollection"/> instance for the text element.
        /// </summary>
        public TextDecorationCollection TextDecorations
        {
            get => (TextDecorationCollection) GetValue(TextDecorationsProperty);
            private set => SetValue(TextDecorationsProperty, value);
        }

        /// <summary>
        /// Gets or sets the text contents of this element.
        /// </summary>
        public string? Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that specifies the horizontal alignment of text content.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get => (TextAlignment) GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets the text trimming behavior to employ when content overflows the content area.
        /// </summary>
        public TextTrimming TextTrimming
        {
            get => (TextTrimming) GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        /// <summary>
        /// Gets or sets how the text content should be wrapped.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping) GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        /// <summary>
        /// Gets the text control used to draw the text for this element.
        /// </summary>
        private FormattedText FormattedText
        {
            get
            {
                if (_formattedText != null)
                    return _formattedText;

                DpiScale dpi = VisualTreeHelper.GetDpi(this);

                _formattedText = new FormattedText(Text ?? string.Empty,
                                                   CultureInfo.CurrentUICulture,
                                                   FlowDirection,
                                                   new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                                                   FontSize,
                                                   Brushes.Black,
                                                   dpi.PixelsPerDip);
                UpdateTextFormat();

                return _formattedText;
            }
        }

        /// <summary>
        /// Gets the geometric control for the outlined text.
        /// </summary>
        private Geometry TextGeometry
        {
            get
            {
                if (_textGeometry != null)
                    return _textGeometry;

                _textGeometry = FormattedText.BuildGeometry(new Point(0, 0));

                return _textGeometry;
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            FormattedText.MaxTextWidth = finalSize.Width;
            FormattedText.MaxTextHeight = Math.Max(double.Epsilon, finalSize.Height);

            _textGeometry = null;

            return base.ArrangeOverride(finalSize);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Measures the size required by the internal <see cref="FormattedText"/> instance, preventing the text
        /// from having its width set to infinite or its height set to zero as that would result in an error.
        /// </remarks>
        protected override Size MeasureOverride(Size availableSize)
        {
            FormattedText.MaxTextWidth = Math.Min(Constants.InfiniteLineWidth, availableSize.Width);
            FormattedText.MaxTextHeight = Math.Max(double.Epsilon, availableSize.Height);

            return new Size(Math.Ceiling(FormattedText.Width), Math.Ceiling(FormattedText.Height));
        }

        /// <inheritdoc/>
        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            FormattedText.PixelsPerDip = newDpi.PixelsPerDip;

            UpdateTextFormat();

            base.OnDpiChanged(oldDpi, newDpi);
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (drawingContext == null)
                throw new ArgumentNullException(nameof(drawingContext));

            drawingContext.DrawGeometry(null, _textPen, TextGeometry);
            drawingContext.DrawGeometry(Fill, null, TextGeometry);
            
            base.OnRender(drawingContext);
        }

        private static void OnTextFormatUpdated(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textElement = (OutlinedTextElement) sender;

            textElement.ResetText(true);
        }

        private static void OnTextInvalidated(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textElement = (OutlinedTextElement) sender;

            textElement.ResetText(false);
        }

        private static void OnStrokePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textElement = (OutlinedTextElement) sender;

            textElement.UpdateTextPen();
        }

        private void ResetText(bool reuseFormattedText)
        {
            if (reuseFormattedText)
                UpdateTextFormat();
            else
                _formattedText = null;

            _textGeometry = null;

            InvalidateMeasure();
            InvalidateVisual();
        }

        private void UpdateTextFormat()
        {
            FormattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            FormattedText.LineHeight = LineHeight;
            FormattedText.TextAlignment = TextAlignment;
            FormattedText.Trimming = TextTrimming;

            FormattedText.SetFontSize(FontSize);
            FormattedText.SetFontStyle(FontStyle);
            FormattedText.SetFontWeight(FontWeight);
            FormattedText.SetFontStretch(FontStretch);
            FormattedText.SetTextDecorations(TextDecorations);
        }

        [MemberNotNull(nameof(_textPen))]
        private void UpdateTextPen()
        {
            _textPen = new Pen(Stroke, StrokeThickness)
            {
                DashCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round,
                StartLineCap = PenLineCap.Round
            };

            InvalidateVisual();
        }
    }
}
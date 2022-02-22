//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using BadEcho.Fenestra.Properties;
using BadEcho.Extensions;
using BadEcho.Logging;

namespace BadEcho.Fenestra.Controls;

/// <summary>
/// Provides a text element that supports both a fill brush for the glyphs and a stroke brush surrounding the glyphs.
/// </summary>
[ContentProperty(nameof(Text))]
public sealed class OutlinedTextElement : FrameworkElement
{
    private const string SELF_FORMAT_ITEM = "{0}";

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
    /// Identifies the <see cref="InnerStroke"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InnerStrokeProperty
        = DependencyProperty.Register(nameof(InnerStroke),
                                      typeof(Brush),
                                      typeof(OutlinedTextElement),
                                      new FrameworkPropertyMetadata(Brushes.Black,
                                                                    FrameworkPropertyMetadataOptions.AffectsRender,
                                                                    OnInnerStrokePropertyChanged));
    /// <summary>
    /// Identifies the <see cref="InnerStrokeThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InnerStrokeThicknessProperty
        = DependencyProperty.Register(nameof(InnerStrokeThickness),
                                      typeof(double),
                                      typeof(OutlinedTextElement),
                                      new FrameworkPropertyMetadata(1.0,
                                                                    FrameworkPropertyMetadataOptions.AffectsRender,
                                                                    OnInnerStrokePropertyChanged));
    /// <summary>
    /// Identifies the <see cref="OuterStroke"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OuterStrokeProperty
        = DependencyProperty.Register(nameof(OuterStroke),
                                      typeof(Brush),
                                      typeof(OutlinedTextElement),
                                      new FrameworkPropertyMetadata(Brushes.Black,
                                                                    FrameworkPropertyMetadataOptions.AffectsRender,
                                                                    OnOuterStrokePropertyChanged));
    /// <summary>
    /// Identifies the <see cref="OuterStrokeThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OuterStrokeThicknessProperty
        = DependencyProperty.Register(nameof(OuterStrokeThickness),
                                      typeof(double),
                                      typeof(OutlinedTextElement),
                                      new FrameworkPropertyMetadata(0.0,
                                                                    FrameworkPropertyMetadataOptions.AffectsRender,
                                                                    OnOuterStrokePropertyChanged));
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
    /// Identifies the <see cref="TextFormatString"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextFormatStringProperty
        = DependencyProperty.Register(nameof(TextFormatString),
                                      typeof(string),
                                      typeof(OutlinedTextElement),
                                      new FrameworkPropertyMetadata(SELF_FORMAT_ITEM, OnTextInvalidated, OnCoerceTextFormatString));
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
    private Geometry? _innerTextGeometry;
    private Geometry? _outerTextGeometry;
    private Pen _innerOutlinePen;
    private Pen _outerOutlinePen;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutlinedTextElement"/> class.
    /// </summary>
    public OutlinedTextElement()
    {
        UpdateInnerOutlinePen();
        UpdateOuterOutlinePen();

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
    /// Gets or sets the <see cref="Brush"/> that specifies how the inner outline is painted.
    /// </summary>
    public Brush InnerStroke
    {
        get => (Brush) GetValue(InnerStrokeProperty);
        set => SetValue(InnerStrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the inner outline.
    /// </summary>
    public double InnerStrokeThickness
    {
        get => (double) GetValue(InnerStrokeThicknessProperty);
        set => SetValue(InnerStrokeThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> that specifies how the outer outline is painted.
    /// </summary>
    public Brush OuterStroke
    {
        get => (Brush) GetValue(OuterStrokeProperty);
        set => SetValue(OuterStrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the outer outline.
    /// </summary>
    public double OuterStrokeThickness
    {
        get => (double) GetValue(OuterStrokeThicknessProperty);
        set => SetValue(OuterStrokeThicknessProperty, value);
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
        private init => SetValue(TextDecorationsProperty, value);
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
    /// Gets or sets a string that specifies how to format the text contents of this element.
    /// </summary>
    /// <remarks>
    /// Leaving this property unset, or setting it back to its default value, will result in no formatting being applied to
    /// the <see cref="Text"/> property.
    /// </remarks>
    public string TextFormatString
    {
        get => (string) GetValue(TextFormatStringProperty);
        set => SetValue(TextFormatStringProperty, value);
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

            var text = !string.IsNullOrEmpty(Text) ? FormatText(Text, TextFormatString) : string.Empty;
                
            _formattedText = new FormattedText(text,
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
    /// Gets the geometric control for the inner portion of the outlined text.
    /// </summary>
    private Geometry InnerTextGeometry
    {
        get
        {
            if (_innerTextGeometry != null)
                return _innerTextGeometry;
                
            _innerTextGeometry = FormattedText.BuildGeometry(new Point(0, 0));

            return _innerTextGeometry;
        }
    }

    /// <summary>
    /// Gets the geometric control for the outer portion of the outlined text.
    /// </summary>
    private Geometry OuterTextGeometry
    {
        get
        {
            if (_outerTextGeometry != null)
                return _outerTextGeometry;

            Geometry widenedPathGeometry
                = InnerTextGeometry.GetWidenedPathGeometry(_innerOutlinePen);

            try
            {
                _outerTextGeometry
                    = Geometry.Combine(InnerTextGeometry, widenedPathGeometry, GeometryCombineMode.Union, null);
            }
            catch (COMException comEx)
            {
                if (comEx.HResult != unchecked((int)0x88980004))
                    throw;

                // For reasons known only to those versed in internal Direct3D mechanics, sometimes the geometry scanner fails to process the data.
                // Creating a combined geometry isn't going to work until another pass, so we just draw over the inner text geometry for now.
                _outerTextGeometry = InnerTextGeometry;
            }

            return _outerTextGeometry;
        }
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
    protected override Size ArrangeOverride(Size finalSize)
    {
        FormattedText.MaxTextWidth = finalSize.Width;
        FormattedText.MaxTextHeight = Math.Max(double.Epsilon, finalSize.Height);

        _innerTextGeometry = null;
        _outerTextGeometry = null;

        return base.ArrangeOverride(finalSize);
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
        Require.NotNull(drawingContext, nameof(drawingContext));

        drawingContext.DrawGeometry(null, _innerOutlinePen, InnerTextGeometry);
        drawingContext.DrawGeometry(null, _outerOutlinePen, OuterTextGeometry);
        drawingContext.DrawGeometry(Fill, null, InnerTextGeometry);
            
        base.OnRender(drawingContext);
    }

    private static string FormatText(string text, string formatString)
    {
        try
        {
            // In the event that our format string is actually a numeric format string, we'll want to convert our text
            // to the appropriate numeric type before formatting it.
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int integerText))
                return formatString.CulturedFormat(integerText);

            // If the text cannot be converted to an integer, then it is either a number containing a fractional part, or not a number at all
            // (at least, not completely a number, i.e., a mix of both letters and numbers).
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out double floatingPointText))
                return formatString.CulturedFormat(floatingPointText);

            // For standard date and time format strings, we'll need the text to be parsed as a DateTime prior to formatting.
            return DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime dateTimeText)
                ? formatString.CulturedFormat(dateTimeText)
                : formatString.CulturedFormat(text);
        }
        catch (FormatException formatEx)
        {   // Given the complexity of format strings, there is really no way to check if a given format string is valid for a given string
            // value prior to actually attempting to format said value with it. We do want to catch any format errors, however, as a mistyped
            // format string will result in the WPF designer being unable to render the particular control instance until the WpfSurface process 
            // gets recycled.
            Logger.Error(Strings.OutlinedTextBadFormatString.InvariantFormat(formatString), formatEx);

            return text;
        }
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

    private static void OnInnerStrokePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var textElement = (OutlinedTextElement) sender;

        textElement.UpdateInnerOutlinePen();
    }
        
    private static void OnOuterStrokePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var textElement = (OutlinedTextElement) sender;

        textElement.UpdateOuterOutlinePen();
    }

    private static object OnCoerceTextFormatString(DependencyObject sender, object baseValue)
        => string.IsNullOrEmpty((string) baseValue)
            ? SELF_FORMAT_ITEM
            : baseValue;

    private void ResetText(bool reuseFormattedText)
    {
        if (reuseFormattedText)
            UpdateTextFormat();
        else
            _formattedText = null;

        _innerTextGeometry = null;
        _outerTextGeometry = null;

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

    [MemberNotNull(nameof(_innerOutlinePen))]
    private void UpdateInnerOutlinePen()
    {
        _innerOutlinePen = new Pen(InnerStroke, InnerStrokeThickness)
                           {
                               DashCap = PenLineCap.Round,
                               EndLineCap = PenLineCap.Round,
                               LineJoin = PenLineJoin.Round,
                               StartLineCap = PenLineCap.Round
                           };

        InvalidateVisual();
    }

    [MemberNotNull(nameof(_outerOutlinePen))]
    private void UpdateOuterOutlinePen()
    {
        _outerOutlinePen = new Pen(OuterStroke, OuterStrokeThickness)
                           {
                               DashCap = PenLineCap.Round,
                               EndLineCap = PenLineCap.Round,
                               LineJoin = PenLineJoin.Round,
                               StartLineCap = PenLineCap.Round
                           };

        InvalidateVisual();
    }
}
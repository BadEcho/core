//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BadEcho.Fenestra;
using BadEcho.Odin;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.Controls
{
    /// <summary>
    /// Provides an area where you can attach child elements to specific anchor points encompassing the area.
    /// </summary>
    public sealed class AnchorPointPanel : Panel
    {
        /// <summary>
        /// Identifies the attached property indicating the anchor point of a child element within a parent
        /// <see cref="AnchorPointPanel"/>.
        /// </summary>
        public static readonly DependencyProperty LocationProperty
            = DependencyProperty.RegisterAttached(NameOf.ReadDependencyPropertyName(() => LocationProperty),
                                                  typeof(AnchorPointLocation),
                                                  typeof(AnchorPointPanel),
                                                  new FrameworkPropertyMetadata(AnchorPointLocation.TopLeft,
                                                                                OnAnchorPointChanged),
                                                  OnValidateAnchorPoint);
        /// <summary>
        /// Gets the value of the <see cref="LocationProperty"/> attached property for a given <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The element from which the property value is read.</param>
        /// <returns>The anchor point that <c>element</c> is attached to within a parent <see cref="AnchorPointPanel"/>.</returns>
        public static AnchorPointLocation GetLocation(UIElement element)
        {
            Require.NotNull(element, nameof(element));
            
            return (AnchorPointLocation) element.GetValue(LocationProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="LocationProperty"/> attached property on a given <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="anchorPoint">The anchor point location to set.</param>
        public static void SetLocation(UIElement element, AnchorPointLocation anchorPoint)
        {
            Require.NotNull(element, nameof(element));

            element.SetValue(LocationProperty, anchorPoint);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Currently, elements are measured out so that they are allowed all the size that they desire. As Vision evolves and more
        /// modules are added to it, we'll be able to reevaluate this so the best approach can be decided on as far as how modules
        /// that are requesting more size than is available are handled.
        /// </remarks>
        protected override Size MeasureOverride(Size availableSize)
        {
            var panelSize = new Size();

            var elements = InternalChildren.OfType<UIElement>()
                                           .ToList();
            var leftElements 
                = GetElementsAtLocations(AnchorPointLocation.TopLeft, AnchorPointLocation.BottomLeft);
            var leftColumnSize 
                = MeasureColumnedAnchorPoints(leftElements, availableSize);

            var centerElements 
                = GetElementsAtLocations(AnchorPointLocation.TopCenter, AnchorPointLocation.BottomCenter);
            var centerColumnSize 
                = MeasureColumnedAnchorPoints(centerElements, availableSize);

            var rightElements 
                = GetElementsAtLocations(AnchorPointLocation.TopRight, AnchorPointLocation.BottomRight);
            var rightColumnSize 
                = MeasureColumnedAnchorPoints(rightElements, availableSize);

            panelSize.Height = Math.Max(Math.Max(leftColumnSize.Height, centerColumnSize.Height),
                                        rightColumnSize.Height);

            panelSize.Width = leftColumnSize.Width + centerColumnSize.Width + rightColumnSize.Width;

            return panelSize;

            IEnumerable<UIElement> GetElementsAtLocations(AnchorPointLocation topLocation, AnchorPointLocation bottomLocation)
            {
                return elements
                       .Select(e => (Element: e, Location: GetLocation(e)))
                       .Where(el => el.Location == topLocation || el.Location == bottomLocation)
                       .Select(el => el.Element)
                       .ToList();
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Elements attached to anchor points are arranged into three separate columns, corresponding to the anchor points
        /// located at the left, center, and right of the panel area. Elements attached to the same anchor point are laid out in
        /// order, from top to bottom.
        /// </para>
        /// <para>
        /// Elements attached to the top variety of anchor are arranged so that the topmost element attached to the anchor point
        /// begin at the top of the panel area. Elements attached to the bottom variety of anchor points are arranged so that the
        /// bottommost element attached to the anchor point ends at the bottom of the panel area.
        /// </para>
        /// </remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var elements = InternalChildren.OfType<UIElement>()
                                           .ToList();
            var topLeftElements 
                = GetElementsAtLocation(AnchorPointLocation.TopLeft);
            var bottomLeftElements 
                = GetElementsAtLocation(AnchorPointLocation.BottomLeft);
            var topCenterElements 
                = GetElementsAtLocation(AnchorPointLocation.TopCenter);
            var bottomCenterElements 
                = GetElementsAtLocation(AnchorPointLocation.BottomCenter);
            var topRightElements 
                = GetElementsAtLocation(AnchorPointLocation.TopRight);
            var bottomRightElements 
                = GetElementsAtLocation(AnchorPointLocation.BottomRight);

            ArrangeColumnedAnchorPoints(topLeftElements, bottomLeftElements, 0, finalSize.Height);

            ArrangeColumnedAnchorPoints(topCenterElements,
                                       bottomCenterElements,
                                       finalSize.Width / 2,
                                       finalSize.Height,
                                       GetXOffsetForCenter);

            ArrangeColumnedAnchorPoints(topRightElements,
                                       bottomRightElements,
                                       finalSize.Width,
                                       finalSize.Height,
                                       GetXOffsetForRight);
            return finalSize;

            static double GetXOffsetForCenter(UIElement element)
            {
                return element.DesiredSize.Width / 2;
            }

            static double GetXOffsetForRight(UIElement element)
            {
                return element.DesiredSize.Width;
            }

            IReadOnlyCollection<UIElement> GetElementsAtLocation(AnchorPointLocation location)
            {
                return elements.Where(e => GetLocation(e) == location)
                               .ToList();
            }
        }
        
        private static Size MeasureColumnedAnchorPoints(IEnumerable<UIElement> columnedElements, Size availableSize)
        {
            var columnSize = new Size();

            foreach (UIElement element in columnedElements)
            {
                element.Measure(availableSize);

                columnSize.Height += element.DesiredSize.Height;
                columnSize.Width = Math.Max(element.DesiredSize.Width, columnSize.Width);
            }

            return columnSize;
        }

        private static void ArrangeColumnedAnchorPoints(IReadOnlyCollection<UIElement> topElements,
                                                        IReadOnlyCollection<UIElement> bottomElements,
                                                        double startingX,
                                                        double panelHeight,
                                                        Func<UIElement, double>? xOffsetForElement = null)
        {
            ArrangeAnchorPoint(topElements, startingX, 0, xOffsetForElement);

            double topHeight = topElements.Sum(e => e.DesiredSize.Height);
            double bottomHeight = bottomElements.Sum(e => e.DesiredSize.Height);

            double startingY = Math.Max(topHeight, panelHeight - bottomHeight);

            ArrangeAnchorPoint(bottomElements, startingX, startingY, xOffsetForElement);
        }

        private static void ArrangeAnchorPoint(IEnumerable<UIElement> elements, 
                                               double startingX, 
                                               double startingY, 
                                               Func<UIElement, double>? xOffsetForElement = null)
        {
            foreach (var element in elements)
            {
                double effectiveX = startingX;

                if (xOffsetForElement != null)
                    effectiveX -= xOffsetForElement(element);

                var elementRect = new Rect(effectiveX, startingY, element.DesiredSize.Width, element.DesiredSize.Height);

                element.Arrange(elementRect);

                startingY += element.DesiredSize.Height;
            }
        }

        private static void OnAnchorPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement) sender;

            var panel = VisualTreeHelper.GetParent(element) as UIElement;

            panel?.InvalidateMeasure();
        }

        private static bool OnValidateAnchorPoint(object value)
        {
            AnchorPointLocation location = (AnchorPointLocation)value;

            return location
                is AnchorPointLocation.TopLeft
                or AnchorPointLocation.BottomLeft
                or AnchorPointLocation.TopCenter
                or AnchorPointLocation.BottomCenter
                or AnchorPointLocation.TopRight
                or AnchorPointLocation.BottomRight;
        }
    }
}

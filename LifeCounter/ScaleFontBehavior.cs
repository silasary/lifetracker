﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace LifeCounter.Behavior
{
    public class ScaleFontBehavior : Behavior<Grid>
    {
        // MaxFontSize
        public double MaxFontSize { get { return (double)GetValue(MaxFontSizeProperty); } set { SetValue(MaxFontSizeProperty, value); } }
        public static readonly DependencyProperty MaxFontSizeProperty = DependencyProperty.Register("MaxFontSize", typeof(double), typeof(ScaleFontBehavior), new PropertyMetadata(20d));

        protected override void OnAttached()
        {
            this.AssociatedObject.SizeChanged += (s, e) => { CalculateFontSize(); };
        }

        private void CalculateFontSize()
        {
            double fontSize = this.MaxFontSize;

            List<TextBlock> tbs = VisualHelper.FindVisualChildren<TextBlock>(this.AssociatedObject);

            // get grid height (if limited)
            double gridHeight = double.MaxValue;
            Grid parentGrid = VisualHelper.FindUpVisualTree<Grid>(this.AssociatedObject.Parent);
            if (parentGrid != null)
            {
                RowDefinition row = parentGrid.RowDefinitions[Grid.GetRow(this.AssociatedObject)];
                gridHeight = row.Height == GridLength.Auto ? double.MaxValue : this.AssociatedObject.ActualHeight;
            }

            foreach (var tb in tbs)
            {
                // get desired size with fontsize = MaxFontSize
                Size desiredSize = MeasureText(tb);
                double widthMargins = tb.Margin.Left + tb.Margin.Right;
                double heightMargins = tb.Margin.Top + tb.Margin.Bottom;

                double desiredHeight = desiredSize.Height + heightMargins;
                double desiredWidth = desiredSize.Width + widthMargins;

                // adjust fontsize if text would be clipped vertically
                if (gridHeight < desiredHeight)
                {
                    double factor = (desiredHeight - heightMargins) / (this.AssociatedObject.ActualHeight - heightMargins);
                    fontSize = Math.Min(fontSize, MaxFontSize / factor);
                }

                // get column width (if limited)
                ColumnDefinition col = this.AssociatedObject.ColumnDefinitions[Grid.GetColumn(tb)];
                double colWidth = col.Width == GridLength.Auto ? double.MaxValue : col.ActualWidth;

                // adjust fontsize if text would be clipped horizontally
                if (colWidth < desiredWidth)
                {
                    double factor = (desiredWidth - widthMargins) / (col.ActualWidth - widthMargins);
                    fontSize = Math.Min(fontSize, MaxFontSize / factor);
                }
            }

            // apply fontsize (always equal fontsizes)
            foreach (var tb in tbs)
            {
                tb.FontSize = fontSize;
            }
        }

        // Measures text size of textblock
        private Size MeasureText(TextBlock tb)
        {
            var formattedText = new FormattedText(tb.Text, CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                this.MaxFontSize, Brushes.Black); // always uses MaxFontSize for desiredSize

            return new Size(formattedText.Width, formattedText.Height);
        }
    }

    class VisualHelper
    {
        public static List<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var o = VisualTreeHelper.GetChild(obj, i);
                if (o != null)
                {
                    if (o is T)
                        children.Add((T)o);

                    children.AddRange(FindVisualChildren<T>(o)); // recursive
                }
            }
            return children;
        }

        public static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            DependencyObject current = initial;

            while (current != null && current.GetType() != typeof(T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }
    }
}
﻿using Mapsui.Geometries;
using Mapsui.Widgets.ScaleBar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mapsui.Rendering.Xaml
{
    public static class ScaleBarWidgetRenderer
    {
        private const float StrokeExternal = 4;
        private const float StrokeInternal = 2;

        private static Brush _brushScaleBar;
        private static Brush _brushScaleBarStroke;
        private static Brush _brushScaleText;
        private static Brush _brushScaleTextStroke;

        public static void Draw(Canvas canvas, ScaleBarWidget scaleBar)
        {
            // If this widget belongs to no viewport, than stop drawing
            if (scaleBar.Map?.CRS == null) return;
            if (scaleBar.Map.Transformation == null) return;
            if (scaleBar.Map.Transformation.IsProjectionSupported(scaleBar.Map.CRS, "EPSG:4326") != true) return;


            _brushScaleBar = new SolidColorBrush(scaleBar.TextColor.ToXaml());
            _brushScaleBarStroke = new SolidColorBrush(scaleBar.Halo.ToXaml());
            _brushScaleText = new SolidColorBrush(scaleBar.TextColor.ToXaml());
            _brushScaleTextStroke = new SolidColorBrush(scaleBar.Halo.ToXaml());

            var textBlock = new OutlinedTextBlock();

            textBlock.Text = "9999 m";
            textBlock.Fill = _brushScaleText;
            textBlock.Stroke = _brushScaleTextStroke;
            textBlock.StrokeThickness = StrokeExternal - StrokeInternal + 1;
            textBlock.FontFamily = new FontFamily(scaleBar.Font.FontFamily);
            textBlock.FontSize = scaleBar.Font.Size;
            textBlock.FontWeight = FontWeights.Bold;

            float scaleBarLength1;
            string scaleBarText1;
            float scaleBarLength2;
            string scaleBarText2;

            (scaleBarLength1, scaleBarText1, scaleBarLength2, scaleBarText2) = scaleBar.GetScaleBarLengthAndText();

            // Calc height of scale bar
            Size textSize;

            // Do this, because height of text changes sometimes (e.g. from 2 m to 1 m)
            textSize = textBlock.MeasureText();

            var scaleBarHeight = textSize.Height + (scaleBar.TickLength + StrokeExternal * 0.5f + scaleBar.TextMargin) * scaleBar.Scale;

            if (scaleBar.ScaleBarMode == ScaleBarMode.Both && scaleBar.SecondaryUnitConverter != null)
            {
                scaleBarHeight *= 2;
            }
            else
            {
                scaleBarHeight += StrokeExternal * 0.5f * scaleBar.Scale;
            }

            scaleBar.Height = (float)scaleBarHeight;

            // Draw lines

            // Get lines for scale bar
            var points = scaleBar.GetScaleBarLinePositions(scaleBarLength1, scaleBarLength2, StrokeExternal);

            // BoundingBox for scale bar
            BoundingBox envelop = new BoundingBox();

            if (points != null)
            {
                // Draw outline of lines
                for (int i = 0; i < points.Length; i += 2)
                {
                    var line = new Line();
                    line.X1 = points[i].X;
                    line.Y1 = points[i].Y;
                    line.X2 = points[i + 1].X;
                    line.Y2 = points[i + 1].Y;
                    line.Stroke = _brushScaleBarStroke;
                    line.StrokeThickness = StrokeExternal;
                    line.StrokeStartLineCap = PenLineCap.Square;
                    line.StrokeEndLineCap = PenLineCap.Square;
                    canvas.Children.Add(line);
                }

                // Draw lines
                for (int i = 0; i < points.Length; i += 2)
                {
                    var line = new Line();
                    line.X1 = points[i].X;
                    line.Y1 = points[i].Y;
                    line.X2 = points[i + 1].X;
                    line.Y2 = points[i + 1].Y;
                    line.Stroke = _brushScaleBar;
                    line.StrokeThickness = StrokeInternal;
                    line.StrokeStartLineCap = PenLineCap.Square;
                    line.StrokeEndLineCap = PenLineCap.Square;
                    canvas.Children.Add(line);
                }

                envelop = points[0].GetBoundingBox();

                for (int i = 1; i < points.Length; i++)
                {
                    envelop = envelop.Join(points[i].GetBoundingBox());
                }

                envelop = envelop.Grow(StrokeExternal * 0.5f * scaleBar.Scale);
            }

            // Draw text

            // Calc text height
            Size textSize1;
            Size textSize2;

            scaleBarText1 = scaleBarText1 ?? string.Empty;
            scaleBarText2 = scaleBarText2 ?? string.Empty;

            textBlock.Text = scaleBarText1;
            textSize1 = textBlock.MeasureText();

            textBlock.Text = scaleBarText2;
            textSize2 = textBlock.MeasureText();

            var boundingBoxText = new BoundingBox(0, 0, textSize.Width, textSize.Height);
            var boundingBoxText1 = new BoundingBox(0, 0, textSize1.Width, textSize1.Height);
            var boundingBoxText2 = new BoundingBox(0, 0, textSize2.Width, textSize2.Height);

            var (posX1, posY1, posX2, posY2) = scaleBar.GetScaleBarTextPositions(boundingBoxText, boundingBoxText1, boundingBoxText2, StrokeExternal);

            // Now draw text
            textBlock.Text = scaleBarText1;
            textBlock.Width = textSize1.Width;
            textBlock.Height = textSize1.Height;

            Canvas.SetLeft(textBlock, posX1);
            Canvas.SetTop(textBlock, posY1);

            canvas.Children.Add(textBlock);

            envelop = envelop.Join(new BoundingBox(posX1, posY1, posX1 + textSize1.Width, posY1 + textSize1.Height));

            if (scaleBar.ScaleBarMode == ScaleBarMode.Both && scaleBar.SecondaryUnitConverter != null)
            {
                textBlock = new OutlinedTextBlock();

                textBlock.Fill = _brushScaleText;
                textBlock.Stroke = _brushScaleTextStroke;
                textBlock.StrokeThickness = StrokeExternal - StrokeInternal + 1;
                textBlock.FontFamily = new FontFamily(scaleBar.Font.FontFamily);
                textBlock.FontSize = scaleBar.Font.Size;
                textBlock.FontWeight = FontWeights.Bold;

                textBlock.Text = scaleBarText2;
                textBlock.Width = textSize2.Width;
                textBlock.Height = textSize2.Height;

                Canvas.SetLeft(textBlock, posX2);
                Canvas.SetTop(textBlock, posY2);

                canvas.Children.Add(textBlock);

                envelop = envelop.Join(new BoundingBox(posX2, posY2, posX2 + textSize2.Width, posY2 + textSize2.Height));
            }

            scaleBar.Envelope = envelop;

            if (scaleBar.ShowEnvelop)
            {
                // Draw a rect around the scale bar for testing
                var rect = new Rectangle();
                rect.Width = envelop.MaxX - envelop.MinX;
                rect.Height = envelop.MaxY - envelop.MinY;
                rect.Stroke = _brushScaleTextStroke;
                rect.StrokeThickness = 1;
                Canvas.SetLeft(rect, envelop.MinX);
                Canvas.SetTop(rect, envelop.MinY);
                canvas.Children.Add(rect);
            }
        }
    }
}
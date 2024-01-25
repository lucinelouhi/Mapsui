﻿using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Color = Mapsui.Styles.Color;

namespace Mapsui.Extensions;

/// <summary>
/// Extensions for PointFeature
/// </summary>
public static class PointFeatureExtensions
{
    // Const for using to access feature fields
    public const string MarkerKey = "Marker";
    public const string MarkerSymbolKey = MarkerKey + ".Symbol";
    public const string MarkerCalloutKey = MarkerKey + ".Callout";
    public const string MarkerColorKey = MarkerKey + ".Color";
    public const string MarkerTouchedKey = MarkerKey+".Touched";
    public const string MarkerInvalidateKey = MarkerKey + ".Invalidate";

    private static readonly string markerImage;
    private static readonly double markerImageHeight;
    private static readonly Regex extractHeight = new Regex("height=\\\"(\\d+)\\\"", RegexOptions.Compiled);

    /// <summary>
    /// Read markerImage and extract height
    /// </summary>
    static PointFeatureExtensions()
    {
        // Load SVG for Marker
        using (var s = new StreamReader(EmbeddedResourceLoader.Load($"Resources.Images.Pin.svg", typeof(PointFeatureExtensions))))
        {
            markerImage = s.ReadToEnd();

            var result = extractHeight.Matches(markerImage);

            if (result.Count < 1)
                return;

            markerImageHeight = result[0].Success ? double.Parse(result[0].Groups[1].Value ?? "") : 0;
        }
    }

    /// <summary>
    /// Init a PointFeature, so that it is a marker
    /// </summary>
    /// <param name="marker">PointFeature to use</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    /// <param name="scale"></param>
    /// <param name="title"></param>
    /// <param name="subtitle"></param>
    /// <param name="touched"></param>
    public static void InitMarker(this PointFeature marker, Action invalidate, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        marker[MarkerKey] = true;

        color = color ?? Color.Red;

        var symbol = new SymbolStyle()
        {
            Enabled = true,
            SymbolType = SymbolType.Image,
            BitmapId = GetPinWithColor(color),
            SymbolOffset = new RelativeOffset(0.0, 0.5),
            SymbolScale = scale,
            Opacity = (float)opacity,
        };

        var callout = new CalloutStyle()
        {
            Enabled = false,
            Type = CalloutType.Single,
            ArrowPosition = 0.5f,
            ArrowAlignment = ArrowAlignment.Bottom,
            SymbolOffset = new Offset(0.0, markerImageHeight * scale),
            Padding = new MRect(10, 5),
            Color = Color.Black,
            BackgroundColor = Color.White,
            MaxWidth = 200,
            TitleFontColor = Color.Black,
            TitleTextAlignment = Widgets.Alignment.Center,
            SubtitleFontColor = Color.Black,
            SubtitleTextAlignment = Widgets.Alignment.Center,
        };

        callout.Title = title;
        callout.TitleFont.Size = 16;
        callout.Subtitle = subtitle;
        callout.SubtitleFont.Size = 12;
        callout.Type = String.IsNullOrEmpty(callout.Subtitle) ? CalloutType.Single : CalloutType.Detail;

        marker.Styles.Clear();
        marker.Styles.Add(symbol);
        marker.Styles.Add(callout);

        marker[MarkerSymbolKey] = symbol;
        marker[MarkerCalloutKey] = callout;
        marker[MarkerColorKey] = color;

        if (invalidate != null) marker[MarkerInvalidateKey] = invalidate;
        if (touched != null) marker[MarkerTouchedKey] = touched;
    }

    /// <summary>
    /// Check if feature is a marker
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is a marker</returns>
    public static bool IsMarker(this PointFeature feature)
    {
        return feature.Fields.Contains(MarkerKey);
    }

    /// <summary>
    /// Get color of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>Color of marker</returns>
    public static Color? GetColor(this PointFeature marker)
    {
        if (!IsMarker(marker))
            return null;

        return marker.Get<Color>(MarkerColorKey);
    }

    /// <summary>
    /// Set color for marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="color">Color to set</param>
    /// <returns>Marker</returns>
    public static PointFeature SetColor(this PointFeature marker, Color color)
    {
        if (!IsMarker(marker))
            return marker;

        SetSymbolValue(marker, (symbol) => symbol.BitmapId = GetPinWithColor(color));

        marker[MarkerColorKey] = color;

        return marker;
    }

    /// <summary>
    /// Get opacity of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>Opacity of marker</returns>
    public static double GetOpacity(this PointFeature marker)
    {
        if (!IsMarker(marker))
            return 1.0;

        var symbol = marker.Get<SymbolStyle>(MarkerSymbolKey);

        if (symbol != null)
        {
            return symbol.Opacity;
        }

        return 1.0;
    }

    /// <summary>
    /// Set opacity of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="scale">Opacity to set</param>
    /// <returns>Marker</returns>
    public static PointFeature SetOpacity(this PointFeature marker, double opacity)
    {
        SetSymbolValue(marker, (symbol) => symbol.Opacity = (float)opacity);

        return marker;
    }

    /// <summary>
    /// Get scale of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>Scale of marker</returns>
    public static double GetScale(this PointFeature marker)
    {
        if (!IsMarker(marker))
            return 1.0;

        var symbol = marker.Get<SymbolStyle>(MarkerSymbolKey);

        if (symbol != null)
        {
            return symbol.SymbolScale;
        }

        return 1.0;
    }

    /// <summary>
    /// Set scale of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="scale">Scale to set</param>
    /// <returns>Marker</returns>
    public static PointFeature SetScale(this PointFeature marker, double scale)
    {
        SetSymbolValue(marker, (symbol) => symbol.SymbolScale = scale);
        // When setting scale, also SymbolOffset of CalloutStyle has to be adjusted
        SetCalloutValue(marker, (callout) => callout.SymbolOffset = new Offset(0.0, markerImageHeight * scale));

        return marker;
    }

    /// <summary>
    /// Get title of callout for this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>Title from callout of marker</returns>
    public static string GetTitle(this PointFeature marker)
    {
        if (!IsMarker(marker))
            return string.Empty;

        var callout = marker.Get<CalloutStyle>(MarkerCalloutKey);

        if (callout != null)
            return callout.Title ?? string.Empty;

        return string.Empty;
    }

    /// <summary>
    /// Set title of callout of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="text">Title to set</param>
    /// <returns>Marker</returns>
    public static PointFeature SetTitle(this PointFeature marker, string text)
    {
        SetCalloutValue(marker, (callout) => callout.Title = text);

        return marker;
    }

    /// <summary>
    /// Get subtitle of callout for this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>Subtitle from callout of marker</returns>
    public static string GetSubtitle(this PointFeature marker)
    {
        if (!IsMarker(marker))
            return string.Empty;

        var callout = marker.Get<CalloutStyle>(MarkerCalloutKey);

        if (callout != null)
            return callout.Subtitle ?? string.Empty;

        return string.Empty;
    }

    /// <summary>
    /// Set subtitle of callout of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="text">Subtitle to set</param>
    /// <returns>Marker</returns>
    public static PointFeature SetSubtitle(this PointFeature marker, string text)
    {
        SetCalloutValue(marker, (callout) => { 
            callout.Subtitle = text; 
            callout.Type = String.IsNullOrEmpty(text) ? CalloutType.Single : CalloutType.Detail; 
        });

        return marker;
    }

    /// <summary>
    /// Show callout of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="layer">Layer this marker belongs to</param>
    /// <returns>Marker</returns>
    public static PointFeature ShowCallout(this PointFeature marker, ILayer layer)
    {
        if (layer is MemoryLayer memoryLayer)
        {
            memoryLayer.HideAllCallouts();
        }

        ChangeCalloutEnabled(marker, true);

        return marker;
    }

    /// <summary>
    /// Hide callout of this marker
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>Marker</returns>
    public static PointFeature HideCallout(this PointFeature marker)
    {
        ChangeCalloutEnabled(marker, false);

        return marker;
    }

    /// <summary>
    /// Check, if callout of this marker is visible
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <returns>True, if callout of marker is visible</returns>
    public static bool HasCallout(this PointFeature marker)
    {
        if (!IsMarker(marker))
            return false;

        var callout = marker.Get<CalloutStyle>(MarkerCalloutKey);

        if (callout != null)
            return callout.Enabled;

        return false;
    }

    /// <summary>
    /// Change the CalloutStyle Enabled flag to a new value
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="flag">True, if the callout should be visible, else false</param>
    private static void ChangeCalloutEnabled(PointFeature marker, bool flag)
    {
        SetCalloutValue(marker, (callout) => callout.Enabled = flag);

        marker.Get<Action>(MarkerInvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Set a value in SymbolStyle
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="action">Action to set value</param>
    private static void SetSymbolValue(PointFeature marker, Action<SymbolStyle> action)
    {
        if (!IsMarker(marker))
            return;

        var symbol = marker.Get<SymbolStyle>(MarkerSymbolKey);

        if (symbol != null)
            action(symbol);

        marker.Get<Action>(MarkerInvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Set a value in CalloutStyle
    /// </summary>
    /// <param name="marker">Marker to use</param>
    /// <param name="action">Action to set value</param>
    private static void SetCalloutValue(PointFeature marker, Action<CalloutStyle> action)
    {
        if (!IsMarker(marker))
            return;

        var callout = marker.Get<CalloutStyle>(MarkerCalloutKey);

        if (callout != null)
            action(callout);

        marker.Get<Action>(MarkerInvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Create a marker image with given color
    /// </summary>
    /// <param name="color">Color to use</param>
    /// <returns>BitmapId for created marker image</returns>
    private static int GetPinWithColor(Color color)
    {
        var colorInHex = $"{color.R:X2}{color.G:X2}{color.B:X2}";

        if (BitmapRegistry.Instance.TryGetBitmapId($"{MarkerKey}_{colorInHex}", out int bitmapId))
            return bitmapId;

        var svg = markerImage.Replace("#000000", $"#{colorInHex}");

        return BitmapRegistry.Instance.Register(svg, $"{MarkerKey}_{colorInHex}");
    }
}
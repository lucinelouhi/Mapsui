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
    public const string MarkerColorKey = MarkerKey + ".Color";
    public const string SymbolKey = MarkerKey + ".Symbol";
    public const string CalloutKey = MarkerKey + ".Callout";
    public const string TouchedKey = MarkerKey+".Touched";
    public const string InvalidateKey = MarkerKey + ".Invalidate";

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
    /// <param name="invalidate">Action to call when something is changed via extensions</param>
    /// <param name="color">Color for this marker</param>
    /// <param name="opacity">Opacity for this marker</param>
    /// <param name="scale">Scale for this marker</param>
    /// <param name="title">Title of callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action to call, when this marker is touched</param>
    public static void InitMarker(this PointFeature marker, Action invalidate, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        marker[MarkerKey] = true;

        color = color ?? Color.Red;

        Init(marker, invalidate, color, opacity, scale, title, subtitle, touched);

        SetSymbolValue(marker, (symbol) => symbol.SymbolType = SymbolType.Image);
        SetSymbolValue(marker, (symbol) => symbol.BitmapId = GetPinWithColor(color));
        SetSymbolValue(marker, (symbol) => symbol.SymbolOffset = new RelativeOffset(0.0, 0.5));

        SetCalloutValue(marker, (callout) => callout.SymbolOffset = new Offset(0.0, markerImageHeight * scale));

        marker[MarkerColorKey] = color;
    }

    /// <summary>
    /// Check, if feature is a marker
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is a marker</returns>
    public static bool IsMarker(this PointFeature feature)
    {
        return feature.Fields.Contains(MarkerKey) 
            && feature[SymbolKey] == feature.Styles.First() 
            && feature[CalloutKey] == feature.Styles.Skip(1).First();
    }

    /// <summary>
    /// Get color of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Color of feature</returns>
    public static Color? GetColor(this PointFeature feature)
    {
        if (IsMarker(feature))
            return feature.Get<Color>(MarkerColorKey);

        return null;
    }

    /// <summary>
    /// Set color for feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="color">Color to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetColor(this PointFeature feature, Color color)
    {
        if (IsMarker(feature))
        {
            SetSymbolValue(feature, (symbol) => symbol.BitmapId = GetPinWithColor(color));
            feature[MarkerColorKey] = color;
        }

        return feature;
    }

    /// <summary>
    /// Get opacity of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Opacity of feature</returns>
    public static double GetOpacity(this PointFeature feature)
    {
        var symbol = feature.Get<SymbolStyle>(SymbolKey);

        return symbol?.Opacity ?? 1.0;
    }

    /// <summary>
    /// Set opacity of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="scale">Opacity to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetOpacity(this PointFeature feature, double opacity)
    {
        SetSymbolValue(feature, (symbol) => symbol.Opacity = (float)opacity);

        return feature;
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

        var symbol = marker.Get<SymbolStyle>(SymbolKey);

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
    /// Get title of callout for this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Title from callout of feature</returns>
    public static string GetTitle(this PointFeature feature)
    {
        var callout = feature.Get<CalloutStyle>(CalloutKey);

        return callout?.Title ?? string.Empty;
    }

    /// <summary>
    /// Set title of callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="text">Title to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetTitle(this PointFeature feature, string text)
    {
        SetCalloutValue(feature, (callout) => callout.Title = text);

        return feature;
    }

    /// <summary>
    /// Get subtitle of callout for this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Subtitle from callout of feature</returns>
    public static string GetSubtitle(this PointFeature feature)
    {
        var callout = feature.Get<CalloutStyle>(CalloutKey);

        return callout?.Subtitle ?? string.Empty;
    }

    /// <summary>
    /// Set subtitle of callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="text">Subtitle to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetSubtitle(this PointFeature feature, string text)
    {
        SetCalloutValue(feature, (callout) => { 
            callout.Subtitle = text; 
            callout.Type = String.IsNullOrEmpty(text) ? CalloutType.Single : CalloutType.Detail; 
        });

        return feature;
    }

    /// <summary>
    /// Show callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="layer">Layer this feature belongs to</param>
    /// <returns>Feature</returns>
    public static PointFeature ShowCallout(this PointFeature feature, ILayer layer)
    {
        if (layer is MemoryLayer memoryLayer)
        {
            memoryLayer.HideAllCallouts();
        }

        ChangeCalloutEnabled(feature, true);

        return feature;
    }

    /// <summary>
    /// Hide callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Feature</returns>
    public static PointFeature HideCallout(this PointFeature feature)
    {
        ChangeCalloutEnabled(feature, false);

        return feature;
    }

    /// <summary>
    /// Check, if callout of this feature is visible
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>True, if callout of feature is visible</returns>
    public static bool HasCallout(this PointFeature feature)
    {
        var callout = feature.Get<CalloutStyle>(CalloutKey);

        return callout?.Enabled ?? false;
    }

    /// <summary>
    /// Init each type of marker/symbol created with this extensions
    /// </summary>
    /// <param name="feature">PointFeature to use</param>
    /// <param name="invalidate">Action to call when something is changed via extensions</param>
    /// <param name="color">Color for this marker</param>
    /// <param name="opacity">Opacity for this marker</param>
    /// <param name="scale">Scale for this marker</param>
    /// <param name="title">Title of callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action to call, when this marker is touched</param>
    private static void Init(this PointFeature feature, Action invalidate, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        var symbol = new SymbolStyle()
        {
            Enabled = true,
            SymbolScale = scale,
            Opacity = (float)opacity,
        };

        var callout = new CalloutStyle()
        {
            Enabled = false,
            Type = CalloutType.Single,
            ArrowPosition = 0.5f,
            ArrowAlignment = ArrowAlignment.Bottom,
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

        feature.Styles.Clear();
        feature.Styles.Add(symbol);
        feature.Styles.Add(callout);

        feature[SymbolKey] = symbol;
        feature[CalloutKey] = callout;

        if (invalidate != null) feature[InvalidateKey] = invalidate;
        if (touched != null) feature[TouchedKey] = touched;
    }

    /// <summary>
    /// Change the CalloutStyle Enabled flag to a new value
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="flag">True, if the callout should be visible, else false</param>
    private static void ChangeCalloutEnabled(PointFeature feature, bool flag)
    {
        SetCalloutValue(feature, (callout) => callout.Enabled = flag);

        feature.Get<Action>(InvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Set a value in SymbolStyle
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="action">Action to set value</param>
    private static void SetSymbolValue(PointFeature feature, Action<SymbolStyle> action)
    {
        var symbol = feature.Get<SymbolStyle>(SymbolKey);

        if (symbol != null)
            action(symbol);

        feature.Get<Action>(InvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Set a value in CalloutStyle
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="action">Action to set value</param>
    private static void SetCalloutValue(PointFeature feature, Action<CalloutStyle> action)
    {
        var callout = feature.Get<CalloutStyle>(CalloutKey);

        if (callout != null)
            action(callout);

        feature.Get<Action>(InvalidateKey)?.Invoke();
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

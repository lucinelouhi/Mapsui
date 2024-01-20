﻿using Mapsui.Extensions;
using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.Special;

public class ExtensionsSample : ISample
{
    public string Name => "Extensions";
    public string Category => "Special";

    public Task<Map> CreateMapAsync()
    {
        return Task.FromResult(CreateMap());
    }

    public static Map CreateMap()
    {
        const string markerLayerName = "Markers";

        _rand = new(1);

        var map = new Map
        {
            CRS = "EPSG:3857"
        };
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Widgets.Add(new ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top });

        using var layer = map.AddMarkerLayer(markerLayerName);

        if (layer == null)
            return map;

        // Read demo SVG
        var tiger = GetSvgFromResources("Images.Ghostscript_Tiger.svg");

        // Read demo Icon
        var icon = GetIconFromResources("Images.icon.png");

        // Add markers
        layer.AddMarker(
            SphericalMercator.FromLonLat(9.0, 48.025), 
            title: "New York", 
            subtitle: "City", 
            touched: (layer, marker, args) => {
                marker.MarkerType = marker.MarkerType >= MarkerType.Pin_9 ? 
                MarkerType.Pin_Circle : 
                marker.MarkerType + 1;
            });
        layer.AddMarker(
            SphericalMercator.FromLonLat(9.00, 48.00), 
            title: "Atlanta", 
            subtitle: "City", 
            touched: (layer, marker, args) => { 
                marker.Color = DemoColor(); 
                marker.Subtitle = $"R:{marker.Color.R} G:{marker.Color.G} B:{marker.Color.B}";
                if (layer is MemoryLayer memoryLayer) memoryLayer.HideAllCallouts();
                marker.ShowCallout();
                args.Handled = true; 
            });

        for (var i = 0; i < 10; i++)
            layer.AddMarker(SphericalMercator.FromLonLat(9.0 + i * 0.015, 48.07), type: MarkerType.Pin_0 + i, title: (MarkerType.Pin_0 + i).ToString(), color: DemoColor());

        for (var i = 0; i < 27; i++)
            layer.AddMarker(SphericalMercator.FromLonLat(9.0 + i * 0.01, 48.05), type: MarkerType.Pin_At + i, title: (MarkerType.Pin_At + i).ToString(), color: DemoColor());

        layer.AddMarker(SphericalMercator.FromLonLat(9.015, 48.025), type: MarkerType.Pin_Questionmark, title: MarkerType.Pin_Questionmark.ToString(), color: DemoColor())
            .AddMarker(SphericalMercator.FromLonLat(9.03, 48.025), type: MarkerType.Pin_Exclamationmark, title: MarkerType.Pin_Exclamationmark.ToString(), color: DemoColor())
            .AddMarker(SphericalMercator.FromLonLat(9.045, 48.025), type: MarkerType.Pin_Cross, title: MarkerType.Pin_Cross.ToString(), color: DemoColor())
            .AddMarker(SphericalMercator.FromLonLat(9.06, 48.025), type: MarkerType.Pin_Dollar, title: MarkerType.Pin_Dollar.ToString(), color: DemoColor())
            .AddMarker(SphericalMercator.FromLonLat(9.075, 48.025), type: MarkerType.Pin_Euro, title: MarkerType.Pin_Euro.ToString(), color: DemoColor());

        for (var i = 0; i < 10; i++)
            layer.AddMarker(SphericalMercator.FromLonLat(9.12 + i * 0.015, 48.0), type: MarkerType.Pin_0 + i, title: (MarkerType.Pin_0 + i).ToString(), color: DemoColor(), scale: 0.5 + _rand.NextDouble());

        // Zoom and center map
        var center = layer.Extent?.Centroid ?? new MPoint(SphericalMercator.FromLonLat(9.05, 48.05));
        var extent = layer.Extent?.Grow(2000) ?? new MRect(SphericalMercator.FromLonLat(8.95, 47.95), SphericalMercator.FromLonLat(9.15, 48.15));

        map.Navigator.CenterOn(center);
        map.Navigator.ZoomToBox(extent);

        return map;
    }

    private static string GetSvgFromResources(string name)
    {
        using var stream = typeof(ExtensionsSample).Assembly.GetManifestResourceStream(typeof(ExtensionsSample).Assembly.GetFullName(name));
        if (stream == null) return string.Empty;
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private static byte[]? GetIconFromResources(string name)
    {
        using var stream2 = EmbeddedResourceLoader.Load(name, typeof(ExtensionsSample));
        if (stream2 == null) return null;
        using var reader2 = new MemoryStream();
        stream2.CopyTo(reader2);

        return reader2.ToArray();
    }

    private static Random _rand = new(1);

    private static Color DemoColor()
    {
        return new Color(_rand.Next(128, 256), _rand.Next(128, 256), _rand.Next(128, 256));
    }
}

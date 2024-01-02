﻿using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using System.Linq;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.Demo;

public class PinSample : ISample
{
    public string Name => "6 Pin Sample";
    public string Category => "Demo";

    public Task<Map> CreateMapAsync()
    {
        return Task.FromResult(CreateMap());
    }

    public static Map CreateMap()
    {
        var map = new Map
        {
            CRS = "EPSG:3857"
        };
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(new MemoryLayer("Markers") { Style = null, });
        map.Widgets.Add(new ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top });

        var layer = (MemoryLayer)map.Layers.Where(l => l.Name.Equals("Markers")).First();

        if (layer == null)
            return map;

        layer.AddMarker(SphericalMercator.FromLonLat(9.0, 48.0))
            .AddMarker(SphericalMercator.FromLonLat(9.1, 48.1), color: Color.Green, scale: 0.75)
            .AddMarker(SphericalMercator.FromLonLat(9.0, 48.1), color: Color.Blue, scale: 0.5);

        var center = layer.Extent?.Centroid ?? new MPoint(SphericalMercator.FromLonLat(9.05, 48.05));
        var extent = layer.Extent?.Grow(2000) ?? new MRect(SphericalMercator.FromLonLat(8.95, 47.95), SphericalMercator.FromLonLat(9.15, 48.15));

        map.Navigator.CenterOn(center);
        map.Navigator.ZoomToBox(extent);

        return map;
    }
}

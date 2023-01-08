﻿using BruTile.Cache;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.DataFormats;

public class TmsSample : ISample
{
    public string Name => " 8 TMS openbasiskaart";
    public string Category => "Data Formats";
    public static IPersistentCache<byte[]>? DefaultCache { get; set; }

    public Task<Map> CreateMapAsync()
    {
        var map = new Map();
        map.Layers.Add(CreateLayer());
        return Task.FromResult(map);
    }

    public static ILayer CreateLayer()
    {
        var url = "https://www.openbasiskaart.nl/mapcache/tms/1.0.0/osm@rd";
        var tileSource = TmsTileSourceBuilder.Build(url, true, DefaultCache);

        var tileLayer = new TileLayer(tileSource)
        {
            Name = "openbasiskaart.nl"
        };

        tileLayer.Attribution.Text = "© OpenStreetMap contributors (via openbasiskaart.nl)";
        tileLayer.Attribution.Url = "https://www.openstreetmap.org/copyright";
        return tileLayer;
    }
}
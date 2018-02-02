﻿using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets.ScaleBar;

namespace Mapsui.Samples.Common.Maps
{
    public static class ScaleBarSample
    {
        public static Map CreateMap()
        {
            var map = new Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            
            // Add many different ScaleBarWidgets to Viewport of Map
            map.Widgets.Add(new ScaleBarWidget(map) { ScaleBarMode = ScaleBarMode.Both, MarginX = 10, MarginY = 10 });
            map.Widgets.Add(new ScaleBarWidget(map) { HorizontalAlignment = Widgets.HorizontalAlignment.Center, VerticalAlignment = Widgets.VerticalAlignment.Bottom, TextAlignment = Widgets.Alignment.Center });
            map.Widgets.Add(new ScaleBarWidget(map) { MaxWidth = 200, HorizontalAlignment = Widgets.HorizontalAlignment.Right, VerticalAlignment = Widgets.VerticalAlignment.Bottom, TextAlignment = Widgets.Alignment.Right, ScaleBarMode = ScaleBarMode.Both, SecondaryUnitConverter = ImperialUnitConverter.Instance });
            map.Widgets.Add(new ScaleBarWidget(map) { TextColor = Color.Red, Halo = Color.Green, HorizontalAlignment = Widgets.HorizontalAlignment.Left, VerticalAlignment = Widgets.VerticalAlignment.Center, TextAlignment = Widgets.Alignment.Right, ScaleBarMode = ScaleBarMode.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance });
            map.Widgets.Add(new ScaleBarWidget(map) { TextColor = Color.Black, Halo = Color.Gray, HorizontalAlignment = Widgets.HorizontalAlignment.Center, VerticalAlignment = Widgets.VerticalAlignment.Center, TextAlignment = Widgets.Alignment.Center, ScaleBarMode = ScaleBarMode.Both});
            map.Widgets.Add(new ScaleBarWidget(map) { Font = new Font { FontFamily = "serif", Size = 16 },  TextColor = Color.Orange, Halo = Color.Yellow, HorizontalAlignment = Widgets.HorizontalAlignment.Right, VerticalAlignment = Widgets.VerticalAlignment.Center, TextAlignment = Widgets.Alignment.Left, ScaleBarMode = ScaleBarMode.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance });
            map.Widgets.Add(new ScaleBarWidget(map) { TextColor = Color.Blue, Halo = Color.Yellow, HorizontalAlignment = Widgets.HorizontalAlignment.Left, VerticalAlignment = Widgets.VerticalAlignment.Top, TextAlignment = Widgets.Alignment.Left, ScaleBarMode = ScaleBarMode.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance });
            map.Widgets.Add(new ScaleBarWidget(map) { TextColor = Color.Cyan, Halo = Color.Yellow, HorizontalAlignment = Widgets.HorizontalAlignment.Center, VerticalAlignment = Widgets.VerticalAlignment.Top, TextAlignment = Widgets.Alignment.Right, ScaleBarMode = ScaleBarMode.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance });
            map.Widgets.Add(new ScaleBarWidget(map) { TextColor = Color.Violet, Font = null, Halo = Color.Yellow, HorizontalAlignment = Widgets.HorizontalAlignment.Right, VerticalAlignment = Widgets.VerticalAlignment.Top, TextAlignment = Widgets.Alignment.Right });
            map.Widgets.Add(new ScaleBarWidget(map) { MaxWidth = 250, ShowEnvelop = true, Font = new Font { FontFamily = "sans serif", Size = 36 }, TickLength = 15, TextColor = Color.Red, Halo = Color.Yellow, HorizontalAlignment = Widgets.HorizontalAlignment.Left, VerticalAlignment = Widgets.VerticalAlignment.Top, TextAlignment = Widgets.Alignment.Left, ScaleBarMode = ScaleBarMode.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, MarginX = 50, MarginY = 50});
            return map;
        }
    }
}
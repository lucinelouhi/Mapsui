﻿using System;
using System.Linq;
using Mapsui.Samples.Common.Maps;
using Mapsui.UI;
using Mapsui.UI.Forms;
using Xamarin.Forms;

namespace Mapsui.Samples.Forms
{
    public class PolylineSample : IFormsSample
    {
        static int markerNum = 1;
        static Random rnd = new Random();

        public string Name => "Add Polyline Sample";

        public string Category => "Forms";

        public bool OnClick(object sender, EventArgs args)
        {
            var mapView = sender as MapView;
            var e = args as MapClickedEventArgs;

            UI.Objects.Drawable f;

            lock (mapView.Drawables)
            {
                if (mapView.Drawables.Count == 0)
                {
                    f = new Polyline { StrokeWidth = 4, StrokeColor = Color.Red };
                    mapView.Drawables.Add(f);
                }
                else
                {
                    f = mapView.Drawables.First();
                }

                if (f is Polyline)
                {
                    ((Polyline)f).Positions.Add(e.Point);
                }
            }

            return true;
        }

        public void Setup(IMapControl mapControl)
        {
            mapControl.Map = OsmSample.CreateMap();
        }
    }
}

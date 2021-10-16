﻿using System.Windows;

namespace Mapsui.UI.Wpf.Extensions
{
    public static class WindowsPointExtensions
    {
        public static Geometries.Point ToMapsui(this Point point)
        {
            return new Geometries.Point(point.X, point.Y);
        }
    }
}
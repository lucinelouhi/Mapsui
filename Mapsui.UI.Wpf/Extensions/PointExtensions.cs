﻿using Mapsui.Geometries;

namespace Mapsui.UI.Wpf.Extensions
{
    public static class PointExtensions
    {
        public static Point ApplyScale(this Point point, double scale)
        {
            return new Point(point.X / scale, point.Y / scale);
        }
    }
}

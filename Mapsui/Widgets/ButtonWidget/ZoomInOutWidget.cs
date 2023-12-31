﻿using Mapsui.Styles;
using System;

namespace Mapsui.Widgets.ButtonWidget;

/// <summary>
/// Widget which shows two buttons (horizontal or vertical) with a "+" and a "-" sign.
/// With this, the user could zoom the map in and out.
/// 
/// Usage
/// To show a ZoomInOutWidget, add a instance of the ZoomInOutWidget to Map.Widgets by
/// 
///   map.Widgets.Add(new ZoomInOutWidget(map));
///   
/// Customize
/// Size: Height and Width of the buttons
/// Orientation: Orientation of the buttons. Could be Horizontal or Vertical. Vertical is default.
/// StrokeColor: Color of button frames
/// TextColor: Color of "+" and "-" signs
/// BackColor: Color of button background
/// Opacity: Opacity of buttons
/// ZoomFactor: Factor for changing Resolution. Default is 2;
/// </summary>
public class ZoomInOutWidget : Widget, ITouchableWidget
{
    /// <summary>
    /// Event handler which is called, when buttons are touched. If there
    /// isn't one, than the default handler is used, which change the Resolution
    /// of Viewport.
    /// </summary>
    public event EventHandler<WidgetTouchedEventArgs>? Touched;

    private double _size = 40;

    /// <summary>
    /// Width and height of buttons
    /// </summary>
    public double Size
    {
        get => _size;
        set
        {
            if (_size == value)
                return;
            _size = value;
            OnPropertyChanged();
        }
    }

    private Orientation _orientation = Orientation.Vertical;

    /// <summary>
    /// Orientation of buttons
    /// </summary>
    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            if (_orientation == value)
                return;
            _orientation = value;
            OnPropertyChanged();
        }
    }

    private Color _strokeColor = new(192, 192, 192);

    /// <summary>
    /// Color of button frames
    /// </summary>
    public Color StrokeColor
    {
        get => _strokeColor;
        set
        {
            if (_strokeColor == value)
                return;
            _strokeColor = value;
            OnPropertyChanged();
        }
    }

    private Color _textColor = new(192, 192, 192);

    /// <summary>
    /// Color of "+" and "-" sign
    /// </summary>
    public Color TextColor
    {
        get => _textColor;
        set
        {
            if (_textColor == value)
                return;
            _textColor = value;
            OnPropertyChanged();
        }
    }

    private Color _backColor = new(224, 224, 224);

    /// <summary>
    /// Color of background
    /// </summary>
    public Color BackColor
    {
        get => _backColor;
        set
        {
            if (_backColor == value)
                return;
            _backColor = value;
            OnPropertyChanged();
        }
    }

    private double _opacity = 0.8f;

    /// <summary>
    /// Opacity of background, frame and signs
    /// </summary>
    public double Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity == value)
                return;
            _opacity = value;
            OnPropertyChanged();
        }
    }

    public TouchableAreaType TouchableArea => TouchableAreaType.Widget;

    public bool HandleWidgetTouched(Navigator navigator, MPoint position, WidgetTouchedEventArgs args)
    {
        Touched?.Invoke(this, args);

        if (args.Handled)
            return true;

        if (Envelope == null)
            return false;

        if (Orientation == Orientation.Vertical && position.Y < Envelope.MinY + Envelope.Height * 0.5 ||
            Orientation == Orientation.Horizontal && position.X < Envelope.MinX + Envelope.Width * 0.5)
        {
            navigator.ZoomIn(500);
        }
        else
        {
            navigator.ZoomOut(500);
        }

        return true;
    }

    public bool HandleWidgetTouching(Navigator navigator, MPoint position, WidgetTouchedEventArgs args)
    {
        return false;
    }

    public bool HandleWidgetMoving(Navigator navigator, MPoint position, WidgetTouchedEventArgs args)
    {
        return false;
    }
}
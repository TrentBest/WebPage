using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Fluent builder for a visual Cartesian coordinate-system GUI node.
/// The builder describes the coordinate plane without coupling it to Blazor,
/// WPF, Unity, Revit, or another renderer.
/// </summary>
public sealed class CoordinateSystemGuiBuilder
{
    private readonly GuiBuilder _builder;
    private readonly CoordinateSystem _coordinates;

    private CoordinateSystemGuiBuilder(string id, CoordinateSystem coordinates)
    {
        _coordinates = coordinates;
        _builder = GuiBuilder.Create("coordinate-system", id)
            .Property("coordinate-space", "cartesian")
            .Property("minimum", coordinates.Minimum.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("maximum", coordinates.Maximum.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("origin-x", coordinates.OriginX.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("origin-y", coordinates.OriginY.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    public static CoordinateSystemGuiBuilder Create(string id)
        => new(id, CoordinateSystem.Normalized);

    public static CoordinateSystemGuiBuilder Create(string id, CoordinateSystem coordinates)
        => new(id, coordinates);

    public CoordinateSystemGuiBuilder Text(string text)
    {
        _builder.Text(text);
        return this;
    }

    public CoordinateSystemGuiBuilder Axes(bool visible = true)
    {
        _builder.Property("axes", visible ? "visible" : "hidden");
        return this;
    }

    public CoordinateSystemGuiBuilder Grid(bool visible = true)
    {
        _builder.Property("grid", visible ? "visible" : "hidden");
        return this;
    }

    public CoordinateSystemGuiBuilder Origin(bool visible = true)
    {
        _builder.Property("origin", visible ? "visible" : "hidden");
        return this;
    }

    public CoordinateSystemGuiBuilder Bounds(bool visible = true)
    {
        _builder.Property("bounds", visible ? "visible" : "hidden");
        return this;
    }

    public CoordinateSystemGuiBuilder Point(string id, double x, double y)
    {
        var panel = _coordinates.ToPanel(x, y);
        _builder.Child("coordinate-point", id)
            .Property("x", panel.X.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("y", panel.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        return this;
    }

    public GuiNode Build() => _builder.Build();
}

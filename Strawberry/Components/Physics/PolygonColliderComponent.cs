using Strawberry.Math;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Components.Physics;

/// <summary>
/// A collider with an arbitrary polygon shape
/// </summary>
public class PolygonColliderComponent : ColliderComponent
{
    Fixture polygon;
    Vertices meterVertices;

    /// <summary>
    /// The underlying fixture of the collider
    /// </summary>
    public override Fixture Fixture => polygon;

    Vector2[] pixelVertices;
    Vector2 offset;

    /// <summary>
    /// The offset of the collider relative to the body's position, in pixels.
    /// </summary>
    public Vector2 Offset
    {
        get => offset;
        set
        {
            offset = value;
            if (polygon != null)
                RecreateFixture();
        }
    }

    /// <summary>
    /// Sets the vertices of the polygon collider in pixels.
    /// </summary>
    /// <param name="positions">The vertex positions in pixels.</param>
    public void SetVertices(Vector2[] positions)
    {
        pixelVertices = positions;
        if (polygon != null)
            RecreateFixture();
    }

    void ComputeMeterVertices()
    {
        if (pixelVertices == null || pixelVertices.Length == 0)
            return;
        var ppm = Scene.PixelPerMeter;
        meterVertices = new Vertices(
            pixelVertices.Select(v => v / ppm + Offset)
        );
    }

    void RecreateFixture()
    {
        if (Fixture?.Body == null)
            return;
        ComputeMeterVertices();
        if (meterVertices == null)
            return;

        var body = Fixture.Body;
        var oldFixture = polygon;
        polygon = body.CreatePolygon(meterVertices, Density);
        FixtureRecreated(oldFixture, polygon);
        body.Remove(oldFixture);
    }

    protected override void CreateFixture(Body body)
    {
        ComputeMeterVertices();
        if (meterVertices == null)
            return;
        polygon = body.CreatePolygon(meterVertices, Density);
    }
}
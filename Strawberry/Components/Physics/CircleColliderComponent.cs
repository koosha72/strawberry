using Strawberry.Common;
using Strawberry.Math;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Components.Physics;

/// <summary>
/// A collider with a rectangular shape
/// </summary>
public class CircleColliderComponent : ColliderComponent
{
    Fixture circle;
    /// <summary>
    /// The underlying fixture of the collider
    /// </summary>
    public override Fixture Fixture { get => circle; }

    private float radius;

    /// <summary>
    /// The radius of the collider in pixels
    /// </summary>
    public float Radius
    {
        get { return radius; }
        set
        {
            radius = value;
            if (circle != null)
            {
                var body = Fixture.Body;
                var oldFixture = circle;
                CreateFixture(body);
                FixtureRecreated(oldFixture, circle);
                body.Remove(oldFixture);
            }
        }
    }

    private Vector2 offset;

    /// <summary>
    /// The offset of the collider relative to size.
    /// </summary>
    public Vector2 Offset
    {
        get { return offset; }
        set
        {
            offset = value;
            if (circle != null)
            {
                var body = Fixture.Body;
                var oldFixture = circle;
                CreateFixture(body);
                FixtureRecreated(oldFixture, circle);
                body.Remove(oldFixture);
            }
        }
    }


    protected override void CreateFixture(Body body)
    {
        var meterSize = Radius / Scene.PixelPerMeter;
        circle = body.CreateCircle(meterSize, Density, Offset / Scene.PixelPerMeter);
    }
}
using Strawberry.Common;
using Strawberry.Math;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Components.Physics;

/// <summary>
/// A collider with a rectangular shape
/// </summary>
public class BoxColliderComponent : ColliderComponent
{
    Fixture box;
    /// <summary>
    /// The underlying fixture of the collider
    /// </summary>
    public override Fixture Fixture { get => box; }

    private Vector2 size;

    /// <summary>
    /// The size of the collider in pixels
    /// </summary>
    public Vector2 Size
    {
        get { return size; }
        set
        {
            size = value;
            if (box != null)
            {
                var body = Fixture.Body;
                var oldFixture = box;
                CreateFixture(body);
                FixtureRecreated(oldFixture, box);
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
            if (box != null)
            {
                var body = Fixture.Body;
                var oldFixture = box;
                CreateFixture(body);
                FixtureRecreated(oldFixture, box);
                body.Remove(oldFixture);
            }
        }
    }


    protected override void CreateFixture(Body body)
    {
        var meterSize = Size / Scene.PixelPerMeter;
        box = body.CreateRectangle(meterSize.X, meterSize.Y, Density, Offset / Scene.PixelPerMeter);
    }
}
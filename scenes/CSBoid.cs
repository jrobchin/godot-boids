using Godot;
using static Godot.GD;

public class CSBoid : Position2D
{
    [Export]
    private float cohesionStrength = 0.01F;

    [Export]
    private float separationStrength = 1.0F;

    [Export]
    private float separationDistance = 100F;

    [Export]
    private float alignmentStrength = 0.1F;

    [Export]
    private float alignmentDistance = 50F;

    [Export]
    private float borderStrength = 350F;

    [Export]
    private float impulse = 0.5F;

    [Export]
    private float maxSpeed = 10F;

    [Export]
    private float influenceStrength = 10F;

    [Export]
    private int maxNeighbours = 20;


    public Vector2 Velocity = Vector2.Zero;

    private Godot.Collections.Dictionary<string, float> limits;
    private Godot.Collections.Dictionary<ulong, CSBoid> neighbours = new Godot.Collections.Dictionary<ulong, CSBoid>();
    private Vector2 influence;
    private Vector2 velC;
    private Vector2 velS;
    private Vector2 velA;
    private Vector2 velB;

    public override void _Ready()
    {
        GetNode<Area2D>("NeighbourArea").Connect("area_entered", this, "onNeighbourAreaAreaEntered");
        GetNode<Area2D>("NeighbourArea").Connect("area_exited", this, "onNeighbourAreaAreaExited");
    }

    public override void _Process(float delta)
    {
        move(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        updateInfluence();
    }

    public void Initialize(Vector2 position, Vector2 velocity, Godot.Collections.Dictionary<string, float> limits)
    {
        this.Position = position;
        this.Velocity = velocity;
        this.limits = limits;
    }

    private void move(float delta)
    {
        Velocity = Velocity.LinearInterpolate(Velocity + influence * delta, impulse);
        Velocity.Clamped(maxSpeed);

        Rotation = Velocity.Angle() + Mathf.Pi / 2;

        Position = Position + Velocity * delta;
    }

    private void updateInfluence()
    {
        influence = calculateInfluence() * influenceStrength;
    }

    private Vector2 calculateInfluence()
    {
        velC = Vector2.Zero;
        velS = Vector2.Zero;
        velA = Vector2.Zero;
        velB = Vector2.Zero;
        Vector2 centerOfMass = Vector2.Zero;
        Vector2 avgVelocity = Vector2.Zero;
        int nBoidsAlign = 0;

        int nNeighbours = 0;
        foreach (CSBoid boid in neighbours.Values)
        {

            centerOfMass += boid.Position;

            Vector2 vecTo = boid.Position - Position;

            if (vecTo.Length() < alignmentDistance)
            {
                nBoidsAlign++;
                avgVelocity += boid.Velocity;
            }

            if (vecTo.Length() < separationDistance)
            {
                velS -= vecTo * separationDistance / vecTo.Length();
            }

            nNeighbours++;
            if (nNeighbours >= maxNeighbours) break;
        }

        if (nNeighbours > 0)
        {
            centerOfMass /= nNeighbours;
            velC = centerOfMass - Position;
        }

        if (nBoidsAlign > 0)
        {
            avgVelocity /= nBoidsAlign;
            velA = avgVelocity - Velocity;
        }

        velC *= cohesionStrength;
        velA *= alignmentStrength;
        velS *= separationStrength;

        velB = Vector2.Zero;
        if (Position.x < limits["x_min"])
        {
            velB += Vector2.Right * borderStrength;
        }
        else if (Position.x > limits["x_max"])
        {
            velB += Vector2.Left * borderStrength;
        }

        if (Position.y < limits["y_min"])
        {
            velB += Vector2.Down * borderStrength;
        }
        else if (Position.y > limits["y_max"])
        {
            velB += Vector2.Up * borderStrength;
        }

        Vector2 influenceVelocity = velC + velA + velS + velB;

        return influenceVelocity;
    }

    private void onNeighbourAreaAreaEntered(Area area)
    {
        if (area.IsInGroup("boid_colliders"))
        {
            CSBoid boid = area.GetParent<CSBoid>();
            if (boid == this)
            {
                return;
            }

            neighbours[boid.GetInstanceId()] = boid;
        }
    }

    private void onNeighbourAreaAreaExited(Area area)
    {
        if (area.IsInGroup("boid_colliders"))
        {
            CSBoid boid = area.GetParent<CSBoid>();
            neighbours.Remove(boid.GetInstanceId());
        }
    }
}

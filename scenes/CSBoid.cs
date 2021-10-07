using Godot;
using static Godot.GD;

public class CSBoid : Position2D
{
    [Export]
    private float cohesionStrength = 0.1F;

    [Export]
    private float separationStrength = 0.5F;

    [Export]
    private float separationDistance = 25F;

    [Export]
    private float alignmentStrength = 0.1F;

    [Export]
    private float alignmentDistance = 25F;

    [Export]
    private float borderStrength = 25F;

    [Export]
    private float targetStrength = 25f;

    [Export]
    private float impulse = 0.9F;

    [Export]
    private float maxSpeed = 100F;

    [Export]
    private float influenceStrength = 5F;

    [Export]
    private int maxNeighbours = -1;

    [Export]
    public bool IsEnemy = false;

    [Export]
    private float enemyStrength = 25f;


    public Vector2 Velocity = Vector2.Zero;
    public Node2D Target;

    private Limits limits;
    private Godot.Collections.Dictionary<ulong, CSBoid> neighbours = new Godot.Collections.Dictionary<ulong, CSBoid>();
    private Vector2 influence;
    private Vector2 velC;
    private Vector2 velS;
    private Vector2 velA;
    private Vector2 velB;
    private Vector2 velT;
    private Vector2 velE;

    public override void _Ready()
    {
        GetNode<Area2D>("NeighbourArea").Connect("area_entered", this, "onNeighbourAreaAreaEntered");
        GetNode<Area2D>("NeighbourArea").Connect("area_exited", this, "onNeighbourAreaAreaExited");

        if (IsEnemy)
        {
            GetNode<Polygon2D>("Polygon2D").Color = new Color(1, 0, 0);
            ((CircleShape2D)GetNode<CollisionShape2D>("NeighbourArea/CollisionShape2D").Shape).Radius *= 2;
        }
    }

    public override void _Process(float delta)
    {
        move(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        updateInfluence();
    }

    public void Initialize(Vector2 position, Vector2 velocity, Limits limits)
    {
        this.Position = position;
        this.Velocity = velocity;
        this.limits = limits;
    }

    private void move(float delta)
    {
        Velocity = Velocity.LinearInterpolate(Velocity + influence * delta, impulse);
        Velocity = Velocity.Clamped(maxSpeed);

        Rotation = Velocity.Angle() + Mathf.Pi / 2;

        Position = Position + Velocity * delta;
    }

    private void updateInfluence()
    {
        influence = calculateInfluence() * influenceStrength;
    }

    private Vector2 calculateInfluence()
    {
        Vector2 influenceVelocity = Vector2.Zero;

        velC = Vector2.Zero;
        velS = Vector2.Zero;
        velA = Vector2.Zero;
        velB = Vector2.Zero;
        velT = Vector2.Zero;
        velE = Vector2.Zero;

        if (Position.x < limits.XMin)
        {
            velB += Vector2.Right * borderStrength;
        }
        else if (Position.x > limits.XMax)
        {
            velB += Vector2.Left * borderStrength;
        }

        if (Position.y < limits.YMin)
        {
            velB += Vector2.Down * borderStrength;
        }
        else if (Position.y > limits.YMax)
        {
            velB += Vector2.Up * borderStrength;
        }

        influenceVelocity += velB;

        Vector2 centerOfMass = Vector2.Zero;
        Vector2 avgVelocity = Vector2.Zero;
        int nBoidsAlign = 0;
        int nNeighbours = 0;
        foreach (CSBoid boid in neighbours.Values)
        {

            centerOfMass += boid.Position;

            Vector2 vecTo = boid.Position - Position;

            if (boid.IsEnemy)
            {
                velE = -vecTo.Normalized() * enemyStrength / vecTo.Length();
            }
            else
            {
                if (vecTo.Length() < alignmentDistance)
                {
                    nBoidsAlign++;
                    avgVelocity += boid.Velocity;
                }

                if (vecTo.Length() < separationDistance)
                {
                    velS -= vecTo * separationDistance / vecTo.Length();
                }
            }

            nNeighbours++;
            if (maxNeighbours != -1 && nNeighbours >= maxNeighbours) break;
        }

        if (nNeighbours > 0)
        {
            centerOfMass /= nNeighbours;
            velC = centerOfMass - Position;
            influenceVelocity += velC;
        }

        if (!IsEnemy)
        {

            if (nBoidsAlign > 0)
            {
                avgVelocity /= nBoidsAlign;
                velA = avgVelocity - Velocity;
            }

            velC *= cohesionStrength;
            velA *= alignmentStrength;
            velS *= separationStrength;

            if (Target != null)
            {
                velT = Position.DirectionTo(Target.Position) * targetStrength;
                influenceVelocity += velT;
            }

            influenceVelocity += velA + velS + velE;
        }

        return influenceVelocity;
    }

    protected virtual void onNeighbourAreaAreaEntered(Area area)
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

    protected virtual void onNeighbourAreaAreaExited(Area area)
    {
        if (area.IsInGroup("boid_colliders"))
        {
            CSBoid boid = area.GetParent<CSBoid>();
            neighbours.Remove(boid.GetInstanceId());
        }
    }
}

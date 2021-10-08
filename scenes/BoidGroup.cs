using System;
using Godot;

public class BoidGroup : Node2D
{
    [Export]
    private float initialVelocity = 10F;

    [Export]
    private int numBoids = 100;

    [Export]
    private bool randomBoids = false;

    [Export]
    private int numCSBoids = 150;

    [Export]
    private int numEnemyBoids = 3;

    private readonly Random rand = new Random();

    private Vector2 viewportSize;

    private Limits limits;

    private Node2D target;

    private Godot.Collections.Array<CSBoid>
        boids = new Godot.Collections.Array<CSBoid>();

    private readonly Godot.Collections.Dictionary<string, PackedScene>
        boidScenes =
            new Godot.Collections.Dictionary<string, PackedScene> {
                {
                    "CSBoid",
                    ResourceLoader
                        .Load<PackedScene>("res://scenes/CSBoid.tscn")
                },
            };

    public override void _Ready()
    {
        viewportSize = GetViewportRect().Size;
        limits = new Limits(0, (int)viewportSize.x, 0, (int)viewportSize.y);

        target = GetNode<Node2D>("../Target");

        if (randomBoids)
        {
            spawnRandomBoids(numBoids);
        }
        else
        {
            for (int i = 0; i < numCSBoids; i++) spawnRandomBoid(boidScenes["CSBoid"]);
            for (int i = 0; i < numEnemyBoids; i++) spawnRandomBoid(boidScenes["NoteBoid"], true);
        }
    }

    private void spawnRandomBoids(int n)
    {
        string[] boidTypes = new string[boidScenes.Keys.Count];
        boidScenes.Keys.CopyTo(boidTypes, 0);

        for (int i = 0; i < numBoids; i++)
        {
            string boidType = boidTypes[rand.Next(boidTypes.Length)];
            spawnRandomBoid(boidScenes[boidType]);
        }
    }

    private void spawnRandomBoid(PackedScene boidScene, bool isEnemy = false)
    {
        spawnBoid(boidScene,
        new Vector2(rand.Next(limits.XMin, limits.XMax),
            rand.Next(limits.YMin, limits.YMax)),
        new Vector2(0, initialVelocity)
            .Rotated((float)rand.NextDouble() * Mathf.Pi * 2),
        isEnemy);
    }

    private void spawnBoid(
        PackedScene boidScene,
        Vector2 position,
        Vector2 initialVelocity,
        bool isEnemy = false
    )
    {
        CSBoid boidInstance = boidScene.Instance<CSBoid>();
        boidInstance.Initialize(position, initialVelocity, limits);

        if (isEnemy)
        {
            boidInstance.IsEnemy = true;
        }
        else
        {
            boidInstance.Target = target;
        }

        AddChild(boidInstance);
        boids.Add(boidInstance);
    }
}

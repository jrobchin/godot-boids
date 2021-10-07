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
    private int numTriggerBoids = 10;

    [Export]
    private int numNoteBoids = 10;

    private readonly Random rand = new Random();

    private Vector2 viewportSize;

    private Limits limits;

    private Godot.Collections.Array<CSBoid>
        boids = new Godot.Collections.Array<CSBoid>();

    private readonly Godot.Collections.Dictionary<string, PackedScene>
        boidScenes =
            new Godot.Collections.Dictionary<string, PackedScene> {
                {
                    "TriggerBoid",
                    ResourceLoader
                        .Load<PackedScene>("res://scenes/TriggerBoid.tscn")
                },
                {
                    "NoteBoid",
                    ResourceLoader
                        .Load<PackedScene>("res://scenes/NoteBoid.tscn")
                }
            };

    public override void _Ready()
    {
        viewportSize = GetViewportRect().Size;
        limits = new Limits(0, (int)viewportSize.x, 0, (int)viewportSize.y);

        if (randomBoids)
        {
            spawnRandomBoids(numBoids);
        }
        else
        {
            for (int i = 0; i < numTriggerBoids; i++) spawnRandomBoid(boidScenes["TriggerBoid"]);
            for (int i = 0; i < numNoteBoids; i++) spawnRandomBoid(boidScenes["NoteBoid"]);
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

    private void spawnRandomBoid(PackedScene boidScene)
    {
        spawnBoid(boidScene,
        new Vector2(rand.Next(limits.XMin, limits.XMax),
            rand.Next(limits.YMin, limits.YMax)),
        new Vector2(0, initialVelocity)
            .Rotated((float)rand.NextDouble() * Mathf.Pi * 2));
    }

    private void spawnBoid(
        PackedScene boidScene,
        Vector2 position,
        Vector2 initialVelocity
    )
    {
        CSBoid boidInstance = boidScene.Instance<CSBoid>();
        boidInstance.Initialize(position, initialVelocity, limits);

        AddChild(boidInstance);
        boids.Add(boidInstance);
    }
}

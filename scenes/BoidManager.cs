using System;
using Godot;

public class BoidManager : Node2D
{
    [Export]
    private float initialVelocity = 10F;

    [Export]
    private int numCSBoids = 150;

    [Export]
    private int numEnemyBoids = 3;

    private readonly Random rand = new Random();

    private Vector2 viewportSize;

    private Limits limits;

    private readonly PackedScene boidScene = ResourceLoader.Load<PackedScene>("res://scenes/CSBoid.tscn");

    public override void _Ready()
    {
        viewportSize = GetViewportRect().Size;
        limits = new Limits(0, (int)viewportSize.x, 0, (int)viewportSize.y);

        for (int i = 0; i < numCSBoids; i++)
            spawnRandomBoid();
        for (int i = 0; i < numEnemyBoids; i++)
            spawnRandomBoid(true);
    }

    private void spawnRandomBoid(bool isEnemy = false)
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

        AddChild(boidInstance);
    }
}

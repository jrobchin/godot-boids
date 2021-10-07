using Godot;
using System;

public class TriggerBoid : CSBoid
{
    protected override void onNeighbourAreaAreaEntered(Area area)
    {
        base.onNeighbourAreaAreaEntered(area);

        if (area.IsInGroup("boid_colliders"))
        {
            CSBoid boid = area.GetParent<CSBoid>();
            if (boid == this) return;

            Type boidType = boid.GetType();

            if (boidType == typeof(NoteBoid))
            {
                ((NoteBoid)boid).PlayNote();
            }

        }
    }
}

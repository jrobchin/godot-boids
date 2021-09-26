extends Position2D

class_name Boid

export var cohesion_strength := 1.0
export var separation_strength := 1.0
export var alignment_strength := 1.0
export var vision_distance := 200

var velocity: Vector2


func move(delta: float, boids: Array):
	velocity = velocity + cohesion(boids)
	position = position + velocity * delta


func cohesion(boids: Array) -> Vector2:
	var center_of_mass := Vector2.ZERO
	for boid in boids:
		if boid == self:
			continue
		center_of_mass += boid.position
	center_of_mass /= boids.size()

	var vel_c := center_of_mass - position
	return vel_c * cohesion_strength


func separation(boids: Array) -> Vector2:
	return Vector2.ZERO


func alignment(boids: Array) -> Vector2:
	return Vector2.ZERO


func update_velocity(boids: Array) -> Vector2:
	return Vector2.ZERO


func update_rotation(boids: Array) -> Vector2:
	return Vector2.ZERO

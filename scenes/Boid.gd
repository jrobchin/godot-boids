extends Position2D

class_name Boid

var velocity: Vector2


func move(delta: float, boids: Array):
	# velocity = update_velocity(boids) * delta
	position = position + velocity * delta


func cohesion(boids: Array) -> Vector2:
	return Vector2.ZERO


func separation(boids: Array) -> Vector2:
	return Vector2.ZERO


func alignment(boids: Array) -> Vector2:
	return Vector2.ZERO


func update_velocity(boids: Array) -> Vector2:
	return Vector2.ZERO


func update_rotation(boids: Array) -> Vector2:
	return Vector2.ZERO

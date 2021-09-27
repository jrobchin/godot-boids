extends Position2D

class_name Boid

export var cohesion_strength := 0.01
export var separation_strength := 1.0
export var separation_distance := 100
export var alignment_strength := 0.1
export var alignment_distance := 50
export var border_strength := 350
export var impulse := 0.5
export var max_speed := 10

var draw := false
var velocity: Vector2
var vel_s := Vector2.ZERO
var vel_c := Vector2.ZERO
var vel_a := Vector2.ZERO
var vel_b := Vector2.ZERO

# func _draw():
# 	if draw:
# 		for pos in sb_pos:
# 			draw_line(Vector2.ZERO, to_local(pos), Color.green, 1, true)

# 		draw_line(Vector2.ZERO, vel_s, Color.purple, 1, true)
# 		draw_line(Vector2.ZERO, vel_c, Color.orange, 1, true)
# 		draw_line(Vector2.ZERO, vel_a, Color.white, 1, true)
# 		draw_line(Vector2.ZERO, vel_b, Color.black, 1, true)


func _process(_delta):
	update()


func move(delta: float, boids: Array, limits: Dictionary):
	var _influence = influence(boids, limits)
	velocity = lerp(velocity, velocity + _influence * delta, impulse)
	if velocity.length() > max_speed:
		velocity = velocity.normalized() * max_speed
	$Sprite.rotation = velocity.angle() + PI / 2
	position = position + velocity * delta


func influence(boids: Array, limits: Dictionary) -> Vector2:
	var center_of_mass := Vector2.ZERO
	var avg_velocity := Vector2.ZERO
	vel_s = Vector2.ZERO
	vel_a = Vector2.ZERO

	var n_boids_align := 0

	for boid in boids:
		if boid == self:
			continue

		center_of_mass += boid.position

		var vec_to = boid.position - position

		if vec_to.length() < alignment_distance:
			n_boids_align += 1
			avg_velocity += boid.velocity

		if vec_to.length() < separation_distance:
			vel_s -= vec_to * separation_distance / vec_to.length()

	var n_boids := boids.size() - 1
	center_of_mass /= n_boids

	vel_c = center_of_mass - position
	vel_c *= cohesion_strength

	if n_boids_align > 0:
		avg_velocity /= n_boids_align
		vel_a = avg_velocity - velocity

	vel_a *= alignment_strength
	vel_s *= separation_strength

	vel_b = Vector2.ZERO
	if position.x < limits["x_min"]:
		vel_b += Vector2.RIGHT * border_strength
	elif position.x > limits["x_max"]:
		vel_b += Vector2.LEFT * border_strength

	if position.y < limits["y_min"]:
		vel_b += Vector2.DOWN * border_strength
	elif position.y > limits["y_max"]:
		vel_b += Vector2.UP * border_strength

	return vel_c + vel_a + vel_s + vel_b

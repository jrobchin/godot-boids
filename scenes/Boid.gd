extends Position2D

class_name Boid

export var cohesion_strength := 0.01
export var separation_strength := 1.0
export var separation_distance := 100
export var alignment_strength := 0.1
export var vision_distance := 200
export var max_speed := 10

var draw := false
var velocity: Vector2
var vel_s := Vector2.ZERO
var vel_c := Vector2.ZERO
var vel_a := Vector2.ZERO
var vel_b := Vector2.ZERO

var sb_pos = []


func _draw():
	if draw:
		for pos in sb_pos:
			draw_line(Vector2.ZERO, to_local(pos), Color.green, 1, true)

		draw_line(Vector2.ZERO, vel_s, Color.purple, 1, true)
		draw_line(Vector2.ZERO, vel_c, Color.orange, 1, true)
		draw_line(Vector2.ZERO, vel_a, Color.white, 1, true)
		draw_line(Vector2.ZERO, vel_b, Color.black, 1, true)


func _process(_delta):
	update()


func move(delta: float, boids: Array, limits: Dictionary):
	var _influence = influence(boids, limits)
	velocity = lerp(velocity, velocity + _influence * delta, 0.5)
	if velocity.length() > max_speed:
		velocity = velocity.normalized() * max_speed
	$Sprite.rotation = velocity.angle() + PI / 2
	position = position + velocity * delta


func influence(boids: Array, limits: Dictionary) -> Vector2:
	var center_of_mass := Vector2.ZERO
	var avg_velocity := Vector2.ZERO
	vel_s = Vector2.ZERO
	sb_pos = []

	for boid in boids:
		if boid == self:
			continue

		center_of_mass += boid.position
		avg_velocity += boid.velocity

		var vec_to = boid.position - position
		if vec_to.length() < separation_distance:
			sb_pos.append(boid.position)
			vel_s -= vec_to * separation_distance / vec_to.length()

	var n_boids := boids.size() - 1
	center_of_mass /= n_boids
	avg_velocity /= n_boids

	vel_c = center_of_mass - position
	vel_c *= cohesion_strength
	vel_a = avg_velocity - velocity
	vel_a *= alignment_strength
	vel_s *= separation_strength

	vel_b = Vector2.ZERO
	var b_str = 1000
	if position.x < limits["x_min"]:
		vel_b += Vector2.RIGHT * b_str
	elif position.x > limits["x_max"]:
		vel_b += Vector2.LEFT * b_str

	if position.y < limits["y_min"]:
		vel_b += Vector2.DOWN * b_str
	elif position.y > limits["y_max"]:
		vel_b += Vector2.UP * b_str

	return vel_c + vel_a + vel_s + vel_b

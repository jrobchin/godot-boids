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

var velocity := Vector2.ZERO
var limits: Dictionary

var _positions := []
var _neighbours := []
var _vel_c := Vector2.ZERO
var _vel_s := Vector2.ZERO
var _vel_a := Vector2.ZERO
var _vel_b := Vector2.ZERO


func _draw():
	draw_line(Vector2.ZERO, _vel_c, Color.red)
	draw_line(Vector2.ZERO, _vel_s, Color.green)
	draw_line(Vector2.ZERO, _vel_a, Color.blue)
	draw_line(Vector2.ZERO, _vel_b, Color.yellow)
	for i in range(_positions.size() - 1):
		draw_line(to_local(_positions[i]), to_local(_positions[i + 1]), Color.cyan, 1, true)


func _process(delta):
	move(delta)
	_positions.append(position)
	if _positions.size() > 250:
		_positions.pop_front()
	update()


func move(delta: float):
	var _influence = influence()
	velocity = lerp(velocity, velocity + _influence * delta, impulse)
	if velocity.length() > max_speed:
		velocity = velocity.normalized() * max_speed

	$Sprite.rotation = velocity.angle() + PI / 2

	position = position + velocity * delta


func influence() -> Vector2:
	_vel_c = Vector2.ZERO
	_vel_s = Vector2.ZERO
	_vel_a = Vector2.ZERO
	_vel_b = Vector2.ZERO

	var center_of_mass := Vector2.ZERO
	var avg_velocity := Vector2.ZERO
	var n_boids_align := 0

	for boid in _neighbours:
		if boid == self:
			continue

		center_of_mass += boid.position

		var vec_to = boid.position - position

		if vec_to.length() < alignment_distance:
			n_boids_align += 1
			avg_velocity += boid.velocity

		if vec_to.length() < separation_distance:
			_vel_s -= vec_to * separation_distance / vec_to.length()

	if _neighbours.size() > 1:
		center_of_mass /= _neighbours.size() - 1
		_vel_c = center_of_mass - position

	if n_boids_align > 0:
		avg_velocity /= n_boids_align
		_vel_a = avg_velocity - velocity

	_vel_c *= cohesion_strength
	_vel_a *= alignment_strength
	_vel_s *= separation_strength

	_vel_b = Vector2.ZERO
	if position.x < limits.get("x_min", -INF):
		_vel_b += Vector2.RIGHT * border_strength
	elif position.x > limits.get("x_max", INF):
		_vel_b += Vector2.LEFT * border_strength

	if position.y < limits.get("y_min", -INF):
		_vel_b += Vector2.DOWN * border_strength
	elif position.y > limits.get("y_max", INF):
		_vel_b += Vector2.UP * border_strength

	var influence_vel = _vel_c + _vel_a + _vel_s + _vel_b
	if is_nan(influence_vel.x) || is_nan(influence_vel.y):
		return Vector2.ZERO
	return influence_vel


func _on_NeighbourArea_area_entered(area):
	if area.is_in_group("boid_colliders"):
		_neighbours.append(area.get_parent())


func _on_NeighbourArea_area_exited(area):
	if area.is_in_group("boid_colliders"):
		_neighbours.remove(_neighbours.find(area.get_parent()))

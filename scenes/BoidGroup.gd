extends Node2D

export var initial_velocity: int = 10
export var initial_boids: int = 20
export var boid_scene: PackedScene

onready var viewport_size := get_viewport_rect().size
onready var limits := {"x_min": 0, "x_max": viewport_size.x, "y_min": 0, "y_max": viewport_size.y}

var rng := RandomNumberGenerator.new()
var influence_thread: Thread
var boids := []


func _ready():
	rng.randomize()

	spawn_random_boids(
		initial_boids, limits["x_min"], limits["x_max"], limits["y_min"], limits["y_max"]
	)


func spawn_random_boids(n: int, x_min: int, x_max: int, y_min: int, y_max: int):
	for _i in range(n):
		spawn_boid(rng.randi_range(x_min, x_max), rng.randi_range(y_min, y_max))


func spawn_boid(x: int, y: int):
	var _position := Vector2(x, y)
	var _velocity := Vector2.UP.rotated(rng.randf_range(0, 2 * PI)) * initial_velocity
	var _limits = limits

	var boid_instance := boid_scene.instance()
	boid_instance.Initialize(_position, _velocity, _limits)

	add_child(boid_instance)
	boids.append(boid_instance)

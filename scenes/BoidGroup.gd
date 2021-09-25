extends Node2D

var rng := RandomNumberGenerator.new()
var boid_scene = preload("res://scenes/Boid.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	rng.randomize()


func _input(event):
	if event.is_action_pressed("debug_1"):
		spawn_boid(rng.randi_range(0, 1024), rng.randi_range(0, 600))


func spawn_boid(x: int, y: int):
	var boid_instance := boid_scene.instance() as Boid
	boid_instance.global_position = Vector2(x, y)
	add_child(boid_instance)

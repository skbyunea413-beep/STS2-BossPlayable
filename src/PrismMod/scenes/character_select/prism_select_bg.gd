extends Control

const STAR_GLOW_PATH := "res://PrismMod/images/character_select/star_glow.png"
const AMBIENT_PRISM_PATHS := [
	"res://PrismMod/images/character_select/ambient_prism_1.png",
	"res://PrismMod/images/character_select/ambient_prism_2.png",
	"res://PrismMod/images/character_select/ambient_prism_3.png",
	"res://PrismMod/images/character_select/ambient_prism_4.png",
	"res://PrismMod/images/character_select/ambient_prism_5.png",
	"res://PrismMod/images/character_select/ambient_prism_6.png",
	"res://PrismMod/images/character_select/ambient_prism_7.png",
]

@onready var background: TextureRect = $Background
@onready var parts: Node2D = $Parts
@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var body: Sprite2D = $Parts/Body
@onready var core: Sprite2D = $Parts/Core
@onready var shard_top: Sprite2D = $Parts/ShardTop
@onready var shard_left: Sprite2D = $Parts/ShardRight
@onready var shard_bottom: Sprite2D = $Parts/ShardBottom

var _rng := RandomNumberGenerator.new()
var _base_parts_position := Vector2.ZERO
var _body_origin := Vector2.ZERO
var _body_base_scale := Vector2.ONE
var _core_origin := Vector2.ZERO
var _core_base_scale := Vector2.ONE
var _core_target := Vector2.ZERO
var _core_target_offset := Vector2.ZERO
var _core_hold := 0.0
var _core_move := 0.0
var _shake_time := 0.0
var _shards: Array[Dictionary] = []
var _ambient_prisms: Array[Dictionary] = []
var _stars: Array[Dictionary] = []

func _ready() -> void:
	_rng.randomize()
	add_to_group("prism_select_splash")
	process_mode = Node.PROCESS_MODE_ALWAYS
	set_process(true)
	animation_player.stop()
	_setup_background_motion()
	_create_ambient_layers()
	parts.position += Vector2(280.0, 0.0)
	shard_top.z_index = 5
	shard_left.z_index = 5
	shard_bottom.z_index = 5
	body.z_index = 10
	core.z_index = 20
	_base_parts_position = parts.position
	_body_origin = body.position
	_body_base_scale = body.scale
	_core_origin = core.position
	_core_base_scale = core.scale
	_core_target = _core_origin
	_core_target_offset = Vector2.ZERO
	_shards = [
		_make_shard(shard_top, 0.11, 0.83, 18.0, 0.3),
		_make_shard(shard_left, -0.09, 0.71, 14.0, 1.9),
		_make_shard(shard_bottom, 0.13, 0.62, 20.0, 3.4),
	]
	_pick_next_core_target()

func _process(delta: float) -> void:
	_shake_time += delta
	background.rotation = sin(_shake_time * 0.22) * 0.03 + _shake_time * 0.012
	_update_ambient_prisms(delta)
	_update_stars(delta)

	var tremble := Vector2(
		sin(_shake_time * 12.0) * 1.1 + sin(_shake_time * 18.0) * 0.45,
		cos(_shake_time * 10.0) * 0.9 + sin(_shake_time * 16.0) * 0.35
	)
	parts.position = _base_parts_position + tremble
	body.position = _body_origin + Vector2(0.0, sin(_shake_time * 1.55) * 7.0)
	body.rotation = sin(_shake_time * 1.1) * 0.012
	body.scale = _body_base_scale * (1.0 + sin(_shake_time * 1.45) * 0.008)

	_update_shards(delta)
	core.scale = _core_base_scale * (1.0 + sin(_shake_time * 3.8) * 0.04)

	if _core_hold > 0.0:
		_core_hold -= delta
		return

	_core_move += delta
	var t: float = clamp(_core_move / 0.45, 0.0, 1.0)
	t = t * t * (3.0 - 2.0 * t)
	core.position = core.position.lerp(_core_target, t)

	if core.position.distance_to(_core_target) < 1.5:
		core.position = _core_target
		_core_hold = _rng.randf_range(0.35, 0.95)
		_pick_next_core_target()

func _pick_next_core_target() -> void:
	_core_move = 0.0
	var angle := _rng.randf_range(0.0, TAU)
	if _core_target_offset.length_squared() > 4.0 and _rng.randf() < 0.78:
		angle = _core_target_offset.angle() + PI + _rng.randf_range(-0.72, 0.72)

	var radius := sqrt(_rng.randf()) * 42.0
	if _core_target_offset.length() < 18.0:
		radius = _rng.randf_range(24.0, 42.0)

	var offset := Vector2(cos(angle) * radius * 0.72, sin(angle) * radius)
	if offset.distance_to(_core_target_offset) < 16.0:
		offset = offset.rotated(PI * 0.55)

	_core_target_offset = offset
	_core_target = _core_origin + offset

func _make_shard(node: Sprite2D, orbit_amount: float, speed: float, jitter_radius: float, phase: float) -> Dictionary:
	return {
		"node": node,
		"origin": node.position,
		"offset": node.position - _body_origin,
		"rotation": node.rotation,
		"orbit_amount": orbit_amount,
		"speed": speed,
		"jitter_radius": jitter_radius,
		"phase": phase,
		"jitter": Vector2.ZERO,
		"jitter_target": Vector2.ZERO,
		"next_jitter": _rng.randf_range(0.2, 0.8),
	}

func _update_shards(delta: float) -> void:
	for shard in _shards:
		var next_jitter: float = shard["next_jitter"] - delta
		shard["next_jitter"] = next_jitter
		if next_jitter <= 0.0:
			var jitter_angle := _rng.randf_range(0.0, TAU)
			var jitter_length: float = sqrt(_rng.randf()) * shard["jitter_radius"]
			shard["jitter_target"] = Vector2(cos(jitter_angle), sin(jitter_angle)) * jitter_length
			shard["next_jitter"] = _rng.randf_range(0.45, 1.35)

		var jitter: Vector2 = shard["jitter"]
		var jitter_target: Vector2 = shard["jitter_target"]
		jitter = jitter.lerp(jitter_target, 1.0 - pow(0.025, delta))
		shard["jitter"] = jitter

		var phase: float = _shake_time * shard["speed"] + shard["phase"]
		var speed_curve := 0.55 + 0.45 * sin(phase * 1.7)
		var orbit_angle: float = sin(phase + sin(phase * 0.37) * 0.6) * shard["orbit_amount"] * speed_curve
		var orbit_offset: Vector2 = shard["offset"].rotated(orbit_angle)
		var node: Sprite2D = shard["node"]
		node.position = body.position + orbit_offset + jitter
		node.rotation = shard["rotation"] + orbit_angle * 0.75 + sin(phase * 2.3) * 0.035

func _setup_background_motion() -> void:
	var viewport_size := get_viewport_rect().size
	if viewport_size == Vector2.ZERO:
		viewport_size = Vector2(1920.0, 1080.0)
	background.pivot_offset = viewport_size * 0.5
	background.scale = Vector2(1.18, 1.18)

func _create_ambient_layers() -> void:
	var star_layer := Node2D.new()
	star_layer.name = "StarGlowLayer"
	add_child(star_layer)
	move_child(star_layer, 1)

	var prism_layer := Node2D.new()
	prism_layer.name = "AmbientPrismLayer"
	add_child(prism_layer)
	move_child(prism_layer, 2)

	var star_texture: Texture2D = load(STAR_GLOW_PATH)
	for i in range(70):
		var star := Sprite2D.new()
		star.texture = star_texture
		star.centered = true
		star.position = _random_star_position()
		star.scale = Vector2.ONE * _rng.randf_range(0.035, 0.13)
		star.modulate = Color(1.0, _rng.randf_range(0.84, 1.0), _rng.randf_range(0.55, 1.0), _rng.randf_range(0.18, 0.65))
		star_layer.add_child(star)
		_stars.append({
			"node": star,
			"origin": star.position,
			"phase": _rng.randf_range(0.0, TAU),
			"speed": _rng.randf_range(2.4, 5.8),
			"drift": Vector2(_rng.randf_range(-34.0, 34.0), _rng.randf_range(-28.0, 28.0)),
			"base_scale": star.scale,
			"base_alpha": star.modulate.a,
		})

	for i in range(18):
		var prism := Sprite2D.new()
		prism.texture = load(AMBIENT_PRISM_PATHS[i % AMBIENT_PRISM_PATHS.size()])
		prism.centered = true
		prism.position = _random_ambient_prism_position()
		var scale_value := _rng.randf_range(0.33, 0.92)
		prism.scale = Vector2.ONE * scale_value
		prism.rotation = _rng.randf_range(-0.55, 0.55)
		prism.modulate = Color(0.78, 0.9, 1.0, _rng.randf_range(0.13, 0.32))
		prism_layer.add_child(prism)
		_ambient_prisms.append({
			"node": prism,
			"origin": prism.position,
			"phase": _rng.randf_range(0.0, TAU),
			"speed": _rng.randf_range(0.55, 1.35),
			"drift": Vector2(_rng.randf_range(-76.0, 76.0), _rng.randf_range(-58.0, 58.0)),
			"rotation_speed": _rng.randf_range(-0.13, 0.13),
			"base_rotation": prism.rotation,
			"base_scale": prism.scale,
			"base_alpha": prism.modulate.a,
			"twinkle_speed": _rng.randf_range(1.1, 3.4),
			"twinkle_phase": _rng.randf_range(0.0, TAU),
		})

func _random_star_position() -> Vector2:
	var viewport_size := get_viewport_rect().size
	if viewport_size == Vector2.ZERO:
		viewport_size = Vector2(1920.0, 1080.0)
	if _rng.randf() < 0.72:
		var angle := _rng.randf_range(0.0, TAU)
		var radius := _rng.randf_range(130.0, 560.0)
		var center := Vector2(1600.0, 475.0)
		return center + Vector2(cos(angle), sin(angle)) * radius
	return Vector2(_rng.randf_range(-60.0, viewport_size.x + 60.0), _rng.randf_range(-40.0, viewport_size.y + 40.0))

func _random_ambient_prism_position() -> Vector2:
	var viewport_size := get_viewport_rect().size
	if viewport_size == Vector2.ZERO:
		viewport_size = Vector2(1920.0, 1080.0)
	var pos := Vector2(_rng.randf_range(-110.0, viewport_size.x + 110.0), _rng.randf_range(20.0, viewport_size.y - 20.0))
	if pos.distance_to(Vector2(1600.0, 475.0)) < 230.0:
		pos += (pos - Vector2(1600.0, 475.0)).normalized() * 230.0
	return pos

func _update_ambient_prisms(delta: float) -> void:
	for prism in _ambient_prisms:
		var node: Sprite2D = prism["node"]
		var phase: float = _shake_time * prism["speed"] + prism["phase"]
		var drift: Vector2 = prism["drift"]
		node.position = prism["origin"] + Vector2(sin(phase) * drift.x, cos(phase * 0.83) * drift.y)
		node.rotation = prism["base_rotation"] + sin(phase * 0.7) * 0.13 + _shake_time * prism["rotation_speed"]
		var twinkle := 0.5 + 0.5 * sin(_shake_time * prism["twinkle_speed"] + prism["twinkle_phase"] + sin(phase) * 0.75)
		twinkle = smoothstep(0.0, 1.0, twinkle)
		var pulse := 1.0 + sin(phase * 1.35) * 0.055 + twinkle * 0.045
		node.scale = prism["base_scale"] * pulse
		var color := node.modulate
		var glow := 0.72 + twinkle * 0.62
		color.r = min(1.0, 0.78 * glow)
		color.g = min(1.0, 0.9 * glow)
		color.b = min(1.0, 1.0 * glow)
		color.a = prism["base_alpha"] * (0.55 + twinkle * 1.15)
		node.modulate = color

func _update_stars(delta: float) -> void:
	for star in _stars:
		var node: Sprite2D = star["node"]
		var phase: float = _shake_time * star["speed"] + star["phase"]
		var drift: Vector2 = star["drift"]
		node.position = star["origin"] + Vector2(sin(phase) * drift.x, cos(phase * 0.77) * drift.y)
		var pulse := 0.72 + sin(phase * 3.1) * 0.28
		node.scale = star["base_scale"] * (0.82 + pulse * 0.45)
		var color := node.modulate
		color.a = star["base_alpha"] * (0.58 + pulse * 0.55)
		node.modulate = color

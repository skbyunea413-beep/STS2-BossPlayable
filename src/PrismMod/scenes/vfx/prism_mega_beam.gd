extends Node2D

const RIBBON_COLORS: Array[Color] = [
	Color("#ff3355"),
	Color("#ff9f1c"),
	Color("#fff275"),
	Color("#43ff88"),
	Color("#36f4ff"),
	Color("#5d7cff"),
	Color("#c45cff"),
]

@export var duration := 0.72
@export var charge_duration := 0.38
@export var point_count := 24
@export var core_width := 34.0
@export var glow_width := 138.0
@export var ribbon_width := 15.0
@export var noise_strength := 42.0
@export var shake_strength := 12.0

var _from := Vector2(220.0, 470.0)
var _to := Vector2(1500.0, 410.0)
var _power := 1.0
var _time := 0.0
var _fade := 0.0
var _beam_fade := 0.0
var _charge_fade := 0.0
var _rng := RandomNumberGenerator.new()

var _flash: Polygon2D
var _charge_glow: Line2D
var _charge_core: Line2D
var _wide_glow: Line2D
var _blue_body: Line2D
var _hot_core: Line2D
var _white_core: Line2D
var _impact_ring: Line2D
var _ribbons: Array[Line2D] = []
var _sparks: Array[Line2D] = []

func setup(from: Vector2, to: Vector2, power_scale := 1.0) -> void:
	_from = from
	_to = to
	_power = clamp(power_scale, 0.85, 2.4)
	if is_inside_tree():
		_apply_static_scale()
		_redraw(true)

func _ready() -> void:
	z_index = 900
	_rng.randomize()
	_build_nodes()
	_apply_static_scale()
	_redraw(true)

	var tween := create_tween()
	tween.tween_property(self, "_fade", 1.0, 0.055).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)
	tween.parallel().tween_property(self, "_charge_fade", 1.0, 0.075).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)
	tween.tween_interval(charge_duration)
	tween.tween_property(self, "_beam_fade", 1.0, 0.045).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)
	tween.parallel().tween_property(self, "_charge_fade", 0.0, 0.11).set_trans(Tween.TRANS_CUBIC).set_ease(Tween.EASE_IN)
	tween.tween_interval(duration * 0.56)
	tween.tween_property(self, "_beam_fade", 0.0, duration * 0.24).set_trans(Tween.TRANS_CUBIC).set_ease(Tween.EASE_IN)
	tween.parallel().tween_property(self, "_fade", 0.0, duration * 0.24).set_trans(Tween.TRANS_CUBIC).set_ease(Tween.EASE_IN)
	tween.finished.connect(queue_free)

func _process(delta: float) -> void:
	_time += delta
	_redraw(false)

func _build_nodes() -> void:
	_flash = Polygon2D.new()
	_flash.color = Color(1.0, 1.0, 1.0, 0.18)
	add_child(_flash)

	_charge_glow = _make_line(Color(0.75, 0.35, 1.0, 0.78), 18.0, 40)
	_charge_glow.closed = true
	_charge_core = _make_line(Color(1.0, 1.0, 1.0, 1.0), 9.0, 41)
	_charge_core.closed = true

	_wide_glow = _make_line(Color(0.42, 0.25, 1.0, 0.30), glow_width, 0)
	_blue_body = _make_line(Color(0.1, 0.93, 1.0, 0.55), 86.0, 1)
	_hot_core = _make_line(Color(0.72, 1.0, 1.0, 0.86), 48.0, 2)
	_white_core = _make_line(Color(1.0, 1.0, 1.0, 1.0), core_width, 3)

	for i in RIBBON_COLORS.size():
		var color := RIBBON_COLORS[i]
		color.a = 0.92
		var ribbon := _make_line(color, ribbon_width, 4 + i)
		_ribbons.append(ribbon)

	for i in 18:
		var spark := _make_line(Color(1.0, 0.95, 0.45, 0.85), 3.0, 20 + i)
		spark.begin_cap_mode = Line2D.LINE_CAP_NONE
		spark.end_cap_mode = Line2D.LINE_CAP_NONE
		_sparks.append(spark)

	_impact_ring = _make_line(Color(0.75, 1.0, 1.0, 0.88), 8.0, 50)
	_impact_ring.closed = true

func _make_line(color: Color, width: float, line_z: int) -> Line2D:
	var line := Line2D.new()
	line.default_color = color
	line.width = width
	line.joint_mode = Line2D.LINE_JOINT_ROUND
	line.begin_cap_mode = Line2D.LINE_CAP_ROUND
	line.end_cap_mode = Line2D.LINE_CAP_ROUND
	line.z_index = line_z
	line.width_curve = _make_width_curve()
	var material := CanvasItemMaterial.new()
	material.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
	line.material = material
	add_child(line)
	return line

func _make_width_curve() -> Curve:
	var curve := Curve.new()
	curve.add_point(Vector2(0.0, 0.06))
	curve.add_point(Vector2(0.12, 1.12))
	curve.add_point(Vector2(0.42, 0.88))
	curve.add_point(Vector2(0.68, 1.24))
	curve.add_point(Vector2(1.0, 0.16))
	return curve

func _apply_static_scale() -> void:
	_wide_glow.width = glow_width * _power
	_blue_body.width = 86.0 * _power
	_hot_core.width = 48.0 * _power
	_white_core.width = core_width * _power
	_charge_glow.width = 18.0 * _power
	_charge_core.width = 9.0 * _power
	for ribbon in _ribbons:
		ribbon.width = ribbon_width * _power

func _redraw(force_noise: bool) -> void:
	var pulse := 1.0 + sin(_time * 42.0) * 0.06 + sin(_time * 77.0) * 0.035
	var dir := _to - _from
	if dir.length_squared() < 1.0:
		dir = Vector2.RIGHT
	var normal := dir.normalized().orthogonal()
	var jitter := normal * sin(_time * 93.0) * shake_strength * _beam_fade * _power
	position = jitter

	_flash.polygon = PackedVector2Array([
		Vector2(-200.0, -200.0),
		Vector2(2300.0, -200.0),
		Vector2(2300.0, 1300.0),
		Vector2(-200.0, 1300.0),
	])
	_flash.color.a = 0.11 * _beam_fade * max(0.0, 1.0 - max(0.0, _time - charge_duration) * 5.0)

	_redraw_charge(normal)

	_apply_points(_wide_glow, _make_wavy_points(noise_strength * 0.62, 0.0, 0.55), _beam_fade * 0.72)
	_apply_points(_blue_body, _make_wavy_points(noise_strength * 0.45, 4.0, 0.76), _beam_fade * 0.82)
	_apply_points(_hot_core, _make_wavy_points(noise_strength * 0.22, 8.0, 1.0), _beam_fade * pulse)
	_apply_points(_white_core, _make_wavy_points(noise_strength * 0.10, 12.0, 1.2), _beam_fade)

	for i in _ribbons.size():
		var phase := float(i) * 2.35
		var offset := normal * sin(_time * 9.0 + phase) * (26.0 + float(i % 3) * 12.0) * _power
		var points := _make_wavy_points(noise_strength * 1.65, phase, 1.35, offset)
		_apply_points(_ribbons[i], points, _beam_fade * (0.72 + 0.18 * sin(_time * 16.0 + phase)))

	for i in _sparks.size():
		_redraw_spark(_sparks[i], i, normal, dir.normalized())

	_redraw_impact(normal)

func _redraw_charge(normal: Vector2) -> void:
	var gather := clamp(_time / max(charge_duration, 0.01), 0.0, 1.0)
	var pulse := 1.0 + sin(_time * 38.0) * 0.12
	var glow_radius := lerp(28.0, 92.0, gather) * pulse * _power
	var core_radius := lerp(10.0, 34.0, gather) * (1.0 + sin(_time * 57.0) * 0.08) * _power
	_draw_ring(_charge_glow, _from, glow_radius, 22, _charge_fade * (0.72 + gather * 0.22), 0.22)
	_draw_ring(_charge_core, _from, core_radius, 16, _charge_fade, 0.10)

func _draw_ring(line: Line2D, center: Vector2, radius: float, count: int, alpha: float, raggedness: float) -> void:
	line.clear_points()
	for i in count:
		var angle := TAU * float(i) / float(count)
		var ragged := 1.0 + sin(angle * 5.0 + _time * 24.0) * raggedness
		line.add_point(center + Vector2(cos(angle), sin(angle)) * radius * ragged)
	var color := line.default_color
	color.a = clamp(alpha, 0.0, 1.0)
	line.default_color = color

func _make_wavy_points(strength: float, phase: float, frequency_scale: float, extra_offset := Vector2.ZERO) -> PackedVector2Array:
	var result := PackedVector2Array()
	var dir := _to - _from
	var normal := dir.normalized().orthogonal()

	for i in point_count:
		var t := float(i) / float(point_count - 1)
		var base := _from.lerp(_to, t)
		var taper := sin(t * PI)
		var wave_a := sin(t * 18.0 * frequency_scale + _time * 24.0 + phase)
		var wave_b := sin(t * 47.0 * frequency_scale - _time * 38.0 + phase * 1.7)
		var tear := sin(t * 91.0 + _time * 61.0 + phase) * 0.18
		var offset := normal * (wave_a * 0.62 + wave_b * 0.30 + tear) * strength * taper * _power
		result.append(base + offset + extra_offset * taper)

	return result

func _apply_points(line: Line2D, points: PackedVector2Array, alpha: float) -> void:
	line.clear_points()
	for point in points:
		line.add_point(point)
	var color := line.default_color
	color.a = clamp(alpha, 0.0, 1.0)
	line.default_color = color

func _redraw_spark(line: Line2D, index: int, normal: Vector2, forward: Vector2) -> void:
	var t := fmod(_time * (1.8 + float(index % 5) * 0.18) + float(index) * 0.137, 1.0)
	var beam_pos := _from.lerp(_to, t)
	var side := -1.0 if index % 2 == 0 else 1.0
	var wave := sin(_time * 31.0 + float(index) * 5.1)
	var spread := (58.0 + float(index % 4) * 18.0) * _power
	var start := beam_pos + normal * side * spread * wave
	var end := start + forward * (36.0 + float(index % 3) * 22.0) + normal * side * 34.0
	line.clear_points()
	line.add_point(start)
	line.add_point((start + end) * 0.5 + normal * side * 18.0)
	line.add_point(end)
	var color := RIBBON_COLORS[index % RIBBON_COLORS.size()]
	color.a = _beam_fade * (0.25 + 0.55 * abs(wave))
	line.default_color = color

func _redraw_impact(normal: Vector2) -> void:
	var radius := (74.0 + sin(_time * 34.0) * 14.0) * _power
	_impact_ring.clear_points()
	for i in 18:
		var angle := TAU * float(i) / 18.0
		var ragged := 1.0 + sin(angle * 7.0 + _time * 26.0) * 0.18
		_impact_ring.add_point(_to + Vector2(cos(angle), sin(angle)) * radius * ragged)
	var color := _impact_ring.default_color
	color.a = _beam_fade * 0.72
	_impact_ring.default_color = color

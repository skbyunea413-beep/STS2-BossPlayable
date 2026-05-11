extends Node2D

const EDGE_COLORS: Array[Color] = [
	Color("#ffdd55"),
	Color("#5fffe1"),
	Color("#8e72ff"),
	Color("#ff5f9f"),
]

@export var charge_duration := 0.28
@export var beam_duration := 0.44
@export var point_count := 13
@export var body_width := 92.0
@export var core_width := 42.0
@export var edge_width := 8.0
@export var roughness := 16.0

var _from := Vector2(220.0, 470.0)
var _to := Vector2(1500.0, 410.0)
var _power := 1.0
var _time := 0.0
var _rng := RandomNumberGenerator.new()

var _charge_outer: Line2D
var _charge_inner: Line2D
var _beam_shadow: Polygon2D
var _beam_body: Polygon2D
var _beam_core: Polygon2D
var _beam_ink_top: Line2D
var _beam_ink_bottom: Line2D
var _edge_lines: Array[Line2D] = []
var _impact_lines: Array[Line2D] = []

func setup(from: Vector2, to: Vector2, power_scale := 1.0) -> void:
	_from = from
	_to = to
	_power = clamp(power_scale, 0.85, 2.1)
	if is_inside_tree():
		_redraw()

func _ready() -> void:
	z_index = 900
	_rng.randomize()
	_build_nodes()
	_redraw()

func _process(delta: float) -> void:
	_time += delta
	_redraw()
	if _time > charge_duration + beam_duration + 0.24:
		queue_free()

func _build_nodes() -> void:
	_charge_outer = _make_line(Color(0.62, 0.92, 1.0, 0.85), 10.0, 20, true)
	_charge_inner = _make_line(Color(1.0, 1.0, 1.0, 1.0), 5.0, 21, true)

	_beam_shadow = _make_poly(Color(0.08, 0.06, 0.18, 0.36), 0)
	_beam_body = _make_poly(Color(0.24, 0.92, 1.0, 0.86), 1)
	_beam_core = _make_poly(Color(1.0, 1.0, 0.94, 0.96), 2)

	_beam_ink_top = _make_line(Color(0.95, 0.98, 1.0, 0.82), edge_width, 4, false)
	_beam_ink_bottom = _make_line(Color(0.40, 0.72, 1.0, 0.68), edge_width, 4, false)

	for i in EDGE_COLORS.size():
		_edge_lines.append(_make_line(EDGE_COLORS[i], edge_width * 0.7, 5 + i, false))

	for i in 14:
		var line := _make_line(EDGE_COLORS[i % EDGE_COLORS.size()], 5.0, 16 + i, false)
		line.begin_cap_mode = Line2D.LINE_CAP_NONE
		line.end_cap_mode = Line2D.LINE_CAP_NONE
		_impact_lines.append(line)

func _make_poly(color: Color, poly_z: int) -> Polygon2D:
	var poly := Polygon2D.new()
	poly.color = color
	poly.z_index = poly_z
	add_child(poly)
	return poly

func _make_line(color: Color, width: float, line_z: int, closed: bool) -> Line2D:
	var line := Line2D.new()
	line.default_color = color
	line.width = width
	line.z_index = line_z
	line.closed = closed
	line.joint_mode = Line2D.LINE_JOINT_ROUND
	line.begin_cap_mode = Line2D.LINE_CAP_ROUND
	line.end_cap_mode = Line2D.LINE_CAP_ROUND
	add_child(line)
	return line

func _redraw() -> void:
	var dir := _to - _from
	if dir.length_squared() < 1.0:
		dir = Vector2.RIGHT
	var forward := dir.normalized()
	var normal := forward.orthogonal()
	var beam_age: float = _time - charge_duration
	var beam_t: float = clamp(beam_age / max(beam_duration, 0.01), 0.0, 1.0)
	var charge_t: float = clamp(_time / max(charge_duration, 0.01), 0.0, 1.0)

	_redraw_charge(charge_t)

	var fire: float = _beam_envelope(beam_age)
	var length_t: float = clamp(beam_age / 0.10, 0.0, 1.0)
	var current_to := _from.lerp(_to, length_t)
	var width_mul: float = fire * (1.0 - 0.42 * beam_t)
	var body: PackedVector2Array = _make_brush_polygon(current_to, body_width * _power * width_mul, roughness * _power)
	var core: PackedVector2Array = _make_brush_polygon(current_to, core_width * _power * width_mul, roughness * 0.35 * _power)

	_beam_shadow.polygon = _offset_polygon(body, normal * 8.0)
	_beam_body.polygon = body
	_beam_core.polygon = core

	_set_poly_alpha(_beam_shadow, fire * 0.24)
	_set_poly_alpha(_beam_body, fire * 0.86)
	_set_poly_alpha(_beam_core, fire)

	_redraw_ink_edges(body, fire, normal)
	_redraw_edge_colors(current_to, fire, normal)
	_redraw_impact(current_to, fire, forward, normal)

func _beam_envelope(age: float) -> float:
	if age < 0.0:
		return 0.0
	if age < 0.045:
		return age / 0.045
	if age < 0.13:
		return 1.16 - (age - 0.045) / 0.085 * 0.18
	var fade_t: float = clamp((age - 0.13) / max(beam_duration - 0.13, 0.01), 0.0, 1.0)
	return max(0.0, 0.98 * (1.0 - fade_t * fade_t))

func _redraw_charge(charge_t: float) -> void:
	var visible: float = max(0.0, 1.0 - max(0.0, _time - charge_duration) / 0.12)
	var pulse: float = 1.0 + sin(_time * 34.0) * 0.08
	_draw_ring(_charge_outer, _from, lerp(18.0, 42.0, charge_t) * pulse * _power, 12, visible * 0.75)
	_draw_ring(_charge_inner, _from, lerp(7.0, 18.0, charge_t) * _power, 10, visible)

func _draw_ring(line: Line2D, center: Vector2, radius: float, count: int, alpha: float) -> void:
	line.clear_points()
	for i in count:
		var angle: float = TAU * float(i) / float(count)
		var ragged: float = 1.0 + sin(angle * 4.0 + _time * 21.0) * 0.12
		line.add_point(center + Vector2(cos(angle), sin(angle)) * radius * ragged)
	var color := line.default_color
	color.a = clamp(alpha, 0.0, 1.0)
	line.default_color = color

func _make_brush_polygon(current_to: Vector2, width: float, jag: float) -> PackedVector2Array:
	var dir := current_to - _from
	if dir.length_squared() < 1.0:
		dir = Vector2.RIGHT
	var forward := dir.normalized()
	var normal := forward.orthogonal()
	var top := PackedVector2Array()
	var bottom := PackedVector2Array()
	for i in point_count:
		var t: float = float(i) / float(point_count - 1)
		var base := _from.lerp(current_to, t)
		var taper: float = sin(t * PI)
		var blast_head: float = 1.0 + smoothstep(0.72, 1.0, t) * 0.38
		var tail_cut: float = smoothstep(0.0, 0.16, t)
		var half: float = width * (0.20 + 0.80 * taper) * blast_head * tail_cut
		var wobble: float = sin(t * 21.0 + _time * 19.0) * jag * taper
		var scratch: float = sin(t * 53.0 - _time * 27.0) * jag * 0.38 * taper
		top.append(base + normal * (half + wobble + scratch))
		bottom.insert(0, base - normal * (half + wobble * 0.8 - scratch))
	for point in bottom:
		top.append(point)
	return top

func _offset_polygon(points: PackedVector2Array, offset: Vector2) -> PackedVector2Array:
	var result := PackedVector2Array()
	for point in points:
		result.append(point + offset)
	return result

func _set_poly_alpha(poly: Polygon2D, alpha: float) -> void:
	var color := poly.color
	color.a = clamp(alpha, 0.0, 1.0)
	poly.color = color

func _redraw_ink_edges(body: PackedVector2Array, alpha: float, normal: Vector2) -> void:
	_beam_ink_top.clear_points()
	_beam_ink_bottom.clear_points()
	for i in point_count:
		_beam_ink_top.add_point(body[i])
		_beam_ink_bottom.add_point(body[body.size() - 1 - i])
	_set_line_alpha(_beam_ink_top, alpha * 0.78)
	_set_line_alpha(_beam_ink_bottom, alpha * 0.54)

func _redraw_edge_colors(current_to: Vector2, alpha: float, normal: Vector2) -> void:
	var forward := (current_to - _from).normalized()
	for i in _edge_lines.size():
		var line := _edge_lines[i]
		line.clear_points()
		var side: float = -1.0 if i % 2 == 0 else 1.0
		var offset: float = (body_width * 0.36 + float(i) * 5.0) * _power * side
		for p in 5:
			var t: float = float(p) / 4.0
			var start_t: float = 0.10 + float(i) * 0.04
			var tt: float = clamp(start_t + t * 0.72, 0.0, 1.0)
			var base := _from.lerp(current_to, tt)
			var wave := sin(tt * 18.0 + _time * 16.0 + float(i)) * 7.0 * _power
			line.add_point(base + normal * (offset + wave) - forward * (1.0 - tt) * 18.0)
		line.width = edge_width * (0.45 + 0.12 * float(i)) * _power
		_set_line_alpha(line, alpha * 0.42)

func _redraw_impact(current_to: Vector2, alpha: float, forward: Vector2, normal: Vector2) -> void:
	for i in _impact_lines.size():
		var line := _impact_lines[i]
		line.clear_points()
		var angle: float = -0.95 + 1.9 * float(i) / max(float(_impact_lines.size() - 1), 1.0)
		var burst_dir := (forward * 0.85 + normal * sin(angle) * 0.85).normalized()
		var len: float = (28.0 + float(i % 4) * 12.0) * _power * alpha
		var start := current_to - forward * 8.0
		line.add_point(start)
		line.add_point(start + burst_dir * len)
		line.width = (3.0 + float(i % 3)) * _power
		_set_line_alpha(line, alpha * (0.18 + 0.55 * smoothstep(0.65, 1.0, alpha)))

func _set_line_alpha(line: Line2D, alpha: float) -> void:
	var color := line.default_color
	color.a = clamp(alpha, 0.0, 1.0)
	line.default_color = color

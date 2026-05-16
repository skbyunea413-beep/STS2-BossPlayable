@tool
extends Node2D

@export_range(0.0, 1.0, 0.01) var progress: float = 0.0:
	set(value):
		progress = value
		queue_redraw()

@export var animate_in_editor: bool = true
@export var start_position: Vector2 = Vector2(620, 290):
	set(value):
		start_position = value
		queue_redraw()

@export var end_position: Vector2 = Vector2(240, 320):
	set(value):
		end_position = value
		queue_redraw()

var _time: float = 0.0


func _process(delta: float) -> void:
	if not animate_in_editor:
		return
	_time += delta
	progress = fposmod(_time * 5.2, 1.0)


func _draw() -> void:
	draw_rect(Rect2(Vector2.ZERO, Vector2(860, 520)), Color(0.055, 0.045, 0.04, 1.0), true)
	_draw_actor_marker(start_position, Color(0.95, 0.22, 0.12, 1.0), "BOSS")
	_draw_actor_marker(end_position, Color(0.25, 0.8, 1.0, 1.0), "PLAYER")

	var eased := progress
	var fist_pos := start_position.lerp(end_position, eased) + Vector2(-40.0, -24.0)
	var dir := (end_position - start_position).normalized()
	var normal := Vector2(-dir.y, dir.x)

	_draw_heat_wake(fist_pos, dir, normal)
	_draw_lava_drips(start_position, end_position, dir, normal)
	_draw_afterimages(start_position, fist_pos, dir, normal)
	_draw_molten_fist(fist_pos, dir, normal)
	_draw_impact(end_position, progress)


func _draw_actor_marker(pos: Vector2, color: Color, label: String) -> void:
	draw_circle(pos, 26.0, color.darkened(0.45))
	draw_circle(pos, 19.0, color)
	draw_string(ThemeDB.fallback_font, pos + Vector2(-24, 48), label, HORIZONTAL_ALIGNMENT_LEFT, -1.0, 16, color)


func _draw_molten_fist(pos: Vector2, dir: Vector2, normal: Vector2) -> void:
	var pulse := 1.0 + sin(progress * TAU * 2.0) * 0.08
	var core := PackedVector2Array([
		pos + dir * 78.0 * pulse,
		pos + normal * 36.0 * pulse,
		pos - dir * 58.0 * pulse + normal * 24.0 * pulse,
		pos - dir * 70.0 * pulse - normal * 12.0 * pulse,
		pos + normal * -38.0 * pulse,
	])
	draw_colored_polygon(core, Color(1.0, 0.26, 0.04, 0.96))
	draw_polyline(core + PackedVector2Array([core[0]]), Color(1.0, 0.88, 0.36, 0.9), 5.0, true)
	draw_circle(pos + dir * 16.0 - normal * 4.0, 48.0 * pulse, Color(1.0, 0.46, 0.04, 0.22))
	draw_circle(pos + dir * 28.0 - normal * 6.0, 22.0 * pulse, Color(1.0, 0.92, 0.42, 0.62))


func _draw_afterimages(start: Vector2, fist_pos: Vector2, dir: Vector2, normal: Vector2) -> void:
	for i in range(5):
		var t := float(i) / 5.0
		var trail_pos := start.lerp(fist_pos, t) - dir * (42.0 + i * 18.0) + normal * sin(t * 11.0 + progress * 5.0) * 12.0
		var radius := 48.0 - i * 6.0
		var alpha := 0.32 - i * 0.045
		draw_circle(trail_pos, radius, Color(1.0, 0.25 + t * 0.25, 0.02, alpha))
		draw_circle(trail_pos - dir * 28.0, radius * 0.58, Color(1.0, 0.78, 0.15, alpha * 0.55))


func _draw_lava_drips(start: Vector2, end: Vector2, dir: Vector2, normal: Vector2) -> void:
	for i in range(9):
		var t := float(i) / 8.0
		if t > progress + 0.18:
			continue
		var base := start.lerp(end, clamp(t / max(progress, 0.08), 0.0, 1.0))
		var fall := 18.0 + i * 4.0 + progress * 24.0
		var p1 := base - dir * (18.0 + i * 9.0) + normal * sin(i * 2.1) * 18.0 + Vector2(0, 16)
		var p2 := p1 + Vector2(-10, fall)
		draw_line(p1, p2, Color(1.0, 0.18 + t * 0.35, 0.01, 0.62 - t * 0.28), 6.0)
		draw_circle(p2, 6.0 + i % 3, Color(1.0, 0.48, 0.04, 0.48))


func _draw_heat_wake(pos: Vector2, dir: Vector2, normal: Vector2) -> void:
	for i in range(7):
		var length := 70.0 + i * 22.0
		var offset := normal * (-48.0 + i * 16.0)
		var from := pos - dir * length + offset
		var to := pos - dir * (length * 0.35) + offset * 0.45
		draw_line(from, to, Color(1.0, 0.82, 0.38, 0.18), 2.0 + i % 2)


func _draw_impact(pos: Vector2, p: float) -> void:
	if p < 0.72:
		return
	var t := inverse_lerp(0.72, 1.0, p)
	var alpha := 1.0 - t
	draw_circle(pos + Vector2(10, -6), 34.0 + t * 52.0, Color(1.0, 0.35, 0.02, 0.42 * alpha))
	draw_arc(pos + Vector2(10, -6), 42.0 + t * 36.0, 0.0, TAU, 24, Color(1.0, 0.86, 0.25, 0.8 * alpha), 5.0)
	for i in range(10):
		var angle := TAU * float(i) / 10.0
		var from := pos + Vector2(cos(angle), sin(angle)) * 22.0
		var to := pos + Vector2(cos(angle), sin(angle)) * (70.0 + t * 30.0)
		draw_line(from, to, Color(1.0, 0.7, 0.16, 0.7 * alpha), 3.0)

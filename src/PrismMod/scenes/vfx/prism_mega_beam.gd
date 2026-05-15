extends Node2D

const ENERGY_091_SHEET: Texture2D = preload("res://PrismMod/images/vfx/energy_091_projectile_loop/energy_091_sheet_768_6x2.png")
const ENERGY_109_SHEET: Texture2D = preload("res://PrismMod/images/vfx/energy_109_charge_start/energy_109_sheet_512_6x4.png")
const ELECTRICITY_016_SHEET: Texture2D = preload("res://PrismMod/images/vfx/electricity_016_charge_radial/electricity_016_sheet_512_6x6.png")
const SCREEN_TRANSITION_OVERLAY_SCRIPT: Script = preload("res://PrismMod/scenes/vfx/prism_screen_transition_overlay.gd")

@export_range(0.0, 1.80, 0.01) var preview_time: float = 0.0
@export var animate_in_editor: bool = false
@export_range(0.1, 3.0, 0.05) var preview_speed: float = 1.0
@export_range(0.6, 2.4, 0.05) var power: float = 1.25
@export_range(50.0, 180.0, 1.0) var body_width: float = 118.0
@export_range(20.0, 120.0, 1.0) var core_width: float = 60.0
@export_range(2.0, 56.0, 1.0) var roughness: float = 26.0
@export_range(0.0, 1.0, 0.01) var hue_offset: float = 0.47
@export_range(0.0, 3.0, 0.01) var rainbow_speed: float = 1.35
@export_range(700.0, 1800.0, 10.0) var beam_length: float = 1420.0
@export_range(0.0, 1.0, 0.01) var release_flash: float = 0.72
@export_range(0.0, 1.0, 0.01) var release_glitch: float = 0.76
@export var show_external_loop: bool = true
@export_range(0.2, 2.0, 0.05) var external_loop_scale: float = 0.92
@export_range(0.0, 1.0, 0.01) var external_loop_alpha: float = 0.74
@export_range(-400.0, 400.0, 5.0) var external_loop_forward_offset: float = 135.0
@export_range(-240.0, 240.0, 5.0) var external_loop_normal_offset: float = -12.0
@export var show_charge_start_hit: bool = true
@export_range(0.2, 2.5, 0.05) var charge_start_hit_scale: float = 0.82
@export_range(0.0, 1.0, 0.01) var charge_start_hit_alpha: float = 0.68
@export var show_electric_charge: bool = true
@export_range(0.2, 2.5, 0.05) var electric_charge_scale: float = 0.92
@export_range(0.0, 1.0, 0.01) var electric_charge_alpha: float = 0.82
@export var show_screen_transition: bool = true
@export_range(0.0, 1.0, 0.01) var screen_transition_alpha: float = 0.82
@export var show_drawn_charge: bool = false
@export var show_wave_head: bool = true
@export var show_guides: bool = false

const CHARGE_DURATION: float = 0.34
const BEAM_DURATION: float = 1.17
const BEAM_HOLD_DURATION: float = 0.87
const POINT_COUNT: int = 43
const PARTICLE_SPEED: float = 6.0
const BEAM_DRIFT_SPEED: float = 900.0

var _from: Vector2 = Vector2.ZERO
var _to: Vector2 = Vector2(1420.0, -48.0)
var _visual_time: float = 0.0
var _effect_time_scale: float = 1.0
var _screen_transition_overlay: Node2D

func setup(from: Vector2, to: Vector2, power_scale: float = 1.0) -> void:
	_from = from
	_to = to
	power = clamp(power_scale, 0.85, 2.1)
	preview_time = 0.0
	_visual_time = 0.0
	queue_redraw()

func set_effect_time_scale(effect_time_scale: float) -> void:
	_effect_time_scale = max(0.1, effect_time_scale)

func set_elapsed(elapsed: float) -> void:
	preview_time = max(preview_time, elapsed)
	_visual_time = max(_visual_time, preview_time * PARTICLE_SPEED * _effect_time_scale)
	queue_redraw()

func finish() -> void:
	queue_free()

func _ready() -> void:
	z_index = 900
	_ensure_screen_transition_overlay()
	_visual_time = preview_time
	queue_redraw()

func _process(delta: float) -> void:
	if animate_in_editor:
		preview_time = fposmod(preview_time + delta * preview_speed, CHARGE_DURATION + BEAM_DURATION + 0.24)
		_visual_time += delta * preview_speed * PARTICLE_SPEED * _effect_time_scale
	else:
		preview_time += delta
		_visual_time = preview_time * PARTICLE_SPEED * _effect_time_scale
	queue_redraw()
	if preview_time > CHARGE_DURATION + BEAM_DURATION + 0.24:
		queue_free()

func _draw() -> void:
	var from: Vector2 = _from
	var to: Vector2 = _to
	var dir: Vector2 = to - from
	if dir.length_squared() < 1.0:
		dir = Vector2.RIGHT
	var forward: Vector2 = dir.normalized()
	var normal: Vector2 = forward.orthogonal()
	var beam_age: float = preview_time - CHARGE_DURATION
	var beam_t: float = clamp(beam_age / BEAM_DURATION, 0.0, 1.0)
	var charge_t: float = clamp(preview_time / CHARGE_DURATION, 0.0, 1.0)
	var fire: float = _beam_envelope(beam_age)
	var length_t: float = clamp(beam_age / 0.10, 0.0, 1.0)
	var glitch: float = _release_glitch_amount(beam_age)
	var target_distance: float = from.distance_to(to) + 720.0
	var launch_distance: float = target_distance * _ease_out_cubic(length_t)
	var drift_distance: float = max(0.0, beam_age - 0.10) * BEAM_DRIFT_SPEED * _effect_time_scale
	var current_to: Vector2 = from + forward * (launch_distance + drift_distance)
	var release_punch: float = 1.0 + 0.24 * (1.0 - smoothstep(0.015, 0.18, beam_age)) * smoothstep(0.0, 0.04, beam_age)
	var width_mul: float = fire * (1.0 - 0.08 * beam_t) * release_punch * (1.0 + glitch * 0.18)

	if show_guides:
		_draw_guides(from, to)

	_draw_charge_start_hit(from, forward, normal, preview_time)
	_draw_electric_charge(from, forward, normal, preview_time, beam_age)
	if show_drawn_charge:
		_draw_charge(from, charge_t)
	_draw_release_flash(from, forward, normal, beam_age)

	if fire <= 0.0:
		return

	var shadow: PackedVector2Array = _make_brush_polygon(from, current_to, body_width * 1.92 * power * width_mul, roughness * power, glitch)
	var shell: PackedVector2Array = _make_brush_polygon(from, current_to, body_width * 1.34 * power * width_mul, roughness * 0.86 * power, glitch)
	var mid: PackedVector2Array = _make_brush_polygon(from, current_to, body_width * 0.86 * power * width_mul, roughness * 0.54 * power, glitch)
	var inner: PackedVector2Array = _make_brush_polygon(from, current_to, core_width * 1.72 * power * width_mul, roughness * 0.24 * power, glitch)
	var hot: PackedVector2Array = _make_brush_polygon(from, current_to, core_width * 1.02 * power * width_mul, roughness * 0.10 * power, glitch)

	var hue: float = fposmod(hue_offset + _visual_time * rainbow_speed, 1.0)
	_draw_poly(shadow, _glitch_color(Color(0.02, 0.01, 0.05, fire * 0.70), glitch, 0.25))
	_draw_poly(shell, _glitch_color(Color.from_hsv(fposmod(hue + 0.00, 1.0), 0.95, 1.0, fire * 0.94), glitch, 0.90))
	_draw_poly(mid, _glitch_color(Color.from_hsv(fposmod(hue + 0.17, 1.0), 0.92, 1.0, fire * 0.92), glitch, 0.78))
	_draw_poly(inner, _glitch_color(Color.from_hsv(fposmod(hue + 0.36, 1.0), 0.64, 1.0, fire * 0.88), glitch, 0.62))
	_draw_poly(hot, Color(1.0, 1.0, 1.0, fire))
	if glitch > 0.01:
		_draw_glitch_slices(from, current_to, normal, fire, hue, glitch)

	_draw_flow_lines(from, current_to, normal, fire, hue, glitch)
	_draw_casting_core(from, forward, normal, fire, hue)
	if show_wave_head:
		_draw_wave_head(current_to, forward, normal, fire, hue, length_t)
	_draw_external_loop(from, forward, normal, beam_age, fire)
	_sync_screen_transition_overlay(from, to, beam_age)

func _draw_release_flash(from: Vector2, forward: Vector2, normal: Vector2, beam_age: float) -> void:
	if release_flash <= 0.0 or beam_age <= 0.0:
		return
	var flash: float = (1.0 - smoothstep(0.015, 0.095, beam_age)) * smoothstep(0.0, 0.028, beam_age) * release_flash
	if flash <= 0.01:
		return
	var center: Vector2 = from + forward * body_width * power * 0.20
	_draw_ellipse(center, forward, normal, body_width * power * 0.82, body_width * power * 0.54, Color(1.0, 1.0, 1.0, flash * 0.82), 36)
	draw_arc(center, body_width * power * (0.78 + flash * 0.26), 0.0, TAU, 72, Color(1.0, 1.0, 1.0, flash), 8.0)
	draw_line(from - forward * body_width * power * 0.18, from + forward * body_width * power * 1.35, Color(1.0, 1.0, 1.0, flash * 0.86), 7.0 * power)

func _draw_charge_start_hit(from: Vector2, forward: Vector2, _normal: Vector2, charge_age: float) -> void:
	if not show_charge_start_hit or charge_age < 0.0 or charge_age > 0.42:
		return
	var animation_age: float = min(charge_age * _effect_time_scale, 0.42)
	var frame: int = clampi(int(floor(animation_age / 0.42 * 22.0)), 0, 21)
	var col: int = frame % 6
	var row: int = floori(float(frame) / 6.0)
	var source: Rect2 = Rect2(Vector2(col * 512, row * 512), Vector2(512, 512))
	var fade: float = 1.0 - smoothstep(0.22, 0.42, charge_age)
	var scale_value: float = charge_start_hit_scale * power * (0.78 + smoothstep(0.0, 0.18, charge_age) * 0.28)
	var size: Vector2 = Vector2(512, 512) * scale_value
	var center: Vector2 = from + forward * body_width * power * 0.04
	draw_set_transform(center, forward.angle(), Vector2.ONE)
	draw_texture_rect_region(
		ENERGY_109_SHEET,
		Rect2(-size * 0.5, size),
		source,
		Color(1.0, 1.0, 1.0, charge_start_hit_alpha * fade)
	)
	draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)

func _draw_electric_charge(from: Vector2, forward: Vector2, _normal: Vector2, charge_age: float, beam_age: float) -> void:
	if not show_electric_charge or charge_age < 0.0:
		return
	var playback_time: float = fposmod(charge_age * 1.55 * _effect_time_scale, 1.42)
	var frame: int = clampi(int(floor(playback_time / 1.42 * 34.0)), 0, 33)
	var col: int = frame % 6
	var row: int = floori(float(frame) / 6.0)
	var source: Rect2 = Rect2(Vector2(col * 512, row * 512), Vector2(512, 512))
	var charge_t: float = clamp(charge_age / CHARGE_DURATION, 0.0, 1.0)
	var appear: float = smoothstep(0.0, 0.08, charge_age)
	var release_fade: float = 1.0 - smoothstep(0.0, 0.16, beam_age)
	var compress: float = lerp(1.12, 0.64, smoothstep(0.0, 1.0, charge_t))
	var pulse: float = 1.0 + sin(_visual_time * 16.0) * 0.035
	var size: Vector2 = Vector2(512, 512) * electric_charge_scale * power * compress * pulse
	var center: Vector2 = from + forward * body_width * power * 0.04
	draw_set_transform(center, forward.angle(), Vector2.ONE)
	draw_texture_rect_region(
		ELECTRICITY_016_SHEET,
		Rect2(-size * 0.5, size),
		source,
		Color(1.0, 1.0, 1.0, electric_charge_alpha * appear * release_fade)
	)
	draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)

func _draw_external_loop(from: Vector2, forward: Vector2, normal: Vector2, beam_age: float, fire: float) -> void:
	if not show_external_loop or beam_age < 0.0:
		return
	var frame: int = int(floor(beam_age * 60.0 * _effect_time_scale)) % 12
	var col: int = frame % 6
	var row: int = floori(float(frame) / 6.0)
	var source: Rect2 = Rect2(Vector2(col * 768, row * 768), Vector2(768, 768))
	var appear: float = smoothstep(0.0, 0.08, beam_age)
	var fade: float = smoothstep(0.0, 0.22, fire) * (1.0 - smoothstep(BEAM_HOLD_DURATION, BEAM_DURATION, beam_age))
	var scale_value: float = external_loop_scale * power * (0.92 + sin(_visual_time * 86.0) * 0.050)
	var size: Vector2 = Vector2(768, 768) * scale_value
	var center: Vector2 = (
		from
		+ forward * (body_width * power * 0.92 + external_loop_forward_offset)
		+ normal * (body_width * power * 0.02 + external_loop_normal_offset)
	)
	draw_set_transform(center, forward.angle(), Vector2.ONE)
	draw_texture_rect_region(
		ENERGY_091_SHEET,
		Rect2(-size * 0.5, size),
		source,
		Color(1.0, 1.0, 1.0, external_loop_alpha * appear * fade)
	)
	draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)

func _ensure_screen_transition_overlay() -> void:
	if _screen_transition_overlay != null and is_instance_valid(_screen_transition_overlay):
		return
	_screen_transition_overlay = SCREEN_TRANSITION_OVERLAY_SCRIPT.new()
	add_child(_screen_transition_overlay)

func _sync_screen_transition_overlay(from: Vector2, to: Vector2, beam_age: float) -> void:
	_ensure_screen_transition_overlay()
	_screen_transition_overlay.call("set_visual_state", beam_age, from, to, screen_transition_alpha, show_screen_transition)

func _release_glitch_amount(beam_age: float) -> float:
	if release_glitch <= 0.0 or beam_age <= 0.0:
		return 0.0
	var attack: float = smoothstep(0.0, 0.018, beam_age)
	var release: float = 1.0 - smoothstep(0.045, 0.16, beam_age)
	var strobe: float = 0.82 + 0.18 * sign(sin(_visual_time * 96.0))
	return clamp(attack * release * strobe * release_glitch, 0.0, 1.0)

func _glitch_color(color: Color, glitch: float, invert_bias: float) -> Color:
	if glitch <= 0.0:
		return color
	var inverted: Color = Color(1.0 - color.r, 1.0 - color.g, 1.0 - color.b, color.a)
	var cold_white: Color = Color(0.94, 1.0, 1.0, color.a)
	var flipped: Color = inverted.lerp(cold_white, 0.25 + invert_bias * 0.30)
	return color.lerp(flipped, clamp(glitch * (0.52 + invert_bias * 0.48), 0.0, 1.0))

func _draw_glitch_slices(from: Vector2, current_to: Vector2, normal: Vector2, alpha: float, hue: float, glitch: float) -> void:
	var dir: Vector2 = current_to - from
	if dir.length_squared() < 1.0:
		return
	var forward: Vector2 = dir.normalized()
	var span: float = body_width * power * (0.82 + glitch * 0.40)
	for i in range(7):
		var u: float = clamp(0.08 + float(i) * 0.12 + sin(_visual_time * 92.0 + float(i)) * 0.048, 0.0, 1.0)
		var center: Vector2 = from.lerp(current_to, u)
		var side: float = -1.0 if i % 2 == 0 else 1.0
		var shift: float = side * span * glitch * (0.22 + float(i % 3) * 0.10)
		var length: float = body_width * power * (0.16 + float(i % 2) * 0.07)
		var half: float = span * (0.38 + float(i % 4) * 0.08) * glitch
		var p1: Vector2 = center - forward * length + normal * (-half + shift)
		var p2: Vector2 = center + forward * length + normal * (-half * 0.60 + shift)
		var p3: Vector2 = center + forward * length * 0.74 + normal * (half + shift)
		var p4: Vector2 = center - forward * length * 0.88 + normal * (half * 0.68 + shift)
		var color: Color = Color.from_hsv(fposmod(hue + 0.50 + float(i) * 0.11, 1.0), 0.18, 1.0, alpha * glitch * 0.72)
		if i % 3 == 0:
			color = Color(0.02, 0.01, 0.04, alpha * glitch * 0.62)
		draw_polygon(PackedVector2Array([p1, p2, p3, p4]), PackedColorArray([color]))

func _draw_guides(from: Vector2, to: Vector2) -> void:
	draw_line(Vector2(0, 612), Vector2(1800, 612), Color(0.16, 0.18, 0.26, 0.8), 2.0)
	draw_line(from, to, Color(1, 1, 1, 0.16), 1.0)
	draw_circle(from, 5.0, Color(1, 1, 1, 0.5))

func _draw_charge(center: Vector2, charge_t: float) -> void:
	var alpha_visible: float = clamp(1.0 - max(0.0, preview_time - CHARGE_DURATION) / 0.12, 0.0, 1.0)
	var charge_ease: float = smoothstep(0.0, 1.0, charge_t)
	var alpha_ease: float = smoothstep(0.0, 0.12, alpha_visible)
	var pulse_rate: float = lerp(8.0, 18.0, charge_ease)
	var pulse: float = 1.0 + sin(_visual_time * pulse_rate) * lerp(0.035, 0.085, charge_ease)
	var compressed: float = lerp(1.0, 0.14, charge_ease)
	var hue: float = fposmod(hue_offset + _visual_time * rainbow_speed + 0.08, 1.0)
	var r_outer: float = body_width * 0.90 * power * 1.25 * pulse * compressed
	var r_inner: float = body_width * 0.90 * power * 0.52 * pulse * compressed
	var halo_alpha: float = alpha_visible * alpha_ease
	draw_arc(center, r_outer, 0.0, TAU, 72, Color.from_hsv(hue, 0.88, 1.0, halo_alpha), 9.0)
	draw_arc(center, r_outer * 0.76, 0.0, TAU, 64, Color.from_hsv(fposmod(hue + 0.18, 1.0), 0.70, 1.0, halo_alpha * 0.72), 5.0)
	draw_arc(center, r_inner, 0.0, TAU, 56, Color(1, 1, 1, halo_alpha), 4.0)

func _draw_flow_lines(from: Vector2, current_to: Vector2, normal: Vector2, alpha: float, hue: float, glitch: float) -> void:
	var span: float = body_width * power * 0.58
	var line_count: int = 24
	for i in range(line_count):
		var lane: float = (float(i) / float(line_count - 1) - 0.5) * 1.55
		var offset: float = lane * span
		var phase: float = fposmod(_visual_time * 11.2 + float(i) * 0.113, 1.0)
		for d in range(2):
			var u1: float = fposmod(phase + float(d) * 0.46, 1.0)
			var u2: float = u1 + 0.22
			var flow_alpha: float = alpha * smoothstep(0.0, 0.08, u1) * (1.0 - smoothstep(0.90, 1.0, u1))
			if flow_alpha <= 0.01:
				continue
			var color: Color = Color.from_hsv(fposmod(hue + float(i) * 0.05 + u1 * 0.52, 1.0), 0.86, 1.0, flow_alpha)
			if abs(lane) < 0.22:
				color = color.lerp(Color.WHITE, 0.62)
				color.a = flow_alpha
			var glitch_offset: float = sign(sin(u1 * 40.0 + float(i))) * body_width * power * 0.16 * glitch
			_draw_flow_segment(from, current_to, normal, offset + glitch_offset, u1, min(u2, 1.0), color, (2.0 + float(i % 3) + glitch * 2.0) * power)
			if u2 > 1.0:
				_draw_flow_segment(from, current_to, normal, offset - glitch_offset, 0.0, u2 - 1.0, color, (2.0 + float(i % 3) + glitch * 2.0) * power)

func _draw_flow_segment(
	from: Vector2,
	current_to: Vector2,
	normal: Vector2,
	offset: float,
	u1: float,
	u2: float,
	color: Color,
	width: float
) -> void:
	if u2 - u1 <= 0.01:
		return
	var points: PackedVector2Array = PackedVector2Array()
	for p in range(5):
		var u: float = lerp(u1, u2, float(p) / 4.0)
		var base: Vector2 = from.lerp(current_to, u)
		var wave: float = sin(u * 33.0 - _visual_time * 196.0 + offset * 0.013) * 10.0 * power
		points.append(base + normal * (offset + wave))
	draw_polyline(points, color, width)

func _draw_casting_core(from: Vector2, forward: Vector2, normal: Vector2, alpha: float, hue: float) -> void:
	var width: float = body_width * power
	var release: float = smoothstep(-0.08, 0.28, preview_time - CHARGE_DURATION)
	var center: Vector2 = from + forward * width * 0.18
	var stream: float = 0.92 + sin(_visual_time * 116.0) * 0.075
	var nozzle_len: float = width * (0.72 + release * 0.46) * stream
	var nozzle_rad: float = width * (0.52 + release * 0.12) * alpha

	_draw_ellipse(center + forward * width * 0.04, forward, normal, nozzle_len * 0.72, nozzle_rad * 0.92, Color.from_hsv(hue, 0.95, 1.0, alpha * 0.88), 36)
	_draw_ellipse(center + forward * width * 0.12, forward, normal, nozzle_len * 0.52, nozzle_rad * 0.62, Color.from_hsv(hue + 0.29, 0.86, 1.0, alpha * 0.92), 32)
	_draw_ellipse(center + forward * width * 0.22, forward, normal, nozzle_len * 0.34, nozzle_rad * 0.34, Color.from_hsv(hue + 0.58, 0.38, 1.0, alpha * 0.96), 28)
	_draw_ellipse(center + forward * width * 0.30, forward, normal, nozzle_len * 0.15, nozzle_rad * 0.16, Color(1.0, 1.0, 1.0, alpha), 22)

	for i in range(14):
		var side: float = -1.0 if i % 2 == 0 else 1.0
		var lane: float = (float(i % 7) / 6.0 - 0.5) * width * side
		var flow: float = fposmod(_visual_time * (18.0 + float(i) * 0.12) + float(i) * 0.11, 1.0)
		var start: Vector2 = center - forward * width * 0.22 + normal * lane * 0.14
		var mid: Vector2 = center + forward * width * (0.24 + flow * 0.18) + normal * lane * (0.24 + flow * 0.16)
		var end: Vector2 = center + forward * width * (0.76 + flow * 0.58) + normal * (lane * 0.62 + side * sin(_visual_time * 74.0 + float(i)) * width * 0.075)
		var ray_alpha: float = alpha * (1.0 - smoothstep(0.86, 1.0, flow)) * 0.78
		draw_polyline(PackedVector2Array([start, mid, end]), Color.from_hsv(hue + float(i) * 0.05, 0.88, 1.0, ray_alpha), 2.0 + float(i % 3))

func _draw_wave_head(current_to: Vector2, forward: Vector2, normal: Vector2, alpha: float, hue: float, length_t: float) -> void:
	var head_alpha: float = alpha * smoothstep(0.10, 0.84, length_t)
	if head_alpha <= 0.01:
		return
	var width: float = body_width * power
	var pulse: float = 1.0 + sin(_visual_time * 148.0) * 0.075
	var head_center: Vector2 = current_to + forward * width * 0.18
	var head_len: float = width * 1.10 * pulse * (0.70 + 0.30 * length_t)
	var head_rad: float = width * 0.82 * pulse * (1.0 - smoothstep(0.92, 1.0, length_t) * 0.20)
	var back: Vector2 = head_center - forward * head_len * 0.60
	var mid: Vector2 = head_center + forward * head_len * 0.12
	var front: Vector2 = head_center + forward * head_len * 0.92
	var shell: PackedVector2Array = PackedVector2Array([
		back + normal * head_rad * 0.68,
		mid + normal * head_rad * 1.02,
		front,
		mid - normal * head_rad * 1.02,
		back - normal * head_rad * 0.68,
	])
	draw_polygon(shell, PackedColorArray([Color.from_hsv(fposmod(hue + 0.15, 1.0), 0.82, 1.0, head_alpha * 0.72)]))
	_draw_ellipse(head_center + forward * head_len * 0.12, forward, normal, head_len * 0.34, head_rad * 0.34, Color(1.0, 1.0, 1.0, head_alpha), 24)
	for i in range(12):
		var angle: float = -1.10 + 2.20 * float(i) / 11.0
		var burst_dir: Vector2 = (forward * cos(angle) + normal * sin(angle)).normalized()
		var start: Vector2 = head_center - forward * width * 0.16
		var end: Vector2 = start + burst_dir * width * (0.46 + float(i % 4) * 0.08) * head_alpha
		var color: Color = Color.from_hsv(fposmod(hue + float(i) * 0.07, 1.0), 0.92, 1.0, head_alpha * 0.82)
		draw_line(start, end, color, (2.0 + float(i % 3)) * power)

func _draw_ellipse(center: Vector2, forward: Vector2, normal: Vector2, radius_forward: float, radius_normal: float, color: Color, count: int) -> void:
	if radius_forward <= 0.05 or radius_normal <= 0.05:
		return
	var points: PackedVector2Array = PackedVector2Array()
	for i in range(count):
		var angle: float = TAU * float(i) / float(count)
		points.append(center + forward * cos(angle) * radius_forward + normal * sin(angle) * radius_normal)
	draw_polygon(points, PackedColorArray([color]))

func _draw_poly(points: PackedVector2Array, color: Color) -> void:
	if points.size() >= 3:
		draw_polygon(points, PackedColorArray([color]))

func _make_brush_polygon(from: Vector2, current_to: Vector2, width: float, jag: float, glitch: float = 0.0) -> PackedVector2Array:
	var dir: Vector2 = current_to - from
	if dir.length_squared() < 1.0:
		dir = Vector2.RIGHT
	var normal: Vector2 = dir.normalized().orthogonal()
	var top: PackedVector2Array = PackedVector2Array()
	var bottom: Array[Vector2] = []
	for i in range(POINT_COUNT):
		var t: float = float(i) / float(POINT_COUNT - 1)
		var base: Vector2 = from.lerp(current_to, t)
		var taper: float = sin(t * PI)
		var head: float = 1.0 - smoothstep(0.88, 1.0, t) * 0.20
		var tail: float = smoothstep(0.0, 0.16, t)
		var noise: float = (
			sin(t * 17.0 - _visual_time * 236.0)
			+ sin(t * 29.0 - _visual_time * 308.0 + 1.7) * 0.82
			+ sin(t * 9.0 - _visual_time * 182.0 + 4.1) * 0.66
		) / 2.32
		var surge: float = sin(t * 5.0 * PI - _visual_time * 18.0) * sin(t * PI)
		var half: float = width * (0.20 + 0.80 * taper) * head * tail + (max(0.0, noise) * 1.28 + abs(surge) * 0.44) * jag
		var zigzag: float = sign(sin(t * 9.0 * PI + _visual_time * 284.0)) * jag * glitch * 0.72
		var tear: float = sin(t * 31.0 + _visual_time * 222.0) * jag * glitch * 0.26
		var glitch_shift: float = clamp(zigzag + tear, -half * 0.42, half * 0.42)
		var wobble: float = (sin(t * 21.0 + _visual_time * 132.0) * 0.72 + surge * 0.62) * jag * (0.54 + glitch * 0.24) * taper
		top.append(base + normal * (half + wobble + glitch_shift))
		bottom.append(base - normal * (half - wobble - glitch_shift * 0.68))
	for i in range(bottom.size() - 1, -1, -1):
		top.append(bottom[i])
	return top

func _beam_envelope(age: float) -> float:
	if age < 0.0:
		return 0.0
	if age < 0.045:
		return age / 0.045
	if age < 0.13:
		return 1.16 - (age - 0.045) / 0.085 * 0.18
	if age < BEAM_HOLD_DURATION:
		return 0.98
	var fade_t: float = clamp((age - BEAM_HOLD_DURATION) / max(BEAM_DURATION - BEAM_HOLD_DURATION, 0.01), 0.0, 1.0)
	return max(0.0, 0.98 * (1.0 - smoothstep(0.0, 1.0, fade_t)))

func _ease_out_cubic(value: float) -> float:
	var t: float = 1.0 - clamp(value, 0.0, 1.0)
	return 1.0 - t * t * t

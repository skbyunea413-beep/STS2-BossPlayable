@tool
extends Node2D

const TRANSITION_005_SHEET: Texture2D = preload("res://PrismMod/images/vfx/transition_v02_electricity_005/transition_v02_electricity_005_sheet_1024_6x4.png")
const SCREEN_TRANSITION_START: float = 0.10
const SCREEN_TRANSITION_DURATION: float = 0.80

var _beam_age: float = -1.0
var _from: Vector2 = Vector2.ZERO
var _to: Vector2 = Vector2.RIGHT
var _alpha: float = 0.82
var _enabled: bool = true

func _ready() -> void:
	z_index = 910
	var additive_material := CanvasItemMaterial.new()
	additive_material.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
	material = additive_material
	queue_redraw()

func set_visual_state(beam_age: float, from: Vector2, to: Vector2, alpha: float, enabled: bool) -> void:
	_beam_age = beam_age
	_from = from
	_to = to
	_alpha = alpha
	_enabled = enabled
	queue_redraw()

func _draw() -> void:
	if not _enabled or _beam_age < SCREEN_TRANSITION_START:
		return
	var transition_age: float = _beam_age - SCREEN_TRANSITION_START
	if transition_age < 0.0 or transition_age > SCREEN_TRANSITION_DURATION:
		return
	var dir: Vector2 = _to - _from
	if dir.length_squared() < 1.0:
		dir = Vector2.RIGHT
	var forward: Vector2 = dir.normalized()
	var normal: Vector2 = forward.orthogonal()
	var progress: float = clamp(transition_age / SCREEN_TRANSITION_DURATION, 0.0, 1.0)
	var frame: int = clampi(int(floor(progress * 23.0)), 0, 23)
	var col: int = frame % 6
	var row: int = floori(float(frame) / 6.0)
	var source: Rect2 = Rect2(Vector2(col * 1024, row * 576), Vector2(1024, 576))
	var appear: float = smoothstep(0.0, 0.08, progress)
	var fade: float = 1.0 - smoothstep(0.88, 1.0, progress)
	var flash: float = 1.0 - abs(progress - 0.94) / 0.13
	var flash_amount: float = clamp(flash, 0.0, 1.0)
	var draw_alpha: float = _alpha * appear * max(fade, flash_amount * 0.72)
	if draw_alpha <= 0.01:
		return
	var center: Vector2 = _from + forward * 720.0 + normal * 4.0
	var size: Vector2 = Vector2(2460.0, 1384.0) * (1.0 + flash_amount * 0.035)
	draw_set_transform(center, forward.angle(), Vector2.ONE)
	draw_texture_rect_region(
		TRANSITION_005_SHEET,
		Rect2(-size * 0.5, size),
		source,
		Color(1.0, 1.0, 1.0, draw_alpha)
	)
	if flash_amount > 0.0:
		draw_rect(Rect2(-size * 0.5, size), Color(1.0, 1.0, 1.0, flash_amount * 0.22), true)
	draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)

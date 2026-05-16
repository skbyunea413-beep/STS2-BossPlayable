extends SceneTree

var _viewport: SubViewport


func _initialize() -> void:
	call_deferred("_render_preview")


func _render_preview() -> void:
	_viewport = SubViewport.new()
	_viewport.size = Vector2i(860, 520)
	_viewport.render_target_update_mode = SubViewport.UPDATE_ALWAYS
	root.add_child(_viewport)

	var packed_scene: PackedScene = load("res://preview/ironclad_enemy_molten_fist_preview.tscn")
	var scene: Node = packed_scene.instantiate()
	scene.animate_in_editor = false
	scene.progress = 0.78
	_viewport.add_child(scene)

	await process_frame
	await process_frame

	var image := _viewport.get_texture().get_image()
	image.save_png("res://preview/ironclad_enemy_molten_fist_preview.png")
	quit()

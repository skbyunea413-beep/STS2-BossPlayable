import math
import random
import tkinter as tk
from dataclasses import dataclass
from tkinter import ttk


WIDTH = 1280
HEIGHT = 720
ORIGIN = (210, 380)
TARGET = (1510, 308)
TARGET_FPS = 60
FPS_MS = max(1, round(1000 / TARGET_FPS))
PREFIRE_DURATION = 0.34


@dataclass
class Particle:
    x: float
    y: float
    vx: float
    vy: float
    life: float
    max_life: float
    size: float
    color: str


def clamp(value, low, high):
    return max(low, min(high, value))


def lerp(a, b, t):
    return a + (b - a) * t


def smoothstep(edge0, edge1, x):
    t = clamp((x - edge0) / max(edge1 - edge0, 0.0001), 0.0, 1.0)
    return t * t * (3.0 - 2.0 * t)


def rgb_to_hex(r, g, b):
    return f"#{int(clamp(r, 0, 255)):02x}{int(clamp(g, 0, 255)):02x}{int(clamp(b, 0, 255)):02x}"


def mix_color(a, b, t):
    a = a.lstrip("#")
    b = b.lstrip("#")
    ar, ag, ab = int(a[0:2], 16), int(a[2:4], 16), int(a[4:6], 16)
    br, bg, bb = int(b[0:2], 16), int(b[2:4], 16), int(b[4:6], 16)
    return rgb_to_hex(lerp(ar, br, t), lerp(ag, bg, t), lerp(ab, bb, t))


def hsv_to_hex(h, s, v):
    h = h % 360.0
    c = v * s
    x = c * (1.0 - abs((h / 60.0) % 2.0 - 1.0))
    m = v - c
    if h < 60.0:
        r, g, b = c, x, 0.0
    elif h < 120.0:
        r, g, b = x, c, 0.0
    elif h < 180.0:
        r, g, b = 0.0, c, x
    elif h < 240.0:
        r, g, b = 0.0, x, c
    elif h < 300.0:
        r, g, b = x, 0.0, c
    else:
        r, g, b = c, 0.0, x
    return rgb_to_hex((r + m) * 255.0, (g + m) * 255.0, (b + m) * 255.0)


def base_head_offset(progress):
    return lerp(46.0, 18.0, smoothstep(0.35, 1.0, progress))


class KamehamehaSimulator:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("Custom Kamehameha Simulator")
        self.root.geometry(f"{WIDTH + 310}x{HEIGHT + 24}")
        self.root.configure(bg="#080914")

        self.canvas = tk.Canvas(
            self.root,
            width=WIDTH,
            height=HEIGHT,
            bg="#070814",
            highlightthickness=0,
        )
        self.canvas.pack(side=tk.LEFT, fill=tk.BOTH)

        self.panel = ttk.Frame(self.root, padding=14)
        self.panel.pack(side=tk.RIGHT, fill=tk.Y)

        self.t = 0.0
        self.phase_t = 0.0
        self.firing = False
        self.shake = 0.0
        self.particles = []
        self.rng = random.Random()
        self.beam_type = tk.StringVar(value="type1")
        self.type1_rainbow = tk.BooleanVar(value=False)

        self.vars = {
            "power": tk.DoubleVar(value=1.45),
            "beam_width": tk.DoubleVar(value=108),
            "turbulence": tk.DoubleVar(value=18),
            "charge_size": tk.DoubleVar(value=86),
            "speed": tk.DoubleVar(value=1.0),
            "impact": tk.DoubleVar(value=1.35),
            "rays": tk.DoubleVar(value=14),
            "hue": tk.DoubleVar(value=198),
        }

        self._build_controls()
        self._tick()

    def _build_controls(self):
        title = ttk.Label(self.panel, text="Custom Kamehameha", font=("Segoe UI", 16, "bold"))
        title.pack(anchor="w", pady=(0, 10))

        ttk.Button(self.panel, text="Fire", command=self.fire).pack(fill=tk.X, pady=(0, 8))
        ttk.Button(self.panel, text="Random Style", command=self.randomize).pack(fill=tk.X, pady=(0, 16))

        type_frame = ttk.LabelFrame(self.panel, text="Beam type", padding=8)
        type_frame.pack(fill=tk.X, pady=(0, 14))
        ttk.Radiobutton(type_frame, text="Type 1: Kamehameha", value="type1", variable=self.beam_type).pack(anchor="w")
        ttk.Radiobutton(type_frame, text="Type 2: Ultra Big Bang", value="type2", variable=self.beam_type).pack(anchor="w")
        ttk.Radiobutton(type_frame, text="Type 3: Angel Eye", value="type3", variable=self.beam_type).pack(anchor="w")
        ttk.Checkbutton(type_frame, text="Rainbow Type 1", variable=self.type1_rainbow).pack(anchor="w", pady=(6, 0))

        self._slider("power", "Power", 0.6, 2.4)
        self._slider("beam_width", "Beam mass", 34, 150)
        self._slider("turbulence", "Edge instability", 3, 46)
        self._slider("charge_size", "Charge sphere", 28, 135)
        self._slider("speed", "Release speed", 0.55, 1.8)
        self._slider("impact", "Screen-end bloom", 0.25, 2.2)
        self._slider("rays", "Flow density", 3, 22)
        self._slider("hue", "Color", 0, 360)

        note = ttk.Label(self.panel, text="Space: fire\nR: random\nEsc: quit", justify=tk.LEFT)
        note.pack(anchor="w", pady=(18, 0))

        self.root.bind("<space>", lambda _event: self.fire())
        self.root.bind("r", lambda _event: self.randomize())
        self.root.bind("<Escape>", lambda _event: self.root.destroy())

    def _slider(self, key, label, low, high):
        ttk.Label(self.panel, text=label).pack(anchor="w")
        ttk.Scale(
            self.panel,
            from_=low,
            to=high,
            variable=self.vars[key],
            orient=tk.HORIZONTAL,
            length=250,
        ).pack(anchor="w", pady=(0, 10))

    def fire(self):
        self.firing = True
        self.phase_t = -PREFIRE_DURATION
        shake_mul = 18.0 if self.beam_type.get() == "type2" else 11.0
        if self.beam_type.get() == "type3":
            shake_mul = 15.0
        self.shake = shake_mul * self.vars["power"].get()
        self.particles.clear()

    def randomize(self):
        self.vars["power"].set(self.rng.uniform(0.9, 2.2))
        self.vars["beam_width"].set(self.rng.uniform(72, 144))
        self.vars["turbulence"].set(self.rng.uniform(8, 34))
        self.vars["charge_size"].set(self.rng.uniform(52, 128))
        self.vars["speed"].set(self.rng.uniform(0.75, 1.45))
        self.vars["impact"].set(self.rng.uniform(0.75, 2.0))
        self.vars["rays"].set(self.rng.uniform(9, 21))
        self.vars["hue"].set(self.rng.uniform(178, 222))

    def _palette(self):
        hue = self.vars["hue"].get()
        rad = math.radians(hue)
        r = 86 + math.cos(rad) * 76
        g = 165 + math.cos(rad - 2.05) * 72
        b = 232 + math.cos(rad + 2.05) * 64
        edge = rgb_to_hex(r, g, b)
        glow = mix_color(edge, "#ffffff", 0.42)
        core = mix_color(edge, "#ffffff", 0.86)
        hot = mix_color(edge, "#fff28a", 0.66)
        return edge, glow, core, hot

    def _visible_endpoint(self):
        ox, oy = ORIGIN
        tx, ty = TARGET
        dx, dy = tx - ox, ty - oy
        if abs(dx) < 0.001:
            return TARGET
        t = (WIDTH + 165 - ox) / dx
        return ox + dx * t, oy + dy * t

    def _tick(self):
        self.t += 0.016
        speed = self.vars["speed"].get()
        self.phase_t += 0.016 * speed
        self.shake *= 0.90
        self._spawn_particles()
        self._update_particles()
        self._draw()
        self.root.after(FPS_MS, self._tick)

    def _spawn_particles(self):
        edge, glow, core, hot = self._palette()
        ox, oy = ORIGIN
        power = self.vars["power"].get()
        ray_count = int(self.vars["rays"].get())
        rainbow_type1 = self.beam_type.get() == "type1" and self.type1_rainbow.get()
        spawn_divisor = 5 if rainbow_type1 else 3
        burst_count = max(3, ray_count // 3) if rainbow_type1 else ray_count
        max_particles = 260 if rainbow_type1 else 650

        collapse_t = 0.0
        if self.firing and self.phase_t < 0.0:
            collapse_t = smoothstep(-PREFIRE_DURATION, 0.0, self.phase_t)

        for _ in range(1 + ray_count // spawn_divisor):
            angle = self.rng.uniform(0, math.tau)
            max_dist = self.vars["charge_size"].get() * lerp(1.45, 0.45, collapse_t)
            dist = self.rng.uniform(18, max_dist)
            inward = self.rng.uniform(45, 135) * (1.0 + collapse_t * 4.2 if self.firing else 1.0)
            self.particles.append(
                Particle(
                    ox + math.cos(angle) * dist,
                    oy + math.sin(angle) * dist,
                    -math.cos(angle) * inward,
                    -math.sin(angle) * inward,
                    self.rng.uniform(0.22, 0.55),
                    0.55,
                    self.rng.uniform(2.0, 5.0) * power,
                    self.rng.choice([edge, glow, core, hot]),
                )
            )

        if self.firing and self.phase_t > 0.18:
            tx, ty = self._visible_endpoint()
            for _ in range(burst_count):
                angle = self.rng.uniform(-1.35, 1.35)
                self.particles.append(
                    Particle(
                        tx + self.rng.uniform(-30, 34),
                        ty + self.rng.uniform(-54, 54),
                        math.cos(angle) * self.rng.uniform(95, 270),
                        math.sin(angle) * self.rng.uniform(95, 270),
                        self.rng.uniform(0.13, 0.38),
                        0.38,
                        self.rng.uniform(2.0, 7.0) * power,
                        self.rng.choice([edge, glow, core, hot]),
                    )
                )

        self.particles = self.particles[-max_particles:]

    def _update_particles(self):
        alive = []
        for p in self.particles:
            p.life -= 0.016
            p.x += p.vx * 0.016
            p.y += p.vy * 0.016
            p.vx *= 0.988
            p.vy *= 0.988
            if p.life > 0:
                alive.append(p)
        self.particles = alive

    def _draw(self):
        self.canvas.delete("all")
        sx = self.rng.uniform(-self.shake, self.shake)
        sy = self.rng.uniform(-self.shake, self.shake)
        edge, glow, core, hot = self._palette()

        self._draw_background(sx, sy)
        self._draw_charge(sx, sy, edge, glow, core)

        if self.firing:
            if self.beam_type.get() == "type2":
                self._draw_big_bang_beam(sx, sy)
            elif self.beam_type.get() == "type3":
                self._draw_angel_eye_laser(sx, sy)
            else:
                self._draw_beam(sx, sy, edge, glow, core, hot)
            if self.phase_t > 1.62:
                self.firing = False

        self._draw_particles(sx, sy)
        self._draw_pose_hint(sx, sy, glow, core)

    def _draw_background(self, sx, sy):
        star_count = 24 if self.beam_type.get() == "type1" and self.type1_rainbow.get() else 40
        for i in range(star_count):
            x = (i * 71 + math.sin(self.t + i) * 24) % WIDTH
            y = (i * 43 + math.cos(self.t * 0.7 + i) * 18) % HEIGHT
            self.canvas.create_oval(x - 1 + sx, y - 1 + sy, x + 1 + sx, y + 1 + sy, fill="#30364d", outline="")

        self.canvas.create_rectangle(0, HEIGHT - 122, WIDTH, HEIGHT, fill="#101222", outline="")
        self.canvas.create_line(0, HEIGHT - 122, WIDTH, HEIGHT - 122, fill="#222842", width=2)

    def _draw_charge(self, sx, sy, edge, glow, core):
        ox, oy = ORIGIN
        power = self.vars["power"].get()
        charge = self.vars["charge_size"].get() * power
        collapse_t = 0.0
        visibility = 1.0
        if self.firing:
            if self.phase_t < 0.0:
                collapse_t = smoothstep(-PREFIRE_DURATION, 0.0, self.phase_t)
            else:
                collapse_t = 1.0
                visibility = 1.0 - smoothstep(0.0, 0.18, self.phase_t)
        pulse = 1.0 + math.sin(self.t * lerp(18.0, 48.0, collapse_t)) * lerp(0.08, 0.19, collapse_t)
        compressed = lerp(1.0, 0.10, collapse_t)
        point_flash = smoothstep(0.78, 1.0, collapse_t)

        for i, scale in enumerate([1.25, 0.82, 0.42]):
            r = charge * scale * pulse * compressed
            color = mix_color("#070814", [edge, glow, core][i], visibility)
            self.canvas.create_oval(
                ox - r + sx,
                oy - r + sy,
                ox + r + sx,
                oy + r + sy,
                outline=color,
                width=max(2, int(lerp(8 - i * 2, 15 - i * 3, collapse_t))),
            )

        ray_count = int(self.vars["rays"].get())
        for i in range(ray_count + int(point_flash * 10)):
            if visibility <= 0.03:
                break
            angle = math.tau * i / max(1, ray_count) - self.t * (2.4 + i % 3 * 0.35)
            r1 = charge * lerp(0.16, 0.015, collapse_t)
            r2 = charge * lerp(1.13 + math.sin(self.t * 5 + i) * 0.18, 0.34 + math.sin(self.t * 12 + i) * 0.04, collapse_t)
            self.canvas.create_line(
                ox + math.cos(angle) * r2 + sx,
                oy + math.sin(angle) * r2 + sy,
                ox + math.cos(angle) * r1 + sx,
                oy + math.sin(angle) * r1 + sy,
                fill=mix_color("#070814", glow, visibility),
                width=2,
                capstyle=tk.ROUND,
            )

        if point_flash > 0.0 and visibility > 0.03:
            flash_r = charge * lerp(0.04, 0.16, point_flash)
            flash_color = mix_color("#070814", mix_color(core, "#ffffff", point_flash), visibility)
            self.canvas.create_oval(
                ox - flash_r + sx,
                oy - flash_r + sy,
                ox + flash_r + sx,
                oy + flash_r + sy,
                fill=flash_color,
                outline="",
            )

    def _draw_beam(self, sx, sy, edge, glow, core, hot):
        head_progress = clamp((self.phase_t + 0.08) / 0.42, 0.0, 1.0)
        progress = clamp((self.phase_t + 0.03) / 0.30, 0.0, 1.0)
        fade = 1.0 - clamp((self.phase_t - 1.17) / 0.42, 0.0, 1.0) ** 2
        if progress <= 0 or fade <= 0:
            return

        ox, oy = ORIGIN
        end_x, end_y = self._visible_endpoint()
        eased_progress = 1.0 - (1.0 - progress) ** 3
        tx = lerp(ORIGIN[0], end_x, eased_progress)
        ty = lerp(ORIGIN[1], end_y, eased_progress)
        dx, dy = tx - ox, ty - oy
        length = max(1.0, math.hypot(dx, dy))
        nx, ny = -dy / length, dx / length
        fx, fy = dx / length, dy / length
        head_x = tx + fx * base_head_offset(progress)
        head_y = ty + fy * base_head_offset(progress)
        release = smoothstep(-0.05, 0.28, self.phase_t)
        base_width = self.vars["beam_width"].get() * self.vars["power"].get() * fade * (0.72 + release * 0.28)
        turbulence = self.vars["turbulence"].get() * self.vars["power"].get() * 0.52

        rainbow = self.type1_rainbow.get()
        if rainbow:
            hue = self.vars["hue"].get()
            edge = hsv_to_hex(hue + self.t * 80.0, 0.95, 1.0)
            glow = hsv_to_hex(hue + 80.0 + self.t * 64.0, 0.78, 1.0)
            core = hsv_to_hex(hue + 165.0 + self.t * 46.0, 0.38, 1.0)
            hot = hsv_to_hex(hue + 245.0 + self.t * 58.0, 0.82, 1.0)

        self._draw_casting_core(ox, oy, nx, ny, fx, fy, sx, sy, edge, glow, core, fade, rainbow)

        if rainbow:
            layers = [
                (2.55, hsv_to_hex(self.vars["hue"].get() + 0.0 + self.t * 72.0, 0.95, 1.0), 0.18),
                (1.72, hsv_to_hex(self.vars["hue"].get() + 105.0 + self.t * 56.0, 0.86, 1.0), 0.24),
                (0.86, hsv_to_hex(self.vars["hue"].get() + 210.0 + self.t * 44.0, 0.58, 1.0), 0.10),
                (0.34, "#ffffff", 0.04),
            ]
        else:
            layers = [
                (2.55, edge, 0.18),
                (1.72, glow, 0.24),
                (0.86, core, 0.10),
                (0.34, "#ffffff", 0.04),
            ]
        for scale, color, jitter_mul in layers:
            points_top = []
            points_bottom = []
            segments = 42
            for i in range(segments + 1):
                u = i / segments
                muzzle = 0.46 + 0.54 * smoothstep(0.0, 0.18, u)
                head = 1.0 - smoothstep(0.88, 1.0, u) * 0.20
                flow_phase = self.t * 58.0 - u * 36.0
                wobble = math.sin(flow_phase) * turbulence * jitter_mul
                scratch = math.sin(flow_phase * 1.41 + 2.0) * turbulence * jitter_mul * 0.32
                chunk_gate = smoothstep(0.08, 0.24, u) * (1.0 - smoothstep(0.94, 1.0, u))
                chunk_noise = (
                    math.sin(u * 24.0 - self.t * 22.0 + scale * 0.9)
                    + math.sin(u * 39.0 - self.t * 31.0 + scale * 2.3) * 0.55
                    + math.sin(u * 13.0 - self.t * 17.0 + scale * 4.1) * 0.42
                ) / 1.97
                chunk = max(0.0, scale - 0.7) * turbulence * 0.86 * chunk_noise * chunk_gate
                half = max(base_width * scale * muzzle * head * 0.38, base_width * scale * muzzle * head * 0.5 + chunk)
                bx = lerp(ox, tx, u)
                by = lerp(oy, ty, u)
                points_top.extend([bx + nx * (half + wobble + scratch) + sx, by + ny * (half + wobble + scratch) + sy])
                points_bottom[0:0] = [bx - nx * (half + wobble - scratch) + sx, by - ny * (half + wobble - scratch) + sy]
            self.canvas.create_polygon(*(points_top + points_bottom), fill=color, outline="")

        self._draw_flow_bands(ox, oy, tx, ty, nx, ny, sx, sy, edge, hot, core, rainbow)
        self._draw_wave_head(head_x, head_y, fx, fy, nx, ny, sx, sy, edge, glow, core, hot, fade, head_progress)
        if head_progress >= 1.0:
            self._draw_impact(tx, ty, fx, fy, nx, ny, sx, sy, edge, glow, core, fade)

    def _draw_big_bang_beam(self, sx, sy):
        edge = "#15b7ff"
        deep = "#1762d7"
        aqua = "#75fff0"
        core = "#ffffff"
        hot = "#ff9b38"

        progress = clamp((self.phase_t + 0.02) / 0.24, 0.0, 1.0)
        fade = 1.0 - clamp((self.phase_t - 1.08) / 0.48, 0.0, 1.0) ** 2
        if progress <= 0.0 or fade <= 0.0:
            return

        ox, oy = ORIGIN
        end_x, end_y = self._visible_endpoint()
        eased = 1.0 - (1.0 - progress) ** 3
        tx = lerp(ox, end_x + 260, eased)
        ty = lerp(oy, end_y - 26, eased)
        dx, dy = tx - ox, ty - oy
        length = max(1.0, math.hypot(dx, dy))
        fx, fy = dx / length, dy / length
        nx, ny = -fy, fx

        power = self.vars["power"].get()
        beam_mass = self.vars["beam_width"].get() * power
        release = smoothstep(-0.06, 0.22, self.phase_t)
        core_width = beam_mass * (1.05 + release * 0.72) * fade
        shell_width = beam_mass * (2.15 + release * 1.18) * fade

        self._draw_big_bang_gate(ox, oy, fx, fy, nx, ny, sx, sy, edge, aqua, core, fade, release)
        self._draw_big_bang_body(ox, oy, tx, ty, fx, fy, nx, ny, sx, sy, shell_width, core_width, edge, deep, aqua, core)
        self._draw_big_bang_shards(ox, oy, tx, ty, fx, fy, nx, ny, sx, sy, edge, deep, aqua, hot, fade)
        self._draw_big_bang_front(tx, ty, fx, fy, nx, ny, sx, sy, edge, aqua, core, hot, fade)

    def _draw_big_bang_gate(self, ox, oy, fx, fy, nx, ny, sx, sy, edge, aqua, core, fade, release):
        width = self.vars["beam_width"].get() * self.vars["power"].get()
        cx = ox + fx * width * 0.62
        cy = oy + fy * width * 0.62
        pulse = 1.0 + math.sin(self.t * 34.0) * 0.08

        for scale, color, alpha in [(1.55, edge, 0.62), (1.06, aqua, 0.76), (0.58, core, 0.92)]:
            rx = width * scale * (0.78 + release * 0.35) * fade * pulse
            ry = width * scale * 1.72 * (0.78 + release * 0.35) * fade * pulse
            fill = mix_color("#070814", color, alpha)
            self.canvas.create_oval(cx - rx + sx, cy - ry + sy, cx + rx + sx, cy + ry + sy, fill=fill, outline="")

        for i in range(18):
            side = -1 if i % 2 else 1
            spread = width * (0.72 + (i % 6) * 0.17) * side
            start_x = cx - fx * width * 0.24 + nx * spread * 0.18
            start_y = cy - fy * width * 0.24 + ny * spread * 0.18
            end_x = cx + fx * width * (0.82 + 0.08 * (i % 4)) + nx * spread
            end_y = cy + fy * width * (0.82 + 0.08 * (i % 4)) + ny * spread
            color = aqua if i % 3 else edge
            self.canvas.create_line(start_x + sx, start_y + sy, end_x + sx, end_y + sy, fill=color, width=3, capstyle=tk.ROUND)

    def _draw_big_bang_body(self, ox, oy, tx, ty, fx, fy, nx, ny, sx, sy, shell_width, core_width, edge, deep, aqua, core):
        layers = [
            (shell_width * 1.22, deep, 0.92, 0.95),
            (shell_width * 0.94, edge, 0.74, 0.70),
            (shell_width * 0.58, aqua, 0.48, 0.42),
            (core_width, core, 0.22, 0.18),
        ]
        for width, color, wobble_mul, chip_mul in layers:
            top = []
            bottom = []
            segments = 48
            for i in range(segments + 1):
                u = i / segments
                throat = 0.28 + 0.72 * smoothstep(0.0, 0.18, u)
                blast = 1.0 + smoothstep(0.58, 1.0, u) * 0.46
                lump = (
                    math.sin(u * 17.0 - self.t * 42.0)
                    + math.sin(u * 29.0 - self.t * 57.0 + 1.7) * 0.65
                    + math.sin(u * 9.0 - self.t * 31.0 + 4.1) * 0.48
                ) / 2.13
                chips = max(0.0, lump) * width * chip_mul
                half = width * throat * blast * 0.5 + chips
                shear = math.sin(u * 13.0 - self.t * 24.0) * width * 0.10 * wobble_mul
                bx = lerp(ox, tx, u)
                by = lerp(oy, ty, u)
                top.extend([bx + nx * (half + shear) + sx, by + ny * (half + shear) + sy])
                bottom[0:0] = [bx - nx * (half - shear) + sx, by - ny * (half - shear) + sy]
            self.canvas.create_polygon(*(top + bottom), fill=color, outline="")

    def _draw_big_bang_shards(self, ox, oy, tx, ty, fx, fy, nx, ny, sx, sy, edge, deep, aqua, hot, fade):
        density = int(self.vars["rays"].get()) + 10
        width = self.vars["beam_width"].get() * self.vars["power"].get()
        for i in range(density):
            u = (self.t * (2.6 + i * 0.04) + i * 0.071) % 1.0
            side = -1 if i % 2 else 1
            lane = side * width * (0.55 + (i % 7) * 0.18)
            streak_len = width * (0.62 + (i % 5) * 0.20)
            base_x = lerp(ox, tx, u)
            base_y = lerp(oy, ty, u)
            drift = math.sin(self.t * 14.0 + i) * width * 0.08
            x1 = base_x + nx * (lane + drift)
            y1 = base_y + ny * (lane + drift)
            x2 = x1 + fx * streak_len + nx * side * width * 0.18
            y2 = y1 + fy * streak_len + ny * side * width * 0.18
            color = hot if i % 8 == 0 else ([edge, deep, aqua][i % 3])
            self.canvas.create_line(x1 + sx, y1 + sy, x2 + sx, y2 + sy, fill=color, width=2 + i % 4, capstyle=tk.ROUND)

        for i in range(18):
            u = (self.t * 1.9 + i * 0.137) % 1.0
            side = -1 if i % 2 else 1
            bx = lerp(ox, tx, u)
            by = lerp(oy, ty, u)
            offset = side * width * (1.0 + (i % 5) * 0.32)
            size = width * (0.09 + (i % 4) * 0.035) * fade
            self.canvas.create_polygon(
                bx + nx * offset + sx,
                by + ny * offset + sy,
                bx + fx * size * 1.8 + nx * (offset + side * size) + sx,
                by + fy * size * 1.8 + ny * (offset + side * size) + sy,
                bx + fx * size * 0.4 + nx * (offset - side * size * 0.7) + sx,
                by + fy * size * 0.4 + ny * (offset - side * size * 0.7) + sy,
                fill="#06101c",
                outline="",
            )

    def _draw_big_bang_front(self, tx, ty, fx, fy, nx, ny, sx, sy, edge, aqua, core, hot, fade):
        width = self.vars["beam_width"].get() * self.vars["power"].get()
        pulse = 1.0 + math.sin(self.t * 48.0) * 0.08
        rx = width * 1.05 * fade * pulse
        ry = width * 2.05 * fade * pulse
        self.canvas.create_oval(tx - rx + sx, ty - ry + sy, tx + rx + sx, ty + ry + sy, fill=aqua, outline="")
        self.canvas.create_oval(tx - rx * 0.52 + sx, ty - ry * 0.52 + sy, tx + rx * 0.52 + sx, ty + ry * 0.52 + sy, fill=core, outline="")

        for i in range(26):
            angle = -1.35 + 2.7 * i / 25
            bx = fx * math.cos(angle) + nx * math.sin(angle)
            by = fy * math.cos(angle) + ny * math.sin(angle)
            length = width * (0.8 + (i % 6) * 0.22) * fade
            color = hot if i % 5 == 0 else edge
            self.canvas.create_line(tx + sx, ty + sy, tx + bx * length + sx, ty + by * length + sy, fill=color, width=2 + i % 3, capstyle=tk.ROUND)

    def _draw_angel_eye_laser(self, sx, sy):
        ox, oy = ORIGIN
        target = (920.0, HEIGHT - 122.0)
        dx, dy = target[0] - ox, target[1] - oy
        length = max(1.0, math.hypot(dx, dy))
        fx, fy = dx / length, dy / length
        nx, ny = -fy, fx

        charge = smoothstep(-PREFIRE_DURATION, -0.02, self.phase_t)
        laser_t = smoothstep(-0.03, 0.02, self.phase_t) * (1.0 - smoothstep(0.10, 0.16, self.phase_t))
        blast_t = smoothstep(0.25, 0.33, self.phase_t)
        fade = 1.0 - smoothstep(0.72, 1.02, self.phase_t)
        if fade <= 0.0:
            return

        eye_x = ox + fx * 30.0
        eye_y = oy + fy * 30.0
        self._draw_angel_eye_flash(eye_x, eye_y, sx, sy, charge, laser_t, fade)

        if laser_t > 0.0:
            self._draw_angel_laser_line(eye_x, eye_y, target[0], target[1], nx, ny, sx, sy, laser_t, fade)
        self._draw_ground_rainbow_blast(target[0], target[1], sx, sy, blast_t, fade)

    def _rainbow_color(self, index, count, value=1.0):
        hue_offset = self.vars["hue"].get()
        return hsv_to_hex(hue_offset + 360.0 * index / max(1, count), 0.92, value)

    def _draw_angel_eye_flash(self, x, y, sx, sy, charge, laser_t, fade):
        flash = max(charge, laser_t)
        pulse = 1.0 + math.sin(self.t * 58.0) * 0.12
        base = self.vars["charge_size"].get() * self.vars["power"].get()
        outer = base * (0.18 + flash * 0.46) * pulse * fade
        inner = base * (0.08 + laser_t * 0.20) * pulse * fade
        self.canvas.create_oval(x - outer + sx, y - outer + sy, x + outer + sx, y + outer + sy, fill=mix_color("#070814", "#f5fbff", 0.72 * flash), outline="")
        self.canvas.create_oval(x - inner + sx, y - inner + sy, x + inner + sx, y + inner + sy, fill="#ffffff", outline="")

        for i in range(8):
            angle = i * math.tau / 8.0 + self.t * 0.8
            r1 = inner * 0.6
            r2 = outer * (1.2 + 0.18 * math.sin(self.t * 7.0 + i))
            self.canvas.create_line(
                x + math.cos(angle) * r1 + sx,
                y + math.sin(angle) * r1 + sy,
                x + math.cos(angle) * r2 + sx,
                y + math.sin(angle) * r2 + sy,
                fill="#eafcff",
                width=2,
                capstyle=tk.ROUND,
            )

    def _draw_angel_laser_line(self, x1, y1, x2, y2, nx, ny, sx, sy, laser_t, fade):
        power = self.vars["power"].get()
        flash_alpha = laser_t * fade
        width = self.vars["beam_width"].get() * 0.11 * power * (0.35 + laser_t * 0.75) * flash_alpha
        jitter = self.vars["turbulence"].get() * 0.12
        for i, scale in enumerate([4.8, 2.2, 0.8]):
            pts = []
            segments = 12
            for s in range(segments + 1):
                u = s / segments
                wobble = math.sin(u * 40.0 - self.t * 72.0 + i) * jitter * (1.0 if i == 0 else 0.35)
                px = lerp(x1, x2, u) + nx * wobble
                py = lerp(y1, y2, u) + ny * wobble
                pts.extend([px + sx, py + sy])
            color = ["#62fff5", "#dfffff", "#ffffff"][i]
            self.canvas.create_line(*pts, fill=color, width=max(1, int(width * scale)), capstyle=tk.ROUND, smooth=True)

    def _draw_rainbow_impact(self, tx, ty, fx, fy, nx, ny, sx, sy, blast_t, fade):
        if blast_t <= 0.0:
            return

        power = self.vars["power"].get()
        impact = self.vars["impact"].get()
        radius = (38.0 + blast_t * 210.0) * power * impact
        alpha = fade * (1.0 - smoothstep(0.72, 1.0, blast_t) * 0.45)

        for i in range(18):
            color = self._rainbow_color(i, 18, 1.0)
            angle1 = math.tau * i / 18.0 + self.t * 1.7
            angle2 = math.tau * (i + 0.62) / 18.0 + self.t * 1.7
            r_inner = radius * (0.10 + 0.08 * math.sin(self.t * 5.0 + i))
            r_outer = radius * (0.72 + 0.24 * math.sin(self.t * 9.0 + i * 1.3))
            points = [
                tx + math.cos(angle1) * r_inner + sx,
                ty + math.sin(angle1) * r_inner + sy,
                tx + math.cos(angle1) * r_outer + sx,
                ty + math.sin(angle1) * r_outer + sy,
                tx + math.cos(angle2) * r_outer * 0.92 + sx,
                ty + math.sin(angle2) * r_outer * 0.92 + sy,
                tx + math.cos(angle2) * r_inner + sx,
                ty + math.sin(angle2) * r_inner + sy,
            ]
            self.canvas.create_polygon(*points, fill=mix_color("#070814", color, alpha), outline="")

        white_r = radius * (0.28 + 0.12 * math.sin(self.t * 20.0))
        self.canvas.create_oval(tx - white_r + sx, ty - white_r + sy, tx + white_r + sx, ty + white_r + sy, fill=mix_color("#070814", "#ffffff", alpha), outline="")

        ray_count = int(self.vars["rays"].get()) + 14
        for i in range(ray_count):
            angle = math.tau * i / ray_count + self.t * (0.8 + (i % 4) * 0.08)
            length = radius * (0.72 + (i % 5) * 0.18)
            start = radius * 0.22
            color = self._rainbow_color(i, ray_count, 1.0)
            self.canvas.create_line(
                tx + math.cos(angle) * start + sx,
                ty + math.sin(angle) * start + sy,
                tx + math.cos(angle) * length + sx,
                ty + math.sin(angle) * length + sy,
                fill=color,
                width=2 + i % 3,
                capstyle=tk.ROUND,
            )

        for i in range(8):
            bolt_angle = math.tau * i / 8.0 + math.sin(self.t * 6.0 + i) * 0.2
            end = radius * (0.55 + 0.13 * (i % 4))
            mid = radius * (0.24 + 0.08 * (i % 3))
            color = self._rainbow_color(i + 3, 8, 1.0)
            self.canvas.create_line(
                tx + sx,
                ty + sy,
                tx + math.cos(bolt_angle + 0.16) * mid + sx,
                ty + math.sin(bolt_angle + 0.16) * mid + sy,
                tx + math.cos(bolt_angle) * end + sx,
                ty + math.sin(bolt_angle) * end + sy,
                fill=color,
                width=3,
                capstyle=tk.ROUND,
                smooth=True,
            )

    def _draw_ground_rainbow_blast(self, gx, gy, sx, sy, blast_t, fade):
        if blast_t <= 0.0:
            return

        power = self.vars["power"].get()
        impact = self.vars["impact"].get()
        width = self.vars["beam_width"].get() * power * impact
        burst_age = max(0.0, self.phase_t - 0.25)
        burst_fade = 1.0 - smoothstep(0.36, 0.62, burst_age)
        alpha = fade * burst_fade
        shoot = 1.0 - (1.0 - blast_t) ** 4
        height = (90.0 + shoot * 980.0) * power * impact
        spread = (24.0 + shoot * 155.0) * power * impact

        # Ground flash first: a hot horizontal scar that sells the impact point.
        scar_w = spread * (1.0 + math.sin(self.t * 18.0) * 0.08)
        scar_h = 12.0 + blast_t * 18.0
        self.canvas.create_oval(
            gx - scar_w + sx,
            gy - scar_h + sy,
            gx + scar_w + sx,
            gy + scar_h + sy,
            fill=mix_color("#070814", "#ffffff", alpha),
            outline="",
        )

        column_count = int(self.vars["rays"].get()) + 18
        for i in range(column_count):
            lane = (i / max(1, column_count - 1) - 0.5) * 2.0
            local = clamp(burst_age * (4.6 + i * 0.09) - (i % 5) * 0.035, 0.0, 1.0)
            local_fade = 1.0 - smoothstep(0.58, 1.0, local)
            col_w = width * (0.035 + (i % 5) * 0.014) * (1.0 + shoot * 0.55) * local_fade
            col_h = height * (0.46 + (i % 7) * 0.08) * (0.42 + local * 0.58)
            bend = math.sin(self.t * 18.0 + i) * spread * 0.05
            base_x = gx + lane * spread * (0.25 + blast_t * 0.65) + bend
            top_x = base_x + math.sin(self.t * 10.0 + i * 2.1) * spread * 0.10
            color = self._rainbow_color(i, column_count, 1.0)
            bright = mix_color("#070814", color, alpha * local_fade)
            top_y = gy - col_h
            points = [
                base_x - col_w + sx,
                gy + sy,
                base_x + col_w + sx,
                gy + sy,
                top_x + col_w * 0.40 + sx,
                top_y + sy,
                top_x - col_w * 0.40 + sx,
                top_y + sy,
            ]
            self.canvas.create_polygon(*points, fill=bright, outline="")

            if i % 3 == 0:
                self.canvas.create_line(
                    base_x + sx,
                    gy + sy,
                    top_x + sx,
                    top_y - col_h * 0.16 + sy,
                    fill=mix_color("#070814", "#ffffff", alpha),
                    width=2,
                    capstyle=tk.ROUND,
                )

        core_w = spread * 0.28 * (1.0 - smoothstep(0.42, 0.72, burst_age) * 0.55)
        core_h = height * 0.98
        self.canvas.create_polygon(
            gx - core_w + sx,
            gy + sy,
            gx + core_w + sx,
            gy + sy,
            gx + core_w * 0.18 + sx,
            gy - core_h + sy,
            gx - core_w * 0.18 + sx,
            gy - core_h + sy,
            fill=mix_color("#070814", "#ffffff", alpha),
            outline="",
        )

        for i in range(22):
            side = -1 if i % 2 else 1
            color = self._rainbow_color(i + 4, 16, 1.0)
            start_x = gx + side * spread * (0.12 + (i % 5) * 0.07)
            start_y = gy - height * (0.02 + (i % 4) * 0.035)
            end_x = start_x + side * spread * (0.34 + (i % 3) * 0.10)
            end_y = gy + 10.0 + (i % 4) * 6.0
            self.canvas.create_line(
                start_x + sx,
                start_y + sy,
                end_x + sx,
                end_y + sy,
                fill=mix_color("#070814", color, alpha),
                width=2 + i % 3,
                capstyle=tk.ROUND,
            )

        for i in range(12):
            dust_x = gx + (i - 4.5) * spread * 0.18
            dust_y = gy + 8.0 + (i % 3) * 5.0
            dust_r = (8.0 + shoot * 30.0) * (0.7 + (i % 4) * 0.12)
            self.canvas.create_oval(
                dust_x - dust_r + sx,
                dust_y - dust_r * 0.45 + sy,
                dust_x + dust_r + sx,
                dust_y + dust_r * 0.45 + sy,
                fill=mix_color("#101222", "#6f7580", alpha * 0.55),
                outline="",
            )

    def _draw_casting_core(self, ox, oy, nx, ny, fx, fy, sx, sy, edge, glow, core, fade, rainbow=False):
        width = self.vars["beam_width"].get() * self.vars["power"].get()
        release = smoothstep(-0.08, 0.28, self.phase_t)
        breathe = 1.0 + math.sin(self.t * 36.0) * 0.09 + math.sin(self.t * 61.0) * 0.035
        cx = ox + fx * width * 0.18
        cy = oy + fy * width * 0.18

        if rainbow:
            hue = self.vars["hue"].get()
            ring_layers = [
                (0.72, hsv_to_hex(hue + 0.0 + self.t * 72.0, 0.95, 1.0), 1.0),
                (0.52, hsv_to_hex(hue + 105.0 + self.t * 56.0, 0.86, 1.0), 1.0),
                (0.32, hsv_to_hex(hue + 210.0 + self.t * 44.0, 0.58, 1.0), 1.0),
                (0.16, "#ffffff", 1.0),
            ]
            for scale, color, alpha in ring_layers:
                r = width * scale * fade * breathe * (0.86 + release * 0.18)
                fill = color if alpha >= 1.0 else mix_color("#070814", color, alpha)
                self.canvas.create_oval(cx - r + sx, cy - r + sy, cx + r + sx, cy + r + sy, fill=fill, outline="")

            for i in range(7):
                angle = self.t * (4.8 + i * 0.12) + i * math.tau / 7.0
                color = hsv_to_hex(hue + i * 52.0 + self.t * 70.0, 0.88, 1.0)
                inner = width * 0.24
                outer = width * (0.50 + 0.08 * math.sin(self.t * 9.0 + i))
                x1 = cx + math.cos(angle) * inner - fx * width * 0.10
                y1 = cy + math.sin(angle) * inner - fy * width * 0.10
                x2 = cx + math.cos(angle + 0.22) * outer + fx * width * 0.32
                y2 = cy + math.sin(angle + 0.22) * outer + fy * width * 0.32
                self.canvas.create_line(x1 + sx, y1 + sy, x2 + sx, y2 + sy, fill=color, width=3, capstyle=tk.ROUND)
            return

        layers = [
            (0.72, edge, 0.55),
            (0.50, glow, 0.76),
            (0.28, core, 0.95),
            (0.13, "#ffffff", 1.0),
        ]
        for scale, color, alpha in layers:
            r = width * scale * fade * breathe * (0.86 + release * 0.18)
            fill = mix_color("#070814", color, alpha)
            self.canvas.create_oval(cx - r + sx, cy - r + sy, cx + r + sx, cy + r + sy, fill=fill, outline="")

        for i in range(9):
            angle = self.t * (3.8 + i * 0.17) + i * math.tau / 9.0
            inner = width * (0.18 + 0.04 * math.sin(self.t * 7.0 + i))
            outer = width * (0.46 + 0.09 * math.sin(self.t * 11.0 + i * 1.7))
            x1 = cx + math.cos(angle) * inner
            y1 = cy + math.sin(angle) * inner
            x2 = cx + math.cos(angle + 0.18) * outer
            y2 = cy + math.sin(angle + 0.18) * outer
            self.canvas.create_line(x1 + sx, y1 + sy, x2 + sx, y2 + sy, fill=glow if i % 2 else core, width=2, capstyle=tk.ROUND)

    def _draw_muzzle_burst(self, ox, oy, nx, ny, fx, fy, sx, sy, edge, glow, core, fade):
        width = self.vars["beam_width"].get() * self.vars["power"].get()
        breathe = 1.0 + math.sin(self.t * 42.0) * 0.06
        release = smoothstep(-0.05, 0.30, self.phase_t)
        for i, radius in enumerate([1.48, 0.92, 0.44]):
            r = width * radius * fade * breathe * (0.72 + release * 0.28)
            color = [edge, glow, core][i]
            self.canvas.create_oval(ox - r + sx, oy - r + sy, ox + r + sx, oy + r + sy, outline=color, width=max(3, 10 - i * 3))

        for i in range(7):
            lane = (i - 3) / 3.0
            flow = (self.t * (7.0 + i * 0.43) + i * 0.19) % 1.0
            length = width * (0.72 + flow * 1.08) * fade
            root_half = width * (0.34 + 0.22 * math.sin(self.t * 18.0 + i)) * fade
            tip_half = width * (0.06 + 0.08 * (1.0 - flow)) * fade
            offset = lane * width * 0.18 * (1.0 - flow * 0.45)
            start_x = ox - fx * (18 + flow * 24) + nx * offset
            start_y = oy - fy * (18 + flow * 24) + ny * offset
            end_x = ox + fx * length + nx * offset * 0.28
            end_y = oy + fy * length + ny * offset * 0.28
            color = [edge, glow, core][i % 3]
            color = mix_color("#070814", color, 0.56 + 0.34 * (1.0 - flow))
            wedge = [
                start_x + nx * root_half + sx,
                start_y + ny * root_half + sy,
                end_x + nx * tip_half + sx,
                end_y + ny * tip_half + sy,
                end_x - nx * tip_half + sx,
                end_y - ny * tip_half + sy,
                start_x - nx * root_half + sx,
                start_y - ny * root_half + sy,
            ]
            self.canvas.create_polygon(*wedge, fill=color, outline="")

    def _draw_origin_wave(self, ox, oy, nx, ny, fx, fy, sx, sy, edge, glow, core):
        power = self.vars["power"].get()
        width = self.vars["beam_width"].get() * power
        local_time = self.phase_t + 0.04
        for i in range(3):
            age = local_time - i * 0.075
            if age < 0.0 or age > 0.34:
                continue
            life = age / 0.34
            alpha = 1.0 - life
            radius_x = (20 + life * 86) * power
            radius_y = (9 + life * 34) * power
            cx = ox + fx * (16 + life * 42)
            cy = oy + fy * (16 + life * 42)
            color = mix_color("#070814", glow if i % 2 else core, alpha * 0.70)
            self.canvas.create_oval(
                cx - radius_x + sx,
                cy - radius_y + sy,
                cx + radius_x + sx,
                cy + radius_y + sy,
                outline=color,
                width=max(1, int(4 * alpha)),
            )

        for i in range(10):
            u = ((self.t * 5.5) + i * 0.13) % 1.0
            spread = width * (0.22 + u * 0.82)
            start = ox + fx * (6 + u * 58)
            start_y = oy + fy * (6 + u * 58)
            side = -1 if i % 2 else 1
            x1 = start + nx * side * spread * 0.25
            y1 = start_y + ny * side * spread * 0.25
            x2 = ox + fx * (45 + u * 95) + nx * side * spread
            y2 = oy + fy * (45 + u * 95) + ny * side * spread
            color = glow if i % 3 else edge
            self.canvas.create_line(x1 + sx, y1 + sy, x2 + sx, y2 + sy, fill=color, width=2, capstyle=tk.ROUND)

    def _draw_wave_head(self, hx, hy, fx, fy, nx, ny, sx, sy, edge, glow, core, hot, fade, head_progress):
        if head_progress <= 0.0:
            return

        width = self.vars["beam_width"].get() * self.vars["power"].get()
        head_alpha = fade * (1.0 - smoothstep(0.92, 1.0, head_progress) * 0.55)
        pulse = 1.0 + math.sin(self.t * 46.0) * 0.07
        length = width * 1.35 * pulse
        radius = width * 1.15 * pulse

        rings = [
            (1.28, edge, 9),
            (0.88, glow, 7),
            (0.48, core, 4),
        ]
        for scale, color, line_width in rings:
            rx = length * scale
            ry = radius * scale
            color = mix_color("#070814", color, head_alpha)
            self.canvas.create_oval(hx - rx + sx, hy - ry + sy, hx + rx + sx, hy + ry + sy, outline=color, width=line_width)

        front = hx + fx * length * 0.86
        front_y = hy + fy * length * 0.86
        back = hx - fx * length * 0.52
        back_y = hy - fy * length * 0.52
        head_poly = [
            back + nx * radius * 0.78 + sx,
            back_y + ny * radius * 0.78 + sy,
            hx + fx * length * 0.22 + nx * radius * 1.06 + sx,
            hy + fy * length * 0.22 + ny * radius * 1.06 + sy,
            front + sx,
            front_y + sy,
            hx + fx * length * 0.22 - nx * radius * 1.06 + sx,
            hy + fy * length * 0.22 - ny * radius * 1.06 + sy,
            back - nx * radius * 0.78 + sx,
            back_y - ny * radius * 0.78 + sy,
        ]
        self.canvas.create_polygon(*head_poly, fill=mix_color("#070814", glow, head_alpha), outline="")

        core_radius = radius * 0.38
        self.canvas.create_oval(
            hx - core_radius + sx,
            hy - core_radius + sy,
            hx + core_radius + sx,
            hy + core_radius + sy,
            fill=mix_color("#070814", "#ffffff", head_alpha),
            outline="",
        )

        for i in range(18):
            angle = -1.25 + 2.5 * i / 17
            burst = (fx * math.cos(angle) + nx * math.sin(angle), fy * math.cos(angle) + ny * math.sin(angle))
            start_x = hx - fx * width * 0.2
            start_y = hy - fy * width * 0.2
            end_x = start_x + burst[0] * width * (0.55 + 0.09 * (i % 5))
            end_y = start_y + burst[1] * width * (0.55 + 0.09 * (i % 5))
            color = hot if i % 3 == 0 else edge
            self.canvas.create_line(start_x + sx, start_y + sy, end_x + sx, end_y + sy, fill=color, width=2 + i % 3, capstyle=tk.ROUND)

    def _draw_flow_bands(self, ox, oy, tx, ty, nx, ny, sx, sy, edge, hot, core, rainbow=False):
        beam_width = self.vars["beam_width"].get() * self.vars["power"].get()
        density = int(self.vars["rays"].get())
        band_count = max(6, density) if rainbow else density * 2
        dash_count = 2 if rainbow else 4
        for i in range(band_count):
            lane = (i / max(1, band_count - 1) - 0.5) * 1.55
            offset = lane * beam_width * (0.42 + 0.10 * math.sin(i))
            dash_offset = (self.t * 2.8 + i * 0.073) % 1.0
            for d in range(dash_count):
                u1 = (dash_offset + d * (0.42 if rainbow else 0.27)) % 1.0
                u2 = min(1.0, u1 + (0.22 if rainbow else 0.15))
                if u1 > 0.93:
                    continue
                wave1 = math.sin(u1 * 33 - self.t * 44 + i) * 6
                wave2 = math.sin(u2 * 33 - self.t * 44 + i) * 6
                x1 = lerp(ox, tx, u1) + nx * (offset + wave1)
                y1 = lerp(oy, ty, u1) + ny * (offset + wave1)
                x2 = lerp(ox, tx, u2) + nx * (offset + wave2)
                y2 = lerp(oy, ty, u2) + ny * (offset + wave2)
                if rainbow:
                    color = hsv_to_hex(self.vars["hue"].get() + i * 28.0 + u1 * 190.0 + self.t * 120.0, 0.88, 1.0)
                    if abs(lane) < 0.22:
                        color = mix_color(color, "#ffffff", 0.58)
                else:
                    color = core if abs(lane) < 0.28 else (hot if i % 3 == 0 else edge)
                self.canvas.create_line(x1 + sx, y1 + sy, x2 + sx, y2 + sy, fill=color, width=2 + (i % 3), capstyle=tk.ROUND)

    def _draw_suction_lines(self, ox, oy, nx, ny, fx, fy, sx, sy, edge, glow):
        charge = self.vars["charge_size"].get() * self.vars["power"].get()
        for i in range(int(self.vars["rays"].get()) + 8):
            side = -1 if i % 2 else 1
            start_dist = charge * (1.3 + 0.9 * ((i * 37) % 100) / 100)
            along = -120 - ((i * 53) % 180)
            lateral = side * start_dist
            pull = (self.t * 3.4 + i * 0.17) % 1.0
            x1 = ox + fx * along + nx * lateral
            y1 = oy + fy * along + ny * lateral
            x2 = lerp(x1, ox + fx * 24, 0.55 + 0.35 * pull)
            y2 = lerp(y1, oy + fy * 24, 0.55 + 0.35 * pull)
            self.canvas.create_line(x1 + sx, y1 + sy, x2 + sx, y2 + sy, fill=glow if i % 3 else edge, width=2, capstyle=tk.ROUND)

    def _draw_impact(self, tx, ty, fx, fy, nx, ny, sx, sy, edge, glow, core, fade):
        impact = self.vars["impact"].get() * self.vars["power"].get() * fade
        radius = 74 * impact * (1.0 + math.sin(self.t * 16) * 0.08)
        self.canvas.create_oval(tx - radius + sx, ty - radius + sy, tx + radius + sx, ty + radius + sy, outline=glow, width=10)
        self.canvas.create_oval(
            tx - radius * 1.55 + sx,
            ty - radius * 0.82 + sy,
            tx + radius * 1.55 + sx,
            ty + radius * 0.82 + sy,
            outline=edge,
            width=5,
        )
        self.canvas.create_oval(tx - radius * 0.45 + sx, ty - radius * 0.45 + sy, tx + radius * 0.45 + sx, ty + radius * 0.45 + sy, fill=core, outline="")

        for i in range(34):
            angle = -1.45 + 2.9 * i / 33
            bx = fx * math.cos(angle) + nx * math.sin(angle)
            by = fy * math.cos(angle) + ny * math.sin(angle)
            length = radius * self.rng.uniform(0.75, 1.9)
            self.canvas.create_line(tx + sx, ty + sy, tx + bx * length + sx, ty + by * length + sy, fill=edge, width=2 + i % 3)

    def _draw_particles(self, sx, sy):
        rainbow_type1 = self.beam_type.get() == "type1" and self.type1_rainbow.get()
        for p in self.particles:
            alpha = clamp(p.life / p.max_life, 0.0, 1.0)
            size = p.size * alpha
            if rainbow_type1 and (alpha < 0.24 or size < 1.8):
                continue
            color = mix_color("#070814", p.color, alpha)
            self.canvas.create_oval(p.x - size + sx, p.y - size + sy, p.x + size + sx, p.y + size + sy, fill=color, outline="")

    def _draw_pose_hint(self, sx, sy, glow, core):
        ox, oy = ORIGIN
        self.canvas.create_oval(ox - 28 + sx, oy - 72 + sy, ox + 28 + sx, oy - 16 + sy, fill="#20243a", outline=glow, width=2)
        self.canvas.create_line(ox - 6 + sx, oy - 18 + sy, ox - 42 + sx, oy + 42 + sy, fill="#3a4167", width=14)
        self.canvas.create_line(ox + 6 + sx, oy - 18 + sy, ox + 38 + sx, oy + 36 + sy, fill="#3a4167", width=14)
        self.canvas.create_line(ox - 18 + sx, oy - 2 + sy, ox + 8 + sx, oy + 4 + sy, fill=core, width=8)
        self.canvas.create_line(ox - 46 + sx, oy + 35 + sy, ox - 78 + sx, oy + 78 + sy, fill="#2c3154", width=12)
        self.canvas.create_line(ox + 35 + sx, oy + 34 + sy, ox + 69 + sx, oy + 75 + sy, fill="#2c3154", width=12)

    def run(self):
        self.root.mainloop()


if __name__ == "__main__":
    KamehamehaSimulator().run()

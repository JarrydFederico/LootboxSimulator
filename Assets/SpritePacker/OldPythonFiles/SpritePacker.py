from PIL import Image
import math

INPUT_FILE = "input.png"
OUTPUT_FILE = "packed_sprite_sheet.png"

SPRITE_SIZE = 128
GRID_COLS = 8
GRID_ROWS = 8
PADDING = 12

SHEET_WIDTH = 1024
SHEET_HEIGHT = 1024
MAX_SPRITES = GRID_COLS * GRID_ROWS

img = Image.open(INPUT_FILE).convert("RGBA")

alpha = img.getchannel("A")
pixels = alpha.load()
width, height = img.size

visited = set()
sprites = []

def is_visible(x, y):
    return pixels[x, y] > 10

for y in range(height):
    for x in range(width):
        if (x, y) in visited or not is_visible(x, y):
            continue

        stack = [(x, y)]
        visited.add((x, y))

        min_x = max_x = x
        min_y = max_y = y

        while stack:
            cx, cy = stack.pop()

            min_x = min(min_x, cx)
            max_x = max(max_x, cx)
            min_y = min(min_y, cy)
            max_y = max(max_y, cy)

            for nx, ny in (
                (cx + 1, cy),
                (cx - 1, cy),
                (cx, cy + 1),
                (cx, cy - 1),
            ):
                if (
                    0 <= nx < width
                    and 0 <= ny < height
                    and (nx, ny) not in visited
                    and is_visible(nx, ny)
                ):
                    visited.add((nx, ny))
                    stack.append((nx, ny))

        box = (min_x, min_y, max_x + 1, max_y + 1)

        if (box[2] - box[0]) > 8 and (box[3] - box[1]) > 8:
            sprites.append(box)

sprites.sort(key=lambda b: (b[1], b[0]))

output = Image.new("RGBA", (SHEET_WIDTH, SHEET_HEIGHT), (0, 0, 0, 0))

for i, box in enumerate(sprites[:MAX_SPRITES]):
    sprite = img.crop(box)

    max_content_size = SPRITE_SIZE - PADDING * 2
    w, h = sprite.size
    scale = min(max_content_size / w, max_content_size / h)

    new_w = int(w * scale)
    new_h = int(h * scale)

    sprite = sprite.resize((new_w, new_h), Image.Resampling.LANCZOS)

    col = i % GRID_COLS
    row = i // GRID_COLS

    x = col * SPRITE_SIZE + (SPRITE_SIZE - new_w) // 2
    y = row * SPRITE_SIZE + (SPRITE_SIZE - new_h) // 2

    output.paste(sprite, (x, y), sprite)

output.save(OUTPUT_FILE)

print(f"Detected {len(sprites)} sprites")
print(f"Packed {min(len(sprites), MAX_SPRITES)} sprites")
print(f"Saved as {OUTPUT_FILE}")
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import A4, letter, A5
from reportlab.lib.units import mm
import io

def get_pagesize(name):
    name = name.lower()
    if name == 'letter': return letter
    if name == 'a5': return A5
    return A4

def create_canvas(file_path, paper_size_name="A4", orientation="portrait"):
    size = get_pagesize(paper_size_name)
    if orientation.lower() == 'landscape':
        size = (size[1], size[0])
    return canvas.Canvas(file_path, pagesize=size)

def draw_text(c, x, y, text, font="Helvetica", size=10, color=(0,0,0)):
    c.setFont(font, size)
    c.setFillColorRGB(*color)
    c.drawString(x, y, text)

def draw_right_aligned_text(c, x, y, text, font="Helvetica", size=10, color=(0,0,0)):
    c.setFont(font, size)
    c.setFillColorRGB(*color)
    c.drawRightString(x, y, text)

def draw_centered_text(c, x, y, text, font="Helvetica", size=10, color=(0,0,0)):
    c.setFont(font, size)
    c.setFillColorRGB(*color)
    c.drawCentredString(x, y, text)

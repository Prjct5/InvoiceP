def hex_to_rgb(hex_code):
    """Converts a hex color string like '#1ABC9C' or '1ABC9C' to an RGB tuple with values 0-1 for ReportLab."""
    if not hex_code:
        return (0, 0, 0)
    hex_code = hex_code.lstrip('#')
    if len(hex_code) == 3:
        hex_code = ''.join(c + c for c in hex_code)
    try:
        return tuple(int(hex_code[i:i+2], 16) / 255.0 for i in (0, 2, 4))
    except ValueError:
        return (0, 0, 0)

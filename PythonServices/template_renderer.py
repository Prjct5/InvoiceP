import os
from utils.pdf_builder import create_canvas, draw_text, draw_right_aligned_text, draw_centered_text, mm, get_pagesize
from utils.color_utils import hex_to_rgb
from utils.qr_generator import generate_qr_buffer

def render_invoice_pdf(invoice, items, customer, template, settings, output_path):
    c = create_canvas(output_path, template['paper_size'], template['orientation'])
    
    # parse dimensions
    page_width, page_height = get_pagesize(template.get('paper_size', 'A4'))
    if template.get('orientation', 'portrait').lower() == 'landscape':
        page_width, page_height = page_height, page_width

    # colors
    bg_col = hex_to_rgb(template.get('header_bg_color', '#2C3E50'))
    text_col = hex_to_rgb(template.get('header_text_color', '#FFFFFF'))
    accent_col = hex_to_rgb(template.get('accent_color', '#1ABC9C'))
    
    # Draw header rect
    c.setFillColorRGB(*bg_col)
    c.rect(0, page_height - 40*mm, page_width, 40*mm, fill=1, stroke=0)
    
    # Header Store Name
    draw_text(c, 10*mm, page_height - 20*mm, settings.get('store_name', 'Store'), font="Helvetica-Bold", size=20, color=text_col)
    
    # Invoice Title
    draw_right_aligned_text(c, page_width - 10*mm, page_height - 20*mm, f"INVOICE {invoice.get('invoice_number', '')}", font="Helvetica-Bold", size=20, color=text_col)
    
    # Dates & Info
    y = page_height - 50*mm
    draw_text(c, 10*mm, y, "Billed To:", font="Helvetica-Bold", size=12)
    draw_text(c, 10*mm, y - 5*mm, f"{customer.get('full_name', '')}", font="Helvetica", size=10)
    if customer.get('phone'): 
        draw_text(c, 10*mm, y - 10*mm, f"Phone: {customer['phone']}", font="Helvetica", size=10)
    
    draw_right_aligned_text(c, page_width - 10*mm, y, f"Date: {invoice.get('invoice_date', '')}", font="Helvetica", size=10)
    draw_right_aligned_text(c, page_width - 10*mm, y - 5*mm, f"Status: {str(invoice.get('status', '')).title()}", font="Helvetica", size=10)
    
    # Items table header
    table_y = y - 30*mm
    c.setFillColorRGB(*hex_to_rgb(template.get('table_header_color', '#ECF0F1')))
    c.rect(10*mm, table_y - 5*mm, page_width - 20*mm, 10*mm, fill=1, stroke=0)
    
    draw_text(c, 12*mm, table_y, "Item Description", font="Helvetica-Bold")
    draw_right_aligned_text(c, page_width - 80*mm, table_y, "Qty", font="Helvetica-Bold")
    draw_right_aligned_text(c, page_width - 50*mm, table_y, "Unit Price", font="Helvetica-Bold")
    draw_right_aligned_text(c, page_width - 12*mm, table_y, "Total", font="Helvetica-Bold")
    
    # Items
    current_y = table_y - 10*mm
    row_alt = False
    for item in items:
        if row_alt:
            c.setFillColorRGB(*hex_to_rgb(template.get('row_alt_color', '#F9F9F9')))
            c.rect(10*mm, current_y - 3*mm, page_width - 20*mm, 8*mm, fill=1, stroke=0)
        row_alt = not row_alt
        
        draw_text(c, 12*mm, current_y, item.get('product_name', ''))
        draw_right_aligned_text(c, page_width - 80*mm, current_y, str(item.get('quantity', 0)))
        draw_right_aligned_text(c, page_width - 50*mm, current_y, f"{item.get('unit_price', 0):,.2f}")
        draw_right_aligned_text(c, page_width - 12*mm, current_y, f"{item.get('line_total', 0):,.2f}")
        
        current_y -= 8*mm
    
    # Totals
    totals_y = current_y - 10*mm
    sym = settings.get('currency_symbol', '')
    draw_right_aligned_text(c, page_width - 50*mm, totals_y, "Subtotal:", font="Helvetica-Bold")
    draw_right_aligned_text(c, page_width - 12*mm, totals_y, f"{sym} {invoice.get('subtotal', 0):,.2f}")
    
    if invoice.get('discount_amount', 0) > 0:
        totals_y -= 6*mm
        draw_right_aligned_text(c, page_width - 50*mm, totals_y, "Discount:")
        draw_right_aligned_text(c, page_width - 12*mm, totals_y, f"- {sym} {invoice['discount_amount']:,.2f}")
        
    totals_y -= 6*mm
    draw_right_aligned_text(c, page_width - 50*mm, totals_y, f"Tax ({settings.get('tax_rate', 0)}%):")
    draw_right_aligned_text(c, page_width - 12*mm, totals_y, f"{sym} {invoice.get('tax_amount', 0):,.2f}")
    
    totals_y -= 6*mm
    draw_right_aligned_text(c, page_width - 50*mm, totals_y, "TOTAL:", font="Helvetica-Bold", size=14, color=accent_col)
    draw_right_aligned_text(c, page_width - 12*mm, totals_y, f"{sym} {invoice.get('total_amount', 0):,.2f}", font="Helvetica-Bold", size=14, color=accent_col)
    
    # Footer and QR
    if template.get('show_qr_code'):
        qr_buf = generate_qr_buffer(f"INV:{invoice.get('invoice_number', '')}|TOTAL:{invoice.get('total_amount', 0)}|TEL:{settings.get('phone', '')}")
        try:
            from reportlab.lib.utils import ImageReader
            c.drawImage(ImageReader(qr_buf), 10*mm, 10*mm, width=30*mm, height=30*mm)
        except Exception as e:
            pass # ignore if qrcode placement fails
        
    if template.get('footer_text'):
        draw_centered_text(c, page_width/2, 20*mm, template['footer_text'], size=9)
        
    c.save()
    return output_path

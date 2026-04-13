import argparse
import os
import datetime
from config import get_db_connection, output_success, output_error
from utils.pdf_builder import create_canvas, draw_text, draw_right_aligned_text, mm

def generate_report(args):
    conn = get_db_connection(args.db_path)
    output_dir = args.output_dir or os.environ.get('TEMP', '.')
    os.makedirs(output_dir, exist_ok=True)
    
    timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
    pdf_path = os.path.join(output_dir, f"{args.type}_{timestamp}.pdf")
    
    c = create_canvas(pdf_path, "A4", "portrait")
    
    draw_text(c, 10*mm, 280*mm, f"Report: {args.type.replace('_', ' ').title()}", font="Helvetica-Bold", size=16)
    draw_right_aligned_text(c, 200*mm, 280*mm, f"Date: {datetime.datetime.now().strftime('%Y-%m-%d %H:%M')}", font="Helvetica", size=10)
    
    c.setLineWidth(1)
    c.line(10*mm, 275*mm, 200*mm, 275*mm)
    
    y = 265*mm
    
    if args.type == 'sales_summary':
        rows = conn.execute("SELECT invoice_date, count(*) as count, sum(total_amount) as total FROM invoices WHERE status = 'paid' GROUP BY invoice_date ORDER BY invoice_date DESC LIMIT 50").fetchall()
        draw_text(c, 10*mm, y, "Date", font="Helvetica-Bold")
        draw_text(c, 70*mm, y, "Invoices", font="Helvetica-Bold")
        draw_text(c, 120*mm, y, "Total Revenue (Paid)", font="Helvetica-Bold")
        y -= 10*mm
        for r in rows:
            draw_text(c, 10*mm, y, str(r['invoice_date']))
            draw_text(c, 70*mm, y, str(r['count']))
            draw_text(c, 120*mm, y, f"{r['total']:,.2f}")
            y -= 8*mm

    elif args.type == 'stock_status':
        rows = conn.execute("SELECT sku, name, stock_qty, min_stock FROM products ORDER BY name").fetchall()
        draw_text(c, 10*mm, y, "SKU", font="Helvetica-Bold")
        draw_text(c, 50*mm, y, "Product Name", font="Helvetica-Bold")
        draw_text(c, 150*mm, y, "Qty", font="Helvetica-Bold")
        draw_text(c, 180*mm, y, "Status", font="Helvetica-Bold")
        y -= 10*mm
        for r in rows:
            draw_text(c, 10*mm, y, str(r['sku']))
            draw_text(c, 50*mm, y, str(r['name']))
            draw_text(c, 150*mm, y, str(r['stock_qty']))
            status = 'LOW' if r['stock_qty'] <= r['min_stock'] and r['stock_qty'] > 0 else ('OUT' if r['stock_qty'] <= 0 else 'OK')
            draw_text(c, 180*mm, y, status)
            y -= 8*mm
            if y < 20*mm:
                c.showPage()
                y = 280*mm
                
    elif args.type == 'customer_ranking':
        rows = conn.execute("SELECT full_name, phone, total_spent FROM customers ORDER BY total_spent DESC LIMIT 50").fetchall()
        draw_text(c, 10*mm, y, "Customer Name", font="Helvetica-Bold")
        draw_text(c, 80*mm, y, "Phone", font="Helvetica-Bold")
        draw_text(c, 150*mm, y, "Total Spent", font="Helvetica-Bold")
        y -= 10*mm
        for r in rows:
            draw_text(c, 10*mm, y, str(r['full_name']))
            draw_text(c, 80*mm, y, str(r['phone'] or ''))
            draw_text(c, 150*mm, y, f"{r['total_spent']:,.2f}")
            y -= 8*mm

    elif args.type == 'product_ranking':
        query = """
        SELECT product_sku, product_name, sum(quantity) as total_qty, sum(line_total) as total_rev
        FROM invoice_items
        GROUP BY product_sku, product_name
        ORDER BY total_qty DESC LIMIT 50
        """
        rows = conn.execute(query).fetchall()
        draw_text(c, 10*mm, y, "SKU", font="Helvetica-Bold")
        draw_text(c, 50*mm, y, "Product Name", font="Helvetica-Bold")
        draw_text(c, 140*mm, y, "Sold Qty", font="Helvetica-Bold")
        draw_text(c, 170*mm, y, "Revenue", font="Helvetica-Bold")
        y -= 10*mm
        for r in rows:
            draw_text(c, 10*mm, y, str(r['product_sku']))
            draw_text(c, 50*mm, y, str(r['product_name'])[:30])
            draw_text(c, 140*mm, y, str(r['total_qty']))
            draw_text(c, 170*mm, y, f"{r['total_rev']:,.2f}")
            y -= 8*mm

    elif args.type == 'audit_trail':
        rows = conn.execute("SELECT logged_at, user_id, action, entity_type, summary FROM audit_logs ORDER BY logged_at DESC LIMIT 50").fetchall()
        draw_text(c, 10*mm, y, "Time", font="Helvetica-Bold")
        draw_text(c, 60*mm, y, "User ID", font="Helvetica-Bold")
        draw_text(c, 90*mm, y, "Action", font="Helvetica-Bold")
        draw_text(c, 130*mm, y, "Entity", font="Helvetica-Bold")
        y -= 10*mm
        for r in rows:
            draw_text(c, 10*mm, y, str(r['logged_at']))
            draw_text(c, 60*mm, y, str(r['user_id']))
            draw_text(c, 90*mm, y, str(r['action']))
            draw_text(c, 130*mm, y, str(r['entity_type']))
            y -= 8*mm
            if y < 20*mm:
                c.showPage()
                y = 280*mm
    else:
        raise ValueError(f"Unknown report type: {args.type}")

    c.save()
    return {"message": f"Report generated", "path": pdf_path}

def main(args):
    try:
        if getattr(args, 'test', False):
            return {"message": "Test placeholder generated", "path": "test_report.pdf"}
            
        if not args.db_path:
            raise ValueError("db-path is required")
            
        result = generate_report(args)
        return result
    except Exception as e:
        output_error(str(e))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--db-path", type=str, required=False)
    parser.add_argument("--output-dir", type=str, required=False)
    parser.add_argument("--type", type=str, default="sales_summary")
    parser.add_argument("--test", action="store_true")
    args = parser.parse_args()
    
    result = main(args)
    if result:
        output_success(result)

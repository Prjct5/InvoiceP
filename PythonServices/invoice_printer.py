import argparse
import sys
import os
import json
from config import get_db_connection, output_success, output_error
from template_renderer import render_invoice_pdf

def main(args):
    try:
        conn = get_db_connection(args.db_path) if args.db_path else None
        
        if getattr(args, 'test', False):
            invoice = {'invoice_number': 'TEST-000', 'invoice_date': '2024-01-01', 'subtotal': 100, 'discount_amount': 0, 'tax_amount': 15, 'total_amount': 115, 'status': 'paid'}
            items = [{'product_name': 'Test product', 'quantity': 1, 'unit_price': 100, 'line_total': 100}]
            customer = {'full_name': 'Test Customer', 'phone': '0100000'}
            template = {'paper_size': 'A4', 'header_bg_color': '#2C3E50', 'header_text_color': '#FFFFFF', 'accent_color': '#1ABC9C', 'show_qr_code': 1}
            settings = {'store_name': 'Test Store', 'currency_symbol': '$', 'tax_rate': 15, 'phone': '0000'}
            output_dir = args.output_dir or os.environ.get('TEMP', '.')
            pdf_path = os.path.join(output_dir, 'test_invoice.pdf')
            render_invoice_pdf(invoice, items, customer, template, settings, pdf_path)
            return {"message": "Test PDF generated", "path": pdf_path}

        if not conn:
            raise ValueError("db-path is required")

        invoice_id = args.invoice_id
        if not invoice_id:
            raise ValueError("invoice_id is required")

        invoice_row = conn.execute("SELECT * FROM invoices WHERE id = ?", (invoice_id,)).fetchone()
        if not invoice_row:
            raise ValueError("Invoice not found")
            
        invoice = dict(invoice_row)
        items = [dict(row) for row in conn.execute("SELECT * FROM invoice_items WHERE invoice_id = ?", (invoice_id,)).fetchall()]
        
        customer_row = conn.execute("SELECT * FROM customers WHERE id = ?", (invoice.get('customer_id'),)).fetchone()
        customer = dict(customer_row) if customer_row else {}
        
        template_row = conn.execute("SELECT * FROM invoice_templates WHERE id = ?", (invoice.get('template_id', 1),)).fetchone()
        template = dict(template_row) if template_row else {}
        
        settings_row = conn.execute("SELECT * FROM store_settings WHERE id = 1").fetchone()
        settings = dict(settings_row) if settings_row else {}
        
        output_dir = args.output_dir or os.environ.get('TEMP', '.')
        os.makedirs(output_dir, exist_ok=True)
        pdf_path = os.path.join(output_dir, f"invoice_{invoice.get('invoice_number', invoice_id)}.pdf")
        
        render_invoice_pdf(invoice, items, customer, template, settings, pdf_path)
        
        if args.action == 'print':
            try:
                import win32api
                import win32print
                printer = settings.get('printer_name') or win32print.GetDefaultPrinter()
                win32api.ShellExecute(0, "print", pdf_path, f'/d:"{printer}"', ".", 0)
                return {"message": "Invoice sent to printer", "path": pdf_path}
            except Exception as pe:
                return {"message": f"Printed failed or unavailable: {str(pe)}", "path": pdf_path}
            
        return {"message": f"Invoice {args.action} generated", "path": pdf_path}
    except Exception as e:
        output_error(str(e))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--invoice-id", type=int, required=False)
    parser.add_argument("--db-path", type=str, required=False)
    parser.add_argument("--output-dir", type=str, required=False)
    parser.add_argument("--action", type=str, default="pdf")
    parser.add_argument("--test", action="store_true")
    args = parser.parse_args()
    
    result = main(args)
    if result:
        output_success(result)

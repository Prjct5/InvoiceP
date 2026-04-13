import argparse
import webbrowser
import urllib.parse
from config import get_db_connection, output_success, output_error

def format_phone_number(phone, store_country_code='20'):
    if not phone:
        return ""
    digits = ''.join(filter(str.isdigit, phone))
    if digits.startswith('0'):
        digits = store_country_code + digits[1:]
    elif not digits.startswith(store_country_code) and len(digits) == 10:
        digits = store_country_code + digits
    return digits

def main(args):
    try:
        if getattr(args, 'test', False):
            return {"message": "Test successful", "url": "https://wa.me/201000000000?text=Test"}
            
        conn = get_db_connection(args.db_path)
        invoice_id = args.invoice_id
        if not invoice_id:
            raise ValueError("invoice_id is required")

        invoice = conn.execute("SELECT * FROM invoices WHERE id = ?", (invoice_id,)).fetchone()
        if not invoice:
            raise ValueError("Invoice not found")
            
        customer = conn.execute("SELECT * FROM customers WHERE id = ?", (invoice['customer_id'],)).fetchone()
        items = conn.execute("SELECT * FROM invoice_items WHERE invoice_id = ?", (invoice_id,)).fetchall()
        settings = conn.execute("SELECT * FROM store_settings WHERE id = 1").fetchone()
        
        target_number = format_phone_number(customer['whatsapp'] or customer['phone'])
        if not target_number:
            raise ValueError("Customer has no phone or whatsapp number")
            
        sym = settings['currency_symbol']
        message_lines = [
            f"Hello {customer['full_name']},",
            "",
            f"Your invoice {invoice['invoice_number']} from {settings['store_name']} is ready.",
            "",
            "Items:"
        ]
        
        for item in items:
            message_lines.append(f"  - {item['product_name']} (x{item['quantity']})   {sym} {item['line_total']:,.2f}")
            
        message_lines.extend([
            "",
            f"Subtotal:   {sym} {invoice['subtotal']:,.2f}",
        ])
        if invoice['discount_amount'] > 0:
            message_lines.append(f"Discount:   -{sym} {invoice['discount_amount']:,.2f}")
            
        message_lines.extend([
            f"Tax ({settings['tax_rate']}%):    {sym} {invoice['tax_amount']:,.2f}",
            f"TOTAL:      {sym} {invoice['total_amount']:,.2f}",
            "",
            f"Payment: {invoice['payment_method'] or 'None'} — {invoice['status'].upper()}",
            "",
            "Thank you for shopping with us!",
        ])
        
        if settings['phone']:
            message_lines.append(f"📞 Contact: {settings['phone']}")
            
        message_text = "\n".join(message_lines)
        encoded_message = urllib.parse.quote(message_text)
        
        url = f"https://wa.me/{target_number}?text={encoded_message}"
        
        webbrowser.open(url)
        return {"message": "WhatsApp opened", "url": url}
    except Exception as e:
        output_error(str(e))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--invoice-id", type=int, required=False)
    parser.add_argument("--db-path", type=str, required=False)
    parser.add_argument("--test", action="store_true")
    args = parser.parse_args()
    
    result = main(args)
    if result:
        output_success(result)

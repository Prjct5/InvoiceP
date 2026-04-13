.read schema.sql
.read seed_data.sql
INSERT INTO users (username, password_hash) VALUES ('admin', 'hash');
INSERT INTO customers (full_name) VALUES ('John Doe');
INSERT INTO products (category_id, name, sku, unit_price, stock_qty) VALUES (1, 'Test Chair', 'CHR-001', 4500, 10);
INSERT INTO invoices (customer_id, user_id, invoice_number, status) VALUES (1, 1, 'TEST-001', 'confirmed');
INSERT INTO invoice_items (invoice_id, product_id, quantity, unit_price, line_total, product_name, product_sku) VALUES (1, 1, 3, 4500, 13500, 'Test Chair', 'CHR-001');

SELECT 'Stock Qty After Invoice:', stock_qty FROM products WHERE id = 1;
SELECT 'Audit:', action, entity_type FROM audit_logs;

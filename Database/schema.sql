-- ============================================================
-- FurniTrack Database Schema v1.0
-- SQLite 3.x
-- Run: sqlite3 furnitrack.db < schema.sql
-- ============================================================

PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;
PRAGMA synchronous = NORMAL;

-- ─────────────────────────────────────────────────────────────
-- STORE SETTINGS (single row, id always = 1)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS store_settings (
    id                INTEGER PRIMARY KEY CHECK (id = 1),
    store_name        TEXT    NOT NULL DEFAULT 'My Furniture Store',
    owner_name        TEXT    NOT NULL DEFAULT '',
    phone             TEXT    NOT NULL DEFAULT '',
    address           TEXT    NOT NULL DEFAULT '',
    logo_path         TEXT             DEFAULT NULL,
    whatsapp_number   TEXT             DEFAULT NULL,
    printer_name      TEXT             DEFAULT NULL,
    currency_symbol   TEXT    NOT NULL DEFAULT 'EGP',
    tax_rate          REAL    NOT NULL DEFAULT 0.0,
    invoice_prefix    TEXT    NOT NULL DEFAULT 'INV',
    invoice_next_num  INTEGER NOT NULL DEFAULT 1,
    github_repo       TEXT             DEFAULT NULL,
    github_token      TEXT             DEFAULT NULL,
    github_branch     TEXT             DEFAULT 'main',
    auto_backup       INTEGER NOT NULL DEFAULT 0,
    theme_color       TEXT    NOT NULL DEFAULT '#1ABC9C',
    created_at        DATETIME NOT NULL DEFAULT (datetime('now')),
    updated_at        DATETIME NOT NULL DEFAULT (datetime('now'))
);

-- Ensure exactly one row always exists
INSERT OR IGNORE INTO store_settings (id) VALUES (1);

-- ─────────────────────────────────────────────────────────────
-- USERS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS users (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    username      TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    password_hash TEXT    NOT NULL,
    full_name     TEXT    NOT NULL DEFAULT '',
    role          TEXT    NOT NULL DEFAULT 'cashier'
                          CHECK (role IN ('admin', 'cashier', 'viewer')),
    is_active     INTEGER NOT NULL DEFAULT 1 CHECK (is_active IN (0, 1)),
    last_login    DATETIME         DEFAULT NULL,
    created_at    DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);

-- ─────────────────────────────────────────────────────────────
-- CUSTOMERS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS customers (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    full_name    TEXT    NOT NULL,
    phone        TEXT             DEFAULT NULL,
    whatsapp     TEXT             DEFAULT NULL,
    address      TEXT             DEFAULT NULL,
    notes        TEXT             DEFAULT NULL,
    total_spent  REAL    NOT NULL DEFAULT 0.0,
    created_at   DATETIME NOT NULL DEFAULT (datetime('now')),
    updated_at   DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_customers_phone    ON customers(phone);
CREATE INDEX IF NOT EXISTS idx_customers_name     ON customers(full_name COLLATE NOCASE);

-- ─────────────────────────────────────────────────────────────
-- CATEGORIES (hierarchical, self-referencing)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS categories (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    description TEXT             DEFAULT NULL,
    parent_id   INTEGER          DEFAULT NULL
                REFERENCES categories(id) ON DELETE SET NULL,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    created_at  DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_categories_parent ON categories(parent_id);

-- ─────────────────────────────────────────────────────────────
-- SUPPLIERS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS suppliers (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    company_name  TEXT    NOT NULL,
    contact_name  TEXT             DEFAULT NULL,
    phone         TEXT             DEFAULT NULL,
    email         TEXT             DEFAULT NULL,
    address       TEXT             DEFAULT NULL,
    notes         TEXT             DEFAULT NULL,
    created_at    DATETIME NOT NULL DEFAULT (datetime('now')),
    updated_at    DATETIME NOT NULL DEFAULT (datetime('now'))
);

-- ─────────────────────────────────────────────────────────────
-- INVOICE TEMPLATES
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS invoice_templates (
    id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    name                TEXT    NOT NULL UNIQUE,
    is_default          INTEGER NOT NULL DEFAULT 0 CHECK (is_default IN (0, 1)),
    -- Layout
    show_logo           INTEGER NOT NULL DEFAULT 1,
    logo_position       TEXT    NOT NULL DEFAULT 'left'
                                CHECK (logo_position IN ('left','center','right')),
    logo_width_px       INTEGER NOT NULL DEFAULT 120,
    -- Colors
    header_bg_color     TEXT    NOT NULL DEFAULT '#2C3E50',
    header_text_color   TEXT    NOT NULL DEFAULT '#FFFFFF',
    accent_color        TEXT    NOT NULL DEFAULT '#1ABC9C',
    table_header_color  TEXT    NOT NULL DEFAULT '#ECF0F1',
    row_alt_color       TEXT    NOT NULL DEFAULT '#F9F9F9',
    -- Typography
    font_family         TEXT    NOT NULL DEFAULT 'Arial',
    font_size_body      INTEGER NOT NULL DEFAULT 10,
    font_size_header    INTEGER NOT NULL DEFAULT 14,
    -- Content toggles
    show_sku            INTEGER NOT NULL DEFAULT 1,
    show_unit           INTEGER NOT NULL DEFAULT 1,
    show_discount_col   INTEGER NOT NULL DEFAULT 1,
    show_tax_line       INTEGER NOT NULL DEFAULT 1,
    show_payment_method INTEGER NOT NULL DEFAULT 1,
    show_signature_box  INTEGER NOT NULL DEFAULT 0,
    show_qr_code        INTEGER NOT NULL DEFAULT 1,
    -- Custom text blocks
    header_tagline      TEXT             DEFAULT NULL,
    footer_text         TEXT             DEFAULT 'Thank you for your business!',
    terms_text          TEXT             DEFAULT NULL,
    -- Paper
    paper_size          TEXT    NOT NULL DEFAULT 'A4'
                                CHECK (paper_size IN ('A4','Letter','A5')),
    orientation         TEXT    NOT NULL DEFAULT 'portrait'
                                CHECK (orientation IN ('portrait','landscape')),
    created_at          DATETIME NOT NULL DEFAULT (datetime('now')),
    updated_at          DATETIME NOT NULL DEFAULT (datetime('now'))
);

-- Seed one default template
INSERT OR IGNORE INTO invoice_templates
    (id, name, is_default)
VALUES (1, 'Classic Professional', 1);

-- ─────────────────────────────────────────────────────────────
-- PRODUCTS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS products (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    category_id  INTEGER NOT NULL
                 REFERENCES categories(id) ON DELETE RESTRICT,
    supplier_id  INTEGER          DEFAULT NULL
                 REFERENCES suppliers(id) ON DELETE SET NULL,
    name         TEXT    NOT NULL,
    sku          TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    description  TEXT             DEFAULT NULL,
    unit_price   REAL    NOT NULL CHECK (unit_price >= 0),
    cost_price   REAL    NOT NULL DEFAULT 0.0 CHECK (cost_price >= 0),
    stock_qty    INTEGER NOT NULL DEFAULT 0,
    min_stock    INTEGER NOT NULL DEFAULT 5,
    unit         TEXT    NOT NULL DEFAULT 'piece',
    image_path   TEXT             DEFAULT NULL,
    is_active    INTEGER NOT NULL DEFAULT 1 CHECK (is_active IN (0, 1)),
    created_at   DATETIME NOT NULL DEFAULT (datetime('now')),
    updated_at   DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_products_sku        ON products(sku);
CREATE INDEX IF NOT EXISTS idx_products_category   ON products(category_id);
CREATE INDEX IF NOT EXISTS idx_products_name       ON products(name COLLATE NOCASE);

-- ─────────────────────────────────────────────────────────────
-- INVOICES
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS invoices (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    invoice_number  TEXT    NOT NULL UNIQUE,
    customer_id     INTEGER NOT NULL
                    REFERENCES customers(id) ON DELETE RESTRICT,
    user_id         INTEGER NOT NULL
                    REFERENCES users(id) ON DELETE RESTRICT,
    template_id     INTEGER NOT NULL DEFAULT 1
                    REFERENCES invoice_templates(id) ON DELETE SET DEFAULT,
    status          TEXT    NOT NULL DEFAULT 'draft'
                    CHECK (status IN ('draft','confirmed','partial','paid','cancelled','refunded')),
    subtotal        REAL    NOT NULL DEFAULT 0.0,
    discount_amount REAL    NOT NULL DEFAULT 0.0,
    tax_amount      REAL    NOT NULL DEFAULT 0.0,
    total_amount    REAL    NOT NULL DEFAULT 0.0,
    amount_paid     REAL    NOT NULL DEFAULT 0.0,
    payment_method  TEXT             DEFAULT NULL
                    CHECK (payment_method IN ('cash','card','bank_transfer','installment',NULL)),
    notes           TEXT             DEFAULT NULL,
    invoice_date    DATETIME NOT NULL DEFAULT (datetime('now')),
    created_at      DATETIME NOT NULL DEFAULT (datetime('now')),
    updated_at      DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_invoices_customer    ON invoices(customer_id);
CREATE INDEX IF NOT EXISTS idx_invoices_date        ON invoices(invoice_date);
CREATE INDEX IF NOT EXISTS idx_invoices_status      ON invoices(status);
CREATE INDEX IF NOT EXISTS idx_invoices_number      ON invoices(invoice_number);

-- ─────────────────────────────────────────────────────────────
-- INVOICE ITEMS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS invoice_items (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    invoice_id   INTEGER NOT NULL
                 REFERENCES invoices(id) ON DELETE CASCADE,
    product_id   INTEGER NOT NULL
                 REFERENCES products(id) ON DELETE RESTRICT,
    quantity     INTEGER NOT NULL CHECK (quantity > 0),
    unit_price   REAL    NOT NULL CHECK (unit_price >= 0),
    discount_pct REAL    NOT NULL DEFAULT 0.0
                          CHECK (discount_pct BETWEEN 0 AND 100),
    line_total   REAL    NOT NULL,
    -- Snapshot of product name at time of sale (for historical accuracy)
    product_name TEXT    NOT NULL,
    product_sku  TEXT    NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_invoice_items_invoice ON invoice_items(invoice_id);
CREATE INDEX IF NOT EXISTS idx_invoice_items_product ON invoice_items(product_id);

-- ─────────────────────────────────────────────────────────────
-- PAYMENTS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS payments (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    invoice_id   INTEGER NOT NULL
                 REFERENCES invoices(id) ON DELETE CASCADE,
    amount_paid  REAL    NOT NULL CHECK (amount_paid > 0),
    method       TEXT    NOT NULL DEFAULT 'cash'
                          CHECK (method IN ('cash','card','bank_transfer','installment')),
    reference    TEXT             DEFAULT NULL,
    notes        TEXT             DEFAULT NULL,
    paid_at      DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_payments_invoice ON payments(invoice_id);

-- ─────────────────────────────────────────────────────────────
-- STOCK MOVEMENTS (append-only ledger)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS stock_movements (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id    INTEGER NOT NULL
                  REFERENCES products(id) ON DELETE RESTRICT,
    user_id       INTEGER          DEFAULT NULL
                  REFERENCES users(id) ON DELETE SET NULL,
    movement_type TEXT    NOT NULL
                  CHECK (movement_type IN (
                      'sale','return','purchase','adjustment','damage','transfer'
                  )),
    quantity      INTEGER NOT NULL,   -- negative = stock out, positive = stock in
    stock_after   INTEGER NOT NULL,   -- snapshot of stock_qty after this movement
    reason        TEXT             DEFAULT NULL,
    reference_doc TEXT             DEFAULT NULL,  -- e.g. "INV-00042"
    moved_at      DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_stock_movements_product ON stock_movements(product_id);
CREATE INDEX IF NOT EXISTS idx_stock_movements_date    ON stock_movements(moved_at);

-- ─────────────────────────────────────────────────────────────
-- AUDIT LOGS
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS audit_logs (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id      INTEGER          DEFAULT NULL
                 REFERENCES users(id) ON DELETE SET NULL,
    action       TEXT    NOT NULL
                 CHECK (action IN ('create','update','delete','login','logout','backup','print','send_whatsapp')),
    entity_type  TEXT    NOT NULL,   -- e.g. 'invoice', 'product', 'customer'
    entity_id    INTEGER          DEFAULT NULL,
    summary      TEXT             DEFAULT NULL,
    old_value    TEXT             DEFAULT NULL,  -- JSON snapshot of before
    new_value    TEXT             DEFAULT NULL,  -- JSON snapshot of after
    logged_at    DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_audit_logs_date   ON audit_logs(logged_at);
CREATE INDEX IF NOT EXISTS idx_audit_logs_user   ON audit_logs(user_id);

-- ─────────────────────────────────────────────────────────────
-- BACKUP HISTORY
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS backup_history (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id        INTEGER          DEFAULT NULL
                   REFERENCES users(id) ON DELETE SET NULL,
    backup_type    TEXT    NOT NULL DEFAULT 'github'
                   CHECK (backup_type IN ('github','local')),
    file_path      TEXT             DEFAULT NULL,
    github_commit  TEXT             DEFAULT NULL,
    file_size_kb   INTEGER          DEFAULT NULL,
    status         TEXT    NOT NULL DEFAULT 'success'
                   CHECK (status IN ('success','failed','in_progress')),
    error_message  TEXT             DEFAULT NULL,
    backed_up_at   DATETIME NOT NULL DEFAULT (datetime('now'))
);

-- ─────────────────────────────────────────────────────────────
-- TRIGGERS
-- ─────────────────────────────────────────────────────────────

-- Auto-deduct stock when invoice item is inserted
CREATE TRIGGER IF NOT EXISTS trg_stock_deduct_on_sale
AFTER INSERT ON invoice_items
BEGIN
    UPDATE products
    SET    stock_qty = stock_qty - NEW.quantity,
           updated_at = datetime('now')
    WHERE  id = NEW.product_id;

    INSERT INTO stock_movements
        (product_id, user_id, movement_type, quantity, stock_after, reason, reference_doc, moved_at)
    SELECT
        NEW.product_id,
        i.user_id,
        'sale',
        -NEW.quantity,
        p.stock_qty,
        'Invoice sale',
        'INV-' || NEW.invoice_id,
        datetime('now')
    FROM invoices i
    JOIN products p ON p.id = NEW.product_id
    WHERE i.id = NEW.invoice_id;
END;

-- Auto-restore stock on invoice item delete (for cancellations)
CREATE TRIGGER IF NOT EXISTS trg_stock_restore_on_cancel
AFTER DELETE ON invoice_items
BEGIN
    UPDATE products
    SET    stock_qty = stock_qty + OLD.quantity,
           updated_at = datetime('now')
    WHERE  id = OLD.product_id;

    INSERT INTO stock_movements
        (product_id, movement_type, quantity, stock_after, reason, reference_doc, moved_at)
    SELECT
        OLD.product_id,
        'return',
        OLD.quantity,
        p.stock_qty,
        'Invoice cancelled/item removed',
        'INV-' || OLD.invoice_id,
        datetime('now')
    FROM products p WHERE p.id = OLD.product_id;
END;

-- Update invoice updated_at timestamp on status change
CREATE TRIGGER IF NOT EXISTS trg_invoice_updated_at
AFTER UPDATE ON invoices
BEGIN
    UPDATE invoices SET updated_at = datetime('now') WHERE id = NEW.id;
END;

-- Update customer total_spent when invoice is confirmed
CREATE TRIGGER IF NOT EXISTS trg_customer_total_on_invoice
AFTER UPDATE OF status ON invoices
WHEN NEW.status = 'paid' AND OLD.status != 'paid'
BEGIN
    UPDATE customers
    SET    total_spent = total_spent + NEW.total_amount,
           updated_at  = datetime('now')
    WHERE  id = NEW.customer_id;
END;

-- Auto-generate invoice number
CREATE TRIGGER IF NOT EXISTS trg_invoice_number_generate
AFTER INSERT ON invoices
WHEN NEW.invoice_number IS NULL OR NEW.invoice_number = ''
BEGIN
    UPDATE invoices
    SET invoice_number = (
        SELECT invoice_prefix || '-' || printf('%05d', invoice_next_num)
        FROM store_settings WHERE id = 1
    )
    WHERE id = NEW.id;

    UPDATE store_settings
    SET invoice_next_num = invoice_next_num + 1
    WHERE id = 1;
END;

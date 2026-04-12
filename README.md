# FurniTrack — Furniture Store Invoice & Inventory Management System
## Complete Development Documentation

**Version:** 1.0.0  
**Author:** Development Team  
**Last Updated:** 2024  
**Status:** Pre-Development — Ready to Build  

---

## Table of Contents

1. [Project Vision & Problem Statement](#1-project-vision--problem-statement)
2. [How This System Solves Each Problem](#2-how-this-system-solves-each-problem)
3. [Architecture Overview](#3-architecture-overview)
4. [Technology Stack & Justification](#4-technology-stack--justification)
5. [Database Design (ERD & Schema)](#5-database-design-erd--schema)
6. [Project File Structure](#6-project-file-structure)
7. [Component 1 — SQLite Database](#7-component-1--sqlite-database)
8. [Component 2 — Python Services Layer](#8-component-2--python-services-layer)
9. [Component 3 — C# WinForms GUI Application](#9-component-3--c-winforms-gui-application)
10. [Invoice Template System](#10-invoice-template-system)
11. [WhatsApp Integration (Free, No API)](#11-whatsapp-integration-free-no-api)
12. [First-Run Setup & Preferences](#12-first-run-setup--preferences)
13. [GitHub Backup Service](#13-github-backup-service)
14. [Audit & Logging System](#14-audit--logging-system)
15. [Build & Distribution](#15-build--distribution)
16. [Development Stages — Step by Step](#16-development-stages--step-by-step)
17. [Stage Completion Checklist](#17-stage-completion-checklist)
18. [Coding Standards & Conventions](#18-coding-standards--conventions)
19. [Testing Strategy](#19-testing-strategy)
20. [Known Limitations & Future Roadmap](#20-known-limitations--future-roadmap)

---

## 1. Project Vision & Problem Statement

### What Problem Are We Solving?

A furniture store owner manages dozens of daily transactions manually — on paper, in spreadsheets, or using expensive cloud software that requires subscriptions and internet connections. This creates several interconnected problems:

**Problem 1 — No unified invoice system.**  
The store has no reliable way to produce professional invoices. Handwritten receipts are error-prone, unprofessional, and impossible to search or retrieve later. When a customer disputes a sale, there is no paper trail.

**Problem 2 — Inventory is tracked manually or not at all.**  
Stock levels are guessed, not measured. Products sell out unexpectedly. Reorder decisions are made on intuition. There is no history of what moved, when, and at what price.

**Problem 3 — Customer communication is fragmented.**  
After a sale, the customer has no record of what they bought. Sending a copy of the invoice requires the customer to be physically present or involves emailing a PDF and hoping they receive it. WhatsApp is the dominant communication channel in many regions but is not integrated with any sales system.

**Problem 4 — No backup or data safety.**  
Business data lives on a single machine. A hardware failure, theft, or accidental deletion destroys years of records with no recovery path.

**Problem 5 — Expensive software or lock-in.**  
Most point-of-sale systems require monthly subscriptions, are cloud-only, or export data in proprietary formats. The owner has no control over their own data.

**Problem 6 — Repeated setup work.**  
Every time a new machine is used or the application is reinstalled, the user must re-enter store name, printer preferences, tax rates, and other settings from scratch.

**Problem 7 — Invoice templates are fixed.**  
Standard invoicing tools produce rigid, identical-looking invoices. The store owner wants to present a branded, professional document that reflects their business identity — not a generic receipt.

---

## 2. How This System Solves Each Problem

| Problem | Solution in FurniTrack |
|---|---|
| No invoice system | Full invoice creation, editing, PDF export, and print from within the app |
| Manual inventory | Real-time stock tracking with automatic deduction on every sale, low-stock alerts |
| Customer communication | One-click WhatsApp link opens WhatsApp Desktop with invoice details pre-filled — free, no API |
| No backup | Automated push of the SQLite database file to a private GitHub repository on exit or on demand |
| Expensive software | Fully local, open architecture, single `.exe`, SQLite database the owner controls completely |
| Repeated setup | First-run wizard saves all preferences to the database; never asked again unless the user wants to change them |
| Rigid templates | Invoice Template Editor lets the user design up to 5 named templates with logo, colors, fonts, column layout, footer text, and signature block — each invoice can choose which template to use |

---

## 3. Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     FurniTrack Application                  │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              C# WinForms GUI (.NET 8)                │  │
│  │                                                      │  │
│  │  Dashboard | Invoices | Products | Customers |       │  │
│  │  Reports   | Settings | Templates | Backup           │  │
│  └──────────────┬────────────────────────┬─────────────┘  │
│                 │                        │                  │
│         Direct SQLite              Process.Start()          │
│                 │                        │                  │
│  ┌──────────────▼───────────┐  ┌────────▼───────────────┐ │
│  │     SQLite Database      │  │   Python Services       │ │
│  │     furnitrack.db        │  │                         │ │
│  │                          │  │  invoice_printer.py     │ │
│  │  - store_settings        │  │  whatsapp_sender.py     │ │
│  │  - users                 │  │  backup_service.py      │ │
│  │  - customers             │  │  report_generator.py    │ │
│  │  - categories            │  │  template_renderer.py   │ │
│  │  - suppliers             │  │                         │ │
│  │  - products              │  │  Each compiled to .exe  │ │
│  │  - invoices              │  │  via PyInstaller        │ │
│  │  - invoice_items         │  └────────────────────────┘ │
│  │  - payments              │                              │
│  │  - invoice_templates     │       ┌────────────────┐    │
│  │  - stock_movements       │       │ WhatsApp Desktop│    │
│  │  - audit_logs            │       │ (opened via URL)│    │
│  │  - backup_history        │       └────────────────┘    │
│  └──────────────────────────┘                              │
│                                          ┌───────────────┐ │
│                                          │  GitHub Repo  │ │
│                                          │  (DB backup)  │ │
│                                          └───────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Data Flow on a Sale

```
User fills invoice form
        │
        ▼
C# validates input (quantities, prices, customer required)
        │
        ▼
DatabaseManager.BeginTransaction()
        │
        ├──► INSERT INTO invoices (header record)
        │
        ├──► INSERT INTO invoice_items (one row per product)
        │         │
        │         └──► SQLite TRIGGER fires automatically:
        │                  UPDATE products SET stock_qty = stock_qty - qty
        │                  INSERT INTO stock_movements (audit trail)
        │
        ├──► INSERT INTO payments (if paid immediately)
        │
        └──► INSERT INTO audit_logs (who did what)
        
DatabaseManager.Commit()
        │
        ▼
UI shows action buttons:
  [Print Invoice]  [Send WhatsApp]  [View PDF]  [New Invoice]
```

---

## 4. Technology Stack & Justification

### C# .NET 8 WinForms — GUI Layer

**Why WinForms over WPF or MAUI?**  
WinForms produces the most straightforward Windows desktop application. It compiles to a single self-contained executable with `dotnet publish -r win-x64 --self-contained`. The UI paradigm (forms, controls, events) is well understood, produces a classic professional business application look, and has zero runtime dependency — the `.exe` runs on any Windows 10+ machine without installing .NET separately.

WPF was considered but rejected because it requires significantly more boilerplate for simple data-entry forms. MAUI was rejected because this is Windows-only.

**Why .NET 8 specifically?**  
Long-term support release (supported until 2026). `PublishSingleFile` and `PublishAot` produce a compact, dependency-free executable.

### SQLite — Database Layer

**Why SQLite over SQL Server, MySQL, or PostgreSQL?**  
SQLite is a single file. No server process. No installation. No network port. No username/password configuration. The database file can be copied, backed up, emailed, or pushed to GitHub as a single binary. For a single-store, single-machine application this is the correct choice. SQLite handles hundreds of thousands of rows without performance issues for this use case.

**Why not JSON files or XML?**  
Relational integrity. A JSON file cannot enforce that an `invoice_item` always references a valid `product`, or that deleting a customer does not leave orphaned invoices. SQLite provides foreign keys, transactions, and triggers — all necessary for reliable financial data.

### Python 3.11 — Services Layer

**Why Python for the services?**  
Python has mature, free libraries for PDF generation (`ReportLab`), HTML-to-PDF rendering (`WeasyPrint`), QR code generation (`qrcode`), GitHub API interaction (`PyGithub`), and image manipulation (`Pillow`). Implementing the same functionality in C# would require heavyweight NuGet packages with licensing complications or significant custom code.

Python scripts are compiled to standalone `.exe` files using PyInstaller. From C#'s perspective, calling a Python service is identical to calling any command-line tool — `Process.Start("invoice_printer.exe", "--invoice-id 42")` — and reading the JSON result from stdout.

**Why not run Python as an embedded interpreter inside C#?**  
Complexity without benefit. PyInstaller-compiled executables are self-contained, version-locked, and deployable without any Python installation on the end-user's machine.

### WhatsApp — Free Desktop Integration

**Why not the WhatsApp Business API?**  
The WhatsApp Business API requires a Meta Business account, application approval, a hosting server, and per-message fees. This is unnecessary when WhatsApp Desktop is already installed on the operator's machine.

**The free solution:**  
WhatsApp Web and WhatsApp Desktop support the `wa.me` deep link format:  
`https://wa.me/[phone]?text=[url-encoded-message]`  
Opening this URL in the default browser triggers WhatsApp Desktop to open a pre-composed message to the customer's number. The operator reviews the message and presses Enter (or the send button). No API, no cost, no approval process.

---

## 5. Database Design (ERD & Schema)

### Entity Relationship Summary

| Entity | Purpose |
|---|---|
| `store_settings` | Single-row config: store name, logo, printer, WhatsApp number, GitHub credentials, tax rate |
| `users` | Application users with roles (admin, cashier). Passwords hashed with bcrypt |
| `customers` | Customer records: name, phone, WhatsApp number, address |
| `categories` | Hierarchical product categories (self-referencing parent_id) |
| `suppliers` | Supplier/vendor records linked to products |
| `products` | Full product catalog with SKU, price, cost, stock qty, image |
| `invoice_templates` | Saved template configurations (colors, fonts, logo position, column layout, footer) |
| `invoices` | Invoice header: customer, cashier, date, totals, status, payment method |
| `invoice_items` | Invoice line items: product, qty, unit price at time of sale, discount, line total |
| `payments` | Payment records (an invoice can have partial payments or multiple payment methods) |
| `stock_movements` | Append-only ledger of every stock change with reason and reference document |
| `audit_logs` | Who did what, to which record, when — for every significant action |
| `backup_history` | Log of every backup attempt with result and GitHub commit hash |

### Cardinality Rules

- One `customer` places zero or many `invoices`
- One `invoice` contains one or many `invoice_items`
- One `product` appears in zero or many `invoice_items`
- One `invoice` has zero or many `payments` (supports installment plans)
- One `category` has zero or many child `categories` (self-join)
- One `product` belongs to exactly one `category` and one `supplier`
- One `user` creates many `invoices`, `stock_movements`, and `audit_logs`
- One `invoice_template` is used by zero or many `invoices`

### Complete DDL Schema

```sql
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
```

### Seed Data

```sql
-- Default categories for a furniture store
INSERT OR IGNORE INTO categories (id, name, description, parent_id, sort_order) VALUES
    (1,  'Sofas & Sectionals',    'All sofa types',              NULL, 1),
    (2,  'Chairs',                'Accent, dining, office',      NULL, 2),
    (3,  'Tables',                'Coffee, dining, side tables', NULL, 3),
    (4,  'Beds & Bedroom',        'Bed frames, headboards',      NULL, 4),
    (5,  'Storage',               'Wardrobes, cabinets, shelves',NULL, 5),
    (6,  'Outdoor',               'Garden & patio furniture',    NULL, 6),
    (7,  'Sectional Sofas',       'L-shaped & modular',         1,    1),
    (8,  'Loveseat',              'Two-seat sofas',              1,    2),
    (9,  'Accent Chairs',         'Decorative single chairs',   2,    1),
    (10, 'Office Chairs',         'Ergonomic & desk chairs',    2,    2),
    (11, 'Coffee Tables',         'Low living room tables',     3,    1),
    (12, 'Dining Tables',         'Kitchen & dining sets',      3,    2);
```

---

## 6. Project File Structure

```
FurniTrack/
│
├── 📁 Database/
│   ├── schema.sql                    ← Full DDL (tables, indexes, triggers)
│   ├── seed_data.sql                 ← Default categories, demo data
│   └── furnitrack.db                 ← Generated at first run (not in source)
│
├── 📁 PythonServices/
│   ├── requirements.txt
│   ├── build_services.bat            ← Runs PyInstaller for all scripts
│   ├── config.py                     ← Shared: reads db_path from app.json
│   ├── invoice_printer.py            ← Renders PDF, sends to printer
│   ├── whatsapp_sender.py            ← Builds wa.me URL, opens browser
│   ├── backup_service.py             ← Pushes DB to GitHub repo
│   ├── report_generator.py           ← Sales, stock, customer reports
│   ├── template_renderer.py          ← Applies invoice_template record to PDF
│   └── 📁 utils/
│       ├── pdf_builder.py            ← ReportLab base: page layout, fonts
│       ├── qr_generator.py           ← QR code for invoice reference
│       └── color_utils.py            ← Hex to RGB conversion helpers
│
├── 📁 CSharpGUI/
│   ├── FurniTrack.sln
│   └── 📁 FurniTrack/
│       ├── Program.cs                ← Entry point: checks first-run
│       ├── App.config
│       ├── app_config.json           ← Runtime config: db_path, python_services_dir
│       │
│       ├── 📁 Data/
│       │   ├── DatabaseManager.cs    ← SQLite connection singleton, migrations
│       │   └── 📁 Repositories/
│       │       ├── IRepository.cs    ← Generic CRUD interface
│       │       ├── CustomerRepo.cs
│       │       ├── ProductRepo.cs
│       │       ├── InvoiceRepo.cs
│       │       ├── InvoiceItemRepo.cs
│       │       ├── PaymentRepo.cs
│       │       ├── TemplateRepo.cs
│       │       ├── CategoryRepo.cs
│       │       ├── SupplierRepo.cs
│       │       └── AuditRepo.cs
│       │
│       ├── 📁 Models/
│       │   ├── Customer.cs
│       │   ├── Product.cs
│       │   ├── Category.cs
│       │   ├── Supplier.cs
│       │   ├── Invoice.cs
│       │   ├── InvoiceItem.cs
│       │   ├── Payment.cs
│       │   ├── InvoiceTemplate.cs
│       │   ├── StoreSettings.cs
│       │   ├── User.cs
│       │   └── StockMovement.cs
│       │
│       ├── 📁 Services/
│       │   ├── AuthService.cs        ← Login, session, role checks
│       │   ├── SettingsService.cs    ← Load/save store_settings
│       │   ├── PythonBridge.cs       ← Process.Start wrapper, JSON parsing
│       │   ├── InvoiceService.cs     ← Business logic: totals, numbering
│       │   ├── StockService.cs       ← Low-stock checks, movement queries
│       │   └── BackupService.cs      ← Triggers backup_service.exe
│       │
│       ├── 📁 Forms/
│       │   ├── SplashForm.cs         ← First-run wizard (5 steps)
│       │   ├── LoginForm.cs
│       │   ├── MainForm.cs           ← MDI shell with sidebar nav
│       │   │
│       │   ├── 📁 Dashboard/
│       │   │   └── DashboardForm.cs  ← Today's stats, low stock alerts
│       │   │
│       │   ├── 📁 Invoices/
│       │   │   ├── InvoiceListForm.cs   ← Searchable, filterable grid
│       │   │   ├── InvoiceNewForm.cs    ← Main invoice creation form
│       │   │   ├── InvoiceViewForm.cs   ← Read-only detail view
│       │   │   └── InvoiceActionBar.cs  ← Print/WhatsApp/PDF buttons
│       │   │
│       │   ├── 📁 Products/
│       │   │   ├── ProductListForm.cs
│       │   │   ├── ProductEditForm.cs
│       │   │   └── StockAdjustForm.cs
│       │   │
│       │   ├── 📁 Customers/
│       │   │   ├── CustomerListForm.cs
│       │   │   ├── CustomerEditForm.cs
│       │   │   └── CustomerHistoryForm.cs
│       │   │
│       │   ├── 📁 Templates/
│       │   │   ├── TemplateListForm.cs
│       │   │   ├── TemplateEditorForm.cs  ← Visual template designer
│       │   │   └── TemplatePreviewForm.cs ← Live PDF preview panel
│       │   │
│       │   ├── 📁 Reports/
│       │   │   └── ReportsForm.cs
│       │   │
│       │   └── 📁 Settings/
│       │       └── SettingsForm.cs
│       │
│       ├── 📁 Controls/
│       │   ├── SidebarPanel.cs       ← Custom navigation sidebar
│       │   ├── SearchBox.cs          ← Reusable search input with clear btn
│       │   ├── NumericUpDownEx.cs    ← Extended numeric input
│       │   └── StatusStrip.cs        ← Bottom bar: user, date, stock alerts
│       │
│       └── 📁 Resources/
│           ├── 📁 Icons/             ← 16x16, 24x24 PNG icons
│           ├── 📁 Fonts/             ← Bundled fonts for PDF rendering
│           └── app.ico
│
├── 📁 Build/
│   ├── build_all.bat                 ← Master build: C# + Python + NSIS
│   ├── installer.nsi                 ← NSIS installer script
│   └── 📁 Output/                   ← Generated at build time
│
└── 📁 Docs/
    ├── FurniTrack_Development_Documentation.md  ← This file
    └── CHANGELOG.md
```

---

## 7. Component 1 — SQLite Database

### Why This Component Exists

Without a structured database, the application would be unable to enforce data integrity. It would be possible to create an invoice for a product that does not exist, to deduct more stock than is available, or to lose historical invoice data when a product is renamed. SQLite with triggers and foreign keys eliminates all of these risks automatically.

### What This Component Does

The database is the single source of truth for everything in the application. When the application starts, `DatabaseManager.cs` opens the SQLite connection and immediately runs `PRAGMA foreign_keys = ON` and `PRAGMA journal_mode = WAL`. WAL (Write-Ahead Logging) ensures that if the application crashes mid-write, the database is not corrupted.

The `DatabaseManager` also checks if the schema is at the current version by querying a `pragma user_version`. On first run (user_version = 0), it executes the full `schema.sql`. On subsequent runs, it checks if migrations are needed and applies them in order. This means the database can be upgraded safely when new versions of the application are released.

### How It Connects to Everything

Every repository class (`CustomerRepo`, `ProductRepo`, etc.) receives the `DatabaseManager` instance through constructor injection. They never open their own connections. The `DatabaseManager` exposes a `GetConnection()` method that returns the single shared connection, which is thread-safe for the single-user desktop scenario.

### Completion Criteria for This Stage

- [ ] `schema.sql` runs without errors on SQLite 3.x
- [ ] All foreign key constraints enforce correctly (test with intentional violations)
- [ ] All triggers fire correctly (insert invoice_item → stock deducts, customer total updates on paid)
- [ ] `seed_data.sql` populates default categories
- [ ] `DatabaseManager.cs` opens, checks version, and runs migrations cleanly
- [ ] The database file can be opened in DB Browser for SQLite and all tables are visible

---

## 8. Component 2 — Python Services Layer

### Why This Component Exists

C# has limited, expensive, or complex options for high-quality PDF generation with custom branding, QR code embedding, and print-spooler control. Python's ReportLab library produces professional-grade PDFs at zero cost. By separating this concern into a standalone service, the PDF rendering code is independent of the GUI framework and can be updated without recompiling the C# application.

### Architecture of Python Bridge

C# calls Python services using `Process.Start()`. All communication is through:
- **Input:** Command-line arguments
- **Output:** A JSON string written to stdout (always the last line)
- **Exit codes:** 0 = success, 1 = error

```csharp
// PythonBridge.cs — how C# calls Python
public static ServiceResult Call(string serviceExe, string[] args) {
    var psi = new ProcessStartInfo {
        FileName = Path.Combine(ServicesDir, serviceExe),
        Arguments = string.Join(" ", args.Select(a => $"\"{a}\"")),
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute        = false,
        CreateNoWindow         = true
    };
    using var proc = Process.Start(psi);
    string output = proc.StandardOutput.ReadToEnd();
    proc.WaitForExit();
    return JsonSerializer.Deserialize<ServiceResult>(output.Trim().Split('\n').Last());
}
```

Each Python script ends with:
```python
print(json.dumps({"success": True, "message": "Invoice printed", "path": pdf_path}))
```

### Service: invoice_printer.py

**Purpose:** Receives an invoice ID, queries the database, renders a PDF using the invoice's linked template, and either saves the PDF or sends it to the printer.

**Arguments:**
```
invoice_printer.exe --invoice-id 42 --action [pdf|print|preview]
                    --db-path "C:\FurniTrack\furnitrack.db"
                    --output-dir "C:\FurniTrack\Exports"
```

**Actions:**
- `pdf` — Renders PDF to output_dir, returns `{"path": "...", "success": true}`
- `print` — Renders PDF to temp file, sends to default printer (or specified printer name from store_settings)
- `preview` — Same as `pdf` but saves to system temp dir

**Template loading:** The script reads the `invoice_templates` record linked to the invoice and builds the ReportLab document using those values. Every visual property (colors, fonts, logo position, footer text) comes from the template record.

### Service: whatsapp_sender.py

**Purpose:** Builds a `wa.me` URL with the customer's WhatsApp number and a pre-formatted invoice message, then opens it in the default browser. WhatsApp Desktop intercepts this URL and opens a compose window.

**The URL format:**
```
https://wa.me/[country_code][number]?text=[url_encoded_message]
```

**Example message built by the script:**
```
Hello Mohamed,

Your invoice INV-00042 from My Furniture Store is ready.

Items:
  - Sofa L-Shape (x1)         4,500 EGP
  - Coffee Table Marble (x1)    850 EGP

Subtotal:   5,350 EGP
Discount:     250 EGP
Tax (14%):    715 EGP
TOTAL:      5,815 EGP

Payment: Cash — PAID

Thank you for shopping with us!
📞 Contact: 01012345678
```

**Arguments:**
```
whatsapp_sender.exe --invoice-id 42 --db-path "..." 
```

**How the operator uses it:**
1. Click "Send WhatsApp" button in the invoice view
2. WhatsApp Desktop opens with the message pre-filled and the customer's number already set
3. The operator presses Enter to send
4. An audit log entry is written: `{action: 'send_whatsapp', entity_type: 'invoice', entity_id: 42}`

**Phone number normalization:**
The script strips non-numeric characters, removes leading zeros, and prepends the country code from `store_settings.whatsapp_number` prefix. Egyptian numbers starting with `0` become `20[rest of number]`.

### Service: backup_service.py

**Purpose:** Copies the SQLite database file to a private GitHub repository as a backup.

**Method:**
1. Reads `github_repo`, `github_token`, and `github_branch` from `store_settings`
2. Reads the current database file as binary
3. Uses the GitHub Contents API (single REST call, no git client required):
   ```
   PUT /repos/{owner}/{repo}/contents/backups/furnitrack_{datetime}.db
   Authorization: Bearer {github_token}
   Body: {"message": "Auto-backup", "content": "[base64-encoded file]"}
   ```
4. Returns the commit SHA
5. C# writes a record to `backup_history`

**No git client is installed on the user's machine.** The GitHub API accepts the file directly as base64 over HTTPS.

### Service: template_renderer.py

**Purpose:** The core PDF engine. Reads an `invoice_templates` record and applies every visual property to the ReportLab document. Called by `invoice_printer.py`.

**Key rendering decisions:**
- The store logo is loaded from `store_settings.logo_path`. If missing, the space is filled with the store name in large text.
- All currency amounts are formatted with `store_settings.currency_symbol` and comma separation.
- The QR code (if `show_qr_code = 1`) encodes the invoice number and store phone, giving the customer a reference they can show in store.
- Signature block (if `show_signature_box = 1`) renders two labeled horizontal lines: "Customer Signature" and "Authorized Signature."

### Service: report_generator.py

**Purpose:** Generates PDF reports for common business analytics queries.

**Report types:**
- `sales_summary` — Daily/weekly/monthly revenue, invoice count, average order value
- `stock_status` — All products with current qty, min_stock, status (OK / LOW / OUT)
- `customer_ranking` — Top customers by total spent
- `product_ranking` — Best-selling products by quantity
- `audit_trail` — Recent audit log entries, filterable by user and action

### Completion Criteria for This Stage

- [ ] `invoice_printer.exe` renders a multi-item invoice PDF correctly from all 3 default templates
- [ ] PDF includes logo, QR code, all line items, totals, and footer text
- [ ] `whatsapp_sender.exe` opens WhatsApp Desktop with correct number and formatted message
- [ ] `backup_service.exe` successfully pushes to a test private GitHub repo and returns commit SHA
- [ ] `report_generator.exe` renders all 5 report types without error
- [ ] All scripts return valid JSON on stdout for both success and error cases
- [ ] All scripts compiled to single `.exe` via PyInstaller with no external dependencies

---

## 9. Component 3 — C# WinForms GUI Application

### Why This Component Exists

The GUI is the operator's entire experience. Every other component — the database, the Python services — exists to serve the GUI. The GUI must be reliable, fast, and obvious. A furniture store cashier should be able to create a complete invoice in under 60 seconds without training.

### Application Startup Sequence

```
Program.cs
    │
    ├── Load app_config.json (db_path, services_dir)
    │
    ├── DatabaseManager.Initialize(dbPath)
    │       └── Apply schema / run migrations
    │
    ├── SettingsService.Load()
    │
    ├── IF store_settings has no data → SplashForm (wizard)
    │       └── After wizard: SaveSettings(), create admin user
    │
    └── LoginForm.ShowDialog()
            └── On success: MainForm.Show()
```

### MainForm — MDI Shell

The `MainForm` is an MDI (Multiple Document Interface) container with:
- A fixed left sidebar (220px wide, dark background `#2C3E50`) containing navigation buttons
- A top toolbar strip showing: store name, logged-in user, current date/time
- A bottom status strip showing: database path, last backup time, low-stock alert count
- The MDI client area (right side) where child forms open

Navigation items in sidebar:
- Dashboard
- New Invoice *(highlighted in accent color)*
- All Invoices
- Products
- Customers
- Invoice Templates
- Reports
- Settings
- Backup Now

### InvoiceNewForm — Core Feature

This is the most important form in the application. It must handle the complete sale flow.

**Layout — two-panel:**
- **Left panel (60% width):** Invoice line items table with add/remove rows
- **Right panel (40% width):** Customer selector, invoice date, payment method, notes, running totals

**Customer selection:**
- Search box with autocomplete (queries customers by name or phone as user types)
- "New Customer" button opens a minimal inline popup (name + phone only — other details optional)
- Once selected, shows customer name and WhatsApp number with a phone icon

**Product entry rows:**
Each row in the items table has:
- Product search (autocomplete by name or SKU, shows current stock in dropdown)
- Quantity spinner (default 1, max = current stock_qty)
- Unit price (pre-filled from product, editable)
- Discount % (default 0, 0–100)
- Line total (auto-calculated, read-only)

**Totals panel (bottom right):**
```
Subtotal:         5,350.00 EGP
Discount:          -250.00 EGP
Tax (14%):          715.00 EGP
────────────────────────────
TOTAL:            5,815.00 EGP
Amount Paid:      5,815.00 EGP  [input field]
Change Due:           0.00 EGP
```

**Save behavior:**
- Saves as `draft` unless "Confirm & Save" is clicked
- "Confirm & Save" transitions status to `confirmed` or `paid` depending on amount_paid
- After save, the action bar appears: [🖨 Print] [📲 Send WhatsApp] [📄 View PDF] [➕ New Invoice]

### TemplateEditorForm — Invoice Template Designer

This form allows the user to configure how their invoices look. It has two panels side-by-side:

**Left panel — controls:**
- Template name (text input)
- Logo position: radio buttons (Left / Center / Right)
- Logo width: numeric slider
- Header background color: color picker button (opens system color dialog)
- Header text color: color picker
- Accent color: color picker
- Font family: dropdown (Arial, Times New Roman, Calibri, Tahoma)
- Body font size: numeric input
- Toggle checkboxes: Show SKU, Show unit, Show discount column, Show tax line, Show payment method, Show QR code, Show signature box
- Footer text: multiline text input
- Terms text: multiline text input (optional, smaller text at bottom of invoice)
- Paper size: A4 / Letter / A5

**Right panel — live preview:**
- A `PictureBox` (or `WebBrowser` control) showing a rendered thumbnail of the invoice with the current settings
- The preview updates when the operator clicks "Preview" or after a 1-second debounce on any change
- Preview is generated by calling `template_renderer.exe --preview --template-json "[json]"` with the current unsaved settings

**Save behavior:**
- "Save Template" writes or updates the `invoice_templates` record
- "Set as Default" sets `is_default = 1` on this template (and clears it on all others)
- Up to 10 named templates can be saved

### Settings Form

Single-page tabbed form with tabs: **Store Info | Printer | WhatsApp | Backup | Users | Display**

All fields pre-populated from `store_settings`. Changes saved on "Save" click. No auto-save to prevent accidental overwrites.

**Store Info tab:**
- Store name, owner name, phone, address, logo upload (browse to image file)
- Tax rate (%), currency symbol, invoice prefix

**WhatsApp tab:**
- Default WhatsApp number (country code included)
- Message template preview (shows how the invoice message will look)
- "Test WhatsApp" button opens a test message to the configured number

**Backup tab:**
- GitHub repository (format: `owner/repo`)
- GitHub personal access token (masked input, "Show" toggle)
- Auto-backup on exit: Yes / No
- "Backup Now" button → triggers backup immediately and shows result

### GUI Design Principles

**Consistency:**
- Every list form has: search bar (top left), filter dropdown (top right), data grid (center), action buttons (bottom or right)
- Every edit form has: labeled fields in a single column or two-column layout, "Save" and "Cancel" buttons (bottom right)
- No floating toolbars, no ribbon, no right-click context menus (too hidden — all actions are visible buttons)

**Color scheme:**
- Sidebar: `#2C3E50` (dark blue-gray)
- Accent / hover: `#1ABC9C` (teal green)
- Background: `#FFFFFF` (white)
- Grid header: `#ECF0F1` (light gray)
- Error/warning: `#E74C3C` (red)
- Font: Segoe UI, 9pt (system default on Windows 10+)

**Keyboard shortcuts:**
- `F1` — New Invoice
- `F2` — Customer List
- `F3` — Product List
- `Ctrl+P` — Print current invoice
- `Ctrl+W` — Send WhatsApp
- `Escape` — Cancel / Close current child form

### Completion Criteria for This Stage

- [ ] Application starts, first-run wizard completes, settings are saved and not asked again on next launch
- [ ] Login form validates password against bcrypt hash correctly
- [ ] Sidebar navigates to all forms without error
- [ ] New invoice with 3+ items can be created, saved, and the stock deduction is visible in the Products list
- [ ] WhatsApp button opens WhatsApp Desktop with the correct number and formatted message
- [ ] Print button sends PDF to selected printer
- [ ] Template editor saves and the new template appears in the invoice form template dropdown
- [ ] Settings form saves and values persist on restart

---

## 10. Invoice Template System

### Problem This Solves

A fixed invoice layout makes the store look generic. A custom layout with the store's colors, logo placement, and footer text makes the invoice a brand touchpoint — the last thing a customer sees before leaving.

### Template Storage

Templates are stored in the `invoice_templates` table (see schema above). Each record holds every visual and content decision as individual columns. This means:
- Templates can be duplicated and modified
- The exact template used for an old invoice is preserved even if the template is later edited (because the invoice stores `template_id`, and the template record is a named version)
- If a template is deleted, invoices that used it fall back to the default template

### Template Variables Available

| Variable | Source |
|---|---|
| `{{store_name}}` | store_settings.store_name |
| `{{store_address}}` | store_settings.address |
| `{{store_phone}}` | store_settings.phone |
| `{{store_logo}}` | store_settings.logo_path |
| `{{invoice_number}}` | invoices.invoice_number |
| `{{invoice_date}}` | invoices.invoice_date |
| `{{customer_name}}` | customers.full_name |
| `{{customer_phone}}` | customers.phone |
| `{{subtotal}}` | invoices.subtotal |
| `{{discount}}` | invoices.discount_amount |
| `{{tax}}` | invoices.tax_amount |
| `{{total}}` | invoices.total_amount |
| `{{payment_method}}` | invoices.payment_method |
| `{{footer_text}}` | invoice_templates.footer_text |
| `{{terms_text}}` | invoice_templates.terms_text |

### Three Built-In Templates (Seeded at Install)

**Template 1 — Classic Professional**
- Dark navy header (`#2C3E50`) with white text
- Teal accent lines
- Logo left-aligned
- Full columns: SKU, description, qty, unit price, discount, total
- QR code bottom-right

**Template 2 — Modern Minimal**
- White header, accent color underline only
- Logo centered, large
- No SKU column, no discount column
- Large total amount in accent color
- No QR code, signature box visible

**Template 3 — Compact Receipt**
- A5 paper size
- Single-column narrow layout
- No logo
- Line items condensed (description and total only)
- Suitable for printing on receipt printers

---

## 11. WhatsApp Integration (Free, No API)

### Problem This Solves

Customers expect to receive their invoices on WhatsApp immediately after purchase. The WhatsApp Business API costs money and requires Meta approval. WhatsApp Desktop is already installed on the operator's machine and supports a deep-link URL that pre-composes a message.

### How It Works — Technical Detail

1. The operator clicks "Send WhatsApp" in the invoice view
2. C# calls `PythonBridge.Call("whatsapp_sender.exe", ["--invoice-id", invoiceId.ToString(), "--db-path", dbPath])`
3. `whatsapp_sender.py` queries the database for the invoice, customer, and items
4. It builds the message string (see example in Section 8)
5. It URL-encodes the message: `urllib.parse.quote(message)`
6. It builds: `url = f"https://wa.me/{normalized_phone}?text={encoded_message}"`
7. It calls: `webbrowser.open(url)` — this opens the default browser
8. If WhatsApp Desktop is installed and the browser recognizes the `wa.me` URL, it passes control to WhatsApp Desktop
9. WhatsApp Desktop opens with the customer's chat and the message pre-filled
10. The operator presses Enter to send
11. The script returns `{"success": true, "message": "WhatsApp opened"}` to stdout
12. C# writes an audit log entry and shows a success toast notification

### Phone Number Formatting Rules

Egyptian numbers:
- `01012345678` → `+201012345678`
- `+201012345678` → `+201012345678` (no change)
- `201012345678` → `+201012345678`

The `whatsapp` field on the customer record stores the number as entered. The `whatsapp_sender.py` normalizes it using the country code derived from `store_settings.whatsapp_number`.

### Operator Experience

The entire flow from "click button" to "ready to press Enter in WhatsApp" takes approximately 2–4 seconds. The operator does not need to type the customer's number or copy any text. They review the pre-composed message (which is already correct) and press Enter.

---

## 12. First-Run Setup & Preferences

### Problem This Solves

Every application restart should feel like continuing where you left off, not starting over. Preferences — printer name, tax rate, currency, template choice — should be set once and remembered forever.

### First-Run Detection

On startup, `Program.cs` calls `SettingsService.IsFirstRun()`:
```csharp
public static bool IsFirstRun() {
    var settings = Load();
    return string.IsNullOrWhiteSpace(settings.StoreName) || settings.StoreName == "My Furniture Store";
}
```

If `IsFirstRun()` returns true, `SplashForm` opens as a modal wizard before `LoginForm`.

### Wizard Steps

**Step 1 — Store Information**
- Store name (required)
- Owner name (required)
- Phone number (required)
- Address (required, shown on invoices)

**Step 2 — Logo & Branding**
- Logo upload: Browse button opens file dialog filtered to `*.png;*.jpg;*.jpeg`
- Logo preview shown at 200px wide
- "Skip for now" link available
- Theme color picker (sets `store_settings.theme_color` used in sidebar)

**Step 3 — Tax & Currency**
- Currency symbol (default: EGP)
- Tax rate % (default: 0, operator enters their local rate)
- Invoice number prefix (default: INV, can be store initials like MFS)
- Starting invoice number (default: 1)

**Step 4 — WhatsApp & Printer**
- WhatsApp number for sending (with country code)
- Printer selection: dropdown populated by `PrinterSettings.InstalledPrinters` in C#
- "Test Print" button prints a test page to confirm

**Step 5 — Admin Account**
- Username (required, no spaces)
- Password (required, minimum 6 characters)
- Confirm password
- "Create Account & Finish" button saves everything and closes wizard

### Preferences That Are Auto-Saved

| Preference | When Saved |
|---|---|
| Last used invoice template | When an invoice is confirmed |
| Last active tab in reports | On tab change |
| Window size and position | On form close |
| Last search query in each list | On form close |
| Column widths in DataGridViews | On form close |
| Selected printer | In settings form |

Window geometry preferences use `app_config.json` (not the database) since they are machine-specific, not store-specific.

---

## 13. GitHub Backup Service

### Problem This Solves

All business data lives in a single SQLite file on a single machine. If that machine fails, the data is gone. GitHub provides unlimited free private repositories. Pushing the database file to GitHub gives the owner a cloud backup without any subscription or server.

### How the Backup Works

The GitHub Contents API allows creating or updating files in a repository using a simple HTTPS PUT request with a personal access token. No git client needs to be installed.

```
PUT https://api.github.com/repos/{owner}/{repo}/contents/backups/furnitrack_{timestamp}.db
Authorization: Bearer {github_token}
Content-Type: application/json

{
  "message": "Auto-backup: My Furniture Store 2024-03-15 09:30",
  "content": "[base64-encoded .db file]",
  "branch": "main"
}
```

The response includes the commit SHA, which is stored in `backup_history.github_commit`.

### Backup Frequency

- **Auto-backup on exit:** If `store_settings.auto_backup = 1`, the backup runs when the user closes the application. A progress dialog shows "Backing up to GitHub..." and the user sees success/failure before the window closes.
- **Manual backup:** "Backup Now" button in the sidebar and in Settings runs the backup immediately.
- **Daily auto-backup:** If the application runs for more than 8 hours (e.g., on a dedicated point-of-sale machine), a background timer triggers a backup every 24 hours.

### Security Considerations

The GitHub personal access token is stored in `store_settings.github_token`. This field is stored in the SQLite database. The database file is on the user's local machine. The token should be scoped to `repo` permissions only on the specific backup repository. Instructions for creating a scoped token are included in the Settings form's help text.

---

## 14. Audit & Logging System

### Problem This Solves

In a business context, knowing what happened and who did it is essential. If a product price changes and old invoices look wrong, the audit log shows when the price was changed and by whom. If an invoice is deleted, the audit log shows it was deleted and by which user.

### What Gets Logged

| Action | Trigger |
|---|---|
| `login` | User authenticates successfully |
| `logout` | User closes application or logs out manually |
| `create` | Any INSERT on invoices, customers, products, invoice_templates |
| `update` | Any UPDATE on invoices (status change logged with old/new status), products (price change logged with old/new price) |
| `delete` | Any DELETE on invoices, customers, products |
| `print` | Invoice sent to printer |
| `send_whatsapp` | WhatsApp opened for an invoice |
| `backup` | Backup run (success or failure) |

### Log Record Structure

```sql
INSERT INTO audit_logs (user_id, action, entity_type, entity_id, summary, old_value, new_value)
VALUES (
    2,                    -- ID of the logged-in cashier
    'update',             -- what happened
    'invoice',            -- which table
    42,                   -- which record
    'Status changed to paid',
    '{"status": "confirmed", "amount_paid": 0}',    -- before (JSON)
    '{"status": "paid", "amount_paid": 5815.00}'    -- after (JSON)
);
```

### Viewing Logs

The Reports form has an "Audit Trail" tab showing the last 1000 audit log entries in a searchable grid. Admins can filter by: user, action type, entity type, and date range.

---

## 15. Build & Distribution

### Build Process

The master `build_all.bat` script performs the following in order:

**Step 1 — Build Python services:**
```bat
cd PythonServices
pip install -r requirements.txt
pyinstaller --onefile --name invoice_printer   invoice_printer.py
pyinstaller --onefile --name whatsapp_sender   whatsapp_sender.py
pyinstaller --onefile --name backup_service    backup_service.py
pyinstaller --onefile --name report_generator  report_generator.py
pyinstaller --onefile --name template_renderer template_renderer.py
```

**Step 2 — Build C# application:**
```bat
cd CSharpGUI
dotnet publish FurniTrack/FurniTrack.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -o ../Build/Output/app
```

**Step 3 — Assemble distribution folder:**
```bat
xcopy /Y /E PythonServices/dist/*.exe Build/Output/services/
xcopy /Y Database/schema.sql Build/Output/
xcopy /Y Database/seed_data.sql Build/Output/
```

**Step 4 — Create installer:**
```bat
makensis Build/installer.nsi
```

### NSIS Installer Script (installer.nsi)

The installer:
- Creates `C:\Program Files\FurniTrack\` directory structure
- Copies all files to installation directory
- Creates desktop shortcut pointing to `FurniTrack.exe`
- Creates Start Menu entry
- Registers in Windows Add/Remove Programs
- On uninstall: removes all installed files but **leaves the database file** (user data is never deleted by uninstall)
- Configures `app_config.json` with default database path: `%APPDATA%\FurniTrack\furnitrack.db`

### app_config.json

This file is the only file the application reads before accessing the database. It contains machine-specific paths:
```json
{
  "db_path": "C:\\Users\\User\\AppData\\Roaming\\FurniTrack\\furnitrack.db",
  "services_dir": "C:\\Program Files\\FurniTrack\\services",
  "exports_dir": "C:\\Users\\User\\Documents\\FurniTrack\\Exports",
  "log_level": "info"
}
```

`app_config.json` lives at `%APPDATA%\FurniTrack\app_config.json` — never in Program Files (which may be read-only).

### Final Distribution Contents

```
FurniTrack_Setup_v1.0.0.exe    ← Single installer, ~80MB
```

What the installer puts on the machine:
```
C:\Program Files\FurniTrack\
├── FurniTrack.exe              ← Main application (~60MB self-contained)
└── services\
    ├── invoice_printer.exe     ← PDF + print service
    ├── whatsapp_sender.exe     ← WhatsApp URL builder
    ├── backup_service.exe      ← GitHub backup
    ├── report_generator.exe    ← Report PDFs
    └── template_renderer.exe  ← Invoice template engine

C:\Users\[User]\AppData\Roaming\FurniTrack\
├── app_config.json
└── furnitrack.db               ← All business data

C:\Users\[User]\Documents\FurniTrack\Exports\
└── [generated PDFs appear here]
```

---

## 16. Development Stages — Step by Step

### Stage 1 — Database Foundation
**Files created:** `schema.sql`, `seed_data.sql`, `DatabaseManager.cs`  
**Goal:** A working, tested SQLite database with all tables, triggers, indexes, and seed data.  
**Verification:** Open DB Browser for SQLite, confirm all tables exist and triggers fire on test INSERTs.  
**After this stage:** The data layer is complete. All subsequent stages can query real data.

---

### Stage 2 — Python Services Foundation
**Files created:** `config.py`, `utils/pdf_builder.py`, `utils/qr_generator.py`, `utils/color_utils.py`, `requirements.txt`  
**Goal:** Shared utilities used by all Python services are written and tested.  
**Verification:** Running `pdf_builder.py` as a standalone script produces a test PDF with a colored header and a QR code.  
**After this stage:** Individual Python services can be written quickly by composing these utilities.

---

### Stage 3 — Invoice Template Renderer
**Files created:** `template_renderer.py`  
**Goal:** Given a template record (as JSON) and invoice data (as JSON), produce a correctly styled PDF.  
**Verification:** 3 different template configurations produce 3 visually distinct PDFs with all invoice data correct.  
**After this stage:** Any service that needs to produce an invoice PDF simply calls `template_renderer`.

---

### Stage 4 — Invoice Printer Service
**Files created:** `invoice_printer.py`  
**Goal:** Full service that reads invoice from SQLite, calls template_renderer, returns PDF path or sends to printer.  
**Verification:** `invoice_printer.exe --invoice-id 1 --action pdf` produces a PDF. `--action print` sends it to the default printer.  
**After this stage:** C# can print any invoice with a single function call.

---

### Stage 5 — WhatsApp Sender Service
**Files created:** `whatsapp_sender.py`  
**Goal:** Builds WhatsApp deep-link URL and opens it in the default browser.  
**Verification:** Running the script opens WhatsApp Desktop with correct number and correctly formatted message.  
**After this stage:** C# can trigger WhatsApp message composition for any invoice.

---

### Stage 6 — Backup Service
**Files created:** `backup_service.py`  
**Goal:** Pushes database file to GitHub repository using the Contents API.  
**Verification:** After running, the GitHub repository contains the database file and a new commit is visible.  
**After this stage:** Automated cloud backup is fully functional.

---

### Stage 7 — Report Generator Service
**Files created:** `report_generator.py`  
**Goal:** All 5 report types produce correct PDFs.  
**Verification:** Each report type tested with real data shows correct figures matching manual database queries.  
**After this stage:** All Python services are complete and compiled to `.exe` files.

---

### Stage 8 — C# Project Scaffold
**Files created:** All `.csproj`, `Program.cs`, `App.config`, `app_config.json`, all Repository files, all Model files  
**Goal:** The C# project compiles and connects to the database. All repositories have working CRUD methods.  
**Verification:** A unit-test console app successfully creates a customer, creates an invoice with items, and reads it back.  
**After this stage:** The data access layer is complete. Forms can be built on top of it.

---

### Stage 9 — Services Layer (C#)
**Files created:** `AuthService.cs`, `SettingsService.cs`, `PythonBridge.cs`, `InvoiceService.cs`, `StockService.cs`, `BackupService.cs`  
**Goal:** Business logic is centralized in service classes. `PythonBridge.cs` calls Python `.exe` files and parses JSON results correctly.  
**Verification:** `PythonBridge.Call("invoice_printer.exe", [...])` returns a correctly parsed `ServiceResult`.  
**After this stage:** Forms only need to call Services — they contain no direct database queries.

---

### Stage 10 — First-Run Wizard & Login
**Files created:** `SplashForm.cs`, `LoginForm.cs`  
**Goal:** Fresh install shows the 5-step wizard, saves settings, creates admin user, and on next launch goes directly to login.  
**Verification:** Delete `store_settings` row, restart app — wizard appears. Complete wizard, restart — login appears, wizard does not.  
**After this stage:** The application bootstraps correctly on any machine.

---

### Stage 11 — MainForm Shell & Sidebar
**Files created:** `MainForm.cs`, `SidebarPanel.cs`, `StatusStrip.cs`  
**Goal:** MDI shell with working sidebar navigation. All nav buttons open placeholder child forms.  
**Verification:** All sidebar buttons respond. The status bar shows correct user and date. Low-stock count appears in status bar.  
**After this stage:** The application has its frame. Individual forms can be built and plugged in.

---

### Stage 12 — Dashboard
**Files created:** `DashboardForm.cs`  
**Goal:** Summary tiles showing today's revenue, invoice count, low-stock products, and top-selling product of the week.  
**Verification:** Data in tiles matches manual SQL queries.  
**After this stage:** Management can see the store's daily status at a glance.

---

### Stage 13 — Products & Categories CRUD
**Files created:** `ProductListForm.cs`, `ProductEditForm.cs`, `StockAdjustForm.cs`  
**Goal:** Full product management: list, search, add, edit, deactivate, manual stock adjustment.  
**Verification:** Adding a product, then finding it in search, editing its price, and adjusting stock — all operations complete without error.  
**After this stage:** The product catalog is fully manageable.

---

### Stage 14 — Customers CRUD
**Files created:** `CustomerListForm.cs`, `CustomerEditForm.cs`, `CustomerHistoryForm.cs`  
**Goal:** Full customer management with invoice history view.  
**Verification:** A customer can be created, their invoices viewed in history, and their total_spent matches the sum of their paid invoices.  
**After this stage:** Customer management is complete.

---

### Stage 15 — New Invoice Form (Core)
**Files created:** `InvoiceNewForm.cs`, `InvoiceActionBar.cs`  
**Goal:** Complete invoice creation: customer selection, item addition, quantity/price/discount editing, save, action buttons.  
**Verification:** Creating an invoice with 3 items reduces stock of each product by the correct quantity. Invoice appears in list. PDF is generated and correct.  
**After this stage:** The application can perform its primary function — selling.

---

### Stage 16 — Invoice List & View
**Files created:** `InvoiceListForm.cs`, `InvoiceViewForm.cs`  
**Goal:** Searchable invoice list with filters (status, date range, customer). Invoice view shows all details and has Print/WhatsApp/PDF buttons.  
**Verification:** An invoice created in Stage 15 appears in the list and can be viewed, printed, and sent to WhatsApp.  
**After this stage:** Full invoice lifecycle is visible and actionable.

---

### Stage 17 — Invoice Template Editor
**Files created:** `TemplateListForm.cs`, `TemplateEditorForm.cs`, `TemplatePreviewForm.cs`  
**Goal:** Visual template designer with live PDF preview. At least 3 saved templates selectable per invoice.  
**Verification:** Creating a new template with custom colors, saving it, then generating an invoice using that template produces a PDF that matches the preview.  
**After this stage:** The invoice template system is fully operational.

---

### Stage 18 — Reports
**Files created:** `ReportsForm.cs`  
**Goal:** All 5 report types generate and display correctly. Reports can be exported to PDF.  
**Verification:** Sales summary report matches manual sum of invoices in date range. Stock report shows correct quantities.  
**After this stage:** Business analytics are available.

---

### Stage 19 — Settings Form
**Files created:** `SettingsForm.cs`  
**Goal:** All settings tabs working. Changes persist on restart. Backup Now button works.  
**Verification:** Change store name in settings, restart app — sidebar shows new store name. Backup Now pushes to GitHub.  
**After this stage:** All configuration is self-service.

---

### Stage 20 — Build, Packaging & Testing
**Files created:** `build_all.bat`, `installer.nsi`  
**Goal:** Complete end-to-end build produces a single setup `.exe` that installs cleanly on a fresh Windows 10 machine.  
**Verification:** Fresh VM, run installer, open app — full workflow from first-run wizard to creating, printing, and WhatsApping an invoice completes without any missing dependencies or errors.  
**After this stage:** The application is ready for production use.

---

## 17. Stage Completion Checklist

### Per-Stage Sign-Off Criteria

Before marking a stage complete and starting the next, all of the following must be true:

- [ ] All files for the stage are created with full, working code (no placeholder `// TODO` methods)
- [ ] The feature works end-to-end with real data from the database
- [ ] All edge cases handled: empty states (no customers, no products), invalid input (letters in quantity field), missing optional data (customer has no WhatsApp number)
- [ ] No unhandled exceptions — all database and service calls wrapped in try/catch with user-friendly error messages
- [ ] Any new database query is tested with both the happy path and an empty result set
- [ ] Python services return valid JSON for both success and failure cases

### Final Pre-Release Checklist

- [ ] All 20 stages complete and signed off
- [ ] Fresh Windows 10 VM install test passes completely
- [ ] Installer creates correct file structure, shortcuts work, uninstall removes application files but preserves database
- [ ] Application handles disconnected scenarios: Python service `.exe` missing (clear error), database file missing (guided recovery)
- [ ] Invoice PDF output reviewed by a human for visual quality
- [ ] WhatsApp message format reviewed — correct phone normalization for Egyptian numbers
- [ ] GitHub backup verified by restoring the database file from GitHub to a new machine
- [ ] All audit log actions write correct records
- [ ] Low-stock alert appears in status bar when a product is below min_stock
- [ ] Application closes cleanly (no hanging processes from Python service calls)

---

## 18. Coding Standards & Conventions

### C# Conventions

**Naming:**
- Classes, Properties, Public Methods: `PascalCase` — `InvoiceService`, `TotalAmount`, `GetByCustomerId()`
- Private fields: `_camelCase` — `_dbManager`, `_settings`
- Local variables and parameters: `camelCase` — `invoiceId`, `customerId`
- Constants: `UPPER_SNAKE_CASE` — `MAX_DISCOUNT_PCT`

**Repository pattern:**
Every repository implements `IRepository<T>`:
```csharp
public interface IRepository<T> {
    T? GetById(int id);
    List<T> GetAll();
    int Insert(T entity);
    bool Update(T entity);
    bool Delete(int id);
}
```
Repositories return `null` (not exceptions) when a record is not found.

**Error handling:**
All service methods return a result object, never throw to the caller:
```csharp
public class ServiceResult<T> {
    public bool Success { get; set; }
    public T? Data     { get; set; }
    public string Error { get; set; } = "";
}
```

**No magic strings:**
Database table and column names are `const string` values in each repository class.

### Python Conventions

**Every script is a command-line tool:**
```python
if __name__ == "__main__":
    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument("--invoice-id", type=int, required=True)
    parser.add_argument("--db-path",    type=str, required=True)
    parser.add_argument("--action",     type=str, default="pdf")
    args = parser.parse_args()
    result = main(args)
    print(json.dumps(result))
    sys.exit(0 if result["success"] else 1)
```

**Every script ends with JSON to stdout.** Never print partial output or debug text to stdout during normal operation. Use `sys.stderr` for debug messages.

**Imports are at the top of each file.** No conditional imports inside functions.

### SQL Conventions

- All table names: `snake_case` plural — `invoice_items`, `stock_movements`
- All column names: `snake_case`
- All primary keys: `id INTEGER PRIMARY KEY AUTOINCREMENT`
- All timestamps: stored as `DATETIME` in SQLite ISO format `2024-03-15 09:30:00`
- All money amounts: `REAL` (SQLite float — sufficient for currency at 2 decimal places)
- All boolean flags: `INTEGER` with `CHECK (col IN (0, 1))`

---

## 19. Testing Strategy

### Database Tests (Manual)

Run these SQL statements directly in DB Browser after each schema change:

```sql
-- Test: Invoice item insertion triggers stock deduction
INSERT INTO invoices (customer_id, user_id, invoice_number, status)
VALUES (1, 1, 'TEST-001', 'confirmed');

-- Check stock before:
SELECT stock_qty FROM products WHERE id = 1;

INSERT INTO invoice_items (invoice_id, product_id, quantity, unit_price, line_total, product_name, product_sku)
VALUES (last_insert_rowid(), 1, 3, 4500, 13500, 'Test Chair', 'CHR-001');

-- Check stock after (should be 3 less):
SELECT stock_qty FROM products WHERE id = 1;

-- Check stock movement was recorded:
SELECT * FROM stock_movements WHERE product_id = 1 ORDER BY moved_at DESC LIMIT 1;
```

### Python Service Tests

Each Python script has a `--test` flag that runs a self-contained validation without needing a real database:

```bash
python invoice_printer.py --test
# Output: {"success": true, "message": "Test PDF generated", "path": "/tmp/test_invoice.pdf"}
```

### C# Form Tests (Manual Walkthrough)

Before marking a form complete, walk through this checklist:
1. Open the form with no data in the database — it should show empty state gracefully
2. Fill all required fields and save — record appears in the list
3. Clear one required field and try to save — validation error shown, no save occurs
4. Open an existing record, change one field, save — changes persist on reopen
5. Delete a record — it disappears from the list
6. Close and reopen the form — state is correct (list repopulated, no leftover data)

---

## 20. Known Limitations & Future Roadmap

### Known Limitations (V1.0)

| Limitation | Reason | Workaround |
|---|---|---|
| Single machine only | No network database | Copy `furnitrack.db` manually between machines, or restore from GitHub backup |
| WhatsApp requires Desktop app installed | No API used | Install WhatsApp Desktop on the POS machine |
| No barcode scanner input | Out of scope V1 | SKU can be typed manually; barcode scanner that types text works with the product search box |
| No installment plan tracking | Simplified for V1 | Partial payments can be recorded; full installment schedule not implemented |
| Backup is full file, not incremental | Simple approach | File is typically small (< 50MB) even with years of data; full backup is fine |
| No multi-language support | Single store assumed | Arabic RTL support can be added to invoice PDFs in V2 |

### Roadmap (V2.0+)

- **Arabic RTL invoice templates** — ReportLab supports RTL text with arabic-reshaper library
- **Barcode/SKU scanner integration** — WinForms KeyPreview to intercept scanner input in product search
- **Multiple store locations** — Database schema already supports this via a `location_id` column addition
- **Customer loyalty points** — Add `points` column to customers, earn on purchase, redeem on invoice
- **Email invoice delivery** — SMTP integration as alternative to WhatsApp
- **Installment schedule management** — Full installment plan with due date tracking and overdue alerts
- **Export to Excel** — Sales data export via ClosedXML for accounting integration
- **Network mode** — Move database to a shared network path for multi-machine use (SQLite WAL handles concurrent reads)

---

*End of FurniTrack Development Documentation v1.0*  
*This document is the single source of truth for all development decisions. Any deviation from this plan must be documented in CHANGELOG.md with the reason for the change.*

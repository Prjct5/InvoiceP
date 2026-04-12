\# FurniTrack — Furniture Store Invoice \& Inventory Management System

\## Complete Development Documentation



\*\*Version:\*\* 1.0.0  

\*\*Author:\*\* Development Team  

\*\*Last Updated:\*\* 2024  

\*\*Status:\*\* Pre-Development — Ready to Build  



\---



\## Table of Contents



1\. \[Project Vision \& Problem Statement](#1-project-vision--problem-statement)

2\. \[How This System Solves Each Problem](#2-how-this-system-solves-each-problem)

3\. \[Architecture Overview](#3-architecture-overview)

4\. \[Technology Stack \& Justification](#4-technology-stack--justification)

5\. \[Database Design (ERD \& Schema)](#5-database-design-erd--schema)

6\. \[Project File Structure](#6-project-file-structure)

7\. \[Component 1 — SQLite Database](#7-component-1--sqlite-database)

8\. \[Component 2 — Python Services Layer](#8-component-2--python-services-layer)

9\. \[Component 3 — C# WinForms GUI Application](#9-component-3--c-winforms-gui-application)

10\. \[Invoice Template System](#10-invoice-template-system)

11\. \[WhatsApp Integration (Free, No API)](#11-whatsapp-integration-free-no-api)

12\. \[First-Run Setup \& Preferences](#12-first-run-setup--preferences)

13\. \[GitHub Backup Service](#13-github-backup-service)

14\. \[Audit \& Logging System](#14-audit--logging-system)

15\. \[Build \& Distribution](#15-build--distribution)

16\. \[Development Stages — Step by Step](#16-development-stages--step-by-step)

17\. \[Stage Completion Checklist](#17-stage-completion-checklist)

18\. \[Coding Standards \& Conventions](#18-coding-standards--conventions)

19\. \[Testing Strategy](#19-testing-strategy)

20\. \[Known Limitations \& Future Roadmap](#20-known-limitations--future-roadmap)



\---



\## 1. Project Vision \& Problem Statement



\### What Problem Are We Solving?



A furniture store owner manages dozens of daily transactions manually — on paper, in spreadsheets, or using expensive cloud software that requires subscriptions and internet connections. This creates several interconnected problems:



\*\*Problem 1 — No unified invoice system.\*\*  

The store has no reliable way to produce professional invoices. Handwritten receipts are error-prone, unprofessional, and impossible to search or retrieve later. When a customer disputes a sale, there is no paper trail.



\*\*Problem 2 — Inventory is tracked manually or not at all.\*\*  

Stock levels are guessed, not measured. Products sell out unexpectedly. Reorder decisions are made on intuition. There is no history of what moved, when, and at what price.



\*\*Problem 3 — Customer communication is fragmented.\*\*  

After a sale, the customer has no record of what they bought. Sending a copy of the invoice requires the customer to be physically present or involves emailing a PDF and hoping they receive it. WhatsApp is the dominant communication channel in many regions but is not integrated with any sales system.



\*\*Problem 4 — No backup or data safety.\*\*  

Business data lives on a single machine. A hardware failure, theft, or accidental deletion destroys years of records with no recovery path.



\*\*Problem 5 — Expensive software or lock-in.\*\*  

Most point-of-sale systems require monthly subscriptions, are cloud-only, or export data in proprietary formats. The owner has no control over their own data.



\*\*Problem 6 — Repeated setup work.\*\*  

Every time a new machine is used or the application is reinstalled, the user must re-enter store name, printer preferences, tax rates, and other settings from scratch.



\*\*Problem 7 — Invoice templates are fixed.\*\*  

Standard invoicing tools produce rigid, identical-looking invoices. The store owner wants to present a branded, professional document that reflects their business identity — not a generic receipt.



\---



\## 2. How This System Solves Each Problem



| Problem | Solution in FurniTrack |

|---|---|

| No invoice system | Full invoice creation, editing, PDF export, and print from within the app |

| Manual inventory | Real-time stock tracking with automatic deduction on every sale, low-stock alerts |

| Customer communication | One-click WhatsApp link opens WhatsApp Desktop with invoice details pre-filled — free, no API |

| No backup | Automated push of the SQLite database file to a private GitHub repository on exit or on demand |

| Expensive software | Fully local, open architecture, single `.exe`, SQLite database the owner controls completely |

| Repeated setup | First-run wizard saves all preferences to the database; never asked again unless the user wants to change them |

| Rigid templates | Invoice Template Editor lets the user design up to 5 named templates with logo, colors, fonts, column layout, footer text, and signature block — each invoice can choose which template to use |



\---



\## 3. Architecture Overview



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

│  │                          │  │  invoice\_printer.py     │ │

│  │  - store\_settings        │  │  whatsapp\_sender.py     │ │

│  │  - users                 │  │  backup\_service.py      │ │

│  │  - customers             │  │  report\_generator.py    │ │

│  │  - categories            │  │  template\_renderer.py   │ │

│  │  - suppliers             │  │                         │ │

│  │  - products              │  │  Each compiled to .exe  │ │

│  │  - invoices              │  │  via PyInstaller        │ │

│  │  - invoice\_items         │  └────────────────────────┘ │

│  │  - payments              │                              │

│  │  - invoice\_templates     │       ┌────────────────┐    │

│  │  - stock\_movements       │       │ WhatsApp Desktop│    │

│  │  - audit\_logs            │       │ (opened via URL)│    │

│  │  - backup\_history        │       └────────────────┘    │

│  └──────────────────────────┘                              │

│                                          ┌───────────────┐ │

│                                          │  GitHub Repo  │ │

│                                          │  (DB backup)  │ │

│                                          └───────────────┘ │

└─────────────────────────────────────────────────────────────┘

```



\### Data Flow on a Sale



```

User fills invoice form

&#x20;       │

&#x20;       ▼

C# validates input (quantities, prices, customer required)

&#x20;       │

&#x20;       ▼

DatabaseManager.BeginTransaction()

&#x20;       │

&#x20;       ├──► INSERT INTO invoices (header record)

&#x20;       │

&#x20;       ├──► INSERT INTO invoice\_items (one row per product)

&#x20;       │         │

&#x20;       │         └──► SQLite TRIGGER fires automatically:

&#x20;       │                  UPDATE products SET stock\_qty = stock\_qty - qty

&#x20;       │                  INSERT INTO stock\_movements (audit trail)

&#x20;       │

&#x20;       ├──► INSERT INTO payments (if paid immediately)

&#x20;       │

&#x20;       └──► INSERT INTO audit\_logs (who did what)

&#x20;       

DatabaseManager.Commit()

&#x20;       │

&#x20;       ▼

UI shows action buttons:

&#x20; \[Print Invoice]  \[Send WhatsApp]  \[View PDF]  \[New Invoice]

```



\---



\## 4. Technology Stack \& Justification



\### C# .NET 8 WinForms — GUI Layer



\*\*Why WinForms over WPF or MAUI?\*\*  

WinForms produces the most straightforward Windows desktop application. It compiles to a single self-contained executable with `dotnet publish -r win-x64 --self-contained`. The UI paradigm (forms, controls, events) is well understood, produces a classic professional business application look, and has zero runtime dependency — the `.exe` runs on any Windows 10+ machine without installing .NET separately.



WPF was considered but rejected because it requires significantly more boilerplate for simple data-entry forms. MAUI was rejected because this is Windows-only.



\*\*Why .NET 8 specifically?\*\*  

Long-term support release (supported until 2026). `PublishSingleFile` and `PublishAot` produce a compact, dependency-free executable.



\### SQLite — Database Layer



\*\*Why SQLite over SQL Server, MySQL, or PostgreSQL?\*\*  

SQLite is a single file. No server process. No installation. No network port. No username/password configuration. The database file can be copied, backed up, emailed, or pushed to GitHub as a single binary. For a single-store, single-machine application this is the correct choice. SQLite handles hundreds of thousands of rows without performance issues for this use case.



\*\*Why not JSON files or XML?\*\*  

Relational integrity. A JSON file cannot enforce that an `invoice\_item` always references a valid `product`, or that deleting a customer does not leave orphaned invoices. SQLite provides foreign keys, transactions, and triggers — all necessary for reliable financial data.



\### Python 3.11 — Services Layer



\*\*Why Python for the services?\*\*  

Python has mature, free libraries for PDF generation (`ReportLab`), HTML-to-PDF rendering (`WeasyPrint`), QR code generation (`qrcode`), GitHub API interaction (`PyGithub`), and image manipulation (`Pillow`). Implementing the same functionality in C# would require heavyweight NuGet packages with licensing complications or significant custom code.



Python scripts are compiled to standalone `.exe` files using PyInstaller. From C#'s perspective, calling a Python service is identical to calling any command-line tool — `Process.Start("invoice\_printer.exe", "--invoice-id 42")` — and reading the JSON result from stdout.



\*\*Why not run Python as an embedded interpreter inside C#?\*\*  

Complexity without benefit. PyInstaller-compiled executables are self-contained, version-locked, and deployable without any Python installation on the end-user's machine.



\### WhatsApp — Free Desktop Integration



\*\*Why not the WhatsApp Business API?\*\*  

The WhatsApp Business API requires a Meta Business account, application approval, a hosting server, and per-message fees. This is unnecessary when WhatsApp Desktop is already installed on the operator's machine.



\*\*The free solution:\*\*  

WhatsApp Web and WhatsApp Desktop support the `wa.me` deep link format:  

`https://wa.me/\[phone]?text=\[url-encoded-message]`  

Opening this URL in the default browser triggers WhatsApp Desktop to open a pre-composed message to the customer's number. The operator reviews the message and presses Enter (or the send button). No API, no cost, no approval process.



\---



\## 5. Database Design (ERD \& Schema)



\### Entity Relationship Summary



| Entity | Purpose |

|---|---|

| `store\_settings` | Single-row config: store name, logo, printer, WhatsApp number, GitHub credentials, tax rate |

| `users` | Application users with roles (admin, cashier). Passwords hashed with bcrypt |

| `customers` | Customer records: name, phone, WhatsApp number, address |

| `categories` | Hierarchical product categories (self-referencing parent\_id) |

| `suppliers` | Supplier/vendor records linked to products |

| `products` | Full product catalog with SKU, price, cost, stock qty, image |

| `invoice\_templates` | Saved template configurations (colors, fonts, logo position, column layout, footer) |

| `invoices` | Invoice header: customer, cashier, date, totals, status, payment method |

| `invoice\_items` | Invoice line items: product, qty, unit price at time of sale, discount, line total |

| `payments` | Payment records (an invoice can have partial payments or multiple payment methods) |

| `stock\_movements` | Append-only ledger of every stock change with reason and reference document |

| `audit\_logs` | Who did what, to which record, when — for every significant action |

| `backup\_history` | Log of every backup attempt with result and GitHub commit hash |



\### Cardinality Rules



\- One `customer` places zero or many `invoices`

\- One `invoice` contains one or many `invoice\_items`

\- One `product` appears in zero or many `invoice\_items`

\- One `invoice` has zero or many `payments` (supports installment plans)

\- One `category` has zero or many child `categories` (self-join)

\- One `product` belongs to exactly one `category` and one `supplier`

\- One `user` creates many `invoices`, `stock\_movements`, and `audit\_logs`

\- One `invoice\_template` is used by zero or many `invoices`



\### Complete DDL Schema



```sql

\-- ============================================================

\-- FurniTrack Database Schema v1.0

\-- SQLite 3.x

\-- Run: sqlite3 furnitrack.db < schema.sql

\-- ============================================================



PRAGMA foreign\_keys = ON;

PRAGMA journal\_mode = WAL;

PRAGMA synchronous = NORMAL;



\-- ─────────────────────────────────────────────────────────────

\-- STORE SETTINGS (single row, id always = 1)

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS store\_settings (

&#x20;   id                INTEGER PRIMARY KEY CHECK (id = 1),

&#x20;   store\_name        TEXT    NOT NULL DEFAULT 'My Furniture Store',

&#x20;   owner\_name        TEXT    NOT NULL DEFAULT '',

&#x20;   phone             TEXT    NOT NULL DEFAULT '',

&#x20;   address           TEXT    NOT NULL DEFAULT '',

&#x20;   logo\_path         TEXT             DEFAULT NULL,

&#x20;   whatsapp\_number   TEXT             DEFAULT NULL,

&#x20;   printer\_name      TEXT             DEFAULT NULL,

&#x20;   currency\_symbol   TEXT    NOT NULL DEFAULT 'EGP',

&#x20;   tax\_rate          REAL    NOT NULL DEFAULT 0.0,

&#x20;   invoice\_prefix    TEXT    NOT NULL DEFAULT 'INV',

&#x20;   invoice\_next\_num  INTEGER NOT NULL DEFAULT 1,

&#x20;   github\_repo       TEXT             DEFAULT NULL,

&#x20;   github\_token      TEXT             DEFAULT NULL,

&#x20;   github\_branch     TEXT             DEFAULT 'main',

&#x20;   auto\_backup       INTEGER NOT NULL DEFAULT 0,

&#x20;   theme\_color       TEXT    NOT NULL DEFAULT '#1ABC9C',

&#x20;   created\_at        DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   updated\_at        DATETIME NOT NULL DEFAULT (datetime('now'))

);



\-- Ensure exactly one row always exists

INSERT OR IGNORE INTO store\_settings (id) VALUES (1);



\-- ─────────────────────────────────────────────────────────────

\-- USERS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS users (

&#x20;   id            INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   username      TEXT    NOT NULL UNIQUE COLLATE NOCASE,

&#x20;   password\_hash TEXT    NOT NULL,

&#x20;   full\_name     TEXT    NOT NULL DEFAULT '',

&#x20;   role          TEXT    NOT NULL DEFAULT 'cashier'

&#x20;                         CHECK (role IN ('admin', 'cashier', 'viewer')),

&#x20;   is\_active     INTEGER NOT NULL DEFAULT 1 CHECK (is\_active IN (0, 1)),

&#x20;   last\_login    DATETIME         DEFAULT NULL,

&#x20;   created\_at    DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_users\_username ON users(username);



\-- ─────────────────────────────────────────────────────────────

\-- CUSTOMERS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS customers (

&#x20;   id           INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   full\_name    TEXT    NOT NULL,

&#x20;   phone        TEXT             DEFAULT NULL,

&#x20;   whatsapp     TEXT             DEFAULT NULL,

&#x20;   address      TEXT             DEFAULT NULL,

&#x20;   notes        TEXT             DEFAULT NULL,

&#x20;   total\_spent  REAL    NOT NULL DEFAULT 0.0,

&#x20;   created\_at   DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   updated\_at   DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_customers\_phone    ON customers(phone);

CREATE INDEX IF NOT EXISTS idx\_customers\_name     ON customers(full\_name COLLATE NOCASE);



\-- ─────────────────────────────────────────────────────────────

\-- CATEGORIES (hierarchical, self-referencing)

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS categories (

&#x20;   id          INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   name        TEXT    NOT NULL UNIQUE COLLATE NOCASE,

&#x20;   description TEXT             DEFAULT NULL,

&#x20;   parent\_id   INTEGER          DEFAULT NULL

&#x20;               REFERENCES categories(id) ON DELETE SET NULL,

&#x20;   sort\_order  INTEGER NOT NULL DEFAULT 0,

&#x20;   created\_at  DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_categories\_parent ON categories(parent\_id);



\-- ─────────────────────────────────────────────────────────────

\-- SUPPLIERS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS suppliers (

&#x20;   id            INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   company\_name  TEXT    NOT NULL,

&#x20;   contact\_name  TEXT             DEFAULT NULL,

&#x20;   phone         TEXT             DEFAULT NULL,

&#x20;   email         TEXT             DEFAULT NULL,

&#x20;   address       TEXT             DEFAULT NULL,

&#x20;   notes         TEXT             DEFAULT NULL,

&#x20;   created\_at    DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   updated\_at    DATETIME NOT NULL DEFAULT (datetime('now'))

);



\-- ─────────────────────────────────────────────────────────────

\-- INVOICE TEMPLATES

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS invoice\_templates (

&#x20;   id                  INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   name                TEXT    NOT NULL UNIQUE,

&#x20;   is\_default          INTEGER NOT NULL DEFAULT 0 CHECK (is\_default IN (0, 1)),

&#x20;   -- Layout

&#x20;   show\_logo           INTEGER NOT NULL DEFAULT 1,

&#x20;   logo\_position       TEXT    NOT NULL DEFAULT 'left'

&#x20;                               CHECK (logo\_position IN ('left','center','right')),

&#x20;   logo\_width\_px       INTEGER NOT NULL DEFAULT 120,

&#x20;   -- Colors

&#x20;   header\_bg\_color     TEXT    NOT NULL DEFAULT '#2C3E50',

&#x20;   header\_text\_color   TEXT    NOT NULL DEFAULT '#FFFFFF',

&#x20;   accent\_color        TEXT    NOT NULL DEFAULT '#1ABC9C',

&#x20;   table\_header\_color  TEXT    NOT NULL DEFAULT '#ECF0F1',

&#x20;   row\_alt\_color       TEXT    NOT NULL DEFAULT '#F9F9F9',

&#x20;   -- Typography

&#x20;   font\_family         TEXT    NOT NULL DEFAULT 'Arial',

&#x20;   font\_size\_body      INTEGER NOT NULL DEFAULT 10,

&#x20;   font\_size\_header    INTEGER NOT NULL DEFAULT 14,

&#x20;   -- Content toggles

&#x20;   show\_sku            INTEGER NOT NULL DEFAULT 1,

&#x20;   show\_unit           INTEGER NOT NULL DEFAULT 1,

&#x20;   show\_discount\_col   INTEGER NOT NULL DEFAULT 1,

&#x20;   show\_tax\_line       INTEGER NOT NULL DEFAULT 1,

&#x20;   show\_payment\_method INTEGER NOT NULL DEFAULT 1,

&#x20;   show\_signature\_box  INTEGER NOT NULL DEFAULT 0,

&#x20;   show\_qr\_code        INTEGER NOT NULL DEFAULT 1,

&#x20;   -- Custom text blocks

&#x20;   header\_tagline      TEXT             DEFAULT NULL,

&#x20;   footer\_text         TEXT             DEFAULT 'Thank you for your business!',

&#x20;   terms\_text          TEXT             DEFAULT NULL,

&#x20;   -- Paper

&#x20;   paper\_size          TEXT    NOT NULL DEFAULT 'A4'

&#x20;                               CHECK (paper\_size IN ('A4','Letter','A5')),

&#x20;   orientation         TEXT    NOT NULL DEFAULT 'portrait'

&#x20;                               CHECK (orientation IN ('portrait','landscape')),

&#x20;   created\_at          DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   updated\_at          DATETIME NOT NULL DEFAULT (datetime('now'))

);



\-- Seed one default template

INSERT OR IGNORE INTO invoice\_templates

&#x20;   (id, name, is\_default)

VALUES (1, 'Classic Professional', 1);



\-- ─────────────────────────────────────────────────────────────

\-- PRODUCTS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS products (

&#x20;   id           INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   category\_id  INTEGER NOT NULL

&#x20;                REFERENCES categories(id) ON DELETE RESTRICT,

&#x20;   supplier\_id  INTEGER          DEFAULT NULL

&#x20;                REFERENCES suppliers(id) ON DELETE SET NULL,

&#x20;   name         TEXT    NOT NULL,

&#x20;   sku          TEXT    NOT NULL UNIQUE COLLATE NOCASE,

&#x20;   description  TEXT             DEFAULT NULL,

&#x20;   unit\_price   REAL    NOT NULL CHECK (unit\_price >= 0),

&#x20;   cost\_price   REAL    NOT NULL DEFAULT 0.0 CHECK (cost\_price >= 0),

&#x20;   stock\_qty    INTEGER NOT NULL DEFAULT 0,

&#x20;   min\_stock    INTEGER NOT NULL DEFAULT 5,

&#x20;   unit         TEXT    NOT NULL DEFAULT 'piece',

&#x20;   image\_path   TEXT             DEFAULT NULL,

&#x20;   is\_active    INTEGER NOT NULL DEFAULT 1 CHECK (is\_active IN (0, 1)),

&#x20;   created\_at   DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   updated\_at   DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_products\_sku        ON products(sku);

CREATE INDEX IF NOT EXISTS idx\_products\_category   ON products(category\_id);

CREATE INDEX IF NOT EXISTS idx\_products\_name       ON products(name COLLATE NOCASE);



\-- ─────────────────────────────────────────────────────────────

\-- INVOICES

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS invoices (

&#x20;   id              INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   invoice\_number  TEXT    NOT NULL UNIQUE,

&#x20;   customer\_id     INTEGER NOT NULL

&#x20;                   REFERENCES customers(id) ON DELETE RESTRICT,

&#x20;   user\_id         INTEGER NOT NULL

&#x20;                   REFERENCES users(id) ON DELETE RESTRICT,

&#x20;   template\_id     INTEGER NOT NULL DEFAULT 1

&#x20;                   REFERENCES invoice\_templates(id) ON DELETE SET DEFAULT,

&#x20;   status          TEXT    NOT NULL DEFAULT 'draft'

&#x20;                   CHECK (status IN ('draft','confirmed','partial','paid','cancelled','refunded')),

&#x20;   subtotal        REAL    NOT NULL DEFAULT 0.0,

&#x20;   discount\_amount REAL    NOT NULL DEFAULT 0.0,

&#x20;   tax\_amount      REAL    NOT NULL DEFAULT 0.0,

&#x20;   total\_amount    REAL    NOT NULL DEFAULT 0.0,

&#x20;   amount\_paid     REAL    NOT NULL DEFAULT 0.0,

&#x20;   payment\_method  TEXT             DEFAULT NULL

&#x20;                   CHECK (payment\_method IN ('cash','card','bank\_transfer','installment',NULL)),

&#x20;   notes           TEXT             DEFAULT NULL,

&#x20;   invoice\_date    DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   created\_at      DATETIME NOT NULL DEFAULT (datetime('now')),

&#x20;   updated\_at      DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_invoices\_customer    ON invoices(customer\_id);

CREATE INDEX IF NOT EXISTS idx\_invoices\_date        ON invoices(invoice\_date);

CREATE INDEX IF NOT EXISTS idx\_invoices\_status      ON invoices(status);

CREATE INDEX IF NOT EXISTS idx\_invoices\_number      ON invoices(invoice\_number);



\-- ─────────────────────────────────────────────────────────────

\-- INVOICE ITEMS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS invoice\_items (

&#x20;   id           INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   invoice\_id   INTEGER NOT NULL

&#x20;                REFERENCES invoices(id) ON DELETE CASCADE,

&#x20;   product\_id   INTEGER NOT NULL

&#x20;                REFERENCES products(id) ON DELETE RESTRICT,

&#x20;   quantity     INTEGER NOT NULL CHECK (quantity > 0),

&#x20;   unit\_price   REAL    NOT NULL CHECK (unit\_price >= 0),

&#x20;   discount\_pct REAL    NOT NULL DEFAULT 0.0

&#x20;                         CHECK (discount\_pct BETWEEN 0 AND 100),

&#x20;   line\_total   REAL    NOT NULL,

&#x20;   -- Snapshot of product name at time of sale (for historical accuracy)

&#x20;   product\_name TEXT    NOT NULL,

&#x20;   product\_sku  TEXT    NOT NULL

);



CREATE INDEX IF NOT EXISTS idx\_invoice\_items\_invoice ON invoice\_items(invoice\_id);

CREATE INDEX IF NOT EXISTS idx\_invoice\_items\_product ON invoice\_items(product\_id);



\-- ─────────────────────────────────────────────────────────────

\-- PAYMENTS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS payments (

&#x20;   id           INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   invoice\_id   INTEGER NOT NULL

&#x20;                REFERENCES invoices(id) ON DELETE CASCADE,

&#x20;   amount\_paid  REAL    NOT NULL CHECK (amount\_paid > 0),

&#x20;   method       TEXT    NOT NULL DEFAULT 'cash'

&#x20;                         CHECK (method IN ('cash','card','bank\_transfer','installment')),

&#x20;   reference    TEXT             DEFAULT NULL,

&#x20;   notes        TEXT             DEFAULT NULL,

&#x20;   paid\_at      DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_payments\_invoice ON payments(invoice\_id);



\-- ─────────────────────────────────────────────────────────────

\-- STOCK MOVEMENTS (append-only ledger)

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS stock\_movements (

&#x20;   id            INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   product\_id    INTEGER NOT NULL

&#x20;                 REFERENCES products(id) ON DELETE RESTRICT,

&#x20;   user\_id       INTEGER          DEFAULT NULL

&#x20;                 REFERENCES users(id) ON DELETE SET NULL,

&#x20;   movement\_type TEXT    NOT NULL

&#x20;                 CHECK (movement\_type IN (

&#x20;                     'sale','return','purchase','adjustment','damage','transfer'

&#x20;                 )),

&#x20;   quantity      INTEGER NOT NULL,   -- negative = stock out, positive = stock in

&#x20;   stock\_after   INTEGER NOT NULL,   -- snapshot of stock\_qty after this movement

&#x20;   reason        TEXT             DEFAULT NULL,

&#x20;   reference\_doc TEXT             DEFAULT NULL,  -- e.g. "INV-00042"

&#x20;   moved\_at      DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_stock\_movements\_product ON stock\_movements(product\_id);

CREATE INDEX IF NOT EXISTS idx\_stock\_movements\_date    ON stock\_movements(moved\_at);



\-- ─────────────────────────────────────────────────────────────

\-- AUDIT LOGS

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS audit\_logs (

&#x20;   id           INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   user\_id      INTEGER          DEFAULT NULL

&#x20;                REFERENCES users(id) ON DELETE SET NULL,

&#x20;   action       TEXT    NOT NULL

&#x20;                CHECK (action IN ('create','update','delete','login','logout','backup','print','send\_whatsapp')),

&#x20;   entity\_type  TEXT    NOT NULL,   -- e.g. 'invoice', 'product', 'customer'

&#x20;   entity\_id    INTEGER          DEFAULT NULL,

&#x20;   summary      TEXT             DEFAULT NULL,

&#x20;   old\_value    TEXT             DEFAULT NULL,  -- JSON snapshot of before

&#x20;   new\_value    TEXT             DEFAULT NULL,  -- JSON snapshot of after

&#x20;   logged\_at    DATETIME NOT NULL DEFAULT (datetime('now'))

);



CREATE INDEX IF NOT EXISTS idx\_audit\_logs\_entity ON audit\_logs(entity\_type, entity\_id);

CREATE INDEX IF NOT EXISTS idx\_audit\_logs\_date   ON audit\_logs(logged\_at);

CREATE INDEX IF NOT EXISTS idx\_audit\_logs\_user   ON audit\_logs(user\_id);



\-- ─────────────────────────────────────────────────────────────

\-- BACKUP HISTORY

\-- ─────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS backup\_history (

&#x20;   id             INTEGER PRIMARY KEY AUTOINCREMENT,

&#x20;   user\_id        INTEGER          DEFAULT NULL

&#x20;                  REFERENCES users(id) ON DELETE SET NULL,

&#x20;   backup\_type    TEXT    NOT NULL DEFAULT 'github'

&#x20;                  CHECK (backup\_type IN ('github','local')),

&#x20;   file\_path      TEXT             DEFAULT NULL,

&#x20;   github\_commit  TEXT             DEFAULT NULL,

&#x20;   file\_size\_kb   INTEGER          DEFAULT NULL,

&#x20;   status         TEXT    NOT NULL DEFAULT 'success'

&#x20;                  CHECK (status IN ('success','failed','in\_progress')),

&#x20;   error\_message  TEXT             DEFAULT NULL,

&#x20;   backed\_up\_at   DATETIME NOT NULL DEFAULT (datetime('now'))

);



\-- ─────────────────────────────────────────────────────────────

\-- TRIGGERS

\-- ─────────────────────────────────────────────────────────────



\-- Auto-deduct stock when invoice item is inserted

CREATE TRIGGER IF NOT EXISTS trg\_stock\_deduct\_on\_sale

AFTER INSERT ON invoice\_items

BEGIN

&#x20;   UPDATE products

&#x20;   SET    stock\_qty = stock\_qty - NEW.quantity,

&#x20;          updated\_at = datetime('now')

&#x20;   WHERE  id = NEW.product\_id;



&#x20;   INSERT INTO stock\_movements

&#x20;       (product\_id, user\_id, movement\_type, quantity, stock\_after, reason, reference\_doc, moved\_at)

&#x20;   SELECT

&#x20;       NEW.product\_id,

&#x20;       i.user\_id,

&#x20;       'sale',

&#x20;       -NEW.quantity,

&#x20;       p.stock\_qty,

&#x20;       'Invoice sale',

&#x20;       'INV-' || NEW.invoice\_id,

&#x20;       datetime('now')

&#x20;   FROM invoices i

&#x20;   JOIN products p ON p.id = NEW.product\_id

&#x20;   WHERE i.id = NEW.invoice\_id;

END;



\-- Auto-restore stock on invoice item delete (for cancellations)

CREATE TRIGGER IF NOT EXISTS trg\_stock\_restore\_on\_cancel

AFTER DELETE ON invoice\_items

BEGIN

&#x20;   UPDATE products

&#x20;   SET    stock\_qty = stock\_qty + OLD.quantity,

&#x20;          updated\_at = datetime('now')

&#x20;   WHERE  id = OLD.product\_id;



&#x20;   INSERT INTO stock\_movements

&#x20;       (product\_id, movement\_type, quantity, stock\_after, reason, reference\_doc, moved\_at)

&#x20;   SELECT

&#x20;       OLD.product\_id,

&#x20;       'return',

&#x20;       OLD.quantity,

&#x20;       p.stock\_qty,

&#x20;       'Invoice cancelled/item removed',

&#x20;       'INV-' || OLD.invoice\_id,

&#x20;       datetime('now')

&#x20;   FROM products p WHERE p.id = OLD.product\_id;

END;



\-- Update invoice updated\_at timestamp on status change

CREATE TRIGGER IF NOT EXISTS trg\_invoice\_updated\_at

AFTER UPDATE ON invoices

BEGIN

&#x20;   UPDATE invoices SET updated\_at = datetime('now') WHERE id = NEW.id;

END;



\-- Update customer total\_spent when invoice is confirmed

CREATE TRIGGER IF NOT EXISTS trg\_customer\_total\_on\_invoice

AFTER UPDATE OF status ON invoices

WHEN NEW.status = 'paid' AND OLD.status != 'paid'

BEGIN

&#x20;   UPDATE customers

&#x20;   SET    total\_spent = total\_spent + NEW.total\_amount,

&#x20;          updated\_at  = datetime('now')

&#x20;   WHERE  id = NEW.customer\_id;

END;



\-- Auto-generate invoice number

CREATE TRIGGER IF NOT EXISTS trg\_invoice\_number\_generate

AFTER INSERT ON invoices

WHEN NEW.invoice\_number IS NULL OR NEW.invoice\_number = ''

BEGIN

&#x20;   UPDATE invoices

&#x20;   SET invoice\_number = (

&#x20;       SELECT invoice\_prefix || '-' || printf('%05d', invoice\_next\_num)

&#x20;       FROM store\_settings WHERE id = 1

&#x20;   )

&#x20;   WHERE id = NEW.id;



&#x20;   UPDATE store\_settings

&#x20;   SET invoice\_next\_num = invoice\_next\_num + 1

&#x20;   WHERE id = 1;

END;

```



\### Seed Data



```sql

\-- Default categories for a furniture store

INSERT OR IGNORE INTO categories (id, name, description, parent\_id, sort\_order) VALUES

&#x20;   (1,  'Sofas \& Sectionals',    'All sofa types',              NULL, 1),

&#x20;   (2,  'Chairs',                'Accent, dining, office',      NULL, 2),

&#x20;   (3,  'Tables',                'Coffee, dining, side tables', NULL, 3),

&#x20;   (4,  'Beds \& Bedroom',        'Bed frames, headboards',      NULL, 4),

&#x20;   (5,  'Storage',               'Wardrobes, cabinets, shelves',NULL, 5),

&#x20;   (6,  'Outdoor',               'Garden \& patio furniture',    NULL, 6),

&#x20;   (7,  'Sectional Sofas',       'L-shaped \& modular',         1,    1),

&#x20;   (8,  'Loveseat',              'Two-seat sofas',              1,    2),

&#x20;   (9,  'Accent Chairs',         'Decorative single chairs',   2,    1),

&#x20;   (10, 'Office Chairs',         'Ergonomic \& desk chairs',    2,    2),

&#x20;   (11, 'Coffee Tables',         'Low living room tables',     3,    1),

&#x20;   (12, 'Dining Tables',         'Kitchen \& dining sets',      3,    2);

```



\---



\## 6. Project File Structure



```

FurniTrack/

│

├── 📁 Database/

│   ├── schema.sql                    ← Full DDL (tables, indexes, triggers)

│   ├── seed\_data.sql                 ← Default categories, demo data

│   └── furnitrack.db                 ← Generated at first run (not in source)

│

├── 📁 PythonServices/

│   ├── requirements.txt

│   ├── build\_services.bat            ← Runs PyInstaller for all scripts

│   ├── config.py                     ← Shared: reads db\_path from app.json

│   ├── invoice\_printer.py            ← Renders PDF, sends to printer

│   ├── whatsapp\_sender.py            ← Builds wa.me URL, opens browser

│   ├── backup\_service.py             ← Pushes DB to GitHub repo

│   ├── report\_generator.py           ← Sales, stock, customer reports

│   ├── template\_renderer.py          ← Applies invoice\_template record to PDF

│   └── 📁 utils/

│       ├── pdf\_builder.py            ← ReportLab base: page layout, fonts

│       ├── qr\_generator.py           ← QR code for invoice reference

│       └── color\_utils.py            ← Hex to RGB conversion helpers

│

├── 📁 CSharpGUI/

│   ├── FurniTrack.sln

│   └── 📁 FurniTrack/

│       ├── Program.cs                ← Entry point: checks first-run

│       ├── App.config

│       ├── app\_config.json           ← Runtime config: db\_path, python\_services\_dir

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

│       │   ├── SettingsService.cs    ← Load/save store\_settings

│       │   ├── PythonBridge.cs       ← Process.Start wrapper, JSON parsing

│       │   ├── InvoiceService.cs     ← Business logic: totals, numbering

│       │   ├── StockService.cs       ← Low-stock checks, movement queries

│       │   └── BackupService.cs      ← Triggers backup\_service.exe

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

│   ├── build\_all.bat                 ← Master build: C# + Python + NSIS

│   ├── installer.nsi                 ← NSIS installer script

│   └── 📁 Output/                   ← Generated at build time

│

└── 📁 Docs/

&#x20;   ├── FurniTrack\_Development\_Documentation.md  ← This file

&#x20;   └── CHANGELOG.md

```



\---



\## 7. Component 1 — SQLite Database



\### Why This Component Exists



Without a structured database, the application would be unable to enforce data integrity. It would be possible to create an invoice for a product that does not exist, to deduct more stock than is available, or to lose historical invoice data when a product is renamed. SQLite with triggers and foreign keys eliminates all of these risks automatically.



\### What This Component Does



The database is the single source of truth for everything in the application. When the application starts, `DatabaseManager.cs` opens the SQLite connection and immediately runs `PRAGMA foreign\_keys = ON` and `PRAGMA journal\_mode = WAL`. WAL (Write-Ahead Logging) ensures that if the application crashes mid-write, the database is not corrupted.



The `DatabaseManager` also checks if the schema is at the current version by querying a `pragma user\_version`. On first run (user\_version = 0), it executes the full `schema.sql`. On subsequent runs, it checks if migrations are needed and applies them in order. This means the database can be upgraded safely when new versions of the application are released.



\### How It Connects to Everything



Every repository class (`CustomerRepo`, `ProductRepo`, etc.) receives the `DatabaseManager` instance through constructor injection. They never open their own connections. The `DatabaseManager` exposes a `GetConnection()` method that returns the single shared connection, which is thread-safe for the single-user desktop scenario.



\### Completion Criteria for This Stage



\- \[ ] `schema.sql` runs without errors on SQLite 3.x

\- \[ ] All foreign key constraints enforce correctly (test with intentional violations)

\- \[ ] All triggers fire correctly (insert invoice\_item → stock deducts, customer total updates on paid)

\- \[ ] `seed\_data.sql` populates default categories

\- \[ ] `DatabaseManager.cs` opens, checks version, and runs migrations cleanly

\- \[ ] The database file can be opened in DB Browser for SQLite and all tables are visible



\---



\## 8. Component 2 — Python Services Layer



\### Why This Component Exists



C# has limited, expensive, or complex options for high-quality PDF generation with custom branding, QR code embedding, and print-spooler control. Python's ReportLab library produces professional-grade PDFs at zero cost. By separating this concern into a standalone service, the PDF rendering code is independent of the GUI framework and can be updated without recompiling the C# application.



\### Architecture of Python Bridge



C# calls Python services using `Process.Start()`. All communication is through:

\- \*\*Input:\*\* Command-line arguments

\- \*\*Output:\*\* A JSON string written to stdout (always the last line)

\- \*\*Exit codes:\*\* 0 = success, 1 = error



```csharp

// PythonBridge.cs — how C# calls Python

public static ServiceResult Call(string serviceExe, string\[] args) {

&#x20;   var psi = new ProcessStartInfo {

&#x20;       FileName = Path.Combine(ServicesDir, serviceExe),

&#x20;       Arguments = string.Join(" ", args.Select(a => $"\\"{a}\\"")),

&#x20;       RedirectStandardOutput = true,

&#x20;       RedirectStandardError  = true,

&#x20;       UseShellExecute        = false,

&#x20;       CreateNoWindow         = true

&#x20;   };

&#x20;   using var proc = Process.Start(psi);

&#x20;   string output = proc.StandardOutput.ReadToEnd();

&#x20;   proc.WaitForExit();

&#x20;   return JsonSerializer.Deserialize<ServiceResult>(output.Trim().Split('\\n').Last());

}

```



Each Python script ends with:

```python

print(json.dumps({"success": True, "message": "Invoice printed", "path": pdf\_path}))

```



\### Service: invoice\_printer.py



\*\*Purpose:\*\* Receives an invoice ID, queries the database, renders a PDF using the invoice's linked template, and either saves the PDF or sends it to the printer.



\*\*Arguments:\*\*

```

invoice\_printer.exe --invoice-id 42 --action \[pdf|print|preview]

&#x20;                   --db-path "C:\\FurniTrack\\furnitrack.db"

&#x20;                   --output-dir "C:\\FurniTrack\\Exports"

```



\*\*Actions:\*\*

\- `pdf` — Renders PDF to output\_dir, returns `{"path": "...", "success": true}`

\- `print` — Renders PDF to temp file, sends to default printer (or specified printer name from store\_settings)

\- `preview` — Same as `pdf` but saves to system temp dir



\*\*Template loading:\*\* The script reads the `invoice\_templates` record linked to the invoice and builds the ReportLab document using those values. Every visual property (colors, fonts, logo position, footer text) comes from the template record.



\### Service: whatsapp\_sender.py



\*\*Purpose:\*\* Builds a `wa.me` URL with the customer's WhatsApp number and a pre-formatted invoice message, then opens it in the default browser. WhatsApp Desktop intercepts this URL and opens a compose window.



\*\*The URL format:\*\*

```

https://wa.me/\[country\_code]\[number]?text=\[url\_encoded\_message]

```



\*\*Example message built by the script:\*\*

```

Hello Mohamed,



Your invoice INV-00042 from My Furniture Store is ready.



Items:

&#x20; - Sofa L-Shape (x1)         4,500 EGP

&#x20; - Coffee Table Marble (x1)    850 EGP



Subtotal:   5,350 EGP

Discount:     250 EGP

Tax (14%):    715 EGP

TOTAL:      5,815 EGP



Payment: Cash — PAID



Thank you for shopping with us!

📞 Contact: 01012345678

```



\*\*Arguments:\*\*

```

whatsapp\_sender.exe --invoice-id 42 --db-path "..." 

```



\*\*How the operator uses it:\*\*

1\. Click "Send WhatsApp" button in the invoice view

2\. WhatsApp Desktop opens with the message pre-filled and the customer's number already set

3\. The operator presses Enter to send

4\. An audit log entry is written: `{action: 'send\_whatsapp', entity\_type: 'invoice', entity\_id: 42}`



\*\*Phone number normalization:\*\*

The script strips non-numeric characters, removes leading zeros, and prepends the country code from `store\_settings.whatsapp\_number` prefix. Egyptian numbers starting with `0` become `20\[rest of number]`.



\### Service: backup\_service.py



\*\*Purpose:\*\* Copies the SQLite database file to a private GitHub repository as a backup.



\*\*Method:\*\*

1\. Reads `github\_repo`, `github\_token`, and `github\_branch` from `store\_settings`

2\. Reads the current database file as binary

3\. Uses the GitHub Contents API (single REST call, no git client required):

&#x20;  ```

&#x20;  PUT /repos/{owner}/{repo}/contents/backups/furnitrack\_{datetime}.db

&#x20;  Authorization: Bearer {github\_token}

&#x20;  Body: {"message": "Auto-backup", "content": "\[base64-encoded file]"}

&#x20;  ```

4\. Returns the commit SHA

5\. C# writes a record to `backup\_history`



\*\*No git client is installed on the user's machine.\*\* The GitHub API accepts the file directly as base64 over HTTPS.



\### Service: template\_renderer.py



\*\*Purpose:\*\* The core PDF engine. Reads an `invoice\_templates` record and applies every visual property to the ReportLab document. Called by `invoice\_printer.py`.



\*\*Key rendering decisions:\*\*

\- The store logo is loaded from `store\_settings.logo\_path`. If missing, the space is filled with the store name in large text.

\- All currency amounts are formatted with `store\_settings.currency\_symbol` and comma separation.

\- The QR code (if `show\_qr\_code = 1`) encodes the invoice number and store phone, giving the customer a reference they can show in store.

\- Signature block (if `show\_signature\_box = 1`) renders two labeled horizontal lines: "Customer Signature" and "Authorized Signature."



\### Service: report\_generator.py



\*\*Purpose:\*\* Generates PDF reports for common business analytics queries.



\*\*Report types:\*\*

\- `sales\_summary` — Daily/weekly/monthly revenue, invoice count, average order value

\- `stock\_status` — All products with current qty, min\_stock, status (OK / LOW / OUT)

\- `customer\_ranking` — Top customers by total spent

\- `product\_ranking` — Best-selling products by quantity

\- `audit\_trail` — Recent audit log entries, filterable by user and action



\### Completion Criteria for This Stage



\- \[ ] `invoice\_printer.exe` renders a multi-item invoice PDF correctly from all 3 default templates

\- \[ ] PDF includes logo, QR code, all line items, totals, and footer text

\- \[ ] `whatsapp\_sender.exe` opens WhatsApp Desktop with correct number and formatted message

\- \[ ] `backup\_service.exe` successfully pushes to a test private GitHub repo and returns commit SHA

\- \[ ] `report\_generator.exe` renders all 5 report types without error

\- \[ ] All scripts return valid JSON on stdout for both success and error cases

\- \[ ] All scripts compiled to single `.exe` via PyInstaller with no external dependencies



\---



\## 9. Component 3 — C# WinForms GUI Application



\### Why This Component Exists



The GUI is the operator's entire experience. Every other component — the database, the Python services — exists to serve the GUI. The GUI must be reliable, fast, and obvious. A furniture store cashier should be able to create a complete invoice in under 60 seconds without training.



\### Application Startup Sequence



```

Program.cs

&#x20;   │

&#x20;   ├── Load app\_config.json (db\_path, services\_dir)

&#x20;   │

&#x20;   ├── DatabaseManager.Initialize(dbPath)

&#x20;   │       └── Apply schema / run migrations

&#x20;   │

&#x20;   ├── SettingsService.Load()

&#x20;   │

&#x20;   ├── IF store\_settings has no data → SplashForm (wizard)

&#x20;   │       └── After wizard: SaveSettings(), create admin user

&#x20;   │

&#x20;   └── LoginForm.ShowDialog()

&#x20;           └── On success: MainForm.Show()

```



\### MainForm — MDI Shell



The `MainForm` is an MDI (Multiple Document Interface) container with:

\- A fixed left sidebar (220px wide, dark background `#2C3E50`) containing navigation buttons

\- A top toolbar strip showing: store name, logged-in user, current date/time

\- A bottom status strip showing: database path, last backup time, low-stock alert count

\- The MDI client area (right side) where child forms open



Navigation items in sidebar:

\- Dashboard

\- New Invoice \*(highlighted in accent color)\*

\- All Invoices

\- Products

\- Customers

\- Invoice Templates

\- Reports

\- Settings

\- Backup Now



\### InvoiceNewForm — Core Feature



This is the most important form in the application. It must handle the complete sale flow.



\*\*Layout — two-panel:\*\*

\- \*\*Left panel (60% width):\*\* Invoice line items table with add/remove rows

\- \*\*Right panel (40% width):\*\* Customer selector, invoice date, payment method, notes, running totals



\*\*Customer selection:\*\*

\- Search box with autocomplete (queries customers by name or phone as user types)

\- "New Customer" button opens a minimal inline popup (name + phone only — other details optional)

\- Once selected, shows customer name and WhatsApp number with a phone icon



\*\*Product entry rows:\*\*

Each row in the items table has:

\- Product search (autocomplete by name or SKU, shows current stock in dropdown)

\- Quantity spinner (default 1, max = current stock\_qty)

\- Unit price (pre-filled from product, editable)

\- Discount % (default 0, 0–100)

\- Line total (auto-calculated, read-only)



\*\*Totals panel (bottom right):\*\*

```

Subtotal:         5,350.00 EGP

Discount:          -250.00 EGP

Tax (14%):          715.00 EGP

────────────────────────────

TOTAL:            5,815.00 EGP

Amount Paid:      5,815.00 EGP  \[input field]

Change Due:           0.00 EGP

```



\*\*Save behavior:\*\*

\- Saves as `draft` unless "Confirm \& Save" is clicked

\- "Confirm \& Save" transitions status to `confirmed` or `paid` depending on amount\_paid

\- After save, the action bar appears: \[🖨 Print] \[📲 Send WhatsApp] \[📄 View PDF] \[➕ New Invoice]



\### TemplateEditorForm — Invoice Template Designer



This form allows the user to configure how their invoices look. It has two panels side-by-side:



\*\*Left panel — controls:\*\*

\- Template name (text input)

\- Logo position: radio buttons (Left / Center / Right)

\- Logo width: numeric slider

\- Header background color: color picker button (opens system color dialog)

\- Header text color: color picker

\- Accent color: color picker

\- Font family: dropdown (Arial, Times New Roman, Calibri, Tahoma)

\- Body font size: numeric input

\- Toggle checkboxes: Show SKU, Show unit, Show discount column, Show tax line, Show payment method, Show QR code, Show signature box

\- Footer text: multiline text input

\- Terms text: multiline text input (optional, smaller text at bottom of invoice)

\- Paper size: A4 / Letter / A5



\*\*Right panel — live preview:\*\*

\- A `PictureBox` (or `WebBrowser` control) showing a rendered thumbnail of the invoice with the current settings

\- The preview updates when the operator clicks "Preview" or after a 1-second debounce on any change

\- Preview is generated by calling `template\_renderer.exe --preview --template-json "\[json]"` with the current unsaved settings



\*\*Save behavior:\*\*

\- "Save Template" writes or updates the `invoice\_templates` record

\- "Set as Default" sets `is\_default = 1` on this template (and clears it on all others)

\- Up to 10 named templates can be saved



\### Settings Form



Single-page tabbed form with tabs: \*\*Store Info | Printer | WhatsApp | Backup | Users | Display\*\*



All fields pre-populated from `store\_settings`. Changes saved on "Save" click. No auto-save to prevent accidental overwrites.



\*\*Store Info tab:\*\*

\- Store name, owner name, phone, address, logo upload (browse to image file)

\- Tax rate (%), currency symbol, invoice prefix



\*\*WhatsApp tab:\*\*

\- Default WhatsApp number (country code included)

\- Message template preview (shows how the invoice message will look)

\- "Test WhatsApp" button opens a test message to the configured number



\*\*Backup tab:\*\*

\- GitHub repository (format: `owner/repo`)

\- GitHub personal access token (masked input, "Show" toggle)

\- Auto-backup on exit: Yes / No

\- "Backup Now" button → triggers backup immediately and shows result



\### GUI Design Principles



\*\*Consistency:\*\*

\- Every list form has: search bar (top left), filter dropdown (top right), data grid (center), action buttons (bottom or right)

\- Every edit form has: labeled fields in a single column or two-column layout, "Save" and "Cancel" buttons (bottom right)

\- No floating toolbars, no ribbon, no right-click context menus (too hidden — all actions are visible buttons)



\*\*Color scheme:\*\*

\- Sidebar: `#2C3E50` (dark blue-gray)

\- Accent / hover: `#1ABC9C` (teal green)

\- Background: `#FFFFFF` (white)

\- Grid header: `#ECF0F1` (light gray)

\- Error/warning: `#E74C3C` (red)

\- Font: Segoe UI, 9pt (system default on Windows 10+)



\*\*Keyboard shortcuts:\*\*

\- `F1` — New Invoice

\- `F2` — Customer List

\- `F3` — Product List

\- `Ctrl+P` — Print current invoice

\- `Ctrl+W` — Send WhatsApp

\- `Escape` — Cancel / Close current child form



\### Completion Criteria for This Stage



\- \[ ] Application starts, first-run wizard completes, settings are saved and not asked again on next launch

\- \[ ] Login form validates password against bcrypt hash correctly

\- \[ ] Sidebar navigates to all forms without error

\- \[ ] New invoice with 3+ items can be created, saved, and the stock deduction is visible in the Products list

\- \[ ] WhatsApp button opens WhatsApp Desktop with the correct number and formatted message

\- \[ ] Print button sends PDF to selected printer

\- \[ ] Template editor saves and the new template appears in the invoice form template dropdown

\- \[ ] Settings form saves and values persist on restart



\---



\## 10. Invoice Template System



\### Problem This Solves



A fixed invoice layout makes the store look generic. A custom layout with the store's colors, logo placement, and footer text makes the invoice a brand touchpoint — the last thing a customer sees before leaving.



\### Template Storage



Templates are stored in the `invoice\_templates` table (see schema above). Each record holds every visual and content decision as individual columns. This means:

\- Templates can be duplicated and modified

\- The exact template used for an old invoice is preserved even if the template is later edited (because the invoice stores `template\_id`, and the template record is a named version)

\- If a template is deleted, invoices that used it fall back to the default template



\### Template Variables Available



| Variable | Source |

|---|---|

| `{{store\_name}}` | store\_settings.store\_name |

| `{{store\_address}}` | store\_settings.address |

| `{{store\_phone}}` | store\_settings.phone |

| `{{store\_logo}}` | store\_settings.logo\_path |

| `{{invoice\_number}}` | invoices.invoice\_number |

| `{{invoice\_date}}` | invoices.invoice\_date |

| `{{customer\_name}}` | customers.full\_name |

| `{{customer\_phone}}` | customers.phone |

| `{{subtotal}}` | invoices.subtotal |

| `{{discount}}` | invoices.discount\_amount |

| `{{tax}}` | invoices.tax\_amount |

| `{{total}}` | invoices.total\_amount |

| `{{payment\_method}}` | invoices.payment\_method |

| `{{footer\_text}}` | invoice\_templates.footer\_text |

| `{{terms\_text}}` | invoice\_templates.terms\_text |



\### Three Built-In Templates (Seeded at Install)



\*\*Template 1 — Classic Professional\*\*

\- Dark navy header (`#2C3E50`) with white text

\- Teal accent lines

\- Logo left-aligned

\- Full columns: SKU, description, qty, unit price, discount, total

\- QR code bottom-right



\*\*Template 2 — Modern Minimal\*\*

\- White header, accent color underline only

\- Logo centered, large

\- No SKU column, no discount column

\- Large total amount in accent color

\- No QR code, signature box visible



\*\*Template 3 — Compact Receipt\*\*

\- A5 paper size

\- Single-column narrow layout

\- No logo

\- Line items condensed (description and total only)

\- Suitable for printing on receipt printers



\---



\## 11. WhatsApp Integration (Free, No API)



\### Problem This Solves



Customers expect to receive their invoices on WhatsApp immediately after purchase. The WhatsApp Business API costs money and requires Meta approval. WhatsApp Desktop is already installed on the operator's machine and supports a deep-link URL that pre-composes a message.



\### How It Works — Technical Detail



1\. The operator clicks "Send WhatsApp" in the invoice view

2\. C# calls `PythonBridge.Call("whatsapp\_sender.exe", \["--invoice-id", invoiceId.ToString(), "--db-path", dbPath])`

3\. `whatsapp\_sender.py` queries the database for the invoice, customer, and items

4\. It builds the message string (see example in Section 8)

5\. It URL-encodes the message: `urllib.parse.quote(message)`

6\. It builds: `url = f"https://wa.me/{normalized\_phone}?text={encoded\_message}"`

7\. It calls: `webbrowser.open(url)` — this opens the default browser

8\. If WhatsApp Desktop is installed and the browser recognizes the `wa.me` URL, it passes control to WhatsApp Desktop

9\. WhatsApp Desktop opens with the customer's chat and the message pre-filled

10\. The operator presses Enter to send

11\. The script returns `{"success": true, "message": "WhatsApp opened"}` to stdout

12\. C# writes an audit log entry and shows a success toast notification



\### Phone Number Formatting Rules



Egyptian numbers:

\- `01012345678` → `+201012345678`

\- `+201012345678` → `+201012345678` (no change)

\- `201012345678` → `+201012345678`



The `whatsapp` field on the customer record stores the number as entered. The `whatsapp\_sender.py` normalizes it using the country code derived from `store\_settings.whatsapp\_number`.



\### Operator Experience



The entire flow from "click button" to "ready to press Enter in WhatsApp" takes approximately 2–4 seconds. The operator does not need to type the customer's number or copy any text. They review the pre-composed message (which is already correct) and press Enter.



\---



\## 12. First-Run Setup \& Preferences



\### Problem This Solves



Every application restart should feel like continuing where you left off, not starting over. Preferences — printer name, tax rate, currency, template choice — should be set once and remembered forever.



\### First-Run Detection



On startup, `Program.cs` calls `SettingsService.IsFirstRun()`:

```csharp

public static bool IsFirstRun() {

&#x20;   var settings = Load();

&#x20;   return string.IsNullOrWhiteSpace(settings.StoreName) || settings.StoreName == "My Furniture Store";

}

```



If `IsFirstRun()` returns true, `SplashForm` opens as a modal wizard before `LoginForm`.



\### Wizard Steps



\*\*Step 1 — Store Information\*\*

\- Store name (required)

\- Owner name (required)

\- Phone number (required)

\- Address (required, shown on invoices)



\*\*Step 2 — Logo \& Branding\*\*

\- Logo upload: Browse button opens file dialog filtered to `\*.png;\*.jpg;\*.jpeg`

\- Logo preview shown at 200px wide

\- "Skip for now" link available

\- Theme color picker (sets `store\_settings.theme\_color` used in sidebar)



\*\*Step 3 — Tax \& Currency\*\*

\- Currency symbol (default: EGP)

\- Tax rate % (default: 0, operator enters their local rate)

\- Invoice number prefix (default: INV, can be store initials like MFS)

\- Starting invoice number (default: 1)



\*\*Step 4 — WhatsApp \& Printer\*\*

\- WhatsApp number for sending (with country code)

\- Printer selection: dropdown populated by `PrinterSettings.InstalledPrinters` in C#

\- "Test Print" button prints a test page to confirm



\*\*Step 5 — Admin Account\*\*

\- Username (required, no spaces)

\- Password (required, minimum 6 characters)

\- Confirm password

\- "Create Account \& Finish" button saves everything and closes wizard



\### Preferences That Are Auto-Saved



| Preference | When Saved |

|---|---|

| Last used invoice template | When an invoice is confirmed |

| Last active tab in reports | On tab change |

| Window size and position | On form close |

| Last search query in each list | On form close |

| Column widths in DataGridViews | On form close |

| Selected printer | In settings form |



Window geometry preferences use `app\_config.json` (not the database) since they are machine-specific, not store-specific.



\---



\## 13. GitHub Backup Service



\### Problem This Solves



All business data lives in a single SQLite file on a single machine. If that machine fails, the data is gone. GitHub provides unlimited free private repositories. Pushing the database file to GitHub gives the owner a cloud backup without any subscription or server.



\### How the Backup Works



The GitHub Contents API allows creating or updating files in a repository using a simple HTTPS PUT request with a personal access token. No git client needs to be installed.



```

PUT https://api.github.com/repos/{owner}/{repo}/contents/backups/furnitrack\_{timestamp}.db

Authorization: Bearer {github\_token}

Content-Type: application/json



{

&#x20; "message": "Auto-backup: My Furniture Store 2024-03-15 09:30",

&#x20; "content": "\[base64-encoded .db file]",

&#x20; "branch": "main"

}

```



The response includes the commit SHA, which is stored in `backup\_history.github\_commit`.



\### Backup Frequency



\- \*\*Auto-backup on exit:\*\* If `store\_settings.auto\_backup = 1`, the backup runs when the user closes the application. A progress dialog shows "Backing up to GitHub..." and the user sees success/failure before the window closes.

\- \*\*Manual backup:\*\* "Backup Now" button in the sidebar and in Settings runs the backup immediately.

\- \*\*Daily auto-backup:\*\* If the application runs for more than 8 hours (e.g., on a dedicated point-of-sale machine), a background timer triggers a backup every 24 hours.



\### Security Considerations



The GitHub personal access token is stored in `store\_settings.github\_token`. This field is stored in the SQLite database. The database file is on the user's local machine. The token should be scoped to `repo` permissions only on the specific backup repository. Instructions for creating a scoped token are included in the Settings form's help text.



\---



\## 14. Audit \& Logging System



\### Problem This Solves



In a business context, knowing what happened and who did it is essential. If a product price changes and old invoices look wrong, the audit log shows when the price was changed and by whom. If an invoice is deleted, the audit log shows it was deleted and by which user.



\### What Gets Logged



| Action | Trigger |

|---|---|

| `login` | User authenticates successfully |

| `logout` | User closes application or logs out manually |

| `create` | Any INSERT on invoices, customers, products, invoice\_templates |

| `update` | Any UPDATE on invoices (status change logged with old/new status), products (price change logged with old/new price) |

| `delete` | Any DELETE on invoices, customers, products |

| `print` | Invoice sent to printer |

| `send\_whatsapp` | WhatsApp opened for an invoice |

| `backup` | Backup run (success or failure) |



\### Log Record Structure



```sql

INSERT INTO audit\_logs (user\_id, action, entity\_type, entity\_id, summary, old\_value, new\_value)

VALUES (

&#x20;   2,                    -- ID of the logged-in cashier

&#x20;   'update',             -- what happened

&#x20;   'invoice',            -- which table

&#x20;   42,                   -- which record

&#x20;   'Status changed to paid',

&#x20;   '{"status": "confirmed", "amount\_paid": 0}',    -- before (JSON)

&#x20;   '{"status": "paid", "amount\_paid": 5815.00}'    -- after (JSON)

);

```



\### Viewing Logs



The Reports form has an "Audit Trail" tab showing the last 1000 audit log entries in a searchable grid. Admins can filter by: user, action type, entity type, and date range.



\---



\## 15. Build \& Distribution



\### Build Process



The master `build\_all.bat` script performs the following in order:



\*\*Step 1 — Build Python services:\*\*

```bat

cd PythonServices

pip install -r requirements.txt

pyinstaller --onefile --name invoice\_printer   invoice\_printer.py

pyinstaller --onefile --name whatsapp\_sender   whatsapp\_sender.py

pyinstaller --onefile --name backup\_service    backup\_service.py

pyinstaller --onefile --name report\_generator  report\_generator.py

pyinstaller --onefile --name template\_renderer template\_renderer.py

```



\*\*Step 2 — Build C# application:\*\*

```bat

cd CSharpGUI

dotnet publish FurniTrack/FurniTrack.csproj ^

&#x20; -c Release ^

&#x20; -r win-x64 ^

&#x20; --self-contained true ^

&#x20; -p:PublishSingleFile=true ^

&#x20; -p:IncludeNativeLibrariesForSelfExtract=true ^

&#x20; -o ../Build/Output/app

```



\*\*Step 3 — Assemble distribution folder:\*\*

```bat

xcopy /Y /E PythonServices/dist/\*.exe Build/Output/services/

xcopy /Y Database/schema.sql Build/Output/

xcopy /Y Database/seed\_data.sql Build/Output/

```



\*\*Step 4 — Create installer:\*\*

```bat

makensis Build/installer.nsi

```



\### NSIS Installer Script (installer.nsi)



The installer:

\- Creates `C:\\Program Files\\FurniTrack\\` directory structure

\- Copies all files to installation directory

\- Creates desktop shortcut pointing to `FurniTrack.exe`

\- Creates Start Menu entry

\- Registers in Windows Add/Remove Programs

\- On uninstall: removes all installed files but \*\*leaves the database file\*\* (user data is never deleted by uninstall)

\- Configures `app\_config.json` with default database path: `%APPDATA%\\FurniTrack\\furnitrack.db`



\### app\_config.json



This file is the only file the application reads before accessing the database. It contains machine-specific paths:

```json

{

&#x20; "db\_path": "C:\\\\Users\\\\User\\\\AppData\\\\Roaming\\\\FurniTrack\\\\furnitrack.db",

&#x20; "services\_dir": "C:\\\\Program Files\\\\FurniTrack\\\\services",

&#x20; "exports\_dir": "C:\\\\Users\\\\User\\\\Documents\\\\FurniTrack\\\\Exports",

&#x20; "log\_level": "info"

}

```



`app\_config.json` lives at `%APPDATA%\\FurniTrack\\app\_config.json` — never in Program Files (which may be read-only).



\### Final Distribution Contents



```

FurniTrack\_Setup\_v1.0.0.exe    ← Single installer, \~80MB

```



What the installer puts on the machine:

```

C:\\Program Files\\FurniTrack\\

├── FurniTrack.exe              ← Main application (\~60MB self-contained)

└── services\\

&#x20;   ├── invoice\_printer.exe     ← PDF + print service

&#x20;   ├── whatsapp\_sender.exe     ← WhatsApp URL builder

&#x20;   ├── backup\_service.exe      ← GitHub backup

&#x20;   ├── report\_generator.exe    ← Report PDFs

&#x20;   └── template\_renderer.exe  ← Invoice template engine



C:\\Users\\\[User]\\AppData\\Roaming\\FurniTrack\\

├── app\_config.json

└── furnitrack.db               ← All business data



C:\\Users\\\[User]\\Documents\\FurniTrack\\Exports\\

└── \[generated PDFs appear here]

```



\---



\## 16. Development Stages — Step by Step



\### Stage 1 — Database Foundation

\*\*Files created:\*\* `schema.sql`, `seed\_data.sql`, `DatabaseManager.cs`  

\*\*Goal:\*\* A working, tested SQLite database with all tables, triggers, indexes, and seed data.  

\*\*Verification:\*\* Open DB Browser for SQLite, confirm all tables exist and triggers fire on test INSERTs.  

\*\*After this stage:\*\* The data layer is complete. All subsequent stages can query real data.



\---



\### Stage 2 — Python Services Foundation

\*\*Files created:\*\* `config.py`, `utils/pdf\_builder.py`, `utils/qr\_generator.py`, `utils/color\_utils.py`, `requirements.txt`  

\*\*Goal:\*\* Shared utilities used by all Python services are written and tested.  

\*\*Verification:\*\* Running `pdf\_builder.py` as a standalone script produces a test PDF with a colored header and a QR code.  

\*\*After this stage:\*\* Individual Python services can be written quickly by composing these utilities.



\---



\### Stage 3 — Invoice Template Renderer

\*\*Files created:\*\* `template\_renderer.py`  

\*\*Goal:\*\* Given a template record (as JSON) and invoice data (as JSON), produce a correctly styled PDF.  

\*\*Verification:\*\* 3 different template configurations produce 3 visually distinct PDFs with all invoice data correct.  

\*\*After this stage:\*\* Any service that needs to produce an invoice PDF simply calls `template\_renderer`.



\---



\### Stage 4 — Invoice Printer Service

\*\*Files created:\*\* `invoice\_printer.py`  

\*\*Goal:\*\* Full service that reads invoice from SQLite, calls template\_renderer, returns PDF path or sends to printer.  

\*\*Verification:\*\* `invoice\_printer.exe --invoice-id 1 --action pdf` produces a PDF. `--action print` sends it to the default printer.  

\*\*After this stage:\*\* C# can print any invoice with a single function call.



\---



\### Stage 5 — WhatsApp Sender Service

\*\*Files created:\*\* `whatsapp\_sender.py`  

\*\*Goal:\*\* Builds WhatsApp deep-link URL and opens it in the default browser.  

\*\*Verification:\*\* Running the script opens WhatsApp Desktop with correct number and correctly formatted message.  

\*\*After this stage:\*\* C# can trigger WhatsApp message composition for any invoice.



\---



\### Stage 6 — Backup Service

\*\*Files created:\*\* `backup\_service.py`  

\*\*Goal:\*\* Pushes database file to GitHub repository using the Contents API.  

\*\*Verification:\*\* After running, the GitHub repository contains the database file and a new commit is visible.  

\*\*After this stage:\*\* Automated cloud backup is fully functional.



\---



\### Stage 7 — Report Generator Service

\*\*Files created:\*\* `report\_generator.py`  

\*\*Goal:\*\* All 5 report types produce correct PDFs.  

\*\*Verification:\*\* Each report type tested with real data shows correct figures matching manual database queries.  

\*\*After this stage:\*\* All Python services are complete and compiled to `.exe` files.



\---



\### Stage 8 — C# Project Scaffold

\*\*Files created:\*\* All `.csproj`, `Program.cs`, `App.config`, `app\_config.json`, all Repository files, all Model files  

\*\*Goal:\*\* The C# project compiles and connects to the database. All repositories have working CRUD methods.  

\*\*Verification:\*\* A unit-test console app successfully creates a customer, creates an invoice with items, and reads it back.  

\*\*After this stage:\*\* The data access layer is complete. Forms can be built on top of it.



\---



\### Stage 9 — Services Layer (C#)

\*\*Files created:\*\* `AuthService.cs`, `SettingsService.cs`, `PythonBridge.cs`, `InvoiceService.cs`, `StockService.cs`, `BackupService.cs`  

\*\*Goal:\*\* Business logic is centralized in service classes. `PythonBridge.cs` calls Python `.exe` files and parses JSON results correctly.  

\*\*Verification:\*\* `PythonBridge.Call("invoice\_printer.exe", \[...])` returns a correctly parsed `ServiceResult`.  

\*\*After this stage:\*\* Forms only need to call Services — they contain no direct database queries.



\---



\### Stage 10 — First-Run Wizard \& Login

\*\*Files created:\*\* `SplashForm.cs`, `LoginForm.cs`  

\*\*Goal:\*\* Fresh install shows the 5-step wizard, saves settings, creates admin user, and on next launch goes directly to login.  

\*\*Verification:\*\* Delete `store\_settings` row, restart app — wizard appears. Complete wizard, restart — login appears, wizard does not.  

\*\*After this stage:\*\* The application bootstraps correctly on any machine.



\---



\### Stage 11 — MainForm Shell \& Sidebar

\*\*Files created:\*\* `MainForm.cs`, `SidebarPanel.cs`, `StatusStrip.cs`  

\*\*Goal:\*\* MDI shell with working sidebar navigation. All nav buttons open placeholder child forms.  

\*\*Verification:\*\* All sidebar buttons respond. The status bar shows correct user and date. Low-stock count appears in status bar.  

\*\*After this stage:\*\* The application has its frame. Individual forms can be built and plugged in.



\---



\### Stage 12 — Dashboard

\*\*Files created:\*\* `DashboardForm.cs`  

\*\*Goal:\*\* Summary tiles showing today's revenue, invoice count, low-stock products, and top-selling product of the week.  

\*\*Verification:\*\* Data in tiles matches manual SQL queries.  

\*\*After this stage:\*\* Management can see the store's daily status at a glance.



\---



\### Stage 13 — Products \& Categories CRUD

\*\*Files created:\*\* `ProductListForm.cs`, `ProductEditForm.cs`, `StockAdjustForm.cs`  

\*\*Goal:\*\* Full product management: list, search, add, edit, deactivate, manual stock adjustment.  

\*\*Verification:\*\* Adding a product, then finding it in search, editing its price, and adjusting stock — all operations complete without error.  

\*\*After this stage:\*\* The product catalog is fully manageable.



\---



\### Stage 14 — Customers CRUD

\*\*Files created:\*\* `CustomerListForm.cs`, `CustomerEditForm.cs`, `CustomerHistoryForm.cs`  

\*\*Goal:\*\* Full customer management with invoice history view.  

\*\*Verification:\*\* A customer can be created, their invoices viewed in history, and their total\_spent matches the sum of their paid invoices.  

\*\*After this stage:\*\* Customer management is complete.



\---



\### Stage 15 — New Invoice Form (Core)

\*\*Files created:\*\* `InvoiceNewForm.cs`, `InvoiceActionBar.cs`  

\*\*Goal:\*\* Complete invoice creation: customer selection, item addition, quantity/price/discount editing, save, action buttons.  

\*\*Verification:\*\* Creating an invoice with 3 items reduces stock of each product by the correct quantity. Invoice appears in list. PDF is generated and correct.  

\*\*After this stage:\*\* The application can perform its primary function — selling.



\---



\### Stage 16 — Invoice List \& View

\*\*Files created:\*\* `InvoiceListForm.cs`, `InvoiceViewForm.cs`  

\*\*Goal:\*\* Searchable invoice list with filters (status, date range, customer). Invoice view shows all details and has Print/WhatsApp/PDF buttons.  

\*\*Verification:\*\* An invoice created in Stage 15 appears in the list and can be viewed, printed, and sent to WhatsApp.  

\*\*After this stage:\*\* Full invoice lifecycle is visible and actionable.



\---



\### Stage 17 — Invoice Template Editor

\*\*Files created:\*\* `TemplateListForm.cs`, `TemplateEditorForm.cs`, `TemplatePreviewForm.cs`  

\*\*Goal:\*\* Visual template designer with live PDF preview. At least 3 saved templates selectable per invoice.  

\*\*Verification:\*\* Creating a new template with custom colors, saving it, then generating an invoice using that template produces a PDF that matches the preview.  

\*\*After this stage:\*\* The invoice template system is fully operational.



\---



\### Stage 18 — Reports

\*\*Files created:\*\* `ReportsForm.cs`  

\*\*Goal:\*\* All 5 report types generate and display correctly. Reports can be exported to PDF.  

\*\*Verification:\*\* Sales summary report matches manual sum of invoices in date range. Stock report shows correct quantities.  

\*\*After this stage:\*\* Business analytics are available.



\---



\### Stage 19 — Settings Form

\*\*Files created:\*\* `SettingsForm.cs`  

\*\*Goal:\*\* All settings tabs working. Changes persist on restart. Backup Now button works.  

\*\*Verification:\*\* Change store name in settings, restart app — sidebar shows new store name. Backup Now pushes to GitHub.  

\*\*After this stage:\*\* All configuration is self-service.



\---



\### Stage 20 — Build, Packaging \& Testing

\*\*Files created:\*\* `build\_all.bat`, `installer.nsi`  

\*\*Goal:\*\* Complete end-to-end build produces a single setup `.exe` that installs cleanly on a fresh Windows 10 machine.  

\*\*Verification:\*\* Fresh VM, run installer, open app — full workflow from first-run wizard to creating, printing, and WhatsApping an invoice completes without any missing dependencies or errors.  

\*\*After this stage:\*\* The application is ready for production use.



\---



\## 17. Stage Completion Checklist



\### Per-Stage Sign-Off Criteria



Before marking a stage complete and starting the next, all of the following must be true:



\- \[ ] All files for the stage are created with full, working code (no placeholder `// TODO` methods)

\- \[ ] The feature works end-to-end with real data from the database

\- \[ ] All edge cases handled: empty states (no customers, no products), invalid input (letters in quantity field), missing optional data (customer has no WhatsApp number)

\- \[ ] No unhandled exceptions — all database and service calls wrapped in try/catch with user-friendly error messages

\- \[ ] Any new database query is tested with both the happy path and an empty result set

\- \[ ] Python services return valid JSON for both success and failure cases



\### Final Pre-Release Checklist



\- \[ ] All 20 stages complete and signed off

\- \[ ] Fresh Windows 10 VM install test passes completely

\- \[ ] Installer creates correct file structure, shortcuts work, uninstall removes application files but preserves database

\- \[ ] Application handles disconnected scenarios: Python service `.exe` missing (clear error), database file missing (guided recovery)

\- \[ ] Invoice PDF output reviewed by a human for visual quality

\- \[ ] WhatsApp message format reviewed — correct phone normalization for Egyptian numbers

\- \[ ] GitHub backup verified by restoring the database file from GitHub to a new machine

\- \[ ] All audit log actions write correct records

\- \[ ] Low-stock alert appears in status bar when a product is below min\_stock

\- \[ ] Application closes cleanly (no hanging processes from Python service calls)



\---



\## 18. Coding Standards \& Conventions



\### C# Conventions



\*\*Naming:\*\*

\- Classes, Properties, Public Methods: `PascalCase` — `InvoiceService`, `TotalAmount`, `GetByCustomerId()`

\- Private fields: `\_camelCase` — `\_dbManager`, `\_settings`

\- Local variables and parameters: `camelCase` — `invoiceId`, `customerId`

\- Constants: `UPPER\_SNAKE\_CASE` — `MAX\_DISCOUNT\_PCT`



\*\*Repository pattern:\*\*

Every repository implements `IRepository<T>`:

```csharp

public interface IRepository<T> {

&#x20;   T? GetById(int id);

&#x20;   List<T> GetAll();

&#x20;   int Insert(T entity);

&#x20;   bool Update(T entity);

&#x20;   bool Delete(int id);

}

```

Repositories return `null` (not exceptions) when a record is not found.



\*\*Error handling:\*\*

All service methods return a result object, never throw to the caller:

```csharp

public class ServiceResult<T> {

&#x20;   public bool Success { get; set; }

&#x20;   public T? Data     { get; set; }

&#x20;   public string Error { get; set; } = "";

}

```



\*\*No magic strings:\*\*

Database table and column names are `const string` values in each repository class.



\### Python Conventions



\*\*Every script is a command-line tool:\*\*

```python

if \_\_name\_\_ == "\_\_main\_\_":

&#x20;   import argparse

&#x20;   parser = argparse.ArgumentParser()

&#x20;   parser.add\_argument("--invoice-id", type=int, required=True)

&#x20;   parser.add\_argument("--db-path",    type=str, required=True)

&#x20;   parser.add\_argument("--action",     type=str, default="pdf")

&#x20;   args = parser.parse\_args()

&#x20;   result = main(args)

&#x20;   print(json.dumps(result))

&#x20;   sys.exit(0 if result\["success"] else 1)

```



\*\*Every script ends with JSON to stdout.\*\* Never print partial output or debug text to stdout during normal operation. Use `sys.stderr` for debug messages.



\*\*Imports are at the top of each file.\*\* No conditional imports inside functions.



\### SQL Conventions



\- All table names: `snake\_case` plural — `invoice\_items`, `stock\_movements`

\- All column names: `snake\_case`

\- All primary keys: `id INTEGER PRIMARY KEY AUTOINCREMENT`

\- All timestamps: stored as `DATETIME` in SQLite ISO format `2024-03-15 09:30:00`

\- All money amounts: `REAL` (SQLite float — sufficient for currency at 2 decimal places)

\- All boolean flags: `INTEGER` with `CHECK (col IN (0, 1))`



\---



\## 19. Testing Strategy



\### Database Tests (Manual)



Run these SQL statements directly in DB Browser after each schema change:



```sql

\-- Test: Invoice item insertion triggers stock deduction

INSERT INTO invoices (customer\_id, user\_id, invoice\_number, status)

VALUES (1, 1, 'TEST-001', 'confirmed');



\-- Check stock before:

SELECT stock\_qty FROM products WHERE id = 1;



INSERT INTO invoice\_items (invoice\_id, product\_id, quantity, unit\_price, line\_total, product\_name, product\_sku)

VALUES (last\_insert\_rowid(), 1, 3, 4500, 13500, 'Test Chair', 'CHR-001');



\-- Check stock after (should be 3 less):

SELECT stock\_qty FROM products WHERE id = 1;



\-- Check stock movement was recorded:

SELECT \* FROM stock\_movements WHERE product\_id = 1 ORDER BY moved\_at DESC LIMIT 1;

```



\### Python Service Tests



Each Python script has a `--test` flag that runs a self-contained validation without needing a real database:



```bash

python invoice\_printer.py --test

\# Output: {"success": true, "message": "Test PDF generated", "path": "/tmp/test\_invoice.pdf"}

```



\### C# Form Tests (Manual Walkthrough)



Before marking a form complete, walk through this checklist:

1\. Open the form with no data in the database — it should show empty state gracefully

2\. Fill all required fields and save — record appears in the list

3\. Clear one required field and try to save — validation error shown, no save occurs

4\. Open an existing record, change one field, save — changes persist on reopen

5\. Delete a record — it disappears from the list

6\. Close and reopen the form — state is correct (list repopulated, no leftover data)



\---



\## 20. Known Limitations \& Future Roadmap



\### Known Limitations (V1.0)



| Limitation | Reason | Workaround |

|---|---|---|

| Single machine only | No network database | Copy `furnitrack.db` manually between machines, or restore from GitHub backup |

| WhatsApp requires Desktop app installed | No API used | Install WhatsApp Desktop on the POS machine |

| No barcode scanner input | Out of scope V1 | SKU can be typed manually; barcode scanner that types text works with the product search box |

| No installment plan tracking | Simplified for V1 | Partial payments can be recorded; full installment schedule not implemented |

| Backup is full file, not incremental | Simple approach | File is typically small (< 50MB) even with years of data; full backup is fine |

| No multi-language support | Single store assumed | Arabic RTL support can be added to invoice PDFs in V2 |



\### Roadmap (V2.0+)



\- \*\*Arabic RTL invoice templates\*\* — ReportLab supports RTL text with arabic-reshaper library

\- \*\*Barcode/SKU scanner integration\*\* — WinForms KeyPreview to intercept scanner input in product search

\- \*\*Multiple store locations\*\* — Database schema already supports this via a `location\_id` column addition

\- \*\*Customer loyalty points\*\* — Add `points` column to customers, earn on purchase, redeem on invoice

\- \*\*Email invoice delivery\*\* — SMTP integration as alternative to WhatsApp

\- \*\*Installment schedule management\*\* — Full installment plan with due date tracking and overdue alerts

\- \*\*Export to Excel\*\* — Sales data export via ClosedXML for accounting integration

\- \*\*Network mode\*\* — Move database to a shared network path for multi-machine use (SQLite WAL handles concurrent reads)



\---



\*End of FurniTrack Development Documentation v1.0\*  

\*This document is the single source of truth for all development decisions. Any deviation from this plan must be documented in CHANGELOG.md with the reason for the change.\*


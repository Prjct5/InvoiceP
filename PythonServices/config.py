import sqlite3
import json
import sys

def get_db_connection(db_path):
    if not db_path:
        raise ValueError("Database path must be provided")
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    return conn

def output_success(data):
    result = {"success": True}
    result.update(data)
    print(json.dumps(result))
    sys.exit(0)

def output_error(message):
    print(json.dumps({"success": False, "error": message}))
    sys.exit(1)

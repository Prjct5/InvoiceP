using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FurniTrack.Models;

namespace FurniTrack.Data.Repositories
{
    public class CustomerRepo : IRepository<Customer>
    {
        private readonly DatabaseManager _db;
        
        public CustomerRepo(DatabaseManager db)
        {
            _db = db;
        }

        public Customer? GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand("SELECT * FROM customers WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Map(reader);
            }
            return null;
        }

        public List<Customer> GetAll()
        {
            var list = new List<Customer>();
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand("SELECT * FROM customers ORDER BY full_name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public int Insert(Customer entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand(@"INSERT INTO customers (full_name, phone, whatsapp, address, notes) 
                                                VALUES (@fn, @ph, @wa, @addr, @notes);
                                                SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@fn", entity.FullName);
            cmd.Parameters.AddWithValue("@ph", (object?)entity.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@wa", (object?)entity.Whatsapp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addr", (object?)entity.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", (object?)entity.Notes ?? DBNull.Value);
            
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Customer entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand(@"UPDATE customers SET 
                                                full_name = @fn, phone = @ph, whatsapp = @wa, 
                                                address = @addr, notes = @notes, updated_at = datetime('now') 
                                                WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@fn", entity.FullName);
            cmd.Parameters.AddWithValue("@ph", (object?)entity.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@wa", (object?)entity.Whatsapp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addr", (object?)entity.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", (object?)entity.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", entity.Id);
            
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand("DELETE FROM customers WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        
        private Customer Map(SQLiteDataReader reader)
        {
            return new Customer
            {
                Id = Convert.ToInt32(reader["id"]),
                FullName = reader["full_name"].ToString() ?? "",
                Phone = reader["phone"] == DBNull.Value ? null : reader["phone"].ToString(),
                Whatsapp = reader["whatsapp"] == DBNull.Value ? null : reader["whatsapp"].ToString(),
                Address = reader["address"] == DBNull.Value ? null : reader["address"].ToString(),
                Notes = reader["notes"] == DBNull.Value ? null : reader["notes"].ToString(),
                TotalSpent = Convert.ToDouble(reader["total_spent"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };
        }
    }
}

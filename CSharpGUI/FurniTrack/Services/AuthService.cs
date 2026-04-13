using System;
using System.Data.SQLite;
using FurniTrack.Models;

namespace FurniTrack.Services
{
    public class AuthService
    {
        private readonly Data.DatabaseManager _db;
        public static User? CurrentUser { get; private set; }

        public AuthService(Data.DatabaseManager db)
        {
            _db = db;
        }

        public bool Login(string username, string password)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand("SELECT * FROM users WHERE username = @u AND is_active = 1", conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string hash = reader["password_hash"].ToString() ?? "";
                if (BCrypt.Net.BCrypt.Verify(password, hash))
                {
                    CurrentUser = new User
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Username = username,
                        FullName = reader["full_name"].ToString() ?? "",
                        Role = reader["role"].ToString() ?? "cashier",
                        IsActive = 1
                    };
                    
                    using var updateCmd = new SQLiteCommand("UPDATE users SET last_login = datetime('now') WHERE id = @id", conn);
                    updateCmd.Parameters.AddWithValue("@id", CurrentUser.Id);
                    updateCmd.ExecuteNonQuery();

                    return true;
                }
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}

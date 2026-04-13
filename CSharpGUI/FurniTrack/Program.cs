using System;
using System.IO;
using System.Windows.Forms;
using FurniTrack.Data;
using FurniTrack.Services;
using FurniTrack.Forms;

namespace FurniTrack
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Mock path for dev
            string dbPath = @"..\..\..\..\Database\furnitrack.db";
            PythonBridge.ServicesDir = @"..\..\..\..\PythonServices";
            
            try 
            {
                var db = new DatabaseManager(dbPath);
                var settingsService = new SettingsService(db);
                var authService = new AuthService(db);

                if (settingsService.IsFirstRun())
                {
                    var splash = new SplashForm(settingsService);
                    if (splash.ShowDialog() != DialogResult.OK)
                    {
                        return; // Exit if setup is cancelled
                    }
                }

                var login = new LoginForm(authService);
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new MainForm());
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show("Error on startup: " + ex.Message);
            }
        }
    }
}
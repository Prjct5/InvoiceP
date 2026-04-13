using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FurniTrack.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Error { get; set; } = "";
    }

    public static class PythonBridge
    {
        public static string ServicesDir { get; set; } = "";

        public static ServiceResult<T> Call<T>(string serviceExe, string[] args)
        {
            try
            {
                var exePath = Path.Combine(ServicesDir, serviceExe);
                if (!File.Exists(exePath))
                {
                    // Fallback to python script directly if the .exe is not built yet
                    exePath = "python";
                    args = new[] { Path.Combine(ServicesDir, serviceExe.Replace(".exe", ".py")) }.Concat(args).ToArray();
                }

                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = string.Join(" ", args.Select(a => $"\"{a}\"")),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null) return new ServiceResult<T> { Error = "Failed to start process" };
                
                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();
                
                var lastLine = output.Trim().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (string.IsNullOrEmpty(lastLine))
                    return new ServiceResult<T> { Error = $"No json output from service. Exit code: {proc.ExitCode}\nStderr: {error}" };
                
                var doc = JsonDocument.Parse(lastLine);
                var success = doc.RootElement.TryGetProperty("success", out var succEl) && succEl.GetBoolean();
                if (!success)
                {
                    var errMsg = doc.RootElement.TryGetProperty("error", out var errEl) ? errEl.GetString() : "Unknown error";
                    return new ServiceResult<T> { Error = errMsg ?? "Unknown error" };
                }
                
                return new ServiceResult<T>
                {
                    Success = true,
                    Data = JsonSerializer.Deserialize<T>(lastLine, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<T> { Error = ex.Message };
            }
        }
    }
}

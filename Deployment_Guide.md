# FurniTrack Deployment & Packaging Guide

This guide covers how to locally test the FurniTrack application, run it during development, and compile the entire project (Python Microservices + SQLite Database + C# WinForms) into a finalized, ready-to-deploy `.exe` package for your users.

---

## 1. Testing and Running Locally

Before packaging the application, you'll want to verify that the core components are speaking to each other properly.

### Running the Python Services
FurniTrack relies on python execution for PDF generation and API endpoints. 
1. Open PowerShell and navigate to the project directory:
   ```powershell
   cd C:\Users\mohaf\Downloads\project\vs\PythonServices
   ```
2. Build the `.exe` wrappers via the provided batch script:
   ```powershell
   .\build_services.bat
   ```
   *This commands PyInstaller to construct standalone python executables in the `dist` folder.*

### Compiling & Running the C# GUI
The C# UI is built on `.NET 8`.
1. Navigate to the C# Application root:
   ```powershell
   cd C:\Users\mohaf\Downloads\project\vs\CSharpGUI\FurniTrack
   ```
2. Run the native command to build and launch:
   ```powershell
   dotnet run
   ```
3. **First-Run Check**: If everything is configured correctly, FurniTrack will open the `SplashForm` wizard to configure the Store Settings. Subsequent runs will display the `LoginForm`.

---

## 2. Packaging into a Single Distribution

Because this project utilizes a combination of `.NET 8`, a local SQLite `.db`, and compiled `Python .exe` scripts, we must bundle these assets together so that the application is fully portable for an end-user.

### Step A: Publish the C# Application
We will utilize the powerful `.NET Publish` compiler to squish the C# framework and its DLL dependencies into one native exe.

1. Navigate to the C# Application:
   ```powershell
   cd C:\Users\mohaf\Downloads\project\vs\CSharpGUI\FurniTrack
   ```
2. Run the command to publish a self-contained, single-file executable for Windows x64:
   ```powershell
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o out_publish
   ```
> [!TIP]
> The `-p:PublishSingleFile=true` flag forces `.NET` to merge the SQLite adapters, BCrypt DLLs, and GUI code into exactly one file (`FurniTrack.exe`).

### Step B: Final Production Folder Structure
Create a new folder on your desktop called `FurniTrackRelease`. Gather your compiled components as follows:

```text
FurniTrackRelease/
│
├── FurniTrack.exe                 <-- The compiled output from 'out_publish'
├── app_config.json                <-- Your config specifying relative paths
│
├── Database/
│   ├── furnitrack.db              <-- Your seeded SQLite database file
│
└── PythonServices/
    └── dist/
        ├── invoice_printer.exe    <-- Built by your build_services.bat
        ├── whatsapp_sender.exe
        ├── backup_service.exe
        └── report_generator.exe
```

> [!CAUTION]
> **Important Config Note:** Ensure your user-facing `app_config.json` inside this release folder points to relative directories so it survives deployment across different computers! For example:
> `{"db_path": ".\\Database\\furnitrack.db", "services_dir": ".\\PythonServices\\dist"}`

---

## 3. Creating the Installer (Optional, Recommended)

You now have a portable `FurniTrackRelease` folder. You can `.zip` this entire directory and send it to your users, and it will run perfectly. 

However, to provide a **Professional Installer** (a standard `setup.exe` that puts a shortcut on their Desktop exactly like standard software):

We recommend using **Inno Setup** (Free, Open Source).
1. Download **Inno Setup**.
2. Open it and click **Create a new script file using the Script Wizard**.
3. Point "Application Main Executable file" to: `FurniTrackRelease\FurniTrack.exe`.
4. Click **Add Folder** and point to your entire `FurniTrackRelease` folder to ensure all the Database and Python nested folders are packaged inside.
5. Finish the wizard and press **Compile**.

Inno Setup will squash your entire project into a single `FurniTrack_Installer.exe` containing compressed UI, python binaries, and your database!

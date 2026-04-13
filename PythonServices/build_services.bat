@echo off
echo Installing requirements...
pip install -r requirements.txt

echo Building invoice_printer...
pyinstaller --onefile --windowed --name invoice_printer invoice_printer.py

echo Building whatsapp_sender...
pyinstaller --onefile --windowed --name whatsapp_sender whatsapp_sender.py

echo Building backup_service...
pyinstaller --onefile --windowed --name backup_service backup_service.py

echo Building report_generator...
pyinstaller --onefile --windowed --name report_generator report_generator.py

echo Build complete. Executables are in the "dist" folder.

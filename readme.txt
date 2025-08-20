--------------------------Worker Service Installation--------------------------
& sc.exe create MyEAService binPath="C:\Program Files\EAWS\EAWorkerService.exe" 

Start-Service -Name "MyEAService"

Set-Service -Name "MyEAService" -StartupType Automatic


--------------------------CLI Self-Contained Publish Command--------------------------
dotnet publish -c Release -r win-x64 --self-contained true --output ./publish
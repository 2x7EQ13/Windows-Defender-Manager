# Windows Defender Manager

![icons8-broken-shield-60](https://github.com/user-attachments/assets/124b4218-6acd-45ea-ad0a-983d2c7c3670)

Windows Defender Manager is an open-source C# program. By interacting with the Antimalware Service Executable through APIs, it will block windows defender by disable the following functions of Microsoft Defender Antivirus:

- Real-time protection
- Cloud-delevired protection
- Automatic sample submission

## How Windows Defender Manager works:

- When executed, it will copy itself to the path **"Program Files\WinDefendMan\WinDefendMan.exe"** and then register a scheduled task named **"Windows Defender Manager Task"**
- At this point, Windows Defender Manager will run in the background and check the **"Real-time protection"** feature every minute, automatically deactivate windows defender if it is somehow turned on.

## Supported Windows versions:

- Windows 11 Lasted version
- Windows 10 Lasted version

# Important note:

### You must disable the "Tamper Protection" feature of Microsoft Defender Antivirus to run successfully.


![turn off tamper protection](https://github.com/user-attachments/assets/93e03f79-f77e-4e8d-9de5-1a547cfb23ed)

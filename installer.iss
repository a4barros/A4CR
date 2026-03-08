#define MyAppVersion "0.0.0"

[Setup]
AppName=A4crypt
AppVersion={#MyAppVersion}
DefaultDirName={commonpf}\A4crypt
DefaultGroupName=A4crypt
OutputDir=dist
OutputBaseFilename=A4crypt-{#MyAppVersion}-setup
Compression=lzma
SolidCompression=yes

[Files]
Source: "publish\windows\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\A4crypt"; Filename: "{app}\GUI.exe"
Name: "{commondesktop}\A4crypt"; Filename: "{app}\GUI.exe"

[Run]
Filename: "{app}\GUI.exe"; Description: "Launch A4crypt"; Flags: nowait postinstall skipifsilent
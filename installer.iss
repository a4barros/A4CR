#ifndef MyAppVersion
  #define MyAppVersion "0.0.0"
#endif

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

[Registry]
Root: HKCR; Subkey: ".a4cr"; ValueType: string; ValueName: ""; ValueData: "A4cryptFile"; Flags: uninsdeletekey
Root: HKCR; Subkey: "A4cryptFile"; ValueType: string; ValueName: ""; ValueData: "A4crypt encrypted file"; Flags: uninsdeletekey
Root: HKCR; Subkey: "A4cryptFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\GUI.exe,0"; Flags: uninsdeletekey
Root: HKCR; Subkey: "A4cryptFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\GUI.exe"" ""%1"""; Flags: uninsdeletekey

[Run]
Filename: "{app}\GUI.exe"; Description: "Launch A4crypt"; Flags: nowait postinstall skipifsilent
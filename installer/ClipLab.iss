; Inno Setup script for Clip Lab.
; Build with: iscc installer\ClipLab.iss
; Expects a framework-dependent single-file publish already sitting in .\publish
; (see .github/workflows/release.yml or README "Building the installer").

#define MyAppName "Clip Lab"
#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#define MyAppPublisher "yiksnele"
#define MyAppURL "https://github.com/flexeykinDev/ClipLab-Pro"
#define MyAppExeName "ClipLab.exe"

[Setup]
AppId={{B6F2B6C8-6C0D-4B6E-9B7A-6E6F6E6F6E6F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
OutputDir=..\installer-output
OutputBaseFilename=ClipLab-Setup-{#MyAppVersion}
SetupIconFile=..\Icons\IconApp.ico
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\publish\ClipLab.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publish\ffmpeg.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
function IsDotNet6DesktopRuntimeInstalled: Boolean;
var
  ResultCode: Integer;
  TempFile: String;
  Output: AnsiString;
  Lines: TStringList;
  I: Integer;
begin
  Result := False;
  TempFile := ExpandConstant('{tmp}\dotnet-runtimes.txt');
  if Exec(ExpandConstant('{cmd}'), '/C dotnet --list-runtimes > "' + TempFile + '" 2>nul',
     '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if LoadStringFromFile(TempFile, Output) then
    begin
      Lines := TStringList.Create;
      try
        Lines.Text := Output;
        for I := 0 to Lines.Count - 1 do
          if Pos('Microsoft.WindowsDesktop.App 6.', Lines[I]) = 1 then
          begin
            Result := True;
            Break;
          end;
      finally
        Lines.Free;
      end;
    end;
  end;
end;

function InitializeSetup: Boolean;
var
  ResultCode: Integer;
begin
  Result := True;
  if not IsDotNet6DesktopRuntimeInstalled then
  begin
    if MsgBox('Clip Lab requires the .NET 6 Desktop Runtime, which was not detected on this system.' + #13#10#13#10 +
       'Continue installing Clip Lab anyway? You will need to install the runtime separately before the app will run.' + #13#10#13#10 +
       'Click "No" to open the download page instead.',
       mbConfirmation, MB_YESNO) = IDNO then
    begin
      ShellExec('open', 'https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime?cid=getdotnetcore&os=windows&arch=x64', '', '', SW_SHOWNORMAL, ewNoWait, ResultCode);
      Result := False;
    end;
  end;
end;

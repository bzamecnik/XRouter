; XRouter installer - template

; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "XRouter"
#define MyAppVersion "1.0"
#define MyAppPublisher "The XRouter Team"
#define MyAppURL "http://www.assembla.com/spaces/xrouter"

#define XRouterServiceName "xrouter"
#define XRouterManagerServiceName "xroutermanager"

[Setup]
; NOTE: AppId must be defined in the file which uses this template
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=LICENSE
InfoBeforeFile=README
OutputDir=installer
OutputBaseFilename=xrouter-setup{#OutputBaseFilenamePostfix}
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\{#BuildType}\DaemonNT.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\DaemonNT.GUI.ConfigEditor.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\DaemonNT.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\ObjectConfigurator.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\RibbonControlsLibrary.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\SchemaTron.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\SimpleDiagrammer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Adapters.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Broker.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.ComponentHosting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Data.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Gateway.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Gui.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Gui.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Manager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\XRouter.Processor.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

Name: "{group}\XRouter Configuration Manager"; Filename: "{app}\XRouter.Gui.exe"; WorkingDir: "{app}"
Name: "{group}\DaemonNT Configuration Editor"; Filename: "{app}\DaemonNT.GUI.ConfigEditor.exe"; WorkingDir: "{app}"

Name: "{group}\XRouter\Install XRouter as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "install {#XRouterServiceName}"
Name: "{group}\XRouter\Uninstall XRouter as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "uninstall {#XRouterServiceName}"
Name: "{group}\XRouter\Start XRouter as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "start {#XRouterServiceName}"
Name: "{group}\XRouter\Stop XRouter as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "stop {#XRouterServiceName}"

Name: "{group}\XRouter Manager\Install XRouter Manager as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "install {#XRouterManagerServiceName}"
Name: "{group}\XRouter Manager\Uninstall XRouter Manager as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "uninstall {#XRouterManagerServiceName}"
Name: "{group}\XRouter Manager\Start XRouter Manager as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "start {#XRouterManagerServiceName}"
Name: "{group}\XRouter Manager\Stop XRouter Manager as Windows Service"; Filename: "{app}\DaemonNT.exe"; WorkingDir: "{app}"; Parameters: "stop {#XRouterManagerServiceName}"

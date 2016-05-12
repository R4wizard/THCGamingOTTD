;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
;General

  ;Name and file
  Name "THCGaming - OpenTTD"
  OutFile "install.exe"

  ;Default installation folder
  InstallDir "$LOCALAPPDATA\THCGamingOTTD"

  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\THCGamingOTTD" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel user

;--------------------------------
;Variables

  Var StartMenuFolder

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  ;Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU"
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\THCGamingOTTD"
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "THCGaming - OpenTTD"

  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !insertmacro MUI_PAGE_INSTFILES

  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "OpenTTD Updater" SecOpenTTD

  SetOutPath "$INSTDIR"

  File /r "bin\Release\*"

  ;Store installation folder
  WriteRegStr HKCU "Software\THCGamingOTTD" "" $INSTDIR

  ;Create uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"

  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application

    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\Uninstall THCGaming - OpenTTD.lnk" "$INSTDIR\uninstall.exe"
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\THCGaming - OpenTTD.lnk" "$INSTDIR\THCGamingOTTD.exe"

  !insertmacro MUI_STARTMENU_WRITE_END

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecOpenTTD ${LANG_ENGLISH} "Tool for downloading OpenTTD and maintaining latest version."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecOpenTTD} $(DESC_SecOpenTTD)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  Delete "$INSTDIR\uninstall.exe"

  RMDir /r "$INSTDIR"

  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder

    Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall THCGaming - OpenTTD.lnk"
	;Delete "$SMPROGRAMS\$StartMenuFolder\THCGaming - OpenTTD.lnk"
  RMDir "$SMPROGRAMS\$StartMenuFolder"

  DeleteRegKey /ifempty HKCU "Software\THCGamingOTTD"

SectionEnd

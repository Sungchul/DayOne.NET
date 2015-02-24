!include "MUI2.nsh"

;--------------------------------
;General

	;Name and file
	Name "DayOne.NET"
	OutFile "setup.exe"

	;Default installation folder
	InstallDir "$LOCALAPPDATA\DayOne.NET"

	;Get installation folder from registry if available
	InstallDirRegKey HKCU "Software\DayOne.NET" ""

	;Request application privileges for Windows Vista
	;RequestExecutionLevel admin

;--------------------------------
;Variables
	Var StartMenuFolder

;--------------------------------
;Interface Settings
	!define MUI_ABORTWARNING
	; user default icon.
	;!define MUI_ICON ICON.ico

;--------------------------------
;Pages

	!insertmacro MUI_PAGE_WELCOME
	; GNU License???
	;!insertmacro MUI_PAGE_LICENSE "${NSISDIR}\Docs\Modern UI\License.txt"
	;!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
 
	;Start Menu Folder Page Configuration
	!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
	!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\DayOne.NET" 
	!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
 
	!insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

	!insertmacro MUI_PAGE_INSTFILES
	!insertmacro MUI_PAGE_FINISH
 
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

 !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "DayOne.NET Excution File" DayOneNET_Install
	SetOutPath "$INSTDIR"
	File "DayOne.NET.exe"
	File /r "*.dll"
 
	;Store installation folder
	WriteRegStr HKCU "Software\DayOne.NET" "" $INSTDIR
 
	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"

	!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
	;create start-menu items
		CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\DayOne.NET.lnk" "$INSTDIR\DayOne.NET.exe" "" "$INSTDIR\DayOne.NET" 0
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
		CreateShortCut "$DESKTOP\DayOne.NET.lnk" "$INSTDIR\DayOne.NET.exe" "" "$INSTDIR\DayOne.NET" 0
	!insertmacro MUI_STARTMENU_WRITE_END
SectionEnd
;--------------------------------
;Descriptions
;Language strings
LangString DESC_ExeInstall ${LANG_ENGLISH} "A DayOne.NET essential excution packages."

;Assign language strings to sections
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${DayOneNET_Install} $(DESC_ExeInstall)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section
Section "Uninstall"

	;Delete Files 
	RMDir /r "$INSTDIR\*.*"  
	Delete "$INSTDIR\Uninstall.exe"

	;Remove the installation directory
	RMDir "$INSTDIR" 
	
	Delete "$DESKTOP\DayOne.NET.lnk"
	!insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
 
	Delete "$SMPROGRAMS\$StartMenuFolder\DayOne.NET.lnk"
	Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
 
	RMDir "$SMPROGRAMS\$StartMenuFolder"

	DeleteRegKey /ifempty HKCU "Software\DayOne.NET"
SectionEnd
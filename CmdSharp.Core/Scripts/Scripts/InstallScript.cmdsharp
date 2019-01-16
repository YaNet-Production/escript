// 
// ESCRIPT Installation Script
// (C) Dz3n
//

func Main
{
	Restart 1

	if $edition||==||core||{#CoreWarning}
	//if $edition||==||core||{#CoreWarning}||{#async CheckNewVersions}
	

	$vMajor=Version major
	$vMinor=Version minor
	$version=#$vMajor.$vMinor
	
	if {ListContains args||-fast}||==||1||{#InstallEscriptYes}

	if {msg ESCRIPT Installation||Welcome to ESCRIPT $version Standard Installation!~n~~n~Do you want to install ESCRIPT?~n~(admin rights will be requested)~n~~n~* The running instances of ESCRIPT will be CLOSED.||5||2}||==||Yes||{#InstallEscriptYes}||{#Canceled}
}

func CheckNewVersions
{
	$verUrl=#$stuffServerUpdateFiles/latest-version.txt
	$vf=DownloadText $verUrl

	StringSplit $vf||.||nVerList
	StringSplit {ver}||.||cVerList

	DebugText $nVerList
	DebugText $cVerList

	ListEnum nVerList||CheckVer

	if $warnNew||==||1||NewVerMsg
}

func NewVerMsg
{
	msg ESCRIPT Installation||You are attempting to install ESCRIPT {ver}, but newer version ($vf) is available. Do you want to install it instead?||5||2}||==||Yes||{#InstallUpdate}
}

func InstallUpdate
{
	update
	if $programDebug||==||1||Break||Exit 0
}

func CheckVer 
{
	DebugText idx=$enumIdx
	$current=ListGetValue cVerList||$enumIdx
	DebugText n=$enumValue
	DebugText c=$current
	if $enumValue||>||$c||{#set warnNew||1}
}

func ShowError ErrorReason
{
	HideStatus
	msg ESCRIPT Installation Failed||$ErrorReason||error||retrycancel
	if $result||==||Cancel||GoExit||InstallEscriptYes
}

func GoExit
{
	if $programDebug||==||1||Break||Exit 1
}

func CheckInstance idx
{
	$processId=ListGetValue running||$idx

	$r5nningProcessPath=ProcessGetPath $processId
	$r5nningProcessPath=ToLower $r5nningProcessPath

	$currentProcessPath=GetProgramPath
	$currentProcessPath=ToLower $currentProcessPath

	if $r5nningProcessPath||!=||$currentProcessPath||pKill $processId
}

func pKill pn
{
	ProcessKill $pn
	DebugText Process #$pn ($r5nningProcessPath != $currentProcessPath) killed (press any key)
	//ReadLine
}

func InstallEscriptYes
{
	// we can remove sleep but it's for text
	$ppath=GetProgramPath
	PathCombine {GetPath ProgramFiles}||ESCRIPT
	$esfolder=#$result

	SetStatus Installing ESCRIPT - Gathering Information||Installing in: $esfolder~n~~n~P.S.: This window is a part of a script. You can create your own script with own window! That's so easy! See documentation and InstallScript.es source code (generated in current folder) for details!
	if {ListContains args||-fast}||!=||1||sleep 1000

	MakeDir $esfolder
	if $result||==||0||{#ShowError Can't create directory: $esfolder}
	//start explorer.exe||$esfolder||0

	$dest=PathCombine $esfolder||escript.exe

	if $dest||==||$ppath||{#ShowError ESCRIPT is already installed}
	
	ProcessFindByName escript||running

	for 0||{ListCount running}||CheckInstance $for

	SetStatusProgress 75
	if {ListContains args||-fast}||!=||1||sleep 2000
	SetStatus Installing ESCRIPT - Copying Files||Installing in: $esfolder~n~~n~P.S.: This window is a part of a script. You can create your own script with own window! That's so easy! See documentation and InstallScript.es source code (generated in current folder) for details!||75

	copy $ppath||$dest||1
	if $result||!=||$cmdDone$||{#ShowError Can't copy $ppath to $dest.}

	SetStatusProgress 100
	if {ListContains args||-fast}||!=||1||sleep 2350

	SetStatus Installing ESCRIPT - Finishing||Installed in: $esfolder~n~~n~Registering file types and creating scripts on a desktop...||0
	
	if {ListContains args||-fast}||!=||1||sleep 1000
	$exitCode=start $dest||-assoc||1||1

	if $exitCode||!=||0||{#msg ESCRIPT Installation Error||Can't complete post-install stage||error||ok}
	SetStatus Installing ESCRIPT - Done||Installed in: $esfolder~n~~n~Starting ESCRIPT...||100

	if {ListContains args||-fast}||!=||1||sleep 250
	if {ListContains args||-fast}||!=||1||{#start $dest||null||0}

	if $programDebug||==||1||Break||Exit 0
}

func Canceled
{
	msg ESCRIPT Installation||The installation was canceled by user.||4||0
	if $programDebug||==||1||Break||Exit 0
}

func CoreWarning
{
	msg ESCRIPT Installation||Core can't be installed. But you can try, lol.||warning||ok
}
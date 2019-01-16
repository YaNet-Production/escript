// 
// ESCRIPT Update Script
// (C) Dz3n
//

func Main
{

	DebugText $args
	if {ListContains args||force}||==||1||SetForce

	$r=ListGetValue args||1

	
	SetStatus ESCRIPT Update||Fetching data about update information...~n~($r)

	$verUrl=#$stuffServerUpdateFiles/$r-version.txt
	$vf=DownloadText $verUrl
	if {StringLength $r}||<=||0||{#set r||latest}

	HideStatus

	if $vf||!=||$cmdDone$||ShowError Can't fetch data from the server

	if $vf||!=||null||{#$text=#ESCRIPT $vf ($r) is available. Do you want to download and install it?~n~~n~* You can update later by typing "update" command.~n~(escript-$r)}||{#$text=#Do you want to download and install the latest version of ESCRIPT?~n~(escript-$r)}
	$msgResult=msg ESCRIPT Update||$text||question||yesno
	if $msgResult||==||Yes||eUpdate
	if $programDebug||==||0||Exit 1||Break
}

func SetForce
{
	echo is force
	$r=ListGetValue args||1
	eUpdate
}

func eUpdate
{
	Restart 1||"force"

	$downPath=#$stuffServerUpdateFiles/escript-$r.exe
	$tmp=GetTempPath
	$exePath=PathCombine $tmp||escript-update.exe
	SetStatus Downloading ESCRIPT Update||Connecting to the main repository...

	DebugText downPath: $downPath
	DebugText exePath: $exePath

	$changelog=Well, instead of waiting, you can read the changelog.~n~Link: https://github.com/feel-the-dz3n/escript/blob/master/changelog.txt~n~If you have some problems, here is the direct link:~n~$downPath
	SetStatus Downloading ESCRIPT Update||$changelog
	DownloadFile $downPath||$exePath

	if $result||!=||$cmdDone$||ShowError Can't download ESCRIPT!

	SetStatusProgress 100
	SetStatus ESCRIPT Update Downloaded||$changelog||100
	sleep 5000
	
	StartProgram $exePath||-install
	
	if $programDebug||==||0||Exit 0||Break
}

func ShowError ErrorReason
{
	HideStatus
	msg ESCRIPT Update Failed||$ErrorReason||error||ok
	if $programDebug||==||1||Break||Exit 1
}

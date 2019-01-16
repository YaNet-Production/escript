
func Main
{
	HideConsole
	$programfiles=GetPath ProgramFiles
	$esfolder=PathCombine $programfiles||ESCRIPT
	
	if {msg ESCRIPT||Open $esfolder?||question||yesno}||==||No||Exit
	start explorer.exe||$esfolder||0
	Exit
}
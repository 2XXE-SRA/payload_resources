' source: https://modexp.wordpress.com/2019/08/30/minidumpwritedump-via-com-services-dll/

Option Explicit

Const SW_HIDE = 0

If (WScript.Arguments.Count <> 1) Then
    WScript.StdOut.WriteLine("procdump - Copyright (c) 2019 odzhan")
    WScript.StdOut.WriteLine("Usage: procdump <process>")
    WScript.Quit
Else
    Dim fso, svc, list, proc, startup, cfg, pid, str, cmd, query, dmp
    
    ' get process id or name
    pid = WScript.Arguments(0)
    
    ' connect with debug privilege
    Set fso  = CreateObject("Scripting.FileSystemObject")
    Set svc  = GetObject("WINMGMTS:{impersonationLevel=impersonate, (Debug)}")
    
    ' if not a number
    If(Not IsNumeric(pid)) Then
      query = "Name"
    Else
      query = "ProcessId"
    End If
    
    ' try find it
    Set list = svc.ExecQuery("SELECT * From Win32_Process Where " & _
      query & " = '" & pid & "'")
    
    If (list.Count = 0) Then
      WScript.StdOut.WriteLine("Can't find active process : " & pid)
      WScript.Quit()
    End If

    For Each proc in list
      pid = proc.ProcessId
      str = proc.Name
      Exit For
    Next

    dmp = fso.GetBaseName(str) & ".bin"
    
    ' if dump file already exists, try to remove it
    If(fso.FileExists(dmp)) Then
      WScript.StdOut.WriteLine("Removing " & dmp)
      fso.DeleteFile(dmp)
    End If
    
    WScript.StdOut.WriteLine("Attempting to dump memory from " & _
      str & ":" & pid & " to " & dmp)
    
    Set proc       = svc.Get("Win32_Process")
    Set startup    = svc.Get("Win32_ProcessStartup")
    Set cfg        = startup.SpawnInstance_
    cfg.ShowWindow = SW_HIDE

    cmd = "rundll32 C:\windows\system32\comsvcs.dll, MiniDump " & _
          pid & " " & fso.GetAbsolutePathName(".") & "\" & _
          dmp & " full"
    
    Call proc.Create (cmd, null, cfg, pid)
    
    ' sleep for a second
    Wscript.Sleep(1000)
    
    If(fso.FileExists(dmp)) Then
      WScript.StdOut.WriteLine("Memory saved to " & dmp)
    Else
      WScript.StdOut.WriteLine("Something went wrong.")
    End If
End If

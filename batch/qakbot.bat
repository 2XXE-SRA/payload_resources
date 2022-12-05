:: QakBot enumeration from: https://securelist.com/qakbot-technical-analysis/103931/

whoami /all
arp -a
ipconfig /all
net view /all
cmd /c set
:: nslookup -querytype=ALL -timeout=10 _ldap._tcp.dc._msdcs.{DOMAIN}
nltest /domain_trusts /all_trusts
net share
route print
netstat -nao
net localgroup
qwinsta
wmic path Win32_BIOS get
wmic path Win32_DiskDrive get
wmic path Win32_PhysicalMemory get
wmic path Win32_Product get
wmic path Win32_PnPEntity get

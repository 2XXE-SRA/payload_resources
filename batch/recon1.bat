:: miscellaneous recon commands taken from intel reports published between 10/2023 and 10/2024

arp -a
dir /a c:\users
ipconfig /all
fsutil fsinfo drives
hostname
net accounts
net config workstation
net group "Domain Computers" /domain
net group "Domain Admins" /domain
net localgroup Administrators
net share
net start
net use
net user
net view /all
net view /all /domain
netsh firewall show all
netstat -ano
nltest /dclist:
nltest /domain_trusts /all_trusts
nltest /dsgetdc:
query user
reg query hklm\software
systeminfo
tasklist
ver
whoami /all
wmic nic get
wmic baseboard list
wmic os get
wmic path win32_logicaldisk get
wmic process list
wmic product list
wmic service list
wmic volume list

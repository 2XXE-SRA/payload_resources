#!/bin/bash
df -aH
ip addr
netstat -tulpn
ps -aux 
who -a 
systemctl list-units --type=service --no-pager
service --status-all
apt list --installed

# PAM

Build via: `gcc -fPIC -fno-stack-protector src.c` (requires libpam)

Load via (as root):

1. `ld -x --shared -o /lib/security/<name>.so <.o file>`
2. Edit `/etc/pam.d/common-auth` (or another conf file) to include: `auth sufficient <name>.so`




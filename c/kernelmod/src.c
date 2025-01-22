// based on: https://sysprog21.github.io/lkmpg/

#include <linux/module.h>
#include <linux/printk.h>

MODULE_LICENSE("GPL");

int init_module(void){ 
    pr_info("loaded\n"); 
    return 0; 
} 

void cleanup_module(void){ 
    pr_info("unloaded\n"); 
} 


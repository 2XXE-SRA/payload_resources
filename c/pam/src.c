// based on: https://github.com/beatgammit/simple-pam/blob/master/src/mypam.c

#include <stdio.h>
#include <security/pam_modules.h>

PAM_EXTERN int pam_sm_setcred(pam_handle_t *pamh, int flags, int argc, const char **argv){
    printf("pam_sm_setcred\n");
    return PAM_SUCCESS;
}

PAM_EXTERN int pam_sm_acct_mgmt(pam_handle_t *pamh, int flags, int argc, const char **argv){
	printf("pam_sm_acct_mgmt\n");
	return PAM_SUCCESS;
}

PAM_EXTERN int pam_sm_authenticate(pam_handle_t *pamh, int flags,int argc, const char **argv){
    printf("pam_sm_authenticate\n");
    return PAM_SUCCESS;
}


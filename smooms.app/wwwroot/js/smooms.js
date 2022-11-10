function getClientTimeZone(){
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}
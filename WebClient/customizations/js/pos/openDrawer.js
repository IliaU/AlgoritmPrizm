async function openDrawer () {
    let response = await fetch(
        UTIL_HOSTNAME + "/OpenDrawer",
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json, version=2',
                'Accept': 'application/json, version=2',
                'Accept-Encoding': 'gzip, deflate, br'
            }
        });
}
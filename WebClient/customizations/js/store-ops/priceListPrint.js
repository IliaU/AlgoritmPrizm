async function printPriceList () {
    let response = await fetch(
        UTIL_HOSTNAME + "/AksRepItem",
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json, version=2',
                'Accept': 'application/json, version=2',
                'Accept-Encoding': 'gzip, deflate, br',
                'Auth-Session': session.token
            },
            body: JSON.stringify([{
                valueString: 'PL',
            }])
        });
    let result = await response.json();
    alert(result["Message"]);
    window.open(UTIL_HOSTNAME + "/AksRepStat", '_blank').focus();
}
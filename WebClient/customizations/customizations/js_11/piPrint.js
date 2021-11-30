async function printInv (design) {
    let href = window.location.href.split("/");
    let SID = href[href.findIndex((element) => (element === "pisheet")) + 1];

    console.log(design, SID);

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
                design: design,
                sid: SID
            }])
        });
    let result = await response.json();
    console.log(result);
}
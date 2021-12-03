async function openVerification (verificationType) {
    const verificationTypeParams = {
        voucher: {
            documentName: "receiving",
            documentItemName: "recvitem",
            documentSIDName: "vousid",
        },
        po: {
            documentName: "transferslip",
            documentItemName: "slipitem",
            documentSIDName: "slipsid",
        },
        so: {
            documentName: "adjustment",
            documentItemName: "adjitem",
            documentSIDName: "adjsid",
        }
    };

    let href = window.location.href.split("/");
    let SID = href[href.findIndex((element) => (element === "new" || element === "view")) + 1];
    session = JSON.parse(sessionStorage.getItem("session"));

    console.log(SID);

    let url =
        HOSTNAME +
        "/api/backoffice/" +
        verificationTypeParams[verificationType].documentName +
        "/" +
        SID +
        "/" +
        verificationTypeParams[verificationType].documentItemName +
        "?";

    let response = await fetch(url, {
        method: "GET",
        headers: {
            'Content-Type': 'application/json, version=2',
            'Accept': 'application/json, version=2',
            'Accept-Encoding': 'gzip, deflate, br',
            'Auth-Session': session.token
        }
    });

    response = await response.json();

    if (response.data.length) {
        // window.location.href = "/verification?type=" + verificationType + "&sid=" + SID;
        window.open("/verification?type=" + verificationType + "&sid=" + SID, '_blank').focus();
    } else {
        alert("В документе отсутствуют позиции для верификации");
    }

}

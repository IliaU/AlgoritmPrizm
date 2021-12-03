const verificationTypeParams = {
    voucher: {
        documentName: "receiving",
        documentItemName: "recvitem",
        documentSIDName: "vousid",
    },
    slip: {
        documentName: "transferslip",
        documentItemName: "slipitem",
        documentSIDName: "slipsid",
    },
    adjustment: {
        documentName: "adjustment",
        documentItemName: "adjitem",
        documentSIDName: "adjsid",
    }
};

const HOSTNAME  = "http://172.16.1.102"


async function openVerification (verificationType) {
    let href = window.location.href.split("/");
    let SID = href[href.findIndex((element) => element === "new") + 1];

    console.log(SID);

    let url =
        HOSTNAME +
        "/api/backoffice/" +
        verificationTypeParams[verificationType].documentName +
        "/" +
        SID +
        verificationTypeParams[verificationType].documentItemName +
        "?";

    let response = await fetch(url, {
        method: "GET",
        headers: {
            'Content-Type': 'application/json, version=2',
            'Accept': 'application/json, version=2',
            'Accept-Encoding': 'gzip, deflate, br',
            'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
            'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
            'Auth-Session': localStorage.getItem('Auth-Session')
        }
    });

    response = await response.json();

    if (response.data.length) {
        window.location.href = "/verification?type=" + verificationType;
    } else {
        alert("В документе отсутствуют позиции для верификации");
    }

}

const HOSTNAME = "http://172.16.1.102";
const verificationTypeParams = {
    voucher: {
        documentName: "receiving",
        documentItemName: "recvitem",
        itemQty: "qty",
    },
    po: {
        documentName: "purchaseorder",
        documentItemName: "poitem",
        itemQty: "ordqty",
    },
    so: {
        documentName: "adjustment",
        documentItemName: "adjitem",
        itemQty: "adjsid",
    }
};
const session = JSON.parse(sessionStorage.getItem("session"));
const verificationField = "note";

const urlParams = new URLSearchParams(window.location.search);
const verificationType = urlParams.get("type");
const SID = urlParams.get("sid");

var documentItems = [];

async function loadDocumentContent () {
    if (!(verificationType && SID)) {
        alert("Недостаточно данных для верификации");
        window.close();
    }

    const items = await getItemsFromAPI(SID, verificationType);
    initItemsTable();

    for (let i = 0; i < items.length; i++) {
        items[i].quantity = items[i][verificationTypeParams[verificationType].itemQty];
        addItem(items[i]);
    }

    documentItems = items;
}

async function loadFile(input) {
    let itemTable = $("#table");
    let errorTable = $("#error-table");
    let errorBlock = $("#errors");

    itemTable.bootstrapTable("removeAll");
    errorTable.bootstrapTable("removeAll");
    errorBlock[0].style.display = "none";

    await loadDocumentContent();

    let file = input.files[0];
    let reader = new FileReader()
    reader.readAsText(file);

    reader.onload = async function() {
        let dataFile = reader.result;
        let itemStrings = dataFile.split("\r\n");

        let lostItems = documentItems;
        let invalidItems = [];

        for (const itemData of itemStrings) {
            let [UPC, quantity] = itemData.split("~");
            if (UPC && quantity) {
                if (UPC.length === 13) {
                    UPC = parseInt(UPC).toString();
                    let verifiedItem = verifyItem(UPC, quantity);
                    if (verifiedItem) {
                        lostItems = lostItems.filter(item => item.itempos !== verifiedItem.itempos);
                    } else {
                        let newItem = await getItemInfo(UPC);
                        if (newItem) {
                            addItem(newItem);
                        } else {
                            let error = {
                                upc: UPC,
                                description: "Этого UPC нет в документе"
                            };
                            invalidItems.push(error);
                        }
                    }
                } else {
                    let error = {
                        upc: UPC,
                        description: "Не соответствует формату"
                    };
                    invalidItems.push(error);
                }
            }
        }

        for (let i = 0; i < lostItems.length; i++) {
            itemTable.bootstrapTable('updateByUniqueId', {
                id: lostItems[i].itempos,
                row: {
                    read: 0,
                    difference: 0 - lostItems[i].quantity,
                }
            });
        }

        if (invalidItems) {
            initErrorTable();
            for (let i = 0; i < invalidItems.length; i++) {
                errorTable.bootstrapTable("append", invalidItems[i]);
            }
        }
    }
}

function verifyItem (UPC, quantity) {
    let item = documentItems.find((item) => item.upc === UPC);

    if (!item) {
        return null;
    }

    let difference = quantity - item.quantity;

    $("#table").bootstrapTable('updateByUniqueId', {
        id: item["itempos"],
        row: {
            read: quantity,
            difference: difference,
        }
    });

    return item;
}

async function getItemInfo (UPC) {
    const cols = ["description1", "description2"];
    let url = HOSTNAME + "/v1/rest/inventory?cols=upc," + cols.join() + "&filter=upc,eq," + UPC;
    let response = await fetch(url, {
        method: "GET",
        headers: {
            'Auth-Session': session.token,
        },
    });
    let result = await response.json();

    if (result) {
        return result[0];
    } else {
        return null;
    }
}

function initItemsTable () {
    $('#table').bootstrapTable({
        columns: [{
            field: 'position',
            title: '№'
        }, {
            field: 'upc',
            title: 'UPC'
        }, {
            field: 'description1',
            title: 'Описание 1'
        }, {
            field: 'description2',
            title: 'Описание 2'
        }, {
            field: 'quantity',
            title: 'Кол-во'
        }, {
            field: 'read',
            title: 'Считано'
        }, {
            field: 'difference',
            title: 'Расхождение'
        }],
        uniqueId: 'position',
        sortName: 'position',
        sortOrder: 'asc'
    });
}

function initErrorTable () {
    $("#errors")[0].style.display = "block";
    $("#error-table").bootstrapTable({
        columns: [{
            field: "upc",
            title: "UPC"
        }, {
            field: "description",
            title: "Описание ошибки"
        }]
    });
}

function addItem (item) {
    $('#table').bootstrapTable("append", {
        position: item.itempos,
        upc: item.upc,
        description1: item.description1,
        description2: item.description2,
        quantity: item.quantity,
        read: item.read ?? 0,
        difference: 0,
    });
}

async function getItemsFromAPI(SID, verificationType) {
    let url =
        HOSTNAME +
        "/api/backoffice/" +
        verificationTypeParams[verificationType].documentName +
        "/" +
        SID +
        "/" +
        verificationTypeParams[verificationType].documentItemName +
        "?cols=itempos,upc,description1,description2," +
        verificationTypeParams[verificationType].itemQty;

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
        return response.data;
    }
}

async function updateField (fieldName, newValue, verificationType) {
    let url =
        HOSTNAME +
        "/api/backoffice/" +
        verificationTypeParams[verificationType].documentName +
        "/" +
        SID +
        "?cols=rowversion";

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

    if (!response.data.length) {
        return null;
    }

    console.log(response.data);
    let rowVersion = response.data[0]["rowversion"];

    url =
        HOSTNAME +
        "/api/backoffice/" +
        verificationTypeParams[verificationType].documentName +
        "/" +
        SID +
        "?filter=rowversion,eq," +
        rowVersion;

    response = await fetch(url, {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json, version=2',
            'Accept': 'application/json, version=2',
            'Accept-Encoding': 'gzip, deflate, br',
            'Auth-Session': session.token
        },
        body: `{"data":[{"rowversion":${rowVersion},"${fieldName}":"${newValue}"}]}`
    });

    response = await response.json();

    if (!response.data.length) {
        return null;
    }
}

function verifyDocument () {
    const d = new Date();
    const userSID = session["employeesid"] ?? null;  // add user sid from session data

    if (!userSID) {
        return null;
    }

    let verificationInformation = `Верифицировано ${userSID}:${d.toJSON()}`;
    let verification = updateField(verificationField, verificationInformation, verificationType);

    return verification;
}


loadDocumentContent();


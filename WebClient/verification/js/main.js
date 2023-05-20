const verificationTypeParams = {
    voucher: {
        documentName: "receiving",
        documentNumber: "vouno",
        documentItemName: "recvitem",
        itemQty: "qty"
    },
    po: {
        documentName: "purchaseorder",
        documentNumber: "pono",
        documentItemName: "poitem",
        itemQty: "ordqty"
    },
    so: {
        documentName: "adjustment",
        documentNumber: "adjno",
        documentItemName: "adjitem",
        itemQty: "adjvalue"
    }
};
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

        let scannedItems = {};
		for (const item of itemStrings) {
			let ALU = item.split("~")[0];
            if (ALU.length > 5) {
                    if (!scannedItems[ALU]) {
                        scannedItems[ALU] = 1;
                    } else {
                        scannedItems[ALU] += 1;
                    }
                } else {
                    alert("ALU " + ALU + " не соответствует формату.");
                }
        }

        let lostItems = documentItems;
        let invalidItems = [];
        for (const [ ALU, quantity ] of Object.entries(scannedItems)) {
            if (ALU.length > 5) {
                // ALU = parseInt(ALU).toString();
                let verifiedItem = verifyItem(ALU, quantity);
                if (verifiedItem) {
                    lostItems = lostItems.filter(item => item.itempos !== verifiedItem.itempos);
                } else {
                    let newItem = await getItemInfo(ALU);
                    newItem.read = quantity;
					newItem.difference = quantity;
                    if (newItem) {
                        addItem(newItem);
                    } else {
                        let error = {
                            alu: ALU,
                            description: "Этого ALU нет в документе"
                        };
                        invalidItems.push(error);
                    }
                }
            } else {
                let error = {
                    alu: ALU,
                    description: "Не соответствует формату"
                };
                invalidItems.push(error);
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

        if (invalidItems.length) {
            initErrorTable();
            for (let i = 0; i < invalidItems.length; i++) {
                errorTable.bootstrapTable("append", invalidItems[i]);
            }
        }
    }
}

function verifyItem (ALU, quantity) {
    let item = documentItems.find((item) => item.alu === ALU);

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

async function getItemInfo (ALU) {
    const cols = ["description1", "description2"];
    let url = HOSTNAME + "/v1/rest/inventory?cols=alu," + cols.join() + "&filter=alu,eq," + ALU;
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
            field: 'alu',
            title: 'ALU'
        }, {
            field: 'description1',
            title: 'Описание 1'
        }, {
            field: 'description2',
            title: 'Описание 2'
        }, {
            field: 'quantity',
            title: 'В документе'
        }, {
            field: 'read',
            title: 'Сканировано'
        }, {
            field: 'difference',
            title: 'Расхождение'
        }],
        uniqueId: 'position',
        sortName: 'position',
        sortOrder: 'asc',
        showExport: true
    });
}

function initErrorTable () {
    $("#errors")[0].style.display = "block";
    $("#error-table").bootstrapTable({
        columns: [{
            field: "alu",
            title: "ALU"
        }, {
            field: "description",
            title: "Описание ошибки"
        }]
    });
}

function addItem (item) {
    $('#table').bootstrapTable("append", {
        position: item.itempos,
        alu: item.alu,
        description1: item.description1,
        description2: item.description2,
        quantity: item.quantity ?? 0,
        read: item.read ?? 0,
        difference: item.difference ?? 0,
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
        "?cols=itempos,upc,description1,description2,alu," +
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
        "?cols=rowversion," +
        verificationTypeParams[verificationType].documentNumber;

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

    // get document number for verification file name and row version for the next request
    let docNumber = response.data[0][verificationTypeParams[verificationType].documentNumber];
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

    return docNumber;
}

async function verifyDocument () {
    const d = new Date();
    const userSID = session["employeesid"] ?? null;  // add user sid from session data

    if (!userSID) {
        return null;
    }

    let verificationInformation = `Верифицировано ${userSID}:${d.toJSON()}`;
    let verifiedDocNum = await updateField(verificationField, verificationInformation, verificationType);

    $('#table').tableExport({
        type: 'excel',
        fileName: `${verificationType}${verifiedDocNum}-${d.toJSON().split("T")[0]}`
    });

    return verifiedDocNum;
}


loadDocumentContent();
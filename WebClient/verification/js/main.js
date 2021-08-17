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

var documentItems = [];


async function init () {const urlParams = new URLSearchParams(window.location.search);
    const verificationType = urlParams.get("type");
    const SID = urlParams.get("sid");

    if (!(verificationType && SID)) {
        alert("Недостаточно данных для верификации");
        window.close();
    }

    const items = await getItemsFromAPI(SID, verificationType);
    initTable();

    for (let i = 0; i < items.length; i++) {
        items[i].quantity = items[i][verificationTypeParams[verificationType].itemQty];
        addItem(items[i]);
    }

    documentItems = items;
}

function loadFile(input) {
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
                        let error = {
                            upc: UPC,
                            description: "Этого UPC нет в документе"
                        };
                        invalidItems.push(error);  
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
            $("#table").bootstrapTable('updateByUniqueId', {
                id: lostItems[i].itempos,
                row: {
                    read: 0,
                    difference: 0 - lostItems[i].quantity,
                }
            });
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
        id: item.itempos,
        row: {
            read: quantity,
            difference: difference,
        }
    });

    return item;
}

async function loadItemInfo (UPC) {
    const cols = ["description1", "description2"];
    let url = HOSTNAME + "/v1/rest/inventory?cols=upc," + cols.join() + "&filter=upc,eq," + UPC;
    let response = await fetch(url, {
        method: "GET",
        headers: {
            'Auth-Session': session.token,
        },
    });
    let result = await response.json();
    console.log(result);
}

function initTable () {
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


init();


const documentTypeParams = {
	voucher: {
		documentName: "receiving",
		documentItemName: "recvitem",
		documentSIDName: "vousid",
		documentItemQty: "qty"
	},
	slip: {
		documentName: "transferslip",
		documentItemName: "slipitem",
		documentSIDName: "slipsid",
		documentItemQty: "qty"
	},
	adjustment: {
		documentName: "adjustment",
		documentItemName: "adjitem",
		documentSIDName: "adjsid",
		documentItemQty: "adjvalue"
	}
};

const upcRE = /^[0-9]{5,13}$/;

function loadFile(input, documentType) {
	let file = input.files[0];
	let reader = new FileReader();
	reader.readAsText(file);

	reader.onload = async function() {
		session = JSON.parse(sessionStorage.getItem("session"));
		let href = window.location.href.split("/");
		let sidIndex = href.findIndex((element) => (element === "new" || element === "edit" || element === "view")) + 1;
    	let documentSID = href[sidIndex].split("?")[0];
		console.log(documentSID);

		let dataFile = reader.result;
		let itemStrings = dataFile.split("\r\n");

		let scannedItems = {};
		for (const item of itemStrings) {
			let UPC = item.split("~")[0];
			if (upcRE.test(UPC)) {
				if (!scannedItems[UPC]) {
					scannedItems[UPC] = 1;
				} else {
					scannedItems[UPC] += 1;
				}
			} else {
				alert("UPC " + UPC + " не соответствует формату.");
			}
		}

		let documentItems = await getItemsFromAPI(documentSID, documentType);

		let invItems = [];
		let updateItems = [];

		for (const [ UPC, quantity ] of Object.entries(scannedItems)) {
			let itemIndex;

			if (documentItems !== undefined) {
				console.log(documentItems);
				itemIndex = documentItems.findIndex(item => item.alu == UPC);
				console.log("item index for " + UPC + " " + quantity + ": " + itemIndex);
			} else {
				itemIndex = -1;
			}
			
			if (itemIndex > -1) {
				let updateItem = documentItems[itemIndex];
				updateItem[documentTypeParams[documentType].documentItemQty] += quantity;
				updateItems.push(updateItem);
			} else {
				let invItem = await generateDocumentItem(UPC, quantity, documentSID, documentType);
				if (invItem === -1) {
					alert("Товара с UPC " + UPC + " не существует!");
				} else {
					invItems.push(invItem);
				}
			}
		}
		console.log("updating", updateItems);
		console.log("creating", invItems);
		
		await updateDocumentItems(updateItems, documentSID, documentType);
		await addDocumentItems(invItems, documentSID, documentType);

		reloadPage();
	};

	reader.onerror = function() {
		console.log(reader.error);
	};
}

async function getItemsFromAPI(SID, verificationType) {
    let url =
        HOSTNAME +
        "/api/backoffice/" +
        documentTypeParams[verificationType].documentName +
        "/" +
        SID +
        "/" +
        documentTypeParams[verificationType].documentItemName +
        "?cols=upc,alu,rowversion," +
        documentTypeParams[verificationType].documentItemQty;

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

async function getSIDByUPC (targetUPC, cols=[]) {
	let url = HOSTNAME + "/v1/rest/inventory?cols=sid,upc," + cols.join() + "&filter=upc,eq," + targetUPC;
	console.log(url);
	let response = await fetch(url, {
		method: "GET",
		headers: {
			'Auth-Session': session.token,
		}
	});
	let result = await response.json();
	if (result.length === 0) {
		return -1;
	}

	return result[0]["sid"];
}

async function generateDocumentItem (UPC, quantity, documentSID, documentType) {
	let itemSID = await getSIDByUPC(UPC);

	if (itemSID == -1) {
		return -1;
	}

	let invItem = {
		originApplication: "RProPrismWeb",
		UPC: UPC,
		qty: quantity,
		itemSID: itemSID
	};

	if (documentType === "adjustment") {
		invItem.adjustmenttype = 0;
		invItem.adjvalue = quantity;
	}

	invItem[documentTypeParams[documentType].documentSIDName] = documentSID;

	return invItem;
}

async function addDocumentItems (items, documentSID, documentType) {
	let response = await fetch(
		HOSTNAME + "/api/backoffice/" + 
		documentTypeParams[documentType].documentName + "/" + documentSID + "/" + 
		documentTypeParams[documentType].documentItemName,
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json, version=2',
				'Accept': 'application/json, version=2',
				'Accept-Encoding': 'gzip, deflate, br',
				'Auth-Session': session.token
			},
			body: JSON.stringify({ "data": items })
		});
	let result = await response.json();
	console.log(result);
}

async function updateDocumentItems(items, documentSID, documentType) {
	items.forEach(async docItem => {
		let response = await fetch(
			HOSTNAME + "/api/backoffice/" + 
			documentTypeParams[documentType].documentName + "/" + documentSID + "/" + 
			documentTypeParams[documentType].documentItemName + "/" + docItem.sid + 
			"?filter=rowversion,eq," + docItem.rowversion,
			{
				method: 'PUT',
				headers: {
					'Content-Type': 'application/json, version=2',
					'Accept': 'application/json, version=2',
					'Accept-Encoding': 'gzip, deflate, br',
					'Auth-Session': session.token
				},
				body: JSON.stringify({ "data": [ docItem ] })
			});
		let result = await response.json();
		console.log(result);
	});
}

function reloadPage() {
	document.location.reload();
}

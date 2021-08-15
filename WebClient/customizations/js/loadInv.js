const HOSTNAME  = "http://172.16.1.102";
const documentTypeParams = {
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

var session;

function loadFile(input, documentType) {
	let file = input.files[0];
	let reader = new FileReader();
	reader.readAsText(file);

	reader.onload = async function() {
		session = JSON.parse(sessionStorage.getItem("session"));
		let href = window.location.href.split("/");
		let documentSID = href[href.findIndex((element) => element === "new") + 1];

		let dataFile = reader.result;
		let itemStrings = dataFile.split("\r\n");

		let invItems = [];

		for (const item of itemStrings) {
			let [UPC, quantity] = item.split("~");
			if (UPC && quantity) {
				if (UPC.length === 13) {
					let invItem = await generateDocumentItem(UPC, parseInt(quantity), documentSID, documentType);
					if (invItem === -1) {
						alert("Товара с UPC " + UPC + " не существует!");
					} else {
						invItems.push(invItem);
					}
				} else {
					alert("UPC " + UPC + " не соответствует формату.");
				}
			}
		}

		console.log(invItems);
		await addDocumentItems(invItems, documentSID, documentType);

		// reloadPage();
	};

	reader.onerror = function() {
		console.log(reader.error);
	};
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

function reloadPage() {
	document.location.reload();
}
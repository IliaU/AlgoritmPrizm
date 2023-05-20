const operations = { // 1=Sale;2=Return;3=Order;4=Exchange;5=Void
	1: "Продажа",
	2: "Возврат",
	3: "Заказ",
	4: "Обмен",
	5: "Отмена",
}

var rows = [];
var quantity;
var doc_quantity;

function initTable1 () {
	$('#table_1').bootstrapTable({
		columns: [{
			field: 'upc',
			title: '№ товара'
		}, {
			field: 'description1',
			title: 'Опис 1'
		}, {
			field: 'description2',
			title: 'Опис 2'
		}, {
			field: 'description3',
			title: 'Опис 3'
		}]
	});
}

function datesSorter(a, b) {
	if (new Date(a) < new Date(b)) return 1;
	if (new Date(a) > new Date(b)) return -1;
	return 0;
}

function rowsSorter(a, b) {
	if (new Date(a.date_time) < new Date(b.date_time)) return 1;
	if (new Date(a.date_time) > new Date(b.date_time)) return -1;
	return 0;
}

function initTable2 () {
	$('#table_2').bootstrapTable({
		columns: [{
			field: 'date_time',
			title: 'Дата/время'
		}, {
			field: 'operation',
			title: 'Операция',
		}, {
			field: 'doc_number',
			title: '№ док-та'
		}, {
			field: 'mag',
			title: 'Маг'
		}, {
			field: 'doc_count',
			title: 'К-во док'
		}, {
			field: 'comp_count',
			title: 'К-во КОМП'
		}, {
			field: 'count',
			title: 'К-во МАГ'
		}],
		sortName: 'date_time',
		sortOrder: 'asc',
		sorter: datesSorter,
	});
}

function applyFilter () {
	// get movements table and filter forms
	let table = $("#table_2");

	// available operations
	let operationsTitles = {
		"voucher": "Накладная",
		"voucher-return": "Накладная возврата",
		"document-sale": "Продажа",
		"document-return": "Возврат продажи",
		"slip-to": "Входящее перемещение",
		"slip-from": "Исходящее перемещение",
		"adjustment": "Корректировка"
	};

	// check selected operations
	let movementTypes = Object.keys(operationsTitles);
	let selectedOperations = [];
	for (let i = 0; i < movementTypes.length; i++) {
		let operation = movementTypes[i];
		if (this[operation].checked) {
			selectedOperations = selectedOperations.concat(operationsTitles[operation]);
		}
	}

	// filter table
	table.bootstrapTable("filterBy", {operation: selectedOperations});
}

async function getSessionToken (authNonce, authNonceResponse) {
	await fetch(HOSTNAME + "/v1/rest/auth?usr=sysadmin&pwd=sysadmin", {
		method: 'GET',
		headers: {
			'Content-Type': 'application/json, version=2',
			'Accept': 'application/json, version=2',
			'Accept-Encoding': 'gzip, deflate, br',
			'Auth-Nonce': authNonce,
			'Auth-Nonce-Response': authNonceResponse,
		},
	}).then(
		(result) => {
			let authSession = result.headers.get('Auth-Session');
			localStorage.setItem('Auth-Session', authSession);
		},
		(error) => {
			console.log(error);
		}
	);
}

async function auth () {
	await fetch(HOSTNAME + "/v1/rest/auth", {
		method: 'GET',
		headers: {
			'Content-Type': 'application/json, version=2',
			'Accept': 'application/json, version=2',
			'Accept-Encoding': 'gzip, deflate, br'
		},
	}).then(
		async (result) => {
			let authNonce = result.headers.get('Auth-Nonce');
			let authNonceResponse = (Math.trunc(authNonce / 13) % 99999) * 17;
			localStorage.setItem('Auth-Nonce', authNonce.toString());
			localStorage.setItem('Auth-Nonce-Response', authNonceResponse.toString());
			await getSessionToken(authNonce, authNonceResponse).then(() => {
				fetch(HOSTNAME + "/v1/rest/sit?ws=reports", {
					method: "GET",
					headers: { 'Auth-Session': localStorage.getItem('Auth-Session') }
				});
			});
		},
		(error) => {
			console.log(error);
		}
	);
}

async function getStores (invn_sid) {
	let cols = ["sid", "store_code", "store_name"];

	let i = 1;
	var stores = [];
	let readAll = false;
	let url = HOSTNAME + "/v1/rest/store?cols=" + cols.join() + "&filter=(active,eq,true)";

	while (!readAll) {
		let storesPage = await fetch(
			url + "&page_no=" + i.toString() + "&page_size=100",
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json, version=2',
					'Accept': 'application/json, version=2',
					'Accept-Encoding': 'gzip, deflate, br',
					'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
					'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
					'Auth-Session': localStorage.getItem('Auth-Session')
				}
			});
		storesPage = await storesPage.json();
		stores = stores.concat(storesPage);

		if ((i > 10) || (storesPage.length < 100)) readAll = true;
		i++;
	}

	for (let i = 0; i < stores.length; i++) {
		 let quantityInfo = await getItemQuantity(invn_sid, stores[i]["sid"]);
		 stores[i]["ohqty"] = quantityInfo["ohqty"];
		 stores[i]["availqty"] = quantityInfo["availqty"];
	}

	return stores;
}

async function loadDataByUPC (targetUPC, cols=[]) {
	let url = HOSTNAME + "/v1/rest/inventory?cols=sid,item_no,upc," + cols.join() + "&filter=upc,eq," + targetUPC;

	let item = await fetch(url, {
		method: "GET",
		headers: {
			'Auth-Session': localStorage.getItem('Auth-Session'),
		}
	});

	item = await item.json();

	return item[0];
}

async function getItemQuantity (invn_sid, store_sid) {
	let response = await fetch(HOSTNAME + "/api/backoffice/?action=getavailqty",
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json, version=2',
				'Accept': 'application/json, version=2',
				'Accept-Encoding': 'gzip, deflate, br',
				'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
				'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
				'Auth-Session': localStorage.getItem('Auth-Session')
			},
			body: JSON.stringify({ "data": [{
					"ItemSid": invn_sid,
					"StoreSid": store_sid,
				}] })
		});
	response = await response.json();

	return response["data"][0];
}

async function getAdjustments (startDate, cols=[]) {
	var adjustments = [];
	let i = 1;
	let readAll = false;
	let url = HOSTNAME + "/api/backoffice/adjustment?filter=createddatetime,ge," + startDate;
	while (!readAll)  {
		await fetch(
			url
			+ "&sort=createddatetime,desc&cols=createddatetime,adjno,storecode" + cols.join() + "&page_no="
			+ i.toString() + "&page_size=100",
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json, version=2',
					'Accept': 'application/json, version=2',
					'Accept-Encoding': 'gzip, deflate, br',
					'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
					'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
					'Auth-Session': localStorage.getItem('Auth-Session')
				}
			}).then(response => {
			console.log(response.json().then(resDocs => {
				adjustments = adjustments.concat(resDocs.data);
				if (resDocs.data.length < 100) {
					readAll = true;
				}
			}));
		});

		if (i > 10) readAll = true;

		i++;
	}

	return adjustments;
}

async function getDocuments (startDate) {
	var documents = [];
	let i = 1;
	let readAll = false;
	let url = HOSTNAME +
		"/v1/rest/document?filter=(created_datetime,ge," + startDate +
		")AND((document_number,nn)OR(order_document_number,nn))";

	while (!readAll)  {
		await fetch(
			url
			+ "&sort=created_datetime,desc&cols=created_datetime,items,document_number,store_code,store_uid&page_no="
			+ i.toString() + "&page_size=100",
			{
					method: 'GET',
					headers: {
						'Content-Type': 'application/json, version=2',
						'Accept': 'application/json, version=2',
						'Accept-Encoding': 'gzip, deflate, br',
						'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
						'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
						'Auth-Session': localStorage.getItem('Auth-Session')
					}
			}).then(response => {
				console.log(response.json().then(resDocs => {
					documents = documents.concat(resDocs);
					if (resDocs.length < 100) {
						readAll = true;
					}
				}));
			});

		i++;
	}

	return documents;
}

async function getSlips (startDate, cols=["slipno", "outstorecode", "instorecode"]) {
	var slips = [];
	let i = 1;
	let readAll = false;
	let url = HOSTNAME + "/api/backoffice/transferslip?filter=createddatetime,ge," + startDate;
	while (!readAll)  {
		await fetch(
			url
			+ "&sort=createddatetime,desc&cols=createddatetime," + cols.join() + "&page_no="
			+ i.toString() + "&page_size=100",
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json, version=2',
					'Accept': 'application/json, version=2',
					'Accept-Encoding': 'gzip, deflate, br',
					'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
					'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
					'Auth-Session': localStorage.getItem('Auth-Session')
				}
			}).then(response => {
			console.log(response.json().then(resDocs => {
				slips = slips.concat(resDocs.data);
				if (resDocs.data.length < 100) {
					readAll = true;
				}
			}));
		});

		if (i > 10) readAll = true;

		i++;
	}

	return slips;
}

async function getVouchers (startDate, cols=[]) {
	var vouchers = [];
	let i = 1;
	let readAll = false;
	let url = HOSTNAME + "/api/backoffice/receiving?filter=createddatetime,ge," + startDate;
	while (!readAll)  {
		await fetch(
			url
			+ "&sort=createddatetime,desc&cols=createddatetime,vouno,storecode,voutype" + cols.join() + "&page_no="
			+ i.toString() + "&page_size=100",
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json, version=2',
					'Accept': 'application/json, version=2',
					'Accept-Encoding': 'gzip, deflate, br',
					'Auth-Nonce': localStorage.getItem('Auth-Nonce'),
					'Auth-Nonce-Response': localStorage.getItem('Auth-Nonce-Response'),
					'Auth-Session': localStorage.getItem('Auth-Session')
				}
			}).then(response => {
			console.log(response.json().then(resDocs => {
				vouchers = vouchers.concat(resDocs.data);
				if (resDocs.data.length < 100) {
					readAll = true;
				}
			}));
		});

		if (i > 10) readAll = true;

		i++;
	}

	return vouchers;
}

async function loadAdjustment (targetUPC, document, save=true, cols=[]) {
	let SID = document.sid;
	cols = cols.concat(["item_type", "createddatetime", "invn_sbs_item_sid", "origvalue", "adjvalue", "adjustmenttype"]);

	let url = HOSTNAME + "/api/backoffice/adjustment/" + SID + "/adjitem?cols=upc," + cols.join() + "&filter=upc,eq," + targetUPC;
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
		let doc = response.data[0];

		if (parseInt(doc.adjustmenttype) !== 0 || !document.adjno) {
			return -1;
		}

		let time = doc["createddatetime"].slice(0,-10).split("T").join(" ")
			?? "Error";
		let operation = "Корректировка";
		let doc_number = document.adjno;
		let store = document.storecode;

		let orig_value = doc["origvalue"];
		let adj_value = doc["adjvalue"];
		let in_doc_quantity = adj_value - orig_value;

		let row = {
			date_time: time,
			operation: operation,
			doc_number: doc_number,
			mag: store,
			doc_count: in_doc_quantity,
			count: adj_value
		}

		// $('#table_2').bootstrapTable("append", row);

		doc_quantity["availqty"] -= in_doc_quantity;
		doc_quantity["ohqty"] -= in_doc_quantity;

		return [row];
	}
}

async function loadDoc (targetUPC, document, save=true, cols=[]) {
	cols = cols.concat(["item_type", "created_datetime", "invn_sbs_item_sid", "quantity", "store_code", "inventory_on_hand_quantity"]);
	for (let i = 0; i < document.items.length; i++) {
		let url = HOSTNAME + document.items[i].link + "?cols=scan_upc," + cols.join() + "&filter=scan_upc,eq," + targetUPC;
		let response = await fetch(url, {
			method: "GET",
			headers: {
				'Auth-Session': localStorage.getItem('Auth-Session'),
			}
		});

		response = await response.json();

		if (response.length) {
			let doc = response[0];

			let time = doc["created_datetime"].slice(0,-10).split("T").join(" ")
				?? "Error";
			let operation = operations[doc["item_type"]] ?? "Unknown operation type";
			let doc_number = document["document_number"];
			let store = document["store_code"];
			let in_doc_quantity = await doc["quantity"];
			let inventory_on_hand_quantity = doc["inventory_on_hand_quantity"];

			if (doc["item_type"] === 1) {
				in_doc_quantity = -in_doc_quantity;
			}

			let row = {
				date_time: time,
				operation: operation,
				doc_number: doc_number,
				mag: store,
				doc_count: in_doc_quantity,
				count: inventory_on_hand_quantity + in_doc_quantity
			};

			return [row];
		}
	}
}

async function loadSlip (targetUPC, document, save=true, cols=[]) {
	let SID = document.sid;
	cols = cols.concat(["item_type", "createddatetime", "invn_sbs_item_sid", "qty"]);

	let url = HOSTNAME + "/api/backoffice/transferslip/" + SID + "/slipitem?cols=upc," + cols.join() + "&filter=upc,eq," + targetUPC;
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
		let doc = response.data[0];

		if (!document.slipno) {
			return -1;
		}

		let time = doc["createddatetime"].slice(0,-10).split("T").join(" ")
			?? "Error";
		let operation = "Перемещение";
		let doc_number = document.slipno;
		let from_store = document.outstorecode;
		let to_store = document.instorecode;
		let qty = await doc.qty;

		let rowFrom = {
			date_time: time,
			operation: operation,
			doc_number: doc_number,
			mag: from_store,
			doc_count: -qty
		};
		let rowTo = {
			date_time: time,
			operation: operation,
			doc_number: doc_number,
			mag: to_store,
			doc_count: qty
		}

		return [rowFrom, rowTo];
	}
}

async function loadVoucher (targetUPC, document, save=true, cols=[]) {
	let SID = document.sid;
	cols = cols.concat(["item_type", "createddatetime", "invn_sbs_item_sid", "qty"]);

	let url = HOSTNAME + "/api/backoffice/receiving/" + SID + "/recvitem?cols=upc," + cols.join() + "&filter=upc,eq," + targetUPC;
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
		let doc = response.data[0];

		let time = doc["createddatetime"].slice(0,-10).split("T").join(" ")
			?? "Error";
		let operation = "Накладная";
		let doc_number = document.vouno;
		let store = document.storecode;
		let type = document.voutype;

		let qty = await doc["qty"];
		if (type === 1) {
			qty = -qty;
		}

		let row = {
			date_time: time,
			operation: operation,
			doc_number: doc_number,
			mag: store,
			doc_count: qty
		}

		// $('#table_2').bootstrapTable("append", row);

		return [row];
	}
}

async function showReport (searchButton) {
	let startDate = $('#date_from')[0].value;
	let UPC = $('#upc_input')[0].value;

	rows = [];

	// add spinner to the search button
	searchButton.disabled = true;

	// try to get item info
	let item = await loadDataByUPC(UPC, ["description1", "description2", "description3"]);
	if (!item) {
		alert("Товара с таким UPC не существует");
		searchButton.disabled = false;
		return;
	}

	// get stores information and item quantity in them
	let stores = await getStores(item["sid"]);
	let companyQuantity = stores.reduce((a, b) => a + b["ohqty"], 0);
	console.log(companyQuantity);

	// load documents
	let docs = await getDocuments(startDate);

	quantity = await getItemQuantity(item["sid"], docs[0]["store_uid"]);
	doc_quantity = quantity;

	// load documents
	for (let i = 0; i < docs.length; i++) {
		let doc = await loadDoc(UPC, docs[i]);

		if (doc) {
			rows = rows.concat(doc);
		}
	}

	// load adjustments
	let adjs = await getAdjustments(startDate);

	for (let i = 0; i < adjs.length; i++) {
		let adj = await loadAdjustment(UPC, adjs[i]);

		if (adj) {
			rows = rows.concat(adj);
		}
	}

	// load slips
	let slips = await getSlips(startDate);

	for (let i = 0; i < slips.length; i++) {
		let slip = await loadSlip(UPC, slips[i]);

		if (slip) {
			rows = rows.concat(slip);
		}
	}

	// load vouchers
	let vouchers = await getVouchers(startDate);

	for (let i = 0; i < vouchers.length; i++) {
		let voucher = await loadVoucher(UPC, vouchers[i]);

		if (voucher) {
			rows = rows.concat(voucher);
		}
	}

	//sort rows by date
	rows = rows.sort(rowsSorter);

	// clean tables and append item info
	let infoTable = $('#table_1');
	let movementsTable = $('#table_2');

	infoTable.bootstrapTable("removeAll");
	movementsTable.bootstrapTable("removeAll");

	infoTable.bootstrapTable("append", item);

	// recount quantities and show all rows
	for (let i = 0; i < rows.length; i++) {
		// get store
		let row = rows[i];
		let storeIndex = stores.findIndex(x => x["store_code"] === row["mag"]);

		// save current quantities
		row["comp_count"] = companyQuantity;
		row["count"] = stores[storeIndex]["ohqty"];

		// update quantities
		companyQuantity -= row["doc_count"];
		stores[storeIndex]["ohqty"] -= row["doc_count"];

		// append row to the table
		movementsTable.bootstrapTable("append", rows[i]);
	}

	// make search button available
	searchButton.disabled = false;
}

// get auth token
auth();

// init tables and remove titles
initTable1();
initTable2();
$(".no-records-found").remove();

// date_to equals current date
document.getElementById('date_to').valueAsDate = new Date();





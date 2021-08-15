const HOSTNAME  = "http://172.16.1.102"
const operations = { // 1=Sale;2=Return;3=Order;4=Exchange;5=Void
	1: "Продажа",
	2: "Возврат",
	3: "Заказ",
	4: "Обмен",
	5: "Отмена",
}

var item_docs = [];
var rows = [];
var quantity;
var doc_quantity;

function initTable1 (data=[]) {
	$('#table_1').bootstrapTable({
		columns: [{
			field: 'number',
			title: '№ товара'
		}, {
			field: 'OKS_name',
			title: 'Имя ОКС'
		}, {
			field: 'OKS',
			title: 'ОКС'
		}, {
			field: 'Opis_1',
			title: 'Опис 1'
		}, {
			field: 'Opis_2',
			title: 'Опис 2'
		}, {
			field: 'Opis_3',
			title: 'Опис 3'
		}],
		data: data
	});
}

function datesSorter(a, b) {
	if (new Date(a) < new Date(b)) return 1;
	if (new Date(a) > new Date(b)) return -1;
	return 0;
}

function initTable2 (data) {
	$('#table_2').bootstrapTable({
		columns: [{
			field: 'date_time',
			title: 'Дата/время'
		}, {
			field: 'operation',
			title: 'Операция'
		}, {
			field: 'doc_number',
			title: '№ док-та'
		}, {
			field: 'kontragent',
			title: 'Контрагент'
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

async function getAdjustments (startDate, cols=[]) {
	var adjustments = [];
	let i = 1;
	let readAll = false;
	let url = HOSTNAME + "/api/backoffice/adjustment?filter=createddatetime,ge," + startDate;
	while (!readAll)  {
		await fetch(
			url
			+ "&sort=createddatetime,desc&cols=createddatetime,adjno,storeno" + cols.join() + "&page_no="
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
			+ "&sort=created_datetime,desc&cols=created_datetime,items,document_number,store_number,store_uid&page_no="
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

async function getSlips (startDate, cols=["slipno", "outstoreno", "instoreno"]) {
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
			+ "&sort=createddatetime,desc&cols=createddatetime,vouno,storeno,voutype" + cols.join() + "&page_no="
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
	cols = cols.concat(["item_type", "createddatetime", "invn_sbs_item_sid", "quantity", "adjustmenttype"]);

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
		let store = document.storeno;

		// let in_doc_quantity = doc["quantity"];
		// if (doc["item_type"] === 1) {
		// 	in_doc_quantity = -in_doc_quantity;
		// }

		// $('#table_2').bootstrapTable("append",
		if (save) {
			$('#table_2').bootstrapTable("append", {
				date_time: time,
				operation: operation,
				doc_number: doc_number,
				kontragent: '',
				mag: store,
				doc_count: "n/a",
				comp_count: "n/a",
				count: "n/a"
			});

			// doc_quantity["availqty"] -= in_doc_quantity;
			// doc_quantity["ohqty"] -= in_doc_quantity;
		}
		return doc;
	}
}

async function loadDoc (targetUPC, document, save=true, cols=[]) {
	cols = cols.concat(["item_type", "created_datetime", "invn_sbs_item_sid", "quantity"]);
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
			let store = document["store_number"];

			let in_doc_quantity = await doc["quantity"];
			if (doc["item_type"] === 1) {
				in_doc_quantity = -in_doc_quantity;
			}

			// $('#table_2').bootstrapTable("append",
			if (save) {
				$('#table_2').bootstrapTable("append", {
					date_time: time,
					operation: operation,
					doc_number: doc_number,
					kontragent: '',
					mag: store,
					doc_count: in_doc_quantity,
					comp_count: doc_quantity["availqty"],
					count: doc_quantity["ohqty"]
				});

				doc_quantity["availqty"] -= in_doc_quantity;
				doc_quantity["ohqty"] -= in_doc_quantity;
			}
			return doc;
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
		let from_store = document.outstoreno;
		let to_store = document.instoreno;

		let qty = await doc.qty;

		// $('#table_2').bootstrapTable("append",
		if (save) {
			// from
			$('#table_2').bootstrapTable("append", {
				date_time: time,
				operation: operation,
				doc_number: doc_number,
				kontragent: '',
				mag: from_store,
				doc_count: -qty,
				comp_count: "n/a",
				count: "n/a"
			});

			// to
			$('#table_2').bootstrapTable("append", {
				date_time: time,
				operation: operation,
				doc_number: doc_number,
				kontragent: '',
				mag: to_store,
				doc_count: qty,
				comp_count: "n/a",
				count: "n/a"
			});
		}
		return doc;
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
		let store = document.storeno;
		let type = document.voutype;

		let qty = await doc["qty"];
		if (type === 1) {
			qty = -qty;
		}

		// $('#table_2').bootstrapTable("append",
		if (save) {
			$('#table_2').bootstrapTable("append", {
				date_time: time,
				operation: operation,
				doc_number: doc_number,
				kontragent: '',
				mag: store,
				doc_count: qty,
				comp_count: "n/a",
				count: "n/a"
			});

			// doc_quantity["availqty"] -= in_doc_quantity;
			// doc_quantity["ohqty"] -= in_doc_quantity;
		}
		return doc;
	}
}

async function loadDataByUPC (targetUPC, cols=[]) {
	let url = HOSTNAME + "/v1/rest/inventory?cols=item_no,upc," + cols.join() + "&filter=upc,eq," + targetUPC;
	await fetch(url, {
		method: "GET",
		headers: {
			'Auth-Session': localStorage.getItem('Auth-Session'),
		}}).then(result => {result.json().then(async item => {
			initTable1([{
				number: item[0]['upc'],
				OKS_name: '',
				OKS: '',
				Opis_1: item[0]['description1'],
				Opis_2: item[0]['description2'],
				Opis_3: item[0]['description3']
			}]);
	});
	});
}

async function getItemQuantity (doc_item, store_sid) {
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
					"ItemSid": doc_item["invn_sbs_item_sid"],
					"StoreSid": store_sid,
				}] })
	});
	response = await response.json();

	return response["data"][0];
}

async function showReport () {
	let startDate = $('#date_from')[0].value;
	let UPC = $('#upc_input')[0].value;

	$('#table_1').bootstrapTable("removeAll");
	$('#table_2').bootstrapTable("removeAll");

	await loadDataByUPC(UPC, ["description1", "description2", "description3"]);

	initTable2();

	// load documents
	let docs = await getDocuments(startDate);
	let firstDoc = await loadDoc(UPC, docs[0], save=false);
	quantity = await getItemQuantity(firstDoc, docs[0]["store_uid"]);
	doc_quantity = quantity;

	for (let i = 0; i < docs.length; i++) {
		let doc = await loadDoc(UPC, docs[i]);
	}

	// load adjustments
	let adjs = await getAdjustments(startDate);

	for (let i = 0; i < adjs.length; i++) {
		await loadAdjustment(UPC, adjs[i]);
	}

	// load slips
	let slips = await getSlips(startDate);

	for (let i = 0; i < slips.length; i++) {
		await loadSlip(UPC, slips[i]);
	}

	// load vouchers
	let vouchers = await getVouchers(startDate);

	for (let i = 0; i < vouchers.length; i++) {
		await loadVoucher(UPC, vouchers[i]);
	}
}

auth();
document.getElementById('date_to').valueAsDate = new Date();





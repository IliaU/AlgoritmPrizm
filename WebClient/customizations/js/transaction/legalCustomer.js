// opens legal person sale modal and saves necessary information
function legalPersonSale ($http, ModelService, NotificationService, deferred) {
	const innRE = /^[0-9]{10}$/;
	let activeDocument;

	try {
		activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
	} catch(e) {
		NotificationService.addAlert(e.message, `JS Error ${e.name}`);
	}
	let customer_uid = activeDocument.bt_cuid;

	// check if client is company
	console.log("active document", activeDocument);
	ModelService.get('Customer', {sid: customer_uid, cols: "title,primary_address_line_3"}).then(function (data) {
		let customer = data[0];
		let address3 =customer.primary_address_line_3;
		console.log("address3", address3);

		if (!innRE.test(address3)) {
			console.log("выхожу", address3);
			deferred.resolve();
		} else {
			// check cash total to be lower then 100k
			$http({
				method: 'GET',
				url: `${HOSTNAME}/v1/rest/document/${activeDocument.sid}/tender?filter=tender_type,eq,0&cols=*`,
				headers: {
					'Content-Type': 'application/json, version=2',
					'Accept': 'application/json, version=2',
					'Auth-Session': session.token
				}
			}).then(function successCallback(response) {
					let data = response.data;
					console.log(data);
					let cashOK = true;
					if (data[0]) {
						let cash = data[0].amount;
						if (cash >= 100000) {
							NotificationService.addAlert("Продажа юр. лицу более чем на 100 т.р. наличными запрешена. Выберите другое соотношение методов оплаты.", "Ошибка");
							
							cashOK = false;
							activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
							activeDocument.comment1 = `phisycal;`;
							activeDocument.save();
							deferred.reject();
						} else {
							deferred.resolve();
						}
					} else {
						deferred.resolve();
					}
			});
		}
	});

	// activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
	// activeDocument.comment1 = `legal;${inn}`;
	// activeDocument.save();
}

// opens legal sale modal on "Tender Transaction" button click if it's enabled in config
// ButtonHooksManager.addHandler(['before_posTransactionTenderTransaction'],
ButtonHooksManager.addHandler(['before_navPosTenderPrintUpdate', 'before_navPosTenderUpdateOnly'],
	function ($q, $http, ModelService, NotificationService) {
		let deferred = $q.defer();

		let activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal"})[0];

		legalPersonSale($http, ModelService, NotificationService, deferred);


		return deferred.promise;
	});

// opens legal sale modal on special button click
// ButtonHooksManager.addHandler(['before_posLegalPersonSale'],
// ButtonHooksManager.addHandler(['before_navPosTenderPrintUpdate'],
// 	function ($q, $uibModal) {
// 		let deferred = $q.defer();

// 		legalPersonSale($uibModal, deferred);

// 		return deferred.promise;
// 	});



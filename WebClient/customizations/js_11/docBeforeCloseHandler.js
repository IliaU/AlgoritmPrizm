// opens legal person sale modal and saves necessary information
function legalPersonSale ($uibModal, deferred) {
	let modalOptions = {
		backdrop: 'static',
		keyboard: false,
		windowClass: 'full',
		templateUrl: '/customizations/views/legal-modal.html',
		controller: ['$scope', '$http', '$uibModal', '$uibModalStack', '$window', 'ModelService', 'NotificationService',
			async function ($scope, $http, $uibModal, $uibModalStack, $window, ModelService, NotificationService) {
				'use strict';
				// modal parameters
				$scope.criteria = {};
				$scope.message = '';
				$scope.caption = '';
				$scope.legalAvailable = false;

				// load current document
				let activeDocument;

				try {
					activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
				} catch(e) {
					NotificationService.addAlert(e.message, `JS Error ${e.name}`);
				}
				let customer_uid = activeDocument.bt_cuid;

				// check if client is company
				console.log("active document", activeDocument);
				ModelService.get('Customer', {sid: customer_uid, cols: "title"}).then( function (data) {
					let customer = data[0];
					let bt_title =customer.title;
					console.log("title", bt_title);

					if (bt_title !== "Company" && bt_title !== "COMPANY") {
						console.log("выхожу", bt_title);
						$uibModalStack.dismissAll();
						deferred.resolve();
					} else {
						// check cash total to be lower then 100k

						// ModelService.get('Tender', {
						// 	cols: 'amount',
						// 	filter: '((tender_type,eq,0),AND,(document_sid,eq,' + activeDocument.sid +'))'
						// }).then(function (data) {
						// use get all tenders 
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
										$uibModalStack.dismissAll();
										deferred.reject();
									}
								}
								// 	} else {
								// 		// check inn in customer info
								// 		ModelService.get('Customer', {sid: customer_uid, cols: "udffield01"}).then( function (data) {
								// 			let customer = data[0];
		
								// 			if (customer.udffield01) {
								// 				activeDocument.comment1 = `legal;${customer.udffield01}`;
								// 				activeDocument.save();
		
								// 				$uibModalStack.dismissAll();
								// 				deferred.resolve();
								// 			}
								// 		});
								// 	}	
								// } else {

								// }
								
								// check inn in customer info if cash amount is lower than 100k
								if (cashOK) {
									ModelService.get('Customer', {sid: customer_uid, cols: "udffield01"}).then( function (data) {
										let customer = data[0];
		
										if (customer.udffield01) {
											activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
											activeDocument.comment1 = `legal;${customer.udffield01}`;
											activeDocument.save();
		
											$uibModalStack.dismissAll();
											deferred.resolve();
										}
									});
								}
							
						});
					}
				});

				// save legal person information
				$scope.save = function () {
					let inn = $scope.criteria["inn"];

					ModelService.get('Customer', {sid: customer_uid, cols: "sid,row_version,udffield01"}).then( function (data) {
						let customer = data[0];
						
						console.log(customer);
						console.log("inn", inn);

						customer.udffield01 = inn;
						customer.save();
					});

					activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
					activeDocument.comment1 = `legal;${inn}`;
					activeDocument.save();

					$uibModalStack.dismissAll();
					deferred.resolve();
				}

				// close modal
				$scope.cancel = function () {
					activeDocument.comment1 = "physical;;";
					activeDocument.save();

					$uibModalStack.dismissAll();
					deferred.resolve();
				}
			}
		]
	};

	let modalInstance = $uibModal.open(modalOptions);
	modalInstance.result.then(function () {
		deferred.resolve();
	});

}

// opens legal sale modal on "Tender Transaction" button click if it's enabled in config
// ButtonHooksManager.addHandler(['before_posTransactionTenderTransaction'],
ButtonHooksManager.addHandler(['before_navPosTenderPrintUpdate'],
	function ($q, $uibModal, ModelService) {
		let deferred = $q.defer();

		let activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal"})[0];

		legalPersonSale($uibModal, deferred);


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



async function displayText (text) {
    let response = await fetch(UTIL_HOSTNAME + "/display", {
		method: "POST",
        body: JSON.stringify({text: text})
	});
}

ButtonHooksManager.addHandler(['before_posTransactionTenderTransaction'],
	function ($q, $uibModal, $http, ModelService) {
		let deferred = $q.defer();

		let activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal"})[0];
        let amount = activeDocument.sale_subtotal;

        let customerMessage = "AMOUNT:".padStart(20, " ") + amount.toFixed(2).toString().padStart(20, " ");
        displayText(customerMessage);

        deferred.resolve();
		return deferred.promise;
	});

ConfigurationManager.addHandler(['ModelEvent', 'ModelService', 'NotificationService', '$http', 'DocumentPersistedData', '$rootScope', '$uibModal',
	function (ModelEvent, ModelService, NotificationService, $http, DocumentPersistedData, $rootScope, $uibModalInstance) {
		let handleBeforeItemInsert = function ($q, item) {
			let deferred = $q.defer();

            let price = item.price;
            let desc = item.item_description2;

		    let customerMessage = desc.padEnd(20, " ") + price.toFixed(2).toString().padStart(20, " ")
            displayText(customerMessage);

            deferred.resolve();
			return deferred.promise;
		};
		let listener = ModelEvent.addListener('item', ['onAfterInsert'], handleBeforeItemInsert);
	}
]);
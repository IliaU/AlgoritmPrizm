function legalPersonSale ($uibModal, deferred) {
	let modalOptions = {
		backdrop: 'static',
		keyboard: false,
		windowClass: 'full',
		templateUrl: '/customizations/views/legal-modal.html',
		controller: ['$scope', '$http', '$uibModal', '$uibModalStack', '$window', 'ModelService', 'NotificationService',
			async function ($scope, $http, $uibModal, $uibModalStack, $window, ModelService, NotificationService) {
				'use strict';
				$scope.criteria = {};
				$scope.message = '';
				$scope.caption = '';
				$scope.legalAvailable = false;

				// check total to be lower then 100k
				let activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal"})[0];
				let body;

				try {
					body = JSON.stringify(activeDocument);
				} catch(e) {
					NotificationService.addAlert(e.message, `JS Error ${e.name}`);
				}

				let subtotal = activeDocument.sale_subtotal;
				if (subtotal >= 100000) {
					$scope.legalAvailable = false;
					$scope.message = "Продажа юр. лицу более чем на 100 т.р. запрешена";

					activeDocument.comment1 = "physical;;";
					activeDocument.save();
				}

				// save legal person information
				$scope.save = function () {
					let inn = $scope.criteria["inn"];
					let procurationNumber = $scope.criteria["procurationNumber"];

					$scope.legalAvailable = true;

					activeDocument.comment1 = `legal;${inn};${procurationNumber}`;
					activeDocument.save();

					$scope.close();
					deferred.resolve();
				}

				// close modal
				$scope.close = function () {
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

ButtonHooksManager.addHandler(['before_posTransactionTenderTransaction'],
	function ($q, $uibModal) {
		let deferred = $q.defer();

		if (CONFIG.autoLegal) {
			legalPersonSale($uibModal, deferred);
		}

		return deferred.promise;
});




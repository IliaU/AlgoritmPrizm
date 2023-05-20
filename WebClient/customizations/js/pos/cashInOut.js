function cashManipulation ($uibModal, type, deferred) {
	let modalOptions = {
		backdrop: 'static',
		keyboard: false,
        centered: true,
		windowClass: 'modal',
		templateUrl: '/customizations/views/cash-in-out-modal.html',
		controller: ['$scope', '$http', '$uibModal', '$uibModalStack', '$window', 'ModelService', 'NotificationService',
			async function ($scope, $http, $uibModal, $uibModalStack, $window, ModelService, NotificationService) {
				'use strict';
				// modal parameters
				$scope.criteria = {};
                $scope.infoMessage = ""
				$scope.manipulationAvailable = true;
                $scope.modalTitle = (type === "in" ? "Внесение" : "Изъятие");
                $scope.submitTitle = (type === "in" ? "Внести" : "Изъять");

				// NotificationService.addAlert(e.message, `JS Error ${e.name}`);
							
				// save 
				$scope.save = function () {
                    //  disable button
                    $scope.manipulationAvailable = false;

                    let url = UTIL_HOSTNAME + (type === "in" ? "/CashIncome" : "/CashOutcome");
                    let data = [{
                        valueDecimal: parseFloat($scope.criteria.amount)
                    }];

                    console.log(url, data);

                    $http({
                        method: 'POST',
                        url: url,
                        headers: {
                            'Content-Type': 'application/json, version=2',
                            'Accept': 'application/json, version=2'
                        },
                        data: JSON.stringify(data)
                    }).then(function successCallback(response) {
                        $scope.infoMessage = `${$scope.modalTitle} сохранено.`;
                    }, function errorCallback(response) {
						var str = response.data || 'Unknown Error!'
						NotificationService.addAlert(response.data, 'Server Error');
						deferred.resolve();
					});
				}

				// close modal
				$scope.cancel = function () {
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

prismApp.controller('cashInOutCntrl', ['$scope', '$q', '$uibModal', '$http', 'NotificationService',
    function ($scope, $q, $uibModal) {
        let deferred = $q.defer();

        console.log('openShiftController Loaded')
        $scope.cashIn = function () {
            console.log("Cash in");
            cashManipulation($uibModal, "in", deferred);
        };

        $scope.cashOut = function () {
            console.log("Cash out");
            cashManipulation($uibModal, "out", deferred);
        }
    }]);
// checks bonus account value and suggests to use them
function suggestBonus ($uibModal, deferred, activeDocument, bonusAmount) {
    let modalOptions = {
        backdrop: 'static',
        keyboard: false,
        windowClass: 'modal',
        templateUrl: '/customizations/views/bonus.html',
        controller: ['$scope', '$http', '$uibModal', '$uibModalStack', '$window', 'ModelService', 'NotificationService',
            async function ($scope, $http, $uibModal, $uibModalStack, $window, ModelService, NotificationService) {
                'use strict';
                // modal parameters
                $scope.criteria = {};
                $scope.message = '';
                $scope.caption = '';
                $scope.bonusAmount = bonusAmount;
                $scope.infoMessage = '';

                
                // spend bonus
                $scope.save = function () {
                    let spendAmount = parseFloat($scope.criteria.amount);

                    spendBonus(ModelService, activeDocument, spendAmount);

                    $uibModalStack.dismissAll();
                    deferred.resolve();
                }

                // accumulate bonus
                $scope.cancel = function () {
                    // activeDocument.comment1 = "physical;;";
                    // activeDocument.save();

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

function getCustomerInfo (ModelService, activeDocument) {
    let sid = activeDocument.sid
    let customer_uid = activeDocument.bt_cuid;

    if (customer_uid === "") return null;
    return {
        sid: sid,
        customerId: customer_uid
    };
}

function getBonusAmount ($http, customerID) {
    $http({
        method: 'GET',
        url: `${UTIL_HOSTNAME}/CustomerBonus`,
        headers: {
            'Content-Type': 'application/json, version=2',
            'Accept': 'application/json, version=2'
        },
        data: JSON.stringify({ "CustomerBonus": customerID })
    }).then(function successCallback(response) {
            let data = response.data;
            return data.amount;
    }, function errorCallback(response) {
        var str = response.data || 'Unknown Error!'
        NotificationService.addAlert(response.data, 'Server Error');
        return null;
    });
}

function spendBonus (ModelService, activeDocument, spendAmount) {
    activeDocument.manual_disc_type = 2;
    activeDocument.manual_disc_value = spendAmount;
    activeDocument.manual_disc_reason = "BONUS";

    activeDocument.save();
}

ButtonHooksManager.addHandler(['before_posTransactionTenderTransaction'],
    function ($q, $uibModal, $http, ModelService) {
        let deferred = $q.defer();

        let activeDocument;
        try {
            activeDocument = ModelService.fromCache('Document', {cols: "sale_subtotal,bt_title,bt_cuid,sid,tenders"})[0];
        } catch(e) {
            return null;
        };

        let customerInfo = getCustomerInfo(ModelService, activeDocument);
        if (!customerInfo) {
            deferred.resolve();
            return deferred.promise;
        }

        // let bonusAmount = getBonusAmount($http, customerID);
        let bonusAmount = 50000;
        if (!bonusAmount) {
            deferred.resolve();
            return deferred.promise;
        }

        let spendAmount = suggestBonus($uibModal, deferred, activeDocument, bonusAmount);
        // if (!spendAmount) {
        //     deferred.resolve();
        //     return deferred.promise;
        // }
       
        return deferred.promise;
    });
prismApp.controller('depositRefundCntrl', ['$scope', '$q', '$http', 'NotificationService', 'ModelService',
    function ($scope, $q, $http,  NotificationService, ModelService) {
        console.log('Deposit refund controller loaded.');

        $scope.printDocument = function() {
            let deferred = $q.defer();

            let activeDocument = ModelService.fromCache('Document')[0];
            let body;

            try {
                body = JSON.stringify(activeDocument);
                console.log(body);
            } catch(e) {
                NotificationService.addAlert(e.message, `JS Error ${e.name}`);
            }

            $http({
                method: 'POST',
                url:    `${UTIL_HOSTNAME}/printfiscdoc`,
                headers: {
                    "Content-Type": "application/json",
                    "Access-Control-Allow-Origin": "*"
                },
                data: body
            }).then(function successCallback(response) {
                deferred.resolve();
            }, function errorCallback(response) {
                var str = response.data || "Error during fiscal preprocessing!"
                NotificationService.addAlert(str,'Fiscal Printer error');
                deferred.reject();
            });

            return deferred.promise;
        }
    }]);

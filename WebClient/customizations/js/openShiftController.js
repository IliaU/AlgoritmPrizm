prismApp.controller('openShiftCntrl', ['$scope', '$q', '$http', 'NotificationService',
    function ($scope, $q, $http,  NotificationService) {
        console.log('openShiftController Loaded')
        $scope.openShift = function() {

            var deferred = $q.defer();
            $http({
             method: 'POST',
             url:    'http://chudakov:5000/openshift',
             headers: {
                 "Content-Type": "application/json",
				 "Access-Control-Allow-Origin": "*"
             },
            }).then(function successCallback(response) {
                deferred.resolve();
             }, function errorCallback(response) {
                 var str = response.data || "Backend server is not running!"
                 NotificationService.addAlert(str,'Fiscal Printer error');
                 deferred.reject();
            });
            return deferred.promise;
        }
    }]);

ButtonHooksManager.addHandler(['before_navLandingXOut'],
    function($q, $http, DocumentPersistedData, NotificationService, $uibModal, Templates, ModelService,$rootScope,HookEvent){
        console.log('xReportController Loaded')
        var deferred = $q.defer();
        $http({
         method: 'POST',
         url:    UTIL_HOSTNAME + '/xreport',
         headers: {
         	"Content-Type": "application/json",
         	"Access-Control-Allow-Origin": "*"
         },
        }).then(function successCallback(response) {
        	deferred.resolve();
         }, function errorCallback(response) {
            var str = response.data || "Error during forming X Report!"
            NotificationService.addAlert(str,'Fiscal Printer error');
         	deferred.reject();
        });

        return deferred.promise;
    }
);
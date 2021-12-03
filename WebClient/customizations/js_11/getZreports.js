ButtonHooksManager.addHandler(['before_navLandingZOut'],
    function($q, $http, DocumentPersistedData, NotificationService, $uibModal, Templates, ModelService,$rootScope,HookEvent){
    	var deferred = $q.defer();
        $http({
         method: 'POST',
         url:    'http://chudakov:5000/zreport',
         headers: {
         	"Content-Type": "application/json",
         	"Access-Control-Allow-Origin": "*"
         },
         data: '' 
        }).then(function successCallback(response) {
        	deferred.resolve();
         }, function errorCallback(response) {
            var str = response.data || "Error during forming Z Report!"
            NotificationService.addAlert(str,'Fiscal Printer error');
         	deferred.reject();
        });

        return deferred.promise;
    }
);
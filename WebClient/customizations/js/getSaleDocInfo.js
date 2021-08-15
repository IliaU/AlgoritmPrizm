ButtonHooksManager.addHandler(['before_navPosTenderPrintUpdate'],
    function($q, $http, DocumentPersistedData, NotificationService, $uibModal, Templates, ModelService,$rootScope,HookEvent){
        console.log('saleDocController Loaded')
    	var deferred = $q.defer();
        activeDocument = ModelService.fromCache('Document')[0];
        
        let body;
        try {
            body = JSON.stringify(activeDocument);
        } catch(e) {
            NotificationService.addAlert(e.message, `JS Error ${e.name}`);
        }

        $http({
         method: 'POST',
         url:    'http://chudakov:5000/sale',
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
);

ButtonHooksManager.addHandler(['after_navPosTenderPrintUpdate'],
    function($q, $http, DocumentPersistedData, NotificationService, $uibModal, Templates, ModelService,$rootScope,HookEvent){
        console.log('saleDocController Loaded')
    	var deferred = $q.defer();
        activeDocument = ModelService.fromCache('Document')[0];

        let body;
        try {
            body = JSON.stringify(activeDocument);
        } catch(e) {
            NotificationService.addAlert(e.message, `JS Error ${e.name}`);
        }

        $http({
         method: 'POST',
         url:    'http://chudakov:5000/printfiscdoc',
         headers: {
         	"Content-Type": "application/json",
         	"Access-Control-Allow-Origin": "*"
         },
         data: body
        }).then(function successCallback(response) {
        	deferred.resolve();
         }, function errorCallback(response) {
            // Delete last active Document
            $http.delete(activeDocument.link)
            var str = response.data || "Error during fiscal postprocessing!"
            NotificationService.addAlert(str,'Fiscal Printer error');
         	deferred.reject();
        });

        return deferred.promise;
    }
);

// Duplicate functionality of navPosTenderPrintUpdate!
ButtonHooksManager.addHandler(['before_navPosTenderUpdateOnly'],
    function($q, $http, DocumentPersistedData, NotificationService, $uibModal, Templates, ModelService,$rootScope,HookEvent){
        console.log('saleDocController Loaded')
    	var deferred = $q.defer();
        activeDocument = ModelService.fromCache('Document')[0];
       
        let body;
        try {
            body = JSON.stringify(activeDocument);
        } catch(e) {
            NotificationService.addAlert(e.message, `JS Error ${e.name}`);
        }

        $http({
         method: 'POST',
         url:    'http://chudakov:5000/sale',
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
);

ButtonHooksManager.addHandler(['after_navPosTenderUpdateOnly'],
    function($q, $http, DocumentPersistedData, NotificationService, $uibModal, Templates, ModelService,$rootScope,HookEvent){
        console.log('saleDocController Loaded')
        
        var deferred = $q.defer();
        activeDocument = ModelService.fromCache('Document')[0];

        let body;
        try {
            body = JSON.stringify(activeDocument);
        } catch(e) {
            NotificationService.addAlert(e.message, `JS Error ${e.name}`);
        }

        $http({
         method: 'POST',
         url:    'http://chudakov:5000/printfiscdoc',
         headers: {
         	"Content-Type": "application/json",
         	"Access-Control-Allow-Origin": "*"
         },
         data: body
        }).then(function successCallback(response) {
        	deferred.resolve();
         }, function errorCallback(response) {
            // Delete last active Document
            $http.delete(activeDocument.link)
            var str = response.data || "Error during fiscal postprocessing!"
            NotificationService.addAlert(str,'Fiscal Printer error');
         	deferred.reject();
        });

        return deferred.promise;
    }
);
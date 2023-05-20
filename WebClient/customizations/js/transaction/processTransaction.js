async function printDocument($q, $http, NotificationService, ModelService, fiscal = true) {
    var deferred = $q.defer();
    activeDocument = ModelService.fromCache('Document', {'cols': '*'})[0];

    let body;
    try {
        body = JSON.stringify(activeDocument);
    } catch(e) {
        NotificationService.addAlert(e.message, `JS Error ${e.name}`);
    }

    let URL = UTIL_HOSTNAME;
    if (fiscal) {
        URL += "/printfiscdoc";
    } else {
        URL += "/printdoccopy";
    }

    $http({
     method: 'POST',
     url:    URL,
     headers: {
         "Content-Type": "application/json",
         "Access-Control-Allow-Origin": "*"
     },
     data: body
    }).then(function successCallback(response) {
        console.log(response.data);

        if (fiscal) {
            let fiscalData = response.data;
            if (fiscalData.fiscDocNum !== undefined) {
                if (fiscalData.fieldName.toLowerCase() === "comment1") {
                    activeDocument.comment1 = fiscalData.fiscDocNum.toString();
                    activeDocument.save();
                } else if (fiscalData.fieldName.toLowerCase() === "comment2") {
                    activeDocument.comment2 = fiscalData.fiscDocNum.toString();
                    activeDocument.save();
                } else {
                    NotificationService.addAlert('Invalid fiscal document number string','Fiscal Printer error');
                    deferred.reject();
                }
            } else {
                NotificationService.addAlert('Invalid fiscal document number','Fiscal Printer error');
                deferred.reject();
            }
        }

        deferred.resolve();
     }, function errorCallback(response) {
        // kill the doc
        $http.delete(activeDocument.link)

        var str = response.data || "Error during fiscal postprocessing!"
        NotificationService.addAlert(str,'Fiscal Printer error');

        deferred.reject();
    });

    return deferred.promise;
}

ButtonHooksManager.addHandler(
    ['after_navPosTenderPrintUpdate', 'after_navPosTenderUpdateOnly'], 
    function ($q, $http, NotificationService, ModelService) {
        printDocument($q, $http, NotificationService, ModelService);
    }
);

prismApp.controller(
    'printCopyController', 
    ['$scope', '$q', '$http', 'NotificationService', 'ModelService', 
        function ($scope, $q, $http, NotificationService, ModelService) {
            $scope.printDocumentCopy = function () {
                console.log("printing document copy");
                printDocument($q, $http, NotificationService, ModelService, fiscal = false);
            }
        }
    ]
);

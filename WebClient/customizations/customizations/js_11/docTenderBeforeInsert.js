var docTenderBeforeInsertHandler = [
    'ModelEvent',
    'ModelService',
    'NotificationService',
    '$http',
    'DocumentPersistedData',
    '$rootScope',
    '$uibModal',
    function (ModelEvent, ModelService, NotificationService, $http, DocumentPersistedData, $rootScope, $uibModalInstance) {
        let handleBeforeTenderInsert = function ($q, tender) {
            if (tender.tender_type === 5) {
                console.log("Списание с бонусного счета");

                let successCallback = function (response) {
                    console.log(response);
                };

                let failureCallback = function (error) {
                    NotificationService.addAlert(`Причина: ${error}`, "Использование бонусного счета недоступно");
                    deferred.reject();
                };

                // load the document to get client's phone number
                let activeDocument = ModelService.fromCache('Document', {cols: "bt_primary_phone_no"})[0];
                console.log(activeDocument.bt_primary_phone_no);

                let code = Math.floor(1000 + Math.random() * 9000).toString();

                $http({
                    method: 'POST',
                    url: `${UTIL_HOSTNAME}/smsgateway`,
                    headers: {
                        "Content-Type": "application/json",
                        "Access-Control-Allow-Origin": "*"
                    },
                    data: {
                        phone: activeDocument.bt_primary_phone_no,
                        text: "test"
                    },
                }).then(successCallback, failureCallback);

            } else {
                var deferred = $q.defer();

                console.log(tender);
                deferred.resolve();
            }

            // reload the page to display tender correctly
            // document.location.reload();
        };
        let listener = ModelEvent.addListener('tender', ['onAfterInsert'], handleBeforeTenderInsert);
    }
]

ConfigurationManager.addHandler(docTenderBeforeInsertHandler);




var DocItemBeforeInsertHandler = ['ModelEvent', 'ModelService', 'NotificationService', '$http', 'DocumentPersistedData', '$rootScope', '$uibModal',
    function (ModelEvent, ModelService, NotificationService, $http, DocumentPersistedData, $rootScope, $uibModalInstance) 
	{
		var handleBeforeItemInsert = function ($q, item) 
		{
            console.log('markingController Loaded')
            var deferred = $q.defer();            
            
            let body;
            try {
                body = JSON.stringify(item);
            } catch(e) {
                NotificationService.addAlert(e.message, `JS Error ${e.name}`);
            }
			
			
			
			
			
			
			
			$http({
                method: 'POST',
                url:    'http://chudakov:5000/marking',
                headers: {
					"Content-Type": "application/json",
                    "Access-Control-Allow-Origin": "*"
                },
                data: body
            }).then(function successCallback(response) {
					console.log(response.data);
						
						
						
						
				
						
					if (response.data.scan_marking) 
					{
						if (response.data.marking_list_full) 
						{
							NotificationService.addAlert('Marking List is full!', 'Marking Error');
							if (item.quantity === 1) {
								$http.delete(`/v1/rest/document/${item.document_sid}/item/${item.sid}`)
							} else {
								item.quantity--;
								item.save();
							}
							deferred.resolve();
						} else {
							// Modal Window to scan marking
							var modalOptions = {
								backdrop: 'static',
								keyboard: false,
								windowClass: 'full',
								templateUrl: '/customizations/views/modal.html',
								controller: ['$scope', '$http', '$uibModalInstance', '$window', 'ModelService',

									function ($scope, $http, $uibModalInstance, $window, ModelService) {
										'use strict';
										$scope.criteria = {};
										$scope.message = '';
										$scope.caption = '';
										$scope.mandatory = response.data.Mandatory;

										$scope.search = function () {

											var marking = $scope.criteria.enterMarking;

											for (let i = 1; i <=11; i++) {
												if (!item[`note${i}`].length) {
													item[`note${i}`] = marking.replace('"','\"');
													break;
												}
											}
											item.save();
											$uibModalInstance.close();
										}

										$scope.close = function () {
											console.log('Close button controller!')
											
											if (item.quantity === 1) {
												$http.delete(`/v1/rest/document/${item.document_sid}/item/${item.sid}`)
											} else {
												item.quantity--;
												item.save();
											}

											$uibModalInstance.close();   
										}
									}
								]
							};

							var modalInstance = $uibModalInstance.open(modalOptions);
							modalInstance.result.then(function () {
								deferred.resolve();
							});
						}

					} else {
						deferred.resolve();
					}
						
						
						
						
						
						
						
						
						
						
						
						
				}, function errorCallback(response) {
						var str = response.data || 'Unknown Error!'
						NotificationService.addAlert(response.data, 'Server Error');
						if (item.quantity == 1) {
							$http.delete(`/v1/rest/document/${item.document_sid}/item/${item.sid}`)
						} else {
							item.quantity--;
							item.save();
						}
						deferred.resolve();
					}
				);
			
			
			
			
			return deferred.promise;
		};
        var listener = ModelEvent.addListener('item', ['onAfterInsert'], handleBeforeItemInsert);
	}
]

ConfigurationManager.addHandler(DocItemBeforeInsertHandler);




var DocItemBeforeInsertHandler = ['ModelEvent', 'LoadingScreen', 'NotificationService', '$http', 'DocumentPersistedData', '$rootScope', '$uibModal', '$state',
    function (ModelEvent, LoadingScreen, NotificationService, $http, DocumentPersistedData, $rootScope, $uibModalInstance, $state) 
	{
		var handleBeforeItemInsert = function ($q, item) 
		{
            console.log('markingController Loaded');
			console.log(item);

            var deferred = $q.defer();

			// disable loader
			LoadingScreen.Enable = false;
            
            let body;
            try {
                body = JSON.stringify(item);
            } catch(e) {
                NotificationService.addAlert(e.message, `JS Error ${e.name}`);
            }
			$http({
                method: 'POST',
                url:    `${UTIL_HOSTNAME}/marking`,
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
								windowClass: 'modal',
								templateUrl: '/customizations/views/modal.html',
								controller: ['$scope', '$http', '$uibModalInstance', '$window', 'ModelService', 'NotificationService',

									function ($scope, $http, $uibModalInstance, $window, ModelService, NotificationService) {
										'use strict';
										$scope.criteria = {};
										$scope.message = '';
										$scope.caption = '';
										$scope.mandatory = response.data.Mandatory;

										$scope.search = function () {
											let marking = $scope.criteria.enterMarking;
											ModelService.get('Item', { sid: item.sid, document_sid: item.document_sid, cols: '*' }).then(function (items) {
												let currentItem = items[0];
												for (let i = 1; i <=11; i++) {
													console.log(currentItem.note1);
													if (!currentItem[`note${i}`].length) {
														currentItem[`note${i}`] = marking.replace('"','\"');
														break;
													}
												}
												currentItem.save().then(function () {
													$state.go($state.current, {}, {reload: true});
													deferred.resolve();
													$uibModalInstance.close();
												});
											});
										}

										$scope.close = function () {
											console.log('Close button controller!')
											
											// if (item.quantity === 1) {
											// 	$http.delete(`/v1/rest/document/${item.document_sid}/item/${item.sid}`)
											// } else {
											// 	item.quantity--;
											// 	item.save();
											// }

											NotificationService.addConfirm('Вы уверены, что хотите отменить маркировку?', 'Отмена маркировки').then(
												function successCallback(closeMarking) {
													if (closeMarking) {
														$uibModalInstance.close();
													}
												}
											);
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
						if (item.quantity === 1) {
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




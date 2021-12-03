var session;

prismApp.controller('loadReceivingCntrl', ['$scope', '$q', '$http', 'NotificationService',
    function ($scope, $q, $http,  NotificationService) {
        $scope.loadFile = function(input) {
            var deferred = $q.defer();
			let file = input.files[0];
			let reader = new FileReader();
			reader.readAsText(file);
			
			console.log("loading file");
		
			reader.onload = async function() {
				session = JSON.parse(sessionStorage.getItem("session"));
				let href = window.location.href.split("/");
				let voucherSID = href[href.findIndex((element) => element === "new") + 1];
		
				let dataFile = reader.result;
				let itemStrings = dataFile.split("\r\n");
		
				let invItems = [];
		
				for (const item of itemStrings) {
					let [UPC, quantity] = item.split("~");
					if (UPC && quantity) {
						let invItem = await getInvItem(UPC, parseInt(quantity), voucherSID);
						invItems.push(invItem);
					}
				}
		
				await addInvItems(invItems, voucherSID);
		
				reloadPage();
			};
		
			reader.onerror = function() {
				console.log(reader.error);
			};
			

            return deferred.promise;
        }

    }]);

async function getSIDByUPC (targetUPC, cols=[]) {
	let url = HOSTNAME + "/v1/rest/inventory?cols=sid,upc," + cols.join() + "&filter=upc,eq," + targetUPC;
	console.log(url);
	let response = await fetch(url, {
		method: "GET",
		headers: {
			'Auth-Session': session.token,
		}
	});
	let result = await response.json();
	if (result.length == 0){
		alert('Товара с таким UPC не существует')
	} else{
		alert('OK')
	}
	return result[0]["sid"];
}

async function getInvItem (UPC, quantity, voucherSID) {
	let itemSID = await getSIDByUPC(UPC);

	let invItem = {
		originApplication: "RProPrismWeb",
		UPC: UPC,
		qty: quantity,
		itemSID: itemSID,
		vouSID: voucherSID,
	};

	console.log(UPC);

	return invItem;
}

async function addInvItems (items, voucherSID) {
	let response = await fetch(HOSTNAME + "/api/backoffice/receiving/" + voucherSID + "/recvitem",
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json, version=2',
				'Accept': 'application/json, version=2',
				'Accept-Encoding': 'gzip, deflate, br',
				'Auth-Session': session.token
			},
			body: JSON.stringify({ "data": items })
		});
	let result = await response.json();
	
	prismApp.controller('myController', ['$addInvItems', 'NotificationService',
	function($addInvItems, NotificationService){
		NotificationService.addAlert('Alert Text');
		if (result.length == 0){
			alertText('Товара с таким UPC не существует')
		} else{
			alertText('OK')
		}
		
   }]);
   
	console.log(result);
}

function reloadPage() {
	document.location.reload();
}
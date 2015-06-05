angular.module('abioka.controllers', [])

.controller('MainCtrl', function($scope, entryRouting, PageService) {
	$scope.entryRouting = entryRouting;
	
    $scope.changePage = function(forward)  {
		PageService.changePage(forward);
    };
})

.controller('EntryIndexCtrl', function($scope, $ionicLoading, EksiSozlukService) {
	EksiSozlukService.all(function(data) {
		$scope.entries = data;
		$ionicLoading.hide();
	});
})

.controller('EntryDetailCtrl', function($scope, $stateParams, $ionicPopup, EksiSozlukService, entryRouting, PageService) {	
	var entryId = parseInt($stateParams.entryId);
	
	entryRouting.ShowPrevious = entryId > 1;
	entryRouting.ShowNext = entryId < 50;
	entryRouting.EntryId = entryId;
	
	$scope.Title = entryId;
	
    EksiSozlukService.get(entryId, true, function(data) {
		$scope.entry = data;
		$scope.loaded = true;

	    var newId = entryId + 1;
		if(newId < 50){
			setTimeout(function(){
				EksiSozlukService.get(newId, false, function(data) {});
			}, 1000);
		}
	});
    
    $scope.reportEvent = function(event)  {
		var forward = event === "swipeleft";
		PageService.changePage(forward);
    };
	
	
    $scope.socialShare = function () {
    	var message = $scope.entry.Url + "\nvia ekşi debe";
    	window.plugins.socialsharing.share(message);
    };
	
	
	var key = "isEksiDebeOpenedBefore";
	var isOpenedBefore = localStorage.getItem(key);
	if(isOpenedBefore != "true"){
		console.log("ilk açılış");
		$ionicPopup.alert({
			title: '<i class="icon ion-happy-outline"></i> ipucu',
			template: "sayfayı sağa ve sola kaydırarak entry'ler arasında gezinebilirsin.",
			okText: 'bakayım'
		}).then(function(res) {
			localStorage.setItem(key, 'true');
		});
	}
});

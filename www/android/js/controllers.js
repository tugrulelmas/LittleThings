angular.module('abioka.controllers', [])

.controller('MainCtrl', function($scope, entryRouting) {
	$scope.entryRouting = entryRouting;
})

.controller('EntryIndexCtrl', function($scope, $ionicLoading, EksiSozlukService) {
	EksiSozlukService.all(function(data) {
		$scope.entries = data;
		$ionicLoading.hide();
	});
})

.controller('EntryDetailCtrl', function($scope, $stateParams, $ionicHistory, $ionicViewSwitcher, $state, EksiSozlukService, entryRouting) {
	var entryId = parseInt($stateParams.entryId);
	var previousUrl = "/tab/entry/" + (entryId - 1);
	var nextUrl = "/tab/entry/" + (entryId + 1);
	
	entryRouting.PreviousUrl = previousUrl;
	entryRouting.NextUrl = nextUrl;
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
		$ionicHistory.clearHistory();
        if(event === "swipeleft" && entryId < 50){
			$ionicViewSwitcher.nextDirection('forward');
			$state.go("tab.entry-detail", {entryId: entryId + 1});
        } else if(event === "swiperight" && entryId > 1){
			$ionicViewSwitcher.nextDirection('back');
			$state.go("tab.entry-detail", {entryId: entryId - 1});
        }
    };
	
	
    $scope.socialShare = function () {
    	var message = $scope.entry.Url + "\nvia ekşi debe";
    	window.plugins.socialsharing.share(message);
    };
});

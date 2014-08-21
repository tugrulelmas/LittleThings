angular.module('abioka.controllers', [])

.controller('EntryIndexCtrl', function($scope, $ionicLoading, EksiSozlukService) {
	EksiSozlukService.all(function(data) {
		$scope.entries = data;
		$ionicLoading.hide();
	});
})

.controller('EntryDetailCtrl', function($scope, $rootScope, $stateParams, $ionicLoading, $location, $ionicViewService, EksiSozlukService) {
    EksiSozlukService.get($stateParams.entryId, function(data) {
		$scope.entry = data;
		if($scope.entry.Sorting == 1){
			$rootScope.ShowPrevious = false;
		} else {
			$rootScope.ShowPrevious = true;
			$rootScope.PreviousUrl = "/tab/entry/" + ($scope.entry.Sorting - 1);
		}
		
		if($scope.entry.Sorting == 50){
			$rootScope.ShowNext = false;
		} else {
			$rootScope.ShowNext = true;
			$rootScope.NextUrl = "/tab/entry/" + ($scope.entry.Sorting + 1);
		}
		$scope.loaded = true;
		$ionicLoading.hide();
	});
    
    $scope.reportEvent = function(event)  {
		$ionicViewService.clearHistory();
        if(event === "swipeleft" && $rootScope.ShowNext){
        	$location.path($rootScope.NextUrl);
			$scope.$apply();
        } else if(event === "swiperight" && $rootScope.ShowPrevious){
        	$location.path($rootScope.PreviousUrl);
			$scope.$apply();
        }
    };
});

angular.module('abioka', ['ionic', 'abioka.services', 'abioka.controllers'])
.value('entryRouting', {
	"ShowPrevious": false,
	"ShowNext": false,
	"PreviousUrl": "",
	"NextUrl": "",
	"EntryId": 0
})
.directive('dynamic', function ($compile) {
  return {
    restrict: 'A',
    replace: true,
    link: function (scope, ele, attrs) {
      scope.$watch(attrs.dynamic, function(html) {
        ele.html(html);
        $compile(ele.contents())(scope);
      });
    }
  };
})

.directive('a', function() {
    return {
        restrict: 'E',
        link: function(scope, elem, attrs) {
			if (attrs.target === "_blank") {
				elem.on('click', function(e){
                    e.preventDefault();
					navigator.app.loadUrl(attrs.href, { openExternal:true });
                });
			}
        }
   };
})

.directive('detectgestures', function($ionicGesture) {
  return {
    restrict :  'A',
    link : function(scope, elem, attrs) {
      $ionicGesture.on('swipeleft', function(event){     
		  scope.reportEvent('swipeleft');
      }, elem);
      
      $ionicGesture.on('swiperight', function(event){     
    	  scope.reportEvent('swiperight');
      }, elem);
    }
  };
})

.config(function($stateProvider, $urlRouterProvider, $httpProvider, $ionicConfigProvider) {
	$httpProvider.defaults.timeout = 5000;
	$ionicConfigProvider.tabs.position('bottom');
	$ionicConfigProvider.navBar.alignTitle('center');
	$ionicConfigProvider.scrolling.jsScrolling(false);
	
  $stateProvider
    .state('tab', {
      url: '/tab',
      abstract: true,
      templateUrl: 'templates/tabs.html'
    })
    .state('tab.entry-index', {
      url: '/entries',
      views: {
        'entries-tab': {
          templateUrl: 'templates/entry-index.html',
          controller: 'EntryIndexCtrl'
        }
      }
    })

    .state('tab.entry-detail', {
      url: '/entry/:entryId',
      views: {
        'entry-tab': {
          templateUrl: 'templates/entry-detail.html',
          controller: 'EntryDetailCtrl'
        }
      },
	  onExit: function($rootScope){
			$rootScope.ShowPrevious = false;
			$rootScope.ShowNext = false;
		}
    })

    .state('tab.about', {
      url: '/about',
      views: {
        'about-tab': {
          templateUrl: 'templates/about.html'
        }
      }
    });

  $urlRouterProvider.otherwise('/tab/entries');
  
});


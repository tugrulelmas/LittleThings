angular.module('abioka.services', [])

.factory('EksiSozlukService', function($http, $ionicLoading, $ionicPopup) {
	var apiUrl = "http://littlethingsapi.abioka.com/api/EksiSozluk";
	
	function showLoading(){
		$ionicLoading.show({
		    content: 'yükleniyor...',
		    animation: 'fade-in',
		    showBackdrop: false,
		    maxWidth: 200,
		    showDelay: 0
		});
	};
	
	function showAlert() {
		var alertPopup = $ionicPopup.alert({
			title: 'bir takım problemler mevcut!',
			template: 'ekşi sözlük\'ün html yapısı değişmiş olabilir ya da bu uygulama sıçtı. bir ihtimal, biraz sonra tekrar deneyince düzelecektir.',
			okText: 'düşüneyim'
		});
		alertPopup.then(function(res) {
		});
	};

  return {
    all: function(callback) {
		showLoading();
		$http.get(apiUrl)
		.success(callback)
		.error(function(data, status, headers, config) {
			$ionicLoading.hide();
			showAlert();
		});
    },
    get: function(entryId, callback) {
		showLoading();
		$http.get(apiUrl + "/" + entryId)
		.success(callback)
		.error(function(data, status, headers, config) {
			$ionicLoading.hide();
			showAlert();
		});
    }
  }
});

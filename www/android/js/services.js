angular.module('abioka.services', [])

.factory('EksiSozlukService', function($http, $ionicLoading, $ionicPopup) {
	var apiUrl = "http://littlethingsapi.abioka.com/api/EksiSozluk";//"http://10.0.2.2/AbiokaLittleThingsApi/api/EksiSozluk";
	var entries = [];
	
	function showLoading(){
		$ionicLoading.show({
			template: '<i class="icon ion-load-d"></i>\n<br/>\nyükleniyor...',
		    animation: 'fade-in',
		    noBackdrop: false,
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
	
	function getFromServer(entryId, showLoadingPanel, callback){
		$http.get(apiUrl + "/" + entryId)
		.success(function(data){
			updateCache(data);
			callback(data);
		})
		.error(function(data, status, headers, config) {
			if(showLoadingPanel){
				$ionicLoading.hide();
				showAlert();
			}
		});
	};
	
	function getFromCache(entryId){
		var result = {};
		
		if(entries == null || entries.length == 0){
			return null;
		};
		
		var found = false;
		for(var i = 0; i < entries.length; i++){
			var entry = entries[i];
			if(entry.Sorting === entryId){
				if(entry.Text != null){
					result = entry;
					found = true;
				}
				break;
			}
		};
		if(!found)
			return null;
		
		return result;
	};
	
	function getEntry(entryId, showLoadingPanel, callback){
		var entry = getFromCache(entryId);
		if(entry != null){
			callback(entry);
		} else {
			getFromServer(entryId, showLoadingPanel, callback);
		}
	}
	
	function updateCache(entry){
		if(entries == null || entries.length == 0){
			return;
		};
		for(var i = 0; i < entries.length; i++){
			if(entries[i].Sorting === entry.Sorting){
				entries[i] = entry;
				break;
			}
		};
	};
	
	function loadAllEntries(callback){
		$http.get(apiUrl)
		.success(function(data){
			entries = data;
			callback(entries);
		})
		.error(function(data, status, headers, config) {
			$ionicLoading.hide();
			showAlert();
		});
	}

  return {
    all: function(callback) {
		showLoading();
		if(entries != null && entries.length > 0){
			callback(entries);
			return;
		};
		
		loadAllEntries(callback);
    },
    get: function(entryId, showLoadingPanel, callback) {
		if(showLoadingPanel){
			//showLoading();
		}
		
		if(entries == null || entries.length == 0){
			loadAllEntries(function(){
				getEntry(entryId, showLoadingPanel, callback);
			})
		} else {
			getEntry(entryId, showLoadingPanel, callback);
		}
    }
  }
})

.factory('PageService', function($ionicHistory, $ionicViewSwitcher, $state, entryRouting) {
	return{
		changePage: function(forward)  {
			$ionicHistory.clearHistory();
			var entryId = entryRouting.EntryId;
			if(forward && entryId < 50){
				$ionicViewSwitcher.nextDirection('forward');
				entryRouting.EntryId = entryId + 1;
				$state.go("tab.entry-detail", {entryId: entryId + 1});
			} else if(!forward && entryId > 1){
				$ionicViewSwitcher.nextDirection('back');
				entryRouting.EntryId = entryId - 1;
				$state.go("tab.entry-detail", {entryId: entryId - 1});
			}
		}
	}
});

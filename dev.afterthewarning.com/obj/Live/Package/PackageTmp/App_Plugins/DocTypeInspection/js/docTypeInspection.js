angular.module("umbraco.resources").factory("doctypeInfoApiResource", function ($http) {

	var doctypeInfoApiResource = {};

	doctypeInfoApiResource.getViewModel = function (id) {
		return $http.get('/umbraco/backoffice/api/DocTypeApi/GetDocTypeInformation?id=' + id).then(function(response) {
			return response.data;
		});
	};

	return doctypeInfoApiResource;
});

angular.module('umbraco').controller('DocTypeInfo.ContextMenu.Controller',
['$scope', '$controller', 'doctypeInfoApiResource',
function ($scope, $controller, doctypeInfoApiResource) {

    /*--- Init functions ---*/
    $scope.init = function() {
        $scope.setVariables();
        $scope.getDocTypeById();
    };

    $scope.setVariables = function() {
        $scope.id = false;
        $scope.docType = {};
    };

    $scope.getDocTypeById = function() {
        $scope.$watch('id', function(newValue, oldValue) {
            if (newValue) {
                return doctypeInfoApiResource.getViewModel($scope.id).then(function (data) {
                    $scope.docType = data;
                    return true;
                });
            }
        }, true);
    };

    /*---- Init ----*/
    $scope.init();

}]);
angular.module('umbraco.directives')
  .config(function($provide) {
    $provide.decorator('umbContextMenuDirective', function($delegate, $controller) {
      var directive, link;
      directive = $delegate[0];

      directive.controller = function($scope, $timeout) {
        angular.extend(this, $controller('DocTypeInfo.ContextMenu.Controller', {$scope: $scope}));
      };

      var compile = directive.compile;
      directive.compile = function() {
        var link = compile.apply(this, arguments);
        return function(scope, elem, attrs) {
          link.apply(this, arguments);
          scope.$watch('currentNode', function(newValue, oldValue) {
                if (newValue) {
                  scope.id = newValue.id;
                }
            }, true);
        };
      };

        
      directive.templateUrl = '/App_Plugins/DocTypeInspection/Views/ContextMenuView.html';

      return $delegate;
    });
});
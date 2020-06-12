angular.module('umbraco.resources').factory('countryResource',
    function ($q, $http) {
        return {
            getAll: function () {
                return $http.get("MBranApi/Country/GetAll");
            }
        };
    }
); 
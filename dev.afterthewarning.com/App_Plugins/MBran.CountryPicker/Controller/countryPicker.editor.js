angular.module('umbraco')
    .controller('MBran.CountryPicker',
    function ($scope, countryResource) {

        countryResource.getAll().then(function (response) {
            console.log(response.data);
            $scope.countries = response.data;
        });
    });
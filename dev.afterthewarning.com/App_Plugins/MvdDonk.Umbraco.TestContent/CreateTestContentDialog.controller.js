angular.module("umbraco").controller("CreateTestContentDialog", function ($scope, $filter, $http, $routeParams, iconHelper, editorState, contentTypeResource, contentResource, notificationsService, navigationService, appState, $timeout, createTestContentResource, navigationService, dialogService, $location) {

    var vm = this;

    vm.page = {};
    vm.page.loading = false;

    vm.contentType = null;
    vm.currentNode = $scope.currentNode;
    vm.createContentForm = null;

    function initialize() {
        vm.page.loading = true;
        contentTypeResource.getAllowedTypes(vm.currentNode.id).then(function (data) {
            vm.allowedTypes = iconHelper.formatContentTypeIcons(data);
            vm.page.loading = false;
        });

        vm.selectContentType = true;
    }


    function createBlank(docType) {
        $location.path('/content/MvdDonk.Umbraco.TestContent/CreateTestContent/' + $scope.currentNode.id).search('doctype=' + docType.alias + '&create=true');
        navigationService.hideDialog();
    }


    vm.createBlank = createBlank;

    initialize();
});
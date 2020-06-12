function createTestContentResource($q, $http, umbRequestHelper) {

    return {
        getContentCreators: function (requestModel) {
            var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/TestContent/CreateTestContentApi/ContentCreators";
            return umbRequestHelper.resourcePromise(
                $http.get(url),
                "Failed to create content");
        },

        startContentCreation: function (requestModel) {
            var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/TestContent/CreateTestContentApi/Create";
            return umbRequestHelper.resourcePromise(
                $http.post(url, requestModel),
                "Failed to create content");
        }
    };
}
angular.module("umbraco.resources").factory("createTestContentResource", createTestContentResource);

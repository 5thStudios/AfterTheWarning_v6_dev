angular.module("umbraco").controller("CreateTestContentController", function ($scope, $filter, $http, $routeParams, $location, editorState, contentTypeResource, contentResource, notificationsService, navigationService, appState, $timeout, createTestContentResource, navigationService, dialogService, angularHelper) {

    var vm = this;
    vm.page = {};
    vm.page.loading = false;

    vm.showSelectDocTypeDialog = true;
    vm.showCreateContentDialog = false;
    vm.contentType = null;
    vm.createContentForm = null;
    vm.numberOfTestItems = 1;
    vm.nodeNameProperty = {
        value: null,
        generateType: null
    };
    vm.page.buttonGroupState = "init";
    vm.defaultButton = {
        labelKey: "testContent_generateAndPublishButton",
        handler: generateAndPublish,
        hotKey: "ctrl+p",
        hotKeyWhenHidden: true,
        alias: "generateAndPublish"
    };
    vm.subButtons = [
        {
            labelKey: "testContent_generateButton",
            handler: generate,
            hotKey: "ctrl+s",
            hotKeyWhenHidden: true,
            alias: "generate"
        }
    ];

    //TODO Grab locales from Bogus -> Database.GetAllLocales ??
    vm.languages = [
        {
            name: "English (US)",
            value: "en"
        },
        {
            name: "Dutch",
            value: "nl"
        },
        {
            name: "French",
            value: "fr"
        },
        {
            name: "German",
            value: "de"
        },
        {
            name: "Spanish",
            value: "es"
        },
    ];
    vm.language = vm.languages[0];

    function initialize() {
        vm.page.loading = true;

        createTestContentResource.getContentCreators().then(function (contentCreators) {
            vm.contentCreators = contentCreators;

            vm.contentCreators = _.sortBy(vm.contentCreators, function (contentCreator) {
                return contentCreator.sortOrder;
            });

            contentResource.getScaffold($routeParams.id, $routeParams.doctype).then(function (data) {
                vm.content = data;
                vm.content.nodeNameProperty = {
                    generateContent: false,
                    generators: getGeneratorsForProperty("Umbraco.Textbox"),
                    generateType: null
                }
                if (vm.content.nodeNameProperty.generators.length > 0) {
                    vm.content.nodeNameProperty.generateType = vm.content.nodeNameProperty.generators[0];
                }

                //Setup editors for properties
                for (var iTab = 0; iTab < vm.content.tabs.length; iTab++) {
                    var tab = vm.content.tabs[iTab];
                    for (var iProperty = 0; iProperty < tab.properties.length; iProperty++) {
                        var property = tab.properties[iProperty];

                        property.generateContent = false;
                        property.generators = getGeneratorsForProperty(property);
                        if (property.generators.length > 0) {
                            property.generateType = property.generators[0];
                        }
                    }
                }

                hackNodeNameGeneratorPosition();


                $scope.$watch(function () {
                    return vm.content.nodeNameProperty.generateContent
                }, function () {
                    var control = $scope.contentForm.headerNameForm.headerName;
                    if (control == undefined) {
                        return;
                    }

                    if (vm.content.nodeNameProperty.generateContent === true && vm.content.nodeNameProperty.generateType && vm.content.nodeNameProperty.generateType.id.length > 0) {
                        //Force it to be valid
                        control.$setValidity("required", true);
                    }
                    else {
                        //If there isn't a value, set it to something and then to empty, will retrigger all validations and so ignore the possibly forced valid
                        if (control.$viewValue == null || control.$viewValue.length == 0) {
                            control.$setViewValue('dummy');
                            control.$setViewValue('');
                        }
                    }
                });

                vm.page.loading = false;
            });
        });
    }

    function generateAndPublish() {
        createTestContent(true);
    };

    function generate() {
        createTestContent(false);
    };

    function createTestContent(publishNodes) {
        $scope.$broadcast('formSubmitting', {
            scope: $scope,
            action: "save"
        });

        if ($scope.contentForm.$invalid) {
            return;
        }

        vm.page.buttonGroupState = "busy";

        var requestModel = {
            parentId: $routeParams.id,
            contentTypeAlias: $routeParams.doctype,
            nodeNameProperty: {
                id: -1,
                alias: null,
                generateType: null,
                defaultValue: null
            },
            numberOfTestItems: vm.numberOfTestItems,
            language: vm.language.value,
            properties: [],
            publishNodes: publishNodes
        };

        switch (typeof (vm.content.name)) {
            case "object":
                break;
            default:
                if (vm.content.name != undefined) {
                    requestModel.nodeNameProperty.defaultValue = vm.content.name;
                }
                break;
        }

        if (vm.content.nodeNameProperty.generateContent === true && vm.content.nodeNameProperty.generateType && vm.content.nodeNameProperty.generateType.id.length > 0) {
            requestModel.nodeNameProperty.generateType = vm.content.nodeNameProperty.generateType.id;
        }


        for (var iTab = 0; iTab < vm.content.tabs.length; iTab++) {
            var tab = vm.content.tabs[iTab];
            for (var iProperty = 0; iProperty < tab.properties.length; iProperty++) {
                var property = tab.properties[iProperty];
                var propertyModel = {
                    id: property.id,
                    alias: property.alias,
                    generateType: null,
                    defaultValue: null
                };

                var selectedValueType = typeof (property.value);
                switch (selectedValueType) {
                    case "object":
                        break;
                    default:
                        if (property.value != undefined) {
                            propertyModel.defaultValue = property.value;
                        }
                        break;
                }

                if (property.generateContent === true && property.generateType && property.generateType.id.length > 0) {
                    propertyModel.generateType = property.generateType.id;
                }
                requestModel.properties.push(propertyModel);
            }
        }

        createTestContentResource.startContentCreation(requestModel).then(function (nodeId) {
            vm.page.buttonGroupState = "success";
            $location.search("");
            $location.path('/content/content/edit/' + nodeId);
        }, function () {
            vm.page.buttonGroupState = "error";
        });

    }

    function getGeneratorsForProperty(property) {
        var editorToLookup = property;
        if (typeof (property) == "object") {
            editorToLookup = property.editor;
        }

        return _.filter(vm.contentCreators, function (generator) {
            return _.some(generator.supportedEditors, function (value) {
                return value == editorToLookup
            });
        });
    }

    function hackNodeNameGeneratorPosition() {
        if ($(".umb-panel-header").length) {
            if ($("#look-whos-editing-too-container").length == 0) {
                var nameGeneratorElement = $("#positionTest");
                $(".umb-editor-header__actions-menu").append(nameGeneratorElement);
                $(".umb-editor-header__actions-menu").css("margin-left", 0);
                nameGeneratorElement.css("display", "");
            }
        } else {
            setTimeout(hackNodeNameGeneratorPosition, 500);
        }
    };

    vm.createTestContent = createTestContent;

    initialize();
}).directive('validityOverriedIfContentCreator', function () {
    return {
        restrict: 'A',
        link: function ($scope) {
            $scope.$watch(function () {
                return $scope.property.generateContent
            }, function () {
                var control = $scope.propertyForm[$scope.property.view];
                if (control == undefined) {
                    return;
                }

                if ($scope.property.generateContent === true && $scope.property.generateType && $scope.property.generateType.id.length > 0) {
                    //Force it to be valid
                    control.$setValidity("required", true);
                }
                else {
                    //If there isn't a value, set it to something and then to empty, will retrigger all validations and so ignore the possibly forced valid
                    if (control.$viewValue == null || control.$viewValue.length == 0) {
                        control.$setViewValue('dummy');
                        control.$setViewValue('');
                    }
                }
            });
        }
    };
});
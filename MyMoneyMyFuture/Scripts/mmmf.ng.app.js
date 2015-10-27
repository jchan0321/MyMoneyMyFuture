

mmmf.ng = {
    app: {
        services: {}
		, controllers: {}
    }
    , controllerInstances: []
	, exceptions: {}
	, examples: {}
	, defaultDependencies: ["ngAnimate", "ngRoute", "ngSanitize", "ngCookies", "ui.bootstrap", ]
    , getModuleDependencies: function () {
        if (mmmf.extraNgDependencies) {
            var newItems = mmmf.ng.defaultDependencies.concat(mmmf.extraNgDependencies);
            return newItems;
        }
        return mmmf.ng.defaultDependencies;
    }
};

mmmf.ng.app.module = angular.module('mmmfApp', mmmf.ng.getModuleDependencies());

mmmf.ng.app.module.value('$mmmf', mmmf.page);

mmmf.ng.app.module.run(["$rootScope", "$location", "$analyticsService", function ($rootScope, $location, $analyticsService) {

    $rootScope.$on("$routeChangeSuccess", function (event, data) {
        console.log("on route success");
        console.log(event);
        console.log(data);

        var action = 1;
        var category = 14;
        var pageUrl = window.location.pathname;
        var controllerType = data.$$route.controller;
        var pageType;

        if (!controllerType)
            controllerType = "";

        switch (controllerType.toLowerCase()) {
            case "blogcontroller":
                pageType = 2;
                break;
            case "eventscontroller":
                pageType = 4;
                break;
            case "analyticscontroller":
                pageType = 5;
                break;
            case "partnerreferralscontroller":
                pageType = 6;
                break;
            case "paymentscontroller":
                pageType = 7;
                break;
            case "mmmfcontroller":
                pageType = 8;
                break;
            case "tagscontroller":
                pageType = 9;
                break;
            case "testscontroller":
                pageType = 10;
                break;
            case "userscontroller":
                pageType = 11;
                break;
            case "logincontroller":
                pageType = 12;
                break;
            case "planscontroller":
                pageType = 13;
                break
            case "passwordscontroller":
                pageType = 14;
                break;
            case "publiccontroller":
                pageType = 15;
                break;
            case "admincontroller":
                pageType = 16;
                break;
            case "quickquestionscontroller":
                pageType = 17;
                break;
            default:
                pageType = 0;
                break;
        }

        var data = {
            "Action": action,
            "Category": category,
            "PageType": pageType,
            "PageURL": pageUrl
        };

        $analyticsService.add(data, mmmf.page.onSuccess, mmmf.page.onError)
    });


}]);

//#region - base functions and objects -

mmmf.ng.exceptions.argumentException = function (msg) {
    this.message = msg;
    var err = new Error();


    console.error(msg + "\n" + err.stack);
}
mmmf.ng.app.services.baseEventServiceFactory = function ($rootScope) {

    var factory = this;

    factory.$rootScope = $rootScope;

    factory.eventService = new function () {

        var thisEventService = this;

        thisEventService.broadcast = function (eventName, arguments) {
            factory.$rootScope.$broadcast(eventName, arguments)
        }

        thisEventService.emit = function (eventName, arguments) {
            factory.$rootScope.$emit(eventName, arguments)
        }

        thisEventService.listen = function (eventName, callback) {
            factory.$rootScope.$on(eventName, callback)
        }

    }

    return factory.eventService;
}

mmmf.ng.app.services.baseService = function ($win, $loc, $util) {
    /*
        when this function is envoked by Angular, Angular wants an instance of the Service object. 
		
    */

    var getChangeNotifier = function ($scope) {
        /*
        will be called when there is an event outside Angular that has modified
        our data and we need to let Angular know about it.
        */
        var self = this;

        self.scope = $scope;

        return function (fx) {
            self.scope.$apply(fx);//this is the magic right here that cause ng to re-evaluate bindings
        }


    }

    var baseService = {
        $window: $win
        , getNotifier: getChangeNotifier
        , $location: $loc
        , $utils: $util
        , merge: $.extend
    };

    return baseService;
}

mmmf.ng.app.controllers.baseController = function ($doc, $logger, $mmmf, $route, $routeParams, $alertService) {
    /*
        this is intended to serve as the base controller
    */

    baseControler = {
        $document: $doc
		, $log: $logger
        , $mmmf: $mmmf
        , merge: $.extend


        , setUpCurrentRequest: function (viewModel) {

            viewModel.currentRequest = { originalPath: "/", isTop: true };

            if (viewModel.$route.current) {
                viewModel.currentRequest = viewModel.$route.current;
                viewModel.currentRequest.locals = {};
                viewModel.currentRequest.isTop = false;
            }

            viewModel.$log.log("setUpCurrentRequest firing:");
            viewModel.$log.debug(viewModel.currentRequest);

        }
        , hasFlag: function (check, flag) {
            return (check & flag) == flag;
        }

    };

    return baseControler;
}

//#endregion

//#region  - core ng wrapper functions --

mmmf.ng.getControllerInstance = function (jQueryObj) {///used to grab an instance of a controller bound to an Element
    console.log(jQueryObj);
    return angular.element(jQueryObj[0]).controller();
}

mmmf.ng.addService = function (ngModule, serviceName, dependencies, factory) {

    if (!ngModule ||
		!serviceName || !factory ||
		!angular.isFunction(factory)) {
        throw new mmmf.ng.exceptions.argumentException("Invalid Service Call");
    }

    if (dependencies && !angular.isArray(dependencies)) {
        throw new mmmf.ng.exceptions.argumentException("Invalid Service Call [dependencies]");
    }
    else if (!dependencies) {
        dependencies = [];
    }

    dependencies.push(factory);

    ngModule.service(serviceName, dependencies);

}

mmmf.ng.registerService = mmmf.ng.addService;

mmmf.ng.addController = function (ngModule, controllerName, dependencies, factory) {
    if (!ngModule ||
		!controllerName || !factory ||
		!angular.isFunction(factory)) {
        throw new mmmf.ng.exceptions.argumentException("Invalid Service defined");
    }

    if (dependencies && !angular.isArray(dependencies)) {
        throw new mmmf.ng.exceptions.argumentException("Invalid Service Call [dependencies]");
    }
    else if (!dependencies) {
        dependencies = [];
    }

    dependencies.push(factory);
    ngModule.controller(controllerName, dependencies);

}

mmmf.ng.registerController = mmmf.ng.addController;


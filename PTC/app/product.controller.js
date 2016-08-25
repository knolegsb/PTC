﻿(function () {
    'use strict';

    angular.module('app').controller('ProductController', ProductController);

    function ProductController($http) {
        var vm = this;
        var dataService = $http;

        vm.product = {
            ProductId : 1,
            ProductName : 'Video Training'
        };

        vm.products = [];

        productList();

        function productList() {
            dataService.get("/api/Product")
                .then(function (result) {
                    vm.products = result.data;
                    debugger;
                }, function (error) {
                    handleException(error);
                });
        }

        function handleExceptioin(error) {
            alert(error.data.ExceptionMessage);
        }
    }
})();
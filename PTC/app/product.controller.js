(function () {
    'use strict';

    angular.module('app').controller('ProductController', ProductController);

    function ProductController($http) {
        var vm = this;
        var dataService = $http;

        //vm.product = {
        //    ProductId : 1,
        //    ProductName : 'Video Training'
        //};

        // Hook up public events
        vm.resetSearch = resetSearch;

        function resetSearch() {
            vm.searchInput = {
                selectedCategory: {
                    CategoryId: 0,
                    CategoryName: ''
                },
                productName: ''
            };

            productList();
        }

        vm.searchCategories = [];
        searchCategoriesList();

        vm.searchInput = {
            selectedCategory: {
                CategoryId: 0,
                CategoryName: ''
            },
            productName: ''
        };

        function searchCategoriesList() {
            dataService.get("/api/Category/GetSearchCategories")
                .then(function (result) {
                    vm.searchCategories = result.data;
                    debugger;
                }, function (error) {
                    handleException(error);
                });
        }

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
function productListingService(productListingResource){
    return{
        getProducts :function(number){
            return productListingResource.getProducts(number)
                .then((data)=>{
                        return data;
                }, function (){});
        }
    }
}
angular.module("umbraco.services").factory("productListingService", productListingService);
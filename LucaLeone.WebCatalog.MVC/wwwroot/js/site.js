$(document).ready(function () {
    // Wire up the Add button to send the new item to the server
    $('.delete-Product-button').on('click', function (e) {
        DeleteProduct(e.target);
    });
    $('.edit-Product-button').on('click', function (e) {
        EditProduct(e.target);
    });
});
function DeleteProduct(product) {
    var productId = product.name;
    console.log("DeleteProduct: " + productId);
    window.location = '/DeleteProduct?id=' + productId;
};
function EditProduct(product) {
    var productId = product.name;
    console.log("EditProduct: " + productId);
    window.location = '/EditProduct/' + productId;
}

# Products Catalog
Solution for managing products catalog. With the MVC app the users can to add, edit, remove, view and search and export (Excel CSV) product catalog items. The Information about product catalog entities are exposed through RESTful service.  

## Features
🚀 Framework: .NET CORE 2.1<br />
📃 API documentation: Swagger with Swashbuckle.AspNetCore<br />
🏗 Patterns: MVC, Dependency injectin, Builder<br />

## For developer only
### Models and Convention
* **Product**:<br />
Price, __Type Decimal for base 10 precision and avoid rounding problems with the base 2 floating-point types__. The Currency is arbitrarily set to €, in a future implementation the problem of the different currency could be solved creating a model for the price, with members: value, currency, country. The Product model will have a list of prices, this way is not only possible to manage the different currency but also the price in different country.
* **LastUpdated**:<br />
Saved in UTC time in the DB, the clients will convert the UTC time to the local time
* **Photo**:<br />
The product photo is represented by a URL to an external resource. A future implementation will allow upload and compression of images.
### Design Pattern
![screens](/Presentation/Architecture.png)
* **MVC**:<br />The division between the WEBAPI solution and the MVC app allows the code to be clean, modular, scalable and reusable.
* **Builder pattern**:<br />
Link: https://en.wikipedia.org/wiki/Builder_pattern#C#
Used for NewProduct class with the interface IProductBuilder,
```c#
public interface IProductBuilder
{
    Product BuildProduct();
}
```
This allows:
- to decouple the database model Product from classes like NewProduct that only have a functional scope
- to reduce the syntax needed to transform a NewProduct to Product, ex:
```c#
public async Task<bool> AddProductAsync(IProductBuilder newProduct)
{
    Product entity = newProduct.BuildProduct();
    _context.Products.Add(entity);
    int saveResult = await _context.SaveChangesAsync();
    return saveResult == 1;
}

```
### Export Function
CSV has been chosen over xls for the file size, a smaller size reduces the computation and memory for the creation and time in the download.
The size difference is visible in small files (40 lines) as well in bigger files.
Same data in the 3 different file:
![screens](/Presentation/export.png)
The CatalogExport is not created upon request, it is created using the FileHelper AsyncEngine after the Edit product, delete Product services. Add Product uses the same file and add a line at the end of the CatalogExport.csv. This way the user that requests the export don’t have to wait for the creation of the file.
While the AddProduct simply add a new line to the existing file, edit or delete re-write the entire file, this can cause an excessive computation with the editing and deleting depending on the magnitude of the requests, but since this app is oriented to the admins of the catalog the editing and deleting requests are limited. 
A future improvement is to write the products ordered by datatime, in this way is possible to do a binary search ( O(log N)  ) in the file in order to delete or edit a single line.
### Api Documentation
API documentation made with Swashbuckle.AspNetCore available at: https://localhost:44303/api-docs
![screens](/Presentation/Swagger.png)
### Data Concurrency
Data concurrency in this project is managed this way:
User1 start editing ProductA, User2 delete ProductA, User1 save changes-> User1 changes are lost and the ProductA deleted
User1 start editing ProductA, User2 start editing ProductA -> User1 saves changes and then User2 saves changes -> User2Changes are maintained (to avoid this in a future implementation could be possible to lock a product so the User2 cannot access the editing function until the user1 finishes the changes) 

### Invalid Query
The validation layer of the WebAPI protects the database from non-sense entries (ex: products with negative price). The MVC app currently does not manage the bad responses from the Web api.
### How to test locally
![screens](/Presentation/SetUp.png)
Right click on the solution>properties and set a multiple startup projects: with action start, click apply, then ok.
The solution implements 2 more functions useful for testing: initCatalog to create a catalog and eraseCatalog.
It’s possible to use these functions using the NavBar of the MVC. After the click go back and refresh the page. Note: these functions are meant for development only.   
Start F5 the multiple projects’ solution and click on initCatalog in the navbar, then go back and refresh the page, this will provide a testing catalog with 4 products.
Read the section down below for the additional packeges.
![screens](/Presentation/navBar.png)
### Additional packages
The Catalog.API projects uses:
For the creation of the CSV file:
```
Install-Package FileHelpers
```
For the documentation:
```
Install-Package Swashbuckle.AspNetCore
```
### Note
Product images displays a default picture in case of: no image, loading error.
### Credits
Logo Icon made with: https://romannurik.github.io/AndroidAssetStudio/icons-launcher.html
### Screenshots
On the left the MVC app, on the right the WEB API
Home Page: 
![screens](/Presentation/screen1.png)
Add Product: 
![screens](/Presentation/screen2.png)
Edit Product (in the home page click to the edit button of product):
![screens](/Presentation/screen3.png)
Search function ( http://localhost:53536/Search?name=confort  ) 
![screens](/Presentation/screen4.png)



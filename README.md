**Before I start my ReadME sir I do want you to note that i have left all my trial and error products on the mywork page and therefore you must please scroll to the bottem where i have added new products that are all available for use. The products above were all testers to get my images working and to ensure that my Availability feature is working and that a user can add an item to the cart. I just did not want to mess up anything on the last day by trying to delete things off my database**

# Project Documentation

This project is a web application built with ASP.NET Core MVC. It's an online store where users can view products, add them to a cart, and place orders.

## Models

### LoginModel

The LoginModel class is responsible for validating user credentials during login. It includes a method ValidateUser that takes an email and password, hashes the password, and compares it with the hashed password stored in the database. If the hashed passwords match, the method returns the user's ID.

### UserTable

The userTable class represents a user in the system. It includes properties for the user's ID, first name, last name, email, and password. It also includes methods for inserting a new user and updating an existing user.

### ProductTable

The productTable class represents a product in the store. It includes properties for the product's ID, name, price, category, availability, description, and image URL. It also includes methods for inserting a new product and fetching all products.

### OrderTable

The orderTable class represents an order placed by a user. It includes properties for the order's ID, the user's ID, the order date, the total amount, and the status. It also includes methods for inserting a new order and fetching all orders by a user's ID.

### OrderDetails

The OrderDetails class represents the details of an order, including the order detail's ID, the order's ID, the product's ID, the quantity, and the price.

### Cart

The Cart class represents a user's shopping cart. It includes a list of CartItem objects and a property for the total amount of the cart.

### CartItem
The CartItem class represents an item in a user's shopping cart. It includes properties for the product's ID, the product's name, the quantity, and the price.

## Controllers

### HomeController

The HomeController class handles actions related to the home page, about us page, contact us page, my work page, login page, admin login page, sign up page, and add product page.

### AccountController
The AccountController class handles actions related to user authentication, including signing up, logging in, and logging out.

### ProductController

The ProductController class handles actions related to products, including adding a product and creating a product.

### ProductDisplayController

The ProductDisplayController class handles actions related to displaying products, adding them to the cart, and checking out.

### TransactionController

The TransactionController class handles actions related to placing an order.

## Views

### _Layout.cshtml

The _Layout.cshtml file is the main layout file for the application. It includes the HTML structure of the page, links to CSS stylesheets, and scripts.

### SignUp.cshtml

The SignUp.cshtml file is the view for the user sign up page. It includes a form for the user to enter their first name, last name, email, and password.

### LogIn.cshtml

The LogIn.cshtml file is the view for the user login page. It includes a form for the user to enter their email and password.

### CartView.cshtml

The CartView.cshtml file is the view for the shopping cart. It displays a table with the items in the cart and their details.

### Confirmation.cshtml

The Confirmation.cshtml file is the view for the order confirmation page. It displays the order number and total amount.

### MyWork.cshtml

The MyWork.cshtml file is the view for the my work page. It displays a list of products.

### OrderHistory.cshtml

The OrderHistory.cshtml file is the view for the order history page. It displays a table with the user's past orders and their details.

### AboutUs.cshtml

The AboutUs.cshtml file is the view for the about us page. It includes information about the company, its mission, commitment, and future plans.

### ContactUs.cshtml

The ContactUs.cshtml file is the view for the contact us page. It includes a contact form and the company's contact details.

## Extensions

### SessionExtensions
The SessionExtensions class includes extension methods for the ISession interface. These methods allow you to store complex objects in the session by serializing them to JSON.

### Styles

The project includes several CSS files for styling the application. These files are linked in the _Layout.cshtml file.

### Database

The project uses a SQL Server database to store data. The connection string for the database is stored in the con_string field in each model class.

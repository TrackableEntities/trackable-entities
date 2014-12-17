Feature: Update Order
	In order to update orders
	As a Web API client
	I want to save orders to the database

@update_orders
Scenario: Create Order
	Given the following new customer orders
	| CustomerId |
	| ABCD       |
	When I submit a POST to create an order
	Then the request should return the new order

@update_orders
Scenario: Modify Order
	Given the following existing customer orders
	| CustomerId |
	| ABCD       |
	And the order is modified
	When I submit a PUT to modify an order
	Then the request should return the modified order

@update_orders
Scenario: Add Order Details
	Given the following existing customer orders
	| CustomerId |
	| ABCD       |
	And order details are added
	When I submit a PUT to modify an order
	Then the request should return the added order details

@update_orders
Scenario: Delete Order
	Given the following existing customer orders
	| CustomerId |
	| ABCD       |
	When I submit a DELETE to delete an order
	Then the order should be deleted

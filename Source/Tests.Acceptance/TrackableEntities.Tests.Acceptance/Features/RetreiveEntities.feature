Feature: Retreive Entities
	In order to retrieve entities
	As a Web API client
	I want to retrieve entities from the database

@retrieve_entities
Scenario: Retreive Customers
	Given the following customers
	| CustomerId | CustomerName       |
	| ABCD       | Test Customer ABCD |
	| EFGH       | Test Customer EFGH |
	When I submit a GET request for customers
	Then the request should return the customers

@retrieve_entities
Scenario: Retreive Customer Orders
	Given the following customers
	| CustomerId | CustomerName       |
	| ABCD       | Test Customer ABCD |
	And the following customer orders
	| CustomerId |
	| ABCD       |
	When I submit a GET request for customer orders
	Then the request should return the orders

@retrieve_entities
Scenario: Retreive Order
	Given the following customers
	| CustomerId | CustomerName       |
	| ABCD       | Test Customer ABCD |
	And the following customer orders
	| CustomerId |
	| ABCD       |
	When I submit a GET request for an order
	Then the request should return the orders

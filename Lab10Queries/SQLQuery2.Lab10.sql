SELECT Customers.CompanyName, COUNT(Orders.OrderID) as TotalOrders
FROM Customers
JOIN Orders ON Customers.CustomerID = Orders.CustomerID
GROUP BY Customers.CompanyName, ContactName
ORDER BY  TotalOrders DESC;
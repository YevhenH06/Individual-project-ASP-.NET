-- Спочатку вставимо виріб
INSERT INTO Production (ProductName, DisplayTime, ExpiryTime, Price, Quantity)
VALUES ('Торт Медовик', GETDATE(), DATEADD(DAY, 3, GETDATE()), 300.00, 10);

-- Потім додаємо знижку (ціна зі знижкою заповниться автоматично)
INSERT INTO Discount (DiscountName, DiscountValue, DiscountedPrice, ProductionId)
VALUES ('Акція на Медовик', 15.00, 0, 1); -- DiscountedPrice буде перезаписано тригером

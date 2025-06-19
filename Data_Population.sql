-- �������� �������� ����
INSERT INTO Production (ProductName, DisplayTime, ExpiryTime, Price, Quantity)
VALUES ('���� �������', GETDATE(), DATEADD(DAY, 3, GETDATE()), 300.00, 10);

-- ���� ������ ������ (���� � ������� ����������� �����������)
INSERT INTO Discount (DiscountName, DiscountValue, DiscountedPrice, ProductionId)
VALUES ('����� �� �������', 15.00, 0, 1); -- DiscountedPrice ���� ������������ ��������

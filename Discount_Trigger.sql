CREATE TRIGGER trg_CalculateDiscountedPrice
ON Discount
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE d
    SET d.DiscountedPrice = 
        ROUND(p.Price - (p.Price * d.DiscountValue / 100.0), 2)
    FROM Discount d
    INNER JOIN inserted i ON d.Id = i.Id
    INNER JOIN Production p ON d.ProductionId = p.Id;
END;

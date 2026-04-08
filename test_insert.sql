USE capled_stock;

-- Clean up
DELETE FROM ARTICLE_CHAMP_VALEUR WHERE ArticleId IN (SELECT Id FROM Equipments WHERE Reference LIKE 'T-001');
DELETE FROM ARTICLE_ETAT_DETAIL WHERE ArticleId IN (SELECT Id FROM Equipments WHERE Reference LIKE 'T-001');
DELETE FROM Equipments WHERE Reference = 'T-001';

-- Insert Equipment
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (5, 'T-001', 'Test Equipment', 'Test Desc', 'NEUF', 10, 2, 1, 1, 'EN_STOCK', 100.00, 50.00, NOW());

-- Get ID
SET @tid = LAST_INSERT_ID();

-- Insert Detail
INSERT INTO ARTICLE_ETAT_DETAIL (ArticleId, GradeVisuel, Observations) VALUES (@tid, 'A', 'Test Observation');

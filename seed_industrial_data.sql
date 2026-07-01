USE capled_stock;

-- 1. NETTOYAGE DES ANCIENNES DONNEES DE TEST (Utilisant les colonnes confirmées)
SET @p_ref_prefix = 'SEED-%';

DELETE FROM ARTICLE_CHAMP_VALEUR WHERE ArticleId IN (SELECT Id FROM Equipments WHERE Reference LIKE @p_ref_prefix);
DELETE FROM ARTICLE_ETAT_DETAIL WHERE ArticleId IN (SELECT Id FROM Equipments WHERE Reference LIKE @p_ref_prefix);
DELETE FROM Equipments WHERE Reference LIKE @p_ref_prefix;

-- 2. RÉCUPÉRATION DES IDS DES CATEGORIES (Déjà créées aux tours précédents)
-- ID 5: Variateurs de Fréquence
-- ID 9: Disjoncteurs
SET @catVar = 5;
SET @catDis = 9;

-- RÉCUPÉRATION DES IDS DES CHAMPS SPÉCIFIQUES (Déjà créés ou à créer)
-- Note: Je m'assure qu'ils existent pour la catégorie 5
INSERT IGNORE INTO CHAMP_SPECIFIQUE (CategorieId, NomChamp, TypeDonnee, Ordre) VALUES (@catVar, 'Puissance', 'TEXTE', 1);
INSERT IGNORE INTO CHAMP_SPECIFIQUE (CategorieId, NomChamp, TypeDonnee, Ordre) VALUES (@catVar, 'Tension', 'TEXTE', 2);
INSERT IGNORE INTO CHAMP_SPECIFIQUE (CategorieId, NomChamp, TypeDonnee, Ordre) VALUES (@catDis, 'Courant', 'NOMBRE', 1);

SET @spVarP = (SELECT Id FROM CHAMP_SPECIFIQUE WHERE CategorieId = @catVar AND NomChamp = 'Puissance' LIMIT 1);
SET @spVarT = (SELECT Id FROM CHAMP_SPECIFIQUE WHERE CategorieId = @catVar AND NomChamp = 'Tension' LIMIT 1);
SET @spDisC = (SELECT Id FROM CHAMP_SPECIFIQUE WHERE CategorieId = @catDis AND NomChamp = 'Courant' LIMIT 1);


-- 3. INSERTION DES 10 ARTICLES INDUSTRIELS

-- Article 1: Variateur Altivar NEUF
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catVar, 'SEED-ATV320', 'Altivar Machine ATV320', 'Variateur de vitesse 1.5 kW 400V 3 phases', 'NEUF', 5, 2, 1, 1, 'EN_STOCK', 350.00, 200.00, NOW());
SET @art1 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art1, @spVarP, '1.5 kW'), (@art1, @spVarT, '380-500 V');

-- Article 2: Variateur Siemens OCCASION
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catVar, 'SEED-G120C', 'Siemens Sinamics G120C', 'Variateur compact pour lignes de production industrielles', 'OCCASION', 1, 1, 1, 1, 'EN_STOCK', 240.00, 100.00, NOW());
SET @art2 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art2, @spVarP, '0.55 kW');
INSERT INTO ARTICLE_ETAT_DETAIL (ArticleId, GradeVisuel, PannesObservees) VALUES (@art2, 'B', 'Rayures façade, ventilateur un peu bruyant.');

-- Article 3: Variateur ABB RECONDITIONNE
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catVar, 'SEED-ACS355', 'ABB ACS355', 'Variateur pour applications hautes performances', 'RECONDITIONNE', 2, 1, 1, 1, 'STOCK_LIMITE', 500.00, 150.00, NOW());
SET @art3 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art3, @spVarP, '4.0 kW');
INSERT INTO ARTICLE_ETAT_DETAIL (ArticleId, GradeVisuel, RevisionsEffectuees) VALUES (@art3, 'A', 'Remplacement condensateurs, test en charge OK.');

-- Article 4: Variateur Danfoss EN_REPARATION
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catVar, 'SEED-FC302', 'Danfoss VLT AutomationDrive', 'Performances premium pour automatisation', 'EN_REPARATION', 0, 1, 1, 1, 'INDISPONIBLE', 950.00, 300.00, NOW());
SET @art4 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art4, @spVarP, '11 kW');
INSERT INTO ARTICLE_ETAT_DETAIL (ArticleId, PannesObservees) VALUES (@art4, 'Carte de puissance HS, en attente de pièces.');

-- Article 5: Disjoncteur Schneider Compact OCCASION
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catDis, 'SEED-NSX100', 'Schneider Compact NSX100N', 'Disjoncteur boitier moulé TM-D', 'OCCASION', 12, 5, 1, 1, 'EN_STOCK', 120.00, 40.00, NOW());
SET @art5 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art5, @spDisC, '100');

-- Article 6: Disjoncteur ABB NEUF
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catDis, 'SEED-S202C', 'ABB S202-C16', 'Disjoncteur modulaire pour tableau tertiaire', 'NEUF', 45, 10, 1, 1, 'EN_STOCK', 15.50, 7.00, NOW());
SET @art6 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art6, @spDisC, '16');

-- Article 7: Disjoncteur Legrand RECONDITIONNE
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catDis, 'SEED-DPX3', 'Legrand DPX3 160', 'Disjoncteur de puissance thermique', 'RECONDITIONNE', 3, 2, 1, 1, 'STOCK_LIMITE', 180.00, 80.00, NOW());
SET @art7 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art7, @spDisC, '160');
INSERT INTO ARTICLE_ETAT_DETAIL (ArticleId, GradeVisuel, TestsFonctionnels) VALUES (@art7, 'A', 'Test isolement et déclenchement OK.');

-- Article 8: Disjoncteur Eaton ENDOMMAGE
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catDis, 'SEED-NZM1', 'Eaton NZM1 Pro', 'Disjoncteur industriel forte intensité (HS)', 'ENDOMMAGE', 1, 0, 1, 1, 'INDISPONIBLE', 100.00, 50.00, NOW());
SET @art8 = LAST_INSERT_ID();
INSERT INTO ARTICLE_ETAT_DETAIL (ArticleId, GradeVisuel, PannesObservees) VALUES (@art8, 'D', 'Déclencheur magnétique HS.');

-- Article 10: Disjoncteur Hager NEUF
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catDis, 'SEED-MHN', 'Hager MHN116 Pro', 'Disjoncteur phase neutre courbe C', 'NEUF', 100, 20, 1, 1, 'EN_STOCK', 8.20, 4.10, NOW());
SET @art10 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art10, @spDisC, '16');


-- 4. INSERTION ARTICLE 9 SANS DETAILS (Pour varier)
INSERT INTO Equipments (CategoryId, Reference, Name, Description, `Condition`, Quantity, MinThreshold, IsPublished, VisibleSite, DisponibiliteSite, PrixVente, PrixAchat, CreatedAt)
VALUES (@catVar, 'SEED-V1000', 'Yaskawa V1000 Premium', 'Variateur compact haute performance.', 'NEUF', 0, 0, 1, 1, 'SUR_COMMANDE', 550.00, 390.00, NOW());
SET @art9 = LAST_INSERT_ID();
INSERT INTO ARTICLE_CHAMP_VALEUR (ArticleId, ChampSpecifiqueId, Valeur) VALUES (@art9, @spVarP, '5.5 kW');

-- 5. CONFIG ARTICLE SIMILAIRE
UPDATE Equipments SET ArticleSimilaireIds = CONCAT('[', @art2, ',', @art3, ']') WHERE Id = @art1;
UPDATE Equipments SET ArticleSimilaireIds = CONCAT('[', @art1, ',', @art3, ']') WHERE Id = @art2;

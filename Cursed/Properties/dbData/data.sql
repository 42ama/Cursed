IF(NOT EXISTS(SELECT 1 FROM dbo.ProductCatalog))
BEGIN
SET IDENTITY_INSERT ProductCatalog ON;
INSERT INTO ProductCatalog
    (Id, Name, CAS, LicenseRequired)
VALUES 
    (1, 'Acetone', 1001, null),
    (2, 'Ammonia', 1002, null),
    (3, 'Copper', 1003, null),
    (4, 'Hydrazine', 1004, null),
    (5, 'Iron', 1005, null),
    (6, 'Mercury', 1006, null),
    (7, 'Potassium', 1007, null),
    (8, 'Silicon', 1008, null),
    (9, 'Sugar', 1009, null),
    (10, 'Sulphuric Acid', 1010, 1),
    (11, 'Water', 1011, null),
    (12, 'Aluminum', 1012, null),
    (13, 'Carbon', 1013, null),
    (14, 'Ethanol', 1014, null),
    (15, 'Hydrochloric Acid', 1015, 1),
    (16, 'Lithium', 1016, null),
    (17, 'Phosphorus', 1017, null),
    (18, 'Radium', 1018, 1),
    (19, 'Sodium', 1019, null),
    (20, 'Sulfur', 1020, null),
    (21, 'Tungsten', 1021, 1),
    (22, 'Hyronalin', 1022, null),
    (23, 'Arithrazine', 1023, null),
    (24, 'Alkysine', 1024, null),
    (25, 'Dylovene', 1025, null),
    (26, 'Imidazoline', 1026, null),
    (27, 'Peridaxon', 1027, null),
    (28, 'Cryoxadone', 1028, null),
    (29, 'Clonexadone', 1029, 1),
    (30, 'Oxycodone', 1030, null),
    (31, 'Paracetamol', 1031, null),
    (32, 'Tramadol', 1032, null),
    (33, 'Inaprovaline', 1033, null),
    (34, 'Dermaline', 1034, null),
    (35, 'Kelotane', 1035, null),
    (36, 'Dexalin', 1036, null),
    (37, 'Dexalin Plus', 1037, null),
    (38, 'Ethylredoxrazine', 1038, null),
    (39, 'Rezadone', 1039, null),
    (40, 'Ryetalyn', 1040, null),
    (41, 'Bicaridine', 1041, null),
    (42, 'Tricordrazine', 1042, null),
    (43, 'Leporazine', 1043, null),
    (44, 'Synaptizine', 1044, null),
    (45, 'Noexcutite', 1045, null),
    (46, 'Spaceacillin', 1046, null),
    (47, 'Hyperzine', 1047, null),
    (48, 'Antidexafen', 1048, null),
    (49, 'Methylphenidate', 1049, null),
    (50, 'Citalopram', 1050, null),
    (51, 'Paroxetine', 1051, null),
    (52, 'Phoron', 1052, null),
    (53, 'MindBreaker Toxin', 1053, null),
    (54, 'CarpoToxin', 1054, null)
;
SET IDENTITY_INSERT ProductCatalog OFF;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.Recipe))
BEGIN
SET IDENTITY_INSERT Recipe ON;
INSERT INTO Recipe
    (Id, Content, TechApproval, GovermentApproval)
VALUES 
    (1, '1 part Radium, 1 part Dylovene -> 2 part Hyronalin', 1, 1),
    (2, '1 part Hyronalin, 1 part Hydrazine -> 2 part Arithrazine', 1, 1),
    (3, '1 part Hydrochloric Acid, 1 part Ammonia, 1 part Dylovene -> 2 part Alkysine', 1, 1),
    (4, '1 part Silicon, 1 part Ammonia, 1 part Potassium -> 3 part Dylovene', 1, 1),
    (5, '1 part Hydrazine, 1 part Carbon, 1 part Dylovene -> 2 part Imidazoline', 1, null),
    (6, '1 part Bicardine, 1 part Clonexadone, 5 part Phoron(catalyst)', 1, null),
    (7, '1 part Dexalin, 1 part Water, 1 part Acetone -> 3 part Cryoxadone', 1, null),
    (8, '1 part Cryoxadone, 1 part Sodium, 0.1 part Phoron, 5 part Phoron (catalyst) -> 2 part Clonexadone, 5 part Phoron', null, 1),
    (9, '1 part Ethanol, 1 part Tramadol; 5 part Phoron (catalyst)', null, 1),
    (10, '1 part Tramadol, 1 part Sugar, 1 part Water', null, 1),
    (11, '1 part Inaprovaline, 1 part Ethanol, 1 part Acetone', null, null),
    (12, '1 part Acetone, 1 part Carbon, 1 part Sugar -> 3 part Inaprovaline', null, null),
    (13, N'1 part Kelotane, 1 part Acetone, 1 part Phosphorus -> 3 part Dermaline', null, null),
    (14, '1 part Silicon, 1 part Carbon -> 2 part Kelotane', null, null),
    (15, '2 part Acetone, 0.1 part Phoron, 5 part Phoron (catalyst) -> 1 part Dexalin, 5 part Phoron', null, null),
    (16, '1 part Dexalin, 1 part Carbon, 1 part Iron -> 3 part Dexalin Plus', null, null),
    (17, '1 part Acetone, 1 part Dylovene, 1 part Carbon -> 3 part Ethylredoxrazine', null, null),
    (18, '1 part CarpoToxin, 1 part Copper', null, null),
    (19, '1 part Arithrazine, 1 part Carbon', null, null),
    (20, '1 part Inaprovaline, 1 part Carbon -> 2 part Bicaridine', null, null),
    (21, '1 part Inaprovaline, 1 part Dylovene', null, null),
    (22, '1 part Silicon, 1 part Copper, 5 part Phoron (catalyst) -> 2 part Leporazine, 5 part Phoron', null, null),
    (23, '1 part Lithium, 1 part Sugar, 1 part Water', null, null),
    (24, '1 part Oxycodone, 1 part Dylovene', null, null),
    (25, '1 part Paracetamol, 1 part Inaprovaline', null, null),
    (26, '1 part Sugar, 1 part Phosphorous, 1 part Sulfur', null, null),
    (27, '1 part Paracetamol, 1 part Carbon.', null, null),
    (28, '1 part MindBreaker Toxin, 1 part Lithium', null, null),
    (29, '1 part MindBreaker Toxin, 1 part Carbon', null, null),
    (30, '1 part MindBreaker Toxin, 1 part Acetone, 1 part Inaprovaline', null, null),
    (31, '2 part Hyronalin, 2 part Hydrazine -> 4 part Arithrazine', 1, 1),
    (32, '2 part Hydrochloric Acid, 2 part Ammonia, 2 part Dylovene -> 4 part Alkysine', null, null)
;
SET IDENTITY_INSERT Recipe OFF;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.Facility))
BEGIN
SET IDENTITY_INSERT Facility ON;
INSERT INTO Facility
    (Id, Name, Latitude, Longitude)
VALUES 
    (1, 'Our facility #1', 11.123456, 22.123456),
    (2, 'Our facility #2', 11.023456, 22.523456),
    (3, 'Our facility #3', 9.123456, 22.223456)
;
SET IDENTITY_INSERT Facility OFF;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.TechProcess))
BEGIN
INSERT INTO TechProcess
    (FacilityId, RecipeId, DayEfficiency)
VALUES 
    (1, 31, 11.2),
    (1, 1, 17.11),
    (1, 3, 13.4),
    (2, 3, 9.7),
    (2, 4, 12.2),
    (2, 31, 14.4),
    (3, 1, 8.8),
    (3, 4, 3.22)
;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.Company))
BEGIN
SET IDENTITY_INSERT Company ON;
INSERT INTO Company
    (Id, Name)
VALUES 
    (1, 'Provider 1'),
    (2, 'Provider 2'),
    (3, 'Customer 1'),
    (4, 'Customer 2')
;
SET IDENTITY_INSERT Company OFF;
END;

IF(NOT EXISTS(SELECT 1 FROM dbo.Storage))
BEGIN
SET IDENTITY_INSERT Storage ON;
INSERT INTO Storage
    (Id, Name, Latitude, Longitude, CompanyId)
VALUES 
    (1, 'Our storage #2', 14.123456, 23.123456, 1),
    (2, 'Our storage #2', 23.123456, 23.723456, 1),
    (3, 'Provider #1 storage #1', 9.123456, 22.223456, 1),
    (4, 'Provider #1 storage #2', 17.123456, 23.223456, 1),
    (5, 'Provider #2 storage #1', 15.123456, 21.223456, 2),
    (6, 'Customer #1 storage #1', 14.123456, 19.223456, 3),
    (7, 'Customer #2 storage #1', 14.923456, 19.223456, 4)
;
SET IDENTITY_INSERT Storage OFF;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.Product))
BEGIN
SET IDENTITY_INSERT Product ON;
INSERT INTO Product
    (Id, UId, Quantity, QuantityUnit, Price, StorageID)
VALUES 
    (1, 2, 450.2, N'мл.', 170, 1),
    (2, 4, 420.33, N'мл.', 120, 1),
    (3, 25, 1500.99, N'мл.', 50, 1),
    (4, 23, 150.1, N'мл.', 170, 2),
    (5, 24, 120.2, N'мл.', 170, 1),
    (6, 2, 50000, N'мл.', 20, 3),
    (7, 4, 50000, N'мл.', 20, 3),
    (8, 7, 50000, N'мл.', 20, 3),
    (9, 8, 50000, N'мл.', 20, 3),
    (10, 15, 50000, N'мл.', 20, 3),
    (11, 18, 50000, N'мл.', 20, 3),
    (12, 22, 50000, N'мл.', 20, 3),
    (13, 2, 50000, N'мл.', 20, 4),
    (14, 4, 50000, N'мл.', 20, 4),
    (15, 7, 50000, N'мл.', 20, 4),
    (16, 8, 50000, N'мл.', 20, 4),
    (17, 15, 50000, N'мл.', 20, 4),
    (18, 22, 50000, N'мл.', 20, 4),
    (19, 25, 50000, N'мл.', 20, 4),
    (20, 25, 50000, N'мл.', 20, 5)
;
SET IDENTITY_INSERT Product OFF;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.RecipeProductChanges))
BEGIN
INSERT INTO RecipeProductChanges
    (RecipeId, ProductId, Type, Quantity)
VALUES 
    (1, 22, 'product', 2), (1, 18, 'material', 1), (1, 25, 'material', 1),
    (2, 23, 'product', 2), (2, 22, 'material', 1), (2, 4, 'material', 1),
    (3, 24, 'product', 2), (3, 15, 'material', 1), (3, 2, 'material', 1), (3, 25, 'material', 1),  
    (4, 25, 'product', 3), (4, 8, 'material', 1), (4, 2, 'material', 1), (4, 7, 'material', 1),
    (5, 26, 'product', 2), (5, 4, 'material', 1), (5, 13, 'material', 1), (5, 25, 'material', 1),
    (6, 27, 'product', 2), (6, 41, 'material', 1), (6, 29, 'material', 1), (6, 52, 'material', 5), (6, 52, 'product', 5),
    (7, 28, 'product', 3), (7, 36, 'material', 1), (7, 11, 'material', 1), (7, 1, 'material', 1),
    (8, 29, 'product', 2), (8, 28, 'material', 1), (8, 19, 'material', 1), (8, 52, 'material', 5.1), (8, 52, 'product', 5),
    (9, 30, 'product', 2), (9, 14, 'material', 1), (9, 32, 'material', 1), (9, 52, 'material', 5), (9, 52, 'product', 5),
    (10, 31, 'product', 3), (10, 32, 'material', 1), (10, 9, 'material', 1), (10, 11, 'material', 1),
    (11, 32, 'product', 3), (11, 33, 'material', 1), (11, 14, 'material', 1), (11, 1, 'material', 1),
    (12, 33, 'product', 3), (12, 1, 'material', 1), (12, 13, 'material', 1), (12, 9, 'material', 1),
    (13, 34, 'product', 3), (13, 35, 'material', 1), (13, 1, 'material', 1), (13, 17, 'material', 1),
    (14, 35, 'product', 2), (14, 8, 'material', 1), (14, 13, 'material', 1),
    (15, 36, 'product', 2), (15, 1, 'material', 1), (15, 52, 'material', 5.1), (15, 52, 'product', 5),
    (16, 37, 'product', 3), (16, 36, 'material', 1), (16, 13, 'material', 1), (16, 5, 'material', 1),
    (17, 38, 'product', 3), (17, 1, 'material', 1), (17, 25, 'material', 1), (17, 13, 'material', 1),
    (18, 39, 'product', 2), (18, 54, 'material', 1), (18, 3, 'material', 1),
    (19, 40, 'product', 2), (19, 23, 'material', 1), (19, 13, 'material', 1),
    (20, 41, 'product', 2), (20, 33, 'material', 1), (20, 13, 'material', 1),
    (21, 42, 'product', 2), (21, 33, 'material', 1), (21, 25, 'material', 1),
    (22, 43, 'product', 2), (22, 8, 'material', 1), (22, 3, 'material', 1), (22, 52, 'material', 5), (22, 52, 'product', 5),
    (23, 44, 'product', 3), (23, 16, 'material', 1), (23, 9, 'material', 1), (23, 11, 'material', 1),
    (24, 45, 'product', 2), (24, 30, 'material', 1), (24, 25, 'material', 1),
    (25, 46, 'product', 2), (25, 31, 'material', 1), (25, 33, 'material', 1),
    (26, 47, 'product', 3), (26, 9, 'material', 1), (26, 17, 'material', 1), (26, 20, 'material', 1),
    (27, 48, 'product', 2), (27, 31, 'material', 1), (27, 13, 'material', 1),
    (28, 49, 'product', 2), (28, 53, 'material', 1), (28, 16, 'material', 1),
    (29, 50, 'product', 2), (29, 53, 'material', 1), (29, 13, 'material', 1),
    (30, 51, 'product', 3), (30, 53, 'material', 1), (30, 1, 'material', 1), (30, 33, 'material', 1),
    (31, 23, 'product', 4), (31, 22, 'material', 2), (31, 4, 'material', 2),
    (32, 24, 'product', 4), (32, 15, 'material', 2), (32, 2, 'material', 2), (32, 25, 'material', 2)
;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.License))
BEGIN
SET IDENTITY_INSERT License ON;
INSERT INTO License
    (Id, ProductId, GovermentNum, Date)
VALUES 
    (1, 10, 100001, '2020-03-04'),
    (2, 15, 100002, '2020-04-04'),
    (3, 18, 100003, '2017-04-04'),
    (4, 18, 100004, '2020-03-04'),
    (5, 21, 100005, '2020-03-04')
;
SET IDENTITY_INSERT License OFF;
END;



IF(NOT EXISTS(SELECT 1 FROM dbo.RecipeInheritance))
BEGIN
INSERT INTO RecipeInheritance
    (ParentId, ChildId)
VALUES 
    (2, 3), (2, 4), (2, 5),
    (3, 6), (2, 31), (3, 32)
;
END;

IF(NOT EXISTS(SELECT 1 FROM dbo.TransactionBatch))
BEGIN
SET IDENTITY_INSERT TransactionBatch ON;
INSERT INTO TransactionBatch
    (Id, CompanyId, Date, Type, IsOpen)
VALUES 
    (1, 2, '2019-11-11', 'income', 0),
	(2, 1, '2019-09-09', 'income', 0)
;
SET IDENTITY_INSERT TransactionBatch OFF;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.Operation))
BEGIN
SET IDENTITY_INSERT Operation ON;
INSERT INTO Operation
    (Id, ProductId, TransactionId, Quantity, Price, StorageFromId, StorageToId)
VALUES 
    (1, 25, 1, 1000, 25, 5, 1),
	(2, 2, 2, 322, 11, 3, 1),
	(3, 4, 2, 322, 24, 3, 1)
;
SET IDENTITY_INSERT Operation OFF;
END;
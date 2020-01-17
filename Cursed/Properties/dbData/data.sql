IF(NOT EXISTS(SELECT 1 FROM dbo.ProductCatalog))
BEGIN
SET IDENTITY_INSERT ProductCatalog ON;
INSERT INTO ProductCatalog
    (Id, Name, Type, CAS, LicenseRequired)
VALUES 
    (1, 'Acetone', 'material', 1001, null),
    (2, 'Ammonia', 'material', 1002, null),
    (3, 'Copper', 'material', 1003, null),
    (4, 'Hydrazine', 'material', 1004, null),
    (5, 'Iron', 'material', 1005, null),
    (6, 'Mercury', 'material', 1006, null),
    (7, 'Potassium', 'material', 1007, null),
    (8, 'Silicon', 'material', 1008, null),
    (9, 'Sugar', 'material', 1009, null),
    (10, 'Sulphuric Acid', 'material', 1010, 1),
    (11, 'Water', 'material', 1011, null),
    (12, 'Aluminum', 'material', 1012, null),
    (13, 'Carbon', 'material', 1013, null),
    (14, 'Ethanol', 'material', 1014, null),
    (15, 'Hydrochloric Acid', 'material', 1015, 1),
    (16, 'Lithium', 'material', 1016, null),
    (17, 'Phosphorus', 'material', 1017, null),
    (18, 'Radium', 'material', 1018, 1),
    (19, 'Sodium', 'material', 1019, null),
    (20, 'Sulfur', 'material', 1020, null),
    (21, 'Tungsten', 'material', 1021, 1),
    (22, 'Hyronalin', 'product', 1022, null),
    (23, 'Arithrazine', 'product', 1023, null),
    (24, 'Alkysine', 'product', 1024, null),
    (25, 'Dylovene', 'product', 1025, null),
    (26, 'Imidazoline', 'product', 1026, null),
    (27, 'Peridaxon', 'product', 1027, null),
    (28, 'Cryoxadone', 'product', 1028, null),
    (29, 'Clonexadone', 'product', 1029, 1),
    (30, 'Oxycodone', 'product', 1030, null),
    (31, 'Paracetamol', 'product', 1031, null),
    (32, 'Tramadol', 'product', 1032, null),
    (33, 'Inaprovaline', 'product', 1033, null),
    (34, 'Dermaline', 'product', 1034, null),
    (35, 'Kelotane', 'product', 1035, null),
    (36, 'Dexalin', 'product', 1036, null),
    (37, 'Dexalin Plus', 'product', 1037, null),
    (38, 'Ethylredoxrazine', 'product', 1038, null),
    (39, 'Rezadone', 'product', 1039, null),
    (40, 'Ryetalyn', 'product', 1040, null),
    (41, 'Bicaridine', 'product', 1041, null),
    (42, 'Tricordrazine', 'product', 1042, null),
    (43, 'Leporazine', 'product', 1043, null),
    (44, 'Synaptizine', 'product', 1044, null),
    (45, 'Noexcutite', 'product', 1045, null),
    (46, 'Spaceacillin', 'product', 1046, null),
    (47, 'Hyperzine', 'product', 1047, null),
    (48, 'Antidexafen', 'product', 1048, null),
    (49, 'Methylphenidate', 'product', 1049, null),
    (50, 'Citalopram', 'product', 1050, null),
    (51, 'Paroxetine', 'product', 1051, null),
    (52, 'Phoron', 'material', 1052, null),
    (53, 'MindBreaker Toxin', 'material', 1053, null),
    (54, 'CarpoToxin', 'material', 1054, null)
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
    (1, 23, 450.2, N'мл.', 170, 1),
    (2, 24, 220.33, N'мл.', 120, 1),
    (3, 25, 150.99, N'мл.', 50, 1),
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
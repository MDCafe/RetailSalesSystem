CREATE TABLE `Customers` (
  `Id` MediumInt NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(45) DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `MobileNo` int(11) DEFAULT NULL,
  `LanNo` int(11) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,  
  `CustomerTypeId` MEDIUMINT,
  `BalanceDue` decimal DEFAULT NULL,
  `CreditLimit` decimal DEFAULT NULL,
  `CreditDays` int not null,
  `IsExistingCustomer` tinyint(1)  not null,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT NULL,
  PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `Products`;
CREATE TABLE `Products` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `Code` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `UnitOfMeasure` mediumint(9) DEFAULT NULL,
  `CategoryId` mediumint(9) DEFAULT NULL,
  `CompanyId` mediumint(9) NOT NULL,
  `ReorderPoint` decimal(10,0) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MOU_idx` (`UnitOfMeasure`),
  KEY `FK_Company_idx` (`CompanyId`),
  KEY `FK_Category_idx` (`CategoryId`),
  CONSTRAINT `FK_Category` FOREIGN KEY (`CategoryId`) REFERENCES `Category` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Company` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_MOU` FOREIGN KEY (`UnitOfMeasure`) REFERENCES `MeasuringUnits` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

  
  CREATE TABLE IF NOT EXISTS `MeasuringUnits` (
   `Id` MEDIUMINT NOT NULL AUTO_INCREMENT,
  `unit` VARCHAR(10) NOT NULL,
  `MeasureQty` decimal NOT NULL,
  PRIMARY KEY (`Id`));
  
  CREATE TABLE IF NOT EXISTS `Category` (
   `Id` MEDIUMINT NOT NULL AUTO_INCREMENT,
   `parentId` MEDIUMINT NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`Id`)); 
  
   CREATE TABLE IF NOT EXISTS `Companies` (
   `Id` MEDIUMINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  `Address` varchar(50) NOT NULL,
  `MobileNo` int(11) DEFAULT NULL,
  `LanNo` int(11) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL, 
  `VATNo` varchar(15) DEFAULT NULL, 
  `IsSupplier` TINYINT(1) not null,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT NULL,
  PRIMARY KEY (`Id`));
  
  
  CREATE TABLE IF NOT EXISTS `Users` (
   `Id` MEDIUMINT NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` VARCHAR(20) NULL,
  PRIMARY KEY (`Id`));
  
  
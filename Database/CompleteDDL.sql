-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema RMS
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `RMS` ;

-- -----------------------------------------------------
-- Schema RMS
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `RMS` DEFAULT CHARACTER SET utf8 ;
USE `RMS` ;

-- -----------------------------------------------------
-- Table `RMS`.`ApplicationDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`ApplicationDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`ApplicationDetails` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(45) NULL DEFAULT NULL,
  `Description` VARCHAR(500) NULL DEFAULT NULL,
  `Address` VARCHAR(45) NULL DEFAULT NULL,
  `City` VARCHAR(45) NULL DEFAULT NULL,
  `Province` VARCHAR(45) NULL DEFAULT NULL,
  `Country` VARCHAR(45) NULL DEFAULT NULL,
  `Lan Line No` VARCHAR(15) NULL DEFAULT NULL,
  `Mobile No` VARCHAR(15) NULL DEFAULT NULL,
  `FacebookURL` VARCHAR(100) NULL DEFAULT NULL,
  `TwitterURL` VARCHAR(100) NULL DEFAULT NULL,
  `EmailAddress` VARCHAR(100) NULL DEFAULT NULL,
  `WhatsAppNo` VARCHAR(15) NULL DEFAULT NULL,
  `LogoUrl` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8
COMMENT = '	';


-- -----------------------------------------------------
-- Table `RMS`.`Category`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Category` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Category` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `parentId` MEDIUMINT(9) NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  `RollingNo` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 14
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`CodeMaster`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`CodeMaster` ;

CREATE TABLE IF NOT EXISTS `RMS`.`CodeMaster` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Code` VARCHAR(5) NULL DEFAULT NULL,
  `Description` VARCHAR(30) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 5
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`Companies`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Companies` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Companies` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  `Address` VARCHAR(50) NOT NULL,
  `MobileNo` INT(11) NULL DEFAULT NULL,
  `LanNo` INT(11) NULL DEFAULT NULL,
  `Email` VARCHAR(45) NULL DEFAULT NULL,
  `VATNo` VARCHAR(15) NULL DEFAULT NULL,
  `IsSupplier` TINYINT(1) NOT NULL,
  `CategoryTypeId` INT(11) NULL DEFAULT NULL,
  `DueAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`Customers`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Customers` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Customers` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(45) NULL DEFAULT NULL,
  `Description` VARCHAR(45) NULL DEFAULT NULL,
  `Address` VARCHAR(500) NULL DEFAULT NULL,
  `City` VARCHAR(45) NULL DEFAULT NULL,
  `MobileNo` INT(11) NULL DEFAULT NULL,
  `LanNo` INT(11) NULL DEFAULT NULL,
  `Email` VARCHAR(45) NULL DEFAULT NULL,
  `CustomerTypeId` MEDIUMINT(9) NULL DEFAULT NULL,
  `BalanceDue` DECIMAL(10,0) NULL DEFAULT NULL,
  `CreditLimit` DECIMAL(10,0) NULL DEFAULT NULL,
  `CreditDays` INT(11) NOT NULL DEFAULT '30',
  `IsExistingCustomer` TINYINT(1) NOT NULL DEFAULT '0',
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 270
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`ExpiryDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`ExpiryDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`ExpiryDetails` (
  `Id` INT(11) NOT NULL,
  `ProductId` INT(11) NULL DEFAULT NULL,
  `ExpiryDate` DATETIME NULL DEFAULT NULL,
  `Quantity` DECIMAL(10,2) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT NULL,
  `ModifiedOn` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`MeasuringUnits`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`MeasuringUnits` ;

CREATE TABLE IF NOT EXISTS `RMS`.`MeasuringUnits` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `unit` VARCHAR(10) NOT NULL,
  `MeasureQty` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`Sales`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Sales` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Sales` (
  `BillId` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `CustomerId` MEDIUMINT(9) NOT NULL,
  `Discount` DECIMAL(10,2) NULL DEFAULT NULL,
  `TransportCharges` DECIMAL(10,2) NULL DEFAULT NULL,
  `TotalAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `IsCancelled` BIT(1) NULL DEFAULT NULL,
  `CustomerOrderNo` VARCHAR(30) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  `RunningBillNo` MEDIUMINT(9) NOT NULL,
  `PaymentMode` CHAR(1) NOT NULL,
  PRIMARY KEY (`BillId`),
  CONSTRAINT `FK_Sales_Cust`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `RMS`.`Customers` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 104
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_Sales_Cust_idx` ON `RMS`.`Sales` (`CustomerId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`PaymentDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`PaymentDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`PaymentDetails` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `BillId` MEDIUMINT(9) NOT NULL,
  `CustomerId` MEDIUMINT(9) NOT NULL,
  `AmountPaid` DECIMAL(10,2) NOT NULL,
  `PaymentMode` VARCHAR(6) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`, `BillId`, `CustomerId`),
  CONSTRAINT `FK_customerId`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `RMS`.`Customers` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_pay_bill`
    FOREIGN KEY (`BillId`)
    REFERENCES `RMS`.`Sales` (`BillId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 38
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_customerId_idx` ON `RMS`.`PaymentDetails` (`CustomerId` ASC);

CREATE INDEX `FK_pay_bill_idx` ON `RMS`.`PaymentDetails` (`BillId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`Products`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Products` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Products` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `Code` VARCHAR(45) NULL DEFAULT NULL,
  `Name` VARCHAR(45) NULL DEFAULT NULL,
  `Description` VARCHAR(100) NULL DEFAULT NULL,
  `UnitOfMeasure` MEDIUMINT(9) NULL DEFAULT NULL,
  `CategoryId` MEDIUMINT(9) NULL DEFAULT NULL,
  `CompanyId` MEDIUMINT(9) NOT NULL,
  `ReorderPoint` DECIMAL(10,2) NOT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_Category`
    FOREIGN KEY (`CategoryId`)
    REFERENCES `RMS`.`Category` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Company`
    FOREIGN KEY (`CompanyId`)
    REFERENCES `RMS`.`Companies` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_MOU`
    FOREIGN KEY (`UnitOfMeasure`)
    REFERENCES `RMS`.`MeasuringUnits` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 6910
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_MOU_idx` ON `RMS`.`Products` (`UnitOfMeasure` ASC);

CREATE INDEX `FK_Company_idx` ON `RMS`.`Products` (`CompanyId` ASC);

CREATE INDEX `FK_Category_idx` ON `RMS`.`Products` (`CategoryId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`PriceDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`PriceDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`PriceDetails` (
  `PriceId` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `BillId` MEDIUMINT(9) NOT NULL,
  `ProductId` MEDIUMINT(9) NOT NULL,
  `Price` DECIMAL(10,2) NOT NULL,
  `SellingPrice` DECIMAL(10,2) NOT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`PriceId`),
  CONSTRAINT `FK_Product`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 3908
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_Product_idx` ON `RMS`.`PriceDetails` (`ProductId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`Purchases`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Purchases` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Purchases` (
  `BillId` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `RunningBillNo` INT(11) NULL DEFAULT NULL,
  `CompanyId` MEDIUMINT(9) NOT NULL,
  `InvoiceNo` VARCHAR(45) NULL DEFAULT NULL,
  `Discount` DECIMAL(10,2) NULL DEFAULT NULL,
  `SpecialDiscount` DECIMAL(10,2) NULL DEFAULT NULL,
  `TotalBillAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `Tax` DECIMAL(10,2) NULL DEFAULT NULL,
  `PaymentMode` CHAR(1) NULL DEFAULT NULL,
  `TransportCharges` DECIMAL(10,2) NULL DEFAULT NULL,
  `CoolieCharges` DECIMAL(10,2) NULL DEFAULT NULL,
  `KCoolieCharges` DECIMAL(10,2) NULL DEFAULT NULL,
  `LocalCoolieCharges` DECIMAL(10,2) NULL DEFAULT NULL,
  `IsCancelled` BIT(1) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`BillId`),
  CONSTRAINT `FK_CompanyId`
    FOREIGN KEY (`CompanyId`)
    REFERENCES `RMS`.`Companies` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 13
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_CompanyId_idx` ON `RMS`.`Purchases` (`CompanyId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`PurchaseDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`PurchaseDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`PurchaseDetails` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `BillId` MEDIUMINT(9) NOT NULL,
  `ProductId` MEDIUMINT(9) NULL DEFAULT NULL,
  `PriceId` MEDIUMINT(9) NULL DEFAULT NULL,
  `PurchasedQty` DECIMAL(10,2) NULL DEFAULT NULL,
  `ActualPrice` DECIMAL(10,2) NOT NULL,
  `Discount` DECIMAL(10,2) NULL DEFAULT NULL,
  `Tax` DECIMAL(10,2) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ProductId`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_PurchaseBillId`
    FOREIGN KEY (`BillId`)
    REFERENCES `RMS`.`Purchases` (`BillId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_price_purchase`
    FOREIGN KEY (`PriceId`)
    REFERENCES `RMS`.`PriceDetails` (`PriceId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 15
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_PurchaseBillId_idx` ON `RMS`.`PurchaseDetails` (`BillId` ASC);

CREATE INDEX `FK_ProductId_idx` ON `RMS`.`PurchaseDetails` (`ProductId` ASC);

CREATE INDEX `FK_price_purchase_idx` ON `RMS`.`PurchaseDetails` (`PriceId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`PurchaseFreeDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`PurchaseFreeDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`PurchaseFreeDetails` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ProductId` MEDIUMINT(9) NOT NULL,
  `FreeQty` DECIMAL(10,2) NULL DEFAULT NULL,
  `FreeAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  `BillId` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ProductFree`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_pdt_bill`
    FOREIGN KEY (`BillId`)
    REFERENCES `RMS`.`PurchaseDetails` (`BillId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_Product_idx` ON `RMS`.`PurchaseFreeDetails` (`ProductId` ASC);

CREATE INDEX `FK_ProductFree_idx` ON `RMS`.`PurchaseFreeDetails` (`ProductId` ASC);

CREATE INDEX `FK_pdt_bill_idx` ON `RMS`.`PurchaseFreeDetails` (`BillId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`PurchasePaymentDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`PurchasePaymentDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`PurchasePaymentDetails` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `PurchaseBillId` MEDIUMINT(9) NOT NULL,
  `CompanyId` MEDIUMINT(9) NOT NULL,
  `AmountPaid` DECIMAL(10,2) NOT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `FK_compId`
    FOREIGN KEY (`CompanyId`)
    REFERENCES `RMS`.`Companies` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_purchaseId`
    FOREIGN KEY (`PurchaseBillId`)
    REFERENCES `RMS`.`Purchases` (`BillId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_compId_idx` ON `RMS`.`PurchasePaymentDetails` (`CompanyId` ASC);

CREATE INDEX `FK_purchaseId_idx` ON `RMS`.`PurchasePaymentDetails` (`PurchaseBillId` ASC);

CREATE INDEX `FK_purchaseIdDetails_idx` ON `RMS`.`PurchasePaymentDetails` (`PurchaseBillId` ASC);

CREATE INDEX `FK_purchaseIdDetail1s_idx` ON `RMS`.`PurchasePaymentDetails` (`PurchaseBillId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`PurchaseReturn`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`PurchaseReturn` ;

CREATE TABLE IF NOT EXISTS `RMS`.`PurchaseReturn` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `BillId` INT(11) NULL DEFAULT NULL,
  `ProductId` MEDIUMINT(9) NOT NULL,
  `Quantity` DECIMAL(10,2) NOT NULL,
  `PriceId` MEDIUMINT(9) NOT NULL,
  `ReturnReasonCode` INT(11) NULL DEFAULT NULL,
  `comments` VARCHAR(200) NULL DEFAULT NULL,
  `CreatedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ProductId_PR`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_returnCode_PR`
    FOREIGN KEY (`ReturnReasonCode`)
    REFERENCES `RMS`.`CodeMaster` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_ProductIdPR_idx` ON `RMS`.`PurchaseReturn` (`ProductId` ASC);

CREATE INDEX `FK_returnCodePR_idx` ON `RMS`.`PurchaseReturn` (`ReturnReasonCode` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`ReturnDamagedStocks`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`ReturnDamagedStocks` ;

CREATE TABLE IF NOT EXISTS `RMS`.`ReturnDamagedStocks` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `BillId` INT(11) NULL DEFAULT NULL,
  `ProductId` MEDIUMINT(9) NOT NULL,
  `Quantity` DECIMAL(10,2) NOT NULL,
  `PriceId` MEDIUMINT(9) NOT NULL,
  `comments` VARCHAR(200) NULL DEFAULT NULL,
  `isReturn` TINYINT(1) NOT NULL,
  `CreatedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ReturnReasonCode` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ProductId_RD`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_returnCode`
    FOREIGN KEY (`ReturnReasonCode`)
    REFERENCES `RMS`.`CodeMaster` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_ProductId_idx` ON `RMS`.`ReturnDamagedStocks` (`ProductId` ASC);

CREATE INDEX `FK_ProductIdRD_idx` ON `RMS`.`ReturnDamagedStocks` (`ProductId` ASC);

CREATE INDEX `FK_returnCode_idx` ON `RMS`.`ReturnDamagedStocks` (`ReturnReasonCode` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`Roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Roles` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Roles` (
  `Id` INT(11) NOT NULL,
  `RoleName` VARCHAR(20) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `RMS`.`SaleDetails`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`SaleDetails` ;

CREATE TABLE IF NOT EXISTS `RMS`.`SaleDetails` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `BillId` MEDIUMINT(9) NOT NULL,
  `ProductId` MEDIUMINT(9) NOT NULL,
  `PriceId` MEDIUMINT(9) NOT NULL,
  `SellingPrice` DECIMAL(10,2) NULL DEFAULT NULL,
  `Qty` DECIMAL(10,2) NULL DEFAULT NULL,
  `Discount` DECIMAL(10,2) NULL DEFAULT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ProdId`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_price_id`
    FOREIGN KEY (`PriceId`)
    REFERENCES `RMS`.`PriceDetails` (`PriceId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `Fk_sale_bill`
    FOREIGN KEY (`BillId`)
    REFERENCES `RMS`.`Sales` (`BillId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 140
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `Fk_sale_bill_idx` ON `RMS`.`SaleDetails` (`BillId` ASC);

CREATE INDEX `FK_ProdId_idx` ON `RMS`.`SaleDetails` (`ProductId` ASC);

CREATE INDEX `FK_price_id_idx` ON `RMS`.`SaleDetails` (`PriceId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`SaleTemp`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`SaleTemp` ;

CREATE TABLE IF NOT EXISTS `RMS`.`SaleTemp` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Guid` VARCHAR(200) NOT NULL,
  `SaleDate` DATETIME NULL DEFAULT NULL,
  `CustomerId` INT(11) NULL DEFAULT NULL,
  `PaymentMode` VARCHAR(6) NULL DEFAULT NULL,
  `OrderNo` VARCHAR(45) NULL DEFAULT NULL,
  `ProductId` INT(11) NULL DEFAULT NULL,
  `Quantity` DECIMAL(10,2) NULL DEFAULT NULL,
  `SellingPrice` DECIMAL(10,2) NULL DEFAULT NULL,
  `DiscountPercentage` DECIMAL(10,2) NULL DEFAULT NULL,
  `DiscountAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `Amount` DECIMAL(10,2) NULL DEFAULT NULL,
  `PriceId` INT(11) NULL DEFAULT NULL,
  `CreatedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`, `Guid`))
ENGINE = InnoDB
AUTO_INCREMENT = 81
DEFAULT CHARACTER SET = utf8
COMMENT = 'Temproary	 table to save sale details	';


-- -----------------------------------------------------
-- Table `RMS`.`Stocks`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Stocks` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Stocks` (
  `ProductId` MEDIUMINT(9) NOT NULL,
  `Quantity` DECIMAL(10,2) NOT NULL,
  `ExpiryDate` DATE NOT NULL,
  `PriceId` MEDIUMINT(9) NOT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT(9) NULL DEFAULT NULL,
  PRIMARY KEY (`ProductId`, `PriceId`, `ExpiryDate`),
  CONSTRAINT `FK_PrdId`
    FOREIGN KEY (`ProductId`)
    REFERENCES `RMS`.`Products` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `FK_PrdId_idx` ON `RMS`.`Stocks` (`ProductId` ASC);


-- -----------------------------------------------------
-- Table `RMS`.`Users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `RMS`.`Users` ;

CREATE TABLE IF NOT EXISTS `RMS`.`Users` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(50) NOT NULL,
  `password` VARCHAR(50) NOT NULL,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` VARCHAR(20) NULL DEFAULT NULL,
  `RoleId` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `fk_role`
    FOREIGN KEY (`RoleId`)
    REFERENCES `RMS`.`Roles` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8;

CREATE INDEX `fk_role_idx` ON `RMS`.`Users` (`RoleId` ASC);


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

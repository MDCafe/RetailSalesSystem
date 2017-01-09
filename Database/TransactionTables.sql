CREATE TABLE IF NOT EXISTS `mydb`.`PurchaseDetails` (  
  `BillId` MEDIUMINT NOT NULL,  
  `ProductId` MEDIUMINT NULL,
  `PurchasedQty` DECIMAL NULL, 
  `ActualPrice` DECIMAL NOT NULL, 
  `Discount` DECIMAL NULL, 
  `Tax` decimal null,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT NULL,
   PRIMARY KEY (`Id`)
  );
  
  CREATE TABLE IF NOT EXISTS `mydb`.`Purchases` (
    `BillId` MEDIUMINT NOT null AUTO_INCREMENT,
    `CompanyId` MEDIUMINT NOT null, -- Supplier Id
    `InvoiceNo` VARCHAR(45) NULL,
    `Discount` decimal Null,
    `TransportCharges` DECIMAL NULL,
    `TotalBillAmount` DECIMAL NULL,
    `Tax` decimal null,
	`AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`UpdatedBy` MEDIUMINT NULL,
   PRIMARY KEY (`BillId`)
  );
  
  CREATE TABLE IF NOT EXISTS `mydb`.`PurchaseFreeDetails` (
    `ProductId` MEDIUMINT NOT null,
	`FreeQty` DECIMAL NULL,
    `FreeAmount` DECIMAL NULL,
    `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`UpdatedBy` MEDIUMINT NULL
  );

CREATE TABLE IF NOT EXISTS `mydb`.`SaleDetails` (  
  `BillId` MEDIUMINT NOT NULL,
  `ProductId` mediumInt Not NULL,
  `CustomerId` MEDIUMINT NOT NULL,
  `PriceId` mediumint Not NULL, 
  `SellingPrice` DECIMAL NULL, 
  `Qty` DECIMAL NULL, 
  `Discount` DECIMAL NULL,  
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT NULL,
   PRIMARY KEY (`Id`)
  );
  
   CREATE TABLE IF NOT EXISTS `mydb`.`Sales` (
    `BillId` MEDIUMINT NOT null AUTO_INCREMENT,
    `CustomerId` MEDIUMINT NOT null,    
    `Discount` decimal Null,
    `TransportCharges` DECIMAL NULL,
    `TotalAmount` DECIMAL NULL,
    `IsCancelled` TINYINT(1) null,
	`AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`UpdatedBy` MEDIUMINT NULL,
   PRIMARY KEY (`BillId`)
  );

CREATE TABLE IF NOT EXISTS `mydb`.`PriceDetails` (
    `PriceId` MEDIUMINT NOT null AUTO_INCREMENT,
	`BillId` MEDIUMINT NOT NULL,  
    `ProductId` MEDIUMINT NOT NULL,    
	`Price` DECIMAL NOT NULL, 
	`SellingPrice` DECIMAL NOT NULL,
    `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`UpdatedBy` MEDIUMINT NULL,
    PRIMARY KEY (`PriceId`)
  );
  
  CREATE TABLE IF NOT EXISTS `mydb`.`Stocks` (
    `ProductId` MEDIUMINT NOT NULL,
    `Quantity` Decimal not null,
    `PriceId` mediumint not null,
    `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
	`UpdatedBy` MEDIUMINT NULL
  );
  
  CREATE TABLE IF NOT EXISTS `mydb`.`ReturnDamagedStocks` (
    `ProductId` MEDIUMINT NOT NULL,
    `Quantity` Decimal not null,
    `PriceId` mediumint not null,
    `comments` varchar(200) null,
    `isReturn` TINYINT(1) not null
  );

CREATE TABLE IF NOT EXISTS `mydb`.`PurchasePaymentDetails` (
  `PurchaseBillId` VARCHAR(45) NOT NUll,
  `CompanyId` decimal not null,
  `AmountPaid` decimal not null,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT NULL
);  

CREATE TABLE IF NOT EXISTS `mydb`.`AdvanceDetails` (
  `BillId` VARCHAR(45) NOT NUll,
  `CustomerId` decimal,
  `CustomerName` VARCHAR(100) null, -- incase of new customer 
  `AmountPaid` decimal not null,
  `AddedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` MEDIUMINT NULL
); 

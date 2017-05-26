CREATE TABLE `PurchaseDetails` (
  `BillId` mediumint(9) NOT NULL,
  `ProductId` mediumint(9) DEFAULT NULL,
  `PurchasedQty` decimal(10,0) DEFAULT NULL,
  `ActualPrice` decimal(10,0) NOT NULL,
  `Discount` decimal(10,0) DEFAULT NULL,
  `Tax` decimal(10,0) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  KEY `FK_PurchaseBillId_idx` (`BillId`),
  KEY `FK_ProductId_idx` (`ProductId`),
  CONSTRAINT `FK_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_PurchaseBillId` FOREIGN KEY (`BillId`) REFERENCES `Purchases` (`BillId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

  
  CREATE TABLE `Purchases` (
  `BillId` mediumint(9) NOT NULL AUTO_INCREMENT,
  `CompanyId` mediumint(9) NOT NULL,
  `InvoiceNo` varchar(45) DEFAULT NULL,
  `Discount` decimal(10,0) DEFAULT NULL,
  `TransportCharges` decimal(10,0) DEFAULT NULL,
  `TotalBillAmount` decimal(10,0) DEFAULT NULL,
  `Tax` decimal(10,0) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`BillId`),
  KEY `FK_CompanyId_idx` (`CompanyId`),
  CONSTRAINT `FK_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

  
 CREATE TABLE `PurchaseFreeDetails` (
  `ProductId` mediumint(9) NOT NULL,
  `FreeQty` decimal(10,0) DEFAULT NULL,
  `FreeAmount` decimal(10,0) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  KEY `FK_Product_idx` (`ProductId`),
  KEY `FK_ProductFree_idx` (`ProductId`),
  CONSTRAINT `FK_ProductFree` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `SaleDetails` (
  `BillId` mediumint(9) NOT NULL,
  `ProductId` mediumint(9) NOT NULL,
  `CustomerId` mediumint(9) NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `SellingPrice` decimal(10,0) DEFAULT NULL,
  `Qty` decimal(10,0) DEFAULT NULL,
  `Discount` decimal(10,0) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  KEY `Fk_sale_bill_idx` (`BillId`),
  KEY `FK_ProdId_idx` (`ProductId`),
  KEY `FK_Cust_idx` (`CustomerId`),
  CONSTRAINT `FK_Cust` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_ProdId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `Fk_sale_bill` FOREIGN KEY (`BillId`) REFERENCES `Sales` (`BillId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

  
   CREATE TABLE IF NOT EXISTS `Sales` (
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


CREATE TABLE `PriceDetails` (
  `PriceId` mediumint(9) NOT NULL AUTO_INCREMENT,
  `BillId` mediumint(9) NOT NULL,
  `ProductId` mediumint(9) NOT NULL,
  `Price` decimal(10,0) NOT NULL,
  `SellingPrice` decimal(10,0) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`PriceId`),
  KEY `FK_Product_idx` (`ProductId`),
  CONSTRAINT `FK_Product` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
  
 CREATE TABLE `Stocks` (
  `ProductId` mediumint(9) NOT NULL,
  `Quantity` decimal(10,0) NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  KEY `FK_PrdId_idx` (`ProductId`),
  CONSTRAINT `FK_PrdId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

  
 CREATE TABLE `ReturnDamagedStocks` (
  `ProductId` mediumint(9) NOT NULL,
  `Quantity` decimal(10,0) NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `comments` varchar(200) DEFAULT NULL,
  `isReturn` tinyint(1) NOT NULL,
  KEY `FK_ProductId_idx` (`ProductId`),
  KEY `FK_ProductIdRD_idx` (`ProductId`),
  CONSTRAINT `FK_ProductId_RD` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `PurchasePaymentDetails` (
  `PurchaseBillId` mediumint(9) NOT NULL,
  `CompanyId` mediumint(9) NOT NULL,
  `AmountPaid` decimal(10,0) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  KEY `FK_compId_idx` (`CompanyId`),
  KEY `FK_purchaseId_idx` (`PurchaseBillId`),
  KEY `FK_purchaseIdDetails_idx` (`PurchaseBillId`),
  KEY `FK_purchaseIdDetail1s_idx` (`PurchaseBillId`),
  CONSTRAINT `FK_compId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_purchaseId` FOREIGN KEY (`PurchaseBillId`) REFERENCES `Purchases` (`BillId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `PaymentDetails`;
CREATE TABLE `PaymentDetails` (
  `Id` MEDIUMINT(9) NOT NULL AUTO_INCREMENT,
  `BillId` MEDIUMINT(9) NOT NULL,
  `CustomerId` MEDIUMINT(9) NOT NULL ,  
  `AmountPaid` decimal(10,0) NOT NULL,
  `PaymentMode` VARCHAR(6) NULL DEFAULT NULL ,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_customerId_idx` (`CustomerId`),
  CONSTRAINT `FK_customerId` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


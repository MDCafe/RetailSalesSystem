-- MySQL dump 10.13  Distrib 8.0.15, for Win64 (x86_64)
--
-- Host: localhost    Database: rms
-- ------------------------------------------------------
-- Server version	8.0.15

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `applicationdetails`
--

DROP TABLE IF EXISTS `applicationdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `applicationdetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Address` varchar(45) DEFAULT NULL,
  `City` varchar(45) DEFAULT NULL,
  `Province` varchar(45) DEFAULT NULL,
  `Country` varchar(45) DEFAULT NULL,
  `Lan Line No` varchar(15) DEFAULT NULL,
  `Mobile No` varchar(15) DEFAULT NULL,
  `FacebookURL` varchar(100) DEFAULT NULL,
  `TwitterURL` varchar(100) DEFAULT NULL,
  `EmailAddress` varchar(100) DEFAULT NULL,
  `WhatsAppNo` varchar(15) DEFAULT NULL,
  `LogoUrl` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='	';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bankbranchdetails`
--

DROP TABLE IF EXISTS `bankbranchdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `bankbranchdetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `BankId` mediumint(9) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Bank_BR_idx` (`BankId`),
  CONSTRAINT `FK_Bank_BR` FOREIGN KEY (`BankId`) REFERENCES `bankdetails` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1239 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bankdetails`
--

DROP TABLE IF EXISTS `bankdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `bankdetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `Code` varchar(10) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `Address` varchar(400) DEFAULT NULL,
  `City` varchar(45) DEFAULT NULL,
  `ContactNo` mediumint(9) DEFAULT NULL,
  `ContactPerson` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8 COMMENT='	';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `category` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `parentId` mediumint(9) NOT NULL,
  `name` varchar(255) NOT NULL,
  `RollingNo` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=213 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chequepaymentdetails`
--

DROP TABLE IF EXISTS `chequepaymentdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `chequepaymentdetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `PaymentId` mediumint(9) NOT NULL,
  `ChequeNo` int(11) DEFAULT NULL,
  `ChequeDate` date DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `IsChequeRealised` bit(1) DEFAULT NULL,
  `PaymentDate` datetime DEFAULT NULL,
  `BankId` mediumint(9) DEFAULT NULL,
  `BankBranchId` mediumint(9) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`,`PaymentId`),
  KEY `FK_CHQ_paymentDetails_idx` (`PaymentId`),
  CONSTRAINT `FK_CHQ_paymentDetails` FOREIGN KEY (`PaymentId`) REFERENCES `paymentdetails` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1072 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `codemaster`
--

DROP TABLE IF EXISTS `codemaster`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `codemaster` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Code` varchar(5) DEFAULT NULL,
  `Description` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `companies`
--

DROP TABLE IF EXISTS `companies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `companies` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Address` varchar(200) NOT NULL,
  `City` varchar(50) DEFAULT NULL,
  `MobileNo` int(11) DEFAULT NULL,
  `LanNo` int(11) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,
  `VATNo` varchar(15) DEFAULT NULL,
  `IsSupplier` tinyint(1) NOT NULL,
  `CategoryTypeId` int(11) DEFAULT NULL,
  `DueAmount` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=266 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `customers` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) DEFAULT NULL,
  `Description` varchar(45) DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `City` varchar(100) DEFAULT NULL,
  `MobileNo` varchar(15) DEFAULT NULL,
  `LanNo` varchar(15) DEFAULT NULL,
  `FaxNo` varchar(15) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,
  `CustomerTypeId` mediumint(9) DEFAULT NULL,
  `BalanceDue` decimal(10,0) DEFAULT NULL,
  `OldBalanceDue` decimal(10,2) DEFAULT NULL,
  `CreditLimit` decimal(10,0) DEFAULT NULL,
  `CreditDays` int(11) NOT NULL DEFAULT '30',
  `ContactPerson` varchar(100) DEFAULT NULL,
  `IsLenderBorrower` tinyint(1) NOT NULL DEFAULT '0',
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=413 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `datebillmapping`
--

DROP TABLE IF EXISTS `datebillmapping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `datebillmapping` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `CustomerTypeId` mediumint(9) DEFAULT NULL,
  `EndOfDate` datetime DEFAULT NULL,
  `EndBillNo` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=227 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `directpaymentdetails`
--

DROP TABLE IF EXISTS `directpaymentdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `directpaymentdetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerId` mediumint(9) DEFAULT NULL,
  `PaidAmount` decimal(10,2) DEFAULT NULL,
  `PaymentDate` datetime DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `expensedetails`
--

DROP TABLE IF EXISTS `expensedetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `expensedetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `ExpenseTypeId` int(11) DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `Comments` varchar(500) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ExpCodeMaster_idx` (`ExpenseTypeId`),
  CONSTRAINT `ExpCodeMaster` FOREIGN KEY (`ExpenseTypeId`) REFERENCES `codemaster` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=774 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `measuringunits`
--

DROP TABLE IF EXISTS `measuringunits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `measuringunits` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `unit` varchar(10) NOT NULL,
  `MeasureQty` decimal(10,2) NOT NULL,
  `AllowFractions` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `paymentdetails`
--

DROP TABLE IF EXISTS `paymentdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `paymentdetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `BillId` mediumint(9) NOT NULL,
  `CustomerId` mediumint(9) DEFAULT NULL,
  `AmountPaid` decimal(10,2) DEFAULT NULL,
  `PaymentMode` mediumint(9) DEFAULT NULL,
  `PaymentDate` datetime DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Fk_sale_pay_idx` (`BillId`),
  CONSTRAINT `Fk_sale_pay` FOREIGN KEY (`BillId`) REFERENCES `sales` (`BillId`)
) ENGINE=InnoDB AUTO_INCREMENT=80168 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pricedetails`
--

DROP TABLE IF EXISTS `pricedetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `pricedetails` (
  `PriceId` mediumint(9) NOT NULL AUTO_INCREMENT,
  `BillId` mediumint(9) NOT NULL,
  `ProductId` mediumint(9) NOT NULL,
  `Price` decimal(10,2) NOT NULL,
  `SellingPrice` decimal(10,2) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`PriceId`),
  KEY `FK_Product_idx` (`ProductId`),
  CONSTRAINT `FK_Product` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6061 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `productcasemapping`
--

DROP TABLE IF EXISTS `productcasemapping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `productcasemapping` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `ProductId` mediumint(9) DEFAULT NULL,
  `ItemPerCase` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsActive` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_prd_cs_idx` (`ProductId`),
  CONSTRAINT `FK_prd_cs` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `products` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `Code` varchar(45) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `UnitOfMeasure` mediumint(9) DEFAULT NULL,
  `CategoryId` mediumint(9) DEFAULT NULL,
  `CompanyId` mediumint(9) NOT NULL,
  `ReorderPoint` decimal(10,2) NOT NULL,
  `SupportsMultiPrice` bit(1) DEFAULT NULL,
  `BarcodeNo` char(13) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  `IsActive` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MOU_idx` (`UnitOfMeasure`),
  KEY `FK_Company_idx` (`CompanyId`),
  KEY `FK_Category_idx` (`CategoryId`),
  CONSTRAINT `FK_Category` FOREIGN KEY (`CategoryId`) REFERENCES `category` (`Id`),
  CONSTRAINT `FK_Company` FOREIGN KEY (`CompanyId`) REFERENCES `companies` (`Id`),
  CONSTRAINT `FK_MOU` FOREIGN KEY (`UnitOfMeasure`) REFERENCES `measuringunits` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2829 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `purchasechequepaymentdetails`
--

DROP TABLE IF EXISTS `purchasechequepaymentdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `purchasechequepaymentdetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `PurchasePaymentId` mediumint(9) DEFAULT NULL,
  `PurchaseBillId` mediumint(9) DEFAULT NULL,
  `ChequeNo` int(11) DEFAULT NULL,
  `ChequeDate` datetime DEFAULT NULL,
  `IsChequeRealised` bit(1) DEFAULT NULL,
  `PaymentDate` datetime DEFAULT NULL,
  `ChequeName` varchar(50) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Purchase_Bill_idx` (`PurchaseBillId`),
  KEY `purchase_Payments_idx` (`PurchasePaymentId`),
  CONSTRAINT `Purchase_Bill` FOREIGN KEY (`PurchaseBillId`) REFERENCES `purchases` (`BillId`),
  CONSTRAINT `pur_pay_det` FOREIGN KEY (`PurchasePaymentId`) REFERENCES `purchasepaymentdetails` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `purchasedetails`
--

DROP TABLE IF EXISTS `purchasedetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `purchasedetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BillId` mediumint(9) NOT NULL,
  `ProductId` mediumint(9) DEFAULT NULL,
  `PriceId` mediumint(9) DEFAULT NULL,
  `PurchasedQty` decimal(10,2) DEFAULT NULL,
  `ActualPrice` decimal(10,2) NOT NULL,
  `Discount` decimal(10,2) DEFAULT NULL,
  `Tax` decimal(10,2) DEFAULT NULL,
  `ItemCoolieCharges` decimal(10,2) DEFAULT NULL,
  `ItemTransportCharges` decimal(10,2) DEFAULT NULL,
  `VATAmount` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_PurchaseBillId_idx` (`BillId`),
  KEY `FK_ProductId_idx` (`ProductId`),
  KEY `FK_price_purchase_idx` (`PriceId`),
  CONSTRAINT `FK_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `FK_PurchaseBillId` FOREIGN KEY (`BillId`) REFERENCES `purchases` (`BillId`),
  CONSTRAINT `FK_price_purchase` FOREIGN KEY (`PriceId`) REFERENCES `pricedetails` (`PriceId`)
) ENGINE=InnoDB AUTO_INCREMENT=34830 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `purchasefreedetails`
--

DROP TABLE IF EXISTS `purchasefreedetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `purchasefreedetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` mediumint(9) NOT NULL,
  `FreeQty` decimal(10,2) DEFAULT NULL,
  `FreeAmount` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  `BillId` mediumint(9) NOT NULL,
  `IsFreeOnly` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`,`ProductId`,`BillId`),
  KEY `FK_Product_idx` (`ProductId`),
  KEY `FK_ProductFree_idx` (`ProductId`),
  KEY `FK_pdt_bill_idx` (`BillId`),
  CONSTRAINT `FK_ProductFree` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `FK_pdt_bill` FOREIGN KEY (`BillId`) REFERENCES `purchases` (`BillId`)
) ENGINE=InnoDB AUTO_INCREMENT=2347 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `purchasepaymentdetails`
--

DROP TABLE IF EXISTS `purchasepaymentdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `purchasepaymentdetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `PurchaseBillId` mediumint(9) NOT NULL,
  `CompanyId` mediumint(9) NOT NULL,
  `AmountPaid` decimal(10,2) NOT NULL,
  `PaymentDate` datetime DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_compId_idx` (`CompanyId`),
  KEY `FK_purchaseId_idx` (`PurchaseBillId`),
  KEY `FK_purchaseIdDetails_idx` (`PurchaseBillId`),
  KEY `FK_purchaseIdDetail1s_idx` (`PurchaseBillId`),
  CONSTRAINT `FK_compId` FOREIGN KEY (`CompanyId`) REFERENCES `companies` (`Id`),
  CONSTRAINT `FK_purchaseId` FOREIGN KEY (`PurchaseBillId`) REFERENCES `purchases` (`BillId`)
) ENGINE=InnoDB AUTO_INCREMENT=9296 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `purchasereturn`
--

DROP TABLE IF EXISTS `purchasereturn`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `purchasereturn` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BillId` int(11) DEFAULT NULL,
  `ProductId` mediumint(9) NOT NULL,
  `Quantity` decimal(10,2) NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `ReturnPrice` decimal(10,2) DEFAULT NULL,
  `ReturnReasonCode` int(11) DEFAULT NULL,
  `MarkedForReturn` bit(1) DEFAULT NULL,
  `ExpiryDate` datetime DEFAULT NULL,
  `comments` varchar(200) DEFAULT NULL,
  `CreatedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_ProductIdPR_idx` (`ProductId`),
  KEY `FK_returnCodePR_idx` (`ReturnReasonCode`),
  CONSTRAINT `FK_ProductId_PR` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `FK_returnCode_PR` FOREIGN KEY (`ReturnReasonCode`) REFERENCES `codemaster` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1010 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `purchases`
--

DROP TABLE IF EXISTS `purchases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `purchases` (
  `BillId` mediumint(9) NOT NULL AUTO_INCREMENT,
  `RunningBillNo` int(11) DEFAULT NULL,
  `CompanyId` mediumint(9) NOT NULL,
  `InvoiceNo` varchar(45) DEFAULT NULL,
  `Discount` decimal(10,2) DEFAULT NULL,
  `SpecialDiscount` decimal(10,2) DEFAULT NULL,
  `TotalBillAmount` decimal(10,2) DEFAULT NULL,
  `Tax` decimal(10,2) DEFAULT NULL,
  `PaymentMode` char(1) DEFAULT NULL,
  `TransportCharges` decimal(10,2) DEFAULT NULL,
  `CoolieCharges` decimal(10,2) DEFAULT NULL,
  `KCoolieCharges` decimal(10,2) DEFAULT NULL,
  `LocalCoolieCharges` decimal(10,2) DEFAULT NULL,
  `IsCancelled` bit(1) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`BillId`),
  KEY `FK_CompanyId_idx` (`CompanyId`),
  CONSTRAINT `FK_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `companies` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9260 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `returndamagedstocks`
--

DROP TABLE IF EXISTS `returndamagedstocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `returndamagedstocks` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BillId` int(11) DEFAULT NULL,
  `ProductId` mediumint(9) NOT NULL,
  `Quantity` decimal(10,2) NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `comments` varchar(200) DEFAULT NULL,
  `isReturn` tinyint(1) NOT NULL,
  `CreatedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ReturnReasonCode` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProductId_idx` (`ProductId`),
  KEY `FK_ProductIdRD_idx` (`ProductId`),
  KEY `FK_returnCode_idx` (`ReturnReasonCode`),
  CONSTRAINT `FK_ProductId_RD` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `FK_returnCode` FOREIGN KEY (`ReturnReasonCode`) REFERENCES `codemaster` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `roles` (
  `Id` int(11) NOT NULL,
  `RoleName` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `saledetails`
--

DROP TABLE IF EXISTS `saledetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `saledetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BillId` mediumint(9) NOT NULL,
  `ProductId` mediumint(9) NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `SellingPrice` decimal(10,2) DEFAULT NULL,
  `CostPrice` decimal(10,2) DEFAULT NULL,
  `Qty` decimal(10,3) DEFAULT NULL,
  `Discount` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Fk_sale_bill_idx` (`BillId`),
  KEY `FK_ProdId_idx` (`ProductId`),
  KEY `FK_price_id_idx` (`PriceId`),
  CONSTRAINT `FK_ProdId` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `FK_price_id` FOREIGN KEY (`PriceId`) REFERENCES `pricedetails` (`PriceId`),
  CONSTRAINT `Fk_sale_bill` FOREIGN KEY (`BillId`) REFERENCES `sales` (`BillId`)
) ENGINE=InnoDB AUTO_INCREMENT=459469 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sales`
--

DROP TABLE IF EXISTS `sales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `sales` (
  `BillId` mediumint(9) NOT NULL AUTO_INCREMENT,
  `CustomerId` mediumint(9) NOT NULL,
  `Discount` decimal(10,2) DEFAULT NULL,
  `TransportCharges` decimal(10,2) DEFAULT NULL,
  `AmountPaid` decimal(10,2) DEFAULT NULL,
  `TotalAmount` decimal(10,2) DEFAULT NULL,
  `IsCancelled` bit(1) DEFAULT NULL,
  `CustomerOrderNo` varchar(30) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  `RunningBillNo` mediumint(9) NOT NULL,
  `PaymentMode` char(1) NOT NULL,
  PRIMARY KEY (`BillId`),
  KEY `FK_Sales_Cust_idx` (`CustomerId`),
  CONSTRAINT `FK_Sales_Cust` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`Id`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=81255 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `saletemp`
--

DROP TABLE IF EXISTS `saletemp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `saletemp` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Guid` varchar(200) NOT NULL,
  `SaleDate` datetime DEFAULT NULL,
  `CustomerId` int(11) DEFAULT NULL,
  `PaymentMode` varchar(6) DEFAULT NULL,
  `OrderNo` varchar(45) DEFAULT NULL,
  `ProductId` int(11) DEFAULT NULL,
  `Quantity` decimal(10,2) DEFAULT NULL,
  `SellingPrice` decimal(10,2) DEFAULT NULL,
  `DiscountPercentage` decimal(10,2) DEFAULT NULL,
  `DiscountAmount` decimal(10,2) DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `PriceId` int(11) DEFAULT NULL,
  `CreatedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`Guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Temproary	 table to save sale details	';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stockadjustments`
--

DROP TABLE IF EXISTS `stockadjustments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `stockadjustments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `StockId` mediumint(9) DEFAULT NULL,
  `OpeningBalance` decimal(10,2) DEFAULT NULL,
  `AdjustedQty` decimal(10,2) DEFAULT NULL,
  `ClosingBalance` decimal(10,2) DEFAULT NULL,
  `CostPrice` decimal(10,2) DEFAULT NULL,
  `StockTransId` int(11) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_STKADJ_STRANS_idx` (`StockTransId`),
  CONSTRAINT `FK_STKADJ_STRANS` FOREIGN KEY (`StockTransId`) REFERENCES `stocktransaction` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2696 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stocks`
--

DROP TABLE IF EXISTS `stocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `stocks` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` mediumint(9) NOT NULL,
  `Quantity` decimal(10,2) NOT NULL,
  `ExpiryDate` date NOT NULL,
  `PriceId` mediumint(9) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_PrdId_idx` (`ProductId`),
  KEY `FK_priceId_idx` (`PriceId`),
  CONSTRAINT `FK_PrdId` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `FK_priceId` FOREIGN KEY (`PriceId`) REFERENCES `pricedetails` (`PriceId`)
) ENGINE=InnoDB AUTO_INCREMENT=4292 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stocktransaction`
--

DROP TABLE IF EXISTS `stocktransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `stocktransaction` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `StockId` int(11) DEFAULT NULL,
  `OpeningBalance` decimal(10,2) DEFAULT NULL,
  `Inward` decimal(10,2) DEFAULT NULL,
  `Outward` decimal(10,2) DEFAULT NULL,
  `ClosingBalance` decimal(10,2) DEFAULT NULL,
  `SalesPurchaseCancelQty` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `fk_stockId_idx` (`StockId`),
  CONSTRAINT `fk_stockId` FOREIGN KEY (`StockId`) REFERENCES `stocks` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=161487 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `swapdetails`
--

DROP TABLE IF EXISTS `swapdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `swapdetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `SwapId` mediumint(9) DEFAULT NULL,
  `StockId` int(11) DEFAULT NULL,
  `Quantity` decimal(10,0) DEFAULT NULL,
  `SellingPrice` decimal(10,0) DEFAULT NULL,
  `CostPrice` decimal(10,0) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_SWAP_MAIN_idx` (`SwapId`),
  CONSTRAINT `FK_SWAP_MAIN` FOREIGN KEY (`SwapId`) REFERENCES `swaps` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `swaps`
--

DROP TABLE IF EXISTS `swaps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `swaps` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `CustomerId` mediumint(9) DEFAULT NULL,
  `LendOrBorrowId` int(11) DEFAULT NULL,
  `TotalAmount` decimal(10,0) DEFAULT NULL,
  `AddedOn` datetime DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `systemdata`
--

DROP TABLE IF EXISTS `systemdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `systemdata` (
  `Id` int(11) NOT NULL,
  `SysDate` date DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `users` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` varchar(20) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_role_idx` (`RoleId`),
  CONSTRAINT `fk_role` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'rms'
--

--
-- Dumping routines for database 'rms'
--
/*!50003 DROP FUNCTION IF EXISTS `GetbillIdForCompanies` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` FUNCTION `GetbillIdForCompanies`(runningBillNo integer,category integer) RETURNS int(11)
    DETERMINISTIC
BEGIN

declare billidValue int;

SELECT 
    billId
INTO billidValue FROM
    purchases p
WHERE
    p.RunningBillNo = runningBillNo
        AND p.CompanyId IN (SELECT 
            Id
        FROM
            companies c
        WHERE
            c.CategoryTypeId = Category);

RETURN billidValue;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `GetbillIdForCustomers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` FUNCTION `GetbillIdForCustomers`(runningBillNo integer,category integer) RETURNS int(11)
    DETERMINISTIC
BEGIN

declare billidValue int;

SELECT 
    billId
INTO billidValue FROM
    sales s
WHERE
    s.RunningBillNo = runningBillNo
        AND s.customerid IN (SELECT 
            Id
        FROM
            CUSTOMERS c
        WHERE
            c.CustomerTypeId = Category);

RETURN billidValue;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `GetSysDate` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` FUNCTION `GetSysDate`() RETURNS datetime
    DETERMINISTIC
BEGIN

declare sysDateTime datetime;

SELECT 
    sysdate()
INTO sysDateTime FROM
    dual;

RETURN sysDateTime;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetCurrentStock` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCurrentStock`(in categoryId int,in productId int, in companyId int)
    DETERMINISTIC
BEGIN

if(categoryId = 0 && productId = 0 && companyId = 0)
then
	select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id;
elseif (categoryId !=0 && productId = 0 && companyId = 0)
then
	select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id
    and c.Id = categoryId;
    
elseif (categoryId =0 && productId != 0 && companyId = 0)
then
		select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id
    and p.id = productId;
    
elseif (categoryId =0 && productId = 0 && companyId != 0)
then
		select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id
    and cp.id = companyId;
end if;    

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetCustomerBalance` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerBalance`(in fromDate datetime,in toDate datetime,in customerId int)
    DETERMINISTIC
BEGIN
select sum(BalAmount) BalanceAmount from
(
	SELECT 
		s.TotalAmount - sum(pd.AmountPaid) BalAmount
	FROM
		rms.PaymentDetails pd  Join sales s on (s.BillId = pd.BillId AND s.customerId = pd.customerid AND s.PaymentMode!=0)
							   left join ChequePaymentDetails cpd on cpd.PaymentId = pd.Id
	WHERE
			pd.customerId = customerId
			AND IFNULL(s.IsCancelled, 0) = 0
			AND ((fromDate is null) or  date(s.AddedOn) >= fromDate)
			AND ((toDate is null) or  date(s.AddedOn) <= toDate)
		
	GROUP BY s.RunningBillNo
	ORDER BY s.RunningBillNo
) as Bal;

-- select fromdate,toDate;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetCustomerPaymentDetails` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerPaymentDetails`(in customerId int)
    DETERMINISTIC
BEGIN
SELECT 
    s.billid,
    s.RunningBillNo,
    SUM(pd.AmountPaid) AmountPaid,
    (s.TotalAmount - SUM(pd.AmountPaid)) BalanceAmount,
    s.TotalAmount,
    s.AddedOn,
    s.PaymentMode
FROM
    rms.PaymentDetails pd,
    sales s
WHERE
    s.BillId = pd.BillId
        AND s.customerId = pd.customerid
        AND pd.customerId = customerId
        AND IFNULL(s.IsCancelled, 0) = 0
        AND s.PaymentMode = 1
GROUP BY s.RunningBillNo
HAVING AmountPaid != s.TotalAmount
ORDER BY s.RunningBillNo;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetCustomerPaymentDetailsReport` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerPaymentDetailsReport`(in fromDate datetime,in toDate datetime,in customerId int)
    DETERMINISTIC
BEGIN
SELECT 
    s.RunningBillNo,
    pd.BillId,
    s.AddedOn BillDate,
    s.TotalAmount,
    pd.AmountPaid,
    (s.TotalAmount - pd.AmountPaid) BalanceAmount,
    pd.PaymentDate PaymentDate,
	/*case when cm.Id = 7 then 'Ca'
		when  cm.Id = 9 then 'Ch' end 'Description',*/
    cpd.ChequeNo,
    cpd.ChequeDate,
    case when ifnull(IsChequeRealised,0) != 0 and cpd.IsChequeRealised = 1 then 'Y' end 'Chq.Realised'
FROM
    rms.PaymentDetails pd  Join sales s on (s.BillId = pd.BillId AND s.customerId = pd.customerid AND s.PaymentMode!=0)
						   -- left Join CodeMaster cm on pd.PaymentMode = cm.Id 
						   left join ChequePaymentDetails cpd on cpd.PaymentId = pd.Id
WHERE
        pd.customerId = customerId
        AND IFNULL(s.IsCancelled, 0) = 0
        AND date(s.AddedOn) >= fromDate
        AND date(s.AddedOn) <= toDate
ORDER BY s.RunningBillNo;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetCustomerWiseSales` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerWiseSales`(in fromDate DateTime, in toDate Datetime,in customerId int)
BEGIN
select s.*,c.Name from sales s,customers c
where s.customerId = c.id
and c.id = customerId
-- and s.paymentMode= 1 
and ifnull(s.IsCancelled,1) = 1
and Date(s.addedOn) >= fromDate and Date(s.addedOn) <= toDate;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetProductPriceForPriceId` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetProductPriceForPriceId`(IN priceId integer)
    DETERMINISTIC
BEGIN

SELECT 
    p.Id AS 'ProductId',
    p.Name AS 'ProductName',
    pd.Price AS 'Price',
    pd.SellingPrice AS 'SellingPrice',
    st.Quantity AS 'Quantity',
    pd.PriceId AS 'PriceId',
    DATE_FORMAT(st.ExpiryDate, '%d/%m/%Y') AS 'ExpiryDate'
FROM
    Products p,
    PriceDetails pd,
    Stocks st
WHERE
    p.Id = pd.ProductId
        AND pd.PriceId = st.PriceId
        AND pd.PriceId = priceId
        ORDER BY ProductName;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetProductsToOrderForProductIds` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetProductsToOrderForProductIds`(in productsIn varchar(255))
    DETERMINISTIC
BEGIN
    SELECT 
    p.Name,
    SUM(st.Quantity) 'Available Qty',
    c.name 'CategoryName',
    p.ReorderPoint
FROM
    stocks st,
    Products p,
    Category c 
WHERE
	FIND_IN_SET(p.id, productsIn)
    AND st.ProductId = p.id
    AND p.CategoryId = c.Id
    AND p.IsActive = 1
GROUP BY c.id , st.ProductId , p.ReorderPoint
HAVING SUM(st.Quantity) <= p.ReorderPoint
ORDER BY c.name , p.Name;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetPurchaseDetails` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchaseDetails`(IN runningBillNo integer, IN category integer)
    DETERMINISTIC
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCompanies(runningBillNo,category); 

SELECT 
    prod.Name,
    pr.Price,
	pfd.freeqty as FreeIssue,
    pr.SellingPrice,
    purchdet.PurchasedQty,
    purchdet.ActualPrice,
    purchdet.Discount ItemDiscount,
    purchdet.ItemCoolieCharges Coolie,
    purchdet.ItemTransportCharges Trans
FROM purchasedetails purchdet Join PriceDetails pr on purchdet.PriceId = pr.PriceId
							  Join Products prod on  purchdet.ProductId = prod.Id
                              left join PurchaseFreeDetails pfd on purchdet.BillId = pfd.BillId and pfd.ProductId = prod.Id
WHERE
	purchdet.BillId = billidValue
	order by purchdet.Id asc;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetPurchasePaymentDetails` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchasePaymentDetails`(in paramCompanyId int)
    DETERMINISTIC
BEGIN
SELECT 
    p.billid,
    p.RunningBillNo,
    SUM(pd.AmountPaid) AmountPaid,
    (p.TotalBillAmount - SUM(pd.AmountPaid)) BalanceAmount,
    p.TotalBillAmount,
    p.AddedOn,
    p.PaymentMode
FROM
    rms.PurchasePaymentDetails pd,
    Purchases p
WHERE
    p.BillId = pd.PurchaseBillId
        AND p.companyId = pd.companyId
        AND p.companyId  = paramCompanyId
        AND IFNULL(p.IsCancelled, 0) = 0
        AND p.PaymentMode = 1
GROUP BY p.RunningBillNo
HAVING AmountPaid != p.TotalBillAmount
ORDER BY p.RunningBillNo;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetPurchaseReturnForCompany` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchaseReturnForCompany`(IN filterCompanyId int )
    DETERMINISTIC
BEGIN

select pr.* from purchaseReturn pr, products p
where pr.ProductId = p.Id
and p.CompanyId = filterCompanyId
and pr.MarkedForReturn = true
and pr.ReturnReasonCode != 3;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetPurchases` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchases`(IN runningBillNo integer, IN category integer)
    DETERMINISTIC
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCompanies(runningBillNo,category);  

SELECT 
    p.addedon,
    p.BillId,
    InvoiceNo,
    p.Discount,
    p.cooliecharges cooliecharges,
    p.kcooliecharges KcoolieCharges,
    p.transportcharges trannsportcharges,
    p.localCoolieCharges localCoolieCharges,
    p.SpecialDiscount,
    p.TotalBillAmount,
    p.IsCancelled,
    c.Name Supplier,
    c.CategoryTypeId,
    CASE
        WHEN c.CategoryTypeId = '11' THEN concat('C', p.RunningBillNo)
        ELSE p.RunningBillNo
    END AS 'RunningBillNo'
from Purchases p, companies c
where p.CompanyId = c.Id
and p.BillId = billidValue;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetReturnsForBillid` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetReturnsForBillid`(IN runningBillNo integer, IN category integer)
    DETERMINISTIC
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCompanies(runningBillNo,category);

select pr.*,p.Name,cm.Description,(pr.ReturnPrice * pr.Quantity) ReturnAmount  
from purchasereturn pr, products p,PriceDetails pd, CodeMaster cm
where pr.productid = p.id
and pd.PriceId = pr.PriceId
and cm.code = "RTN" and pr.billId = billidValue
and cm.Id = pr.ReturnReasonCode;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetSales` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSales`(IN fromSalesDate Date, IN toSalesDate date,IN categoryId int,in internalUserId int)
BEGIN
select s.BillId,s.AddedOn,C.Name as Customer,CustomerOrderNo,
CASE
        WHEN s.TransportCharges= 0.00 THEN null
        ELSE s.TransportCharges
    END AS 'TransportCharges',
	CASE
        WHEN s.isCancelled =1 THEN 'Cancelled'
        ELSE NULL
    END AS 'Cancelled',
RunningBillNo,s.addedOn,
sum(sd.Discount) + if(isnull(s.discount),0,s.discount) Discount,
(sum(sd.sellingprice *sd.qty) - (sum(if(isnull(sd.discount),0,sd.discount)) + if(isnull(s.discount),0,s.discount))) + s.TransportCharges TotalAmount,
CASE
        WHEN s.PaymentMode = '0' AND (isnull(s.AmountPaid) = 1 OR s.AmountPaid = 0) THEN 
					(sum(sd.sellingprice *sd.qty) - (sum(if(isnull(sd.discount),0,sd.discount)) + if(isnull(s.discount),0,s.discount)))
					+ s.TransportCharges
        ELSE 
			s.AmountPaid
    END AS 'Cash Sales',
    CASE
        WHEN PaymentMode = '1'  THEN 
				(sum(sd.sellingprice *sd.qty) - (sum(if(isnull(sd.discount),0,sd.discount)) + if(isnull(s.discount),0,s.discount))) 
                -(if(isnull(s.AmountPaid),0,s.AmountPaid)) + s.TransportCharges 
        ELSE NULL
    END AS 'Credit Sales'
from sales s,Customers c, SaleDetails sd
where s.CustomerId = c.Id
and sd.BillId = s.BillId
and c.CustomerTypeId = categoryId
and -- Date(s.addedOn) >= fromSalesDate and Date(s.addedOn) <= toSalesDate and
(s.RunningBillNo > 
	(select EndBillNo from DateBillMapping 
	where id = (select id from DateBillMapping where date(EndOfDate) < fromSalesDate and CustomerTypeId = categoryId order by date(EndOfDate) desc LIMIT 1 
	))
and 
s.RunningBillNo <= (select EndBillNo from DateBillMapping where date(EndOfDate) = toSalesDate and CustomerTypeId = categoryId)
)
and (if(isnull(s.IsCancelled),0,s.IsCancelled)) = 0 
and s.UpdatedBy = if(internalUserId = 0,s.UpdatedBy,internalUserId)
group by s.billId
order by s.RunningBillNo 


/*union

select ModifiedOn, (select Name as 'Name' from customers where Id = (select customerId from sales sa where sa.BillId =rs.billid)) as Customer,
'','','Return',(select RunningBillNo from sales sa where sa.BillId =rs.billid) RunningBillNo,rs.CreatedOn,'','','', 
-rs.Quantity * (select sellingPrice from PriceDetails where PriceId = rs.PriceId) TotalAmount
from ReturnDamagedStocks rs 
where Date(rs.ModifiedOn) >= fromSalesDate and Date(rs.ModifiedOn) <= toSalesDate*/ ;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetSalesDetailsForBillId` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSalesDetailsForBillId`(IN runningBillNo integer, IN category integer)
    DETERMINISTIC
BEGIN
declare billidValue int;
SET  billidValue = GetbillIdForCustomers(runningBillNo,category);
SELECT 
    s.AddedOn,    
    c.Name Customer,
    CustomerOrderNo,
    TransportCharges,
    RunningBillNo,
    s.Discount,
    CASE
        WHEN s.PaymentMode = '0' THEN 'Cash'
        ELSE 'Credit'
    END AS PaymentMode,
    TotalAmount,
    sd.Discount AS ItemDiscount,
    price AS Price,
    p.Name AS ProductName,
    sd.Qty,
    sd.SellingPrice,sd.productId
FROM
    sales s inner join SaleDetails sd on (s.BillId = sd.BillId and s.billId = billidValue)
			inner Join Customers c on c.id = s.customerId
            inner join pricedetails pd on (sd.PriceId = pd.PriceId )
            inner join Products p on (sd.ProductId = p.Id)
order by sd.id;        
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetSalesGraphReport` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSalesGraphReport`()
    DETERMINISTIC
BEGIN
DECLARE CreditSalesCursor CURSOR FOR
	(SELECT SUM(totalamount) TotalAmount, DATE_FORMAT(s.addedOn, '%b-%y') SalesYearMonth
	FROM sales s,Customers c
	WHERE  paymentMode = 1
    and s.CustomerId = c.id 
	and c.CustomerTypeId !=7 
	GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn));

DECLARE HotelSalesCursor CURSOR FOR
	(SELECT SUM(totalamount) TotalAmount, DATE_FORMAT(s.addedOn, '%b-%y') SalesYearMonth
	FROM sales s,Customers c
	WHERE  paymentMode = 1
    and s.CustomerId = c.id 
	and c.CustomerTypeId =7 
	GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn));

Drop table IF exists `SalesReportTbl`;

Create temporary table SalesReportTbl
(SELECT SUM(totalamount) CashSales, DATE_FORMAT(addedOn, '%b-%y') SalesYearMonth,0 CreditSales, 0 HotelSales,0 TotalSales
	FROM sales
	WHERE  paymentMode = 0
	GROUP BY YEAR(AddedOn) , MONTH(AddedOn)
);

open CreditSalesCursor;
Begin
		DECLARE exit_flag INT DEFAULT 0;			
		DECLARE amt decimal;
		Declare yearMonth varchar(20); 
		DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET exit_flag = 1;
        
get_creditLoop: LOOP
	FETCH CreditSalesCursor INTO amt,yearMonth;
		IF exit_flag THEN 
			LEAVE get_creditLoop;
		END IF;
		update SalesReportTbl set CreditSales = amt where SalesYearMonth = yearMonth;	
	END LOOP get_creditLoop;
End;
close CreditSalesCursor;

open HotelSalesCursor;
Begin
		DECLARE exit_flag INT DEFAULT 0;			
		DECLARE hotelAmt decimal;
		Declare hotelYearMonth varchar(20); 
	    DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET exit_flag = 1;

get_hotelsLoop: LOOP
	FETCH HotelSalesCursor INTO hotelAmt,hotelYearMonth;
		IF exit_flag THEN 
			LEAVE get_hotelsLoop;
		END IF;
		update SalesReportTbl set HotelSales = hotelAmt where SalesYearMonth = hotelYearMonth;	
	END LOOP get_hotelsLoop;
End;
Close  HotelSalesCursor;   
            
update SalesReportTbl stb set TotalSales = stb.CashSales + stb.CreditSales + stb.HotelSales;

select * from SalesReportTbl;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetStockBalances` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetStockBalances`(in categoryId int,in productId int, in companyId int)
BEGIN 

if(categoryId = 0 and productId = 0 and companyId = 0)
then
    select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity,pr.Price CostPrice, sum(s.quantity) * pr.Price Value
	from products p Join category c on c.Id = p.CategoryId
                    Join Stocks s on s.ProductId = p.Id and s.Id = (select st.Id from stocks st where st.productId = p.id order by st.ModifiedOn Desc limit 1)
                    join pricedetails pr on pr.PriceId = s.PriceId
                    Join Companies cp on cp.id = p.CompanyId
	where p.IsActive = true
	group by p.id;					
elseif (categoryId !=0 and productId = 0 and companyId = 0)
then
 select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity,pr.Price CostPrice, sum(s.quantity) * pr.Price Value
	from products p Join category c on c.Id = p.CategoryId and c.Id = categoryId
                    Join Stocks s on s.ProductId = p.Id and s.Id = (select st.Id from stocks st where st.productId = p.id order by st.ModifiedOn Desc limit 1)
                    join pricedetails pr on pr.PriceId = s.PriceId
                    Join Companies cp on cp.id = p.CompanyId
	where p.IsActive = true
    group by p.id;		
    
elseif (categoryId =0 and productId != 0 and companyId = 0)
then
	select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity,pr.Price CostPrice, sum(s.quantity) * pr.Price Value
	from products p Join category c on c.Id = p.CategoryId 
                    Join Stocks s on s.ProductId = p.Id and s.Id = (select st.Id from stocks st where st.productId = p.id order by st.ModifiedOn Desc limit 1)
                    join pricedetails pr on pr.PriceId = s.PriceId
                    Join Companies cp on cp.id = p.CompanyId
    where p.Id = productId and p.IsActive = true               
	group by p.id;	
elseif (categoryId =0 and productId = 0 and companyId != 0)
then
	select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity,pr.Price CostPrice, sum(s.quantity) * pr.Price Value
	from products p Join category c on c.Id = p.CategoryId 
                    Join Stocks s on s.ProductId = p.Id and s.Id = (select st.Id from stocks st where st.productId = p.id order by st.ModifiedOn Desc limit 1)
                    join pricedetails pr on pr.PriceId = s.PriceId
                    Join Companies cp on cp.id = p.CompanyId and cp.Id = companyId            
    where p.IsActive = true
	group by p.id;	
end if;    

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetStockDetails` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetStockDetails`(in categoryId int,in productId int, in companyId int, 
													 in fromDate date, in ToDate date)
BEGIN

if(categoryId = 0 and productId = 0 and companyId = 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,COALESCE(st.OpeningBalance,0) OpeningBal,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId
					Join Companies cp on cp.Id = p.CompanyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate;
elseif (categoryId =0 and productId != 0 and companyId = 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,COALESCE(st.OpeningBalance,0) OpeningBal,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty,
    sa.StockTransId
	from StockTransaction st Join Stocks s on st.stockId = s.id and date(st.addedOn) between fromDate and toDate
							 Join Products p on s.ProductId = p.Id and p.Id = ProductId
                             Join PriceDetails pd on pd.ProductId = p.Id and s.PriceId = pd.PriceId 
							 Join category c on c.Id = p.CategoryId
							 Join Companies cp on cp.Id = p.CompanyId							 							 
							 left join StockAdjustments sa on sa.StockId = s.Id	and sa.StockTransId = st.Id;
							
elseif (categoryId !=0 and productId = 0 and companyId = 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,COALESCE(st.OpeningBalance,0) OpeningBal,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId and c.id = categoryId
					Join Companies cp on cp.Id = p.CompanyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate;		
    

elseif (categoryId =0 and productId = 0 and companyId != 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,COALESCE(st.OpeningBalance,0) OpeningBal,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId
					Join Companies cp on cp.Id = p.CompanyId and cp.Id = companyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate;
end if;    

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `MarkEndOfDay` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `MarkEndOfDay`()
BEGIN
declare systemDateTime datetime; 
declare maxRunningBillNo int; 

set systemDateTime = (select GetSysDate()); 

select ifnull(max(s.RunningBillNo),0)  into maxRunningBillNo
from sales s
where date(s.AddedOn) = date(systemDateTime) and
s.CustomerId in (select Id from customers c where c.CustomerTypeId = 7);

call MarkEndOfDayOnCustomerType(systemDateTime, maxRunningBillNo,7);

select ifnull(max(s.RunningBillNo),0)  into maxRunningBillNo
from sales s
where date(s.AddedOn) = date(systemDateTime) and
s.CustomerId in (select Id from customers c where c.CustomerTypeId != 7);

call MarkEndOfDayOnCustomerType(systemDateTime, maxRunningBillNo,9);
 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `MarkEndOfDayOnCustomerType` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`RMS`@`%` PROCEDURE `MarkEndOfDayOnCustomerType`(in systemDateTime datetime,
 in maxRunningBillNo int,in customerTypeId int)
BEGIN
declare recordCount int;

select count(*) into recordCount
from DateBillMapping d 
where date(d.EndOfDate) = (select date(GetSysDate())) and d.CustomerTypeId = customerTypeId;

if(recordCount > 0 ) then 
	select 1;
else
	insert DateBillMapping (EndOfDate,EndBillNo,customerTypeId) values (systemDateTime,maxRunningBillNo,customerTypeId);
	select 0;
end if;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-05-09 13:29:46

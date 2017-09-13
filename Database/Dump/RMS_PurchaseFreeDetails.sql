CREATE DATABASE  IF NOT EXISTS `RMS` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `RMS`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: RMS
-- ------------------------------------------------------
-- Server version	5.7.14-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `PurchaseFreeDetails`
--

DROP TABLE IF EXISTS `PurchaseFreeDetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `PurchaseFreeDetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` mediumint(9) NOT NULL,
  `FreeQty` decimal(10,2) DEFAULT NULL,
  `FreeAmount` decimal(10,2) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  `BillId` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Product_idx` (`ProductId`),
  KEY `FK_ProductFree_idx` (`ProductId`),
  KEY `FK_pdt_bill_idx` (`BillId`),
  CONSTRAINT `FK_ProductFree` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_pdt_bill` FOREIGN KEY (`BillId`) REFERENCES `PurchaseDetails` (`BillId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PurchaseFreeDetails`
--

LOCK TABLES `PurchaseFreeDetails` WRITE;
/*!40000 ALTER TABLE `PurchaseFreeDetails` DISABLE KEYS */;
INSERT INTO `PurchaseFreeDetails` (`Id`, `ProductId`, `FreeQty`, `FreeAmount`, `AddedOn`, `ModifiedOn`, `UpdatedBy`, `BillId`) VALUES (1,5919,1.00,125.00,'2017-07-29 17:09:43','2017-08-06 18:46:15',NULL,9);
/*!40000 ALTER TABLE `PurchaseFreeDetails` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-08-27 16:13:11

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
-- Table structure for table `Purchases`
--

DROP TABLE IF EXISTS `Purchases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Purchases` (
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
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`BillId`),
  KEY `FK_CompanyId_idx` (`CompanyId`),
  CONSTRAINT `FK_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Purchases`
--

LOCK TABLES `Purchases` WRITE;
/*!40000 ALTER TABLE `Purchases` DISABLE KEYS */;
INSERT INTO `Purchases` (`BillId`, `RunningBillNo`, `CompanyId`, `InvoiceNo`, `Discount`, `SpecialDiscount`, `TotalBillAmount`, `Tax`, `PaymentMode`, `TransportCharges`, `CoolieCharges`, `KCoolieCharges`, `LocalCoolieCharges`, `IsCancelled`, `AddedOn`, `ModifiedOn`, `UpdatedBy`) VALUES (6,1,5,'345',NULL,NULL,1350.00,NULL,'0',NULL,100.00,NULL,NULL,NULL,'2017-07-29 17:09:43','2017-08-02 18:59:04',NULL),(7,2,4,'456',NULL,NULL,1250.00,NULL,'0',NULL,NULL,NULL,NULL,NULL,'2017-07-29 17:24:01','2017-08-02 18:59:04',NULL),(8,3,4,'34',NULL,NULL,1250.00,NULL,'0',NULL,NULL,NULL,NULL,'','2017-07-29 17:28:44','2017-08-13 12:50:33',NULL),(9,4,4,'34',NULL,NULL,625.00,NULL,'0',NULL,NULL,NULL,NULL,NULL,'2017-07-29 17:30:39','2017-08-02 18:59:04',NULL),(11,6,4,'675',NULL,NULL,3750.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2017-08-08 18:03:58','2017-08-08 18:03:58',NULL),(12,7,5,'23',0.00,NULL,1625.00,NULL,'0',NULL,NULL,NULL,NULL,NULL,'2017-08-08 18:23:34','2017-08-13 12:13:38',NULL);
/*!40000 ALTER TABLE `Purchases` ENABLE KEYS */;
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

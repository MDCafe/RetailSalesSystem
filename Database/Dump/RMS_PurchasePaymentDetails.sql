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
-- Table structure for table `PurchasePaymentDetails`
--

DROP TABLE IF EXISTS `PurchasePaymentDetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `PurchasePaymentDetails` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `PurchaseBillId` mediumint(9) NOT NULL,
  `CompanyId` mediumint(9) NOT NULL,
  `AmountPaid` decimal(10,2) NOT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_compId_idx` (`CompanyId`),
  KEY `FK_purchaseId_idx` (`PurchaseBillId`),
  KEY `FK_purchaseIdDetails_idx` (`PurchaseBillId`),
  KEY `FK_purchaseIdDetail1s_idx` (`PurchaseBillId`),
  CONSTRAINT `FK_compId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_purchaseId` FOREIGN KEY (`PurchaseBillId`) REFERENCES `Purchases` (`BillId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PurchasePaymentDetails`
--

LOCK TABLES `PurchasePaymentDetails` WRITE;
/*!40000 ALTER TABLE `PurchasePaymentDetails` DISABLE KEYS */;
INSERT INTO `PurchasePaymentDetails` (`id`, `PurchaseBillId`, `CompanyId`, `AmountPaid`, `AddedOn`, `ModifiedOn`, `UpdatedBy`) VALUES (1,6,5,350.00,'2017-07-29 17:09:43','2017-07-29 17:09:43',NULL),(2,7,4,250.00,'2017-07-29 17:24:02','2017-07-29 17:24:02',NULL),(3,8,4,250.00,'2017-07-29 17:28:44','2017-07-29 17:28:44',NULL),(5,11,4,0.00,'2017-08-08 18:03:58','2017-08-08 18:03:58',NULL),(6,12,5,0.00,'2017-08-08 18:23:34','2017-08-08 18:23:34',NULL);
/*!40000 ALTER TABLE `PurchasePaymentDetails` ENABLE KEYS */;
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

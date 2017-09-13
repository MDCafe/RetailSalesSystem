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
-- Table structure for table `PaymentDetails`
--

DROP TABLE IF EXISTS `PaymentDetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `PaymentDetails` (
  `Id` mediumint(9) NOT NULL AUTO_INCREMENT,
  `BillId` mediumint(9) NOT NULL,
  `CustomerId` mediumint(9) NOT NULL,
  `AmountPaid` decimal(10,2) NOT NULL,
  `PaymentMode` varchar(6) DEFAULT NULL,
  `AddedOn` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdatedBy` mediumint(9) DEFAULT NULL,
  PRIMARY KEY (`Id`,`BillId`,`CustomerId`),
  KEY `FK_customerId_idx` (`CustomerId`),
  KEY `FK_pay_bill_idx` (`BillId`),
  CONSTRAINT `FK_customerId` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_pay_bill` FOREIGN KEY (`BillId`) REFERENCES `Sales` (`BillId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PaymentDetails`
--

LOCK TABLES `PaymentDetails` WRITE;
/*!40000 ALTER TABLE `PaymentDetails` DISABLE KEYS */;
INSERT INTO `PaymentDetails` (`Id`, `BillId`, `CustomerId`, `AmountPaid`, `PaymentMode`, `AddedOn`, `ModifiedOn`, `UpdatedBy`) VALUES (1,9,142,0.00,NULL,'2017-08-11 15:01:05','2017-08-11 15:01:05',NULL),(2,10,137,0.00,NULL,'2017-08-11 15:05:20','2017-08-11 15:05:20',NULL),(3,12,140,0.00,NULL,'2017-08-11 15:17:47','2017-08-11 15:17:47',NULL),(4,14,142,0.00,NULL,'2017-08-11 15:32:24','2017-08-11 15:32:24',NULL),(5,15,137,0.00,NULL,'2017-08-11 15:36:37','2017-08-11 15:36:37',NULL),(6,17,141,0.00,NULL,'2017-08-11 15:42:26','2017-08-11 15:42:26',NULL),(7,20,137,0.00,NULL,'2017-08-11 16:20:27','2017-08-11 16:20:27',NULL),(8,21,138,0.00,NULL,'2017-08-11 16:25:54','2017-08-11 16:25:54',NULL),(9,22,137,0.00,NULL,'2017-08-11 16:28:57','2017-08-11 16:28:57',NULL),(10,23,138,0.00,NULL,'2017-08-11 16:48:24','2017-08-11 16:48:24',NULL),(11,24,141,0.00,NULL,'2017-08-11 16:50:37','2017-08-11 16:50:37',NULL),(12,26,137,500.00,NULL,'2017-08-11 18:29:22','2017-08-11 18:29:22',NULL),(13,28,137,3000.00,NULL,'2017-08-11 18:54:33','2017-08-11 18:54:33',NULL),(14,30,140,0.00,NULL,'2017-08-11 18:59:40','2017-08-11 18:59:40',NULL),(15,31,137,0.00,NULL,'2017-08-11 19:06:11','2017-08-11 19:06:11',NULL),(16,33,137,0.00,NULL,'2017-08-11 19:16:21','2017-08-11 19:16:21',NULL),(17,34,137,0.00,NULL,'2017-08-11 19:20:01','2017-08-11 19:20:01',NULL),(18,35,138,0.00,NULL,'2017-08-11 19:23:54','2017-08-11 19:23:54',NULL),(19,36,139,0.00,NULL,'2017-08-11 19:30:40','2017-08-11 19:30:40',NULL),(20,37,139,0.00,NULL,'2017-08-11 19:36:52','2017-08-11 19:36:52',NULL),(21,38,138,0.00,NULL,'2017-08-11 19:44:50','2017-08-11 19:44:50',NULL),(22,39,137,0.00,NULL,'2017-08-11 19:46:55','2017-08-11 19:46:55',NULL),(23,40,139,0.00,NULL,'2017-08-11 19:48:43','2017-08-11 19:48:43',NULL),(24,41,138,0.00,NULL,'2017-08-11 19:51:53','2017-08-11 19:51:53',NULL),(25,42,138,0.00,NULL,'2017-08-11 20:04:56','2017-08-11 20:04:56',NULL),(26,43,137,0.00,NULL,'2017-08-11 20:43:58','2017-08-11 20:43:58',NULL),(27,44,137,0.00,NULL,'2017-08-11 20:50:18','2017-08-11 20:50:18',NULL),(28,49,137,0.00,NULL,'2017-08-12 10:32:15','2017-08-12 10:32:15',NULL),(29,50,141,0.00,NULL,'2017-08-12 10:37:55','2017-08-12 10:37:55',NULL),(30,51,137,0.00,NULL,'2017-08-12 10:39:44','2017-08-12 10:39:44',NULL),(31,52,140,0.00,NULL,'2017-08-12 10:42:01','2017-08-12 10:42:01',NULL),(32,53,139,0.00,NULL,'2017-08-12 10:45:01','2017-08-12 10:45:01',NULL),(33,54,139,0.00,NULL,'2017-08-12 11:05:34','2017-08-12 11:05:34',NULL),(34,55,140,0.00,NULL,'2017-08-12 11:07:53','2017-08-12 11:07:53',NULL),(35,56,142,0.00,NULL,'2017-08-12 11:08:42','2017-08-12 11:08:42',NULL),(36,60,141,0.00,NULL,'2017-08-12 11:14:14','2017-08-12 11:14:14',NULL),(37,95,137,10000.00,NULL,'2017-08-12 17:15:05','2017-08-12 17:15:05',NULL);
/*!40000 ALTER TABLE `PaymentDetails` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-08-27 16:13:09

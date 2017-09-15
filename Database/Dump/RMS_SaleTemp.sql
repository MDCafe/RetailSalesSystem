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
-- Table structure for table `SaleTemp`
--

DROP TABLE IF EXISTS `SaleTemp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `SaleTemp` (
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
) ENGINE=InnoDB AUTO_INCREMENT=81 DEFAULT CHARSET=utf8 COMMENT='Temproary	 table to save sale details	';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SaleTemp`
--

LOCK TABLES `SaleTemp` WRITE;
/*!40000 ALTER TABLE `SaleTemp` DISABLE KEYS */;
INSERT INTO `SaleTemp` (`Id`, `Guid`, `SaleDate`, `CustomerId`, `PaymentMode`, `OrderNo`, `ProductId`, `Quantity`, `SellingPrice`, `DiscountPercentage`, `DiscountAmount`, `Amount`, `PriceId`, `CreatedOn`, `ModifiedOn`) VALUES (1,'25e0ca60-04d9-49c2-b16e-747a0bc9f44a','2017-08-08 17:47:12',140,'0',NULL,5923,0.00,130.00,0.00,0.00,NULL,2920,'2017-08-08 17:48:12','2017-08-08 17:49:12'),(4,'8e156161-b0b4-4215-b4a2-e474182b2e52','2017-08-11 12:52:20',138,'0','1234',5922,12.00,130.00,0.00,0.00,1560.00,2919,'2017-08-11 13:04:04','2017-08-11 13:04:04'),(5,'5445ba2c-b0c4-473a-b6e9-2fa57de6dead','2017-08-11 13:04:39',138,'0',NULL,5919,12.00,130.00,0.00,0.00,1560.00,2916,'2017-08-11 13:06:37','2017-08-11 13:06:37'),(6,'bbec7628-a5b7-4ac3-ac8c-c9c0d16f1c91','2017-08-11 13:09:07',140,'0','234',5936,10.00,130.00,0.00,0.00,1300.00,2933,'2017-08-11 13:10:48','2017-08-11 13:10:48'),(9,'45b72c55-b9d6-4eb8-9375-0c109621b7f3','2017-08-11 13:25:54',144,'0','3434',5924,10.00,130.00,0.00,0.00,1300.00,2921,'2017-08-11 13:27:01','2017-08-11 13:27:01'),(10,'a1cca9f9-46b1-493a-abe4-5c12385c4eb9','2017-08-11 13:28:57',140,'0','234',5922,12.00,130.00,0.00,0.00,1560.00,2919,'2017-08-11 13:31:09','2017-08-11 13:31:09'),(11,'817a2242-6269-4197-bd82-a4b820d6a1a6','2017-08-11 15:04:31',137,'0',NULL,5920,10.00,130.00,0.00,0.00,1300.00,2917,'2017-08-11 15:05:33','2017-08-11 15:05:33'),(12,'817a2242-6269-4197-bd82-a4b820d6a1a6','2017-08-11 15:04:31',137,'0',NULL,6883,10.00,130.00,0.00,0.00,1300.00,3880,'2017-08-11 15:05:33','2017-08-11 15:05:33'),(16,'9cb31dba-c55d-4ae2-9112-9463618ac0f9','2017-08-11 16:08:48',138,'0',NULL,5926,1.00,130.00,0.00,0.00,130.00,2923,'2017-08-11 16:18:42','2017-08-11 16:18:42'),(17,'9cb31dba-c55d-4ae2-9112-9463618ac0f9','2017-08-11 16:08:48',138,'0',NULL,5923,1.00,130.00,0.00,0.00,130.00,2920,'2017-08-11 16:18:42','2017-08-11 16:18:42'),(18,'ae34750c-9128-4eeb-94c9-3fd3be3263fd','2017-08-11 16:19:54',138,'0','',5916,10.00,130.00,0.00,0.00,1300.00,2913,'2017-08-11 16:25:58','2017-08-11 16:25:58'),(26,'ce43dc5e-fb29-486e-a5c5-276c4d2144fc','2017-08-11 18:56:55',137,'0',NULL,6375,100.00,130.00,0.00,0.00,13000.00,3372,'2017-08-11 18:58:27','2017-08-11 18:58:27'),(27,'ce43dc5e-fb29-486e-a5c5-276c4d2144fc','2017-08-11 18:56:55',137,'0',NULL,6053,100.50,130.00,0.00,0.00,13065.00,3050,'2017-08-11 18:58:28','2017-08-11 18:58:28'),(29,'4987f579-ecc8-4e40-9c02-63ecd6481595','2017-08-11 19:04:12',137,'0',NULL,5922,100.50,130.00,0.00,0.00,13065.00,2919,'2017-08-11 19:06:22','2017-08-11 19:06:22'),(31,'a990f116-fec9-41ab-90a0-8838ceca7e34','2017-08-11 19:28:51',139,'0',NULL,5916,1.90,130.00,0.00,0.00,247.00,2913,'2017-08-11 19:31:08','2017-08-11 19:31:08'),(32,'24ad8d60-3bd9-4f84-ad33-f8002be4347a','2017-08-11 20:49:45',137,'0','',0,NULL,NULL,0.00,0.00,NULL,0,'2017-08-12 10:00:32','2017-08-12 10:00:32'),(48,'61faa0ff-206f-4f2c-8b64-32f91af06e03','2017-08-12 11:23:06',138,'0','',5919,12.20,130.00,0.00,0.00,1586.00,2916,'2017-08-12 11:46:53','2017-08-12 11:46:53'),(50,'a630bef8-24c2-4595-902d-c692012a4f3b','2017-08-12 12:15:23',137,'0',NULL,5918,10.00,130.00,5.00,0.00,1235.00,2915,'2017-08-12 12:16:28','2017-08-12 12:16:28'),(52,'3065a5d6-3e21-4471-a017-61a095fffff2','2017-08-12 12:20:26',139,'0','',5919,10.00,130.00,5.00,0.00,1235.00,2916,'2017-08-12 12:24:41','2017-08-12 12:24:41'),(55,'ac2a08c6-161e-4791-9e76-2ef2f4e088df','2017-08-12 13:25:30',138,'0',NULL,5930,12.00,130.00,5.00,0.00,1482.00,2927,'2017-08-12 13:26:30','2017-08-12 13:26:30'),(57,'5c014cbc-96cc-4291-9faa-dcf3483697f9','2017-08-12 17:04:28',137,'1','',6053,10.00,130.00,0.00,0.00,1300.00,3050,'2017-08-12 17:13:29','2017-08-12 17:13:29'),(58,'5c014cbc-96cc-4291-9faa-dcf3483697f9','2017-08-12 17:04:28',137,'1','',5935,1.00,130.00,0.00,0.00,130.00,2932,'2017-08-12 17:13:29','2017-08-12 17:13:29'),(59,'5c014cbc-96cc-4291-9faa-dcf3483697f9','2017-08-12 17:04:28',137,'1','',5917,NULL,130.00,0.00,0.00,NULL,2914,'2017-08-12 17:13:29','2017-08-12 17:13:29'),(69,'2acf7d90-8117-4b88-a92d-599f49ec0713','2017-08-13 09:37:59',137,'0','4567',6717,1.50,130.00,0.00,0.00,195.00,3714,'2017-08-13 09:42:00','2017-08-13 09:42:00'),(70,'2acf7d90-8117-4b88-a92d-599f49ec0713','2017-08-13 09:37:59',137,'0','4567',6053,2.00,130.00,0.00,0.00,260.00,3050,'2017-08-13 09:42:00','2017-08-13 09:42:00'),(71,'2acf7d90-8117-4b88-a92d-599f49ec0713','2017-08-13 09:37:59',137,'0','4567',6552,1.50,130.00,0.00,0.00,195.00,3549,'2017-08-13 09:42:00','2017-08-13 09:42:00'),(72,'2acf7d90-8117-4b88-a92d-599f49ec0713','2017-08-13 09:37:59',137,'0','4567',6204,0.95,130.00,0.00,0.00,124.02,3201,'2017-08-13 09:42:00','2017-08-13 09:42:00'),(73,'2acf7d90-8117-4b88-a92d-599f49ec0713','2017-08-13 09:37:59',-1,'0','4567',6375,1.00,130.00,0.00,0.00,130.00,3372,'2017-08-13 09:42:00','2017-08-13 09:42:00'),(78,'ad57080b-3fa5-4365-95fc-aed6887a1c24','2017-08-13 11:27:01',141,'1','345',5919,100.00,130.00,0.00,0.00,13000.00,2916,'2017-08-13 11:30:04','2017-08-13 11:30:04'),(80,'cd697ab9-2c40-4a22-aba3-e663560d3141','2017-08-13 12:20:21',140,'0','',5920,15.00,130.00,0.00,0.00,1950.00,2917,'2017-08-13 12:23:02','2017-08-13 12:23:02');
/*!40000 ALTER TABLE `SaleTemp` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-08-27 16:13:13
CREATE SCHEMA `facilitylog` ;
USE facilitylog ;
CREATE TABLE `a_vals` (
  `Date_Time` varchar(45) DEFAULT NULL,
  `CW_Supply` float DEFAULT NULL,
  `CW_PreFiltPres` float DEFAULT NULL,
  `CW_PostFiltPres` float DEFAULT NULL,
  `CW_FiltDif` float DEFAULT NULL,
  `HW_PreFiltPres` float DEFAULT NULL,
  `HW_PostFiltPres` float DEFAULT NULL,
  `HW_FiltDif` float DEFAULT NULL,
  `HW_Temp` float DEFAULT NULL,
  `HW_Flow` float DEFAULT NULL,
  `ST_FeedWaterPres` float DEFAULT NULL,
  `ST_HeadPres` float DEFAULT NULL,
  `ST_LowPres` float DEFAULT NULL,
  `ST_MedPres` float DEFAULT NULL,
  `ST_LowDem` float DEFAULT NULL,
  `ST_MedDem` float DEFAULT NULL,
  `ST_Flow` float DEFAULT NULL,
  `EL_NorthA` float DEFAULT NULL,
  `EL_NorthB` float DEFAULT NULL,
  `EL_NorthC` float DEFAULT NULL,
  `EL_SouthA` float DEFAULT NULL,
  `EL_SouthB` float DEFAULT NULL,
  `EL_SouthC` float DEFAULT NULL,
  `AR_LinePres` float DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
CREATE TABLE `alarm_history` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Date_Time` varchar(14) DEFAULT NULL,
  `State` varchar(7) DEFAULT NULL,
  `Message` varchar(45) DEFAULT NULL,
  `Alarm_Type` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=138 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
CREATE TABLE `system_log` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Date_Time` varchar(14) DEFAULT NULL,
  `Message` varchar(45) DEFAULT NULL,
  `Active_User` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=501 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

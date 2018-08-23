
use WarehouseLaborEfficiency;


SET FOREIGN_KEY_CHECKS = 0;




CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`tbl_bu` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `bu` VARCHAR(50) CHARACTER SET 'utf8mb4' NULL,
  PRIMARY KEY (`id`));

CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`sys_roles` (
  `id` VARCHAR(50) NOT NULL,
  `RoleName` VARCHAR(20) CHARACTER SET 'utf8mb4' NOT NULL,
  PRIMARY KEY (`id`));

CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`sys_rights` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `RightName` VARCHAR(20) CHARACTER SET 'utf8mb4' NOT NULL,
  `RightContent` VARCHAR(300) NULL,
  PRIMARY KEY (`id`));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`sys_user_role_conn` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `RefUserID` INT NOT NULL,
  `RefRoleID` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `unq_sys_user_role_conn` (`RefUserID` ASC, `RefRoleID` ASC));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`task_log` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Msg` VARCHAR(50) CHARACTER SET 'utf8mb4' NULL,
  `HappenTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`sys_role_right_conn` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `RefRoleID` VARCHAR(50) NOT NULL,
  `RefRightID` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `unq_sys_role_right_conn` (`RefRoleID` ASC, `RefRightID` ASC));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`tbl_WeekData_bak` (
  `id` INT NOT NULL,
  `Date` DATE NOT NULL,
  `Warehouse` VARCHAR(50) CHARACTER SET 'utf8mb4' NULL,
  `HC_FCST` INT NULL,
  `HC_Actual` INT NULL,
  `HC_Support` INT NULL,
  `HC_Utilization` FLOAT(24,0) NULL,
  `Case_ID_in` INT NULL,
  `Case_ID_out` INT NULL,
  `Pallet_In` INT NULL,
  `Pallet_Out` INT NULL,
  `Jobs_Rec` INT NULL,
  `Jobs_Issue` INT NULL,
  `Reel_ID_Rec` INT NULL,
  `UpdateTime` DATETIME(6) NULL);


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`sys_user` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `ADAccount` VARCHAR(50) NOT NULL,
  `FullName` VARCHAR(50) NOT NULL,
  `Email` VARCHAR(100) NOT NULL,
  `LastLogon` DATETIME(6) NULL,
  `IsValid` TINYINT(1) NOT NULL DEFAULT 1,
  `IsAdmin` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`tbl_WeekData` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Date` DATE NOT NULL,
  `Warehouse` VARCHAR(50) CHARACTER SET 'utf8mb4' NULL,
  `HC_FCST` INT NULL,
  `HC_Actual` INT NULL,
  `HC_Support` INT NULL,
  `HC_Utilization` FLOAT(24,0) NULL DEFAULT 0.0,
  `Case_ID_in` INT NULL,
  `Case_ID_out` INT NULL,
  `Pallet_In` INT NULL,
  `Pallet_Out` INT NULL,
  `Jobs_Rec` INT NULL,
  `Jobs_Issue` INT NULL,
  `Reel_ID_Rec` INT NULL,
  `UpdateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `unq_tbl_WeekData` (`Date` ASC, `Warehouse` ASC));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`tbl_MonthData` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Date` DATE NOT NULL,
  `Warehouse` VARCHAR(50) CHARACTER SET 'utf8mb4' NULL,
  `HC_FCST` INT NULL,
  `HC_Actual` INT NULL,
  `HC_Support` INT NULL,
  `HC_Utilization` FLOAT(24,0) NULL DEFAULT 0.0,
  `Case_ID_in` INT NULL,
  `Case_ID_out` INT NULL,
  `Pallet_In` INT NULL,
  `Pallet_Out` INT NULL,
  `Jobs_Rec` INT NULL,
  `Jobs_Issue` INT NULL,
  `Reel_ID_Rec` INT NULL,
  `UpdateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `unq_tbl_MonthData` (`Date` ASC, `Warehouse` ASC));


CREATE TABLE IF NOT EXISTS `WarehouseLaborEfficiency`.`tbl_HCData` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Date` DATE NOT NULL,
  `Warehouse` VARCHAR(50) CHARACTER SET 'utf8mb4' NULL,
  `Overall` INT NULL DEFAULT 0,
  `System_Clerk` INT NULL DEFAULT 0,
  `Inventory_Control` INT NULL DEFAULT 0,
  `RTV_Scrap` INT NULL DEFAULT 0,
  `Receiving` INT NULL DEFAULT 0,
  `Shipping` INT NULL DEFAULT 0,
  `Forklift_Driver` INT NULL DEFAULT 0,
  `Total` INT NULL DEFAULT 0,
  `UpdateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `unq_tbl_HCData` (`Date` ASC, `Warehouse` ASC));

-- ==========================================================================


CREATE  OR REPLACE VIEW V_USER_RIGHTS
AS
	select u.id as UserID,u.ADAccount,u.FullName,u.Email,u.IsAdmin,ri.id as RightID,ri.RightName,ri.RightContent 
	from  sys_user u
	join sys_user_role_conn ur_conn
	on u.id=ur_conn.RefUserID
	join sys_roles r
	on ur_conn.RefRoleID=r.id
	join sys_role_right_conn rr_conn
	on r.id = rr_conn.RefRoleID
	join sys_rights ri
	on rr_conn.RefRightID=ri.id;
    


CREATE  OR REPLACE VIEW V_Tbl_WeekData
AS
SELECT id
	  ,`Date`
      ,`Warehouse`
      ,`HC_FCST`
      ,`HC_Actual`
      ,`HC_Support`
      ,CAST(`HC_Utilization`*100 AS decimal(18,2)) as `HC_Utilization`
      ,`Case_ID_in`
      ,`Case_ID_out`
      ,`Pallet_In`
      ,`Pallet_Out`
      ,`Jobs_Rec`
      ,`Jobs_Issue`
      ,`Reel_ID_Rec`
  FROM `tbl_WeekData`
;


CREATE  OR REPLACE VIEW V_Tbl_MonthData
AS
SELECT id
	  ,`Date`
      ,`Warehouse`
      ,`HC_FCST`
      ,`HC_Actual`
      ,`HC_Support`
      ,CAST(`HC_Utilization`*100 AS decimal(18,2))  as `HC_Utilization`
      ,`Case_ID_in`
      ,`Case_ID_out`
      ,`Pallet_In`
      ,`Pallet_Out`
      ,`Jobs_Rec`
      ,`Jobs_Issue`
      ,`Reel_ID_Rec`
  FROM `tbl_MonthData`
;


CREATE  OR REPLACE VIEW V_Tbl_HCData
AS
SELECT id
      ,`Date`
      ,`Warehouse`
      ,`Overall`
      ,`System_Clerk`
      ,`Inventory_Control`
      ,`RTV_Scrap`
      ,`Receiving`
      ,`Shipping`
      ,`Forklift_Driver`
      ,`Total`
  FROM `tbl_HCData`
;


-- ==================================================
USE `WarehouseLaborEfficiency`;


DELIMITER $$
USE `WarehouseLaborEfficiency`$$
CREATE DEFINER = CURRENT_USER FUNCTION `FN_Check_UserRight`(`userAd` varchar(50), `rightID` int)
RETURNS int
DETERMINISTIC
BEGIN
	if exists ( 
		select *
		from V_USER_RIGHTS v
		where v.ADAccount=userAd
		and (v.RightID=rightID or v.IsAdmin>0)
        limit 1
	) then
		return 1;
	else
		return 0;
	end if;
END;
$$
DELIMITER ;


-- ----------------------------------
SET FOREIGN_KEY_CHECKS = 1;


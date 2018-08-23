
use WarehouseLaborEfficiency;

create table tbl_bu(
	id int identity primary key,
	bu nvarchar(50)
);

insert into tbl_bu values (N'PCBA B11');
insert into tbl_bu values (N'PCBA B13');
insert into tbl_bu values (N'PCBA B15');
insert into tbl_bu values (N'Mech');


select * from tbl_bu

drop table tbl_WeekData;

create table tbl_WeekData(
	id int identity primary key,
	[Date] date NOT NULL,
	Warehouse nvarchar(50),
	HC_FCST int,
	HC_Actual int,
	HC_Support int,
	HC_Utilization real default(0.0),
	Case_ID_in int,
	Case_ID_out int,
	Pallet_In int,
	Pallet_Out int,	
	Jobs_Rec int,
	Jobs_Issue int,
	Reel_ID_Rec int,
	UpdateTime datetime default(GetDate()),
	constraint unq_tbl_WeekData unique([Date],Warehouse)
);


drop table tbl_MonthData;

create table tbl_MonthData(
	id int identity primary key,
	[Date] date NOT NULL,
	Warehouse nvarchar(50),
	HC_FCST int,
	HC_Actual int,
	HC_Support int,
	HC_Utilization real default(0.0),
	Case_ID_in int,
	Case_ID_out int,
	Pallet_In int,
	Pallet_Out int,	
	Jobs_Rec int,
	Jobs_Issue int,
	Reel_ID_Rec int,
	UpdateTime datetime default(GetDate()),
	constraint unq_tbl_MonthData unique([Date],Warehouse)
);


drop table tbl_HCData;

create table tbl_HCData(
	id int identity primary key,
	[Date] date NOT NULL,
	Warehouse nvarchar(50),
	Overall int default(0),
	System_Clerk int default(0),
	Inventory_Control int default(0),
	RTV_Scrap int default(0),
	Receiving int default(0),
	Shipping int  default(0),
	Forklift_Driver int default(0),
	Total int default(0),
	UpdateTime datetime default(GetDate()),
	constraint unq_tbl_HCData unique([Date],Warehouse)
);


--------------
alter table tbl_WeekData
	alter column [Date] date not null;
alter table tbl_MonthData
	alter column [Date] date not null;
alter table tbl_HCData
	alter column [Date] date not null;

--------------------------
CREATE VIEW V_Tbl_WeekData
AS
SELECT id
	  ,[Date]
      ,[Warehouse]
      ,[HC_FCST]
      ,[HC_Actual]
      ,[HC_Support]
      ,CAST([HC_Utilization]*100 AS NUMERIC(18,2)) as [HC_Utilization]
      ,[Case_ID_in]
      ,[Case_ID_out]
      ,[Pallet_In]
      ,[Pallet_Out]
      ,[Jobs_Rec]
      ,[Jobs_Issue]
      ,[Reel_ID_Rec]
  FROM [dbo].[tbl_WeekData]
GO

CREATE VIEW V_Tbl_MonthData
AS
SELECT id
	  ,[Date]
      ,[Warehouse]
      ,[HC_FCST]
      ,[HC_Actual]
      ,[HC_Support]
      ,CAST([HC_Utilization]*100 AS NUMERIC(18,2))  as [HC_Utilization]
      ,[Case_ID_in]
      ,[Case_ID_out]
      ,[Pallet_In]
      ,[Pallet_Out]
      ,[Jobs_Rec]
      ,[Jobs_Issue]
      ,[Reel_ID_Rec]
  FROM [dbo].[tbl_MonthData]
GO


CREATE VIEW V_Tbl_HCData
AS
SELECT id
      ,[Date]
      ,[Warehouse]
      ,[Overall]
      ,[System_Clerk]
      ,[Inventory_Control]
      ,[RTV_Scrap]
      ,[Receiving]
      ,[Shipping]
      ,[Forklift_Driver]
      ,[Total]
  FROM [dbo].[tbl_HCData]
GO

drop view V_Tbl_WeekData
drop view V_Tbl_MonthData
drop view V_Tbl_HCData
-------------------------------------------------------------------
truncate table task_log;

create table task_log(
	id int identity primary key,
	Msg nvarchar(50),
	HappenTime DateTime default(getdate())
);
GO

/*==============================================================*/
/* Table: sys_user                                              */
/*==============================================================*/
create table sys_user (
   id                   int identity,
   ADAccount            varchar(50)          not null,
   FullName             varchar(50)          not null,
   Email                varchar(100)         not null,
   LastLogon            datetime             null,
   IsValid              bit                  not null default 1,
   IsAdmin              bit                  not null default 0,
   constraint PK_sys_user primary key (id)
)
go

---------------------
drop table sys_roles;
drop table sys_rights;
drop table sys_user_role_conn;
drop table sys_role_right_conn;
drop table sys_role_right_conn;
----------------------


create table sys_roles(
	id varchar(50) primary key,
	RoleName NVARCHAR(20) NOT NULL
);

create table sys_rights(
	id int identity primary key,
	RightName NVARCHAR(20) NOT NULL,	
	RightContent varchar(300)
);

create table sys_user_role_conn(
	id int identity primary key,
	RefUserID int not null, --sys_user
	RefRoleID varchar(50) not null, --sys_roles
	constraint unq_sys_user_role_conn unique(RefUserID,RefRoleID)
);

create table sys_role_right_conn(
	id int identity primary key,
	RefRoleID varchar(50) not null, --sys_roles
	RefRightID int not null, -- sys_rights
	constraint unq_sys_role_right_conn unique(RefRoleID,RefRightID)
);

drop VIEW V_USER_RIGHTS;

CREATE VIEW V_USER_RIGHTS
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


DROP FUNCTION dbo.FN_Check_UserRight;
CREATE FUNCTION dbo.FN_Check_UserRight(@userAd varchar(50), @rightID int)
RETURNS int
AS
BEGIN
	IF EXISTS( 
		select top 1 *
		from V_USER_RIGHTS v
		where v.ADAccount=@userAd
		and (v.RightID=@rightID or v.IsAdmin>0)
	)
	return 1;

	return 0;
END;
GO

select * from sys_roles
-- truncate table sys_roles


insert into sys_roles
values ('B87251FE-847F-433D-99C8-8B1216BE3CC4',
N'只读浏览'
);
insert into sys_roles
values ('CD1E2D21-3441-4C81-9133-D31086A3CC7F',
N'下载数据'
);
insert into sys_roles
values ('90D1D346-A8F6-43D2-9F12-53FA3B5A8D4F',
N'上传数据'
);

select * from sys_roles
select * from sys_rights
select * from sys_role_right_conn

-- 权限
insert into sys_rights
values(
N'只读浏览',
null
);
insert into sys_rights
values(
N'下载数据',
null
);
insert into sys_rights
values(
N'上传数据',
null
);

-- 角色权限关联
insert into sys_role_right_conn
values (
'B87251FE-847F-433D-99C8-8B1216BE3CC4',
1
);
insert into sys_role_right_conn
values (
'CD1E2D21-3441-4C81-9133-D31086A3CC7F',
2
);
insert into sys_role_right_conn
values (
'90D1D346-A8F6-43D2-9F12-53FA3B5A8D4F',
3
);


-------------------------------------------------------------------

truncate table sys_roles
truncate table sys_role_rights

select newid();


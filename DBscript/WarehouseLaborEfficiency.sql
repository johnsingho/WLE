
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
	UpdateTime datetime default(GetDate())
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
	UpdateTime datetime default(GetDate())
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
	UpdateTime datetime default(GetDate())
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

-------------------------------------------------------------------




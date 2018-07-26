
use CommodityRecv_Dashboard;

---------------------------------------
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

create table tbl_cr_Condition(
	id int identity,
	CommodityCode  varchar(40),
	CostItemNumber varchar(40) primary key,
	CommodityCodeDescription varchar(300)
);
GO

create nonclustered index idx_tbl_cr_Condition_CostItemNumber 
on tbl_cr_Condition(CostItemNumber);
GO

create table tbl_cr_Baan(
	id int identity primary key,
	PONUMBER varchar(80),
	SUPPLIERNBR varchar(80),
	POTYPE varchar(20),
	BUYER varchar(100),
	REFERENCEA varchar(80),
	REFERENCEB varchar(80),
	ITEM varchar(80),
	ITEMDSE nvarchar(200),
	QTY int,
	UNIT varchar(20),
	RECEIVEDATE DateTime,
	CompCode varchar(8)
);
GO

create table tbl_cr_mailReceiver(
	id int identity primary key,
	cnName nvarchar(40),
	enName varchar(40),
	mailAddr varchar(120) UNIQUE NOT NULL,
	mailAddrType int default(0) -- 0 for to, 1 for cc
);
GO

alter table tbl_cr_mailReceiver
	add isValid bit not null default(1); -- 1有效，0无效
GO


-- 对比结果
/*
CREATE VIEW v_CommodityRecvCmp
AS
select ba.PONUMBER,cond.CostItemNumber as ITEM,ba.QTY,ba.UNIT,ba.REFERENCEB as Receiver, ba.RECEIVEDATE, ba.CompCode
from tbl_cr_Condition cond
JOIN tbl_cr_Baan ba
on cond.CostItemNumber=ba.ITEM
GO
*/

CREATE VIEW v_CommodityRecvCmp
AS
select t.PONUMBER, t.ITEM, SUM(t.Qty) as Qty, t.UNIT, t.Receiver, min(t.RECEIVEDATE) as RECEIVEDATE, t.CompCode, t.CommodityCodeDescription
from 
(
select ba.PONUMBER,cond.CostItemNumber as ITEM,ba.QTY,ba.UNIT,ba.REFERENCEB as Receiver, ba.RECEIVEDATE, ba.CompCode, cond.CommodityCodeDescription
from tbl_cr_Condition cond
JOIN tbl_cr_Baan ba
on cond.CostItemNumber=ba.ITEM
) as t
group by t.PONUMBER, t.ITEM, t.UNIT, t.UNIT, t.Receiver, t.CompCode, t.CommodityCodeDescription
GO


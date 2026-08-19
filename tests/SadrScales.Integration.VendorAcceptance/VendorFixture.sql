SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

CREATE TABLE dbo.SADR_Store
(
    StoreCode varchar(50) NOT NULL,
    StoreName nvarchar(100) NULL,
    Descriptions nvarchar(150) NULL,
    CONSTRAINT PK_VA_Store PRIMARY KEY CLUSTERED (StoreCode)
);
INSERT INTO dbo.SADR_Store(StoreCode, StoreName, Descriptions)
VALUES('0', N'Default', N'Default Store');

CREATE TABLE dbo.SADR_ItemClass
(
    ItemClassCode varchar(50) NOT NULL,
    ItemClassName nvarchar(100) NULL,
    Descriptions nvarchar(150) NULL,
    CONSTRAINT PK_VA_ItemClass PRIMARY KEY CLUSTERED (ItemClassCode)
);
INSERT INTO dbo.SADR_ItemClass(ItemClassCode, ItemClassName, Descriptions)
VALUES('0', N'Default', N'Default Group');

CREATE TABLE dbo.SADR_Item
(
    ItemClassCode varchar(50) NOT NULL CONSTRAINT DF_VA_Item_Group DEFAULT('0'),
    PluNo int NOT NULL,
    PluUnit int NULL CONSTRAINT DF_VA_Item_Unit DEFAULT(3),
    UnitPrice int NULL CONSTRAINT DF_VA_Item_Price DEFAULT(0),
    PrintFormat int NULL CONSTRAINT DF_VA_Item_Print DEFAULT(0),
    PluCost int NULL CONSTRAINT DF_VA_Item_Cost DEFAULT(0),
    BarFormat int NULL CONSTRAINT DF_VA_Item_Bar DEFAULT(0),
    BarFlags int NULL CONSTRAINT DF_VA_Item_Flags DEFAULT(0),
    ItemCode varchar(10) NULL CONSTRAINT DF_VA_Item_Code DEFAULT('0'),
    IndexBarcode varchar(50) NULL CONSTRAINT DF_VA_Item_Index DEFAULT('0'),
    Tare float NULL CONSTRAINT DF_VA_Item_Tare DEFAULT(0),
    ShelfDate int NULL CONSTRAINT DF_VA_Item_Shelf DEFAULT(0),
    ShelfDatePrint int NULL CONSTRAINT DF_VA_Item_ShelfPrint DEFAULT(0),
    SaleDatePrint int NULL CONSTRAINT DF_VA_Item_SaleDate DEFAULT(0),
    SaleTimePrint int NULL CONSTRAINT DF_VA_Item_SaleTime DEFAULT(0),
    OnlyTare int NULL CONSTRAINT DF_VA_Item_OnlyTare DEFAULT(0),
    TaxRate float NULL CONSTRAINT DF_VA_Item_Tax DEFAULT(0),
    PluName nvarchar(100) NULL,
    Text1 nvarchar(100) NULL,
    Text2 nvarchar(100) NULL,
    Text3 nvarchar(100) NULL,
    Text4 nvarchar(100) NULL,
    Text5 nvarchar(100) NULL,
    Text6 nvarchar(100) NULL,
    Text7 nvarchar(100) NULL,
    DeleteFlag int NULL CONSTRAINT DF_VA_Item_Delete DEFAULT(0),
    [TimeStamp] timestamp NOT NULL,
    CONSTRAINT PK_VA_Item PRIMARY KEY CLUSTERED (PluNo),
    CONSTRAINT FK_VA_Item_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);

CREATE TABLE dbo.SADR_Scale
(
    ScaleID int NOT NULL,
    Port int NOT NULL CONSTRAINT DF_VA_Scale_Port DEFAULT(5000),
    Mac varchar(50) NULL,
    StoreCode varchar(50) NULL CONSTRAINT DF_VA_Scale_Store DEFAULT('0'),
    ItemClassCode varchar(50) NULL CONSTRAINT DF_VA_Scale_Group DEFAULT('0'),
    GroupName nvarchar(50) NULL,
    Category nvarchar(50) NULL,
    Version varchar(50) NULL,
    DeviceName nvarchar(50) NULL,
    StoreName nvarchar(50) NULL,
    ScaleIP varchar(20) NULL,
    AutoSendItems int NULL CONSTRAINT DF_VA_Scale_AutoSend DEFAULT(0),
    Status nvarchar(50) NULL,
    LastSendItem bigint NOT NULL CONSTRAINT DF_VA_Scale_LastItem DEFAULT(0),
    LastSendKey bigint NOT NULL CONSTRAINT DF_VA_Scale_LastKey DEFAULT(0),
    LastReceiveFID int NOT NULL CONSTRAINT DF_VA_Scale_LastFid DEFAULT(0),
    AutoGetInvoice int NULL CONSTRAINT DF_VA_Scale_AutoGet DEFAULT(0),
    SendScaleDetail int NULL CONSTRAINT DF_VA_Scale_SendDetail DEFAULT(0),
    GetScaleDetail int NULL CONSTRAINT DF_VA_Scale_GetDetail DEFAULT(0),
    HotKeyCountPerPage smallint NOT NULL CONSTRAINT DF_VA_Scale_KeyCount DEFAULT(40),
    HotKeyPageCount tinyint NOT NULL CONSTRAINT DF_VA_Scale_PageCount DEFAULT(3),
    Used int NULL CONSTRAINT DF_VA_Scale_Used DEFAULT(0),
    CONSTRAINT PK_VA_Scale PRIMARY KEY CLUSTERED (ScaleID),
    CONSTRAINT FK_VA_Scale_Store FOREIGN KEY(StoreCode) REFERENCES dbo.SADR_Store(StoreCode),
    CONSTRAINT FK_VA_Scale_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);
CREATE UNIQUE INDEX UX_VA_Scale_IP_Port ON dbo.SADR_Scale(ScaleIP, Port) WHERE ScaleIP IS NOT NULL;

CREATE TABLE dbo.SADR_ScaleItemClass
(
    ScaleID int NOT NULL,
    ItemClassCode varchar(50) NOT NULL,
    CONSTRAINT PK_VA_ScaleItemClass PRIMARY KEY CLUSTERED (ScaleID, ItemClassCode),
    CONSTRAINT FK_VA_ScaleItemClass_Scale FOREIGN KEY(ScaleID) REFERENCES dbo.SADR_Scale(ScaleID) ON DELETE CASCADE,
    CONSTRAINT FK_VA_ScaleItemClass_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);

CREATE TABLE dbo.SADR_ScaleItemMap
(
    ScaleID int NOT NULL,
    PluNo int NOT NULL,
    ItemCode int NOT NULL,
    PageNo int NULL,
    KeyNo int NULL,
    CONSTRAINT PK_VA_ScaleItemMap PRIMARY KEY CLUSTERED (ScaleID, PluNo),
    CONSTRAINT FK_VA_ScaleItemMap_Scale FOREIGN KEY(ScaleID) REFERENCES dbo.SADR_Scale(ScaleID) ON DELETE CASCADE,
    CONSTRAINT FK_VA_ScaleItemMap_Item FOREIGN KEY(PluNo) REFERENCES dbo.SADR_Item(PluNo) ON DELETE CASCADE,
    CONSTRAINT CK_VA_ScaleItemMap_ItemCode CHECK(ItemCode > 0),
    CONSTRAINT CK_VA_ScaleItemMap_Page CHECK(PageNo IS NULL OR PageNo BETWEEN 0 AND 2),
    CONSTRAINT CK_VA_ScaleItemMap_Key CHECK(KeyNo IS NULL OR KeyNo > 0),
    CONSTRAINT CK_VA_ScaleItemMap_Pair CHECK((PageNo IS NULL AND KeyNo IS NULL) OR (PageNo IS NOT NULL AND KeyNo IS NOT NULL))
);
CREATE UNIQUE INDEX UX_VA_ScaleItemMap_ItemCode ON dbo.SADR_ScaleItemMap(ScaleID, ItemCode);
CREATE UNIQUE INDEX UX_VA_ScaleItemMap_HotKey ON dbo.SADR_ScaleItemMap(ScaleID, PageNo, KeyNo)
WHERE PageNo IS NOT NULL AND KeyNo IS NOT NULL;

CREATE TABLE dbo.SADR_KeyAssignment
(
    ItemClassCode varchar(50) NOT NULL,
    PageNo int NOT NULL,
    KeyNo int NOT NULL,
    PluNo int NOT NULL,
    [TimeStamp] timestamp NOT NULL,
    CONSTRAINT PK_VA_KeyAssignment PRIMARY KEY CLUSTERED (ItemClassCode, PageNo, KeyNo),
    CONSTRAINT FK_VA_KeyAssignment_Group FOREIGN KEY(ItemClassCode) REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);

CREATE TABLE dbo.SADR_Logs
(
    ID int IDENTITY(1,1) NOT NULL,
    DeviceNo int NOT NULL,
    Identify nvarchar(50) NULL,
    [DateTime] datetime NOT NULL,
    FID int NOT NULL,
    SID int NOT NULL,
    Salesman int NOT NULL,
    SubID int NOT NULL,
    TotalPrice float NOT NULL,
    PLU int NOT NULL,
    Class int NOT NULL,
    Dept int NOT NULL,
    Amount float NOT NULL,
    Unit int NOT NULL,
    LogType int NOT NULL,
    Tax float NOT NULL,
    Text1 nvarchar(50) NULL,
    Text2 nvarchar(50) NULL,
    Text3 nvarchar(50) NULL,
    Text4 nvarchar(50) NULL,
    UnitPrice float NOT NULL,
    CoFID int NOT NULL,
    PLUName nvarchar(50) NOT NULL,
    CONSTRAINT PK_VA_Logs PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT UX_VA_Logs UNIQUE (DeviceNo, FID, SubID)
);

CREATE TABLE dbo.SADR_Total
(
    TotalID int IDENTITY(1,1) NOT NULL,
    ScaleID int NOT NULL,
    SaleDateTime smalldatetime NULL,
    LableStatus int NULL CONSTRAINT DF_VA_Total_Status DEFAULT(0),
    ReceiptNo int NOT NULL,
    TotalBarcode varchar(50) NULL,
    ItemBarcode varchar(50) NOT NULL,
    NTrans int NULL,
    SubDiscAmt int NULL CONSTRAINT DF_VA_Total_SubDisc DEFAULT(0),
    DiscAmt int NULL CONSTRAINT DF_VA_Total_Disc DEFAULT(0),
    AmtOfATax int NULL CONSTRAINT DF_VA_Total_ATax DEFAULT(0),
    AmtOfVTax int NULL CONSTRAINT DF_VA_Total_VTax DEFAULT(0),
    PriceWTax int NULL CONSTRAINT DF_VA_Total_Price DEFAULT(0),
    ClerkNo int NULL,
    CONSTRAINT PK_VA_Total PRIMARY KEY CLUSTERED (ItemBarcode),
    CONSTRAINT UX_VA_Total_TotalID UNIQUE (TotalID),
    CONSTRAINT FK_VA_Total_Scale FOREIGN KEY(ScaleID) REFERENCES dbo.SADR_Scale(ScaleID)
);
CREATE INDEX IX_VA_Total_ScaleBarcode ON dbo.SADR_Total(ScaleID, TotalBarcode);

CREATE TABLE dbo.SADR_Detail
(
    DetailID int IDENTITY(1,1) NOT NULL,
    TotalID int NOT NULL,
    TotalBarcode varchar(50) NULL,
    ItemBarcode varchar(50) NOT NULL,
    TransNo int NULL,
    PluNo int NOT NULL,
    Weight float NULL CONSTRAINT DF_VA_Detail_Weight DEFAULT(0),
    QTY float NULL CONSTRAINT DF_VA_Detail_Qty DEFAULT(0),
    Uprice int NULL,
    UpriceAfDisc int NULL CONSTRAINT DF_VA_Detail_UpriceAfter DEFAULT(0),
    StPointDiscStat int NULL CONSTRAINT DF_VA_Detail_Stat DEFAULT(0),
    TTLPriceDiscAmt int NULL CONSTRAINT DF_VA_Detail_Disc DEFAULT(0),
    ActPrice int NULL,
    TaxRtNo int NULL CONSTRAINT DF_VA_Detail_Tax DEFAULT(0),
    ItemStatus int NULL,
    CONSTRAINT PK_VA_Detail PRIMARY KEY CLUSTERED (DetailID),
    CONSTRAINT FK_VA_Detail_Total FOREIGN KEY(TotalID) REFERENCES dbo.SADR_Total(TotalID)
);
CREATE INDEX IX_VA_Detail_Total ON dbo.SADR_Detail(TotalID);

CREATE TABLE dbo.SADR_PriceLog
(
    ID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PluNo int NOT NULL,
    IndexBarcode varchar(50) NULL,
    PluName nvarchar(100) NULL,
    LastPrice int NOT NULL,
    NewPrice int NOT NULL,
    [DateTime] datetime NOT NULL,
    [User] nvarchar(100) NULL
);

CREATE TABLE dbo.VendorDestinationInvoice
(
    TotalBarcode varchar(50) NOT NULL PRIMARY KEY,
    SavedAtUtc datetime2(0) NOT NULL
);

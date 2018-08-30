using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarehouseLaborEfficiencyBLL;
using System.IO;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        static UnitTest1()
        {
            //CustomConfig.ConnStrMain = "server=dmnnte801;user id=admin;password=dmn@1a2b3c4d;database=WarehouseLaborEfficiency;min pool size=4;max pool size=100;packet size=3072";
            //CustomConfig.ConnStrMain = "Datasource=10.201.63.2;port=3306;Database=WarehouseLaborEfficiency;uid=eapp;pwd=eapp2018SQL;";
            CustomConfig.ConnStrMain = "Datasource=10.201.144.50;port=3306;Database=WarehouseLaborEfficiency;uid=OfficeMyApp;pwd=Of!2018ice;";
        }

        [TestMethod]
        public void TestImportWeekData()
        {
            //var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData0821\WeekData_badTest.xlsx";
            //var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\WeekData.xlsx";
            //var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\WeekData1.xlsx";
            var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\bug\Week Data导入了一个空行产生全0日期.xlsx";
            var sErr = string.Empty;
            var nImp = WLE_Data.ImportWeekData(new FileInfo(sFile), out sErr);
            Assert.IsTrue(nImp > 0);
            Assert.AreEqual(string.Empty, sErr);
        }

        [TestMethod]
        public void TestImportMonthData()
        {
            var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\MonthData.xlsx";
            var sErr = string.Empty;
            var nImp = WLE_Data.ImportMonthData(new FileInfo(sFile), out sErr);
            Assert.IsTrue(nImp > 0);
            Assert.AreEqual(string.Empty, sErr);
        }

        [TestMethod]
        public void TestImportHCData()
        {
            var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\HCData.xlsx";
            var sErr = string.Empty;
            var nImp = WLE_Data.ImportHCData(new FileInfo(sFile), out sErr);
            Assert.IsTrue(nImp > 0);
            Assert.AreEqual(string.Empty, sErr);
        }


    }
}

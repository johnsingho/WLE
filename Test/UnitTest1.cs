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
            CustomConfig.ConnStrMain = "server=dmnnte801;user id=admin;password=dmn@1a2b3c4d;database=WarehouseLaborEfficiency;min pool size=4;max pool size=100;packet size=3072";
        }

        [TestMethod]
        public void TestImportWeekData()
        {
            var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\WeekData.xlsx";
            var sErr = string.Empty;
            var bImp = WLE_Data.ImportWeekData(new FileInfo(sFile), out sErr);
            Assert.IsTrue(bImp);
            Assert.AreEqual(string.Empty, sErr);
        }

        [TestMethod]
        public void TestImportMonthData()
        {
            var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\MonthData.xlsx";
            var sErr = string.Empty;
            var bImp = WLE_Data.ImportMonthData(new FileInfo(sFile), out sErr);
            Assert.IsTrue(bImp);
            Assert.AreEqual(string.Empty, sErr);
        }

        [TestMethod]
        public void TestImportHCData()
        {
            var sFile = @"D:\he\Doc\20180724发物流人力计算管控系统Dashboard\testData\HCData.xlsx";
            var sErr = string.Empty;
            var bImp = WLE_Data.ImportHCData(new FileInfo(sFile), out sErr);
            Assert.IsTrue(bImp);
            Assert.AreEqual(string.Empty, sErr);
        }


    }
}

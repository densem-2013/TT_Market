using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TT_Market.Core.DAL;
using TT_Market.Core.Domains;
using TT_Market.Web.Models.HelpClasses;

namespace TT_Market.Web.Tests
{
    [TestClass]
    public class ReadTest
    {
        private static readonly string _pathXls =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Web\App_Data\Uploads\шипшина.xls";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tire> _tireRepository;

        public ReadTest()
        {
            _unitOfWork = new UnitOfWork();
            _tireRepository = _unitOfWork.Repository<Tire>();
        }

        [TestMethod]
        public void GetRowReadTest()
        {
            ImportData.ParseAndInsert(_pathXls);
            int count = _tireRepository.GetAll().Count();
            Assert.IsTrue(count > 0);
        }
    }
}

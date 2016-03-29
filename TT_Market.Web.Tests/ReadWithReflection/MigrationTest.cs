using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TT_Market.Core.DAL;
using TT_Market.Core.Domains;
using TT_Market.Core.Identity;

namespace TT_Market.Web.Tests.ReadWithReflection
{
    [TestClass]
    public class MigrationTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model> _modelRepository;
        private readonly IRepository<PriceDocument> _pdocRepository;
        private readonly IRepository<PriceReadSetting> _readSetRepo; 

        private static readonly string _path =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Core\DBinitial\Read";

        public MigrationTest()
        {
            _unitOfWork = new UnitOfWork();
            _modelRepository = _unitOfWork.Repository<Model>();
            _pdocRepository = _unitOfWork.Repository<PriceDocument>();
            _readSetRepo = _unitOfWork.Repository<PriceReadSetting>();
        }

        [TestMethod]
        public void CreateModelsWithRelations()
        {
            ApplicationDbInitializer.InitializeIdentityForEf(_path, new ApplicationDbContext());
            Assert.IsTrue(_modelRepository.GetAll().Any());
        }

        [TestMethod]
        public void GetReadSetting()
        {
            ApplicationDbInitializer.LoadPricesReadSettingsFromXml(_path, new ApplicationDbContext());
            int count = _readSetRepo.GetAll().Count();
            Assert.IsTrue(count>1);
        }
    }
}

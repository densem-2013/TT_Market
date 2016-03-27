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

        private static readonly string _path =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Core\DBinitial\Read";

        public MigrationTest()
        {
            _unitOfWork = new UnitOfWork();
            _modelRepository = _unitOfWork.Repository<Model>();
            _pdocRepository = _unitOfWork.Repository<PriceDocument>();
        }

        [TestMethod]
        public void CreateModelsWithRelations()
        {
            ApplicationDbInitializer.InitializeIdentityForEf(_path, new ApplicationDbContext());
            Assert.IsTrue(_modelRepository.GetAll().Any());
        }

        [TestMethod]
        public void GetReadSettingPath()
        {
            List<PriceDocument> prisedocs = _pdocRepository.GetAll().ToList();
            List<string> paths=new List<string>();
            foreach (PriceDocument pdoc in prisedocs)
            {
                string filepath = _path.Replace(@"DBinitial\Read", @"DocReadSettings\") + pdoc.FileName.Substring(0, pdoc.FileName.Length - (pdoc.FileName.Length - pdoc.FileName.LastIndexOf("."))) + ".xml";
                paths.Add(filepath);
            }
            Assert.IsTrue(paths.Any());
        }
    }
}

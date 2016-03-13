using System;
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

        private static readonly string _path =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Core\DBinitial\Read";

        public MigrationTest()
        {
            _unitOfWork = new UnitOfWork();
            _modelRepository = _unitOfWork.Repository<Model>();
        }

        [TestMethod]
        public void CreateModelsWithRelations()
        {
            ApplicationDbInitializer.InitializeIdentityForEf(_path, new ApplicationDbContext());
            Assert.IsTrue(_modelRepository.GetAll().Any());
        }
    }
}

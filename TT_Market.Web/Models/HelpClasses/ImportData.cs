using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using NPOI.OpenXmlFormats.Dml;

namespace TT_Market.Web.Models.HelpClasses
{
    public static class ImportData
    {
        private static readonly ApplicationDbContext _db = new ApplicationDbContext();

        public static int ParseAndInsert(string path)
        {

            return 0;
        }

        public static int AddDataToDb(string entity, string property, string value)
        {
            Assembly assembly = Assembly.LoadFrom("TT_Market.Core.dll");
            var type = assembly.GetType("TT_Market.Core.Domains." + entity);
            _db.Set(type).Load();
            var collection = _db.Set(type).Local;

            var newEntity = Activator.CreateInstance(type);
            type.InvokeMember(property, BindingFlags.SetProperty, Type.DefaultBinder, newEntity,
                new Object[] {value});
            collection.Add(newEntity);
            _db.SaveChanges();

            return 0;
        }
    }
}
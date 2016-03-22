namespace TT_Market.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentTitle = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        City_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .Index(t => t.City_Id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Country_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .Index(t => t.Country_Id);
            
            CreateTable(
                "dbo.CityTitles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        City_Id = c.Int(),
                        PriceLanguage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .ForeignKey("dbo.PriceLanguages", t => t.PriceLanguage_Id)
                .Index(t => t.City_Id)
                .Index(t => t.PriceLanguage_Id);
            
            CreateTable(
                "dbo.PriceLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageName = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PriceDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DownLoadDate = c.DateTime(nullable: false),
                        FileName = c.String(),
                        Agent_Id = c.Int(),
                        PriceLanguage_Id = c.Int(),
                        PriceReadSetting_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Agents", t => t.Agent_Id)
                .ForeignKey("dbo.PriceLanguages", t => t.PriceLanguage_Id)
                .ForeignKey("dbo.PriceReadSettings", t => t.PriceReadSetting_Id)
                .Index(t => t.Agent_Id)
                .Index(t => t.PriceLanguage_Id)
                .Index(t => t.PriceReadSetting_Id);
            
            CreateTable(
                "dbo.PriceReadSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransformMask = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TirePropositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PriceCode = c.String(),
                        ExtendedData = c.String(),
                        RegularPrice = c.Double(nullable: false),
                        DiscountPrice = c.Double(nullable: false),
                        SpecialPrice = c.Double(nullable: false),
                        PriceDocument_Id = c.Int(),
                        Stock_Id = c.Int(),
                        Tire_Id = c.Int(),
                        Currency_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceDocuments", t => t.PriceDocument_Id)
                .ForeignKey("dbo.Stocks", t => t.Stock_Id)
                .ForeignKey("dbo.Tires", t => t.Tire_Id)
                .ForeignKey("dbo.Currencies", t => t.Currency_Id)
                .Index(t => t.PriceDocument_Id)
                .Index(t => t.Stock_Id)
                .Index(t => t.Tire_Id)
                .Index(t => t.Currency_Id);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StockValue = c.Int(nullable: false),
                        City_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .Index(t => t.City_Id);
            
            CreateTable(
                "dbo.Tires",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TireTitle = c.String(),
                        ConvSign_Id = c.Int(),
                        Diameter_Id = c.Int(),
                        Height_Id = c.Int(),
                        AutoType_Id = c.Int(),
                        Brand_Id = c.Int(),
                        Model_Id = c.Int(),
                        PressIndex_Id = c.Int(),
                        ProdactionYear_Id = c.Int(),
                        SpeedIndex_Id = c.Int(),
                        Stock_Id = c.Int(),
                        Width_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConvSigns", t => t.ConvSign_Id)
                .ForeignKey("dbo.Diameters", t => t.Diameter_Id)
                .ForeignKey("dbo.Heights", t => t.Height_Id)
                .ForeignKey("dbo.AutoTypes", t => t.AutoType_Id)
                .ForeignKey("dbo.Brands", t => t.Brand_Id)
                .ForeignKey("dbo.Models", t => t.Model_Id)
                .ForeignKey("dbo.PressIndexes", t => t.PressIndex_Id)
                .ForeignKey("dbo.ProductionYears", t => t.ProdactionYear_Id)
                .ForeignKey("dbo.SpeedIndexes", t => t.SpeedIndex_Id)
                .ForeignKey("dbo.Stocks", t => t.Stock_Id)
                .ForeignKey("dbo.Widths", t => t.Width_Id)
                .Index(t => t.ConvSign_Id)
                .Index(t => t.Diameter_Id)
                .Index(t => t.Height_Id)
                .Index(t => t.AutoType_Id)
                .Index(t => t.Brand_Id)
                .Index(t => t.Model_Id)
                .Index(t => t.PressIndex_Id)
                .Index(t => t.ProdactionYear_Id)
                .Index(t => t.SpeedIndex_Id)
                .Index(t => t.Stock_Id)
                .Index(t => t.Width_Id);
            
            CreateTable(
                "dbo.ConvSigns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Diameters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DSize = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Heights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Models",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModelTitle = c.String(),
                        AutoType_Id = c.Int(),
                        Brand_Id = c.Int(),
                        Homol_Id = c.Int(),
                        ProtectorType_Id = c.Int(),
                        Season_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AutoTypes", t => t.AutoType_Id)
                .ForeignKey("dbo.Brands", t => t.Brand_Id)
                .ForeignKey("dbo.HomolAttributes", t => t.Homol_Id)
                .ForeignKey("dbo.ProtectorTypes", t => t.ProtectorType_Id)
                .ForeignKey("dbo.Seasons", t => t.Season_Id)
                .Index(t => t.AutoType_Id)
                .Index(t => t.Brand_Id)
                .Index(t => t.Homol_Id)
                .Index(t => t.ProtectorType_Id)
                .Index(t => t.Season_Id);
            
            CreateTable(
                "dbo.AutoTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BrandTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CountryTitles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Country_Id = c.Int(),
                        PriceLanguage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .ForeignKey("dbo.PriceLanguages", t => t.PriceLanguage_Id)
                .Index(t => t.Country_Id)
                .Index(t => t.PriceLanguage_Id);
            
            CreateTable(
                "dbo.HomolAttributes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtectorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SeasonTitles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        PriceLanguage_Id = c.Int(),
                        Season_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceLanguages", t => t.PriceLanguage_Id)
                .ForeignKey("dbo.Seasons", t => t.Season_Id)
                .Index(t => t.PriceLanguage_Id)
                .Index(t => t.Season_Id);
            
            CreateTable(
                "dbo.PressIndexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductionYears",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SpeedIndexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Widths",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyTitle = c.String(),
                        CurrentExchangeRate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        LastLoginTime = c.DateTime(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Agent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Agents", t => t.Agent_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Agent_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CountryBrands",
                c => new
                    {
                        Country_Id = c.Int(nullable: false),
                        Brand_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Country_Id, t.Brand_Id })
                .ForeignKey("dbo.Countries", t => t.Country_Id, cascadeDelete: true)
                .ForeignKey("dbo.Brands", t => t.Brand_Id, cascadeDelete: true)
                .Index(t => t.Country_Id)
                .Index(t => t.Brand_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Agent_Id", "dbo.Agents");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.TirePropositions", "Currency_Id", "dbo.Currencies");
            DropForeignKey("dbo.Agents", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.CityTitles", "PriceLanguage_Id", "dbo.PriceLanguages");
            DropForeignKey("dbo.TirePropositions", "Tire_Id", "dbo.Tires");
            DropForeignKey("dbo.Tires", "Width_Id", "dbo.Widths");
            DropForeignKey("dbo.Tires", "Stock_Id", "dbo.Stocks");
            DropForeignKey("dbo.Tires", "SpeedIndex_Id", "dbo.SpeedIndexes");
            DropForeignKey("dbo.Tires", "ProdactionYear_Id", "dbo.ProductionYears");
            DropForeignKey("dbo.Tires", "PressIndex_Id", "dbo.PressIndexes");
            DropForeignKey("dbo.Tires", "Model_Id", "dbo.Models");
            DropForeignKey("dbo.SeasonTitles", "Season_Id", "dbo.Seasons");
            DropForeignKey("dbo.SeasonTitles", "PriceLanguage_Id", "dbo.PriceLanguages");
            DropForeignKey("dbo.Models", "Season_Id", "dbo.Seasons");
            DropForeignKey("dbo.Models", "ProtectorType_Id", "dbo.ProtectorTypes");
            DropForeignKey("dbo.Models", "Homol_Id", "dbo.HomolAttributes");
            DropForeignKey("dbo.Tires", "Brand_Id", "dbo.Brands");
            DropForeignKey("dbo.Models", "Brand_Id", "dbo.Brands");
            DropForeignKey("dbo.CountryTitles", "PriceLanguage_Id", "dbo.PriceLanguages");
            DropForeignKey("dbo.CountryTitles", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.Cities", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.CountryBrands", "Brand_Id", "dbo.Brands");
            DropForeignKey("dbo.CountryBrands", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.Models", "AutoType_Id", "dbo.AutoTypes");
            DropForeignKey("dbo.Tires", "AutoType_Id", "dbo.AutoTypes");
            DropForeignKey("dbo.Tires", "Height_Id", "dbo.Heights");
            DropForeignKey("dbo.Tires", "Diameter_Id", "dbo.Diameters");
            DropForeignKey("dbo.Tires", "ConvSign_Id", "dbo.ConvSigns");
            DropForeignKey("dbo.TirePropositions", "Stock_Id", "dbo.Stocks");
            DropForeignKey("dbo.Stocks", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.TirePropositions", "PriceDocument_Id", "dbo.PriceDocuments");
            DropForeignKey("dbo.PriceDocuments", "PriceReadSetting_Id", "dbo.PriceReadSettings");
            DropForeignKey("dbo.PriceDocuments", "PriceLanguage_Id", "dbo.PriceLanguages");
            DropForeignKey("dbo.PriceDocuments", "Agent_Id", "dbo.Agents");
            DropForeignKey("dbo.CityTitles", "City_Id", "dbo.Cities");
            DropIndex("dbo.CountryBrands", new[] { "Brand_Id" });
            DropIndex("dbo.CountryBrands", new[] { "Country_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Agent_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SeasonTitles", new[] { "Season_Id" });
            DropIndex("dbo.SeasonTitles", new[] { "PriceLanguage_Id" });
            DropIndex("dbo.CountryTitles", new[] { "PriceLanguage_Id" });
            DropIndex("dbo.CountryTitles", new[] { "Country_Id" });
            DropIndex("dbo.Models", new[] { "Season_Id" });
            DropIndex("dbo.Models", new[] { "ProtectorType_Id" });
            DropIndex("dbo.Models", new[] { "Homol_Id" });
            DropIndex("dbo.Models", new[] { "Brand_Id" });
            DropIndex("dbo.Models", new[] { "AutoType_Id" });
            DropIndex("dbo.Tires", new[] { "Width_Id" });
            DropIndex("dbo.Tires", new[] { "Stock_Id" });
            DropIndex("dbo.Tires", new[] { "SpeedIndex_Id" });
            DropIndex("dbo.Tires", new[] { "ProdactionYear_Id" });
            DropIndex("dbo.Tires", new[] { "PressIndex_Id" });
            DropIndex("dbo.Tires", new[] { "Model_Id" });
            DropIndex("dbo.Tires", new[] { "Brand_Id" });
            DropIndex("dbo.Tires", new[] { "AutoType_Id" });
            DropIndex("dbo.Tires", new[] { "Height_Id" });
            DropIndex("dbo.Tires", new[] { "Diameter_Id" });
            DropIndex("dbo.Tires", new[] { "ConvSign_Id" });
            DropIndex("dbo.Stocks", new[] { "City_Id" });
            DropIndex("dbo.TirePropositions", new[] { "Currency_Id" });
            DropIndex("dbo.TirePropositions", new[] { "Tire_Id" });
            DropIndex("dbo.TirePropositions", new[] { "Stock_Id" });
            DropIndex("dbo.TirePropositions", new[] { "PriceDocument_Id" });
            DropIndex("dbo.PriceDocuments", new[] { "PriceReadSetting_Id" });
            DropIndex("dbo.PriceDocuments", new[] { "PriceLanguage_Id" });
            DropIndex("dbo.PriceDocuments", new[] { "Agent_Id" });
            DropIndex("dbo.CityTitles", new[] { "PriceLanguage_Id" });
            DropIndex("dbo.CityTitles", new[] { "City_Id" });
            DropIndex("dbo.Cities", new[] { "Country_Id" });
            DropIndex("dbo.Agents", new[] { "City_Id" });
            DropTable("dbo.CountryBrands");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Currencies");
            DropTable("dbo.Widths");
            DropTable("dbo.SpeedIndexes");
            DropTable("dbo.ProductionYears");
            DropTable("dbo.PressIndexes");
            DropTable("dbo.SeasonTitles");
            DropTable("dbo.Seasons");
            DropTable("dbo.ProtectorTypes");
            DropTable("dbo.HomolAttributes");
            DropTable("dbo.CountryTitles");
            DropTable("dbo.Countries");
            DropTable("dbo.Brands");
            DropTable("dbo.AutoTypes");
            DropTable("dbo.Models");
            DropTable("dbo.Heights");
            DropTable("dbo.Diameters");
            DropTable("dbo.ConvSigns");
            DropTable("dbo.Tires");
            DropTable("dbo.Stocks");
            DropTable("dbo.TirePropositions");
            DropTable("dbo.PriceReadSettings");
            DropTable("dbo.PriceDocuments");
            DropTable("dbo.PriceLanguages");
            DropTable("dbo.CityTitles");
            DropTable("dbo.Cities");
            DropTable("dbo.Agents");
        }
    }
}

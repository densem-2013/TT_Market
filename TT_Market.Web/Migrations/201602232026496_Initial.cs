namespace TT_Market.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChildCellColumnNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ColumnName = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceTitleCells", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.PriceTitleCells",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.Int(nullable: false),
                        ColumnName = c.String(),
                        WorkSheet_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkSheets", t => t.WorkSheet_Id)
                .Index(t => t.WorkSheet_Id);
            
            CreateTable(
                "dbo.PriceLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DownLoadDate = c.DateTime(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        FileName = c.String(),
                        PriceLanguage_Id = c.Int(),
                        Provider_Id = c.Int(),
                        ReadSetting_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceLanguages", t => t.PriceLanguage_Id)
                .ForeignKey("dbo.Providers", t => t.Provider_Id)
                .ForeignKey("dbo.ReadSettings", t => t.ReadSetting_Id)
                .Index(t => t.PriceLanguage_Id)
                .Index(t => t.Provider_Id)
                .Index(t => t.ReadSetting_Id);
            
            CreateTable(
                "dbo.CellValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        PriceList_Id = c.Int(),
                        PriceTitleCell_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceLists", t => t.PriceList_Id)
                .ForeignKey("dbo.PriceTitleCells", t => t.PriceTitleCell_Id)
                .Index(t => t.PriceList_Id)
                .Index(t => t.PriceTitleCell_Id);
            
            CreateTable(
                "dbo.PriceLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Providers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReadSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProviderCell = c.String(),
                        ProviderPhoneCell = c.String(),
                        ProviderEmailCell = c.String(),
                        InsertDateCell = c.String(),
                        ColumnNameRow = c.Int(nullable: false),
                        Provider_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Providers", t => t.Provider_Id)
                .Index(t => t.Provider_Id);
            
            CreateTable(
                "dbo.WorkSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        PriceList_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceLists", t => t.PriceList_Id)
                .Index(t => t.PriceList_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PriceListPriceTitleCells",
                c => new
                    {
                        PriceList_Id = c.Int(nullable: false),
                        PriceTitleCell_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PriceList_Id, t.PriceTitleCell_Id })
                .ForeignKey("dbo.PriceLists", t => t.PriceList_Id, cascadeDelete: true)
                .ForeignKey("dbo.PriceTitleCells", t => t.PriceTitleCell_Id, cascadeDelete: true)
                .Index(t => t.PriceList_Id)
                .Index(t => t.PriceTitleCell_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PriceTitleCells", "WorkSheet_Id", "dbo.WorkSheets");
            DropForeignKey("dbo.WorkSheets", "PriceList_Id", "dbo.PriceLists");
            DropForeignKey("dbo.ReadSettings", "Provider_Id", "dbo.Providers");
            DropForeignKey("dbo.PriceLists", "ReadSetting_Id", "dbo.ReadSettings");
            DropForeignKey("dbo.PriceLists", "Provider_Id", "dbo.Providers");
            DropForeignKey("dbo.PriceLists", "PriceLanguage_Id", "dbo.PriceLanguages");
            DropForeignKey("dbo.PriceListPriceTitleCells", "PriceTitleCell_Id", "dbo.PriceTitleCells");
            DropForeignKey("dbo.PriceListPriceTitleCells", "PriceList_Id", "dbo.PriceLists");
            DropForeignKey("dbo.CellValues", "PriceTitleCell_Id", "dbo.PriceTitleCells");
            DropForeignKey("dbo.CellValues", "PriceList_Id", "dbo.PriceLists");
            DropForeignKey("dbo.ChildCellColumnNames", "Parent_Id", "dbo.PriceTitleCells");
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.PriceTitleCells", new[] { "WorkSheet_Id" });
            DropIndex("dbo.WorkSheets", new[] { "PriceList_Id" });
            DropIndex("dbo.ReadSettings", new[] { "Provider_Id" });
            DropIndex("dbo.PriceLists", new[] { "ReadSetting_Id" });
            DropIndex("dbo.PriceLists", new[] { "Provider_Id" });
            DropIndex("dbo.PriceLists", new[] { "PriceLanguage_Id" });
            DropIndex("dbo.PriceListPriceTitleCells", new[] { "PriceTitleCell_Id" });
            DropIndex("dbo.PriceListPriceTitleCells", new[] { "PriceList_Id" });
            DropIndex("dbo.CellValues", new[] { "PriceTitleCell_Id" });
            DropIndex("dbo.CellValues", new[] { "PriceList_Id" });
            DropIndex("dbo.ChildCellColumnNames", new[] { "Parent_Id" });
            DropTable("dbo.PriceListPriceTitleCells");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.WorkSheets");
            DropTable("dbo.ReadSettings");
            DropTable("dbo.Providers");
            DropTable("dbo.PriceLanguages");
            DropTable("dbo.CellValues");
            DropTable("dbo.PriceLists");
            DropTable("dbo.PriceTitleCells");
            DropTable("dbo.ChildCellColumnNames");
        }
    }
}

namespace GitTime.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "c.Contact",
                c => new
                    {
                        pk_ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        Email = c.String(maxLength: 128),
                        FirstName = c.String(maxLength: 64),
                        LastName = c.String(maxLength: 64),
                        Password = c.String(maxLength: 128),
                        Subtype = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.pk_ID);
            
            CreateTable(
                "p.Project",
                c => new
                    {
                        pk_ID = c.Int(nullable: false, identity: true),
                        fk_CompanyContactID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        Repository = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.pk_ID)
                .ForeignKey("c.Contact", t => t.fk_CompanyContactID)
                .Index(t => t.fk_CompanyContactID);
            
            CreateTable(
                "t.Timecard",
                c => new
                    {
                        pk_ID = c.Int(nullable: false, identity: true),
                        fk_ProjectID = c.Int(nullable: false),
                        fk_PersonContactID = c.Int(nullable: false),
                        IssueNumber = c.Int(),
                        IssueDescription = c.String(),
                        EntryDate = c.DateTime(nullable: false),
                        Hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.pk_ID)
                .ForeignKey("c.Contact", t => t.fk_PersonContactID)
                .ForeignKey("p.Project", t => t.fk_ProjectID)
                .Index(t => t.fk_ProjectID)
                .Index(t => t.fk_PersonContactID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("t.Timecard", "fk_ProjectID", "p.Project");
            DropForeignKey("t.Timecard", "fk_PersonContactID", "c.Contact");
            DropForeignKey("p.Project", "fk_CompanyContactID", "c.Contact");
            DropIndex("t.Timecard", new[] { "fk_PersonContactID" });
            DropIndex("t.Timecard", new[] { "fk_ProjectID" });
            DropIndex("p.Project", new[] { "fk_CompanyContactID" });
            DropTable("t.Timecard");
            DropTable("p.Project");
            DropTable("c.Contact");
        }
    }
}

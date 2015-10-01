namespace GitTime.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonRoles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "c.Role",
                c => new
                    {
                        pk_ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.pk_ID)
                .Index(t => t.Name, unique: true, name: "UNQ_RoleName");
            
            CreateTable(
                "c.ContactRole",
                c => new
                    {
                        fk_ContactID = c.Int(nullable: false),
                        fk_RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.fk_ContactID, t.fk_RoleID })
                .ForeignKey("c.Contact", t => t.fk_ContactID)
                .ForeignKey("c.Role", t => t.fk_RoleID)
                .Index(t => t.fk_ContactID)
                .Index(t => t.fk_RoleID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("c.ContactRole", "fk_RoleID", "c.Role");
            DropForeignKey("c.ContactRole", "fk_ContactID", "c.Contact");
            DropIndex("c.ContactRole", new[] { "fk_RoleID" });
            DropIndex("c.ContactRole", new[] { "fk_ContactID" });
            DropIndex("c.Role", "UNQ_RoleName");
            DropTable("c.ContactRole");
            DropTable("c.Role");
        }
    }
}

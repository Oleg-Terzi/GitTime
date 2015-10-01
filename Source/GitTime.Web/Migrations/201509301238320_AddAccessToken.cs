namespace GitTime.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "c.AccessToken",
                c => new
                    {
                        pk_ID = c.Int(nullable: false, identity: true),
                        fk_ContactID = c.Int(nullable: false),
                        Application = c.String(nullable: false, maxLength: 16),
                        Key = c.String(nullable: false),
                        UtcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.pk_ID)
                .ForeignKey("c.Contact", t => t.fk_ContactID)
                .Index(t => new { t.fk_ContactID, t.Application }, unique: true, name: "UNQ_AccessToken");
            
        }
        
        public override void Down()
        {
            DropForeignKey("c.AccessToken", "fk_ContactID", "c.Contact");
            DropIndex("c.AccessToken", "UNQ_AccessToken");
            DropTable("c.AccessToken");
        }
    }
}

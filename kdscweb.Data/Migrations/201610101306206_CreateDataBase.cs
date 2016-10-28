namespace kdscweb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDataBase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TAB_ENGAGEMENT",
                c => new
                    {
                        ENGAGEMENT_ID = c.String(nullable: false, maxLength: 100, unicode: false),
                        NAME = c.String(maxLength: 100, unicode: false),
                        DESCRIPTION = c.String(maxLength: 100, unicode: false),
                        OFFICE_ID = c.String(maxLength: 100, unicode: false),
                        OFFICE_NAME = c.String(maxLength: 100, unicode: false),
                        PARTNER = c.String(maxLength: 100, unicode: false),
                        DIRECTOR = c.String(maxLength: 100, unicode: false),
                        MANAGER_NO = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.ENGAGEMENT_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TAB_ENGAGEMENT");
        }
    }
}

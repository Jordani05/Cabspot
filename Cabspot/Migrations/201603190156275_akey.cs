namespace Cabspot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class akey : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.clientesMovil", "apikey", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.clientesMovil", "apikey", c => c.String(maxLength: 32, storeType: "nvarchar"));
        }
    }
}

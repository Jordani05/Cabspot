namespace Cabspot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.contactos", "telefonoMovil", c => c.String(nullable: false, maxLength: 10, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.contactos", "telefonoMovil", c => c.String(maxLength: 10, unicode: false));
        }
    }
}

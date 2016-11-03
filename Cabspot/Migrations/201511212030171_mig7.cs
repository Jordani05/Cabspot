namespace Cabspot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("contactos", "telefonoMovil", c => c.String(nullable: false, maxLength: 12, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("contactos", "telefonoMovil", c => c.String(maxLength: 12, unicode: false));
        }
    }
}

namespace Cabspot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstadoTaxista : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.taxistas", "ultimaActualizacionEstado", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.taxistas", "ultimaActualizacionEstado");
        }
    }
}

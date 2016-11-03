namespace Cabspot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class autenticacion : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("cabspotdb.vehiculo", "registradoPor", "cabspotdb.empleados");
            //DropIndex("cabspotdb.vehiculos", new[] { "registradoPor" });
            CreateTable(
                "cabspotdb.autenticacionSms",
                c => new
                    {
                        idSms = c.Int(nullable: false, identity: true),
                        idCliente = c.Int(nullable: false),
                        codigo = c.String(maxLength: 15, storeType: "nvarchar"),
                        verificado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idSms)
                .ForeignKey("cabspotdb.clientes", t => t.idCliente, cascadeDelete: true)
                .Index(t => t.idCliente);
            
            //AddColumn("cabspotdb.taxistas", "codigoTaxita", c => c.String(nullable: false, maxLength: 10, unicode: false));
            //AddColumn("cabspotdb.vehiculos", "empleados_idEmpleado", c => c.Int());
            //AddColumn("cabspotdb.vehiculos", "empleados1_idEmpleado", c => c.Int());
            //CreateIndex("cabspotdb.vehiculos", "empleados_idEmpleado");
            //CreateIndex("cabspotdb.vehiculos", "empleados1_idEmpleado");
            //AddForeignKey("cabspotdb.vehiculos", "empleados1_idEmpleado", "cabspotdb.empleados", "idEmpleado");
            //AddForeignKey("cabspotdb.vehiculos", "empleados_idEmpleado", "cabspotdb.empleados", "idEmpleado");
            //DropColumn("cabspotdb.taxistas", "codigoTaxista");
        }
        
        public override void Down()
        {
            //AddColumn("cabspotdb.taxistas", "codigoTaxista", c => c.String(nullable: false, maxLength: 10, unicode: false));
            //DropForeignKey("cabspotdb.vehiculos", "empleados_idEmpleado", "cabspotdb.empleados");
            //DropForeignKey("cabspotdb.vehiculos", "empleados1_idEmpleado", "cabspotdb.empleados");
            DropForeignKey("cabspotdb.autenticacionSms", "idCliente", "cabspotdb.clientes");
            //DropIndex("cabspotdb.vehiculos", new[] { "empleados1_idEmpleado" });
            //DropIndex("cabspotdb.vehiculos", new[] { "empleados_idEmpleado" });
            DropIndex("cabspotdb.autenticacionSms", new[] { "idCliente" });
            //DropColumn("cabspotdb.vehiculos", "empleados1_idEmpleado");
            //DropColumn("cabspotdb.vehiculos", "empleados_idEmpleado");
            //DropColumn("cabspotdb.taxistas", "codigoTaxita");
            DropTable("cabspotdb.autenticacionSms");
            //CreateIndex("cabspotdb.vehiculos", "registradoPor");
            //AddForeignKey("cabspotdb.vehiculo", "registradoPor", "cabspotdb.empleados", "idEmpleado");
            //RenameTable(name: "cabspotdb.vehiculos", newName: "vehiculo");
        }
    }
}

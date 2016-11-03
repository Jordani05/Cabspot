namespace Cabspot.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Cabspot.Models;
    using System.Data.Entity.Validation;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<CabspotDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;

            // register mysql code generator
            SetSqlGenerator("MySql.Data.MySqlClient",  new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(CabspotDB context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //seeding the database-----------------------------------------------------------------
            
                //condicionVehiculo
                context.condicionvehiculos.AddOrUpdate(
                    p => p.condicionVehiculo,
                    new condicionvehiculos { condicionVehiculo = "Excelente" },
                    new condicionvehiculos { condicionVehiculo = "Buena" },
                    new condicionvehiculos { condicionVehiculo = "Regular" },
                    new condicionvehiculos { condicionVehiculo = "Mala" }
                );

                //estadoCarreras
                context.estadocarreras.AddOrUpdate(
                    p => p.estadoCarrera,
                    new estadocarreras { estadoCarrera = "En Espera" },
                    new estadocarreras { estadoCarrera = "Completada" },
                    new estadocarreras { estadoCarrera = "Cancelada" },
                    new estadocarreras { estadoCarrera = "En Curso" }
                 );

                ////estadoDisponibilidad
                context.estadodisponibilidad.AddOrUpdate(
                    p => p.estadoDisponibilidad,
                    new estadodisponibilidad { estadoDisponibilidad = "Disponible" },
                    new estadodisponibilidad { estadoDisponibilidad = "Ocupado" },
                    new estadodisponibilidad { estadoDisponibilidad = "Offline" },
                    new estadodisponibilidad { estadoDisponibilidad = "No Activo" }
                 );

                //estadoEmpleados
                context.estadoempleados.AddOrUpdate(
                    p => p.estadoEmpleado,
                    new estadoempleados { estadoEmpleado = "Activo" },
                    new estadoempleados { estadoEmpleado = "Temp Inactivo" },
                    new estadoempleados { estadoEmpleado = "Inactivo" }

                );

                //estadoEncuestas
                context.estadoencuestas.AddOrUpdate(
                    p => p.estadoEncuesta,
                    new estadoencuestas { estadoEncuesta = "Completada" },
                    new estadoencuestas { estadoEncuesta = "Rechazada" },
                    new estadoencuestas { estadoEncuesta = "En Espera" }
                );

                //estadoSolicitud
                context.estadosolicitud.AddOrUpdate(
                    p => p.estadoSolicitud,
                    new estadosolicitud { estadoSolicitud = "Rechazada" },
                    new estadosolicitud { estadoSolicitud = "Aceptada" },
                    new estadosolicitud { estadoSolicitud = "En Espera" }
                );

                //estadoVehiculos
                context.estadovehiculos.AddOrUpdate(
                     p => p.estadoVehiculo,
                     new estadovehiculos { estadoVehiculo = "Activo" },
                     new estadovehiculos { estadoVehiculo = "Inactivo" },
                     new estadovehiculos { estadoVehiculo = "Temp Inactivo" }
                );

                //metodoPago
                context.metodopago.AddOrUpdate(
                    p => p.metodoPago,
                    new metodopago { metodoPago = "Efectivo" },
                    new metodopago { metodoPago = "Electronico" }
                );

                //preguntas
                context.preguntas.AddOrUpdate(
                    p => p.pregunta,
                    new preguntas { pregunta = "Condicion Vehiculo" },
                    new preguntas { pregunta = "Cortesia Conductor" },
                    new preguntas { pregunta = "Manejo Conductor" }
                );

                //roles
                context.roles.AddOrUpdate(
                    p => p.rol,
                    new roles { rol = "Super Admin" },
                    new roles { rol = "Admin" },
                    new roles { rol = "Power User" },
                    new roles { rol = "User" },
                    new roles { rol = "Developer" }

                );

                //tipoVehiculos
                context.tipovehiculos.AddOrUpdate(
                    p => p.tipoVehiculo,
                    new tipovehiculos { tipoVehiculo = "Sedan" },
                    new tipovehiculos { tipoVehiculo = "Station" },
                    new tipovehiculos { tipoVehiculo = "HatchBack" },
                    new tipovehiculos { tipoVehiculo = "Jeep" },
                    new tipovehiculos { tipoVehiculo = "Camioneta" },
                    new tipovehiculos { tipoVehiculo = "Minibus" },
                    new tipovehiculos { tipoVehiculo = "Van" }
                );

                //viaSolicitud
                context.viasolicitud.AddOrUpdate(
                    p => p.viaSolicitud,
                    new viasolicitud { viaSolicitud = "Movil" },
                    new viasolicitud { viaSolicitud = "Web" },
                    new viasolicitud { viaSolicitud = "SMS" }
                );


                //saving
                base.Seed(context);
                SaveChanges(context);

            
                            
            //-------------------------------------------------------------------------------------
        }


        private void SaveChanges(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }
    }
}

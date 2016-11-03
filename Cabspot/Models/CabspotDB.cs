namespace Cabspot.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CabspotDB : DbContext
    {
        public CabspotDB()
            : base("name=CabspotDB")
        {
        }
        
        public virtual DbSet<autenticacionsms> autenticacionSms { get; set; }
        public virtual DbSet<autenticacionsmstaxista> autenticacionSmsTaxista { get; set; }
        public virtual DbSet<bases> bases { get; set; }
        public virtual DbSet<carreras> carreras { get; set; }
        public virtual DbSet<clientes> clientes { get; set; }
        public virtual DbSet<comentarios> comentarios { get; set; }
        public virtual DbSet<condicionvehiculos> condicionvehiculos { get; set; }
        public virtual DbSet<contactos> contactos { get; set; }
        public virtual DbSet<direcciones> direcciones { get; set; }
        public virtual DbSet<empleados> empleados { get; set; }
        public virtual DbSet<encuestas> encuestas { get; set; }
        public virtual DbSet<estadocarreras> estadocarreras { get; set; }
        public virtual DbSet<estadodisponibilidad> estadodisponibilidad { get; set; }
        public virtual DbSet<estadoempleados> estadoempleados { get; set; }
        public virtual DbSet<estadoencuestas> estadoencuestas { get; set; }
        public virtual DbSet<estadosolicitud> estadosolicitud { get; set; }
        public virtual DbSet<estadovehiculos> estadovehiculos { get; set; }
        public virtual DbSet<metodopago> metodopago { get; set; }
        public virtual DbSet<municipios> municipios { get; set; }
        public virtual DbSet<personas> personas { get; set; }
        public virtual DbSet<preguntas> preguntas { get; set; }
        public virtual DbSet<provincias> provincias { get; set; }
        public virtual DbSet<roles> roles { get; set; }
        public virtual DbSet<solicitudes> solicitudes { get; set; }
        public virtual DbSet<sugerencias> sugerencias { get; set; }
        public virtual DbSet<taxistas> taxistas { get; set; }
        public virtual DbSet<tipovehiculos> tipovehiculos { get; set; }
        public virtual DbSet<vehiculos> vehiculos { get; set; }
        public virtual DbSet<viasolicitud> viasolicitud { get; set; }
        public virtual DbSet<encuestaspreguntas> encuestaspreguntas { get; set; }
        public virtual DbSet<clientesMovil> clientesMovil { get; set; }
        public virtual DbSet<notificacionCliente> notificacionCliente { get; set; }
        public virtual DbSet<notificacionTaxista> notificacionTaxista { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<bases>()
                .Property(e => e.nombreBase)
                .IsUnicode(false);

            modelBuilder.Entity<clientes>()
                .Property(e => e.nombreUsuario)
                .IsUnicode(false);

            modelBuilder.Entity<comentarios>()
                .Property(e => e.comentario)
                .IsUnicode(false);

            modelBuilder.Entity<condicionvehiculos>()
                .Property(e => e.condicionVehiculo)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.telefonoMovil)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.telefonoResidencial)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.telefonoTrabajo)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.fax)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.emailAlternativo)
                .IsUnicode(false);

            modelBuilder.Entity<contactos>()
                .Property(e => e.paginaWeb)
                .IsUnicode(false);

            modelBuilder.Entity<direcciones>()
                .Property(e => e.numeroPuerta)
                .IsUnicode(false);

            modelBuilder.Entity<direcciones>()
                .Property(e => e.numeroEdificio)
                .IsUnicode(false);

            modelBuilder.Entity<direcciones>()
                .Property(e => e.nombreEdificio)
                .IsUnicode(false);

            modelBuilder.Entity<direcciones>()
                .Property(e => e.calle)
                .IsUnicode(false);

            modelBuilder.Entity<direcciones>()
                .Property(e => e.ciudad)
                .IsUnicode(false);

            modelBuilder.Entity<empleados>()
                .Property(e => e.usuario)
                .IsUnicode(false);

            modelBuilder.Entity<empleados>()
                .Property(e => e.contrasena)
                .IsUnicode(false);

            //modelBuilder.Entity<empleados>()
            //    .HasMany(e => e.empleados1)
            //    .WithOptional(e => e.empleados2)
            //    .HasForeignKey(e => e.registradoPor);

            //modelBuilder.Entity<empleados>()
            //    .HasMany(e => e.vehiculos)
            //    .WithOptional(e => e.empleados)
            //    .HasForeignKey(e => e.registradoPor);

            //modelBuilder.Entity<empleados>()
            //    .HasMany(e => e.vehiculos1)
            //    .WithOptional(e => e.empleados1)
            //    .HasForeignKey(e => e.modificadoPor);



            modelBuilder.Entity<estadocarreras>()
                .Property(e => e.estadoCarrera)
                .IsUnicode(false);

            modelBuilder.Entity<estadodisponibilidad>()
                .Property(e => e.estadoDisponibilidad)
                .IsUnicode(false);

            modelBuilder.Entity<estadoempleados>()
                .Property(e => e.estadoEmpleado)
                .IsUnicode(false);

            modelBuilder.Entity<estadoencuestas>()
                .Property(e => e.estadoEncuesta)
                .IsUnicode(false);

            modelBuilder.Entity<estadosolicitud>()
                .Property(e => e.estadoSolicitud)
                .IsUnicode(false);

            modelBuilder.Entity<estadovehiculos>()
                .Property(e => e.estadoVehiculo)
                .IsUnicode(false);

            modelBuilder.Entity<metodopago>()
                .Property(e => e.metodoPago)
                .IsUnicode(false);

            modelBuilder.Entity<metodopago>()
                .HasMany(e => e.carreras)
                .WithRequired(e => e.metodopago)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<municipios>()
                .Property(e => e.nombreMunicipio)
                .IsUnicode(false);

            modelBuilder.Entity<personas>()
                .Property(e => e.identificacion)
                .IsUnicode(false);

            modelBuilder.Entity<personas>()
                .Property(e => e.nombres)
                .IsUnicode(false);

            modelBuilder.Entity<personas>()
                .Property(e => e.apellidos)
                .IsUnicode(false);

            modelBuilder.Entity<personas>()
                .Property(e => e.sexo)
                .IsUnicode(false);

            modelBuilder.Entity<personas>()
                .Property(e => e.foto)
                .IsUnicode(false);

            modelBuilder.Entity<personas>()
                .Property(e => e.nacionalidad)
                .IsUnicode(false);

            modelBuilder.Entity<preguntas>()
                .Property(e => e.pregunta)
                .IsUnicode(false);

            modelBuilder.Entity<provincias>()
                .Property(e => e.nombreProvincia)
                .IsUnicode(false);

            modelBuilder.Entity<roles>()
                .Property(e => e.rol)
                .IsUnicode(false);

            modelBuilder.Entity<sugerencias>()
                .Property(e => e.sugerencia)
                .IsUnicode(false);

            modelBuilder.Entity<taxistas>()
                .Property(e => e.codigoTaxista)
                .IsUnicode(false);

            modelBuilder.Entity<tipovehiculos>()
                .Property(e => e.tipoVehiculo)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.chasis)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.placa)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.marca)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.modelo)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.serie)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.anio)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.color)
                .IsUnicode(false);

            modelBuilder.Entity<vehiculos>()
                .Property(e => e.unidad)
                .IsUnicode(false);

            modelBuilder.Entity<viasolicitud>()
                .Property(e => e.viaSolicitud)
                .IsUnicode(false);

            modelBuilder.Entity<viasolicitud>()
                .HasMany(e => e.carreras)
                .WithRequired(e => e.viasolicitud)
                .WillCascadeOnDelete(false);
        }
    }
}

using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UforyAPIREST.Models
{
    public partial class UforyDBContext : DbContext
    {
        public UforyDBContext()
        {
            
        }

        public UforyDBContext(DbContextOptions<UforyDBContext> options): base(options)
        {
        }

        public virtual DbSet<Archivos> Archivos { get; set; }
        public virtual DbSet<Comentarios> Comentarios { get; set; }
        public virtual DbSet<Notas> Notas { get; set; }
        public virtual DbSet<Proyectos> Proyectos { get; set; }
        public virtual DbSet<Dibujos> Dibujos { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Tareas> Tareas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetValue<string>("ConnectionStrings:ConnectionString");
                
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Archivos>(entity =>
            {
                entity.HasKey(e => e.IdArchivo)
                    .HasName("PK__Archivos__9B69644338CD79AF");

                entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");

                entity.Property(e => e.Duracion).HasColumnName("duracion");

                entity.Property(e => e.Subida)
                    .HasColumnName("subida")
                    .HasColumnType("datetime");

                entity.Property(e => e.UltimaMod)
                    .HasColumnName("ultima_mod")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.NombreCloud)
                    .IsRequired()
                    .HasColumnName("nombre_cloud")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Peso).HasColumnName("peso");

                entity.Property(e => e.ResH).HasColumnName("res_h");

                entity.Property(e => e.ResV).HasColumnName("res_v");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasColumnName("tipo")
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tareas>(entity =>
            {
                entity.HasKey(e => e.IdTarea)
                    .HasName("PK__Tareas__9BE8C4888BD12BA1");

                entity.Property(e => e.IdTarea).HasColumnName("id_tarea");

                entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasColumnName("contenido")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Completada)
                    .HasColumnName("completada")
                    .IsRequired();

                entity.Property(e => e.Posicion).HasColumnName("posicion");
            });

            modelBuilder.Entity<Comentarios>(entity =>
            {
                entity.HasKey(e => e.IdComentario)
                    .HasName("PK__Comentar__1BA6C6F48A0A9E00");

                entity.Property(e => e.IdComentario).HasColumnName("id_comentario");

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasColumnName("contenido")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.Identificador)
                    .IsRequired()
                    .HasColumnName("identificador")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");

                entity.Property(e => e.TiempoFin).HasColumnName("tiempo_fin");

                entity.Property(e => e.TiempoInicio).HasColumnName("tiempo_inicio");

                entity.Property(e => e.PosH).HasColumnName("pos_h");

                entity.Property(e => e.PosV).HasColumnName("pos_v");
            });

            /*modelBuilder.Entity<Ideas>(entity =>
            {
                entity.HasKey(e => e.IdIdea)
                    .HasName("PK__Ideas__9BE8C4888BD12BA1");

                entity.Property(e => e.IdIdea).HasColumnName("id_idea");

                entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");

                entity.Property(e => e.TiempoFinS).HasColumnName("tiempo_fin_s");

                entity.Property(e => e.TiempoInicioS).HasColumnName("tiempo_inicio_s");
            });*/

            /*modelBuilder.Entity<Marcas>(entity =>
            {
                entity.HasKey(e => e.IdMarca)
                    .HasName("PK__Marcas__7E43E99E1D1F314D");

                entity.Property(e => e.IdMarca).HasColumnName("id_marca");

                entity.Property(e => e.IdComentario).HasColumnName("id_comentario");

                entity.Property(e => e.PosH).HasColumnName("pos_h");

                entity.Property(e => e.PosV).HasColumnName("pos_v");
            });*/

            modelBuilder.Entity<Notas>(entity =>
            {
                entity.HasKey(e => e.IdNota)
                    .HasName("PK__Notas__26991D8CDF6588FD");

                entity.Property(e => e.IdNota).HasColumnName("id_nota");

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasColumnName("contenido")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.Creacion)
                    .HasColumnName("creacion")
                    .HasColumnType("datetime");

                entity.Property(e => e.UltimaMod)
                    .HasColumnName("ultima_mod")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(75)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Proyectos>(entity =>
            {
                entity.HasKey(e => e.IdProyecto)
                    .HasName("PK__Proyecto__F38AD81DF9DAC36F");

                entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");

                entity.Property(e => e.Creacion)
                    .HasColumnName("creacion")
                    .HasColumnType("datetime");

                entity.Property(e => e.UltimaMod)
                    .HasColumnName("ultima_mod")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(75)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Dibujos>(entity =>
            {
                entity.HasKey(e => e.IdDibujo)
                    .HasName("PK__Dibujos__7F2D34D537469BF7");

                entity.Property(e => e.IdDibujo).HasColumnName("id_dibujo");

                entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");

                entity.Property(e => e.TiempoInicio).HasColumnName("tiempo_inicio");

                entity.Property(e => e.TiempoFin).HasColumnName("tiempo_fin");

                entity.Property(e => e.NombreCloud)
                    .IsRequired()
                    .HasColumnName("nombre_cloud")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Identificador)
                    .IsRequired()
                    .HasColumnName("identificador")
                    .HasMaxLength(36)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__Usuarios__4E3E04ADF0EC3FD6");

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasColumnName("pass")
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Suscripcion).HasColumnName("suscripcion");

                entity.Property(e => e.SaltPass)
                    .IsRequired()
                    .HasColumnName("salt_pass")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Sesion)
                    .IsRequired()
                    .HasColumnName("sesion")
                    .HasMaxLength(36)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

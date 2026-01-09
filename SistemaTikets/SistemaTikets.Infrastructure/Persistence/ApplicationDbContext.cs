using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Prioridad> Prioridades { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<TipoSolicitud> TiposSolicitud { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Solicitud> Solicitudes { get; set; }
    public DbSet<TrazabilidadSolicitud> TrazabilidadesSolicitud { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<Encargado> Encargados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuracion de Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuracion de Area
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("areas");
            entity.HasKey(e => e.IdArea);
            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuracion de Prioridad
        modelBuilder.Entity<Prioridad>(entity =>
        {
            entity.ToTable("prioridades");
            entity.HasKey(e => e.IdPrioridad);
            entity.Property(e => e.IdPrioridad).HasColumnName("id_prioridad");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuracion de Estado
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.ToTable("estados");
            entity.HasKey(e => e.IdEstado);
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuracion de TipoSolicitud
        modelBuilder.Entity<TipoSolicitud>(entity =>
        {
            entity.ToTable("tipos_solicitud");
            entity.HasKey(e => e.IdTipoSolicitud);
            entity.Property(e => e.IdTipoSolicitud).HasColumnName("id_tipo_solicitud");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.Property(e => e.IdArea).HasColumnName("id_area");

            entity.HasOne(e => e.Area)
                .WithMany(a => a.TiposSolicitud)
                .HasForeignKey(e => e.IdArea)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuracion de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdAreaAsignada).HasColumnName("id_area_asignada");
            entity.Property(e => e.IdCreadoPor).HasColumnName("id_creado_por");
            entity.Property(e => e.FechaCreacionUsuario).HasColumnName("fecha_creacion_usuario").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);

            entity.HasIndex(e => e.Username).IsUnique();

            entity.HasOne(e => e.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AreaAsignada)
                .WithMany(a => a.UsuariosAsignados)
                .HasForeignKey(e => e.IdAreaAsignada)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreadoPor)
                .WithMany(u => u.UsuariosCreados)
                .HasForeignKey(e => e.IdCreadoPor)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuracion de Solicitud
        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.ToTable("solicitudes");
            entity.HasKey(e => e.IdSolicitud);
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20);
            entity.Property(e => e.Asunto).HasColumnName("asunto").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.ArchivoUrl).HasColumnName("archivo_url");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.Property(e => e.IdTipoSolicitud).HasColumnName("id_tipo_solicitud");
            entity.Property(e => e.IdPrioridad).HasColumnName("id_prioridad");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdGestorAsignado).HasColumnName("id_gestor_asignado");
            entity.Property(e => e.IdAsignadoPor).HasColumnName("id_asignado_por");
            entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");

            entity.HasIndex(e => e.Codigo).IsUnique();

            entity.HasOne(e => e.Solicitante)
                .WithMany(u => u.SolicitudesCreadas)
                .HasForeignKey(e => e.IdSolicitante)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Area)
                .WithMany(a => a.Solicitudes)
                .HasForeignKey(e => e.IdArea)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TipoSolicitud)
                .WithMany(t => t.Solicitudes)
                .HasForeignKey(e => e.IdTipoSolicitud)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Prioridad)
                .WithMany(p => p.Solicitudes)
                .HasForeignKey(e => e.IdPrioridad)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Estado)
                .WithMany(es => es.Solicitudes)
                .HasForeignKey(e => e.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.GestorAsignado)
                .WithMany(u => u.SolicitudesAsignadas)
                .HasForeignKey(e => e.IdGestorAsignado)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.AsignadoPor)
                .WithMany(u => u.SolicitudesAsignadasPor)
                .HasForeignKey(e => e.IdAsignadoPor)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuracion de TrazabilidadSolicitud
        modelBuilder.Entity<TrazabilidadSolicitud>(entity =>
        {
            entity.ToTable("trazabilidad_solicitudes");
            entity.HasKey(e => e.IdTrazabilidad);
            entity.Property(e => e.IdTrazabilidad).HasColumnName("id_trazabilidad");
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.IdUsuarioActor).HasColumnName("id_usuario_actor");
            entity.Property(e => e.Accion).HasColumnName("accion").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.FechaEvento).HasColumnName("fecha_evento").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Solicitud)
                .WithMany(s => s.Trazabilidades)
                .HasForeignKey(e => e.IdSolicitud)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UsuarioActor)
                .WithMany(u => u.Trazabilidades)
                .HasForeignKey(e => e.IdUsuarioActor)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuracion de Comentario
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.ToTable("comentarios");
            entity.HasKey(e => e.IdComentario);
            entity.Property(e => e.IdComentario).HasColumnName("id_comentario");
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Texto).HasColumnName("texto").IsRequired();
            entity.Property(e => e.FechaComentario).HasColumnName("fecha_comentario").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Solicitud)
                .WithMany(s => s.Comentarios)
                .HasForeignKey(e => e.IdSolicitud)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuracion de Encargado
        modelBuilder.Entity<Encargado>(entity =>
        {
            entity.ToTable("encargados");
            entity.HasKey(e => e.IdEncargado);
            entity.Property(e => e.IdEncargado).HasColumnName("id_encargado");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);

            entity.HasIndex(e => new { e.IdUsuario, e.IdArea }).IsUnique();

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.EncargadosDeAreas)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Area)
                .WithMany(a => a.Encargados)
                .HasForeignKey(e => e.IdArea)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

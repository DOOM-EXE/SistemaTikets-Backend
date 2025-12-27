using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaTikets.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id_area = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.id_area);
                });

            migrationBuilder.CreateTable(
                name: "estados",
                columns: table => new
                {
                    id_estado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados", x => x.id_estado);
                });

            migrationBuilder.CreateTable(
                name: "prioridades",
                columns: table => new
                {
                    id_prioridad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prioridades", x => x.id_prioridad);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "tipos_solicitud",
                columns: table => new
                {
                    id_tipo_solicitud = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_area = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_solicitud", x => x.id_tipo_solicitud);
                    table.ForeignKey(
                        name: "FK_tipos_solicitud_areas_id_area",
                        column: x => x.id_area,
                        principalTable: "areas",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_completo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    id_rol = table.Column<int>(type: "integer", nullable: false),
                    id_area_asignada = table.Column<int>(type: "integer", nullable: true),
                    id_creado_por = table.Column<int>(type: "integer", nullable: true),
                    fecha_creacion_usuario = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_usuarios_areas_id_area_asignada",
                        column: x => x.id_area_asignada,
                        principalTable: "areas",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarios_usuarios_id_creado_por",
                        column: x => x.id_creado_por,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "solicitudes",
                columns: table => new
                {
                    id_solicitud = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    asunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    archivo_url = table.Column<string>(type: "text", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    id_solicitante = table.Column<int>(type: "integer", nullable: false),
                    id_area = table.Column<int>(type: "integer", nullable: false),
                    id_tipo_solicitud = table.Column<int>(type: "integer", nullable: false),
                    id_prioridad = table.Column<int>(type: "integer", nullable: false),
                    id_estado = table.Column<int>(type: "integer", nullable: false),
                    id_gestor_asignado = table.Column<int>(type: "integer", nullable: true),
                    id_asignado_por = table.Column<int>(type: "integer", nullable: true),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitudes", x => x.id_solicitud);
                    table.ForeignKey(
                        name: "FK_solicitudes_areas_id_area",
                        column: x => x.id_area,
                        principalTable: "areas",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitudes_estados_id_estado",
                        column: x => x.id_estado,
                        principalTable: "estados",
                        principalColumn: "id_estado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitudes_prioridades_id_prioridad",
                        column: x => x.id_prioridad,
                        principalTable: "prioridades",
                        principalColumn: "id_prioridad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitudes_tipos_solicitud_id_tipo_solicitud",
                        column: x => x.id_tipo_solicitud,
                        principalTable: "tipos_solicitud",
                        principalColumn: "id_tipo_solicitud",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitudes_usuarios_id_asignado_por",
                        column: x => x.id_asignado_por,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitudes_usuarios_id_gestor_asignado",
                        column: x => x.id_gestor_asignado,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitudes_usuarios_id_solicitante",
                        column: x => x.id_solicitante,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comentarios",
                columns: table => new
                {
                    id_comentario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_solicitud = table.Column<int>(type: "integer", nullable: false),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    texto = table.Column<string>(type: "text", nullable: false),
                    fecha_comentario = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios", x => x.id_comentario);
                    table.ForeignKey(
                        name: "FK_comentarios_solicitudes_id_solicitud",
                        column: x => x.id_solicitud,
                        principalTable: "solicitudes",
                        principalColumn: "id_solicitud",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comentarios_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trazabilidad_solicitudes",
                columns: table => new
                {
                    id_trazabilidad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_solicitud = table.Column<int>(type: "integer", nullable: false),
                    id_usuario_actor = table.Column<int>(type: "integer", nullable: true),
                    accion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    fecha_evento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trazabilidad_solicitudes", x => x.id_trazabilidad);
                    table.ForeignKey(
                        name: "FK_trazabilidad_solicitudes_solicitudes_id_solicitud",
                        column: x => x.id_solicitud,
                        principalTable: "solicitudes",
                        principalColumn: "id_solicitud",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trazabilidad_solicitudes_usuarios_id_usuario_actor",
                        column: x => x.id_usuario_actor,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_areas_nombre",
                table: "areas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_id_solicitud",
                table: "comentarios",
                column: "id_solicitud");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_id_usuario",
                table: "comentarios",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_estados_nombre",
                table: "estados",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prioridades_nombre",
                table: "prioridades",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_codigo",
                table: "solicitudes",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_area",
                table: "solicitudes",
                column: "id_area");

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_asignado_por",
                table: "solicitudes",
                column: "id_asignado_por");

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_estado",
                table: "solicitudes",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_gestor_asignado",
                table: "solicitudes",
                column: "id_gestor_asignado");

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_prioridad",
                table: "solicitudes",
                column: "id_prioridad");

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_solicitante",
                table: "solicitudes",
                column: "id_solicitante");

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_id_tipo_solicitud",
                table: "solicitudes",
                column: "id_tipo_solicitud");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_solicitud_id_area",
                table: "tipos_solicitud",
                column: "id_area");

            migrationBuilder.CreateIndex(
                name: "IX_trazabilidad_solicitudes_id_solicitud",
                table: "trazabilidad_solicitudes",
                column: "id_solicitud");

            migrationBuilder.CreateIndex(
                name: "IX_trazabilidad_solicitudes_id_usuario_actor",
                table: "trazabilidad_solicitudes",
                column: "id_usuario_actor");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_area_asignada",
                table: "usuarios",
                column: "id_area_asignada");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_creado_por",
                table: "usuarios",
                column: "id_creado_por");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_username",
                table: "usuarios",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentarios");

            migrationBuilder.DropTable(
                name: "trazabilidad_solicitudes");

            migrationBuilder.DropTable(
                name: "solicitudes");

            migrationBuilder.DropTable(
                name: "estados");

            migrationBuilder.DropTable(
                name: "prioridades");

            migrationBuilder.DropTable(
                name: "tipos_solicitud");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalMicrocursos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIndicesOptimizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_LeccionesCompletadas_UsuarioId_LeccionId",
                table: "LeccionesCompletadas",
                newName: "IX_LeccionesCompletadas_Usuario_Leccion");

            migrationBuilder.RenameIndex(
                name: "IX_Lecciones_CursoId_Orden",
                table: "Lecciones",
                newName: "IX_Lecciones_Curso_Orden");

            migrationBuilder.RenameIndex(
                name: "IX_Inscripciones_UsuarioId_CursoId",
                table: "Inscripciones",
                newName: "IX_Inscripciones_Usuario_Curso");

            migrationBuilder.CreateIndex(
                name: "IX_LeccionesCompletadas_FechaCompletada",
                table: "LeccionesCompletadas",
                column: "FechaCompletada");

            migrationBuilder.CreateIndex(
                name: "IX_LeccionesCompletadas_UsuarioId",
                table: "LeccionesCompletadas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecciones_CursoId",
                table: "Lecciones",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_FechaInscripcion",
                table: "Inscripciones",
                column: "FechaInscripcion");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_UsuarioId",
                table: "Inscripciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_FechaCreacion",
                table: "Cursos",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Nivel",
                table: "Cursos",
                column: "Nivel");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Publicado_Nivel",
                table: "Cursos",
                columns: new[] { "EstaPublicado", "Nivel" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeccionesCompletadas_FechaCompletada",
                table: "LeccionesCompletadas");

            migrationBuilder.DropIndex(
                name: "IX_LeccionesCompletadas_UsuarioId",
                table: "LeccionesCompletadas");

            migrationBuilder.DropIndex(
                name: "IX_Lecciones_CursoId",
                table: "Lecciones");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_FechaInscripcion",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_UsuarioId",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_FechaCreacion",
                table: "Cursos");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_Nivel",
                table: "Cursos");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_Publicado_Nivel",
                table: "Cursos");

            migrationBuilder.RenameIndex(
                name: "IX_LeccionesCompletadas_Usuario_Leccion",
                table: "LeccionesCompletadas",
                newName: "IX_LeccionesCompletadas_UsuarioId_LeccionId");

            migrationBuilder.RenameIndex(
                name: "IX_Lecciones_Curso_Orden",
                table: "Lecciones",
                newName: "IX_Lecciones_CursoId_Orden");

            migrationBuilder.RenameIndex(
                name: "IX_Inscripciones_Usuario_Curso",
                table: "Inscripciones",
                newName: "IX_Inscripciones_UsuarioId_CursoId");
        }
    }
}

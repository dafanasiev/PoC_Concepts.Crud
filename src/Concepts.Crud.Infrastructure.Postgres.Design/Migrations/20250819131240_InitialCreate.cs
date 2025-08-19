using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concepts.Crud.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "crud");

            migrationBuilder.CreateTable(
                name: "activity_type",
                schema: "crud",
                columns: table => new
                {
                    activity_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    gc = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_type", x => x.activity_type_id);
                });

            migrationBuilder.CreateTable(
                name: "client_request",
                schema: "crud",
                columns: table => new
                {
                    client_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_request", x => x.client_request_id);
                });

            migrationBuilder.CreateTable(
                name: "entity_ref",
                schema: "crud",
                columns: table => new
                {
                    entity_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                    referred_class_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    gc = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_ref", x => x.entity_ref_id);
                });

            migrationBuilder.CreateTable(
                name: "relationship_type",
                schema: "crud",
                columns: table => new
                {
                    relationship_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    gc = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relationship_type", x => x.relationship_type_id);
                });

            migrationBuilder.CreateTable(
                name: "activity",
                schema: "crud",
                columns: table => new
                {
                    activity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_group = table.Column<bool>(type: "boolean", nullable: false),
                    activity_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gc = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity", x => x.activity_id);
                    table.ForeignKey(
                        name: "FK_activity_activity_type_activity_type_id",
                        column: x => x.activity_type_id,
                        principalSchema: "crud",
                        principalTable: "activity_type",
                        principalColumn: "activity_type_id");
                });

            migrationBuilder.CreateTable(
                name: "activity_relationship",
                schema: "crud",
                columns: table => new
                {
                    activity_relationship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    activity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gc = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_relationship", x => x.activity_relationship_id);
                    table.ForeignKey(
                        name: "FK_activity_relationship_activity_activity_id",
                        column: x => x.activity_id,
                        principalSchema: "crud",
                        principalTable: "activity",
                        principalColumn: "activity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_activity_relationship_entity_ref_entity_ref_id",
                        column: x => x.entity_ref_id,
                        principalSchema: "crud",
                        principalTable: "entity_ref",
                        principalColumn: "entity_ref_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_activity_relationship_relationship_type_relationship_type_id",
                        column: x => x.relationship_type_id,
                        principalSchema: "crud",
                        principalTable: "relationship_type",
                        principalColumn: "relationship_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_activity_type_id",
                schema: "crud",
                table: "activity",
                column: "activity_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_relationship_activity_id",
                schema: "crud",
                table: "activity_relationship",
                column: "activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_relationship_entity_ref_id",
                schema: "crud",
                table: "activity_relationship",
                column: "entity_ref_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_relationship_relationship_type_id",
                schema: "crud",
                table: "activity_relationship",
                column: "relationship_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_relationship",
                schema: "crud");

            migrationBuilder.DropTable(
                name: "client_request",
                schema: "crud");

            migrationBuilder.DropTable(
                name: "activity",
                schema: "crud");

            migrationBuilder.DropTable(
                name: "entity_ref",
                schema: "crud");

            migrationBuilder.DropTable(
                name: "relationship_type",
                schema: "crud");

            migrationBuilder.DropTable(
                name: "activity_type",
                schema: "crud");
        }
    }
}

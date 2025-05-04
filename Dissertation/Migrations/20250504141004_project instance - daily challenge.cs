using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dissertation.Migrations
{
    /// <inheritdoc />
    public partial class projectinstancedailychallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppliedChallengeKey",
                table: "ProjectInstances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChallengeAppliedDate",
                table: "ProjectInstances",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedChallengeKey",
                table: "ProjectInstances");

            migrationBuilder.DropColumn(
                name: "LastChallengeAppliedDate",
                table: "ProjectInstances");
        }
    }
}

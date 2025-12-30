using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserFollowRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "UserFollows",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_AuthorId",
                table: "UserFollows",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_AspNetUsers_AuthorId",
                table: "UserFollows",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_AspNetUsers_AuthorId",
                table: "UserFollows");

            migrationBuilder.DropIndex(
                name: "IX_UserFollows_AuthorId",
                table: "UserFollows");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "UserFollows");
        }
    }
}

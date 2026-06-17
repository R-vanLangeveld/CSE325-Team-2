using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSE325_Team_2.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanCollaboratorsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTasks_Users_AssigneeId",
                table: "EventTasks");

            migrationBuilder.DropIndex(
                name: "IX_EventTasks_AssigneeId",
                table: "EventTasks");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "EventTasks");

            migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "EventTasks",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignee",
                table: "EventTasks");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "EventTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventTasks_AssigneeId",
                table: "EventTasks",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTasks_Users_AssigneeId",
                table: "EventTasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

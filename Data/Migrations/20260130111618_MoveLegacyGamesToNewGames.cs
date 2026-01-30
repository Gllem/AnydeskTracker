using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveLegacyGamesToNewGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add games from "Games" table
            migrationBuilder.Sql(@"
                INSERT OR IGNORE INTO GameCatalog (Url, Name)
                SELECT
                  lower(trim(g.GameUrl)) AS Url,
                  trim(g.GameName) AS Name
                FROM Games g
                WHERE trim(ifnull(g.GameUrl, '')) <> '';
            ");

            // Add games from "BotGames" table
            migrationBuilder.Sql(@"
                INSERT OR IGNORE INTO GameCatalog (Url, Name)
				SELECT
				  lower(trim(bg.GameURL)) AS Url,
				  '' AS Name
				FROM BotGames bg
				WHERE trim(ifnull(bg.GameURL, '')) <> ''
				  AND NOT EXISTS (
				    SELECT 1
				    FROM GameCatalog c
				    WHERE c.Url = lower(trim(bg.GameURL))
				  );
            ");

            // Move GameUserSchedule to GameSchedule
            migrationBuilder.Sql(@"
				INSERT INTO GameSchedule (Id, GameId, DayOfWeek)
				SELECT
				  gus.Id,
				  c.Id as GameId,
				  gus.DayOfWeek
				FROM GameUserSchedule gus
				JOIN Games g ON g.Id = gus.GameId
				JOIN GameCatalog c ON c.Url = lower(trim(g.GameUrl));
			");

            // Move GameUserSchedulesUsers to GameUserScheduleToUser
            migrationBuilder.Sql(@"
				INSERT INTO GameUserScheduleToUser (GameUserScheduleId, UserId)
				SELECT
				  guu.AssignedSchedulesId as GameUserScheduleId,
				  guu.UsersId as UserId
				FROM GameUserSchedulesUsers guu;
			");

            // Move GlobalGameOrders
            migrationBuilder.Sql(@"
				INSERT INTO BotGameOrderGlobal (GameId, ""Order"")
				SELECT
				  c.Id as GameId,
				  bg.GlobalOrder as ""Order""
				FROM BotGames bg
				JOIN GameCatalog c ON c.Url = lower(trim(bg.GameURL))
				WHERE bg.IsGlobal = 1;
			");

            // Move OverridenGameOrders
            migrationBuilder.Sql(@"
				INSERT INTO BotGameOrderOverride (PcId, GameId, ""Order"")
				SELECT
				  pbtg.PcModelId as PcId,
				  c.Id as GameId,
				  pbtg.""Order"" as ""Order""
				FROM PcModelToBotGames pbtg
				JOIN BotGames bg ON bg.Id = pbtg.BotGameId
				JOIN GameCatalog c ON c.Url = lower(trim(bg.GameURL));
			");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

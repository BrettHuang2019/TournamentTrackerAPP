using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";

        public void CreatePrizeModel(PrizeModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }
        public void CreatePersonModel(PersonModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@Phone", model.CellphoneNumber);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        public void CreateTeamModel(TeamModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");


                foreach (PersonModel teamMember in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", teamMember.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

            }
        }

        public void CreateTournamentModel(TournamentModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentTeams(connection, model);
                SaveTournamentPrizes(connection, model);
                SaveTournamentRounds(connection, model);

            }
        }

        private void SaveTournament(IDbConnection connection, TournamentModel tournament)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", tournament.TournamentName);
            p.Add("@EntryFee", tournament.EntryFee);

            p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

            tournament.Id = p.Get<int>("@id");
        }

        private void SaveTournamentTeams(IDbConnection connection, TournamentModel tournament)
        {
            foreach (TeamModel team in tournament.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", tournament.Id);
                p.Add("@TeamId", team.Id);

                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel tournament)
        {
            foreach (PrizeModel prize in tournament.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", tournament.Id);
                p.Add("@PrizeId", prize.Id);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel tournament)
        {
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@id");

                    if (matchup.Entries.Count <= 0) return;

                    foreach (MatchupEntryModel teamEntry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);

                        if (teamEntry.ParentMatchup != null)
                            p.Add("@ParentMatchupId", teamEntry.ParentMatchup.Id);

                        if (teamEntry.TeamCompeting != null)
                            p.Add("@TeamCompetingId", teamEntry.TeamCompeting.Id);

                        p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }
            return output;
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return output;
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            List<TeamModel> allTeams = GetTeam_All();
            List<MatchupModel> allMatchups = GetMatchup_All();


            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournament_GetAll").ToList();

                foreach (TournamentModel t in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", t.Id);

                    string[] teamIds = connection.Query<string>("dbo.spTournamentEntryTeamId_GetByTournament", p, commandType: CommandType.StoredProcedure).ToArray();
                    t.EnteredTeams = allTeams.Where(team => teamIds.Contains(team.Id.ToString())).ToList();
                    t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();
                    t.Rounds = allMatchups.GroupBy(m => m.MatchupRound).Select(group => group.ToList()).ToList();
                }
            }
            return output;
        }

        public List<MatchupModel> GetMatchup_All()
        {
            List<MatchupModel> output;
            List<TeamModel> allTeams = GetTeam_All();

            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            { 
                output = connection.Query<MatchupModel>("dbo.spMatchups_GetAll").ToList();

                foreach(MatchupModel m in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@MatchupId", m.Id);
                    string winnerIdStr = connection.Query<string>("dbo.spMatchups_GetWinnerIdByMatchupId", p, commandType: CommandType.StoredProcedure).First();

                    if (int.TryParse(winnerIdStr, out int winnerId))
                    {
                        m.Winner = allTeams.First(team => team.Id == winnerId);
                    }

                    List<MatchupEntryQueryModel> matchupEntryQueryModels = connection.Query<MatchupEntryQueryModel>("dbo.spMatchupEntries_GetByMatchupId", p, commandType: CommandType.StoredProcedure).ToList();

                        List<MatchupEntryModel> entries = new List<MatchupEntryModel>();
                    if (matchupEntryQueryModels.Count > 0)
                    {
                        foreach (MatchupEntryQueryModel queryModel in matchupEntryQueryModels)
                        {
                            entries.Add(queryModel.GetMatchupEntryModel(output, allTeams));
                        }
                    }
                    m.Entries = entries;
                }
            }
            return output;
        }

        public void UpdateMatchResult(MatchupModel matchup)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@MatchupId", matchup.Id);
                p.Add("@WinnerId", matchup.Winner.Id);

                connection.Execute("spMatchups_UpdateWinnerByMatchupId", p, commandType: CommandType.StoredProcedure);

                foreach (MatchupEntryModel entry in matchup.Entries)
                {
                    p = new DynamicParameters();
                    p.Add("@MatchupId", matchup.Id);
                    p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                    p.Add("@NewScore", entry.Score);

                    connection.Execute("spMatchupEntries_UpdateScoreByMatchupIdAndTeamId", p, commandType: CommandType.StoredProcedure);
                }

                p = new DynamicParameters();
                p.Add("@ParentMatchupId", matchup.Id);
                p.Add("@TeamCompetingId", matchup.Winner.Id);

                connection.Execute("spMatchupEntries_UpdateTeamByParentMatchupId", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

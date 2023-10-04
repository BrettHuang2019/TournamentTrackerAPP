using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName) // PrizeModels.csv
        {
            return $@"{ConfigurationManager.AppSettings["filePath"]}\{fileName}";
        }

        public static List<string> LoadFile(this string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<string> { };
            }

            return File.ReadLines(filePath).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }
            return output;
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];
                output.Add(p);
            }
            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');
                t.TeamMembers = people.Where(p => personIds.Contains(p.Id.ToString())).ToList();
                if (!t.TeamMembers.Any()) { throw new Exception("No teamMember in this team."); }

                output.Add(t);
            }
            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines, string matchupEntriesFileName, string matchupFileName, string teamFileName, string peopleFileName, string prizeFileName)
        {
            List<TournamentModel> output = new List<TournamentModel>();

            List<TeamModel> allTeams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> allPrizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> allMatchups = matchupFileName.FullFilePath().LoadFile().ConvertToMatchupModels(matchupEntriesFileName,matchupFileName, teamFileName, peopleFileName);


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TournamentModel t = new TournamentModel();
                t.Id = int.Parse(cols[0]);
                t.TournamentName = cols[1];
                t.EntryFee = decimal.Parse(cols[2]);
                t.EnteredTeams = new List<TeamModel>();
                t.Prizes = new List<PrizeModel>();
                t.Rounds = new List<List<MatchupModel>>();

                string[] teamIds = cols[3].Split('|');
                t.EnteredTeams = allTeams.Where(t => teamIds.Contains(t.Id.ToString())).ToList();
                if (!t.EnteredTeams.Any()) { throw new Exception("No team in this tournament."); }

                string[] prizeIds = cols[4].Split('|');
                t.Prizes = allPrizes.Where(p => prizeIds.Contains(p.Id.ToString())).ToList();
                //if (!t.Prizes.Any()) { throw new Exception("No prize in this tournament."); }

                string[] roundsStrArr = cols[5].Split('|');
                if (!roundsStrArr.Any()) { throw new Exception("There is an empty round in this tournament."); }
                foreach (string round in roundsStrArr)
                {
                    string[] matchupIds = round.Split('^');
                    List<MatchupModel> matchupListInRound = allMatchups.Where(m => matchupIds.Contains(m.Id.ToString())).ToList();
                    t.Rounds.Add(matchupListInRound);
                }

                output.Add(t);
            }
            return output;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines, string matchupEntriesFileName, string matchupFileName, string teamFileName, string peopleFileName)
        {
            List<MatchupModel> output = new List<MatchupModel>();


            List<TeamModel> allTeams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);

            // Populate with empty entries
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel m = new MatchupModel();
                m.Id = int.Parse(cols[0]);
                m.MatchupRound = int.Parse(cols[1]);
                m.Winner = int.TryParse(cols[3], out int winnerTeamId) ? allTeams.First(m => m.Id == winnerTeamId) : null;

                output.Add(m);
            }

            List<MatchupEntryModel> allMatchupEntries = matchupEntriesFileName.FullFilePath().LoadFile().ConvertToMatchupEntriesModels(output, teamFileName, peopleFileName);

            // Populate with entries
            MatchupModel currentMatchup = null;
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                int id = int.Parse(cols[0]);
                currentMatchup = output.First(m => m.Id == id);

                string[] entryIds = cols[2].Split('|');
                currentMatchup.Entries = allMatchupEntries.Where(t => entryIds.Contains(t.Id.ToString())).ToList();
                if (!currentMatchup.Entries.Any()) { throw new Exception("No matched entry is found."); }
            }

            return output;
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntriesModels(this List<string> lines, List<MatchupModel> allMatchups, string teamFileName, string peopleFileName)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            //List<MatchupModel> allMatchups = matchupFileName.FullFilePath().LoadFile().ConvertToMatchupModels(matchupFileName, matchupEntriesFileName, teamFileName, peopleFileName);
            List<TeamModel> allTeams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupEntryModel p = new MatchupEntryModel();
                p.Id = int.Parse(cols[0]);

                if (allMatchups != null)
                {
                    p.Matchup = int.TryParse(cols[1], out int matchupId) ? allMatchups.FirstOrDefault(m => m.Id == matchupId) : null;
                    p.ParentMatchup = int.TryParse(cols[2], out int parentMatchupId) ? allMatchups.FirstOrDefault(m => m.Id == parentMatchupId) : null;
                }

                p.TeamCompeting = int.TryParse(cols[3], out int teamId) ? allTeams.First(m => m.Id == teamId) : null;
                p.Score = int.TryParse(cols[4], out int score) ? score : null;
                output.Add(p);
            }
            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PrizeModel p in models)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PersonModel p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellphoneNumber}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel t in models)
            {
                string teamMembersStr = string.Join("|", t.TeamMembers.Select(member => member.Id));
                lines.Add($"{t.Id},{t.TeamName},{teamMembersStr}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel t in models)
            {
                string enterTeamsStr = string.Join("|", t.EnteredTeams.Select(team => team.Id));
                string prizesStr = string.Join("|", t.Prizes.Select(prize => prize.Id));
                string roundsStr = string.Join("|", t.Rounds.Select(roundList => string.Join("^", roundList.Select(round => round.Id))));

                lines.Add($"{t.Id},{t.TournamentName},{t.EntryFee},{enterTeamsStr},{prizesStr},{roundsStr}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void  SaveToMatchFile(this List<MatchupModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (MatchupModel m in models)
            {
                string entries = string.Join("|", m.Entries.Select(entry => entry.Id));
                string winner = m.Winner == null ? string.Empty : m.Winner.Id.ToString();
                lines.Add($"{m.Id},{m.MatchupRound},{entries},{winner}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        public static void SaveToMatchEntriesFile(this List<MatchupEntryModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in models)
            {
                string matchupId = e.Matchup == null ? string.Empty : e.Matchup.Id.ToString();
                string parentMatchupId = e.ParentMatchup ==null ? string.Empty : e.ParentMatchup.Id.ToString();
                string teamCompetingId = e.TeamCompeting == null ? string.Empty : e.TeamCompeting.Id.ToString();
                string score = e.Score == null ? string.Empty : e.Score.ToString();
                lines.Add($"{e.Id},{matchupId},{parentMatchupId},{teamCompetingId},{score}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }


    }
}

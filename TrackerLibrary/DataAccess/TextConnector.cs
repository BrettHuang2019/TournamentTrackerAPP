using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public void CreatePrizeModel(PrizeModel model)
        {
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            int currentId = 1;
            if(prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }
            model.Id = currentId;
            prizes.Add(model);

            prizes.SaveToPrizeFile(GlobalConfig.PrizesFile);
        }
        public void CreatePersonModel(PersonModel model)
        {
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }
            model.Id = currentId;
            people.Add(model);

            people.SaveToPeopleFile(GlobalConfig.PeopleFile);
        }

        public void CreateTournamentModel(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels(GlobalConfig.MatchupEntriesFile, GlobalConfig.MatchupFile, GlobalConfig.TeamFile, GlobalConfig.PeopleFile, GlobalConfig.PrizesFile);
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }
            model.Id = currentId;

            CreateRounds(model.Rounds);

            tournaments.Add(model);

            tournaments.SaveToTournamentFile(GlobalConfig.TournamentFile);
        }

        public void CreateRounds(List<List<MatchupModel>> rounds)
        {
            List<MatchupModel> allMatchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels(GlobalConfig.MatchupEntriesFile, GlobalConfig.MatchupFile, GlobalConfig.TeamFile, GlobalConfig.PeopleFile);
            List<MatchupEntryModel> allMatchupEntries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntriesModels(allMatchups, GlobalConfig.TeamFile, GlobalConfig.PeopleFile);


            // Save all new matchupEntries with empty matchupId
            List<MatchupEntryModel> newMatchupEntries = rounds
                .SelectMany(matchupList => matchupList)
                .SelectMany(matchup => matchup.Entries)
                .ToList();
            int currentId = 1;
            if (allMatchupEntries.Count > 0)
                currentId = allMatchupEntries.Max(x => x.Id) + 1;

            foreach (MatchupEntryModel entry in newMatchupEntries)
            {
                entry.Id = currentId;
                allMatchupEntries.Add(entry);
                currentId++;
            }
            allMatchupEntries.SaveToMatchEntriesFile(GlobalConfig.MatchupEntriesFile);

            // Save all matchups
            List<MatchupModel> newMatchups = rounds.SelectMany(matchupList => matchupList).ToList();
            currentId = 1;
            if (allMatchups.Count > 0)
                currentId = allMatchups.Max(x => x.Id) + 1;

            foreach(MatchupModel matchup in newMatchups)
            {
                matchup.Id = currentId;
                foreach (MatchupEntryModel matchupEntryModel in matchup.Entries)
                {
                    matchupEntryModel.Matchup = matchup;
                }
                allMatchups.Add(matchup);
                currentId++;
            }
            allMatchups.SaveToMatchFile(GlobalConfig.MatchupFile);

            // Save all matchup entries again with no empty match reference. 
            allMatchupEntries.SaveToMatchEntriesFile(GlobalConfig.MatchupEntriesFile);
        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public void CreateTeamModel(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);
            int currentId = 1;
            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }
            model.Id = currentId;

            teams.Add(model);
            teams.SaveToTeamFile(GlobalConfig.TeamFile);
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);
        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels(GlobalConfig.MatchupEntriesFile, GlobalConfig.MatchupFile, GlobalConfig.TeamFile, GlobalConfig.PeopleFile, GlobalConfig.PrizesFile);
        }

        public void UpdateMatchResult(MatchupModel matchup)
        {
            List<MatchupModel> allMatchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels(GlobalConfig.MatchupEntriesFile, GlobalConfig.MatchupFile, GlobalConfig.TeamFile, GlobalConfig.PeopleFile);
            List<MatchupEntryModel> allMatchupEntries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntriesModels(allMatchups, GlobalConfig.TeamFile, GlobalConfig.PeopleFile);

            int currentMatchupIndex = allMatchups.FindIndex(m => m.Id == matchup.Id);
            allMatchups[currentMatchupIndex] = matchup;

            foreach(MatchupEntryModel entry in matchup.Entries)
            {
                int loadEntryIndex = allMatchupEntries.FindIndex(e=> e.Id == entry.Id);
                allMatchupEntries[loadEntryIndex] = entry;
            }

            allMatchups.SaveToMatchFile(GlobalConfig.MatchupFile);
            allMatchupEntries.SaveToMatchEntriesFile(GlobalConfig.MatchupEntriesFile);
        }
    }
}

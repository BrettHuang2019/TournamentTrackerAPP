using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {

        public static void CreateRounds(TournamentModel tournament)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(tournament.EnteredTeams);
            int rounds = FindNumberOfRounds(tournament.EnteredTeams.Count);
            int byes = (int)Math.Pow(2, rounds) - tournament.EnteredTeams.Count;
            tournament.Rounds.Add(CreateFirstRound(byes, randomizedTeams)); 

            CreateOtherRounds(tournament, rounds);
        }

        private static void CreateOtherRounds(TournamentModel tournament, int rounds)
        {
            if (rounds < 2) return;

            List<MatchupModel> previousRound = tournament.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();

            MatchupModel newMathcup = new MatchupModel();

            for (int i = 2; i <= rounds; i++)
            {
                previousRound = tournament.Rounds[i - 2];

                foreach (MatchupModel matchup in previousRound)
                {
                    newMathcup.MatchupRound = i;
                    newMathcup.Entries.Add(new MatchupEntryModel() { ParentMatchup = matchup });

                    if (newMathcup.Entries.Count >= 2)
                    {
                        currentRound.Add(newMathcup);
                        newMathcup = new MatchupModel();
                    }
                }

                tournament.Rounds.Add(currentRound);
                currentRound = new List<MatchupModel>();
            }

        }

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> firstRound = new List<MatchupModel>();

            int matches = (teams.Count + byes) / 2;

            MatchupModel matchup = new MatchupModel() { MatchupRound = 1 };

            for (int i = 0; i < teams.Count; i++)
            {
                matchup.Entries.Add(new MatchupEntryModel() { TeamCompeting = teams[i] });

                if(byes > 0 || matchup.Entries.Count >=2)
                {
                    firstRound.Add(matchup);
                    matchup = new MatchupModel() { MatchupRound = 1 };
                    byes--;
                }
            }
            return firstRound;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            if (teamCount <=0) return 0;
            int rounds = 1;
            int Matches = 1;
            while(Matches < teamCount)
            {
                rounds++;
                Matches = (int)Math.Pow(2, rounds);
            }

            return rounds;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(team => Guid.NewGuid()).ToList();

        }
    }
}

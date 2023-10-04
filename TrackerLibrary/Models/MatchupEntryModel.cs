using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        public int Id { get; set; }
        public MatchupModel Matchup { get; set; }
        public MatchupModel ParentMatchup { get; set; }
        public TeamModel TeamCompeting { get; set; }
        public double? Score { get; set; }

    }

    public class MatchupEntryQueryModel
    {
        public int Id { get; set; }
        public int MatchupId { get; set; }
        public int? ParentMatchupId { get; set; }
        public int? TeamCompetingId { get; set; }
        public double? Score { get; set; }

        public MatchupEntryModel GetMatchupEntryModel(List<MatchupModel> allMatchups, List<TeamModel> allTeams)
        {
            return new MatchupEntryModel
            {
                Id = Id,
                Matchup = allMatchups.First(matchup => matchup.Id == MatchupId),
                ParentMatchup = ParentMatchupId == null ? null : allMatchups.First(matchup => matchup.Id == ParentMatchupId),
                TeamCompeting = TeamCompetingId == null ? null : allTeams.First(team => team.Id == TeamCompetingId),
                Score = Score
            };
        }
    }
}

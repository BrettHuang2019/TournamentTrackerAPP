using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        public void CreatePrizeModel(PrizeModel model);
        public void CreatePersonModel(PersonModel model);
        public void CreateTeamModel(TeamModel model);
        public void CreateTournamentModel (TournamentModel model);
        public List<TeamModel> GetTeam_All();
        public List<PersonModel> GetPerson_All();
        public List<TournamentModel> GetTournament_All();
        public void UpdateMatchResult(MatchupModel matchup);
    }
}

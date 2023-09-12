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
        public PrizeModel CreatePrizeModel(PrizeModel model);
        public PersonModel CreatePersonModel(PersonModel model);
        public TeamModel CreateTeamModel(TeamModel model);  
        public List<PersonModel> GetPerson_All();
    }
}

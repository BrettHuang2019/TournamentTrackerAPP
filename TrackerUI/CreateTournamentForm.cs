using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        BindingList<TeamModel> availableTeams = new BindingList<TeamModel>(GlobalConfig.Connection.GetTeam_All());
        BindingList<TeamModel> selectedTeams = new BindingList<TeamModel> { };
        BindingList<PrizeModel> selectedPrizes = new BindingList<PrizeModel> { };

        private ITournamentRequester callingForm;

        public CreateTournamentForm(ITournamentRequester callingForm)
        {
            InitializeComponent();
            InitializaTeamLists();
            this.callingForm= callingForm;
        }

        private void InitializaTeamLists()
        {
            selectTeamDropdown.DataSource = availableTeams;
            selectTeamDropdown.DisplayMember = "TeamName";

            tournamentTeamsListbox.DataSource = selectedTeams;
            tournamentTeamsListbox.DisplayMember = "TeamName";

            prizesListbox.DataSource = selectedPrizes;
            prizesListbox.DisplayMember = "PlaceName";
        }

        private void headerLabel_Click(object sender, EventArgs e)
        {

        }

        private void roundLabel_Click(object sender, EventArgs e)
        {

        }

        private void selectTeamDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropdown.SelectedItem;
            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Call CreatePrize Form
            CreatePrizeForm createPrizeForm = new CreatePrizeForm(this);
            createPrizeForm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm createTeamForm = new CreateTeamForm(this);
            createTeamForm.Show();
        }

        private void deleteSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListbox.SelectedItem;
            if (p != null)
            {
                selectedPrizes.Remove(p);

            }
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListbox.SelectedItem;
            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            decimal entryFee;

            if (string.IsNullOrEmpty(tournamentNameValue.Text)) {
                MessageBox.Show("Tournament Name is required.");
                return;
            }

            if (decimal.TryParse(entryFeeValue.Text, out entryFee))
            {
                if (entryFee <= 0)
                {
                    MessageBox.Show("Tournament EntryFee should be greater than 0.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Tournament EntryFee is not a valid number.", "Invalid Fee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TournamentModel tournament = new TournamentModel();
            tournament.TournamentName = tournamentNameValue.Text;
            tournament.EntryFee = entryFee;
            tournament.Prizes = selectedPrizes.ToList();
            tournament.EnteredTeams = selectedTeams.ToList();

            TournamentLogic.CreateRounds(tournament);


            GlobalConfig.Connection.CreateTournamentModel(tournament);
            callingForm.TournamentComplete(tournament);

            this.Close();
        }
    }
}

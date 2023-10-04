using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;
using TrackerLibrary;

namespace TrackerUI
{
    public partial class TournamentDashboardForm : Form, ITournamentRequester
    {
        BindingList<TournamentModel> allTournaments = new BindingList<TournamentModel>(GlobalConfig.Connection.GetTournament_All());
        public TournamentDashboardForm()
        {
            InitializeComponent();
            InitTournamentList();
        }

        private void InitTournamentList()
        {
            loadExistedTournamentDropdown.DataSource = allTournaments;
            loadExistedTournamentDropdown.DisplayMember = "TournamentName";
        }

        private void loadExistedTournamentDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Call CreatePrize Form
            CreateTournamentForm createTournamentForm = new CreateTournamentForm(this);
            createTournamentForm.Show();
        }

        public void TournamentComplete(TournamentModel model)
        {
            allTournaments.Add(model);
        }

        private void loadTournamentButton_Click(object sender, EventArgs e)
        {
            TournamentModel t = (TournamentModel)loadExistedTournamentDropdown.SelectedItem;

            TournamentViewerForm tournamentViewerForm = new TournamentViewerForm(t);
            tournamentViewerForm.Show();
        }
    }
}

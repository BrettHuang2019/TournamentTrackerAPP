using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        private BindingList<PersonModel> avaliableTeamMembers = new BindingList<PersonModel>(GlobalConfig.Connection.GetPerson_All());
        private BindingList<PersonModel> selectedTeamMembers = new BindingList<PersonModel>();

        ITeamRequester callingForm;

        public CreateTeamForm(ITeamRequester callingForm)
        {
            InitializeComponent();
            //CreateSampleData();
            WireUpLists();
            this.callingForm = callingForm;
        }

        private void CreateSampleData()
        {
            avaliableTeamMembers.Add(new PersonModel { FirstName = "Tim", LastName = "Corey" });
            avaliableTeamMembers.Add(new PersonModel { FirstName = "Sue", LastName = "Storm" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "Jane", LastName = "Smith" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Bill", LastName = "Jones" });
        }

        private void WireUpLists()
        {
            selectTeamMemberDropdown.DataSource = avaliableTeamMembers;
            selectTeamMemberDropdown.DisplayMember = "FullName";

            teamMembersListbox.DataSource = selectedTeamMembers;
            teamMembersListbox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel person = new PersonModel();
                person.FirstName = firstNameValue.Text;
                person.LastName = lastNameValue.Text;
                person.EmailAddress = emailValue.Text;
                person.CellphoneNumber = cellphoneValue.Text;

                GlobalConfig.Connection.CreatePersonModel(person);
                selectedTeamMembers.Add(person);

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields");
            }
        }

        private bool ValidateForm()
        {
            if (firstNameValue.Text.Length == 0) { return false; }
            if (lastNameValue.Text.Length == 0) { return false; }
            if (emailValue.Text.Length == 0) { return false; }
            if (cellphoneValue.Text.Length == 0) { return false; }

            return true;
        }

        private void addTeamMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropdown.SelectedItem;
            if (p != null)
            {
                avaliableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListbox.SelectedItem;
            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                avaliableTeamMembers.Add(p);
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(teamNameValue.Text.Trim()) || selectedTeamMembers.Count <= 0) return;

            TeamModel model = new TeamModel();
            model.TeamName = teamNameValue.Text;
            model.TeamMembers = selectedTeamMembers.ToList();

            GlobalConfig.Connection.CreateTeamModel(model);

            callingForm.TeamComplete(model);
            this.Close();
        }

        private void selectTeamMemberDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

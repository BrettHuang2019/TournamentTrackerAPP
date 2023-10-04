using System.ComponentModel;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel currentTournament;
        private List<MatchupModel> matchupsInCurrentRound = new List<MatchupModel>();
        private List<MatchupModel> flattenMatchups = new List<MatchupModel>();
        private MatchupModel currentMatchup;

        public TournamentViewerForm(TournamentModel tournament)
        {
            InitializeComponent();
            currentTournament = tournament;
            InitializaRoundLists(tournament);

            // Init tournament name
            tournamentNameLabel.Text = tournament.TournamentName;

            // Reset unplayed toggle
            unplayedOnlyCheckbox.Checked = false;

            // Init Matchup Lists
            flattenMatchups = tournament.Rounds.SelectMany(round => round).ToList();
            matchupsInCurrentRound = flattenMatchups.Where(matchup => matchup.MatchupRound == GetCurrentRound()).ToList();
            RefreshMatchupLists(matchupsInCurrentRound);

            // Init first Matchup info
            currentMatchup = matchupsInCurrentRound[0];
            UpdateMatchupInfo(currentMatchup);
        }

        private void InitializaRoundLists(TournamentModel tournament)
        {
            string[] roundNames = new string[tournament.Rounds.Count];
            for (int i = 0; i < roundNames.Length; i++)
            {
                roundNames[i] = $"Round-{i + 1}";
            }
            roundDropdown.DataSource = roundNames;
            roundDropdown.DisplayMember = "ToString";
        }

        private int GetCurrentRound()
        {
            string currectRoundStr = roundDropdown.SelectedItem.ToString();
            if (int.TryParse(currectRoundStr.Substring(currectRoundStr.IndexOf('-') + 1), out int selectedRound))
            {
                return selectedRound;
            }
            return 1;
        }

        private void RefreshMatchupLists(List<MatchupModel> matchupList)
        {
            string[] matchupNames = new string[matchupList.Count];
            for (int i = 0; i < matchupNames.Length; i++)
            {
                matchupNames[i] = $"{matchupList[i].MatchupRound}-{i + 1}";
            }
            matchupListbox.DataSource = matchupNames;
            matchupListbox.DisplayMember = "ToString";
        }

        private void UpdateMatchupInfo(MatchupModel matchup)
        {
            teamOneScoreValue.Text = "";
            teamTwoScoreValue.Text = "";

            if (matchup.Entries.Count > 1)
            {
                teamOneNameLabel.Text = matchup.Entries[0].TeamCompeting == null ? "ToBeDetermined" : matchup.Entries[0].TeamCompeting.TeamName;
                teamTwoNameLabel.Text = matchup.Entries[1].TeamCompeting == null ? "ToBeDetermined" : matchup.Entries[1].TeamCompeting.TeamName;
                scoreButton.Enabled = true;

                if (matchup.Entries[0].Score != null)
                {
                    teamOneScoreValue.Text = matchup.Entries[0].Score.ToString();
                    teamTwoScoreValue.Text = matchup.Entries[1].Score.ToString();
                    scoreButton.Enabled = false;
                }
            }
            else // 1 matchup only
            {
                teamOneNameLabel.Text = matchup.Entries[0].TeamCompeting.TeamName;
                teamTwoNameLabel.Text = "Empty";
                scoreButton.Enabled = true;

                if (matchup.Entries[0].Score != null)
                {
                    teamOneScoreValue.Text = matchup.Entries[0].Score.ToString();
                    scoreButton.Enabled = false;
                }
            }
        }


        private void TournamentViewerForm_Load(object sender, EventArgs e)
        {

        }

        private void roundDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            matchupsInCurrentRound = flattenMatchups.Where(matchup => matchup.MatchupRound == GetCurrentRound()).ToList();
            RefreshMatchupLists(matchupsInCurrentRound);
            if (matchupsInCurrentRound.Count > 0)
            {
                currentMatchup = matchupsInCurrentRound[0];
                UpdateMatchupInfo(currentMatchup);
            }
        }

        private void matchupListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentmatchup = matchupListbox.SelectedItem.ToString();
            if (int.TryParse(currentmatchup.Substring(currentmatchup.IndexOf('-') + 1), out int selectedMatchup))
            {
                currentMatchup = matchupsInCurrentRound[selectedMatchup - 1];
                UpdateMatchupInfo(currentMatchup);
            }
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            bool isUnplayedOnly = unplayedOnlyCheckbox.Checked;
            if (isUnplayedOnly)
            {
                List<MatchupModel> unplayedMatchups = matchupsInCurrentRound.Where(matchup => matchup.Winner == null).ToList();
                RefreshMatchupLists(unplayedMatchups);
            }
            else
            {
                RefreshMatchupLists(matchupsInCurrentRound);
            }
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            if (currentMatchup.Entries.Count==1)
            {
                currentMatchup.Winner = currentMatchup.Entries[0].TeamCompeting;
                currentMatchup.Entries[0].Score = 0;

                MatchupEntryModel nextMatchupEntry = flattenMatchups.SelectMany(matchup => matchup.Entries).FirstOrDefault(entry => entry.ParentMatchup?.Id== currentMatchup.Id);
                nextMatchupEntry.TeamCompeting = currentMatchup.Winner;

                GlobalConfig.Connection.UpdateMatchResult(currentMatchup);
                UpdateMatchupInfo(currentMatchup);

                return;
            }


            bool teamOneScoreValid = int.TryParse(teamOneScoreValue.Text, out int teamOneScore) && teamOneScore >= 0;
            bool teamTwoScoreValid = int.TryParse(teamTwoScoreValue.Text, out int teamTwoScore) && teamTwoScore >= 0;

            if (!teamOneScoreValid || !teamTwoScoreValid || teamOneScore == teamTwoScore)
            {
                MessageBox.Show("You need to fill in all of the score fields correctly.");
            }
            else
            {
                MatchupEntryModel entryOne = currentMatchup.Entries.First(entry => entry.TeamCompeting.TeamName == teamOneNameLabel.Text);
                MatchupEntryModel entryTwo = currentMatchup.Entries.First(entry => entry.TeamCompeting.TeamName == teamTwoNameLabel.Text);

                entryOne.Score = teamOneScore;
                entryTwo.Score = teamTwoScore;

                currentMatchup.Winner = teamOneScore > teamTwoScore ? entryOne.TeamCompeting : entryTwo.TeamCompeting;

                MatchupEntryModel nextMatchupEntry = flattenMatchups.SelectMany(matchup => matchup.Entries).FirstOrDefault(entry => entry.ParentMatchup?.Id == currentMatchup.Id);
                if (nextMatchupEntry != null)
                {
                    nextMatchupEntry.TeamCompeting = currentMatchup.Winner;
                }

                GlobalConfig.Connection.UpdateMatchResult(currentMatchup);
            }

            UpdateMatchupInfo(currentMatchup);
        }
    }
}
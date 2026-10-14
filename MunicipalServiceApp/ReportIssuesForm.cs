using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MunicipalServiceApp
{
    public partial class ReportIssuesForm : Form
    {
        //private List<Issue> reportedIssues = new List<Issue>();
        private string attachmentPath = "";
        public ReportIssuesForm()
        {
            InitializeComponent();

            cmbCategory.Items.Add("Roads");
            cmbCategory.Items.Add("Water");
            cmbCategory.Items.Add("Electricity");
            cmbCategory.Items.Add("Sanitation");
            cmbCategory.Items.Add("Waste Management");
            cmbCategory.Items.Add("Street Lighting");
            cmbCategory.Items.Add("Other");

            cmbCategory.SelectedIndex = -1;
        }

        private void ReportIssuesForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show(
                    "Please enter the location of the issue.",
                    "Location Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtLocation.Focus();
                return;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select an issue category.",
                    "Category Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                MessageBox.Show(
                    "Please provide a description of the issue.",
                    "Description Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                rtbDescription.Focus();
                return;
            }

            if (rtbDescription.Text.Trim().Length < 10)
            {
                MessageBox.Show(
                    "Please provide a more detailed description of the issue.",
                    "Description Too Short",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                rtbDescription.Focus();
                return;
            }

            Issue newIssue = new Issue
            {
                Location = txtLocation.Text.Trim(),
                Category = cmbCategory.SelectedItem.ToString(),
                Description = rtbDescription.Text.Trim(),
                AttachmentPath = lblAttachment.Text,
                DateReported = DateTime.Now
            };

            //reportedIssues.Add(newIssue);
            IssueRepository.Issues.Add(newIssue);


            MessageBox.Show("Issue submitted successfully!\n\n" +
                            "Location: " + newIssue.Location + "\n" +
                            "Category: " + newIssue.Category + "\n\n" +
                            "Thank you for helping improve our community!",
                            "Issue Reported",
             MessageBoxButtons.OK,
             MessageBoxIcon.Information);

            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Clear();

            lblAttachment.Text = "No file selected";

            progressBarReport.Value = 0;

            lblEngagement.Text = "Let's help improve our community!";
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an Image or Document";

                openFileDialog.Filter =
                    "Image Files|*.jpg;*.jpeg;*.png;*.gif|" +
                    "Document Files|*.pdf;*.doc;*.docx;*.txt|" +
                    "All Files|*.*";

                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    attachmentPath = openFileDialog.FileName;

                    lblAttachment.Text = "Selected: " + Path.GetFileName(attachmentPath);

                    UpdateProgress();

                }
            }
        }

        private void UpdateProgress()
        {
            int progress = 0;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                progress += 25;
            }

            if (cmbCategory.SelectedIndex != -1)
            {
                progress += 25;
            }

            if (!string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                progress += 25;
            }

            if (!string.IsNullOrWhiteSpace(attachmentPath))
            {
                progress += 25;
            }

            progressBarReport.Value = progress;

            if (progress == 0)
            {
                lblEngagement.Text = "Let's help improve our community!";
            }
            else if (progress == 25)
            {
                lblEngagement.Text = "Great start! Please provide the issue category.";
            }
            else if (progress == 50)
            {
                lblEngagement.Text = "You're halfway there! Add a description.";
            }
            else if (progress == 75)
            {
                lblEngagement.Text = "Almost done! You can attach supporting evidence.";
            }
            else
            {
                lblEngagement.Text = "Excellent! Your report is ready to submit.";
            }
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateProgress();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProgress();
        }

        private void rtbDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateProgress();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}

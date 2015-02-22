using System;
using System.Windows.Forms;

namespace EntitiesSelectionWizard
{
    public partial class EntitiesSelectionDialog : Form
    {
        private PageSelection _pageIndex = PageSelection.FirstPage;

        public EntitiesSelectionDialog()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            entitiesListView1.Items[0].Selected = true;
            entitiesListView2.Items[0].Selected = true;
        }

        public EntitiesSelection EntitiesSelection { get; set; }

        private void SetPageState()
        {
            if (_pageIndex == PageSelection.FirstPage)
            {
                entitiesListView1.Visible = true;
                entitiesListView2.Visible = false;

                previousButton.Enabled = false;
                EntitiesTypeSelection selection = (EntitiesTypeSelection) GetListViewSelection(entitiesListView1);
                if (selection == EntitiesTypeSelection.ServiceEntities)
                {
                    nextButton.Enabled = false;
                    finishButton.Enabled = true;
                    AcceptButton = finishButton;
                }
                else
                {
                    nextButton.Enabled = true;
                    finishButton.Enabled = false;
                    AcceptButton = nextButton;
                }
            }
            else
            {
                entitiesListView1.Visible = false;
                entitiesListView2.Visible = true;

                previousButton.Enabled = true;
                nextButton.Enabled = false;
                finishButton.Enabled = true;
                AcceptButton = finishButton;
            }
        }

        private int GetListViewSelection(ListView listView)
        {
            int selection = 0;
            if (listView.SelectedIndices.Count > 0)
                selection = listView.SelectedIndices[0];
            return selection;
        }

        private string GetSelectedEntityType()
        {
            EntitiesTypeSelection selection = (EntitiesTypeSelection)GetListViewSelection(entitiesListView1);
            switch (selection)
            {
                case EntitiesTypeSelection.ClientEntities:
                    return "Client";
                case EntitiesTypeSelection.ServiceEntities:
                    return "Service";
                case EntitiesTypeSelection.SharedEntities:
                    return "Shared";
                default:
                    return string.Empty;
            }
        }

        private void entitiesListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EntitiesTypeSelection selection = (EntitiesTypeSelection)GetListViewSelection(entitiesListView1);
            switch (selection)
            {
                case EntitiesTypeSelection.ClientEntities:
                    descriptionLabel.Text = Constants.Descriptions.ClientEntities;
                    break;
                case EntitiesTypeSelection.ServiceEntities:
                    descriptionLabel.Text = Constants.Descriptions.ServiceEntities;
                    break;
                case EntitiesTypeSelection.SharedEntities:
                    descriptionLabel.Text = Constants.Descriptions.SharedEntities;
                    break;
            }
            SetPageState();
        }

        private void entitiesListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string entityType = GetSelectedEntityType();
            EntitiesCategorySelection selection = (EntitiesCategorySelection)GetListViewSelection(entitiesListView2);
            switch (selection)
            {
                case EntitiesCategorySelection.PortableEntities:
                    descriptionLabel.Text = string.Format(Constants.Descriptions.PortableEntities, entityType);
                    break;
                case EntitiesCategorySelection.DotNetEntities:
                    descriptionLabel.Text = string.Format(Constants.Descriptions.DotNetEntities, entityType);
                    break;
            }
            SetPageState();
        }

        private EntitiesSelection ToSelectionType(EntitiesTypeSelection typeSelection)
        {
            switch (typeSelection)
            {
                case EntitiesTypeSelection.ServiceEntities:
                    return EntitiesSelection.Service;
                case EntitiesTypeSelection.ClientEntities:
                    return EntitiesSelection.Client;
                case EntitiesTypeSelection.SharedEntities:
                    return EntitiesSelection.Shared;
                default:
                    return EntitiesSelection.None;
            }
        }

        private EntitiesSelection ToSelectionType(EntitiesCategorySelection categorySelection)
        {
            switch (categorySelection)
            {
                case EntitiesCategorySelection.PortableEntities:
                    return EntitiesSelection.Portable;
                case EntitiesCategorySelection.DotNetEntities:
                    return EntitiesSelection.DotNet45;
                default:
                    return EntitiesSelection.None;
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            _pageIndex = PageSelection.FirstPage;
            SetPageState();
            entitiesListView1.Focus();
            entitiesListView1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            _pageIndex = PageSelection.SecondPage;
            SetPageState();
            entitiesListView2.Focus();
            entitiesListView2_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            EntitiesSelection = EntitiesSelection.None;
            DialogResult = DialogResult.Cancel;
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            EntitiesTypeSelection typeSelection = (EntitiesTypeSelection)GetListViewSelection(entitiesListView1);
            EntitiesCategorySelection categorySelection = EntitiesCategorySelection.DotNetEntities;
            if (typeSelection != EntitiesTypeSelection.ServiceEntities)
                categorySelection = (EntitiesCategorySelection)GetListViewSelection(entitiesListView2);

            EntitiesSelection entitiesType = ToSelectionType(typeSelection);
            EntitiesSelection entitiesCategory = ToSelectionType(categorySelection);
            EntitiesSelection = entitiesType | entitiesCategory;
            DialogResult = DialogResult.OK;
        }

        private enum PageSelection
        {
            FirstPage = 0,
            SecondPage = 1
        }

        private enum EntitiesTypeSelection
        {
            ServiceEntities = 0,
            ClientEntities = 1,
            SharedEntities = 2
        }

        private enum EntitiesCategorySelection
        {
            PortableEntities = 0,
            DotNetEntities = 1
        }
    }
}

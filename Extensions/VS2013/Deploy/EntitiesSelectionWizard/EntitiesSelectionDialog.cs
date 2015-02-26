using System;
using System.Windows.Forms;

namespace EntitiesSelectionWizard
{
    public partial class EntitiesSelectionDialog : Form
    {
        private readonly bool _multiproject;
        private readonly bool _webApiSharedPortable;
        private readonly ListViewItem _portableListViewItemItem;
        private PageSelection _pageIndex = PageSelection.FirstPage;

        public EntitiesSelectionDialog(bool multiproject = false, 
            bool webApiSharedPortable = false)
        {
            Application.EnableVisualStyles();
            InitializeComponent();

            _multiproject = multiproject;
            _webApiSharedPortable = webApiSharedPortable;
            _portableListViewItemItem = portableDotNetListView.Items[1];

            if (_multiproject)
            {
                clientServiceListView.Visible = true;
                serviceClientSharedListView.Visible = false;
            }
            else
            {
                clientServiceListView.Visible = false;
                serviceClientSharedListView.Visible = true;
            }

            clientServiceListView.Items[0].Selected = true;
            serviceClientSharedListView.Items[0].Selected = true;
            portableDotNetListView.Items[0].Selected = true;
        }

        public EntitiesSelection EntitiesSelection { get; set; }

        private void SetPageState()
        {
            if (_pageIndex == PageSelection.FirstPage)
            {
                if (_multiproject)
                    clientServiceListView.Visible = true;
                else
                    serviceClientSharedListView.Visible = true;
                portableDotNetListView.Visible = false;

                previousButton.Enabled = false;
                if (_multiproject)
                {
                    nextButton.Enabled = true;
                    finishButton.Enabled = false;
                    AcceptButton = nextButton;
                }
                else
                {
                    ListView entitiesTypeListView;
                    if (_multiproject) entitiesTypeListView = clientServiceListView;
                    else entitiesTypeListView = serviceClientSharedListView;
                    EntitiesTypeSelection selection = GetEntitiesTypeSelection(entitiesTypeListView);
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
            }
            else
            {
                clientServiceListView.Visible = false;
                serviceClientSharedListView.Visible = false;
                portableDotNetListView.Visible = true;

                previousButton.Enabled = true;
                nextButton.Enabled = false;
                finishButton.Enabled = true;
                AcceptButton = finishButton;
            }
        }

        private EntitiesTypeSelection GetEntitiesTypeSelection(ListView entitiesTypeListView)
        {
            int selection = 0;
            if (entitiesTypeListView.SelectedIndices.Count > 0)
                selection = entitiesTypeListView.SelectedIndices[0];
            if (ReferenceEquals(entitiesTypeListView, clientServiceListView))
            {
                switch (selection)
                {
                    case 0:
                        return EntitiesTypeSelection.ClientServiceEntities;
                    default:
                        return EntitiesTypeSelection.SharedEntities;
                }
            }
            if (ReferenceEquals(entitiesTypeListView, serviceClientSharedListView))
            {
                switch (selection)
                {
                    case 0:
                        return EntitiesTypeSelection.ServiceEntities;
                    case 1:
                        return EntitiesTypeSelection.ClientEntities;
                    default:
                        return EntitiesTypeSelection.SharedEntities;
                }
            }
            throw new InvalidOperationException("Unsupported entities type selection.");
        }

        private EntitiesCategorySelection GetEntitiesCategorySelection(ListView entitiesCategoryListView)
        {
            int selection = 0;
            if (entitiesCategoryListView.SelectedIndices.Count > 0)
                selection = entitiesCategoryListView.SelectedIndices[0];
            if (ReferenceEquals(entitiesCategoryListView, portableDotNetListView))
            {
                switch (selection)
                {
                    case 0:
                        return EntitiesCategorySelection.DotNetEntities;
                    default:
                        return EntitiesCategorySelection.PortableEntities;
                }
            }
            throw new InvalidOperationException("Unsupported entities category selection.");
        }

        private string GetSelectedEntityType()
        {
            EntitiesTypeSelection selection;
            if (_multiproject)
            {
                selection = GetEntitiesTypeSelection(clientServiceListView);
                switch (selection)
                {
                    case EntitiesTypeSelection.ClientServiceEntities:
                        return "Separate Client / Service";
                    case EntitiesTypeSelection.SharedEntities:
                        return "Shared";
                }
            }
            else
            {
                selection = GetEntitiesTypeSelection(serviceClientSharedListView);
                switch (selection)
                {
                    case EntitiesTypeSelection.ClientEntities:
                        return "Client";
                    case EntitiesTypeSelection.ServiceEntities:
                        return "Service";
                    case EntitiesTypeSelection.SharedEntities:
                        return "Shared";
                }
            }
            return string.Empty;
        }

        private void clientServiceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            EntitiesTypeSelection selection = GetEntitiesTypeSelection(clientServiceListView);
            switch (selection)
            {
                case EntitiesTypeSelection.ClientServiceEntities:
                    descriptionLabel.Text = Constants.Descriptions.ClientServiceEntities;
                    break;
                case EntitiesTypeSelection.SharedEntities:
                    descriptionLabel.Text = Constants.Descriptions.SharedEntities;
                    break;
            }
            SetPageState();
        }

        private void serviceClientSharedListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            EntitiesTypeSelection selection = GetEntitiesTypeSelection(serviceClientSharedListView);
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

        private void portableDotNetListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            string entityType = GetSelectedEntityType();
            EntitiesCategorySelection selection = GetEntitiesCategorySelection(portableDotNetListView);
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
                case EntitiesTypeSelection.ClientServiceEntities:
                    return EntitiesSelection.ClientService;
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
            if (_multiproject)
            {
                clientServiceListView.Focus();
                clientServiceListView_SelectedIndexChanged(this, EventArgs.Empty);
            }
            else
            {
                serviceClientSharedListView.Focus();
                serviceClientSharedListView_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            _pageIndex = PageSelection.SecondPage;
            if (_webApiSharedPortable)
            {
                EntitiesTypeSelection selection = GetEntitiesTypeSelection(clientServiceListView);
                switch (selection)
                {
                    case EntitiesTypeSelection.ClientServiceEntities:
                        if (!portableDotNetListView.Items.Contains(_portableListViewItemItem))
                        {
                            portableDotNetListView.Items.Insert(1, _portableListViewItemItem);
                            portableDotNetListView.Items[1].EnsureVisible();
                            portableDotNetListView.Items[0].Focused = true;
                            portableDotNetListView.Items[0].Selected = true;
                        }
                        break;
                    case EntitiesTypeSelection.SharedEntities:
                        if (portableDotNetListView.Items.Contains(_portableListViewItemItem))
                        {
                            portableDotNetListView.Items.Remove(_portableListViewItemItem);
                            portableDotNetListView.Items[0].Focused = true;
                            portableDotNetListView.Items[0].Selected = true;
                        }
                        break;
                }
            }
            SetPageState();
            portableDotNetListView.Focus();
            portableDotNetListView_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            EntitiesSelection = EntitiesSelection.None;
            DialogResult = DialogResult.Cancel;
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            EntitiesTypeSelection typeSelection;
            if (_multiproject)
                typeSelection = GetEntitiesTypeSelection(clientServiceListView);
            else
                typeSelection = GetEntitiesTypeSelection(serviceClientSharedListView);
            EntitiesCategorySelection categorySelection = EntitiesCategorySelection.DotNetEntities;

            if (typeSelection != EntitiesTypeSelection.ServiceEntities)
                categorySelection = GetEntitiesCategorySelection(portableDotNetListView);

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
            ClientServiceEntities = 2,
            SharedEntities = 3
        }

        private enum EntitiesCategorySelection
        {
            PortableEntities = 0,
            DotNetEntities = 1
        }
    }
}

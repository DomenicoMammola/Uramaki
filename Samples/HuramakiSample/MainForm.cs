using Mammola.Uramaki.Base;
using Mammola.Uramaki.MasterDetail;
using Mammola.Uramaki.UI;
using System.Windows.Forms;


namespace HuramakiSample
{

  public partial class MainForm : Form
  {
    UramakiFramework uramakiFW;
    UramakiDesktopManager uramakiDesktopManager;

    public MainForm()
    {
      InitializeComponent();
      uramakiFW = new UramakiFramework();
      uramakiDesktopManager = new UramakiDesktopManager();

      MasterQueryUramakiTransformer MasterQueryTransformer = new MasterQueryUramakiTransformer(); 
      uramakiFW.AddTransformer(MasterQueryTransformer);
      MasterQueryUramakiDesktopPublisherGrid QueryGridPublisher = new MasterQueryUramakiDesktopPublisherGrid(ref uramakiDesktopManager);
      uramakiFW.AddPublisher(QueryGridPublisher);


      uramakiDesktopManager.Init(ref uramakiFW, this);

    }
  }
}

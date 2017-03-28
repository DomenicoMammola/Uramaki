using Mammola.Uramaki.Base;
using Mammola.Uramaki.MasterDetail;
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
      MasterQueryUramakiPublisherGrid QueryGridPublisher = new MasterQueryUramakiPublisherGrid();
      uramakiFW.AddPublisher(QueryGridPublisher);


      uramakiDesktopManager.Init(ref uramakiFW, this);

    }
  }
}

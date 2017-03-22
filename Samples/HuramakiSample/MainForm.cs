using Mammola.Huramaki.WinformsUI.Base;
using Mammola.Huramaki.WinformsUI.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HuramakiSample
{

  public partial class MainForm : Form
  {
    HuramakiFramework huramakiFW;
    HuramakiDesktopManager huramakiDesktopManager;

    public MainForm()
    {
      InitializeComponent();
      huramakiFW = new HuramakiFramework();
      huramakiDesktopManager = new HuramakiDesktopManager();

      huramakiDesktopManager.Init(ref huramakiFW, this);

    }
  }
}

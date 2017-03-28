using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mammola.Uramaki.Base;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;

namespace Mammola.Uramaki.Base
{
public class UramakiDesktopManager
  {
    private class TMenuItemInfo
    {
      public UramakiPublisher Publisher;
      public UramakiTransformer Transformer;
    }


    private UramakiFramework Framework;
    private Form ParentForm;
    private DockPanel DockPanel;
    private ToolStrip MainToolbar;

    int LastActivePanelHashCode;
    private ToolStripButton LoadMenuButton;
    private ToolStripButton SaveMenuButton;
    private ToolStripDropDownButton  AddNewButton;
    private ToolStripButton AddChildButton;
    private ContextMenuStrip TransformersPopupMenu;


    private void CreateStandardToolbar()
    {
      LoadMenuButton = new ToolStripButton("Load");
      MainToolbar.Items.Add(LoadMenuButton);
      SaveMenuButton = new ToolStripButton("Save");
      MainToolbar.Items.Add(SaveMenuButton);
    }

    private void CreateCustomizeToolbar()    
    {
      AddNewButton = new ToolStripDropDownButton("New");
      MainToolbar.Items.Add(AddNewButton);      
      TransformersPopupMenu = new ContextMenuStrip();
      TransformersPopupMenu.Opening += FTransformersPopupMenu_Popup;
      AddNewButton.DropDown = TransformersPopupMenu;
      List<UramakiTransformer> availableTransformers = this.Framework.GetAvailableTransformers(UramakiRoll.NullUramakiId);
      foreach (UramakiTransformer TempTransformer in availableTransformers)
      {       
        ToolStripMenuItem itm = new ToolStripMenuItem(TempTransformer.GetDescription());
        itm.Tag = TempTransformer.GetMyId();         

        TransformersPopupMenu.Items.Add(itm);
        
        List<UramakiPublisher>AvailablePublishers = this.Framework.GetAvailablePublishers(TempTransformer.GetOutputUramakiId());

        foreach (UramakiPublisher TempPublisher in AvailablePublishers)
        {                            
          TMenuItemInfo tempMenuInfo = new TMenuItemInfo();
          tempMenuInfo.Publisher = TempPublisher;
          tempMenuInfo.Transformer = TempTransformer;

          ToolStripMenuItem itm2 = new ToolStripMenuItem(TempPublisher.GetDescription());
          itm2.Tag = tempMenuInfo;                   
          itm2.Click += FBarManager_ItemClick;

          itm.DropDownItems.Add(itm2);
        }
      }
        
    }

    private void FTransformersPopupMenu_Popup(object sender, EventArgs e)
    {
      //throw new NotImplementedException();
    }

    
    void FBarManager_ItemClick(object sender, EventArgs e)
    {
      ToolStripMenuItem currentButton =  sender as ToolStripMenuItem;

      if (currentButton == null) return;


      if ((currentButton.Tag != null) && (currentButton.Tag is TMenuItemInfo))
      {
        UramakiPublisher tempPublisher = (currentButton.Tag as TMenuItemInfo).Publisher;
        UramakiTransformer tempTransformer = (currentButton.Tag as TMenuItemInfo).Transformer;
        
        List<UramakiActualTransformation> actualTransformations = new List<UramakiActualTransformation>();
        UramakiActualTransformation tmpTransformation = new UramakiActualTransformation();
        tmpTransformation.Transformer =  tempTransformer;
        tmpTransformation.TransformationContext = tempTransformer.CreateTransformerContext();
        actualTransformations.Add(tmpTransformation);

        UramakiActualPublication tempPublication = new UramakiActualPublication();
        tempPublication.Publisher = tempPublisher;

        UramakiPlate actualPlate = Framework.BuildPlate(UramakiFramework.NullId, ref actualTransformations, ref tempPublication);
        actualPlate.DockPanel.Text = "Report";
        actualPlate.DockPanel.Show(DockPanel, DockState.Float);
      }
      //this.FDockPanel.
      //DockPanel TempPanel = FDockManager.AddPanel(DockingStyle.Float);

    }

    public void Init(ref UramakiFramework framework, Form parentForm)
    {
      this.Framework = framework;
      this.ParentForm = parentForm;
      this.ParentForm.IsMdiContainer = true;            

      this.DockPanel = new DockPanel();
      this.DockPanel.Dock = DockStyle.Fill;
      ParentForm.Controls.Add(DockPanel);
      this.DockPanel.ActiveContentChanged += FDockPanel_ActiveContentChanged;
      
      MainToolbar = new ToolStrip();
      MainToolbar.Parent = ParentForm;
      MainToolbar.Dock = DockStyle.Top;      
      CreateStandardToolbar();
      CreateCustomizeToolbar();   
    }

    private void FDockPanel_ActiveContentChanged(object sender, EventArgs e)
    {
      if ((sender as DockPanel).ActiveContent == null)
      {
        return;
      }
      int TempNewHashCode = ((sender as DockPanel).ActiveContent as DockContent).GetHashCode();
      if (LastActivePanelHashCode != TempNewHashCode)
      {
        LastActivePanelHashCode = TempNewHashCode;
        ((sender as DockPanel).ActiveContent as DockContent).Text = TempNewHashCode.ToString();
        //MessageBox.Show(TempNewHashCode.ToString());      
      }
    }
  
  }
  }

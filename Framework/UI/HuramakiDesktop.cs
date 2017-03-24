using System;
using System.Windows.Forms;

using Mammola.Huramaki.WinformsUI.Base;
using WeifenLuo.WinFormsUI.Docking;


namespace Mammola.Huramaki.UI
{
  public class HuramakiDesktopPlate : DockContent
  {

    private HuramakiFramework framework;

    public HuramakiDesktopPlate() : base()
    {
      /*
      FTempListBoxControl = new ListBoxControl();
      this.Controls.Add(FTempListBoxControl);
      FTempListBoxControl.Dock = DockStyle.Fill;
      FTempListBoxControl.BeginUpdate();
      FTempListBoxControl.Items.Add("ciccio");
      FTempListBoxControl.Items.Add("pippo");
      FTempListBoxControl.Items.Add("pluto");
      FTempListBoxControl.EndUpdate();*/
    }

  }


  public class HuramakiDesktopManager
  {
    private class TMenuItemInfo
    {
      public HuramakiPublisher Publisher;
      public HuramakiTransformer Transformer;
    }


    private HuramakiFramework framework;
    private Form parentForm;
    private DockPanel dockPanel;
    private ToolStrip mainToolbar;

    int FLastActivePanelHashCode;
    private ToolStripButton loadMenuButton;
    private ToolStripButton saveMenuButton;
    private ToolStripButton FAddNewButton;
    private ToolStripButton FAddChildButton;
    //private PopupMenu FTransformersPopupMenu;


    private void CreateStandardToolbar()
    {
      loadMenuButton = new ToolStripButton("Load");
      mainToolbar.Items.Add(loadMenuButton);
      saveMenuButton = new ToolStripButton("Save");
      mainToolbar.Items.Add(saveMenuButton);
    }

    private void CreateCustomizeToolbar()
    {/*
      FBarManager.BeginUpdate();
      try
      {
        FCustomToolbar = new Bar(FBarManager, "Customize Toolbar");      
        FCustomToolbar.DockStyle = BarDockStyle.Top;        

        FTransformersPopupMenu = new PopupMenu(FBarManager);
        FTransformersPopupMenu.Popup += FTransformersPopupMenu_Popup;
        List<HuramakiTransformer> AvailableTransformers = this.Framework.GetAvailableTransformers(Huramaki.NullOboId);
        foreach (HuramakiTransformer TempTransformer in AvailableTransformers)
        {
          //BarButtonItem 
          BarButtonItem TempMenuItem = new BarButtonItem(FBarManager, TempTransformer.GetDescription());        
          TempMenuItem.Tag = TempTransformer.GetMyId(); 
          TempMenuItem.ButtonStyle = BarButtonStyle.DropDown;
          TempMenuItem.ActAsDropDown = true;  
          TempMenuItem.DropDownEnabled = true;
          TempMenuItem.AllowDrawArrow = true;        
          BarItemLink TempBarItemLink = FTransformersPopupMenu.AddItem(TempMenuItem);
          List<HuramakiPublisher>AvailablePublishers = this.Framework.GetAvailablePublishers(TempTransformer.GetOutpuHuramakiId());
          PopupMenu TempPopupMenu = new PopupMenu(FBarManager);
          TempMenuItem.DropDownControl = TempPopupMenu;
          foreach (HuramakiPublisher TempPublisher in AvailablePublishers)
          {
            BarButtonItem TempMenuItemPub = new BarButtonItem(FBarManager, TempPublisher.GetDescription());
            TMenuItemInfo TempMenuInfo = new TMenuItemInfo();
            TempMenuInfo.Publisher = TempPublisher;
            TempMenuInfo.Transformer = TempTransformer;
            TempMenuItemPub.Tag = TempMenuInfo;
            BarItemLink TempBarItemLinkPub =  TempPopupMenu.AddItem(TempMenuItemPub);
          }
        }

        FAddNewButton = new BarButtonItem(FBarManager, "Add new.."); 
        FAddNewButton.DropDownControl = FTransformersPopupMenu;       
        FAddNewButton.ButtonStyle = BarButtonStyle.DropDown;
        FAddNewButton.ActAsDropDown = true;  
        FAddNewButton.DropDownEnabled = true;
        FAddNewButton.AllowDrawArrow = true;
        
        
        FCustomToolbar.AddItem(FAddNewButton);
        FAddChildButton = new BarButtonItem(FBarManager, "Add new child..");        
        FCustomToolbar.AddItem(FAddChildButton);               
      }
      finally
      {
        FBarManager.EndUpdate();
      }
      FCustomToolbar.DockRow = 0;*/
    }

    private void FTransformersPopupMenu_Popup(object sender, EventArgs e)
    {
      //throw new NotImplementedException();
    }

    /*
    void FBarManager_ItemClick(object sender, ItemClickEventArgs e)
    {
      BarButtonItem currentButton = e.Item as BarButtonItem;
      if (currentButton == null) return;


      if ((currentButton.Tag != null) && (currentButton.Tag is TMenuItemInfo))
      {
        HuramakiPublisher TempPublisher = (currentButton.Tag as TMenuItemInfo).Publisher;
        HuramakiTransformer TempTransformer = (currentButton.Tag as TMenuItemInfo).Transformer;


        //Framework.BuildPlate(HuramakiFramework.NullId, nil, nil);
        HuramakiDesktopPlate TempPanel = new HuramakiDesktopPlate();
        TempPanel.Text = "Report";
        TempPanel.Show(FDockPanel, DockState.Float);
      }
      //this.FDockPanel.
      //DockPanel TempPanel = FDockManager.AddPanel(DockingStyle.Float);

    }*/

    public void Init(ref HuramakiFramework aFramework, Form aParentForm)
    {
      this.framework = aFramework;
      this.parentForm = aParentForm;
      this.parentForm.IsMdiContainer = true;            
      mainToolbar = new ToolStrip();
      mainToolbar.Parent = parentForm;
      mainToolbar.Dock = DockStyle.Top;
      CreateStandardToolbar();
      CreateCustomizeToolbar();
      //FBarManager.ItemClick += new ItemClickEventHandler(FBarManager_ItemClick);

      this.dockPanel = new DockPanel();
      this.dockPanel.Dock = DockStyle.Fill;
      parentForm.Controls.Add(dockPanel);
      this.dockPanel.ActiveContentChanged += FDockPanel_ActiveContentChanged;
      
      //FDockManager = new DockManager();
      //FDockManager.Form = FParentForm;
      //FDockManager.ActivePanelChanged += FDockManager_ActivePanelChanged;    
    }

    private void FDockPanel_ActiveContentChanged(object sender, EventArgs e)
    {
      if ((sender as DockPanel).ActiveContent == null)
      {
        return;
      }
      int TempNewHashCode = ((sender as DockPanel).ActiveContent as DockContent).GetHashCode();
      if (FLastActivePanelHashCode != TempNewHashCode)
      {
        FLastActivePanelHashCode = TempNewHashCode;
        ((sender as DockPanel).ActiveContent as DockContent).Text = TempNewHashCode.ToString();
        //MessageBox.Show(TempNewHashCode.ToString());      
      }
    }

    public HuramakiDesktopPlate CreateDesktopPlate()
    {
      HuramakiDesktopPlate TempPanel = new HuramakiDesktopPlate();
      TempPanel.Text = "Report";
      TempPanel.Show(dockPanel, DockState.Float);
      return TempPanel;
    }

    //private void FDockManager_ActivePanelChanged(object sender, ActivePanelChangedEventArgs e)
    //{
    //throw new NotImplementedException();
    //}
  }
}

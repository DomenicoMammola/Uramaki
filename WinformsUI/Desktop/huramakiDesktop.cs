using System;
using System.Windows.Forms;

using Mammola.Huramaki.WinformsUI.Base;
using WeifenLuo.WinFormsUI.Docking;


namespace Mammola.Huramaki.WinformsUI.Desktop
{
  public class HuramakiDesktopPlate : DockContent
  {

    public HuramakiFramework Framework;

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


    private HuramakiFramework Framework;
    private Form FParentForm;
    private DockPanel FDockPanel;
    private ToolStrip FMainToolbar;

    int FLastActivePanelHashCode;
    private ToolStripButton FLoadMenuButton;
    private ToolStripButton FSaveMenuButton;
    private ToolStripButton FAddNewButton;
    private ToolStripButton FAddChildButton;
    //private PopupMenu FTransformersPopupMenu;


    private void CreateStandardToolbar()
    {
      FLoadMenuButton = new ToolStripButton("Load");
      FMainToolbar.Items.Add(FLoadMenuButton);
      FSaveMenuButton = new ToolStripButton("Save");
      FMainToolbar.Items.Add(FSaveMenuButton);
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

    public void Init(ref HuramakiFramework aFramework, ref Form aParentForm)
    {
      this.Framework = aFramework;
      this.FParentForm = aParentForm;
      this.FParentForm.IsMdiContainer = true;
      CreateStandardToolbar();
      CreateCustomizeToolbar();
      //FBarManager.ItemClick += new ItemClickEventHandler(FBarManager_ItemClick);

      this.FDockPanel = new DockPanel();
      this.FDockPanel.Dock = DockStyle.Fill;
      FParentForm.Controls.Add(FDockPanel);
      this.FDockPanel.ActiveContentChanged += FDockPanel_ActiveContentChanged;

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
      TempPanel.Show(FDockPanel, DockState.Float);
      return TempPanel;
    }

    //private void FDockManager_ActivePanelChanged(object sender, ActivePanelChangedEventArgs e)
    //{
    //throw new NotImplementedException();
    //}
  }
}

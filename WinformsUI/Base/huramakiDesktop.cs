using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using DevExpress.XtraBars.Docking;

using DevExpress.XtraBars;
using DevExpress.XtraEditors;

using RCL.obo.Base;
using RCL.obo.Framework;
using WeifenLuo.WinFormsUI.Docking;



namespace RCL.obo.Desktop
{  
  public class TOboDesktopPlate : DockContent
  {

    public TOboFramework Framework;

    public TOboDesktopPlate():base()
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


  public class TOboDesktopManager
  {
    private class TMenuItemInfo 
    {
      public TOboPublisher Publisher;
      public TOboTransformer Transformer;
    }


    private TOboFramework Framework;
    private Form FParentForm;
    private DockPanel FDockPanel;
    private BarManager FBarManager;
    private Bar FMainToolbar; 
    private Bar FCustomToolbar;   
    int FLastActivePanelHashCode;
    private BarButtonItem FLoadMenuButton;
    private BarButtonItem FSaveMenuButton;
    private BarButtonItem FAddNewButton;
    private BarButtonItem FAddChildButton;
    private PopupMenu FTransformersPopupMenu;

    
    private void CreateStandardToolbar()
    {
      FBarManager.BeginUpdate();
      try
      {
        FMainToolbar = new Bar(FBarManager, "Main Toolbar");      
        FMainToolbar.DockStyle = BarDockStyle.Top;
        FLoadMenuButton = new BarButtonItem(FBarManager, "Load..");        
        FMainToolbar.AddItem(FLoadMenuButton);
        FSaveMenuButton = new BarButtonItem(FBarManager, "Save..");        
        FMainToolbar.AddItem(FSaveMenuButton);
      }
      finally
      {
        FBarManager.EndUpdate();
      }
    }

    private void CreateCustomizeToolbar()
    {
      FBarManager.BeginUpdate();
      try
      {
        FCustomToolbar = new Bar(FBarManager, "Customize Toolbar");      
        FCustomToolbar.DockStyle = BarDockStyle.Top;        

        FTransformersPopupMenu = new PopupMenu(FBarManager);
        FTransformersPopupMenu.Popup += FTransformersPopupMenu_Popup;
        List<TOboTransformer> AvailableTransformers = this.Framework.GetAvailableTransformers(TObo.NullOboId);
        foreach (TOboTransformer TempTransformer in AvailableTransformers)
        {
          //BarButtonItem 
          BarButtonItem TempMenuItem = new BarButtonItem(FBarManager, TempTransformer.GetDescription());        
          TempMenuItem.Tag = TempTransformer.GetMyId(); 
          TempMenuItem.ButtonStyle = BarButtonStyle.DropDown;
          TempMenuItem.ActAsDropDown = true;  
          TempMenuItem.DropDownEnabled = true;
          TempMenuItem.AllowDrawArrow = true;        
          BarItemLink TempBarItemLink = FTransformersPopupMenu.AddItem(TempMenuItem);
          List<TOboPublisher>AvailablePublishers = this.Framework.GetAvailablePublishers(TempTransformer.GetOutputOboId());
          PopupMenu TempPopupMenu = new PopupMenu(FBarManager);
          TempMenuItem.DropDownControl = TempPopupMenu;
          foreach (TOboPublisher TempPublisher in AvailablePublishers)
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
      FCustomToolbar.DockRow = 0;
    }

    private void FTransformersPopupMenu_Popup(object sender, EventArgs e)
    {
      //throw new NotImplementedException();
    }

    private void ResetBarPositions() 
    {
      FBarManager.BeginUpdate();
      try 
      {
        int i = 0;
        foreach(DevExpress.XtraBars.Bar bar in FBarManager.Bars) 
        {
          bar.Offset = 0;
          bar.DockCol = i;
          bar.DockRow = 0;
          i++;
          bar.ApplyDockRowCol();
        }
      }
      finally 
      {
        FBarManager.EndUpdate();
      }
    }

    void FBarManager_ItemClick(object sender, ItemClickEventArgs e) 
    {
      BarButtonItem currentButton = e.Item as BarButtonItem;
      if (currentButton == null) return;
      
      
      if ((currentButton.Tag != null) && (currentButton.Tag is TMenuItemInfo))
      {
        TOboPublisher TempPublisher = (currentButton.Tag as TMenuItemInfo).Publisher;
        TOboTransformer TempTransformer = (currentButton.Tag as TMenuItemInfo).Transformer;

        
        //Framework.BuildPlate(TOboFramework.NullId, nil, nil);
        TOboDesktopPlate TempPanel = new TOboDesktopPlate();
        TempPanel.Text = "Report";        
        TempPanel.Show(FDockPanel, DockState.Float);
      }
      //this.FDockPanel.
      //DockPanel TempPanel = FDockManager.AddPanel(DockingStyle.Float);
      
    }

    public void Init (ref TOboFramework aFramework, ref Form aParentForm)
    {
      this.Framework = aFramework;
      this.FParentForm = aParentForm;
      this.FParentForm.IsMdiContainer = true;
      FBarManager = new BarManager();
      FBarManager.Form = aParentForm;      
      CreateStandardToolbar();
      CreateCustomizeToolbar();
      ResetBarPositions();
      FBarManager.ItemClick += new ItemClickEventHandler(FBarManager_ItemClick);

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

    public TOboDesktopPlate CreateDesktopPlate()
    {
      TOboDesktopPlate TempPanel = new TOboDesktopPlate();
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

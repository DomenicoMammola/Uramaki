// This is part of the Uramaki Framework for Winforms
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// This software is distributed without any warranty.
//
// @author Domenico Mammola (mimmo71@gmail.com - www.mammola.net)


using System;
using System.Windows.Forms;

using Mammola.Uramaki.Base;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections.Generic;

namespace Mammola.Uramaki.UI
{
  public class HuramakiDesktopPlate : DockContent
  {

    private UramakiFramework framework;

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


  public class UramakiDesktopManager
  {
    private class TMenuItemInfo
    {
      public UramakiPublisher Publisher;
      public UramakiTransformer Transformer;
    }


    private UramakiFramework framework;
    private Form parentForm;
    private DockPanel dockPanel;
    private ToolStrip mainToolbar;

    int FLastActivePanelHashCode;
    private ToolStripButton loadMenuButton;
    private ToolStripButton saveMenuButton;
    private ToolStripDropDownButton  addNewButton;
    private ToolStripButton FAddChildButton;
    private ContextMenuStrip FTransformersPopupMenu;


    private void CreateStandardToolbar()
    {
      loadMenuButton = new ToolStripButton("Load");
      mainToolbar.Items.Add(loadMenuButton);
      saveMenuButton = new ToolStripButton("Save");
      mainToolbar.Items.Add(saveMenuButton);
    }

    private void CreateCustomizeToolbar()    
    {
      addNewButton = new ToolStripDropDownButton("New");
      mainToolbar.Items.Add(addNewButton);      
      FTransformersPopupMenu = new ContextMenuStrip();
      FTransformersPopupMenu.Opening += FTransformersPopupMenu_Popup;
      addNewButton.DropDown = FTransformersPopupMenu;
      List<UramakiTransformer> AvailableTransformers = this.framework.GetAvailableTransformers(UramakiRoll.NullUramakiId);
      foreach (UramakiTransformer TempTransformer in AvailableTransformers)
      {       
        ToolStripMenuItem itm = new ToolStripMenuItem(TempTransformer.GetDescription());
        itm.Tag = TempTransformer.GetMyId();         

        FTransformersPopupMenu.Items.Add(itm);
        
        List<UramakiPublisher>AvailablePublishers = this.framework.GetAvailablePublishers(TempTransformer.GetOutputUramakiId());

        foreach (UramakiPublisher TempPublisher in AvailablePublishers)
        {                            
          TMenuItemInfo TempMenuInfo = new TMenuItemInfo();
          TempMenuInfo.Publisher = TempPublisher;
          TempMenuInfo.Transformer = TempTransformer;

          ToolStripMenuItem itm2 = new ToolStripMenuItem(TempPublisher.GetDescription());
          itm2.Tag = TempMenuInfo;                   
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
        UramakiPublisher TempPublisher = (currentButton.Tag as TMenuItemInfo).Publisher;
        UramakiTransformer TempTransformer = (currentButton.Tag as TMenuItemInfo).Transformer;


        //Framework.BuildPlate(HuramakiFramework.NullId, nil, nil);
        HuramakiDesktopPlate TempPanel = new HuramakiDesktopPlate();
        TempPanel.Text = "Report";
        TempPanel.Show(dockPanel, DockState.Float);
      }
      //this.FDockPanel.
      //DockPanel TempPanel = FDockManager.AddPanel(DockingStyle.Float);

    }

    public void Init(ref UramakiFramework aFramework, Form aParentForm)
    {
      this.framework = aFramework;
      this.parentForm = aParentForm;
      this.parentForm.IsMdiContainer = true;            

      this.dockPanel = new DockPanel();
      this.dockPanel.Dock = DockStyle.Fill;
      parentForm.Controls.Add(dockPanel);
      this.dockPanel.ActiveContentChanged += FDockPanel_ActiveContentChanged;
      
      mainToolbar = new ToolStrip();
      mainToolbar.Parent = parentForm;
      mainToolbar.Dock = DockStyle.Top;      
      CreateStandardToolbar();
      CreateCustomizeToolbar();
      //FBarManager.ItemClick += new ItemClickEventHandler(FBarManager_ItemClick);

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
        MessageBox.Show(TempNewHashCode.ToString());      
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

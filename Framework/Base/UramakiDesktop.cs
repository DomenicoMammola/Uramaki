using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mammola.Uramaki.Base;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.Xml;

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
    private DockPanel DockMainPanel;
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
      LoadMenuButton.Click += LoadMenuButton_Click;
      SaveMenuButton = new ToolStripButton("Save");
      MainToolbar.Items.Add(SaveMenuButton);
      SaveMenuButton.Click += SaveMenuButton_Click;
    }

    private void LoadMenuButton_Click(object sender, EventArgs e)
    {
      DockMainPanel.LoadFromXml("c:\\temp\\prova2.xml", );
    }

    private void SaveMenuButton_Click(object sender, EventArgs e)
    {

      XmlWriter writer = XmlWriter.Create("c:\\temp\\prova.xml");
      writer.WriteStartDocument();
      writer.WriteStartElement("layout");

      writer.WriteAttributeString("version", "1");

      Framework.SaveToXml(ref writer);
      
      writer.WriteEndElement(); // layout

      

      writer.WriteEndDocument();
      writer.Flush();

      DockMainPanel.SaveAsXml("c:\\temp\\prova2.xml");
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
        tempPublication.PublicationContext = tempPublisher.CreatePublisherContext();

        UramakiPlate actualPlate = Framework.BuildPlate(UramakiFramework.NullId, ref actualTransformations, ref tempPublication);
        actualPlate.Text = "Report";
        actualPlate.Show(DockMainPanel, DockState.Float);
      }
    }

    public void Init(ref UramakiFramework framework, Form parentForm)
    {
      this.Framework = framework;
      this.ParentForm = parentForm;
      this.ParentForm.IsMdiContainer = true;            

      this.DockMainPanel = new DockPanel();
      this.DockMainPanel.Dock = DockStyle.Fill;
      ParentForm.Controls.Add(DockMainPanel);
      this.DockMainPanel.ActiveContentChanged += FDockPanel_ActiveContentChanged;
      
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

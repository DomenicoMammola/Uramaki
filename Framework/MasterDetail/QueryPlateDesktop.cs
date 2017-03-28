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
using Mammola.Uramaki.Base;
//using DevExpress.XtraGrid;

namespace Mammola.Uramaki.MasterDetail
{

  public class QueryUramakiPlate : UramakiPlate
  {
    //private DevExpress.XtraGrid.GridControl FGridControl;
    //private DevExpress.XtraGrid.Views.Grid.GridView FGridView;
    

    public override UramakiRoll GetUramaki(string uramakiId)
    {
      return null;
    }

    public override void StartTransaction(Guid transactionId)    
    {
      // fa nulla
    }

    public override void EndTransaction(Guid transactionId)
    {
      // fa nulla
    }

    public void BuildGrid()
    {
      /*this.FGridControl = new DevExpress.XtraGrid.GridControl();
      this.FGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.FGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
      //this.gridControl1.Location = new System.Drawing.Point(0, 0);
      this.FGridControl.MainView = this.FGridView;
      //this.gridControl1.Name = "GridControl";
      //this.gridControl1.Size = new System.Drawing.Size(643, 285);
      this.FGridControl.TabIndex = 0;
      this.FGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {this.FGridView});
      this.FGridView.GridControl = this.FGridControl;
      */
    }

    //public OboAskToRefreshMyChilds AskToRefreshMyChilds;
  }
}

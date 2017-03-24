// This is part of the Uramaki Framework for Winforms
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// This software is distributed without any warranty.
//
// @author Domenico Mammola (mimmo71@gmail.com - www.mammola.net)


using Mammola.Uramaki.Base;
using Mammola.Uramaki.UI;

namespace Mammola.Uramaki.MasterDetail
{

 public class MasterQueryUramakiDesktopTransformer : MasterQueryUramakiTransformer
 {
   public override bool Configure (UramakiRoll aInput, ref UramakiTransformationContext aContext)
   {
     MasterQueryUramakiTransformationContext TempContext = (MasterQueryUramakiTransformationContext)aContext;

     return true;
   }   
 }

 public class MasterQueryUramakiDesktopPublisherGrid : MasterQueryUramakiPublisherGrid
 {
    private UramakiDesktopManager FDesktopManager;

    public override UramakiPlate CreatePlate()
    {
       QueryUramakiPlate TempPlate = new QueryUramakiPlate();
       return TempPlate;
    }

    public override void Publish (UramakiRoll aInput, ref UramakiPlate aPlate, ref UramakiPublicationContext aContext)
    {
      QueryUramakiPlate TempPlate = aPlate as QueryUramakiPlate;
      if (TempPlate.DesktopPlate == null)
      {
        TempPlate.DesktopPlate = FDesktopManager.CreateDesktopPlate();
        TempPlate.BuildGrid();
      }
    }

    public MasterQueryUramakiDesktopPublisherGrid (ref UramakiDesktopManager DesktopManager)
    {
      FDesktopManager = DesktopManager;
    }

 }

}
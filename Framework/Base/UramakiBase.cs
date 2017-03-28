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
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;

namespace Mammola.Uramaki.Base
{

  public class UramakiException : Exception 
  {
    public UramakiException(): base() {}
    public UramakiException(string message): base(message) {}
    public UramakiException(string message, Exception inner): base(message, inner) {}
  }

  public delegate void UramakiAskToRefreshMyChilds(UramakiPlate aPlate);

  public abstract class UramakiRoll
  {
    public abstract string GetMyId();
    public abstract string GetDescription();
    public abstract bool CanBeCached();

    public abstract void Init();
    public abstract void BeforeRead();
    public abstract void AfterRead();

    public static string NullUramakiId = "**NULL**"; 
  }

  public abstract class UramakiPlate
  {
    private Guid instanceIdentifier;
    public Guid InstanceIdentifier 
    {
      get
      {
        return instanceIdentifier;
      }
    }    
    
    private UramakiFramework framework;
    public UramakiFramework Framework 
    {
      get
      {
        return framework;
      }
    }
    
    private DockContent dockpanel;
    public DockContent DockPanel 
    {
      get
      {
        return dockpanel;
      }
    }

    public void Init (ref DockContent dockContent, Guid newInstanceIdentifier)
    {
      this.dockpanel = dockContent;
      this.instanceIdentifier = newInstanceIdentifier;
    }

    public abstract UramakiRoll GetUramaki(string uramakiId);
    public abstract void StartTransaction(Guid transactionId);
    public abstract void EndTransaction(Guid transactionId);

    public UramakiAskToRefreshMyChilds AskToRefreshMyChilds;
  }

  public abstract class UramakiPublicationContext
  {
  }

  public abstract class UramakiPublisher
  {
    public abstract string GetMyId();
    public abstract string GetDescription();
    public abstract string GetHelpDescription();

    public abstract string GetInputUramakiId();

    public abstract UramakiPlate CreatePlate();    
    public abstract UramakiPublicationContext CreatePublisherContext();
    public abstract void StartTransaction(Guid transactionId);
    public abstract void EndTransaction(Guid transactionId);

    public abstract void Publish (UramakiRoll input, ref UramakiPlate plate, ref UramakiPublicationContext context);    
  }

  public abstract class UramakiTransformationContext
  {
    public abstract void SaveToXML (ref XmlWriter writer);
    public abstract void LoadFromXML (ref XmlReader reader);
  }
  

  public abstract class UramakiTransformer
  {
    public abstract string GetMyId();
    public abstract string GetDescription();
    public abstract string GetHelpDescription();

    public abstract string GetInputUramakiId();    
    public abstract string GetOutputUramakiId();

    //public abstract Huramaki CreateOutpuHuramaki();
    public abstract UramakiTransformationContext CreateTransformerContext();

    public abstract bool Configure (UramakiRoll input, ref UramakiTransformationContext context);
    public abstract UramakiRoll Transform (UramakiRoll input, ref UramakiTransformationContext context);        

    public abstract void StartTransaction(Guid transactionId);
    public abstract void EndTransaction(Guid transactionId);
  }
}
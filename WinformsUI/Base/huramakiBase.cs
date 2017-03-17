// This is part of the Huramaki Framework for Winforms
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


namespace Mammola.Huramaki.WinformsUI.Base
{

  public delegate void HuramakiAskToRefreshMyChilds(HuramakiPlate aPlate);

  public abstract class Huramaki
  {
    public abstract string GetMyId();
    public abstract string GetDescription();
    public abstract bool CanBeCached();

    public abstract void Init();
    public abstract void BeforeRead();
    public abstract void AfterRead();

    public static string NullHuramakiId = "**NULL**"; 
  }

  public abstract class HuramakiPlate
  {
    public Guid MyActualId;

    public abstract Huramaki GeHuramaki(string aHuramakiId);
    public abstract void StartTransaction(Guid aTransactionId);
    public abstract void EndTransaction(Guid aTransactionId);

    public HuramakiAskToRefreshMyChilds AskToRefreshMyChilds;
  }

  public abstract class HuramakiPublicationContext
  {
  }

  public abstract class HuramakiPublisher
  {
    public abstract string GetMyId();
    public abstract string GetDescription();
    public abstract string GetHelpDescription();

    public abstract string GetInpuHuramakiId();

    public abstract HuramakiPlate CreatePlate();    
    public abstract HuramakiPublicationContext CreatePublisherContext();
    public abstract void StartTransaction(Guid aTransactionId);
    public abstract void EndTransaction(Guid aTransactionId);

    public abstract void Publish (Huramaki aInput, ref HuramakiPlate aPlate, ref HuramakiPublicationContext aContext);    
  }

  public abstract class HuramakiTransformationContext
  {
    public abstract void SaveToXML (ref XmlWriter aWriter);
    public abstract void LoadFromXML (ref XmlReader aReader);
  }
  

  public abstract class HuramakiTransformer
  {
    public abstract string GetMyId();
    public abstract string GetDescription();
    public abstract string GetHelpDescription();

    public abstract string GetInpuHuramakiId();    
    public abstract string GetOutpuHuramakiId();

    //public abstract Huramaki CreateOutpuHuramaki();
    public abstract HuramakiTransformationContext CreateTransformerContext();

    public abstract bool Configure (Huramaki aInput, ref HuramakiTransformationContext aContext);
    public abstract Huramaki Transform (Huramaki aInput, ref HuramakiTransformationContext aContext);        

    public abstract void StartTransaction(Guid aTransactionId);
    public abstract void EndTransaction(Guid aTransactionId);
  }
}
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


namespace Mammola.Uramaki.Base
{

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
    public Guid MyActualId;

    public abstract UramakiRoll GetUramaki(string aUramakiId);
    public abstract void StartTransaction(Guid aTransactionId);
    public abstract void EndTransaction(Guid aTransactionId);

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
    public abstract void StartTransaction(Guid aTransactionId);
    public abstract void EndTransaction(Guid aTransactionId);

    public abstract void Publish (UramakiRoll aInput, ref UramakiPlate aPlate, ref UramakiPublicationContext aContext);    
  }

  public abstract class UramakiTransformationContext
  {
    public abstract void SaveToXML (ref XmlWriter aWriter);
    public abstract void LoadFromXML (ref XmlReader aReader);
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

    public abstract bool Configure (UramakiRoll aInput, ref UramakiTransformationContext aContext);
    public abstract UramakiRoll Transform (UramakiRoll aInput, ref UramakiTransformationContext aContext);        

    public abstract void StartTransaction(Guid aTransactionId);
    public abstract void EndTransaction(Guid aTransactionId);
  }
}
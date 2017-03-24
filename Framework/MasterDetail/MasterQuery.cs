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
using Mammola.Uramaki.Base;

namespace Mammola.Uramaki.MasterDetail
{

  public class MasterQueryUramakiRoll: UramakiRoll
  {
    private MasterQuery FMasterQuery;

    public static string UramakiId = "Uramaki.MasterQuery";    

    public override string GetMyId()
    {
      return MasterQueryUramakiRoll.UramakiId;
    }

    public override string GetDescription()
    {
      return "Master query"; 
    }

    public override bool CanBeCached()
    {
      return true;
    }

    public override void Init()
    {
      // do nothing
    }

    public override void BeforeRead()
    {
      // do nothing
    }
    
    public override void AfterRead()
    {
      // do nothing
    }
    
    public MasterQueryUramakiRoll ()
    {
      FMasterQuery = new MasterQuery();
    }
    
    public MasterQuery MasterQuery
    {
      get { return FMasterQuery; }
    }
  }

  public class MasterQueryUramakiTransformationContext : UramakiTransformationContext
  {
    private MasterQuery FMasterQuery;

    public MasterQueryUramakiTransformationContext()
    {
      FMasterQuery = new MasterQuery();
    }

    public MasterQuery MasterQuery
    {
      get { return FMasterQuery; }
    }

    public override void SaveToXML (ref XmlWriter aWriter)
    {
      aWriter.WriteStartElement("masterQueryConfiguration");
      aWriter.WriteElementString("masterQuery", FMasterQuery.SQLString);
      aWriter.WriteEndElement();
    }

    public override void LoadFromXML (ref XmlReader aReader)
    {
      aReader.ReadStartElement("masterQueryConfiguration");      
      FMasterQuery.SQLString = aReader.ReadElementContentAsString();
    }
    
  }

  public class MasterQueryUramakiTransformer : UramakiTransformer
  {
    public static string OboId = "Uramaki.MasterQueryTransformer";

    public override string GetMyId()
    {
      return MasterQueryUramakiTransformer.OboId;
    }

    public override string GetDescription()
    {
      return "Database query";
    }

    public override string GetHelpDescription()
    {
      return "Define a query to read some data from the database.";
    }

    public override string GetInputUramakiId()
    {
      return UramakiRoll.NullUramakiId;
    }

    public override string GetOutputUramakiId()
    {
      return MasterQueryUramakiRoll.UramakiId;
    }

    public override UramakiTransformationContext CreateTransformerContext()
    {
      return new MasterQueryUramakiTransformationContext();
    }

    public override bool Configure (UramakiRoll aInput, ref UramakiTransformationContext aContext)
    {
      return true;
    }

    public override UramakiRoll Transform (UramakiRoll aInput, ref UramakiTransformationContext aContext)
    {
      MasterQueryUramakiRoll OutputUramaki = new MasterQueryUramakiRoll();
      MasterQueryUramakiTransformationContext TempContext = (MasterQueryUramakiTransformationContext)aContext;

      if (TempContext.MasterQuery.SQLString.Length == 0)
        this.Configure(null, ref aContext);

      OutputUramaki.MasterQuery.CopyFrom (TempContext.MasterQuery);

      return OutputUramaki;
    }        

    public override void StartTransaction(Guid aTransactionId)
    {
    }

    public override void EndTransaction(Guid aTransactionId)
    {
    }        

  }

  public class UramakiQueryPublisherGridContext : UramakiPublicationContext
  {
    // anything?
  }

  public class MasterQueryUramakiPublisherGrid : UramakiPublisher
  {
    public static string UramakiId = "Uramaki.MasterQueryPublisherGrid";

    public override string GetMyId()
    {
      return MasterQueryUramakiPublisherGrid.UramakiId;
    }

    public override string GetDescription()
    {
      return "Grid";
    }

    public override string GetHelpDescription()
    {
      return "Publish the data extracted by a query into a grid.";
    }

    public override string GetInputUramakiId()
    {
      return MasterQueryUramakiRoll.UramakiId;
    }

    public override UramakiPlate CreatePlate()
    {
      return null;
    }

    public override UramakiPublicationContext CreatePublisherContext()
    {
      return new UramakiQueryPublisherGridContext();
    }

    public override void StartTransaction(Guid aTransactionId)
    {
      // do nothing
    }

    public override void EndTransaction(Guid aTransactionId)
    {
      // do nothing
    }

    public override void Publish (UramakiRoll aInput, ref UramakiPlate aPlate, ref UramakiPublicationContext aContext)
    {
      // do nothing
    }
    
    
  }

}
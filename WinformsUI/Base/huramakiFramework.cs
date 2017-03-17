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
using System.Collections.Generic;

namespace Mammola.Huramaki.WinformsUI.Base
{ 
  
  /*public class HuramakiAvailableTransformation
  {
    public string TransformerId;
    public string TransformerDescription;    
  }*/

  public class HuramakiActualTransformation
  {
    public HuramakiTransformer Transformer;
    public HuramakiTransformationContext TransformationContext;
  }

  public class HuramakiActualPublication
  {
    public HuramakiPublisher Publisher;
    public HuramakiPublicationContext PublicationContext;
  }

  public class HuramakiFramework
  {
    public static Guid NullId = Guid.Empty;

    private class HuramakiLivingTransformation
    {
      public HuramakiTransformer Transformer;
      public HuramakiTransformationContext TransformationContext;

      public HuramakiLivingTransformation()
      {
        Transformer = null;
        TransformationContext = null;
      }
    }

    private class HuramakiLivingPlate 
    {
      public bool IsNullPlate;
      public HuramakiLivingPlate ParentPlate;
      public HuramakiPublisher Publisher;
      public HuramakiPublicationContext PublicationContext;
      public List<HuramakiLivingTransformation> Transformations;
      public HuramakiPlate Plate;
      public List<HuramakiLivingPlate> ChildPlates;

      public HuramakiLivingPlate()
      {
        Plate = null;
        ParentPlate = null;        
        Publisher = null;
        PublicationContext = null;
        Transformations = new List<HuramakiLivingTransformation>();
        IsNullPlate = false;
        ChildPlates = new List<HuramakiLivingPlate>();        
      }
    }

    private Dictionary<string, HuramakiTransformer> FTransformers;
    private Dictionary<string, HuramakiPublisher> FPublishers;
    private Dictionary<Guid, HuramakiLivingPlate> FLivingPlates;
    private Guid FCurrentTransactionId;

    public HuramakiFramework()
    {
      FTransformers = new Dictionary<string, HuramakiTransformer>();
      FPublishers = new Dictionary<string, HuramakiPublisher>();
      FLivingPlates = new Dictionary<Guid, HuramakiLivingPlate>();
      FCurrentTransactionId = Guid.Empty;
    }

    public void AddPublisher (HuramakiPublisher aPublisher)
    {
      if (! FPublishers.ContainsKey(aPublisher.GetMyId()))
      {
        FPublishers.Add(aPublisher.GetMyId(), aPublisher);      
      }
    }

    private Huramaki BuildHuramakiFromTransformations (HuramakiLivingPlate aParentPlate, List<HuramakiLivingTransformation> aTransformations, HuramakiPublisher aPublisher)
    {
      string currenHuramakiId = "";
      Huramaki currenHuramaki = null;

      if (aTransformations.Count > 0)
      {
        currenHuramakiId = aTransformations[0].Transformer.GetOutpuHuramakiId();
        if (! currenHuramakiId.Equals(Huramaki.NullHuramakiId))
        {
          currenHuramaki = aParentPlate.Plate.GeHuramaki(currenHuramakiId);
        }
          
        for (int i = 0; i < aTransformations.Count; i++)
        { 
          currenHuramaki = aTransformations[i].Transformer.Transform(currenHuramaki, ref aTransformations[i].TransformationContext);
        }       
      }
      else
      {
        currenHuramakiId = aPublisher.GetInpuHuramakiId();
        
        if (! currenHuramakiId.Equals(Huramaki.NullHuramakiId))
        {
          currenHuramaki = aParentPlate.Plate.GeHuramaki(currenHuramakiId);
        }
      }
      return currenHuramaki;    
    }

    private void ServeAskToRefreshMyChilds(HuramakiPlate aPlate)
    {
      HuramakiLivingPlate TempLivingPlate;
      if (FLivingPlates.TryGetValue(aPlate.MyActualId, out TempLivingPlate))
      {
        Huramaki CurrenHuramaki = null;
        
        this.StartTransaction();
        try
        {
          foreach (HuramakiLivingPlate ChildPlate in TempLivingPlate.ChildPlates)
          {
            CurrenHuramaki = BuildHuramakiFromTransformations(TempLivingPlate, ChildPlate.Transformations, ChildPlate.Publisher);
            ChildPlate.Publisher.Publish(CurrenHuramaki, ref ChildPlate.Plate, ref ChildPlate.PublicationContext);      
          }
        }
        finally
        {
          this.EndTransaction();
        }
      }
    }

    public void AddTransformer (HuramakiTransformer aTransformer)
    {
      if (! FTransformers.ContainsKey(aTransformer.GetMyId()))
      {
        FTransformers.Add(aTransformer.GetMyId(), aTransformer);
      }
    } 
    
    public HuramakiPlate BuildPlate(Guid aParentPlateId, ref List<HuramakiActualTransformation> aTransformations, ref HuramakiActualPublication aPublication)
    {
      HuramakiLivingPlate TempParentPlate;      

      if (aParentPlateId == NullId)
      {
        TempParentPlate = new HuramakiLivingPlate();
        TempParentPlate.IsNullPlate = true;
      }
      else
      {
        if (! FLivingPlates.TryGetValue(aParentPlateId, out TempParentPlate))
        {
          return null;
        }
      }
      HuramakiLivingPlate TempNewLivingPlate = new HuramakiLivingPlate();
      TempNewLivingPlate.ParentPlate = TempParentPlate;      
      TempNewLivingPlate.Publisher = aPublication.Publisher;
      TempNewLivingPlate.PublicationContext = aPublication.PublicationContext;
      TempNewLivingPlate.Plate = TempNewLivingPlate.Publisher.CreatePlate();
      TempNewLivingPlate.Plate.MyActualId = new Guid();
      TempNewLivingPlate.Plate.AskToRefreshMyChilds = this.ServeAskToRefreshMyChilds;
      foreach(HuramakiActualTransformation TempTransf in aTransformations)
      {
        HuramakiLivingTransformation TempLivingTransf = new HuramakiLivingTransformation();
        TempLivingTransf.Transformer = TempTransf.Transformer;
        TempLivingTransf.TransformationContext = TempTransf.TransformationContext;
          
        TempNewLivingPlate.Transformations.Add(TempLivingTransf);
      }

      FLivingPlates.Add(TempNewLivingPlate.Plate.MyActualId, TempNewLivingPlate);

      //string currenHuramakiId = "";
      Huramaki currenHuramaki = BuildHuramakiFromTransformations(TempNewLivingPlate.ParentPlate, TempNewLivingPlate.Transformations, TempNewLivingPlate.Publisher);  

      /*
      if (TempNewLivingPlate.Transformations.Count > 0)
      {
        currenHuramakiId = TempNewLivingPlate.Transformations[0].Transformer.GetOutpuHuramakiId();
        if (! currenHuramakiId.Equals(Huramaki.NullHuramakiId))
        {
          currenHuramaki = TempNewLivingPlate.ParentPlate.Plate.GeHuramaki(currenHuramakiId);
        }
          
        for (int i = 0; i < TempNewLivingPlate.Transformations.Count; i++)
        { 
          currenHuramaki = TempNewLivingPlate.Transformations[i].Transformer.Transform(currenHuramaki, ref TempNewLivingPlate.Transformations[i].TransformationContext);
        }       
      }
      else
      {
        currenHuramakiId = TempNewLivingPlate.Publisher.GetInpuHuramakiId();
        
        if (! currenHuramakiId.Equals(Huramaki.NullHuramakiId))
        {
          currenHuramaki = TempNewLivingPlate.ParentPlate.Plate.GeHuramaki(currenHuramakiId);
        }
      }
      */
      TempParentPlate.ChildPlates.Add(TempNewLivingPlate);
      TempNewLivingPlate.Publisher.Publish(currenHuramaki, ref TempNewLivingPlate.Plate, ref TempNewLivingPlate.PublicationContext);      

      return TempNewLivingPlate.Plate;               
    }

    private void StartTransaction()
    {
      if (! FCurrentTransactionId.Equals(Guid.Empty))
      {
        throw new System.Exception("HuramakiFramework: Transaction already in progress.");
      }

      FCurrentTransactionId = Guid.NewGuid();

      
      foreach (KeyValuePair<string, HuramakiTransformer> entry in FTransformers)
      {
        entry.Value.StartTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<string, HuramakiPublisher> entry in FPublishers)
      {
        entry.Value.StartTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<Guid, HuramakiLivingPlate> entry in FLivingPlates)
      {
        entry.Value.Plate.StartTransaction(FCurrentTransactionId);
      }

    }

    private void EndTransaction()
    {
      if (FCurrentTransactionId.Equals(Guid.Empty))
      {
        throw new System.Exception("HuramakiFramework: No transaction is in progress.");
      }
            
      foreach (KeyValuePair<string, HuramakiTransformer> entry in FTransformers)
      {
        entry.Value.EndTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<string, HuramakiPublisher> entry in FPublishers)
      {
        entry.Value.EndTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<Guid, HuramakiLivingPlate> entry in FLivingPlates)
      {
        entry.Value.Plate.EndTransaction(FCurrentTransactionId);
      }

      FCurrentTransactionId = Guid.Empty;
    }

    public List <HuramakiTransformer> GetAvailableTransformers (string aInpuHuramakiId)
    {
      List<HuramakiTransformer> TempList = new List<HuramakiTransformer>();
      foreach (KeyValuePair<string, HuramakiTransformer > entry in FTransformers)
      {
         if (String.Compare(entry.Value.GetInpuHuramakiId(), aInpuHuramakiId, true) == 0)
         {
           TempList.Add(entry.Value);
         }
      }
      return TempList;
    }

    public List <HuramakiPublisher> GetAvailablePublishers (string aInpuHuramakiId)
    {
      List<HuramakiPublisher> TempList = new List<HuramakiPublisher>();
      foreach(KeyValuePair<string, HuramakiPublisher> entry in FPublishers)
      {
        if (String.Compare(entry.Value.GetInpuHuramakiId(), aInpuHuramakiId, true) == 0)
        {
          TempList.Add(entry.Value);
        }
      }
      return TempList;
    }
    
  }

}
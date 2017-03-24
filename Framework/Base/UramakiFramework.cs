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
using System.Collections.Generic;

namespace Mammola.Uramaki.Base
{ 
  
  /*public class HuramakiAvailableTransformation
  {
    public string TransformerId;
    public string TransformerDescription;    
  }*/

  public class UramakiActualTransformation
  {
    public UramakiTransformer Transformer;
    public UramakiTransformationContext TransformationContext;
  }

  public class HuramakiActualPublication
  {
    public UramakiPublisher Publisher;
    public UramakiPublicationContext PublicationContext;
  }

  public class UramakiFramework
  {
    public static Guid NullId = Guid.Empty;

    private class UramakiLivingTransformation
    {
      public UramakiTransformer Transformer;
      public UramakiTransformationContext TransformationContext;

      public UramakiLivingTransformation()
      {
        Transformer = null;
        TransformationContext = null;
      }
    }

    private class UramakiLivingPlate 
    {
      public bool IsNullPlate;
      public UramakiLivingPlate ParentPlate;
      public UramakiPublisher Publisher;
      public UramakiPublicationContext PublicationContext;
      public List<UramakiLivingTransformation> Transformations;
      public UramakiPlate Plate;
      public List<UramakiLivingPlate> ChildPlates;

      public UramakiLivingPlate()
      {
        Plate = null;
        ParentPlate = null;        
        Publisher = null;
        PublicationContext = null;
        Transformations = new List<UramakiLivingTransformation>();
        IsNullPlate = false;
        ChildPlates = new List<UramakiLivingPlate>();        
      }
    }

    private Dictionary<string, UramakiTransformer> FTransformers;
    private Dictionary<string, UramakiPublisher> FPublishers;
    private Dictionary<Guid, UramakiLivingPlate> FLivingPlates;
    private Guid FCurrentTransactionId;

    public UramakiFramework()
    {
      FTransformers = new Dictionary<string, UramakiTransformer>();
      FPublishers = new Dictionary<string, UramakiPublisher>();
      FLivingPlates = new Dictionary<Guid, UramakiLivingPlate>();
      FCurrentTransactionId = Guid.Empty;
    }

    public void AddPublisher (UramakiPublisher aPublisher)
    {
      if (! FPublishers.ContainsKey(aPublisher.GetMyId()))
      {
        FPublishers.Add(aPublisher.GetMyId(), aPublisher);      
      }
    }

    private UramakiRoll BuildHuramakiFromTransformations (UramakiLivingPlate aParentPlate, List<UramakiLivingTransformation> aTransformations, UramakiPublisher aPublisher)
    {
      string currenHuramakiId = "";
      UramakiRoll currenHuramaki = null;

      if (aTransformations.Count > 0)
      {
        currenHuramakiId = aTransformations[0].Transformer.GetOutputUramakiId();
        if (! currenHuramakiId.Equals(UramakiRoll.NullUramakiId))
        {
          currenHuramaki = aParentPlate.Plate.GetUramaki(currenHuramakiId);
        }
          
        for (int i = 0; i < aTransformations.Count; i++)
        { 
          currenHuramaki = aTransformations[i].Transformer.Transform(currenHuramaki, ref aTransformations[i].TransformationContext);
        }       
      }
      else
      {
        currenHuramakiId = aPublisher.GetInputUramakiId();
        
        if (! currenHuramakiId.Equals(UramakiRoll.NullUramakiId))
        {
          currenHuramaki = aParentPlate.Plate.GetUramaki(currenHuramakiId);
        }
      }
      return currenHuramaki;    
    }

    private void ServeAskToRefreshMyChilds(UramakiPlate aPlate)
    {
      UramakiLivingPlate TempLivingPlate;
      if (FLivingPlates.TryGetValue(aPlate.MyActualId, out TempLivingPlate))
      {
        UramakiRoll CurrenHuramaki = null;
        
        this.StartTransaction();
        try
        {
          foreach (UramakiLivingPlate ChildPlate in TempLivingPlate.ChildPlates)
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

    public void AddTransformer (UramakiTransformer aTransformer)
    {
      if (! FTransformers.ContainsKey(aTransformer.GetMyId()))
      {
        FTransformers.Add(aTransformer.GetMyId(), aTransformer);
      }
    } 
    
    public UramakiPlate BuildPlate(Guid aParentPlateId, ref List<UramakiActualTransformation> aTransformations, ref HuramakiActualPublication aPublication)
    {
      UramakiLivingPlate TempParentPlate;      

      if (aParentPlateId == NullId)
      {
        TempParentPlate = new UramakiLivingPlate();
        TempParentPlate.IsNullPlate = true;
      }
      else
      {
        if (! FLivingPlates.TryGetValue(aParentPlateId, out TempParentPlate))
        {
          return null;
        }
      }
      UramakiLivingPlate TempNewLivingPlate = new UramakiLivingPlate();
      TempNewLivingPlate.ParentPlate = TempParentPlate;      
      TempNewLivingPlate.Publisher = aPublication.Publisher;
      TempNewLivingPlate.PublicationContext = aPublication.PublicationContext;
      TempNewLivingPlate.Plate = TempNewLivingPlate.Publisher.CreatePlate();
      TempNewLivingPlate.Plate.MyActualId = new Guid();
      TempNewLivingPlate.Plate.AskToRefreshMyChilds = this.ServeAskToRefreshMyChilds;
      foreach(UramakiActualTransformation TempTransf in aTransformations)
      {
        UramakiLivingTransformation TempLivingTransf = new UramakiLivingTransformation();
        TempLivingTransf.Transformer = TempTransf.Transformer;
        TempLivingTransf.TransformationContext = TempTransf.TransformationContext;
          
        TempNewLivingPlate.Transformations.Add(TempLivingTransf);
      }

      FLivingPlates.Add(TempNewLivingPlate.Plate.MyActualId, TempNewLivingPlate);

      //string currenHuramakiId = "";
      UramakiRoll currenHuramaki = BuildHuramakiFromTransformations(TempNewLivingPlate.ParentPlate, TempNewLivingPlate.Transformations, TempNewLivingPlate.Publisher);  

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

      
      foreach (KeyValuePair<string, UramakiTransformer> entry in FTransformers)
      {
        entry.Value.StartTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<string, UramakiPublisher> entry in FPublishers)
      {
        entry.Value.StartTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<Guid, UramakiLivingPlate> entry in FLivingPlates)
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
            
      foreach (KeyValuePair<string, UramakiTransformer> entry in FTransformers)
      {
        entry.Value.EndTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<string, UramakiPublisher> entry in FPublishers)
      {
        entry.Value.EndTransaction(FCurrentTransactionId);
      }

      foreach (KeyValuePair<Guid, UramakiLivingPlate> entry in FLivingPlates)
      {
        entry.Value.Plate.EndTransaction(FCurrentTransactionId);
      }

      FCurrentTransactionId = Guid.Empty;
    }

    public List <UramakiTransformer> GetAvailableTransformers (string aInpuHuramakiId)
    {
      List<UramakiTransformer> TempList = new List<UramakiTransformer>();
      foreach (KeyValuePair<string, UramakiTransformer > entry in FTransformers)
      {
         if (String.Compare(entry.Value.GetInputUramakiId(), aInpuHuramakiId, true) == 0)
         {
           TempList.Add(entry.Value);
         }
      }
      return TempList;
    }

    public List <UramakiPublisher> GetAvailablePublishers (string aInpuHuramakiId)
    {
      List<UramakiPublisher> TempList = new List<UramakiPublisher>();
      foreach(KeyValuePair<string, UramakiPublisher> entry in FPublishers)
      {
        if (String.Compare(entry.Value.GetInputUramakiId(), aInpuHuramakiId, true) == 0)
        {
          TempList.Add(entry.Value);
        }
      }
      return TempList;
    }
    
  }

}
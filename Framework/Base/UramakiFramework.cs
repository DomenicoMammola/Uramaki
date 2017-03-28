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
using WeifenLuo.WinFormsUI.Docking;

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

  public class UramakiActualPublication
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

    private Dictionary<string, UramakiTransformer> Transformers;
    private Dictionary<string, UramakiPublisher> Publishers;
    private Dictionary<Guid, UramakiLivingPlate> LivingPlates;
    private Guid CurrentTransactionId;

    public UramakiFramework()
    {
      Transformers = new Dictionary<string, UramakiTransformer>();
      Publishers = new Dictionary<string, UramakiPublisher>();
      LivingPlates = new Dictionary<Guid, UramakiLivingPlate>();
      CurrentTransactionId = Guid.Empty;
    }

    public void AddPublisher (UramakiPublisher publisher)
    {
      if (! Publishers.ContainsKey(publisher.GetMyId()))
      {
        Publishers.Add(publisher.GetMyId(), publisher);      
      }
    }

    private UramakiRoll BuildUramakiFromTransformations (UramakiLivingPlate parentPlate, List<UramakiLivingTransformation> transformations, UramakiPublisher publisher)
    {
      string currenHuramakiId = "";
      UramakiRoll currenHuramaki = null;

      if (transformations.Count > 0)
      {
        currenHuramakiId = transformations[0].Transformer.GetInputUramakiId();
        if (! currenHuramakiId.Equals(UramakiRoll.NullUramakiId))
        {
          currenHuramaki = parentPlate.Plate.GetUramaki(currenHuramakiId);
        }
          
        for (int i = 0; i < transformations.Count; i++)
        { 
          currenHuramaki = transformations[i].Transformer.Transform(currenHuramaki, ref transformations[i].TransformationContext);
        }       
      }
      else
      {
        currenHuramakiId = publisher.GetInputUramakiId();
        
        if (! currenHuramakiId.Equals(UramakiRoll.NullUramakiId))
        {
          currenHuramaki = parentPlate.Plate.GetUramaki(currenHuramakiId);
        }
      }
      return currenHuramaki;    
    }

    private void ServeAskToRefreshMyChilds(UramakiPlate plate)
    {
      UramakiLivingPlate tempLivingPlate;
      if (LivingPlates.TryGetValue(plate.InstanceIdentifier, out tempLivingPlate))
      {
        UramakiRoll currenHuramaki = null;
        
        this.StartTransaction();
        try
        {
          foreach (UramakiLivingPlate ChildPlate in tempLivingPlate.ChildPlates)
          {
            currenHuramaki = BuildUramakiFromTransformations(tempLivingPlate, ChildPlate.Transformations, ChildPlate.Publisher);
            ChildPlate.Publisher.Publish(currenHuramaki, ref ChildPlate.Plate, ref ChildPlate.PublicationContext);      
          }
        }
        finally
        {
          this.EndTransaction();
        }
      }
    }

    public void AddTransformer (UramakiTransformer transformer)
    {
      if (! Transformers.ContainsKey(transformer.GetMyId()))
      {
        Transformers.Add(transformer.GetMyId(), transformer);
      }
    } 
    
    public UramakiPlate BuildPlate(Guid parentPlateId, ref List<UramakiActualTransformation> transformations, ref UramakiActualPublication publication)
    {
      UramakiLivingPlate tempParentPlate;      

      if (parentPlateId == NullId)
      {
        tempParentPlate = new UramakiLivingPlate();
        tempParentPlate.IsNullPlate = true;
      }
      else
      {
        if (! LivingPlates.TryGetValue(parentPlateId, out tempParentPlate))
        {
          return null;
        }
      }
      UramakiLivingPlate tempNewLivingPlate = new UramakiLivingPlate();
      tempNewLivingPlate.ParentPlate = tempParentPlate;      
      tempNewLivingPlate.Publisher = publication.Publisher;
      tempNewLivingPlate.PublicationContext = publication.PublicationContext; 
      DockContent tmpDockContent = new DockContent();     
      tempNewLivingPlate.Plate = tempNewLivingPlate.Publisher.CreatePlate();      
      tempNewLivingPlate.Plate.Init(ref tmpDockContent, Guid.NewGuid());
      tempNewLivingPlate.Plate.AskToRefreshMyChilds = this.ServeAskToRefreshMyChilds;
      foreach(UramakiActualTransformation tempTransf in transformations)
      {
        UramakiLivingTransformation tempLivingTransf = new UramakiLivingTransformation();
        tempLivingTransf.Transformer = tempTransf.Transformer;
        tempLivingTransf.TransformationContext = tempTransf.TransformationContext;
          
        tempNewLivingPlate.Transformations.Add(tempLivingTransf);
      }

      LivingPlates.Add(tempNewLivingPlate.Plate.InstanceIdentifier, tempNewLivingPlate);

      //string currenHuramakiId = "";
      UramakiRoll currenHuramaki = BuildUramakiFromTransformations(tempNewLivingPlate.ParentPlate, tempNewLivingPlate.Transformations, tempNewLivingPlate.Publisher);  

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
      tempParentPlate.ChildPlates.Add(tempNewLivingPlate);
      tempNewLivingPlate.Publisher.Publish(currenHuramaki, ref tempNewLivingPlate.Plate, ref tempNewLivingPlate.PublicationContext);      

      return tempNewLivingPlate.Plate;               
    }

    private void StartTransaction()
    {
      if (! CurrentTransactionId.Equals(Guid.Empty))
      {
        throw new UramakiException("UramakiFramework: Transaction already in progress.");
      }

      CurrentTransactionId = Guid.NewGuid();

      
      foreach (KeyValuePair<string, UramakiTransformer> entry in Transformers)
      {
        entry.Value.StartTransaction(CurrentTransactionId);
      }

      foreach (KeyValuePair<string, UramakiPublisher> entry in Publishers)
      {
        entry.Value.StartTransaction(CurrentTransactionId);
      }

      foreach (KeyValuePair<Guid, UramakiLivingPlate> entry in LivingPlates)
      {
        entry.Value.Plate.StartTransaction(CurrentTransactionId);
      }

    }

    private void EndTransaction()
    {
      if (CurrentTransactionId.Equals(Guid.Empty))
      {
        throw new UramakiException("UramakiFramework: No transaction is in progress.");
      }
            
      foreach (KeyValuePair<string, UramakiTransformer> entry in Transformers)
      {
        entry.Value.EndTransaction(CurrentTransactionId);
      }

      foreach (KeyValuePair<string, UramakiPublisher> entry in Publishers)
      {
        entry.Value.EndTransaction(CurrentTransactionId);
      }

      foreach (KeyValuePair<Guid, UramakiLivingPlate> entry in LivingPlates)
      {
        entry.Value.Plate.EndTransaction(CurrentTransactionId);
      }

      CurrentTransactionId = Guid.Empty;
    }

    public List <UramakiTransformer> GetAvailableTransformers (string inputUramakiId)
    {
      List<UramakiTransformer> tempList = new List<UramakiTransformer>();
      foreach (KeyValuePair<string, UramakiTransformer > entry in Transformers)
      {
         if (String.Compare(entry.Value.GetInputUramakiId(), inputUramakiId, true) == 0)
         {
           tempList.Add(entry.Value);
         }
      }
      return tempList;
    }

    public List <UramakiPublisher> GetAvailablePublishers (string inputUramakiId)
    {
      List<UramakiPublisher> tempList = new List<UramakiPublisher>();
      foreach(KeyValuePair<string, UramakiPublisher> entry in Publishers)
      {
        if (String.Compare(entry.Value.GetInputUramakiId(), inputUramakiId, true) == 0)
        {
          tempList.Add(entry.Value);
        }
      }
      return tempList;
    }
    
  }

}
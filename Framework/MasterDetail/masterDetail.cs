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
using System.Data;
using System.Text;
using Mammola.Uramaki.Data;


namespace Mammola.Uramaki.MasterDetail
{
  public class MasterQuery
  {
    private string FSQLString;

    public string SQLString
    {
      get {return FSQLString;}
      set {FSQLString = value;}
    }

    public void CopyFrom (MasterQuery aSource)
    {
      this.FSQLString = aSource.FSQLString;
    }
  }

  public enum DetailConditionKind
  {
    ckField,
    ckConstant
  }

  public class MasterDetailCondition
  {
    private string FRefValue;
    private string FColumn;
    private DetailConditionKind FKind;
    private Type FDataType;

    public MasterDetailCondition()
    {
      FRefValue = null;
      FColumn = null;
      FKind = DetailConditionKind.ckField;
      FDataType = null;
    }

    public string RefValue
    {
      get { return FRefValue; }
      set { FRefValue = value; }
    }

    public string Column
    { 
      get { return FColumn; }
      set { FColumn = value; }
    }
    public DetailConditionKind Kind
    {
      get { return FKind; }
      set { FKind = value; }
    }

    public Type DataType 
    {
      get {return FDataType;}
      set {FDataType = value;}
    }
  
  }

  public class MasterDetailConditions
  {
    private List<MasterDetailCondition> FList;

    public MasterDetailConditions()
    {
      FList = new List<MasterDetailCondition>();  
    }

    public MasterDetailCondition AddCondition()
    {
      MasterDetailCondition Temp = new MasterDetailCondition();
      FList.Add(Temp);
      return Temp;
    }

    public int Count
    {
      get { return FList.Count; }
    }

    public MasterDetailCondition this[int index]
    {
      get { return (FList[index]); }
      set { FList[index] = value; }
    }
  }

  public class TDetailQueryExecutionPlan
  {
    Dictionary<string,string>  FRefValueFieldsAliases;
    

    public TDetailQueryExecutionPlan()
    {
      FRefValueFieldsAliases = new Dictionary<string, string>();
    }

    public void AddRefValueFieldAlias(string aRefField, string aAlias)
    {
      FRefValueFieldsAliases.Add(aRefField, aAlias);
    }

    public string GetRefValueFieldAlias(string aRefField)
    {
      string TempString;
      FRefValueFieldsAliases.TryGetValue(aRefField, out TempString);
      return TempString;
    } 
    
    public void Clear()
    {
      FRefValueFieldsAliases.Clear();
    }   
  }

  public class DetailQuery
  {
    private string FSQLString;
    private MasterDetailConditions FConditions;
    private TDetailQueryExecutionPlan FLastExecutionPlan;
    public static string PlaceHolder = "?CONDITIONS?";
  
    public DetailQuery()
    {
      FConditions = new MasterDetailConditions();
      FLastExecutionPlan = new TDetailQueryExecutionPlan();
    }

    public string SQLString
    {
      get { return FSQLString; }
      set { FSQLString = value; }
    }

    public MasterDetailConditions Conditions
    {
      get { return FConditions; }
    }

    public string BuildSQLQuery (ImDataProvider aSourceDataProvider, bool aPerformDataAnalysis)
    {
      string TempCondition = "";
      string ORString = "";
      string ANDString = "";
      char CharAlias = 'A';
      string CurrentAliasPrefix = "";
      //int FieldConditionsCount = 0;
      StringBuilder TempStringBuilder = new StringBuilder();
      List<MasterDetailCondition> FieldConditions = new List<MasterDetailCondition>();


      if (aPerformDataAnalysis)
      {
        FLastExecutionPlan.Clear();
      }

      if (FConditions.Count > 0)
      {        
        for (int i = 0; i < FConditions.Count; i++)
        {
          TempStringBuilder.Append(ANDString);
          TempStringBuilder.Append('(');
          TempStringBuilder.Append(FConditions[i].Column);
          TempStringBuilder.Append('=');
          if (FConditions[i].Kind == DetailConditionKind.ckField)
          {
            FieldConditions.Add(FConditions[i]);
            TempStringBuilder.Append('@');
            FLastExecutionPlan.AddRefValueFieldAlias(FConditions[i].RefValue, (CurrentAliasPrefix + CharAlias));            
            TempStringBuilder.Append(CurrentAliasPrefix);
            TempStringBuilder.Append(CharAlias);
            TempStringBuilder.Append("^!^");
            if (CharAlias.Equals('Z'))
            {
              CurrentAliasPrefix = CurrentAliasPrefix + "A";
              CharAlias = 'A';
            }
            else
            {
              CharAlias++;
            }
          }
          else
          {
            TempStringBuilder.Append(FConditions[i].RefValue);
          }
          
          TempStringBuilder.Append(')');
          if (i == 0)
          {
            ANDString = "AND";
          }
        }        
      
        TempCondition = TempStringBuilder.ToString();
        TempStringBuilder.Clear();
        TempStringBuilder.Append('(');

        if (aPerformDataAnalysis)
        {
          
          Object [] LastValues = new Object [FieldConditions.Count];

          for (int i = 0; i < FieldConditions.Count -1; i++)
          {
            LastValues[i] = null;
          }

          bool DifferentValues = false;
          Object TempValue;

          aSourceDataProvider.First();
          int k = 0;          
          while (! aSourceDataProvider.Eof)
          {
            DifferentValues = false;            
            for (int i = 0; i < FieldConditions.Count; i++)
            {
              TempValue = aSourceDataProvider.GetValue(FieldConditions[i].RefValue);
              if (((TempValue == null) ^ (LastValues[i] == null)) || (! LastValues[i].Equals (TempValue)))
              {
                DifferentValues = true;
                LastValues[i] = TempValue;
              }             
            }

            if (DifferentValues)
            {
              TempStringBuilder.Append(ORString);
              TempStringBuilder.Append(TempCondition.Replace("^!^", k.ToString()));
              if (k == 0)
              {
                ORString = "OR";        
              }
            }
            
            k++;
            aSourceDataProvider.Next();
          }
        }
        else
        {
          for (int k = 0; k < aSourceDataProvider.Count; k++)
          {
            TempStringBuilder.Append(ORString);
            TempStringBuilder.Append(TempCondition.Replace("^!^", k.ToString()));
            if (k == 0)
            {
              ORString = "OR";        
            }
          }
        }
        TempStringBuilder.Append(')');
        return FSQLString.Replace(DetailQuery.PlaceHolder, TempStringBuilder.ToString());
      }
      else
      {
        return FSQLString.Replace(DetailQuery.PlaceHolder, "(1=0)");
      }
    }

  }
}

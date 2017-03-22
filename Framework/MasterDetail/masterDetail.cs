using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Mammola.Huramaki.MasterDetail
{
  public class TMasterQuery
  {
    private string FSQLString;

    public string SQLString
    {
      get {return FSQLString;}
      set {FSQLString = value;}
    }

    public void CopyFrom (TMasterQuery aSource)
    {
      this.FSQLString = aSource.FSQLString;
    }
  }

  public enum TDetailConditionKind
  {
    ckField,
    ckConstant
  }

  public class TMasterDetailCondition
  {
    private string FRefValue;
    private string FColumn;
    private TDetailConditionKind FKind;
    private Type FDataType;

    public TMasterDetailCondition()
    {
      FRefValue = null;
      FColumn = null;
      FKind = TDetailConditionKind.ckField;
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
    public TDetailConditionKind Kind
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

  public class TMasterDetailConditions
  {
    private List<TMasterDetailCondition> FList;

    public TMasterDetailConditions()
    {
      FList = new List<TMasterDetailCondition>();  
    }

    public TMasterDetailCondition AddCondition()
    {
      TMasterDetailCondition Temp = new TMasterDetailCondition();
      FList.Add(Temp);
      return Temp;
    }

    public int Count
    {
      get { return FList.Count; }
    }

    public TMasterDetailCondition this[int index]
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

  public class TDetailQuery
  {
    private string FSQLString;
    private TMasterDetailConditions FConditions;
    private TDetailQueryExecutionPlan FLastExecutionPlan;
    public static string PlaceHolder = "?CONDITIONS?";
  
    public TDetailQuery()
    {
      FConditions = new TMasterDetailConditions();
      FLastExecutionPlan = new TDetailQueryExecutionPlan();
    }

    public string SQLString
    {
      get { return FSQLString; }
      set { FSQLString = value; }
    }

    public TMasterDetailConditions Conditions
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
      List<TMasterDetailCondition> FieldConditions = new List<TMasterDetailCondition>();


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
          if (FConditions[i].Kind == TDetailConditionKind.ckField)
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
        return FSQLString.Replace(TDetailQuery.PlaceHolder, TempStringBuilder.ToString());
      }
      else
      {
        return FSQLString.Replace(TDetailQuery.PlaceHolder, "(1=0)");
      }
    }

  }
}

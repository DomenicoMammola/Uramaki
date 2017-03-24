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
using System.Data;
using System.Collections.Generic;

namespace Mammola.Uramaki.Data
{
  public class mDataField
  {
    public string Name;
    public Type DataType;
    public int Size;

    void AssignFromDataColumn (DataColumn aColumn)
    {
      this.Name = aColumn.ColumnName;
      this.DataType = aColumn.DataType;
      this.Size = aColumn.MaxLength;
    }
  }

  public class mDataFields
  {
    private List<mDataField> FList;
    
    public mDataFields()
    {
      FList = new List<mDataField>();
    } 

    public int Count 
    {
      get {return FList.Count;}
    }
  }

  public interface ImDataProvider
  {
    int Count {get;}
    void First();
    Object GetValue(string aFieldName);
    void Next();
    bool Eof {get;}
  }

  public interface ImVariablesProvider
  {
    int Levels {get;}
    List<string> GetKeysForLevel(int aLevel);
  }

  // http://www.csvreader.com/posts/datatable_index.php
  public class DataTableIndex
	{
		private DataView lookupTable = null;

		public DataTableIndex(DataTable data, params DataColumn[] index)
		{
			bool first = true;
			string sort = "";

			// create a comma separated list of sort columns
			foreach (DataColumn column in index)
			{
				if (!first)
				{
					sort += ",";
				}

				first = false;

				// use brackets to handle column names with spaces, etc
				sort += "[" + column.ColumnName + "]";
			}

			// use a DataView because it internally creates an index to cover the sort criteria
			lookupTable = new DataView(
				data, 
				null, 
				sort, 
				DataViewRowState.CurrentRows);
		}

		/// <summary>
		///		Searches for DataRow's using an indexed lookup.
		/// </summary>
		/// <param name="value">
		///		Value order must directly match the order of the columns passed in to the constructor.
		/// </param>
		/// <returns>
		///		The matching DataRow's.
		/// </returns>
		public DataRow[] Find(params object[] value)
		{
			DataRowView[] found = lookupTable.FindRows(value);

			DataRow[] matchingRows = new DataRow[found.Length];

			for (int i = 0; i < found.Length; i++)
			{
				matchingRows[i] = found[i].Row;
			}

			return matchingRows;
		}
	}

}
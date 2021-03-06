// ConstraintCollection.cs - NUnit Test Cases for testing the ConstraintCollection 
//	class.
//	
// Authors:
//   Franklin Wise (gracenote@earthlink.net)
//   Martin Willemoes Hansen (mwh@sysrq.dk)
//   Roopa Wilson (rowilson@novell.com)	
//
// (C) Franklin Wise
// (C) 2003 Martin Willemoes Hansen

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using NUnit.Framework;
using System;
using System.Data;

namespace MonoTests.System.Data
{
	[TestFixture]
	public class ConstraintCollectionTest : Assertion {
		private DataTable _table;
		private DataTable _table2;
		private Constraint _constraint1;
		private Constraint _constraint2;

		[SetUp]
		public void GetReady() 
		{
			//Setup DataTable
			_table = new DataTable("TestTable");
			_table.Columns.Add("Col1",typeof(int));
			_table.Columns.Add("Col2",typeof(int));
			_table.Columns.Add("Col3",typeof(int));

			_table2 = new DataTable("TestTable");
			_table2.Columns.Add("Col1",typeof(int));
			_table2.Columns.Add("Col2",typeof(int));

			//Use UniqueConstraint to test Constraint Base Class
			_constraint1 = new UniqueConstraint(_table.Columns[0],false); 
			_constraint2 = new UniqueConstraint(_table.Columns[1],false); 

			// not sure why this is needed since a new _table was just created
			// for us, but this Clear() keeps the tests from throwing
			// an exception when the Add() is called.
			_table.Constraints.Clear();
		}

		[Test]
		public void Add()
		{
			ConstraintCollection col = _table.Constraints;
			col.Add(_constraint1);
			col.Add(_constraint2);

			AssertEquals("Count doesn't equal added.",2, col.Count);
		}

		[Test]
		public void AddExceptions()
		{
			ConstraintCollection col = _table.Constraints;
			
			//null
			try 
			{
				col.Add(null);
				Fail("B1: Failed to throw ArgumentNullException.");
			}
			catch (ArgumentNullException) {}
			catch (AssertionException exc) {throw exc;}
			catch 
			{
				Fail("A1: Wrong exception type");
			}

			//duplicate name
			try 
			{
				_constraint1.ConstraintName = "Dog";
				_constraint2.ConstraintName = "dog"; //case insensitive
				col.Add(_constraint1);
				col.Add(_constraint2);
#if NET_1_1
#else
				Fail("Failed to throw Duplicate name exception.");
#endif
				col.Remove (_constraint2); // only for !1.0
				col.Remove (_constraint1);
			}
#if ! NET_1_1
			catch (DuplicateNameException) {
			}
#endif
			catch (AssertionException exc) {throw exc;}
/* Don't use such catch. They cover our eyes from the exact exception location.
			catch (Exception exc)
			{
				Fail("A2: Wrong exception type. " + exc.ToString());
			}
*/
			//Constraint Already exists
			try 
			{
				col.Add(_constraint1);
#if NET_1_1
#else
				Fail("B2: Failed to throw ArgumentException.");
#endif
				col.Remove (_constraint1);
			}
			catch (ArgumentException) {
#if NET_1_1
#else
				throw;
#endif
			}
			catch (AssertionException exc) {throw exc;}
			catch 
			{
				Fail("A3: Wrong exception type");
			}
		}

		[Test]
		public void Indexer()
		{
			Constraint c1 = new UniqueConstraint(_table.Columns[0]);
			Constraint c2 = new UniqueConstraint(_table.Columns[1]);

			c1.ConstraintName = "first";
			c2.ConstraintName = "second";


			_table.Constraints.Add(c1);
			_table.Constraints.Add(c2);

			AssertSame("A1", c1, _table.Constraints[0]); 
			AssertSame("A2", c2, _table.Constraints[1]);

			AssertSame("A3", c1, _table.Constraints["first"]);
			AssertSame("A4", c2, _table.Constraints["sEcond"]); //case insensitive

		}

		[Test]
		public void IndexOf()
		{
			Constraint c1 = new UniqueConstraint(_table.Columns[0]);
			Constraint c2 = new UniqueConstraint(_table.Columns[1]);

			c1.ConstraintName = "first";
			c2.ConstraintName = "second";

			_table.Constraints.Add(c1);
			_table.Constraints.Add(c2);

			AssertEquals("A1", 0, _table.Constraints.IndexOf(c1));
			AssertEquals("A2", 1, _table.Constraints.IndexOf(c2));
			AssertEquals("A3", 0, _table.Constraints.IndexOf("first"));
			AssertEquals("A4", 1, _table.Constraints.IndexOf("second"));
		}

		[Test]
		public void Contains()
		{
			Constraint c1 = new UniqueConstraint(_table.Columns[0]);
			Constraint c2 = new UniqueConstraint(_table.Columns[1]);

			c1.ConstraintName = "first";
			c2.ConstraintName = "second";

			_table.Constraints.Add(c1);

			Assert("A1", _table.Constraints.Contains(c1.ConstraintName)); //true
			Assert("A2", _table.Constraints.Contains(c2.ConstraintName) == false); //doesn't contain
		}

		[Test]
		public void IndexerFailures()
		{
			_table.Constraints.Add(new UniqueConstraint(_table.Columns[0]));

			//This doesn't throw
			AssertNull(_table.Constraints["notInCollection"]);
			
			//Index too high
			try 
			{
				Constraint c = _table.Constraints[_table.Constraints.Count];
				Fail("B1: Failed to throw IndexOutOfRangeException.");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw exc;}
			catch 
			{
				Fail("A1: Wrong exception type");
			}

			//Index too low
			try 
			{
				Constraint c = _table.Constraints[-1];
				Fail("B2: Failed to throw IndexOutOfRangeException.");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw exc;}
			catch 
			{
				Fail("A2: Wrong exception type");
			}	

		}

		[Test]
		public void AddFkException1()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(_table);
			_table2.TableName = "TestTable2";
			ds.Tables.Add(_table2);

			_table.Rows.Add(new object [] {1});
			_table.Rows.Add(new object [] {1});

			//FKC: can't create unique constraint because duplicate values already exist
			try
			{
				ForeignKeyConstraint fkc = new ForeignKeyConstraint( _table.Columns[0],
											_table2.Columns[0]);
				
				_table2.Constraints.Add(fkc);	//should throw			
				Fail("B1: Failed to throw ArgumentException.");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw exc;}
			catch (Exception exc)
			{
				Fail("A1: Wrong Exception type. " + exc.ToString());
			}


		}


		[Test]
		public void AddFkException2()
		{
			//Foreign key rules only work when the tables
			//are apart of the dataset
			DataSet ds = new DataSet();
			ds.Tables.Add(_table);
			_table2.TableName = "TestTable2";
			ds.Tables.Add(_table2);

			_table.Rows.Add(new object [] {1});
			
			// will need a matching parent value in 
			// _table
			_table2.Rows.Add(new object [] {3}); 
								

			//FKC: no matching parent value
			try
			{
				ForeignKeyConstraint fkc = new ForeignKeyConstraint( _table.Columns[0],
					_table2.Columns[0]);
				
				_table2.Constraints.Add(fkc);	//should throw			
				Fail("B1: Failed to throw ArgumentException.");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw exc;}
			catch (Exception exc)
			{
				Fail("A1: Wrong Exception type. " + exc.ToString());
			}


		}


		[Test]
		public void AddUniqueExceptions()
		{
			

			//UC: can't create unique constraint because duplicate values already exist
			try
			{
				_table.Rows.Add(new object [] {1});
				_table.Rows.Add(new object [] {1});
				UniqueConstraint uc = new UniqueConstraint( _table.Columns[0]);
				
				_table.Constraints.Add(uc);	//should throw			
				Fail("B1: Failed to throw ArgumentException.");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw exc;}
			catch (Exception exc)
			{
				Fail("A1: Wrong Exception type. " + exc.ToString());
			}
		}

		[Test]
                //Tests AddRange (), CanRemove (), RemoveAt (), Remove (), Exceptions of  Remove(), and Clear ()
                public void AddRemoveTest ()
                {
                        AddRange ();
//                      CanRemove (); This test is ignored
                        Remove ();
//                      RemoveAt (); This test is ignored

			// This test is expected to be failed, so don't reuse it.
//                        RemoveExceptions ();
                        _table.Constraints.Remove (_table.Constraints [0]);

			Clear ();
                }

		[Test]
		public void AddRange()
		{
			_constraint1.ConstraintName = "UK1";
                        _constraint2.ConstraintName = "UK12";
                                                                                                    
                        ForeignKeyConstraint _constraint3 = new ForeignKeyConstraint ("FK2", _table.Columns [0],
                                        _table2.Columns [0]);
                        UniqueConstraint _constraint4=new UniqueConstraint("UK2", _table2.Columns [1]);
                                                                                                    
                        // Add the constraints.
                        Constraint [] constraints = {_constraint1, _constraint2};
                        _table.Constraints.AddRange (constraints);
                                                                                                                                                                                                         
                        Constraint [] constraints1 = {_constraint3, _constraint4};
                        _table2.Constraints.AddRange (constraints1);
                                                                                                    
                        AssertEquals ("A1", "UK1", _table.Constraints [0].ConstraintName);
                        AssertEquals ("A2", "UK12", _table.Constraints [1].ConstraintName);
                                                                                                    
                        AssertEquals ("A3", "FK2", _table2.Constraints [0].ConstraintName);
                        AssertEquals ("A4", "UK2", _table2.Constraints [1].ConstraintName);

		}
		
		[Test]
		[Category ("NotDotNet")]
		// Even after EndInit(), MS.NET does not fill Table property
		// on UniqueConstraint.
		public void TestAddRange2()
                {
                        DataTable table = new DataTable ("Table");
                        DataColumn column1 = new DataColumn ("col1");
                        DataColumn column2 = new DataColumn ("col2");
                        DataColumn column3 = new DataColumn ("col3");
                        table.Columns.Add (column1);
                        table.Columns.Add (column2);
                        table.Columns.Add (column3);
                        string []columnNames = {"col1", "col2", "col3"};
                                                                                                    
                        Constraint []constraints = new Constraint[3];
                        constraints [0] = new UniqueConstraint ("Unique1",column1);
                        constraints [1] = new UniqueConstraint ("Unique2",column2);
                        constraints [2] = new UniqueConstraint ("Unique3", columnNames, true);
                                                                                                    
                        table.BeginInit();
                        //Console.WriteLine(table.InitStatus == DataTable.initStatus.BeginInit);
                        table.Constraints.AddRange (constraints);
                                                                                                    
                        //Check the table property of UniqueConstraint Object
                        try{
                                Assertion.AssertNull ("#01", constraints [2].Table);
                        }
                        catch (Exception e) {
                                Assertion.Assert ("#A02", "System.NullReferenceException".Equals (e.GetType().ToString()));
                        }

			table.EndInit();

			// After EndInit is called the constraints associated with most recent call to AddRange() must be
			// added to the ConstraintCollection
                        Assertion.Assert ("#A03", constraints [2].Table.ToString().Equals ("Table"));
                        Assertion.Assert ("#A04", table.Constraints.Contains ("Unique1"));
                        Assertion.Assert ("#A05", table.Constraints.Contains ("Unique2"));
                        Assertion.Assert ("#A06", table.Constraints.Contains ("Unique3"));

                }
        


		[Test]
		public void Clear()
		{
 			try {
                               _table.Constraints.Clear (); //Clear all constraints
                                AssertEquals ("A1", 0, _table.Constraints.Count); //No constraints should remain
                                _table2.Constraints.Clear ();
                                AssertEquals ("A2", 0, _table2.Constraints.Count);
                        }
                        catch (Exception e) {
                                Console.WriteLine (e);
                        }

		}

		[Test]
		[Ignore ("This never works on MS.NET (and it should not)")]
		public void CanRemove()
		{
			AssertEquals ("A1", false, _table.Constraints.CanRemove (_table.Constraints [0]));

		}

		[Test]
		public void CollectionChanged()
		{
		}
#if false
		//
		// If this fails on MS.NET and its supposed to fail, why do we have this as Ignore?
		//
		[Test]
		[Ignore ("MS.NET fails this test (and it should fail)")]
		public void RemoveAt()
		{
			 _table2.Constraints.RemoveAt (1); //Remove constraint and again add it
                         AssertEquals ("A1", 1, _table2.Constraints.Count);                                                  UniqueConstraint _constraint4  = new UniqueConstraint ("UK2", _table2.Columns [1]);
                         // Add the constraints.
                         Constraint [] constraints = {_constraint4};
                         _table.Constraints.AddRange (constraints);

		}
#endif

		//[Test]
		[Ignore ("MS.NET fails this test (and it should fail)")]
		public void Remove()
		{
			_table2.Constraints.Remove (_table2.Constraints [1]); //Remove constraint and again add it
                        AssertEquals ("A1", 1, _table2.Constraints.Count);                      
                        UniqueConstraint _constraint4 = new UniqueConstraint ("UK2", _table2.Columns [1]);                                                                               
                        // Add the constraints.
                        Constraint [] constraints = {_constraint4};
                        _table2.Constraints.AddRange (constraints);
		}

		[Test]
		public void RemoveExceptions()
		{
			try {
                                //Remove constraint that cannot be removed
                                _table.Constraints.Remove (_table.Constraints [0]);
				Fail ("A1");
			} catch (Exception e) {
				AssertEquals ("A2", typeof (IndexOutOfRangeException), e.GetType ());
                        }
                }

	}
	
}

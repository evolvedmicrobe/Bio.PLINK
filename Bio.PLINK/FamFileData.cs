// Code Copyright Nigel Delaney, 2013
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

namespace Bio.PLINK
{
	public class FamFileData : BasePlinkFile
	{
		List<PedLineInfo> data = new List<PedLineInfo> ();
		public List<Family> Families;

		public int TotalIndividuals { get { return data.Count; } }

		public FamFileData (string fName) : base (fName)
		{

			int lineNum = 0;
			foreach (string line in File.ReadAllLines (fName)) {
				PedLineInfo pi = new PedLineInfo (line, lineNum);
				data.Add (pi);
				lineNum++;
			}
			data.Sort ();
			data.TrimExcess ();

			Families = data.GroupBy (x => x.FamilyID).Select (x => new Family (x))
				.ToList ();
		}
	}
}


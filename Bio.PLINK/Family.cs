// Code Copyright Nigel Delaney, 2013
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Bio.PLINK
{
	[DebuggerDisplay ("Family={FamilyID}")]
	public class Family
	{
		public string FamilyID;
		public PedLineInfo Father;
		public PedLineInfo Mother;
		public PedLineInfo[] Children;
		public bool INCOMPLETE = false;

		public Family (IGrouping<string,PedLineInfo> data)
		{
			FamilyID = data.Key;
			var cData = data.ToList ();
			var fathers = data.Where (x => x.PaternalID != "0").Select (x => x.PaternalID).Distinct ().ToList ();
			var mothers = data.Where (x => x.MaternalID != "0").Select (x => x.MaternalID).Distinct ().ToList ();
			if (fathers.Count != 1 || mothers.Count != 1) {
				INCOMPLETE = true;
			} else {
				var fid = fathers [0];
				var mid = mothers [0];
				var father = data.Where (x => x.IndividualID == fid).ToList ();
				var mother = data.Where (x => x.IndividualID == mid).ToList ();
				if (mother.Count != 1 || father.Count != 1) {
					INCOMPLETE = true;
				} else {
					Mother = mother [0];
					Father = father [0];
					Children = data.Where (x => (x.IndividualID != fid && x.IndividualID != mid)).ToArray ();
				}
			}
		}
	}
}


// Code Copyright Nigel Delaney, 2013
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

namespace Bio.PLINK
{
	public class BimFileData : BasePlinkFile
	{
		public List<BIMVariant> Variants;

		public BimFileData (string bimFile) : base (bimFile)
		{

			Variants = new List<BIMVariant> ();
			int lineNum = 0;
			foreach (var s in File.ReadLines (bimFile)) {
				Variants.Add (new BIMVariant (s, lineNum));
				lineNum++;
			}
			Variants.TrimExcess ();
		
			Variants.Sort ();
		}

		/// <summary>
		/// Binary search to get the index in the file that corresponds to a particular SNP
		/// based on its string ID.  Does this by way of binary search, which for 1e6 SNPs
		/// should not be too many checks
		/// </summary>
		/// <returns>The index of variant.</returns>
		public int? GetIndexOfVariant (string variantName)
		{
			BIMVariant bi = new BIMVariant (variantName);
			var ind = Variants.BinarySearch (bi);
			if (ind < 0) {
				return null;
			} else { 
				return ind;
			}
		}
	}
}


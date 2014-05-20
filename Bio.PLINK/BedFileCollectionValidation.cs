// Code Copyright Nigel Delaney, 2013
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Bio.PLINK
{
	/// <summary>
	/// A collection of classes designed to verify that BAM Files are what they say they are.
	/// Written for the epistasis project specifically.
	/// </summary>
	public class BedFileCollectionValidation
	{
		static long ConvertPositionToLong (int Chr, int Pos)
		{
			return (Chr << 32) | Pos;
		}

		Dictionary<long,BIMVariant> variantsAlreadySeenByPos;
		Dictionary<string,BIMVariant> variantsAlreadySeenByName;

		public BedFileCollectionValidation (IEnumerable<string> bedFilePrefixes)
		{
			variantsAlreadySeenByPos = new Dictionary<long, BIMVariant> ();
			variantsAlreadySeenByName = new Dictionary<string, BIMVariant> ();
			foreach (var bf in bedFilePrefixes) {
				var curRead = new BedReader (bf);
				foreach (var v in curRead.VariantInformation.Variants) {
					long pos = ConvertPositionToLong (v.Chromosome, v.BPposition);
					var id = v.VariantID;
				}
			}

		}
	}
}


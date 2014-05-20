// Code Copyright Nigel Delaney, 2013
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

namespace Bio.PLINK
{
	public struct BIMVariant : IComparable<BIMVariant>
	{
		public readonly byte Chromosome;
		public readonly int BPposition;
		public readonly string VariantID;
		//int cMorgans;//not using
		/// <summary>
		/// The first variant in the file, e.g. A, C,G,T
		/// </summary>
		public readonly byte FirstVariant;
		/// <summary>
		/// The second variant in the file, e.g. A,C,G,T
		/// </summary>
		public readonly byte SecondVariant;
		/// <summary>
		/// The position in the bim file (line number)
		/// </summary>
		public readonly int PositionNumber;

		public BIMVariant (string line, int positionNumber)
		{
			var sp = line.Split (Constants.Delimiters);
			if (sp.Length != 6) {
				throw new Exception ("Bad Error, line length not 5:" + line);
			}
			Chromosome = (byte)Convert.ToInt16 (sp [0]);
			VariantID = sp [1].ToLowerInvariant ();
			//cMorgans=sp[2];
			BPposition = Convert.ToInt32 (sp [3]);
			if (sp [4].Length != 1 || sp [5].Length != 1) {
				throw new ArgumentException ("Variant lengths off:\n" + line);
			}
			FirstVariant = (byte)sp [4] [0];
			SecondVariant = (byte)sp [5] [0];
			PositionNumber = positionNumber;
		}

		public BIMVariant (string variantID) : this ()
		{
			this.VariantID = variantID;
			BPposition = 0;
			Chromosome = 0;
			FirstVariant = 0;
			SecondVariant = 0;
			PositionNumber = 0;


		}

		public int CompareTo (BIMVariant other)
		{
			if (!String.IsNullOrEmpty (VariantID)
			    && !String.IsNullOrEmpty (other.VariantID)) {
				return String.CompareOrdinal (VariantID, other.VariantID);
			} else {
				int curComp = Chromosome.CompareTo (other.Chromosome);
				if (curComp == 0) {
					curComp = BPposition.CompareTo (other.BPposition);
					if (curComp == 0) {
						if (FirstVariant == other.FirstVariant && SecondVariant == other.SecondVariant) {
							throw new Exception ("Same name but different Values!!!");
						}
					}
				}
				return curComp;
			}
		}
	}
}

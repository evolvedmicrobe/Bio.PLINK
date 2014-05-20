// Code Copyright Nigel Delaney, 2013
using System;

namespace Bio.PLINK
{
	public class PedLineInfo : IComparable<PedLineInfo>
	{
		public string FamilyID { get; private set; }

		public string IndividualID{ get; private set; }

		public string PaternalID { get; private set; }

		public string MaternalID { get; private set; }

		public Gender Sex { get; private set; }

		public string Phenotype { get; private set; }

		public int LineNumber { get; private set; }

		public PedLineInfo (string line, int lineNumber)
		{
			var sp = line.Split (Constants.Delimiters, 6);
			FamilyID = sp [0];
			IndividualID = sp [1];
			PaternalID = sp [2];
			MaternalID = sp [3];
			Phenotype = sp [5];
			var gender = sp [4];
			if (gender == "1") {
				Sex = Gender.Male;
			} else if (gender == "2") {
				Sex = Gender.Female;
			} else {
				Sex = Gender.Other;
			}
			LineNumber = lineNumber;
		}

		public int CompareTo (PedLineInfo other)
		{
			int toRet = FamilyID.CompareTo (other.FamilyID);
			if (toRet != 0) {
				return toRet;
			} else {
				return IndividualID.CompareTo (other.IndividualID);
			}
		}
	}

	public enum Gender : byte
	{
		Male = 1,
		Female = 2,
		Other = 0
	}
}


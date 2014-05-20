// Code Copyright Nigel Delaney, 2013
using System;

namespace Bio.PLINK
{
	public abstract class BasePlinkFile
	{
		public BasePlinkFile (string fName)
		{
			ValidateFile (fName);
		}

		public static void ValidateFile (string fName)
		{
		
			if (fName == null) {
				throw new NullReferenceException ("fName");
			} else if (!System.IO.File.Exists (fName)) {
				throw new System.IO.FileNotFoundException (fName);
			}

		}
	}
}


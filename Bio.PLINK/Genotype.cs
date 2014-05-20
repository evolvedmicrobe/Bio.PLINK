// Code Copyright Nigel Delaney, 2013
using System;

namespace Bio.PLINK
{
	/// <summary>
	/// From: http://pngu.mgh.harvard.edu/~purcell/plink/binary.shtml
	/// 
	/// Genotypes are encoded as:
	/// 
	///  00  Homozygote "1"/"1"
	///  01  Heterozygote
	///  11  Homozygote "2"/"2"
	///  10  Missing genotype
	/// 
	/// Note that because PLINK used bitfields in C++ the ordering is totally reversed, but I will be
	/// unpack them as such, to avoid conversions
	/// </summary>
	public enum Genotype : byte
	{
		HomozygoteAlt = 0,
		MissingGenotype = 1,
		Heterozygote = 2,
		HomozygoteRef = 3
	}
}


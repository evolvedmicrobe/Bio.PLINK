// Code Copyright Nigel Delaney, 2013
using System;
using System.Linq;

namespace Bio.PLINK
{
	public class VariantData
	{
		public readonly Genotype[] Genotypes;
		public BIMVariant Variant;
		const double BAD_FLAG = -10;
		double pfrequency = BAD_FLAG;
		double pMissing = BAD_FLAG;

		public double MajorAlleleFrequency {
			get { 
				if (pfrequency == BAD_FLAG) {
					calculateMetrics ();
				}
				return pfrequency;
			}
		}

		public double PercentMissing {
			get {
				if (pMissing == BAD_FLAG) {
					calculateMetrics ();
				}
				return pMissing;
			}
		}

		private void calculateMetrics ()
		{
			double alleleCount = 0;
			double totalMissing = 0;
			for (int i = 0; i < this.Genotypes.Length; i++) {
				var curGeno = Genotypes [i];
				if (curGeno == Genotype.HomozygoteRef) {
					alleleCount += 1;
				} else if (curGeno == Genotype.Heterozygote) {
					alleleCount += .5;
				} else if (curGeno == Genotype.MissingGenotype) {
					totalMissing += 1;
				}
			}
			double totalCounts = Genotypes.Length - totalMissing;
			pfrequency = alleleCount / totalCounts;
			pfrequency = Math.Min (pfrequency, 1 - pfrequency);
			pMissing = totalMissing / (double)Genotypes.Length;
		}

		public VariantData (BIMVariant variant, Genotype[] data)
		{
			this.Genotypes = data;
			this.Variant = variant;
		}
	}
}


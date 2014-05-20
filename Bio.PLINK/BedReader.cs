using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace Bio.PLINK
{
	public class BedReader
	{
		/// <summary>
		/// The name of the binary PED File
		/// </summary>
		public string BedFileName{ get; set; }

		/// <summary>
		/// The Bim File Name
		/// </summary>
		public string BimFileName{ get; set; }

		/// <summary>
		/// The Fam File that matches
		/// </summary>
		/// <value>The name of the fam file.</value>
		public string FamFileName{ get; set; }

		public int NumIndividuals {
			get {
				return FamilyData.TotalIndividuals;
			}
		}

		public FamFileData FamilyData;
		public BimFileData VariantInformation;
		/// <summary>
		/// A listing of cached genotypes to make the decompression of the 
		/// bits faster, see the Genotypes for an explanation of each
		/// </summary>
		Genotype[][] cachedUnPackedGenotypes;
		/// <summary>
		/// The number of bytes in each row of a BED file, determined by
		/// knowing there are 4 genotypes per byte and space at the end if left over
		/// </summary>
		int bytesPerVariant;
		/// <summary>
		/// The raw data from the file.
		/// </summary>
		byte[] fileData;
		/// <summary>
		/// The magic byte1 for the bed file.
		/// </summary>
		const byte magicByte1 = 0x6c;
		/// <summary>
		/// The magic byte2 for the bed file
		/// </summary>
		const byte magicByte2 = 0x1B;
		/// <summary>
		/// To account for the two magic bytes and the
		/// snp major byte
		/// </summary>
		const int HEADER_OFFSET = 3;
		/// <summary>
		/// Indicates if the BED File is in SNP Major format
		/// (i.e. list all individuals for first SNP, all individuals for second SNP, etc)
		/// </summary>
		public readonly bool IsSNPMajor;
		int additionalByteAtEnd;
		VariantCache cache = new VariantCache ();

		public BedReader (string fnamePrefix)
		{
			//load files
			FamFileName = fnamePrefix + ".fam";
			BimFileName = fnamePrefix + ".bim";
			BedFileName = fnamePrefix + ".bed";
			string[] names = new[] { FamFileName, BimFileName, BedFileName };
			foreach (var n in names) {
				if (!File.Exists (n)) {
					throw new FileNotFoundException (n);
				}
			}
			FamilyData = new FamFileData (FamFileName);
			VariantInformation = new BimFileData (BimFileName);

			//load up the unpacking cache
			makeCachedValues ();

			//calculate space required for return values
			int numVariants = VariantInformation.Variants.Count;
			additionalByteAtEnd = NumIndividuals % Constants.VARIANTS_PER_BYTE;
			int addAdditional = additionalByteAtEnd > 0 ? 1 : 0;
			bytesPerVariant = NumIndividuals / Constants.VARIANTS_PER_BYTE + addAdditional;

			//Load all data 
			fileData = File.ReadAllBytes (BedFileName);
			//verify data and format
			var byte1 = fileData [0];
			var byte2 = fileData [1];
			if (byte1 != magicByte1 || byte2 != magicByte2) {
				throw new Exception ("Magic bytes not found in bed file! : " + BedFileName);
			}
			var issnpMajor = fileData [2];
			if (issnpMajor > 1) {
				throw new FormatException ("Third byte in Bed File was not 0 or 1");
			}
			if (issnpMajor != (byte)1) {
				throw new NotImplementedException ("This library only parses SNP Major items so far");
			}
			IsSNPMajor = issnpMajor != 0;

		}

		public unsafe VariantData GetGenotypeDataForSNP (string variantID)
		{

			VariantData toRet;
			bool isCached = cache.TryGetValue (variantID, out toRet);
			if (isCached) {
				return toRet;
			}

			var ind = VariantInformation.GetIndexOfVariant (variantID);
			if (ind.HasValue) {
				var index = ind.Value;
				var variantInfo = VariantInformation.Variants [index];
				int startIndex = HEADER_OFFSET + (bytesPerVariant * variantInfo.PositionNumber);
				Genotype[] toReturn = new Genotype[NumIndividuals];
				int copyToIndex = 0;
				//now to unpack the values, grab the packed byte
				//then the cached unpacked value, and then copy the values over
				fixed(byte* pData=fileData) {
					byte* pCurVal = pData + startIndex;
					int endVal = bytesPerVariant;
					if (additionalByteAtEnd > 0) {
						endVal--;
					}
					for (int i = 0; i < endVal; i++) {
						byte curValue = *pCurVal;
						Genotype[] cachedUnpacked = cachedUnPackedGenotypes [Convert.ToInt32 (curValue)];
						Array.Copy (cachedUnpacked, 0, toReturn, copyToIndex, Constants.VARIANTS_PER_BYTE);
						copyToIndex += Constants.VARIANTS_PER_BYTE;
						pCurVal++;
					}
					//copy a portion of last byte if needed 
					if (additionalByteAtEnd > 0) {
						byte curValue = *pCurVal;
						Genotype[] cachedUnpacked = cachedUnPackedGenotypes [Convert.ToInt32 (curValue)];
						Array.Copy (cachedUnpacked, 0, toReturn, copyToIndex, additionalByteAtEnd);
					}
				}
				toRet = new VariantData (variantInfo, toReturn);
				cache.AddToCache (variantID, toRet);
				return toRet;
			}
			return null;
		}

		/// <summary>
		/// Make all the cached values by enumerating 
		/// 1-255 and unpacking each time
		/// </summary>
		private void makeCachedValues ()
		{
			//should be plenty fast so no need for faster
			cachedUnPackedGenotypes = new Genotype[256][];
			for (int i = 0; i < cachedUnPackedGenotypes.Length; i++) {
				int j = i;
				var cur = new Genotype[4];
				for (int k = 0; k < 4; k++) {
					byte curval = (byte)(j & 3);
					cur [k] = (Genotype)curval;
					j = j >> 2;
				}
				cachedUnPackedGenotypes [i] = cur;
			}
		}
	}
}

// Code Copyright Nigel Delaney, 2013
using System;
using System.Collections.Generic;

namespace Bio.PLINK
{
	/// <summary>
	/// A class that holds variant data in a cache.
	/// </summary>
	public class VariantCache
	{
		const int MaxItems = 1;
		Dictionary<string,VariantData> cache;
		List<string> orderOfItems;

		public VariantCache ()
		{
			orderOfItems = new List<string> (MaxItems);
			cache = new Dictionary<string, VariantData> (MaxItems);
		}

		public bool TryGetValue (string key, out VariantData data)
		{
			bool hasValue;
			lock (this) {
				hasValue = cache.TryGetValue (key, out data);
				int curCount = orderOfItems.Count;
				if (hasValue && orderOfItems [curCount - 1] != key) {
					lock (this) {
						orderOfItems.Remove (key);
						orderOfItems.Add (key);
					}
				}
			}
			return hasValue;
		}

		public void AddToCache (string key, VariantData data)
		{
			lock (this) {
				if (!cache.ContainsKey (key)) {
					if (orderOfItems.Count == MaxItems && orderOfItems.Count > 0) {
						var toR = orderOfItems [0];
						cache.Remove (toR);
						orderOfItems.Remove (toR);
					}
					cache [key] = data;
					orderOfItems.Add (key);
				}
			}
		}
	}
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace CraftSynth.BuildingBlocks.Common
{
	public static class ExtenderClass
	{
		#region Generics

		public static T2? NullOr<T, T2>(this T obj, T2 nonNullCase) where T2 : struct
		{
			if (obj == null)
			{
				return null;
			}
			else
			{
				return nonNullCase;
			}
		}

		public static T2? NullOr<T, T2>(this T obj, T2? nonNullCase) where T2 : struct 
		{
			if (obj == null)
			{
				return null;
			}
			else
			{
				return nonNullCase;
			}
		} 

		public static bool IsNull<T>(this T? nullableObject) where T : struct
		{
			return !nullableObject.HasValue;
		}

		/// <summary>
		/// Gets property name from expression without magic strings. 
		/// Examle: ExtenderClass.GetPropertyName(()=>Model.AdvertisementImage)  
		/// "Extended" expressions such model.Propery.PropertyA are not supported.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string GetPropertyName<T>(System.Linq.Expressions.Expression<Func<T>> expression)
		{
			var body = expression.Body as System.Linq.Expressions.MemberExpression;
			if (body == null)
			{
				throw new ArgumentException("'expression' should be a member expression");
			}

			return body.Member.Name;
		}

		public static bool SequenceEqual<T>(this IEnumerable<T> s1, IEnumerable<T> s2)
		{
			bool equal;
			if (s1 == null && s2 == null)
			{
				equal = true;
			}
			else if (s1 == null || s2 == null)
			{
				equal = false;
			}
			else if(s1.LongCount()!=s2.LongCount())
			{
				equal = false;
			}
			else
			{
				equal = true;
				 System.Collections.IEnumerator en = s1.GetEnumerator();
				 System.Collections.IEnumerator en2 = s2.GetEnumerator();
				while (en.MoveNext())
				{
					en2.MoveNext();
					if (en.Current != null && en.Current != null)
					{
						if (
							(en.Current != null && en2.Current == null)
							|| (en.Current == null && en2.Current != null)
							|| (!en.Current.Equals(en2.Current))
							)
						{
							equal = false;
							break;
						}
					}
				}
			}
			return equal;
		}
		#endregion

		#region Enum
		/// <summary>
		/// Gets Description from enum value ie [Description("MasterCard")] returns MasterCard
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string Description(this Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(
					typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}
		#endregion

		#region String
		public static string ToNonNullString<T>(this T? nullableObject) where T : struct
		{
			return ToNonNullString<T>(nullableObject, string.Empty);
		}

		public static string ToNonNullString<T>(this T? nullableObject, string nullCaseString) where T : struct
		{
			if (nullableObject.HasValue)
			{
				return nullableObject.Value.ToString();
			}
			else
			{
				return nullCaseString;
			}
		}

		public static string ToNonNullString(this DateTime? nullableObject)
		{
			return ToNonNullString(nullableObject, string.Empty, null);
		}

		public static string ToNonNullString(this DateTime? nullableObject, string nullCaseString)
		{
			return ToNonNullString(nullableObject, nullCaseString, null);
		}

		public static string ToNonNullString(this DateTime? nullableObject, string nullCaseString, string format)
		{
			if (nullableObject.HasValue)
			{
				if (format == null)
				{
					return nullableObject.Value.ToString();
				}
				else
				{
					return nullableObject.Value.ToString(format);
				}
			}
			else
			{
				return nullCaseString;
			}
		}

		public static string ToNonNullString(this string nullableObject)
		{
			return ToNonNullString(nullableObject, string.Empty);
		}

		public static string ToNonNullString(this string nullableObject, string nullCaseString)
		{
			if (nullableObject != null)
			{
				return nullableObject;
			}
			else
			{
				return nullCaseString;
			}
		}

		public static string ToNonNullNonEmptyString(this string nullableObject, string nullOrEmptyCaseString)
		{
			if (nullableObject != null && nullableObject.Trim().Length>0)
			{
				return nullableObject;
			}
			else
			{
				return nullOrEmptyCaseString;
			}
		}

		public static bool IsNullOrWhiteSpace(this string nullableObject)
		{
			return string.IsNullOrEmpty(nullableObject) || string.IsNullOrEmpty(nullableObject.Trim());
		}

		public static bool IsNOTNullOrWhiteSpace(this string nullableObject)
		{
			return nullableObject.ToNonNullString().Trim().Length>0;
		}

		public static string EnclosedWithPercentSign(this string nullableObject)
		{
			string result = nullableObject;

			if (nullableObject.IsNOTNullOrWhiteSpace())
			{
				if (!nullableObject.StartsWith("%"))
				{
					result = "%" + result;
				}

				if (!nullableObject.EndsWith("%"))
				{
					result = result + "%";
				}
			}

			return result;
		}

		public static string AppendIfNotNullOrWhiteSpace(this string nullableObject, string stringToAppend)
		{
			string result = nullableObject;
			if (nullableObject.IsNOTNullOrWhiteSpace())
			{
				result += result + stringToAppend;
			}
			return result;
		}

		/// <summary>
		/// Returns bytes of specified string. No encoding is used.
		/// PROS: No data loss as with encoding when char is illegal.
		/// CONS: This and ToString method must be both used on same machine - Other case not tested.
		/// Source: http://stackoverflow.com/questions/472906/net-string-to-byte-array-c-sharp
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ToBytes(this string s)
		{
			byte[] bytes = new byte[s.Length * sizeof(char)];
			System.Buffer.BlockCopy(s.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		/// <summary>
		/// Returns string build up with bytes - no encoding is used.
		/// PROS: No data loss as with encoding when char is illegal.
		/// CONS: This and ToString method must be both used on same machine - Other case not tested.
		/// Source: http://stackoverflow.com/questions/472906/net-string-to-byte-array-c-sharp
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToString(this byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		/// <summary>
		/// All except these chars are replaced:
		/// (space)!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
		/// </summary>
		/// <param name="s"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static string ReplaceNonUSAndNonPrintableChars(this string s, string r)
		{
			StringBuilder sb1 = new StringBuilder(s.Length);
			foreach (char ch in s)
			{
				if (0x20 <= ch && ch <= 0x7E)
				{
					sb1.Append(ch);
				}
				else
				{
					sb1.Append(r);
				}
			}
			return sb1.ToString();
		}
		
		/// <summary>
		/// All except these chars are replaced:
		/// A-Z,a-Z,0-9
		/// </summary>
		/// <param name="s"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static string ReplaceNonAlphaNumericCharacters(this string s, string r)
		{
			StringBuilder sb1 = new StringBuilder(s.Length);
			foreach (char ch in s)
			{
				if (Char.IsLetterOrDigit(ch))
				{
					sb1.Append(ch);
				}
				else
				{
					sb1.Append(r);
				}
			}
			return sb1.ToString();
		}	
		
		/// <summary>
		/// All except these chars are replaced:
		/// A-Z,a-Z
		/// </summary>
		/// <param name="s"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static string ReplaceNonAlphaCharacters(this string s, string r)
		{
			StringBuilder sb1 = new StringBuilder(s.Length);
			foreach (char ch in s)
			{
				if (Char.IsLetter(ch))
				{
					sb1.Append(ch);
				}
				else
				{
					sb1.Append(r);
				}
			}
			return sb1.ToString();
		}

		/// <summary>
		/// All chars found in Path.GetInvalidFileNameChars() or Path.GetInvalidPathChars() are replaced.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static string ReplaceInvalidFileSystemCharacters(this string s, string r)
		{
			var invalid = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

			foreach (char c in invalid)
			{
				s = s.Replace(c.ToString(), r);
			}

			return s;
		}	

		public static string RemoveRepeatedWords(this string s)
		{
			if (s == null)
			{
				//just return s;
			}
			else if (s.Length == 0)
			{
				//just return s;
			}
			else
			{
				var words = s.Split(' ').ToList();
				List<string> uniqueWordList = new List<string>();
				foreach (string word in words)
				{
					if (!uniqueWordList.Contains(word))
					{
						uniqueWordList.Add(word);
					}
				}
				s = uniqueWordList.ToSingleString(null, null, ' ');
				return s;
			}
			return s;
		}

		/// <summary>
		/// All except these chars are replaced with: &#(char code):
		/// (space)!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string AddHtmlReferenceToNonUSAndNonPrintableChars(this string s)
		{
			StringBuilder sb1 = new StringBuilder(s.Length);
			foreach (char ch in s)
			{
				if (0x20 <= ch && ch <= 0x7E)
				{
					sb1.Append(ch);
				}
				else
				{
					sb1.Append("&#" + (int)ch);
				}
			}
			return sb1.ToString();
		}

		/// <summary>
		/// Returns true if contains any character outside from this list:
		/// (space)!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool HasNonUSOrNonPrintableChars(this string s)
		{
			foreach (char ch in s)
			{
				if (0x20 <= ch && ch <= 0x7E)
				{
					//
				}
				else
				{
					return true;
				}
			}
			return false;
		}

		public static string PrependWithTimestamp(this string s, bool useUtc = false)
		{
			DateTime now = useUtc ? DateTime.UtcNow : DateTime.Now;
			s = string.Format("{0}.{1}.{2} {3}:{4}:{5} ({6}) {7}",
				now.Year,
				now.Month.ToString().PadLeft(2, '0'),
				now.Day.ToString().PadLeft(2, '0'),
				now.Hour.ToString().PadLeft(2, '0'),
				now.Minute.ToString().PadLeft(2, '0'),
				now.Second.ToString().PadLeft(2, '0'),
				useUtc ? "UTC" : "local",
				s);
			return s;
		}
		
	
		public static string FirstXChars(this string s, int count, string endingIfTrimmed)
		{
			string r = string.Empty;

			if (s.ToNonNullString().Length >= count)
			{

				r = s.ToNonNullString().Substring(0, count - endingIfTrimmed.ToNonNullString().Length).ToNonNullString() + endingIfTrimmed.ToNonNullString();
			}
			else
			{
				r = s.ToNonNullString();
			}

			return r;
		}

		/// <summary>
		/// Returns ending of string.
		/// If count is bigger than character count in string whole string is returned. 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="count"></param>
		/// <param name="startingPartIfTrimmed">Can be used as '...' to indicate that result was trimmed at the beginning.</param>
		/// <returns></returns>
		public static string LastXChars(this string s, int count, string startingPartIfTrimmed)
		{
			string r = s.ToNonNullString();
			startingPartIfTrimmed = startingPartIfTrimmed.ToNonNullString();

			if (r.Length >= count)
			{
				if (startingPartIfTrimmed.Length > count)
				{
					throw new ArgumentException("Length of startingPartIfTrimmed must be smaller than count.");
				}
				r = r.Substring(r.Length - count, count);
				r = r.Remove(0, startingPartIfTrimmed.Length);
				r = startingPartIfTrimmed + r;
			}

			return r;
		}

		/// <summary>
		/// Example: from "Today is lovely wether!" makes "Toda...her!"
		/// </summary>
		/// <param name="s"></param>
		/// <param name="maxLength"></param>
		/// <param name="middleReplacement"></param>
		/// <returns></returns>
		public static string Bubble(this string s, int maxLength, string middleReplacement)
		{
			if (maxLength < middleReplacement.Length + 2)
			{
				throw new ArgumentException("maxLength too small or middleReplacement too long");
			}
			string r;
			if (s.Length < maxLength)
			{
				r = s;
			}
			else
			{
				int reminder;
				int countOfCharsVisibleAtBeginning = Math.DivRem(maxLength - middleReplacement.Length, 2, out reminder);
				int countOfCharsVisibleAtTheEnd = countOfCharsVisibleAtBeginning + reminder;

				r = s.FirstXChars(countOfCharsVisibleAtBeginning, string.Empty);
				r = r + middleReplacement;
				r = r + s.LastXChars(countOfCharsVisibleAtTheEnd, string.Empty);
			}
			return r;
		}

		public static string GetSubstringBefore(this string s, string endMarker)
		{
			string r = null;

			if (s != null && !string.IsNullOrEmpty(endMarker))
			{
				int endMarkerIndex = s.IndexOf(endMarker);

				if (endMarkerIndex == 0)
				{
					r = string.Empty;
				}
				else if (endMarkerIndex > 0)
				{
					r = s.Substring(0, endMarkerIndex);
				}
			}

			return r;
		}

		public static string GetSubstringAfter(this string s, string startMarker)
		{
			string r = null;

			if (s != null && !string.IsNullOrEmpty(startMarker))
			{
				int startMarkerIndex = s.IndexOf(startMarker);
				int endMarkerIndex = s.Length;

				if (startMarkerIndex >= 0 && endMarkerIndex >= 0)
				{
					startMarkerIndex = startMarkerIndex + startMarker.Length;

					r = s.Substring(startMarkerIndex, endMarkerIndex - startMarkerIndex);
				}
			}

			return r;
		}

		public static string GetSubstringAfterLastOccurence(this string s, string marker)
		{
			string r = null;

			if (s != null && !string.IsNullOrEmpty(marker))
			{
				int startMarkerIndex = s.LastIndexOf(marker);
				int endMarkerIndex = s.Length;

				if (startMarkerIndex >= 0 && endMarkerIndex >= 0)
				{
					startMarkerIndex = startMarkerIndex + marker.Length;

					r = s.Substring(startMarkerIndex, endMarkerIndex - startMarkerIndex);
				}
			}

			return r;
		}
		public static string GetSubstring(this string s, string startMarker, string endMarker, bool includeMarkersInResult = false, int? searchFromIndex = null, bool caseSensitive = true, string notFoundCaseResult = null)
		{
			int? notUsedA;
			int? notUsedB;
			string r = GetSubstring(s, startMarker, endMarker, includeMarkersInResult,out notUsedA, out notUsedB, searchFromIndex, caseSensitive);
			return r;
		}
		public static string GetSubstring(this string s, string startMarker, string endMarker, bool includeMarkersInResult, out int? startIndex, out int? endIndex, int? searchFromIndex, bool caseSensitive = true, string notFoundCaseResult = null)
		{
			string r = notFoundCaseResult;
			startIndex = null;
			endIndex = null;

			if (s != null && !string.IsNullOrEmpty(startMarker) && !string.IsNullOrEmpty(endMarker))
			{
				int startMarkerIndex = s.IndexOf(startMarker, searchFromIndex??0, caseSensitive?StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
				int endMarkerIndex = -1;
				if (startMarkerIndex > -1 && startMarkerIndex + startMarker.Length < s.Length) 
				{ 
					endMarkerIndex = s.IndexOf(endMarker, startMarkerIndex+startMarker.Length, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
				}

				if (startMarkerIndex >= 0 && endMarkerIndex >= 0)
				{
					if (!includeMarkersInResult)
					{
						startMarkerIndex = startMarkerIndex + startMarker.Length;

						r = s.Substring(startMarkerIndex, endMarkerIndex - startMarkerIndex);

						startIndex = startMarkerIndex;
						endIndex = endMarkerIndex+endMarker.Length;
					}
					else
					{
						r = s.Substring(startMarkerIndex, endMarkerIndex - startMarkerIndex + endMarker.Length);

						startIndex = startMarkerIndex;
						endIndex = endMarkerIndex + endMarker.Length;
					}
				}
			}

			return r;
		}
		public static List<string> GetSubstrings(this string sOrig, string startMarker, string endMarker, bool caseSensitive = true)
		{
			var r = new List<string>();

			if (sOrig == null)
			{
				throw new Exception("GetSubstrings: sOrig can not be null.");
			}

			string s = sOrig;
			while (true)
			{
				if (s != null && !string.IsNullOrEmpty(startMarker) && !string.IsNullOrEmpty(endMarker))
				{
					int startMarkerIndex = s.IndexOf(startMarker, caseSensitive?StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
					if (startMarkerIndex >= 0)
					{
						int endMarkerIndex = s.IndexOf(endMarker, startMarkerIndex + startMarker.Length, caseSensitive?StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

						if (startMarkerIndex >= 0 && endMarkerIndex >= 0)
						{
							startMarkerIndex = startMarkerIndex + startMarker.Length;

							r.Add(s.Substring(startMarkerIndex, endMarkerIndex - startMarkerIndex));
						}
						else
						{
							break;
						}
						s = s.Remove(0, endMarkerIndex + endMarker.Length);
					}
					else
					{
						break;
					}
				}
			}

			return r;
		}

		public static string ReplaceSubstrings(this string s, string startMarker, string endMarker, string replacement, bool preserveMarkers = false, bool caseSensitive = true)
		{
			var parts = s.GetSubstrings(startMarker, endMarker, caseSensitive);
			if (preserveMarkers)
			{
				replacement = startMarker + replacement + endMarker;
			}
			foreach (string p in parts)
			{
				s = s.Replace(startMarker+p+endMarker, replacement);
			}
			return s;
		}

		public static string RemoveSubstring(this string s, string startMarker, string endMarker, bool removeMarkersAlso)
		{
			string r = null;

			if (s != null && !string.IsNullOrEmpty(startMarker) && !string.IsNullOrEmpty(endMarker))
			{
				int startMarkerIndex = s.IndexOf(startMarker);
				int endMarkerIndex = s.IndexOf(endMarker, startMarkerIndex);

				if (startMarkerIndex >= 0 && endMarkerIndex >= 0)
				{
					if (!removeMarkersAlso)
					{
						startMarkerIndex = startMarkerIndex + startMarker.Length;
					}
					else
					{
						endMarkerIndex = endMarkerIndex + endMarker.Length;
					}

					r = s.Remove(startMarkerIndex, endMarkerIndex - startMarkerIndex);
				}
			}

			return r;
		}

		/// <summary>
		/// Returns string with replaced part. Returns null on error.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="partToReplaceStartIndex"></param>
		/// <param name="partToReplaceEndIndex"></param>
		/// <param name="replacementString"></param>
		/// <returns></returns>
		public static string Replace(this string s, int partToReplaceStartIndex, int partToReplaceEndIndex, string replacementString)
		{
			if (partToReplaceStartIndex >= s.Length || partToReplaceEndIndex >= s.Length ||
				partToReplaceStartIndex > partToReplaceEndIndex)
			{
				s = null;
			}
			else
			{
				string firstPart = s.Substring(0, partToReplaceStartIndex);
				string lastPart = s.Substring(partToReplaceEndIndex);
				s = firstPart + replacementString + lastPart;
			}
			return s;
		}

		public static string RemoveSpaces(this string s)
		{
			if (s == null)
			{
				return s;
			}
			else
			{
				return s.Replace(" ", "");
			}
		}

		public static string RemoveRepeatedSpaces(this string s)
		{
			if (s == null)
			{
				return s;
			}
			else
			{
				string newS = s;
				do
				{
					s = newS;
					newS = s.Replace("  ", " ");
				} while (newS.Length!=s.Length);
				
				return s;
			}
		}

		public static List<string> DistinctStrings(this List<string> items)
		{
			var r = new List<string>();
			foreach (var item in items)
			{
				if (!r.Contains(item))
				{
					r.Add(item);
				}
			}
			return r;
		}

		public static string PrepandHiddenHtmlZeros(this string number, int length)
		{
			string r = number;
			string zeros = string.Empty;

			while (r.Length + zeros.Length < length)
			{
				zeros += " ";
			}
			//zeros = string.Format("<span style=\"visibility:hidden\">{0}</span>", zeros);
			return zeros + number;
		}

		/// <summary>
		/// String length check is performed. I.e. “test”. SafeLeftSubstring(50) returns “test”
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string SafeLeftSubstring(this String data, int length)
		{
			return String.IsNullOrEmpty(data) ? data : data.Substring(0, data.Length > length ? length : data.Length);
		}

		public static string ToProperStringDisplayValue(this decimal value, int decimalPlaces)
		{
			var d = (double)value;

			d = d * Math.Pow(10, decimalPlaces);
			d = Math.Truncate(d);
			d = d / Math.Pow(10, decimalPlaces);

			return string.Format("{0:N" + Math.Abs(decimalPlaces) + "}", d);
		}

		

		public static List<string> RemoveDuplicates(this List<string> list)
		{

			Dictionary<string, object> map = new Dictionary<string, object>();
			int i = 0;
			while (i < list.Count)
			{
				string current = list[i];
				if (map.ContainsKey(current))
				{
					list.RemoveAt(i);
				}
				else
				{
					i++;
					map.Add(current, null);
				}
			}

			return list;
		}

		public static int IndexOfNthOccurrence(this string s, string part, int N)
		{
			var offset = -1;
			for (int i = 1; i <=N ; i++)
			{
				if (offset < 0)
				{
					offset = s.IndexOf(part);
				}
				else
				{
					offset = s.IndexOf(part, offset + 1);
				}
			}

			return offset;
		}

		public static List<int> IndexesOfOccurrences(this string s, string part, bool caseSensitive)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s parameter is null");
			}

			if (part == null || part.Length==0)
			{
				throw new Exception("part parameter is null or zero-length");
			}

			List<int> r = new List<int>();
			
			if (!caseSensitive)
			{
				s = s.ToLower();
				part = part.ToLower();
			}

			var offset = -1;
			do
			{
				if (offset < 0)
				{
					offset = s.IndexOf(part);
				}
				else
				{
					if (offset + 1 < part.Length)
					{
						offset = s.IndexOf(part, offset + 1);
					}
					else
					{
						offset = -1;
					}
				}

				if (offset >= 0)
				{
					r.Add(offset);
				}
			} while (offset >= 0);

			return r;
		}

		/// <summary>
		/// Finds consecvutive words and returns first next index after them.
		///  Example: for text.IndexAfterWords(true, "charset",ExtenderClass.OPTIONAL_SPACES,"=",ExtenderClass.OPTIONAL_SPACES,"\"") it will find all these:
		///  'charset = "' 
		///  'charset    = "'
		///  'charset="'
		/// </summary>
		/// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
		/// <param name="words">The words. You can use ExtenderClass.OPTIONAL_SPACES and ExtenderClass.OPTIONAL_SPACES as special constants.</param>
		/// <returns></returns>
		public static int IndexAfterWords(this string text, bool caseSensitive, params string[] words)
		{
			int index = -1;
			string origText = text;

			if (text.IsNOTNullOrWhiteSpace() && text.Length >= words.Sum(w => w.Length))
			{
				//if (caseSensitive == false)
				//{
				//	text = text.ToLower();
				//	for (int i = 0; i < words.Count(); i++)
				//	{
				//		words[i] = words[i].ToLower();
				//	}
				//}
				StringComparison sc = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

				index = 0;
				while (index != -1)
				{
					index = origText.IndexOf(words[0], index, sc);
					if (index != -1)
					{
						index += words[0].Length;
						text = origText.Substring(index);
						if (words.Count() > 1)
						{
							int index2 = index;
							for (int i = 1; i < words.Count(); i++)
							{
								if (text != null && index2 != -1)
								{
									switch (words[i])
									{
										case OPTIONAL_SPACES:
											text = text.TrimSpacesAtStartAndAdvanceIndex(ref index2, false);
											break;
										case REQUIRED_SPACES:
											text = text.TrimSpacesAtStartAndAdvanceIndex(ref index2, true);
											break;
										default:
											text = text.TrimXAtStartAndAdvanceIndex(words[i], ref index2, true, caseSensitive);
											break;
									}
								}
							}

							if (index2 != -1)
							{//all words found
								index = index2;
								break;
							}
						}
					}
				}

			}

			return index;
		}

		public const string OPTIONAL_SPACES = "<({OS})>";
		public const string REQUIRED_SPACES = "<({RS})>";

		public static string TrimXAtStartAndAdvanceIndex(this string text, string X, ref int index, bool resetResultAndIndexIfNotFound, bool caseSensitive=true)
		{
			if (text != null)
			{
				StringComparison sc = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
				if (text.StartsWith(X, sc))
				{
					text = text.Substring(X.Length);
					index += X.Length;
				}
				else
				{
					if (resetResultAndIndexIfNotFound)
					{
						text = null;
						index = -1;
					}
				}
			}

			return text;
		}

		public static string TrimSpacesAtStartAndAdvanceIndex(this string text, ref int index, bool resetResultAndIndexIfNotFound)
		{
			if (text != null)
			{
				string textTrimmed = text.TrimStart(' ');
				if (textTrimmed.Length == text.Length)
				{
					if (resetResultAndIndexIfNotFound)
					{
						text = null;
						index = -1;
					}
				}
				else
				{
					index += text.Length - textTrimmed.Length;
					text = textTrimmed;
				}
			}

			return text;
		}

		public static int OccurrencesCount(this string s, string keyword)
		{
			var offset = -1;
			int i = 0;
			while(i==0 || offset>=0)
			{
				if (offset < 0)
				{
					offset = s.IndexOf(keyword);
				}
				else
				{
					offset = s.IndexOf(keyword, offset + keyword.Length);
				}

				if (offset < 0)
				{
					break;
				}
				else
				{
					i++;
				}
			}

			return i;
		}

		public static string RemoveLastXChars(this string s, int x, string resultIfStringIsShorter = "")
		{
			if (s.Length < x)
			{
				return resultIfStringIsShorter;
			}
			else
			{
				return s.Remove(s.Length - x);
			}
		}

		public static string RemoveFirstXChars(this string s, int x, string resultIfStringIsShorter = "")
		{
			if (s.Length < x)
			{
				return resultIfStringIsShorter;
			}
			else
			{
				return s.Substring(x);
			}
		}

		public static string TrimNonDigitsAtEnd(this string s)
		{
			while (s.Length > 0)
			{
				if (s[s.Length - 1]!='0' && s[s.Length - 1]!='1' && s[s.Length - 1]!='2' && s[s.Length - 1]!='3' && s[s.Length - 1]!='4' && s[s.Length - 1]!='5' && 
					s[s.Length - 1]!='6' && s[s.Length - 1]!='7' && s[s.Length - 1]!='8' && s[s.Length - 1]!='9')//is number?
				{
					s = s.Remove(s.Length - 1);//s = s.RemoveLastXChars(1);
				}
				else
				{
					break;
				}
			}

			return s;
		}

		public static List<string> ToLines(this string s, bool keepZeroLengthLines, bool keepWhitespaceLines, List<string> nullCaseResult)
		{
			List<string> r;

			if (s == null)
			{
				r = nullCaseResult;
			}
			else
			{
				StringSplitOptions sso = keepZeroLengthLines ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries;
				r = s.Split(new string[] {"\n\r","\r\n","\n","\r"}, sso).ToList();
				if (r.Count > 0 && !keepWhitespaceLines)
				{
					for (int i = r.Count - 1; i >= 0; i--)
					{
						if (r[i].IsNullOrWhiteSpace())
						{
							r.RemoveAt(i);
						}
					}
				}
			}

			return r;
		}

		public static List<string> Split(this string s, string splitter, bool keepZeroLengthLines, bool keepWhitespaceLines, List<string> nullCaseResult)
		{
			List<string> r;

			if (s == null)
			{
				r = nullCaseResult;
			}
			else
			{
				StringSplitOptions sso = keepZeroLengthLines ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries;
				r = s.Split(new string[] {splitter}, sso).ToList();
				if (r.Count > 0 && !keepWhitespaceLines)
				{
					for (int i = r.Count - 1; i >= 0; i--)
					{
						if (r[i].IsNullOrWhiteSpace())
						{
							r.RemoveAt(i);
						}
					}
				}
			}

			return r;
		}

		public static bool AllLettersAreUppercase(this string s)
		{
			//for (int i = 0; i < s.Length; i++)
			//{
			//	if (Char.IsLetter(s[i]) && !Char.IsUpper(s[i]))
			//		return false;
			//}
			return s.All(c => !Char.IsLetter(c) || Char.IsUpper(c));
		}
		
		public static bool AllLettersAreLowercase(this string s)
		{
			//for (int i = 0; i < s.Length; i++)
			//{
			//	if (Char.IsLetter(s[i]) && !Char.IsUpper(s[i]))
			//		return false;
			//}
			return s.All(c => !Char.IsLetter(c) || Char.IsLower(c));
		}

		/// <summary>
		/// Capitalizes first letter and lowers remaining letters. Be careful -if first letter is not in A-Z like space nothing is capitalized.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToSentenceCase(this string s)
		{
			StringBuilder sb1 = new StringBuilder(s.Length);
			int i = 0;
			foreach (char ch in s)
			{
				if (i==0)
				{
					sb1.Append(Char.ToUpper(ch));
				}
				else
				{
					sb1.Append(Char.ToLower(ch));
				}
				i++;
			}
			return sb1.ToString();
		}

		/// <summary>
		/// In every word that starts with letter A-Z capitalizes that letter. Words a, an, the, at, by, for, in, of, on, to, up, and, as, but, or, nor are not capitalized if they are between spaces/words. If null is passed null is returned.
		/// Source: https://www.google.com/search?site=&source=hp&q=titles+capitalization+and+or&oq=titles+capitalization+and+or&gs_l=hp.3...3259.13356.0.13943.21.18.0.3.3.0.159.1910.4j13.17.0....0...1c.1.64.hp..1.19.1837.0Il_xsB1Ds0
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string s, bool preserveAcronyms = false, bool whenAllWordsAreCapitalizedNoneIsAcronym = true)
		{
			if (s == null)
			{
				return null;
			}
			else
			{
				StringBuilder sb1 = new StringBuilder(s.Length);
				bool lastCharWasSpaceOrBeggining = true;
				int i = 0;
				foreach (char ch in s)
				{
					if (lastCharWasSpaceOrBeggining)
					{
						sb1.Append(Char.ToUpper(ch));
					}
					else
					{
						sb1.Append(Char.ToLower(ch));
					}
					lastCharWasSpaceOrBeggining = ch == ' ';
					i++;
				}
				string r = sb1.ToString();
				r = r.Replace(" A ",  " a ");
				r = r.Replace(" An ", " an ");
				r = r.Replace(" The "," the ");
				r = r.Replace(" At ", " at ");
				r = r.Replace(" By ", " by ");
				r = r.Replace(" For "," for ");
				r = r.Replace(" In ", " in ");
				r = r.Replace(" Of ", " of ");
				r = r.Replace(" On ", " on ");
				r = r.Replace(" To ", " to ");
				r = r.Replace(" Up ", " up ");
				r = r.Replace(" And "," and ");
				r = r.Replace(" As ", " as ");
				r = r.Replace(" But "," but ");
				r = r.Replace(" Or ", " or ");
				r = r.Replace(" Nor "," nor ");
				
				if (preserveAcronyms)
				{
					if (whenAllWordsAreCapitalizedNoneIsAcronym && s.AllLettersAreUppercase())
					{
						//whole string is capitalized - no acronyms inside
					}
					else
					{
						var originalWords = s.Split();
						var sentenceCasedWords = r.Split();
						List<string> finalWords = new List<string>();
						int j = 0;
						foreach (var originalWord in originalWords)
						{
							if (originalWord.Length > 1 && originalWord.AllLettersAreUppercase())
							{
								//use acronym from original
								finalWords.Add(originalWord);

							}
							else
							{
								//use already sentance-cased word
								finalWords.Add(sentenceCasedWords[j]);
							}
							j++;
						}
						r = finalWords.ToSingleString(null, "", ' ');
					}
				}

				return r;
			}
		}

		public static string LowerFirstChar(this string s)
		{
			string r = s;
			if (!string.IsNullOrEmpty(r))
			{
				r = s[0].ToString().ToLower() + s.Remove(0, 1);
			}
			return r;
		}

		public static string LowerFirstCharOfEveryWord(this string s)
		{
			string r = string.Empty;
			string[] words;
			if (!string.IsNullOrEmpty(s))
			{
				words = s.Split();
				foreach (string word in words)
				{
					r = r + word[0].ToString().ToLower() + word.Remove(0, 1);
					r = r + " ";
				}
				r = r.Trim();
			}
			return r;
		}

		public static string UpperFirstChar(this string s)
		{
			string r = s;
			if (!string.IsNullOrEmpty(r))
			{
				r = s[0].ToString().ToUpper() + s.Remove(0, 1);
			}
			return r;
		}

		public static string UpperFirstCharOfEveryWord(this string s)
		{
			string r = string.Empty;
			string[] words;
			if (!string.IsNullOrEmpty(s))
			{
				words = s.Split();
				foreach (string word in words)
				{
					r = r + word[0].ToString().ToUpper() + word.Remove(0, 1);
					r = r + " ";
				}
				r = r.Trim();
			}
			return r;
		}

		public static bool ContainsDigit(this string s, bool nullCaseResult = false)
		{
			bool r = false;
			if (s == null)
			{
				r = nullCaseResult;
			}
			else
			{
				foreach (char c in s)
				{
					if (Char.IsDigit(c))
					{
						r = true;
						break;
					}
				}
			}
			return r;
		}
		
		public static bool ContainsLetter(this string s, bool nullCaseResult = false)
		{
			bool r = false;
			if (s == null)
			{
				r = nullCaseResult;
			}
			else
			{
				foreach (char c in s)
				{
					if (Char.IsLetter(c))
					{
						r = true;
						break;
					}
				}
			}
			return r;
		}

		public static bool ContainsCapitalLetter(this string s, bool nullCaseResult = false)
		{
			bool r = false;
			if (s == null)
			{
				r = nullCaseResult;
			}
			else
			{
				foreach (char c in s)
				{
					if (Char.IsLetter(c) && Char.IsUpper(c))
					{
						r = true;
						break;
					}
				}
			}
			return r;
		}

		public static bool ContainsNonLetterNonDigit(this string s, bool nullCaseResult = false)
		{
			bool r = false;
			if (s == null)
			{
				r = nullCaseResult;
			}
			else
			{
				foreach (char c in s)
				{
					if (!Char.IsLetterOrDigit(c))
					{
						r = true;
						break;
					}
				}
			}
			return r;
		}
		#endregion

		#region CSV
		/// <summary>
		/// Splits string instance on every comma sign. 
		/// Does trimming all the way. 
		/// Returns list if resuting parts which does not include empty or whitespace items.
		/// On any error returns null.
		/// </summary>
		/// <param name="nullableString"></param>
		/// <returns></returns>
		public static List<string> ParseCSV(this string nullableString)
		{
			return ParseCSV<string>(nullableString, new char[] { ',' }, new char[] { ' ' }, false, null, null, null);
		}

		public static List<string> ParseCSV(this string nullableString, char[] separator)
		{
			return ParseCSV<string>(nullableString, separator, new char[] { ' ' }, false, null, null, null);
		}
		
		public static List<T> ParseCSV<T>(this string nullableString, char[] separator, char[] trimChars, bool includeEmptyOrWhiteSpaceItems, List<T> nullCaseResult, List<T> errorCaseResult, List<T> emptyStringCaseResult)
		{
			List<T> r = null;

			if (nullableString == null)
			{
				r = nullCaseResult;
			}
			else
			{
				try
				{
					string v = nullableString;
					if (trimChars != null)
					{
						v = v.Trim(trimChars);
					}

					string[] parts = nullableString.Split(separator, StringSplitOptions.None);
					if (parts.Length == 1 && parts[0] == string.Empty)
					{
						return emptyStringCaseResult;
					}
					else
					{
						foreach (var part in parts)
						{
							string p = part;
							if (trimChars != null)
							{
								p = p.Trim(trimChars);
							}

							if (!p.IsNullOrWhiteSpace() || includeEmptyOrWhiteSpaceItems)
							{
								if (r == null)
								{
									r = new List<T>();
								}

								T pAsT = (T)Convert.ChangeType((object)p, typeof(T));
								r.Add(pAsT);
							}
						}
					}
				}
				catch (Exception)
				{
					r = errorCaseResult;
				}
			}

			return r;
		}

		/// <summary>
		/// Converts list of values to comma-separated-values (CSV) string. Empty and whitespace items are not included in resulting string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string ToCSV<T>(this List<T> list)
		{
			return ToCSV<T>(list, false);
		}

		/// <summary>
		/// Converts list of values to comma-separated-values (CSV) string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="includeEmptyOrWhiteSpaceItems"></param>
		/// <returns></returns>
		public static string ToCSV<T>(this List<T> list, bool includeEmptyOrWhiteSpaceItems)
		{
			return ToCSV<T>(list, ",", new char[] { ' ' }, includeEmptyOrWhiteSpaceItems, null, null);
		}

		/// <summary>
		/// Creates the CSV from a generic list.
		/// </summary>
		public static string ToCSV<T>(this List<T> list, string separator, char[] trimChars, bool includeEmptyOrWhiteSpaceItems, string nullCaseResult, string errorCaseResult)
		{
			string r = null;

			if (list == null)
			{
				r = nullCaseResult;
			}
			else
			{
				try
				{
					StringBuilder sb = new StringBuilder(string.Empty);
					foreach (var v in list)
					{
						string item = string.Format("{0}", v);

						if (trimChars != null)
						{
							item = item.Trim(trimChars);
						}

						if (!item.IsNullOrWhiteSpace() ||
						item.IsNullOrWhiteSpace() && includeEmptyOrWhiteSpaceItems)
						{
							if (sb.Length > 0)
							{
								sb.Append(separator);
							}
							sb.Append(item);
						}
					}
					r = sb.ToString();
				}
				catch (Exception)
				{
					r = errorCaseResult;
				}
			}

			return r;
		}

		public static string ToSingleString(this IEnumerable<string> ss, string nullCaseResult = null, string zeroItemsCaseResult = "", char? separator = null)
		{
			string r;

			if (ss == null)
			{
				r = nullCaseResult;
			}
			else if (ss.Count() == 0)
			{
				r = zeroItemsCaseResult;
			}
			else
			{
				r = string.Empty;
				foreach (string s in ss)
				{
					r = r + s + separator??string.Empty;
				}
				if (separator != null)
				{
					r = r.TrimEnd(separator.Value);
				}
			}

			return r;
		}

		public static string ToSingleString(this IEnumerable<string> ss, string nullCaseResult = null, string zeroItemsCaseResult = "", string separator = null)
		{
			string r;

			if (ss == null)
			{
				r = nullCaseResult;
			}
			else if (ss.Count() == 0)
			{
				r = zeroItemsCaseResult;
			}
			else
			{
				r = string.Empty;
				int i = 0;
				foreach (string s in ss)
				{
					r = r + s;
					if (i+1<ss.Count())
					{
						r = r+separator ?? string.Empty;
					}
					i++;
				}
			}

			return r;
		}
		#endregion

		#region Numbers
		public static int Round(this decimal v)
		{
			return (int)Math.Round(v);
		}
		public static int Round(this float v)
		{
			return (int)Math.Round(v);
		}
		public static int Round(this double v)
		{
			return (int)Math.Round(v);
		}
		public static long RoundToLong(this decimal v)
		{
			return (long)Math.Round(v);
		}
		public static long RoundToLong(this float v)
		{
			return (long)Math.Round(v);
		}
		public static long RoundToLong(this double v)
		{
			return (long)Math.Round(v);
		}

		#region LimitToRange
		/// <summary>
		/// if first parameter is smaller than min it will be set to min. 
		/// If first parameter is bigger then max it will be set to max. 
		/// Returns true if value was changed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool LimitToRange(ref byte value, byte min, byte max)
		{
			if (value < min)
			{
				value = min;
				return true;
			}else if (value > max)
			{
				value = max;
				return true;
			}
			return false;
		}

		/// <summary>
		/// if first parameter is smaller than min it will be set to min. 
		/// If first parameter is bigger then max it will be set to max. 
		/// Returns true if value was changed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool LimitToRange(ref int value, int min, int max)
		{
			if (value < min)
			{
				value = min;
				return true;
			}
			else if (value > max)
			{
				value = max;
				return true;
			}
			return false;
		}

		/// <summary>
		/// if first parameter is smaller than min it will be set to min. 
		/// If first parameter is bigger then max it will be set to max. 
		/// Returns true if value was changed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool LimitToRange(ref long value, long min, long max)
		{
			if (value < min)
			{
				value = min;
				return true;
			}
			else if (value > max)
			{
				value = max;
				return true;
			}
			return false;
		}

		/// <summary>
		/// if first parameter is smaller than min it will be set to min. 
		/// If first parameter is bigger then max it will be set to max. 
		/// Returns true if value was changed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool LimitToRange(ref decimal value, decimal min, decimal max)
		{
			if (value < min)
			{
				value = min;
				return true;
			}
			else if (value > max)
			{
				value = max;
				return true;
			}
			return false;
		}

		/// <summary>
		/// if first parameter is smaller than min it will be set to min. 
		/// If first parameter is bigger then max it will be set to max. 
		/// Returns true if value was changed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool LimitToRange(ref float value, float min, float max)
		{
			if (value < min)
			{
				value = min;
				return true;
			}
			else if (value > max)
			{
				value = max;
				return true;
			}
			return false;
		}

		/// <summary>
		/// if first parameter is smaller than min it will be set to min. 
		/// If first parameter is bigger then max it will be set to max. 
		/// Returns true if value was changed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool LimitToRange(ref double value, double min, double max)
		{
			if (value < min)
			{
				value = min;
				return true;
			}
			else if (value > max)
			{
				value = max;
				return true;
			}
			return false;
		}			
		#endregion

		public static string GetDigitsAfterDecimalPoint(this decimal n, int numberOfDigits)
		{
			int result = Decimal.ToInt32((Decimal.Round(n, 2) - Decimal.Floor(n)) * 100);
			return result > 9 ? result.ToString() : "0" + result;
		}

		public static string GetDigitsBeforeDecimalPoint(this decimal n)
		{
			//need to floor n first?
			return Decimal.ToInt32(n).ToString();
		}

		public static int MiddleValue(int a, int b)
		{
			if (a == b)
			{
				return a;
			}
			if (a < b)
			{
				return a + ((b - a)/2);
			}
			else
			{
				return b + ((a - b)/2);
			}

		}
		#endregion

		#region Parameter
		/// <summary>
		/// Tells if for example in string: someCommand /parA:1 /ParB
		/// parameter ParA is present.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="parameterName"></param>
		/// <param name="hideErrors"></param>
		/// <param name="resultInErrorCase"></param>
		/// <param name="parameterPrefix"></param>
		/// <param name="separatorBetweenKeyAndValue"></param>
		/// <returns></returns>
		public static bool GetParameterPresence(this string line, string parameterName, bool hideErrors, bool resultInErrorCase, char parameterPrefix = '/', char? separatorBetweenKeyAndValue = null)
		{
			bool found;
			
			try
			{
				line = line.Trim();

				if (parameterName.StartsWith(parameterPrefix.ToString()))
				{
					parameterName = parameterName.TrimStart(parameterPrefix);
				}
				
				string[] parts = line.SplitButDontBreakQuotedParts('"').ToArray();
				string parameterPart = null;
				if (parts.Length > 0)
				{
					string separator = string.Empty;
					if (separatorBetweenKeyAndValue != null)
					{
						separator = separatorBetweenKeyAndValue.ToString();
					}
					parameterPart = parts.SingleOrDefault(p => p.ToLower().StartsWith(parameterPrefix.ToString().ToLower() + parameterName.ToLower() + separator.ToLower()));
				}

				found = !string.IsNullOrEmpty(parameterPart);	
			}catch (Exception exception)
			{
				if (hideErrors)
				{
					found = resultInErrorCase;
				}
				else
				{
					Exception outerException = new Exception(String.Format("Can not read parameter '{0}' from command '{1}'.", parameterName, line.Split()[0]), exception);
					throw outerException;
				}
			}

			return found;
		}

		/// <summary>
		/// Example from: somecommand /paramNameA:1 /paramNameB
		/// extracts 1. Its not case sensitive.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="line"></param>
		/// <param name="throwErrorIfNotFound"></param>
		/// <param name="resultInNotFoundCase"></param>
		/// <param name="throwErrorIfEmptyString"></param>
		/// <param name="resultInEmptyStringCase"></param>
		/// <param name="hideErrors"></param>
		/// <param name="resultInErrorCase"></param>
		/// <param name="separatorBetweenKeyAndValue"></param>
		/// <returns></returns>
		public static T GetParameterValue<T>(this string line, string parameterName, bool throwErrorIfNotFound, T resultInNotFoundCase, bool throwErrorIfEmptyString, T resultInEmptyStringCase, bool hideErrors, T resultInErrorCase, char parameterPrefix = '/', char separatorBetweenKeyAndValue = ':', bool allowQuotedValue = false, char quoteChar = '"')
		{
			T r = default(T);

			try
			{
				line = line.Trim();
				bool found = false;

				if (parameterName.StartsWith(parameterPrefix.ToString()))
				{
					parameterName = parameterName.TrimStart(parameterPrefix);
				}

				List<string> parts = null;
				if (!allowQuotedValue)
				{
					parts = line.Split(' ').ToList();
				}
				else
				{
					parts = line.SplitButDontBreakQuotedParts(quoteChar);
				}
				string parameterPart = null;
				if (parts.Count > 0)
				{
					parameterPart = parts.SingleOrDefault(p => p.ToLower().StartsWith(parameterPrefix.ToString().ToLower() + parameterName.ToLower() + separatorBetweenKeyAndValue.ToString().ToLower()));
				}

				found = !string.IsNullOrEmpty(parameterPart);	
				
				if (found == false)
				{
					if (!throwErrorIfNotFound)
					{
						r = resultInNotFoundCase;
					}
					else
					{
						string errorMessage;
						if (resultInNotFoundCase == null)
						{
							errorMessage = string.Format("Missing parameter '{0}' from command '{1}'", parts[0]);
						}else
						{
							errorMessage = resultInEmptyStringCase.ToString();
						}
						throw new Exception(errorMessage);
					}
				}
				else
				{
					string value = parameterPart.Split(separatorBetweenKeyAndValue)[1];

					if (value == String.Empty)
					{
						if (!throwErrorIfEmptyString)
						{
							r = resultInEmptyStringCase;
						}
						else
						{
							string errorMessage;
							if (resultInEmptyStringCase == null)
							{
								errorMessage = string.Format("Empty value for parameter '{0}' in command '{1}' is not allowed.", parts[0]);
							}else
							{
								errorMessage = resultInErrorCase.ToString();
							}
							throw new Exception(errorMessage);
						}
					}
					else
					{
						if (allowQuotedValue && value.StartsWith(quoteChar.ToString()) && value.EndsWith(quoteChar.ToString()))
						{
							value = value.Substring(1);
							value = value.RemoveLastXChars(1);
						}
						r = (T)Convert.ChangeType(value, typeof(T));
					}
				}
			}
			catch (Exception exception)
			{
				if (hideErrors)
				{
					r = resultInErrorCase;
				}
				else
				{
					Exception outerException = new Exception(String.Format("Can not read parameter '{0}' from command '{1}'.", parameterName, line.Split()[0]), exception);
					throw outerException;
				}
			}
			return r;
		}

		/// <summary>
		/// For example from: someCommand arg1 "arg2 with space" "arg3"
		/// returns list:
		///   someCommand
		///   arg1
		///   arg2 with space
		///   arg3
		/// </summary>
		/// <param name="line"></param>
		/// <param name="includeFirstWord"></param>
		/// <param name="leaveBrackets"></param>
		/// <param name="bracketChar"></param>
		/// <returns></returns>
		public static List<string> GetParameters(this string line, bool includeFirstWord = true,bool leaveBrackets = false, char bracketChar = '"')
		{
			List<string> r = null;
			string tempLine = string.Empty;

			//inside brackets replace spaces with special string

			bool bracketOpen = false;
			foreach (char c in line)
			{
				if (c == bracketChar)
				{
					bracketOpen = !bracketOpen;
					if (leaveBrackets)
					{
						tempLine += c;
					}
				}
				else
				{
					if (c == ' ' && bracketOpen)
					{
						tempLine += "[(<space>)]";
					}
					else
					{
						tempLine += c;
					}
				}
			}

			r = tempLine.Split(' ').ToList();

			if (!includeFirstWord && r.Any())
			{
				r.RemoveAt(0);
			}

			for (int i = 0; i < r.Count; i++)
			{
				r[i] = r[i].Replace("[(<space>)]", " ");
			}

			return r;
		}

		/// <summary>
		/// For example from: arg1 "arg2 with space" "arg3"
		/// returns list:   
		///   arg1
		///   arg2 with space
		///   arg3
		/// </summary>
	
		/// <returns></returns>
		public static List<string> SplitButDontBreakQuotedParts(this string line, char quoteChar = '"')
		{
			List<string> r = null;
			string tempLine = string.Empty;

			//inside quote replace spaces with special string

			bool quoteOpen = false;
			foreach (char c in line)
			{
				if (c == quoteChar)
				{
					quoteOpen = !quoteOpen;
					tempLine += c;
				}
				else
				{
					if (c == ' ' && quoteOpen)
					{
						tempLine += "[(<space>)]";
					}
					else
					{
						tempLine += c;
					}
				}
			}

			r = tempLine.Split(' ').ToList();
			
			for (int i = 0; i < r.Count; i++)
			{
				r[i] = r[i].Replace("[(<space>)]", " ");
			}

			return r;
		}
		
		public static string RemoveParameter(this string line, string parameterName, bool hideErrors, string resultInErrorCase, char parameterPrefix = '/', char bracketChar = '"', char? separatorBetweenKeyAndValue = null)
		{
			string newLine;

			try
			{
				if (parameterName.StartsWith(parameterPrefix.ToString()))
				{
					parameterName = parameterName.TrimStart(parameterPrefix);
				}

				var parameters = GetParameters(line, true, true, bracketChar);
				
				var parametersMatched =parameters.Where(
					p =>p.ToLower().StartsWith(parameterPrefix.ToString().ToLower() + parameterName.ToLower() +(separatorBetweenKeyAndValue == null ? "" : separatorBetweenKeyAndValue.Value.ToString().ToLower()))
					).ToList();

				if (parametersMatched.Count() != 1)
				{
					throw new Exception("None or more than one matching parameter found in line.");
				}
				else
				{
					parameters.Remove(parametersMatched[0]);
				}

				newLine = string.Empty;
				foreach (string p in parameters)
				{
					newLine = newLine + " " + p;
				}
				newLine = newLine.Trim();
			}
			catch (Exception exception)
			{
				if (hideErrors)
				{
					newLine = resultInErrorCase;
				}
				else
				{
					Exception outerException = new Exception(String.Format("Can not read or remove parameter '{0}' from command '{1}'.", parameterName, line.Split()[0]), exception);
					throw outerException;
				}
			}

			return newLine;
		}
		#endregion
		
		#region KeyValuePair
		public static KeyValuePair<T1, T2> UpdateKey<T1, T2>(this KeyValuePair<T1, T2> pair, T1 newKey)
		{
			pair = new KeyValuePair<T1, T2>(newKey, pair.Value);
			return pair;
		}
		
		public static KeyValuePair<T1, T2> UpdateValue<T1, T2>(this KeyValuePair<T1, T2> pair, T2 newValue)
		{
			pair = new KeyValuePair<T1, T2>(pair.Key, newValue);
			return pair;
		}

		public static KeyValuePair<string,string> UpdateKey(this KeyValuePair<string,string> pair, string newKey)
		{
			pair = new KeyValuePair<string,string>(newKey, pair.Value);
			return pair;
		}
		
		public static KeyValuePair<string,string> UpdateValue(this KeyValuePair<string,string> pair, string newValue)
		{
			pair = new KeyValuePair<string,string>(pair.Key, newValue);
			return pair;
		}

		public static List<KeyValuePair<string, string>> RemoveDuplicates(this List<KeyValuePair<string, string>> list, bool compareByKey, bool compareByValue, bool caseSensitive = true)
		{
			List<KeyValuePair<string, string>> r = new List<KeyValuePair<string, string>>();
			StringComparison sc = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			foreach (KeyValuePair<string, string> kvp in list)
			{
				bool alreadyAdded = false;

				if (compareByKey)
				{
					alreadyAdded = r.Exists(rkvp => string.Compare(rkvp.Key, kvp.Key, sc) == 0);
				}
				else if (compareByValue)
				{
					alreadyAdded = r.Exists(rkvp => string.Compare(rkvp.Value, kvp.Value, sc) == 0);
				}
				else if (compareByKey && compareByValue)
				{
					alreadyAdded = r.Exists(rkvp => string.Compare(rkvp.Key, kvp.Key, sc) == 0) && r.Exists(rkvp => string.Compare(rkvp.Value, kvp.Value, sc) == 0);
				}
				else
				{
					throw new Exception("Either compareByKey or compareByValue must be true or both can be true.");
				}

				if (!alreadyAdded)
				{
					r.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value));
				}
			}

			return r;
		}
		#endregion
		
		#region Byte array
		public static string ToCSVOfDecimals(this byte[] bytes)
		{
			string r = string.Empty;
			StringBuilder sb = new StringBuilder();
			foreach (byte b in bytes)
			{
				sb.Append(System.Convert.ToDecimal(b));
				sb.Append(",");
			}
			r = sb.ToString().TrimEnd(',');
			return r;
		}

		public static byte[] FromCSVOfDecimals(this string csvOfDecimals)
		{
			List<byte> bytes = new List<byte>();

			try
			{
				foreach (string d in csvOfDecimals.Split(new char[]{ ','}, StringSplitOptions.RemoveEmptyEntries))
				{
					bytes.Add(byte.Parse(d));
				}
			}
			catch {
				bytes = new List<byte>();
			}

			return bytes.ToArray();
		}

		/// <summary>
		/// Source: http://www.pvladov.com/2012/07/arbitrary-to-decimal-numeral-system.html
		/// </summary>
		/// <param name="number">The arbitrary numeral system number to convert.</param>
		/// <param name="baseA">The radix of the numeral system the given number
		/// is in (in the range [2, 36]).</param>
		/// <returns></returns>
		public static long FromDifferentBase(string number, string allPossibleDigits)//"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		{
			if (allPossibleDigits.Length < 2)
			{
				throw new ArgumentException("allPossibleDigits must contain at lease two digits.");
			}

			if (String.IsNullOrEmpty(number))
			{
				return 0;
			}

			// Make sure the arbitrary numeral system number is in upper case
			number = number.ToUpperInvariant();

			long result = 0;
			long multiplier = 1;
			for (int i = number.Length - 1; i >= 0; i--)
			{
				char c = number[i];
				if (i == 0 && c == '-')
				{
					// This is the negative sign symbol
					result = -result;
					break;
				}

				int digit = allPossibleDigits.IndexOf(c);
				if (digit == -1)
				{
					throw new ArgumentException("Invalid character in the arbitrary numeral system number", "number");
				}

				result += digit * multiplier;
				multiplier *= allPossibleDigits.Length;
			}

			return result;
		}

		public static string ToDifferentBase(this int value, string allPossibleDigits)
		{
			int i = 32;
			char[] buffer = new char[i];
			int targetBase = allPossibleDigits.Length;

			do
			{
				buffer[--i] = allPossibleDigits[value % targetBase];
				value = value / targetBase;
			}
			while (value > 0);

			char[] result = new char[32 - i];
			Array.Copy(buffer, i, result, 0, 32 - i);

			return new string(result);
		}

		public static string ToHex(this byte[] bytes)
		{
			StringBuilder r = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				r.AppendFormat("{0:x2}", b);
			}
			return r.ToString();
		}

		public static byte[] FromHex(this String hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return bytes;
		}

		public static string ToAlphaOnlyHex(this byte[] bytes)
		{
			string allChars = "0123456789ABCDEFGHIJKLMNOP";
			string hex = ToHex(bytes);
			StringBuilder alphaOnlyHex = new StringBuilder();
			foreach (char c in hex.ToUpper())
			{
				alphaOnlyHex.Append(allChars[allChars.IndexOf(c) + 10]);
			}
			return alphaOnlyHex.ToString();
		}

		public static byte[] FromAlphaOnlyHex(this string alphaOnlyHex)
		{
			string allChars = "0123456789ABCDEFGHIJKLMNOP";
			StringBuilder hex = new StringBuilder();
			foreach (char c in alphaOnlyHex)
			{
				hex.Append(allChars[allChars.IndexOf(c) - 10]);
			}
			return FromHex(hex.ToString());
		}

		public static T ToStructure<T>(this byte[] bytes) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),typeof(T));
			handle.Free();
			return stuff;
		}

		public static byte[] ReplaceStringInBytes(this byte[] source, string searchPhrase, string replacement, Encoding encoding)
		{
			byte[] searchPhraseAsUtf8Bytes = encoding.GetBytes(searchPhrase);
			byte[] replacementAsUtf8Bytes = encoding.GetBytes(replacement);
			byte[] r = ReplaceBytes(source, searchPhraseAsUtf8Bytes, replacementAsUtf8Bytes);
			return r;
		}

		public static byte[] ReplaceBytes(this byte[] source, byte[] searchPhrase, byte[] replacement)
		{
			byte[] r = null;

			var matchedPositions = IndexesOfAllOccurrencesOfBytes(source, searchPhrase);
			if (matchedPositions.Count == 0)
			{
				r = new byte[source.Length];
				Buffer.BlockCopy(source, 0, r, 0, source.Length);
			}
			else
			{
				if (searchPhrase.Length <= replacement.Length)
				{
					r = new byte[source.Length + ((replacement.Length - searchPhrase.Length) * matchedPositions.Count)];
				}
				else
				{
					r = new byte[source.Length - ((searchPhrase.Length - replacement.Length) * matchedPositions.Count)];
				}

				int rPosition = 0;
				int sourcePosition = 0;
				foreach (int matchedPosition in matchedPositions)
				{
					if (sourcePosition > source.Length)
					{
						break;
					}
					Buffer.BlockCopy(source, sourcePosition, r, rPosition, matchedPosition - sourcePosition);
					rPosition = rPosition + (matchedPosition - sourcePosition);
					sourcePosition = matchedPosition + searchPhrase.Length;


					Buffer.BlockCopy(replacement, 0, r, rPosition, replacement.Length);
					rPosition = rPosition + replacement.Length;
				}
				if (sourcePosition <= source.Length)
				{
					Buffer.BlockCopy(source, sourcePosition, r, rPosition, source.Length - sourcePosition);
				}
			}

			return r;
		}

		public static List<int> IndexesOfAllOccurrencesOfBytes(this byte[] source, byte[] searchPhrase)
		{
			List<int> r = new List<int>();

			int i = -1;
			while (true)
			{
				i = IndexOfBytes(source, searchPhrase, i + 1, source.Length);
				if (i != -1)
				{
					r.Add(i);
				}
				else
				{
					break;
				}
			}

			return r;
		}

		/// <summary>
		/// Source: post at Thursday, March 31, 2011 5:08 AM at https://social.msdn.microsoft.com/Forums/vstudio/en-US/15514c1a-b6a1-44f5-a06c-9b029c4164d7/searching-a-byte-array-for-a-pattern-of-bytes?forum=csharpgeneral
		/// </summary>
		/// <param name="array"></param>
		/// <param name="pattern"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static int IndexOfBytes(byte[] array, byte[] pattern, int startIndex, int count)
		{
			if (array == null || array.Length == 0 || pattern == null || pattern.Length == 0 || count == 0)
			{
				return -1;
			}
			int i = startIndex;
			int endIndex = count > 0 ? Math.Min(startIndex + count, array.Length) : array.Length;
			int fidx = 0;
			int lastFidx = 0;

			while (i < endIndex)
			{
				lastFidx = fidx;
				fidx = (array[i] == pattern[fidx]) ? ++fidx : 0;
				if (fidx == pattern.Length)
				{
					return i - fidx + 1;
				}
				if (lastFidx > 0 && fidx == 0)
				{
					i = i - lastFidx;
					lastFidx = 0;
				}
				i++;
			}
			return -1;
		}
		#endregion
	}
}

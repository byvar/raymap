using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenSpace {
	public class PartialHttpStream : Stream, IDisposable {
		private SortedList<long, byte[]> caches;
		private const int CacheLen = 8192;

		// Cache for short requests.
		//private readonly byte[] cache;
		private int cacheLen;
		//private Stream stream;
		//private long position = 0;
		private long? length;
		//private long cachePosition;
		//private int cacheCount;
		private int lastRequestRead = 0;

		public PartialHttpStream(string url, int cacheLen = CacheLen, long? length = null) {
			if (string.IsNullOrEmpty(url))
				throw new ArgumentException("url empty");
			if (cacheLen <= 0)
				throw new ArgumentException("cacheLen must be greater than 0");

			Url = url;
			this.cacheLen = cacheLen;
			//cache = new byte[cacheLen];
			caches = new SortedList<long, byte[]>();
			this.length = length;
		}

		public string Url { get; private set; }

		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return false; } }
		public override bool CanSeek { get { return true; } }

		public override long Position { get; set; }

		/// <summary>
		/// Lazy initialized length of the resource.
		/// </summary>
		public override long Length {
			get {
				/*if (length == null)
					length = HttpGetLength();*/
				return length.Value;
			}
		}

		/// <summary>
		/// Count of HTTP requests. Just for statistics reasons.
		/// </summary>
		public int HttpRequestsCount { get; private set; }

		public override void SetLength(long value) { throw new NotImplementedException(); }
		public void SetCacheLength(int value) {
			cacheLen = value;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0 || offset >= buffer.Length)
				throw new ArgumentException(nameof(offset));
			if (count < 0 || offset + count > buffer.Length)
				throw new ArgumentException(nameof(count));
			if (count == 0) return 0;
			int numRead = 0;
			for (int i = 0; i < caches.Count; i++) {
				long cachePos = caches.Keys[i];
				if (Position >= cachePos && Position < cachePos + caches.Values[i].Length) {
					// position is in cache
					long startIndexInCache = Position - cachePos;
					long lengthInCache = Math.Min(caches.Values[i].Length - startIndexInCache, count);
					Array.Copy(caches.Values[i], startIndexInCache, buffer, offset, lengthInCache);
					Position += lengthInCache;
					offset += (int)lengthInCache;
					count -= (int)lengthInCache;
					numRead += (int)lengthInCache;
				}
				if (count <= 0) break;
			}
			return numRead;
		}

		/*public override int Read(byte[] buffer, int offset, int count) {
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0 || offset >= buffer.Length)
				throw new ArgumentException(nameof(offset));
			if (count < 0 || offset + count > buffer.Length)
				throw new ArgumentException(nameof(count));
			if (count == 0) return 0;

			// Try to read parts from cache
			long lastPosition = Position;
			int numRead = 0;
			int countInitial = count;
			SortedList<int, int> filledRanges = new SortedList<int, int>();
			for (int i = 0; i < caches.Count; i++) {
				long cachePos = caches.Keys[i];
				if (Position >= cachePos && Position < cachePos + caches.Values[i].Length) {
					// position is in cache
					long startIndexInCache = Position - cachePos;
					long lengthInCache = Math.Min(caches.Values[i].Length - startIndexInCache, count);
					Array.Copy(caches.Values[i], startIndexInCache, buffer, offset, lengthInCache);
					Position += lengthInCache;
					offset += (int)lengthInCache;
					count -= (int)lengthInCache;
					numRead += (int)lengthInCache;
				} else if (Position < cachePos && Position + count > cachePos) {
					long startIndexInBuffer = cachePos - Position;
					if (Position + count <= cachePos + caches.Values[i].Length) {
						// Cache contains end of buffer
						//long lengthInCache = caches.Values[i].Length - ((cachePos + caches.Values[i].Length) - (Position + count));
						long lengthInCache = Position + count - cachePos;
						Array.Copy(caches.Values[i], 0, buffer, offset + startIndexInBuffer, lengthInCache);
						count -= (int)lengthInCache;
						numRead += (int)lengthInCache;
					} else {
						// Cache contains just a part of the buffer
						Array.Copy(caches.Values[i], 0, buffer, offset + startIndexInBuffer, caches.Values[i].Length);
						numRead += caches.Values[i].Length;
						filledRanges.Add((int)startIndexInBuffer, caches.Values[i].Length);
					}
				}
				if (count <= 0) break;
			}
			if (count > 0 && Position < Length) {
				// Need to request from web
				SortedList<int, int> downloadRanges = new SortedList<int, int>(); // range start relative to Position, range length
				if (filledRanges.Count > 0) {
					downloadRanges.Add(0, filledRanges.First().Key);
					KeyValuePair<int, int> lastFilledRange = filledRanges.Last();
					if (lastFilledRange.Key + lastFilledRange.Value < count) {
						downloadRanges.Add(lastFilledRange.Key + lastFilledRange.Value, count);
					}
					if (filledRanges.Count > 1) {
						for (int i = 0; i < filledRanges.Count-1; i++) {
							if (filledRanges.Keys[i] + filledRanges.Values[i] < filledRanges.Keys[i + 1]) {
								downloadRanges.Add(filledRanges.Keys[i] + filledRanges.Values[i], filledRanges.Keys[i + 1]);
							}
						}
					}
				} else {
					downloadRanges.Add(0, count);
				}
				foreach (KeyValuePair<int, int> range in downloadRanges) {
					//UnityEngine.Debug.Log("Download range: " + range.Key + " - " + range.Value);
					if (range.Value > cacheLen) cacheLen = range.Value;
					long rangePos = Position + range.Key;
					IEnumerable<KeyValuePair<long, byte[]>> biggerCache = caches.Where(c => (c.Key >= rangePos + range.Value));
					IEnumerable<KeyValuePair<long, byte[]>> smallerCache = caches.Where(c => (c.Key + c.Value.Length <= rangePos));
					long newDataLength = Math.Min(cacheLen, Length - rangePos);
					long startPosition = rangePos;
					long addLengthBefore = 0;
					if (biggerCache.Count() > 0) {
						newDataLength = Math.Min(cacheLen, biggerCache.First().Key - rangePos);
					}
					long addLengthAfter = newDataLength - range.Value;
					if (newDataLength < cacheLen && rangePos > 0) {
						if (smallerCache.Count() > 0) {
							addLengthBefore = Math.Min(cacheLen, rangePos - (smallerCache.Last().Key + smallerCache.Last().Value.Length));
						} else {
							addLengthBefore = Math.Min(cacheLen, rangePos);
						}
						startPosition -= addLengthBefore;
						newDataLength += addLengthBefore;
					}
					//UnityEngine.Debug.Log(range.Key + " - " + range.Value + ": " + Length + " - " + rangePos + " - " + newDataLength);
					byte[] newData = new byte[newDataLength];
					int dataRead = HttpRead(newData, 0, (int)newDataLength, startPosition);
					Array.Resize(ref newData, dataRead);
					int numReadLocal = (int)Math.Min(range.Value, dataRead - addLengthBefore);
					Array.Copy(newData, addLengthBefore, buffer, offset + range.Key, numReadLocal);
					AddCache(startPosition, newData);
					numRead += numReadLocal;
				}
			}
			Position = lastPosition + numRead;
			return (int)(Position - lastPosition);
		}*/

		public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }

		public override long Seek(long pos, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.End:
					Position = Length + pos;
					break;

				case SeekOrigin.Begin:
					Position = pos;
					break;

				case SeekOrigin.Current:
					Position += pos;
					break;
			}
			return Position;
		}

		public override void Flush() {
		}

		/*private int ReadFromCache(byte[] buffer, ref int offset, ref int count) {
			if (cachePosition > Position || (cachePosition + cacheCount) <= Position)
				return 0; // cache miss
			int ccOffset = (int)(Position - cachePosition);
			int ccCount = Math.Min(cacheCount - ccOffset, count);
			Array.Copy(cache, ccOffset, buffer, offset, ccCount);
			offset += ccCount;
			count -= ccCount;
			return ccCount;
		}*/

		private void AddCache(long position, byte[] cache) {
			bool checkIfCanAddCache = true;
			while (checkIfCanAddCache) {
				checkIfCanAddCache = false;
				for(int i = 0; i < caches.Count; i++) {
					byte[] curCache = caches.Values[i];
					long curPos = caches.Keys[i];
					if (curPos == position + cache.Length) {
						int currentLength = cache.Length;
						Array.Resize(ref cache, cache.Length + curCache.Length);
						Array.Copy(curCache, 0, cache, currentLength, curCache.Length);
						caches.Remove(curPos);
						checkIfCanAddCache = true;
						break;
					}
				}
			}
			checkIfCanAddCache = true;
			while (checkIfCanAddCache) {
				checkIfCanAddCache = false;
				for (int i = 0; i < caches.Count; i++) {
					byte[] curCache = caches.Values[i];
					long curPos = caches.Keys[i];
					if (position == curPos + curCache.Length) {
						int currentLength = curCache.Length;
						Array.Resize(ref curCache, cache.Length + curCache.Length);
						Array.Copy(cache, 0, curCache, currentLength, cache.Length);
						cache = curCache;
						position = curPos;
						caches.Remove(curPos);
						checkIfCanAddCache = true;
						break;
					}
				}
			}
			caches.Add(position, cache);
		}

		public async UniTask FillCacheForRead(int count) {
			if (count <= 0) return;
			await MapLoader.WaitIfNecessary();

			// Try to read parts from cache
			long lastPosition = Position;
			int numRead = 0;
			int countInitial = count;
			SortedList<int, int> filledRanges = new SortedList<int, int>();
			/*IEnumerable<KeyValuePair<long, byte[]>> biggerCache = caches.Where((pair) => (pair.Key >= Position + count));
			IEnumerable<KeyValuePair<long, byte[]>> smallerCache = caches.Where((pair) => (pair.Key + pair.Value.Length <= Position));
			int cacheStart = 0, cacheEnd = 0;
			if (smallerCache.Count() > 0) cacheStart = smallerCache.Count();
			if (biggerCache.Count() > 0) cacheEnd = caches.IndexOfKey(biggerCache.First().Key) - 1;*/
			for (int i = 0; i < caches.Count; i++) {
				long cachePos = caches.Keys[i];
				if (Position >= cachePos && Position < cachePos + caches.Values[i].Length) {
					// position is in cache
					long startIndexInCache = Position - cachePos;
					long lengthInCache = Math.Min(caches.Values[i].Length - startIndexInCache, count);
					//Array.Copy(caches.Values[i], startIndexInCache, buffer, offset, lengthInCache);
					Position += lengthInCache;
					//offset += (int)lengthInCache;
					count -= (int)lengthInCache;
					numRead += (int)lengthInCache;
				} else if (Position < cachePos && Position + count > cachePos) {
					long startIndexInBuffer = cachePos - Position;
					if (Position + count <= cachePos + caches.Values[i].Length) {
						// Cache contains end of buffer
						//long lengthInCache = caches.Values[i].Length - ((cachePos + caches.Values[i].Length) - (Position + count));
						long lengthInCache = Position + count - cachePos;
						//Array.Copy(caches.Values[i], 0, buffer, offset + startIndexInBuffer, lengthInCache);
						count -= (int)lengthInCache;
						numRead += (int)lengthInCache;
					} else {
						// Cache contains just a part of the buffer
						//Array.Copy(caches.Values[i], 0, buffer, offset + startIndexInBuffer, caches.Values[i].Length);
						numRead += caches.Values[i].Length;
						filledRanges.Add((int)startIndexInBuffer, caches.Values[i].Length);
					}
				}
				if (count <= 0) break;
			}
			if (count > 0 && Position < Length) {
				// Need to request from web
				SortedList<int, int> downloadRanges = new SortedList<int, int>(); // range start relative to Position, range length
				if (filledRanges.Count > 0) {
					downloadRanges.Add(0, filledRanges.First().Key);
					KeyValuePair<int, int> lastFilledRange = filledRanges.Last();
					if (lastFilledRange.Key + lastFilledRange.Value < count) {
						downloadRanges.Add(lastFilledRange.Key + lastFilledRange.Value, count);
					}
					if (filledRanges.Count > 1) {
						for (int i = 0; i < filledRanges.Count - 1; i++) {
							if (filledRanges.Keys[i] + filledRanges.Values[i] < filledRanges.Keys[i + 1]) {
								downloadRanges.Add(filledRanges.Keys[i] + filledRanges.Values[i], filledRanges.Keys[i + 1]);
							}
						}
					}
				} else {
					downloadRanges.Add(0, count);
				}
				foreach (KeyValuePair<int, int> range in downloadRanges) {
					//UnityEngine.Debug.Log("Download range: " + range.Key + " - " + range.Value);
					if (range.Value > cacheLen) cacheLen = range.Value;
					long rangePos = Position + range.Key;
					IEnumerable<KeyValuePair<long, byte[]>> biggerCache = caches.Where(c => (c.Key >= rangePos + range.Value));
					IEnumerable<KeyValuePair<long, byte[]>> smallerCache = caches.Where(c => (c.Key + c.Value.Length <= rangePos));
					long newDataLength = Math.Min(cacheLen, Length - rangePos);
					long startPosition = rangePos;
					long addLengthBefore = 0;
					if (biggerCache.Count() > 0) {
						newDataLength = Math.Min(cacheLen, biggerCache.First().Key - rangePos);
					}
					long addLengthAfter = newDataLength - range.Value;
					if (newDataLength < cacheLen && rangePos > 0) {
						if (smallerCache.Count() > 0) {
							addLengthBefore = Math.Min(cacheLen, rangePos - (smallerCache.Last().Key + smallerCache.Last().Value.Length));
						} else {
							addLengthBefore = Math.Min(cacheLen, rangePos);
						}
						startPosition -= addLengthBefore;
						newDataLength += addLengthBefore;
					}
					//UnityEngine.Debug.Log(range.Key + " - " + range.Value + ": " + Length + " - " + rangePos + " - " + newDataLength);
					byte[] newData = new byte[newDataLength];
					await HttpRead(newData, 0, (int)newDataLength, startPosition);
					int dataRead = lastRequestRead;
					Array.Resize(ref newData, dataRead);
					int numReadLocal = (int)Math.Min(range.Value, dataRead - addLengthBefore);
					//Array.Copy(newData, addLengthBefore, buffer, offset + range.Key, numReadLocal);
					AddCache(startPosition, newData);
					numRead += numReadLocal;
					/*UnityEngine.Debug.Log("Caches: " + caches.Count + " - Count: " + numRead + "/" + countInitial);
					for (int i = 0; i < caches.Count; i++) {
						UnityEngine.Debug.Log("Cache " + i + " - " + caches.Keys[i] + " - " + caches.Values[i].Length);
					}*/
				}
			}
			Position = lastPosition;
			/*Position = lastPosition + numRead;
			return (int)(Position - lastPosition);*/
		}

		private async UniTask HttpRead(byte[] buffer, int offset, int count, long startPosition) {
			HttpRequestsCount++;
			UnityWebRequest www = UnityWebRequest.Get(Url);
			string state = MapLoader.Loader.loadingState;
			int totalSize = caches.Sum(c => c.Value.Length);
			MapLoader.Loader.loadingState = state + "\nDownloading part of bigfile: " + Url.Replace(FileSystem.serverAddress, "") + " (New size: " + Util.SizeSuffix(totalSize + count,0) + "/" + Util.SizeSuffix(Length, 0) + ")";
			UnityEngine.Debug.Log("Requesting range: " + string.Format("bytes={0}-{1}", startPosition, startPosition + count - 1) + " - " + Url);
			www.SetRequestHeader("Range", string.Format("bytes={0}-{1}", startPosition, startPosition + count - 1));
			await www.SendWebRequest();
			while (!www.isDone) {
				await UniTask.WaitForEndOfFrame();
			}
			if (!www.isHttpError && !www.isNetworkError) {
				byte[] data = www.downloadHandler.data;
				int nread = Math.Min(data.Length, count);
				Array.Copy(data, 0, buffer, offset, nread);
				lastRequestRead = nread;
			} else {
				lastRequestRead = 0;
			}

			MapLoader.Loader.loadingState = state;
			/*using (BinaryReader sr = new BinaryReader(httpResponse.GetResponseStream(), Encoding.GetEncoding(httpResponse.CharacterSet))) {
				sr.ReadBlock(buffer, offset, count);
			}*/
			/*using (Stream stream = response.GetResponseStream())
				nread = stream.Read(buffer, offset, count);*/
		}

		/*private long HttpGetLength() {
			HttpRequestsCount++;
			UnityWebRequest www = UnityWebRequest.Head(Url);
			IEnumerator e = FileSystem.HandleRequest(www);
			while (e.MoveNext()) ;
			if (!www.isHttpError && !www.isNetworkError) {
				long contentLength;
				if (long.TryParse(www.GetResponseHeader("Content-Length"), out contentLength)) {
					return contentLength;
				}
			}
			return 0;
		}*/

		private new void Dispose() {
			base.Dispose();
			/*if (stream != null) {
				stream.Dispose();
				stream = null;
			}*/
		}
	}
}

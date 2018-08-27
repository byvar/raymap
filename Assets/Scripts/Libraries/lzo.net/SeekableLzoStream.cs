//
// Copyright (c) 2017, Bianco Veigel
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace lzo.net
{
    /// <summary>
    /// Seekable wrapper for lzo decompression 
    /// </summary>
    /// <remarks>
    /// Forward seeking is implemented by decoding and discarding data until target is readched.
    /// Backward seeking is added by taking snapshots of internal state in regular intervals.
    /// Each snapshot uses roughly 48K of memory. The snapshot interval can be specified.
    /// While seeking, the last snapshot before the desired position is restored and decompression
    /// is resumed from this state until the position is reached
    /// </remarks>
    public class SeekableLzoStream:LzoStream
    {
        struct Snapshot
        {
            public readonly long OutputPosition;
            public readonly long InputPosition;
            public readonly int Instruction;
            public readonly LzoState State;

            private readonly RingBuffer _ringBuffer;

            public RingBuffer RingBuffer {
                get { return _ringBuffer.Clone(); }
            }

            public Snapshot(long outputPosition, long inputPosition, RingBuffer ringBuffer, int instruction, LzoState state)
            {
                _ringBuffer = ringBuffer.Clone();
                Instruction = instruction;
                State = state;
                InputPosition = inputPosition;
                OutputPosition = outputPosition;
            }
        }

        private readonly int _snapshotInterval;
        private readonly Stack<Snapshot> _snapshots;

        /// <summary>
        /// creates a new seekable lzo stream for decompression
        /// </summary>
        /// <param name="stream">the compressed stream</param>
        /// <param name="mode">currently only decompression is supported</param>
        public SeekableLzoStream(Stream stream, CompressionMode mode)
            :this(stream, mode, false) { }

        /// <summary>
        /// creates a new seekable lzo stream for decompression
        /// </summary>
        /// <param name="stream">the compressed stream</param>
        /// <param name="mode">currently only decompression is supported</param>
        /// <param name="leaveOpen">true to leave the stream open after disposing the LzoStream object; otherwise, false</param>
        public SeekableLzoStream(Stream stream, CompressionMode mode, bool leaveOpen)
            :this(stream, mode, leaveOpen, 10*MaxWindowSize) { }

        /// <summary>
        /// creates a new seekable lzo stream for decompression
        /// </summary>
        /// <param name="stream">the compressed stream</param>
        /// <param name="mode">currently only decompression is supported</param>
        /// <param name="leaveOpen">true to leave the stream open after disposing the LzoStream object; otherwise, false</param>
        /// <param name="snapshotInterval">specifies the interval for creating snapshots of internal state.</param>
        public SeekableLzoStream(Stream stream, CompressionMode mode, bool leaveOpen, int snapshotInterval)
            :base(stream, mode, leaveOpen)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("stream must be seekable", stream.ToString());

            _snapshotInterval = snapshotInterval;
            _snapshots = new Stack<Snapshot>();
            TakeSnapshot();
        }

        protected override int Decode(byte[] buffer, int offset, int count)
        {
            TakeSnapshot();
            return base.Decode(buffer, offset, count);
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var position = OutputPosition;
            var targetPosition = offset;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    break;
                case SeekOrigin.Current:
                    targetPosition += position;
                    break;
                case SeekOrigin.End:
                    targetPosition = Length + targetPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(origin.ToString(), origin, null);
            }
            if (targetPosition == position) return position;
            if (targetPosition < position)
            {
                Snapshot snapshot;
                while ((snapshot = _snapshots.Peek()).OutputPosition > targetPosition)
                {
                    _snapshots.Pop();
                }

                OutputPosition = position = snapshot.OutputPosition;
                RingBuffer = snapshot.RingBuffer;
                Instruction = snapshot.Instruction;
                State = snapshot.State;
                DecodedBuffer = null;
                Source.Seek(snapshot.InputPosition, SeekOrigin.Begin);
            }
            if (targetPosition > position)
            {
                var total = targetPosition - position;
                var buffer = new byte[1024];
                var count = 1024;
                do
                {
                    if (total < count)
                        count = (int)total;
                    total -= Read(buffer, 0, count);
                } while (total > 0);
            }
            return Position;
        }

        private void TakeSnapshot()
        {
            if (_snapshots.Count > 0 && (_snapshots.Peek().InputPosition + _snapshotInterval) > Source.Position)
                return;
            _snapshots.Push(new Snapshot(Position, Source.Position, RingBuffer, Instruction, State));
        }
    }
}

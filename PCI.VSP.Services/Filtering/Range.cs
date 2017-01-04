using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Filtering
{
    interface IRange<T> where T : IComparable<T>
    {
        T Start { get; }
        T End { get; }
        bool Contains(T valueToFind);
        Int32 CompareTo(IRange<T> other);
        Int32 CompareTo(T other);
    }

    class Range<T> : IRange<T> where T : IComparable<T>
    {
        private readonly T _start;
        private readonly T _end;

        public Range(T start, T end)
        {
            if (start.CompareTo(end) <= 0)
            {
                this._start = start;
                this._end = end;
            }
            else
            {
                this._start = end;
                this._end = start;
            }
        }

        public T Start { get { return this._start; } }
        public T End { get { return this._end; } }

        public bool Contains(T valueToFind)
        {
            return valueToFind.CompareTo(Start) >= 0 && valueToFind.CompareTo(End) <= 0;
        }

        public Int32 CompareTo(IRange<T> range)
        {
            if ((this.Start.CompareTo(range.Start) < 0 && this.End.CompareTo(range.End) > 0) ||
                (this.Start.CompareTo(range.Start) > 0 && this.End.CompareTo(range.End) > 0))
                return 1;
            if ((this.Start.CompareTo(range.Start) > 0 && this.End.CompareTo(range.End) < 0) ||
                (this.Start.CompareTo(range.Start) < 0 && this.End.CompareTo(range.End) < 0))
                return -1;
            else
                return 0; // ranges overlap; we consider this a match
        }

        public Int32 CompareTo(T other)
        {
            if (this.Start.CompareTo(other) <= 0 && this.End.CompareTo(other) >= 0)
                return 0;
            if (this.Start.CompareTo(other) < 0 && this.End.CompareTo(other) < 0)
                return -1;
            if (this.Start.CompareTo(other) > 0 && this.End.CompareTo(other) > 0)
                return 1;
            throw new InvalidCompareResultException();
        }
    }

    class InvalidCompareResultException : InvalidOperationException
    {

    }
}

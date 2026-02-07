using System;
using UnityEngine;

namespace Whaledevelop.UI
{
    public readonly struct ViewId : IEquatable<ViewId>
    {
        public string Value { get; }
        public int Hash { get; }

        public ViewId(string value)
        {
            Value = value;
            Hash = Animator.StringToHash(value);
        }

        public static ViewId From(string value)
        {
            
            return new ViewId(value);
        }

        public static implicit operator ViewId(string value)
        {
            
            return new ViewId(value);
        }

        public bool Equals(ViewId other)
        {
            
            return Hash == other.Hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ViewId other)
            {
                
                return Equals(other);
            }

            
            return false;
        }

        public override int GetHashCode()
        {
            
            return Hash;
        }

        public override string ToString()
        {
            
            return Value;
        }

        public static bool operator ==(ViewId left, ViewId right)
        {
            
            return left.Equals(right);
        }

        public static bool operator !=(ViewId left, ViewId right)
        {
            
            return !left.Equals(right);
        }
    }
}
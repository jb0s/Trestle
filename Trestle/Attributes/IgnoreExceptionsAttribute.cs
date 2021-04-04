using System;
using Org.BouncyCastle.Asn1;

namespace Trestle.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreExceptionsAttribute : Attribute
    {
    }
}
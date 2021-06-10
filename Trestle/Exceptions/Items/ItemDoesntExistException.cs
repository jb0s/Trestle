using System;

namespace Trestle.Exceptions.Items
{
    public class ItemDoesntExistException : Exception
    {
        public override string Message => "The item associated with this material does not exist.";
    }
}
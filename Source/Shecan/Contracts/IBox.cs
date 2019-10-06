using System;

namespace Shecan.Contracts
{
    public interface IBox
    {
        Action CloseCallback { get; set; }
        Action CompleteCallback { get; set; }
    }
}
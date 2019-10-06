using System;

namespace ShecanDesktop.Contracts
{
    public interface IBox
    {
        Action CloseCallback { get; set; }
        Action CompleteCallback { get; set; }
    }
}
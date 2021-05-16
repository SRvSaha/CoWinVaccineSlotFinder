using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators
{
    public interface IValidator<T>
    {
        bool IsValid(T value);
    }
}

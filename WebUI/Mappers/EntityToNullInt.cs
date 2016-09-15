using System;
using ProcessAccelerator.Core.Model;
using Omu.ValueInjecter;

namespace ProcessAccelerator.WebUI.Mappers
{
    public class EntityToNullInt : LoopValueInjection
    {
        protected override bool TypesMatch(Type sourceType, Type targetType)
        {
            return sourceType.IsSubclassOf(typeof(DelEntity)) && targetType == typeof(int?);
        }

        protected override object SetValue(object o)
        {
            if (o == null) return null;
            return (o as DelEntity).ID;
        }
    }
}
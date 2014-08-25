using System;

namespace IWantTo.Client.Core.DataStorage.Contract
{
    public interface IBusinessEntity : IEquatable<IBusinessEntity>
    {
        long Id { get; set; }
    }
}
